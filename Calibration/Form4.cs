using Calibration.BaslerSDK;
using AlarmLibrary;
using HalconDotNet;
using System;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Calibration
{
    public partial class Form4 : DockContent
    {
        WindowDisplayCtl hWindow;
        EpsonRobot EpsonR;
        DataClass DC = DataClass.Instance;
        BaslerCamera Basler;
        HImage Image;
        AutoResetEvent _waitHandle = new AutoResetEvent(false);
        public Form4()
        {
            InitializeComponent();
            hWindow = new WindowDisplayCtl();
            hWindow.Dock = DockStyle.Fill;
            panel1.Controls.Add(hWindow);
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            Basler = DC.BaslerList.Find(p => p.UserName == "50502696");
            if (Basler == null)
            {
                Basler = new BaslerCamera();
                Basler.UserName = "50502696";
                DC.BaslerList.Add(Basler);
            }              
        }

        private void StartCli_Click(object sender, EventArgs e)
        {
            if (Basler.Connected)
            {
                Hwind_Activated();
                Task task = new Task(() =>
                {
                    double CirRow, CirColumn;
                    try
                    {
                        HTuple px = new HTuple(), py = new HTuple(), qx = new HTuple(), qy = new HTuple();
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, "Tool " + 3) != true) { NotifyG.Add("设置工具坐标系失败!"); return; }
                        //点1
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 0)) != true) return;
                        Thread.Sleep(520);//等待运动到位
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);//等待坐标刷新
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);//等待机构稳定防抖动
                        if (!GrabImageWait()) return;
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点2
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 15, 0)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);//等待机构稳定防抖动
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点3
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 30, 0)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点4
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 40)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点5
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 15, 40)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点6
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 30, 40)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点7
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 80)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        //点8
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 15, 80)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));

                        ///点9
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 30, 80)) != true) return;
                        Thread.Sleep(320);
                        if (EpsonR.ExecuteCMD(ERemotCMD.Execute, Spel.RealPos) != true) return;
                        Thread.Sleep(80);
                        qx = qx.TupleConcat(EpsonR.Status.PosInfo.X);
                        qy = qy.TupleConcat(EpsonR.Status.PosInfo.Y);
                        Thread.Sleep(100);
                        if (!GrabImageWait()) return;//采图
                        if (!GetCenterPoint(Image, hWindow, out CirRow, out CirColumn)) { return; }
                        px = px.TupleConcat(Math.Round(CirRow, 3));
                        py = py.TupleConcat(Math.Round(CirColumn, 3));


                        HHomMat2D _homMat2D = new HHomMat2D();
                        _homMat2D.VectorToHomMat2d(px, py, qx, qy);
                        HMisc HM = new HMisc();
                        HMisc.WriteTuple(_homMat2D.RawData, Environment.CurrentDirectory + @"\Location.tup");

                        Thread.Sleep(200);
                        double _qx, _qy;
                        _qx = _homMat2D.AffineTransPoint2d(px[0], py[0], out _qy);

                        MessageBox.Show(string.Format("标定结果检测：\r\n输入像素坐标 Row {0} Col {1} \r\n输出世界坐标 Row {2} Col {3}   \r\n实际世界坐标 Row {4} Col{5} \r\n差异 Row {6} Col {7}",
                                                        px[0].D, py[0].D,
                                                        qx[0].D.ToString("0.000"), qy[0].D.ToString("0.000"),
                                                        _qx.ToString("0.000"), _qy.ToString("0.000"),
                                                        (_qx - qx[0].D).ToString("0.000"), (_qy - qy[0].D).ToString("0.000")), "标定完成");
                        EpsonR.ExecuteCMD(ERemotCMD.Execute, string.Format("{0} {1} +X({2}) +Y({3}) ", Spel.Move.ToString(), "P40", 0, 0));
                    }
                    catch (Exception ex)
                    {
                        NotifyG.Add(ex.ToString());
                    }
                });
                task.Start();
            }
            else
            {
                MessageBox.Show("请打开相机");
            }
        }

        private void Show(HImage Image)
        {
            this.Image = Image;
            _waitHandle.Set();
        }

        private bool GrabImageWait()
        {
            bool result = false;
            if (Basler.GrabImage() && _waitHandle.WaitOne(100))
            {
                result = true;
            }
            return result;
        }
        

        private bool GetCenterPoint(HImage Image, WindowDisplayCtl WindowCtl, out double CirRow, out double CirColumn)
        {
            bool Result;
            try
            {
                hWindow.ShowImage(Image);
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

        private void ConnRobot_Click(object sender, EventArgs e)
        {
            EpsonR = new EpsonRobot();
           bool IsCon= EpsonR.Connect();
           bool IsIni= EpsonR.Init();
            if (IsCon && IsIni)
            {
                ConnRobot.BackColor=Color.Green;
                ConnRobot.Text = "Robot已连接";
            }
        }

        private void Hwind_Activated()
        {
            Basler.Clear_Grabevent();
            Basler.EventGrab += Show;
        }
    }
}
