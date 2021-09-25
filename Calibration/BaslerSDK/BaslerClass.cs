using System;
using System.Collections.Generic;
using HalconDotNet;
using Basler.Pylon;
using System.Runtime.InteropServices;

namespace Calibration.BaslerSDK
{
	public class BaslerClass
	{
		List<ICameraInfo> allCameras = null;//ICameraInfo对象的列表，用于保存遍历到的所有相机信息
		Camera myCamera = null;//相机对象
		HImage image = null;

		//构造函数
		public BaslerClass()
		{
		}

		/// <summary>
		/// 连接相机 返回-1为失败，0为成功
		/// </summary>
		/// <param name="username"></param>
		/// <returns></returns>
		public int connectCamera(string username)
		{
			allCameras = CameraFinder.Enumerate();//获取所有相机设备
			for (int i = 0; i < allCameras.Count; i++)
			{
				try
				{
					if (allCameras[i][CameraInfoKey.UserDefinedName] == username)
					{
						//如果当前相机信息中序列号是指定的序列号，则实例化相机类
						myCamera = new Camera(allCameras[i]);
						myCamera.Open();//打开相机
						return 0;
					}
					continue;
				}
				catch
				{
					return -1;
				}
			}
			return -1;
		}

		public int startCamera()//相机开始采集，返回-1为失败，0为成功
		{
			try
			{
				myCamera.StreamGrabber.Start();
			}
			catch
			{
				return -1;
			}
			return 0;
		}
		public int stopCamera()//停止相机采集，返回-1为失败，1为成功
		{
			try
			{
				myCamera.StreamGrabber.Stop();
			}
			catch
			{
				return -1;
			}
			return 0;
		}

		public int closeCamera()//关闭相机，返回-1为失败，0为成功
		{
			try
			{
				myCamera.Close();
			}
			catch
			{
				return -1;
			}
			return 0;
		}

		public int softTrigger()//发送软触发命令
		{
			try
			{
				myCamera.ExecuteSoftwareTrigger();
			}
			catch
			{
				return -1;
			}
			return 0;
		}

		public HImage ReadBuffer()//读取相机buffer并转换成Halcon HImage格式的图像
		{
			if (myCamera == null)
			{
				return null;
			}
			IGrabResult grabResult = myCamera.StreamGrabber.RetrieveResult(4000, TimeoutHandling.ThrowException);//读取buffer，超时时间为4000ms
			image = new HImage();
			using (grabResult)
			{
				if (grabResult.GrabSucceeded)
				{
					if (IsMonoData(grabResult))
					{
						//如果是黑白图像，则利用GenImage1算子生成黑白图像
						byte[] buffer = grabResult.PixelData as byte[];
						IntPtr p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
						image.GenImage1("byte", grabResult.Width, grabResult.Height, p);
					}
					else
					{
						//if (grabResult.PixelTypeValue != PixelType.RGB8packed)
						//{
						//如果图像不是RGB8格式，则将图像转换为RGB8，然后生成彩色图像
						//因为GenImageInterleaved算子能够生成的图像的数据，常见的格式只有RGB8
						//如果采集的图像是RGB8则不需转换，具体生成图像方法请自行测试编写。
						byte[] buffer_rgb = new byte[grabResult.Width * grabResult.Height * 3];
						Basler.Pylon.PixelDataConverter convert = new PixelDataConverter();
						convert.Parameters[PLPixelDataConverter.InconvertibleEdgeHandling].SetValue("Clip");
						convert.OutputPixelFormat = PixelType.RGB8packed;
						convert.Convert(buffer_rgb, grabResult);
						IntPtr p = Marshal.UnsafeAddrOfPinnedArrayElement(buffer_rgb, 0);
						image.GenImageInterleaved(p, "rgb", grabResult.Width, grabResult.Height, 0, "byte", grabResult.Width, grabResult.Height, 0, 0, -1, 0);
					}
					return image;
				}
				else
				{
					return null;
				}
			}
		}

		public int setExposureTime(long ExposureTimeNum)//设置曝光时间us
		{
			try
			{
				myCamera.Parameters[PLCamera.ExposureTimeAbs].SetValue(ExposureTimeNum);
			}
			catch
			{
				return -1;
			}
			return 0;
		}
		public int pixelFormat(uint pixelType)//设置图像格式，黑白相机的只能设置成黑白图像
		{
			//1：Mono8 2：彩色YUV422
			try
			{
				if (pixelType == 1)
				{
					myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono8);
				}
				else if (pixelType == 2)
				{
					myCamera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.YUV422Packed);
				}
			}
			catch
			{
				return -1;
			}
			return 0;
		}

		public int setHeight(long height)//设置图像高度，数值不要超过相机的分辨率
		{
			try
			{
				if (myCamera.Parameters[PLCamera.Height].TrySetValue(height))
					return 0;
				else
					return -1;
			}
			catch
			{
				return -1;
			}
		}
		public int setWidth(long width)//设置图像宽度，数值不要超过相机的分辨率
		{
			try
			{
				if (myCamera.Parameters[PLCamera.Width].TrySetValue(width))
					return 0;
				else
					return -1;
			}
			catch
			{
				return -1;
			}
		}
		public int setOffsetX(long offsetX)//设置图像水平偏移
		{
			try
			{
				if (myCamera.Parameters[PLCamera.OffsetX].TrySetValue(offsetX))
					return 0;
				else
					return -1;
			}
			catch
			{
				return -1;
			}
		}
		public int setOffsetY(long offsetY)//设置图像竖直偏移
		{
			try
			{
				if (myCamera.Parameters[PLCamera.OffsetY].TrySetValue(offsetY))
					return 0;
				else
					return -1;
			}
			catch
			{
				return -1;
			}
		}
		public int setTriggerMode(uint TriggerModeNum)//设置触发模式开关
		{
			//1:为On 触发模式
			//0:Off 连续模式
			try
			{
				if (myCamera.Parameters[PLCamera.TriggerMode].TrySetValue(TriggerModeNum == 1 ? "On" : "Off"))
					return 0;
				else
					return -1;
			}
			catch
			{
				return -1;
			}
		}

		public int closeBalanceAuto()//关闭自动白平衡
		{
			try
			{
				myCamera.Parameters[PLCamera.BalanceWhiteAuto].TrySetValue("Off");
			}
			catch
			{
				return -1;
			}
			return 0;
		}
		public int setSoftTrigger()//设置软触发
		{
			try
			{
				if (myCamera.Parameters[PLCamera.TriggerSource].TrySetValue("Software"))//设置为软触发
					return 0;
				else
					return -1;
			}
			catch
			{
				return -1;
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

		public void DestroyCamera()
		{
			// Disable all parameter controls.
			if (myCamera != null)
			{
				myCamera.Close();
				myCamera.Dispose();
				myCamera = null;
			}
		}
	}
}
