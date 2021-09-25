using Calibration.BaslerSDK;
using HalconDotNet;
using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

namespace Calibration
{

    public partial class Form1:Form
    {
        bool CamOK = false;
        bool Stop;
        EpsonRobot EpsonR;
        HImage Image=new HImage();
        WindowDisplayCtl hWindow1, hWindow2;
        BaslerCamera Basler;
        bool GrabOK;
        uint SelectHwindow = 0;
        public Form1()
        {
            InitializeComponent();
            Ini();
        }
        private void Ini()
        {
            hWindow1 = new WindowDisplayCtl();
            hWindow2 = new WindowDisplayCtl();
            hWindow1.Dock = DockStyle.Fill;
            hWindow2.Dock = DockStyle.Fill;
            groupBox2.Controls.Add(hWindow1);
            panel2.Controls.Add(hWindow2);                  
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Basler = new BaslerCamera();
            EpsonR = new EpsonRobot();

            EpsonR.Connect();
            EpsonR.Init();
            EpsonR.LogAppend += ShowMsg;//追加日志               
        }


        /// <summary>
        /// 开始建工具坐标系
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartFun_Click(object sender, EventArgs e)
        {
            if (!CamOK) return;
            Stop = false;
            SelectHwindow = 1;
            ThreadPool.QueueUserWorkItem(new WaitCallback(Fun));          
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
        private void Fun(Object o)
        {
            double CirRow, CirColumn;
            int ImageWidth = 1920, ImageHeight = 1080;
            while (!Stop)
            {
                try
                {
                    if (!GrabImageWait()) break;
                    if (!GetCenterPoint(Image, hWindow1, out CirRow, out CirColumn)) break;//求圆心坐标

                    string Message = "R:" + (CirRow - ImageHeight / 2 - 1).ToString("f1")
                                            + "  " + "C:" + (CirColumn - ImageWidth / 2 - 1).ToString("f1");

                    hWindow1.ShowMessage(25, "微软雅黑",Message, "image", 1, 1,"magenta", "false");

                    Thread.Sleep(1);
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.ToString());
                }
            }
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Basler.Close();
        }


        /// <summary>
        /// 求标定板的圆心
        /// </summary>
        /// <param name="Image"></param>
        /// <param name="CirRow"></param>
        /// <param name="CirColumn"></param>
        /// <returns></returns>
        private bool GetCenterPoint(HImage Image , WindowDisplayCtl WindowCtl, out double CirRow, out double CirColumn)
        {
            bool Result ;           
            try
            {
                HRegion ho_Region;
                ho_Region = Image.Threshold(0.0,90.0);

                HRegion ho_ConnectedRegions;
                ho_ConnectedRegions =ho_Region.Connection();

                HRegion ho_SelectedRegions;
                ho_SelectedRegions = ho_ConnectedRegions.SelectShape((new HTuple("area")).TupleConcat("circularity"), "and",
                                                                     (new HTuple(16823.5)).TupleConcat(0.5), (new HTuple(24823.5)).TupleConcat(1));

                HRegion ho_RegionDilation;
                ho_RegionDilation = ho_SelectedRegions.DilationCircle(9.0);

                HImage ho_ImageReduced;
                ho_ImageReduced = Image.ReduceDomain(ho_RegionDilation);

               HXLDCont ho_Edges= ho_ImageReduced.EdgesSubPix("canny", 2, 4, 60);
                
                double hv_Radius, hv_StartPhi, hv_EndPhi; string hv_PointOrder;
                ho_Edges.FitCircleContourXld("algebraic", -1, 0, 0, 3, 2, out CirRow,
                    out CirColumn,out hv_Radius, out hv_StartPhi, out hv_EndPhi, out hv_PointOrder);

                HXLDCont ho_ContCircle = new HXLDCont();
                ho_ContCircle.GenCircleContourXld(CirRow, CirColumn, hv_Radius, 0, 2*Math.PI, "positive", 1.0);

                HXLDCont ho_Cross1 = new HXLDCont();
                ho_Cross1.GenCrossContourXld(CirRow, CirColumn, 100, 45*Math.PI/180);

                WindowCtl.Set_Colour("green");
                WindowCtl.ShowObject(ho_ContCircle);

                WindowCtl.Set_Colour("red");
                WindowCtl.ShowObject(ho_Cross1);

                Result = true;
            }
            catch (Exception ex)
            {
                CirRow = 0; CirColumn = 0;

                Result = false;
            }
            return Result;
        }


        private void Show(HImage Image)
        {
            if (this.IsHandleCreated)
            {
                hWindow1.BeginInvoke(new MethodInvoker(() =>
                {
                    if (SelectHwindow == 2) 
                    { hWindow2.ShowImage(Image); }
                    else
                    { hWindow1.ShowImage(Image); }
                }
             ));
            }
            this.Image.Dispose();
            this.Image = Image;
            GrabOK = true;
        }
        
        private void ShowMsg(string str)
        {
            if (Log.IsHandleCreated)
            {
                Log.BeginInvoke(new MethodInvoker(() =>
                {
                    Log.AppendText(str + "\r\n");
                }
             ));
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!CamOK)
            {
                if (OpenCam("50502696"))
                {
                    Basler.SetExposureTime("2500");
                    OpenC.BackColor = Color.Green;
                    OpenC.Text = "已打开";
                    button2.BackColor = Color.Green;
                    button2.Text = "已打开";
                }
            }
        }

        #region
        private void StartCli_Click_1(object sender, EventArgs e)
        {
            if (!CamOK) return;
            SelectHwindow = 2;
            ThreadPool.QueueUserWorkItem((object o) =>
            {
                double CirRow, CirColumn;
                try
                {
                    HTuple px = new HTuple(), py = new HTuple(), qx = new HTuple(), qy = new HTuple();
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, "Tool " + 3) != true) { ShowMsg("设置工具坐标系失败!"); return; }
                   //点1
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 0)) != true) return;
                    Thread.Sleep(800);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点2
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 15, 0)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);


                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点3
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 30, 0)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);


                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点4
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 40)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点5
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 15, 40)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点6
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 30, 40)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点7
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 80)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    //点8
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 15, 80)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));

                    ///点9
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 30, 80)) != true) return;
                    Thread.Sleep(500);
                    if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                    qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                    qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);

                    if (!GrabImageWait()) return;//采图
                    if (!GetCenterPoint(Image, hWindow2, out CirRow, out CirColumn)) { return; }
                    px = px.TupleConcat(Math.Round(CirRow, 3));
                    py = py.TupleConcat(Math.Round(CirColumn, 3));


                    HHomMat2D _homMat2D = new HHomMat2D();
                    _homMat2D.VectorToHomMat2d(px, py, qx, qy);
                    HMisc HM = new HMisc();
                    HMisc.WriteTuple(_homMat2D.RawData, Environment.CurrentDirectory + @"\Location.tup");

                    Thread.Sleep(200);
                    double _qx, _qy;
                    _qx= _homMat2D.AffineTransPoint2d( px[0], py[0], out _qy);

                    MessageBox.Show(string.Format("标定结果检测：\r\n输入像素坐标 Row {0} Col {1} \r\n输出世界坐标 Row {2} Col {3}   \r\n实际世界坐标 Row {4} Col{5} \r\n差异 Row {6} Col {7}",
                                                    px[0].D, py[0].D,
                                                    qx[0].D.ToString("0.000"), qy[0].D.ToString("0.000"),
                                                    _qx.ToString("0.000"), _qy.ToString("0.000"),
                                                    (_qx - qx[0].D).ToString("0.000"), (_qy - qy[0].D).ToString("0.000")), "标定完成");
                }
                catch (Exception ex)
                {
                    ShowMsg(ex.ToString());
                }
            });
        }
        #endregion

        private void tabControl1_Selected(object sender, TabControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
            {
                Stop = true;
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (!CamOK) return;
            GrabImageWait();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Basler.ContinuousGrabImage();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Basler.Stop();
        }

        bool GrabImageWait()
        {
            GrabOK = false;
            var grabImageCount = 0;
            while (true)
            {
                if (grabImageCount > 2)
                {
                  return false;
                }
                Basler.GrabImage();
                Thread.Sleep(50);
                if (GrabOK) return true;        
                grabImageCount++;               
            }
        }
    }
}
