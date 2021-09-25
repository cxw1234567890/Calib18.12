using Calibration.BaslerSDK;
using AlarmLibrary;
using HalconDotNet;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Calibration
{
    public partial class Form3 : DockContent
    {
        DataClass DC = DataClass.Instance;
        WindowDisplayCtl hWindow;
        BaslerCamera Basler;
        bool Stop;
        HImage _image;
        EventWaitHandle _waitHandle = new AutoResetEvent(false);
        public Form3()
        {
            InitializeComponent();
            hWindow = new WindowDisplayCtl();
            hWindow.Dock = DockStyle.Fill;
            panel1.Controls.Add(hWindow);
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            Basler = DC.BaslerList.Find(p => p.UserName == "50502696");
            if (Basler == null)
            {
                Basler = new BaslerCamera();
                Basler.UserName = "50502696";
                DC.BaslerList.Add(Basler);
            }
        }
        private void Show(HImage Image)
        {
            _image = Image;
            _waitHandle.Set();
            if (hWindow.IsHandleCreated)
            {
                hWindow.BeginInvoke(
                new MethodInvoker(() => hWindow.ShowImage(Image))
                                               );
            }
        }

        private bool GrabImageWait()
        {
            bool result = false;
            //等待时间根据相机的帧率设定
            if (Basler.GrabImage() && _waitHandle.WaitOne(95))
            {               
                result = true;
            }
            return result;
        }

        /// <summary>
        /// 开始建工具坐标系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartFun_Click(object sender, EventArgs e)
        {
            if (Basler.Connected)
            {
                Hwind_Activated();
                Stop = false;
                Task T = new Task(Fun);
                T.Start();
            }
            else
            {
                MessageBox.Show("请打开相机");
            }
        }
        /// <summary>
        /// 停止建工具坐标系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopFun_Click(object sender, EventArgs e)
        {
            Stop = true;
        }
        
        /// <summary>
        /// 建坐标系的方法
        /// </summary>
        /// <param name="o"></param>
        private void Fun()
        {
            double CirRow, CirColumn;
            int ImageWidth = 1920, ImageHeight = 1080;
            while (!Stop)
            {
                try
                {
                    if (!GrabImageWait()) return;
                    if (!GetCenterPoint(_image, hWindow, out CirRow, out CirColumn)) break;//求圆心坐标
                    string Message = "R:" + (CirRow - ImageHeight / 2 - 1).ToString("f1")
                                            + "  " + "C:" + (CirColumn - ImageWidth / 2 - 1).ToString("f1");

                    hWindow.ShowMessage(25, "微软雅黑", Message, "image", 1, 1, "magenta", "false");

                    Thread.Sleep(1);
                }

                catch (Exception ex)
                {
                    NotifyG.Add(ex.ToString());
                }
            }
        }
        /// <summary>
        /// 求标定板的圆心
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="CirRow"></param>
        /// <param name="CirColumn"></param>
        /// <returns></returns>
        private bool GetCenterPoint(HImage Image, WindowDisplayCtl WindowCtl, out double CirRow, out double CirColumn)
        {
            bool Result;
            try
            {
                HRegion ho_Region;
                ho_Region = Image.Threshold(0.0, 90.0);

                HRegion ho_ConnectedRegions;
                ho_ConnectedRegions = ho_Region.Connection();

                HRegion ho_SelectedRegions;
                ho_SelectedRegions = ho_ConnectedRegions.SelectShape((new HTuple("area")).TupleConcat("circularity"), "and",
                                                                     (new HTuple(16823.5)).TupleConcat(0.5), (new HTuple(24823.5)).TupleConcat(1));

                HRegion ho_RegionDilation;
                ho_RegionDilation = ho_SelectedRegions.DilationCircle(9.0);

                HImage ho_ImageReduced;
                ho_ImageReduced = Image.ReduceDomain(ho_RegionDilation);

                HXLDCont ho_Edges = ho_ImageReduced.EdgesSubPix("canny", 2, 4, 60);

                double hv_Radius, hv_StartPhi, hv_EndPhi; string hv_PointOrder;
                ho_Edges.FitCircleContourXld("algebraic", -1, 0, 0, 3, 2, out CirRow,
                    out CirColumn, out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);

                HXLDCont ho_ContCircle = new HXLDCont();
                ho_ContCircle.GenCircleContourXld(CirRow, CirColumn, hv_Radius, 0, 2 * Math.PI, "positive", 1.0);

                HXLDCont ho_Cross1 = new HXLDCont();
                ho_Cross1.GenCrossContourXld(CirRow, CirColumn, 100, 45 * Math.PI / 180);

                WindowCtl.Set_Colour("green");
                WindowCtl.ShowObject(ho_ContCircle);

                WindowCtl.Set_Colour("red");
                WindowCtl.ShowObject(ho_Cross1);

                Result = true;
            }
            catch (Exception ex)
            {
                NotifyG.Add(ex.ToString());
                CirRow = 0; CirColumn = 0;
                Result = false;
            }
            return Result;
        }

        private void Hwind_Activated()
        {
            Basler.Clear_Grabevent();
            Basler.EventGrab += Show;
        }

    }
}
