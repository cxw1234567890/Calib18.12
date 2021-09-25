using Basler.Pylon;
using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using AlarmLibrary;

namespace Calibration.BaslerSDK
{
    public class BaslerCamera
    {
        private PixelDataConverter converter = null;
        private IntPtr latestFrameAddress = IntPtr.Zero;
        private Camera camera = null;
        public string UserName;
        /// <summary>
        /// if >= Sfnc2_0_0,说明是us的相机
        /// </summary>
        private Version Sfnc2_0_0 = new Version(2, 0, 0);

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                try
                {
                    if (camera != null)
                    {
                        return camera.IsConnected;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    NotifyG.Add(ex.ToString());
                    return false;
                }
            }
        }

        /// <summary>
        /// 打开相机
        /// </summary>
        public bool Open()
        {
            try
            {
                if (camera != null && camera.IsConnected) { return true; }
                if (string.IsNullOrEmpty(UserName)) { return false; }
                // 枚举相机列表
                List<ICameraInfo> allCameraInfos = null;
                try
                {
                    allCameraInfos = CameraFinder.Enumerate();
                }
                catch
                {
                    return false;
                }
                foreach (ICameraInfo cameraInfo in allCameraInfos)
                {
                    if (UserName == cameraInfo[CameraInfoKey.UserDefinedName])
                    {
                        if (camera != null)
                        {
                            //camera.CameraOpened -= Configuration.AcquireContinuous;
                            camera.StreamGrabber.ImageGrabbed -= StreamGrabber_ImageGrabbed;
                            camera.Close();
                        }
                        camera = new Camera(cameraInfo);
                        //camera.CameraOpened += Configuration.AcquireContinuous;
                        camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
                        latestFrameAddress = IntPtr.Zero;
                        converter = new PixelDataConverter();
                        break;
                    }
                }
                if (camera == null) { return false; }
                if (camera.Open().IsOpen)
                {
                    //设置内存中接收图像缓冲区大小
                    camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(10);
                    SetHeartbeatTimeout("5000");
                    SetSoftwareTrigger();
                }
                return camera.IsConnected;
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 单张采集
        /// </summary>
        /// <returns></returns>
        public bool GrabImage()
        {
            try
            {
                if (camera.StreamGrabber.IsGrabbing) { camera.StreamGrabber.Stop(); }
                camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                camera.StreamGrabber.Start(1, GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                if (camera.WaitForFrameTriggerReady(1000, TimeoutHandling.ThrowException))
                {
                   camera.ExecuteSoftwareTrigger();
                }
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 连续取图
        /// </summary>
        public bool ContinuousGrabImage()
        {
            if (camera == null) { return false; }
            try
            {
                SetFreerun();
                if (camera.StreamGrabber.IsGrabbing)
                {
                    return false;
                }
                {
                    camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                    camera.StreamGrabber.Start(GrabStrategy.LatestImages, GrabLoop.ProvidedByStreamGrabber);
                }
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
                return false;
            }
            return true;
        }

        /// <summary>
        /// 停止取图
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (camera == null) {  return false; }
            try
            {
                camera.StreamGrabber.Stop();             
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
                return false;
            }
            SetSoftwareTrigger();
            return true;
        }

        /// <summary>
        /// 设置增益
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetGain(string value = "")
        {
            if (camera == null) {  return false; }
            if (!camera.IsOpen) {  return false; }
            try
            {
                camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off);
                if (!string.IsNullOrEmpty(value))
                {
                    var dValue = Convert.ToDouble(value);
                    camera.Parameters[PLCamera.Gain].TrySetValue(dValue);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置曝光
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetExposureTime(string value = "")
        {
            if (camera == null) {  return false; }
            if (!camera.IsOpen) { return false; }
            try
            {
                camera.Parameters[PLCamera.ExposureAuto].TrySetValue(PLCamera.ExposureAuto.Off);
                camera.Parameters[PLCamera.ExposureMode].TrySetValue(PLCamera.ExposureMode.Timed);
                if (!string.IsNullOrEmpty(value))
                {
                    var dValue = Convert.ToDouble(value);
                    camera.Parameters[PLCamera.ExposureTimeAbs].TrySetValue(dValue);
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取曝光
        /// </summary>
        /// <returns></returns>
        public double GetExposureTime()
        {
            double result = 0;
            if (camera == null)   return result; 
            if (!camera.IsOpen)  return result; 
            try
            {
                result = camera.Parameters[PLCamera.ExposureTimeAbs].GetValue();
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        /// <summary>
        /// 设置心跳
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool SetHeartbeatTimeout(string value = "")
        {
            if (camera == null) { return false; }
            if (!camera.IsOpen) { return false; }
            try
            {
                // 判断是否是网口相机，网口相机才有心跳时间
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        var dValue = Convert.ToInt64(value);
                        camera.Parameters[PLCamera.GevHeartbeatTimeout].TrySetValue(dValue);
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 关闭相机,释放相关资源
        /// </summary>
        public bool Close()
        {
            try
            {
                if (camera != null)
                {
                    camera.Close();
                    camera.Dispose();
                    camera = null;
                }
                if (converter != null)
                {
                    converter.Dispose();
                    converter = null;
                }
                if (latestFrameAddress != null)
                {
                    Marshal.FreeHGlobal(latestFrameAddress);
                    latestFrameAddress = IntPtr.Zero;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 设置相机Freerun模式
        /// </summary>
        public void SetFreerun()
        {
            try
            {
                // Set an enum parameter.
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                    }
                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 设置相机软触发模式
        /// </summary>
        public void SetSoftwareTrigger()
        {
            try
            {
                // Set an enum parameter.
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                    }
                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Software);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 设置相机外触发模式
        /// </summary>
        public void SetExternTrigger()
        {
            try
            {
                if (camera.GetSfncVersion() < Sfnc2_0_0)
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Line1);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.AcquisitionStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Line1);
                        }
                    }

                    //Sets the trigger delay time in microseconds.
                    camera.Parameters[PLCamera.TriggerDelayAbs].SetValue(0);        // 设置触发延时

                    //Sets the absolute value of the selected line debouncer time in microseconds
                    camera.Parameters[PLCamera.LineSelector].TrySetValue(PLCamera.LineSelector.Line1);
                    camera.Parameters[PLCamera.LineMode].TrySetValue(PLCamera.LineMode.Input);
                    camera.Parameters[PLCamera.LineDebouncerTimeAbs].SetValue(0);       // 设置去抖延时，过滤触发信号干扰

                }
                else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
                {
                    if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart))
                    {
                        if (camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart))
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.Off);

                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Line1);
                        }
                        else
                        {
                            camera.Parameters[PLCamera.TriggerSelector].TrySetValue(PLCamera.TriggerSelector.FrameBurstStart);
                            camera.Parameters[PLCamera.TriggerMode].TrySetValue(PLCamera.TriggerMode.On);
                            camera.Parameters[PLCamera.TriggerSource].TrySetValue(PLCamera.TriggerSource.Line1);
                        }
                    }

                    //Sets the trigger delay time in microseconds.//float
                    camera.Parameters[PLCamera.TriggerDelay].SetValue(0);       // 设置触发延时

                    //Sets the absolute value of the selected line debouncer time in microseconds
                    camera.Parameters[PLCamera.LineSelector].TrySetValue(PLCamera.LineSelector.Line1);
                    camera.Parameters[PLCamera.LineMode].TrySetValue(PLCamera.LineMode.Input);
                    camera.Parameters[PLCamera.LineDebouncerTime].SetValue(0);       // 设置去抖延时，过滤触发信号干扰

                }
            }
            catch (Exception ex)
            {
            }
        }

        #region event
        public event Action<HImage> EventGrab;
        private void OnGrab(HImage image)
        {
            EventGrab?.Invoke(image);
        }
        public void Clear_Grabevent()
        {
            try
            {
                if (EventGrab != null)
                {
                    Delegate[] dels = EventGrab.GetInvocationList();
                    foreach (Delegate d in dels)
                    {
                        EventGrab -= d as Action<HImage>;
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
            }
        }
        #endregion

        #region private
        //取图中
        private void StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e.GrabResult;
                HImage image = new HImage();
                image.Dispose();
                using (grabResult)
                {
                    if (grabResult.GrabSucceeded)
                    {
                        if (IsMonoData(grabResult)) //黑白图片
                        {
                            converter.OutputPixelFormat = PixelType.Mono8;
                            if (latestFrameAddress == IntPtr.Zero)
                            {
                                latestFrameAddress = Marshal.AllocHGlobal((Int32)grabResult.PayloadSize);
                            }
                            converter.Convert(latestFrameAddress, grabResult.PayloadSize, grabResult);
                            image.GenImage1("byte", grabResult.Width, grabResult.Height, latestFrameAddress);
                        }
                        else //彩色图片
                        {
                            int imageWidth = grabResult.Width - 1;
                            int imageHeight = grabResult.Height - 1;
                            int payloadSize = imageWidth * imageHeight;
                            if (latestFrameAddress == IntPtr.Zero)
                            {
                                latestFrameAddress = Marshal.AllocHGlobal(3 * payloadSize);
                            }
                            converter.OutputPixelFormat = PixelType.RGB8packed;
                            converter.Parameters[PLPixelDataConverter.InconvertibleEdgeHandling].SetValue("Clip");
                            converter.Convert(latestFrameAddress, 3 * payloadSize, grabResult);

                            image.GenImageInterleaved(latestFrameAddress, "rgb",
                                     imageWidth, imageHeight, -1, "byte", imageWidth, imageHeight, 0, 0, -1, 0);
                        }
                        OnGrab(image);
                    }
                }
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
            }
        }

        private Boolean IsMonoData(IGrabResult iGrabResult)//判断图像是否为黑白格式
        {
            switch (iGrabResult.PixelTypeValue)
            {
                case PixelType.Mono1packed:
                case PixelType.Mono2packed:
                case PixelType.Mono4packed:
                case PixelType.Mono8:
                case PixelType.Mono8signed:
                case PixelType.Mono10:
                case PixelType.Mono10p:
                case PixelType.Mono10packed:
                case PixelType.Mono12:
                case PixelType.Mono12p:
                case PixelType.Mono12packed:
                case PixelType.Mono16:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 掉线重连回调函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnConnectionLost(Object sender, EventArgs e)
        {
            try
            {
                const int cTimeOutMs = 20;
                System.Threading.Thread.Sleep(100);
                camera.Close();
                for (int i = 0; i < 1000; i++)
                {
                    try
                    {
                        camera.Open(cTimeOutMs, TimeoutHandling.ThrowException);
                        if (camera.IsOpen)
                        {
                            Open();
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
            }
        }
        #endregion
    }
}
