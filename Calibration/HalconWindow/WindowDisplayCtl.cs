using System;
using System.Drawing;
using System.Windows.Forms;
using HalconDotNet;

namespace Calibration
{
    /// <summary>
    /// 对Halcon控件的封装
    /// </summary>
    public partial class WindowDisplayCtl : UserControl
    {
        private readonly object lockObj = new object();
        public int imageWidth, imageHeight;

        /// <summary>
        /// 图像
        /// </summary>
        public HImage image=new HImage();

        public WindowDisplayCtl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示图像
        /// </summary>
        /// <param name="_image">图片</param>
        /// <returns></returns>
        public bool ShowImage(HImage _image)
        {
            lock (lockObj)
            {
                try
                {
                    if (hWindowControl == null) { return false; }

                    if (!HObjectOperations.ObjectValided(_image))
                    {
                        return false;
                    }
                    hWindowControl.HalconWindow.ClearWindow();
                    Set_Colour("blue");
                    _image.GetImageSize(out imageWidth, out imageHeight);
                    DispImageFit(hWindowControl, new HImage(_image));
                    //hWindowControl.SetFullImagePart(null);
                    //hWindowControl.HalconWindow.SetPart(0, 0, -2, -2);
                    image.Dispose();
                    image=_image.CopyImage();
                    _image.DispObj(hWindowControl.HalconWindow);
                    DisplayCross();
                }
                catch (HalconException ex)
                {
                    return false;
                }
                return true;
            }
        }
        public bool ShowObject(HObject obj)
        {
            lock (lockObj)
            {
                try
                {
                    if (hWindowControl == null) return false;
                    if (hWindowControl.ParentForm == null) return false;
                    if (obj != null)
                    {

                        if (!HObjectOperations.ObjectValided(obj)) return false;
                        hWindowControl.HalconWindow.DispObj(obj);
                        // hWindowControl.Invoke(new Action(() => { hWindowControl.HalconWindow.DispObj(displayObject); }));
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
        }
        private void DispImageFit(HSmartWindowControl _hWindowControl, HImage hImage)
        {
            if (!HObjectOperations.ObjectValided(hImage)) return;

            double ratioWindow = (double)_hWindowControl.WindowSize.Width / (double)_hWindowControl.WindowSize.Height;
            double ratioImage = (double)imageWidth / (double)imageHeight;

            int _beginRow, _begin_Col, _endRow, _endCol;

            if (ratioWindow >= ratioImage)
            {
                _beginRow = 0;
                _endRow = imageHeight - 1;
                _begin_Col = -(int)(imageWidth * (ratioWindow / ratioImage - 1) / 2);
                _endCol = (int)(imageWidth + imageWidth * (ratioWindow / ratioImage - 1) / 2);
                //将多出来的部分一分为二，分别叠加到两端
            }
            else
            {
                _begin_Col = 0;
                _endCol = imageWidth - 1;
                _beginRow = -(int)(imageHeight * (ratioImage / ratioWindow - 1) / 2);
                _endRow = (int)(imageHeight+ imageHeight * (ratioImage / ratioWindow - 1));
            }
            _hWindowControl.HalconWindow.SetPart(_beginRow, _begin_Col, _endRow, _endCol);
        }


        private void DisplayCross()
        {
            try
            {
                HXLDCont ho_Cross = new HXLDCont();
                ho_Cross.GenCrossContourXld((double)imageHeight / 2, (double)imageWidth / 2, 2000, 0);
                hWindowControl.HalconWindow.DispObj(ho_Cross);
            }
            catch (Exception ex)
            {
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="Size">字体大小</param>
        /// <param name="Font">字体</param>
        /// <param name="Bold">是否加粗</param>
        /// <param name="Slant">是否倾斜</param>
        /// <param name="Msg">打印字符串</param>
        /// <param name="CoordSystem">参考位置image或者windows</param>
        /// <param name="row">纵向坐标</param>
        /// <param name="col">横向坐标</param>
        /// <param name="color">字体颜色</param>
        /// <param name="Box">是否有底框</param>
        /// <returns>是否设置成功</returns>
        public bool ShowMessage(int Size, string Font, string Msg, string CoordSystem, double row,
            double col, string color, string Box)
        {
            lock (lockObj)
            {
                try
                {

                    if (hWindowControl == null) return false;
                    if (hWindowControl.ParentForm == null) return false;
                    HTuple TempFont = hWindowControl.HalconWindow.QueryFont();//可以查询所有可用字体
                    hWindowControl.HalconWindow.SetFont(Font+"-Normal-"+Size);
                    disp_message(hWindowControl.HalconWindow, Msg, CoordSystem.ToLower(), row, col, color, Box.ToLower());
                }
                catch (Exception ex)
                {
                    return false;
                }
                return true;
            }
        }
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {
            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed
            //String: A tuple of strings containing the text message to be displayed
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position
            //   A tuple of values is allowed to display text at different
            //   positions.
            //Column: The column coordinate of the desired text position
            //   A tuple of values is allowed to display text at different
            //   positions.
            //Color: defines the color of the text as string.
            //   If set to [], '' or 'auto' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically...
            //   - if |Row| == |Column| == 1: for each new textline
            //   = else for each text position.
            //Box: If Box[0] is set to 'true', the text is written within an orange box.
            //     If set to' false', no box is displayed.
            //     If set to a color string (e.g. 'white', '#FF00CC', etc.),
            //       the text is written in a box of that color.
            //     An optional second value for Box (Box[1]) controls if a shadow is displayed:
            //       'true' -> display a shadow in a default color
            //       'false' -> display no shadow
            //       otherwise -> use given string as color string for the shadow color
            //
            //It is possible to display multiple text strings in a single call.
            //In this case, some restrictions apply:
            //- Multiple text positions can be defined by specifying a tuple
            //  with multiple Row and/or Column coordinates, i.e.:
            //  - |Row| == n, |Column| == n
            //  - |Row| == n, |Column| == 1
            //  - |Row| == 1, |Column| == n
            //- If |Row| == |Column| == 1,
            //  each element of String is display in a new textline.
            //- If multiple positions or specified, the number of Strings
            //  must match the number of positions, i.e.:
            //  - Either |String| == n (each string is displayed at the
            //                          corresponding position),
            //  - or     |String| == 1 (The string is displayed n times).
            //
            //
            //Convert the parameters for disp_text.
            if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP = 12;
            }
            //
            //Convert the parameter Box to generic parameters.
            hv_GenParamName.Dispose();
            hv_GenParamName = new HTuple();
            hv_GenParamValue.Dispose();
            hv_GenParamValue = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                {
                    //Display no box
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual("true"))) != 0)
                {
                    //Set a color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(0));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                {
                    //Display no shadow.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual("true"))) != 0)
                {
                    //Set a shadow color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(1));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            //Restore default CoordSystem behavior.
            if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
            {
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP = "image";
            }
            //
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                //disp_text does not accept an empty string for Color.
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Color_COPY_INP_TMP = new HTuple();
            }
            //
            HOperatorSet.DispText(hv_WindowHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                hv_GenParamValue);

            hv_Color_COPY_INP_TMP.Dispose();
            hv_Column_COPY_INP_TMP.Dispose();
            hv_CoordSystem_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_GenParamName.Dispose();
            hv_GenParamValue.Dispose();

            return;
        }


        private void hWindowControl_HMouseDown(object sender, HMouseEventArgs e)
        {
            lock (lockObj)
            {
                try
                {
                    if (hWindowControl == null) return;
                    if (image == null) return;
                    if (!image.IsInitialized()) return;

                    int button_state;
                    double Row, Column;
                    string str_value;
                    string str_position;
                    bool _isXOut = true, _isYOut = true;
                    HTuple channel_count;

                    //获取当前位置
                    channel_count = image.CountChannels();
                    hWindowControl.HalconWindow.GetMpositionSubPix(out Row, out Column, out button_state);

                    str_position = String.Format("Row: {0:0.0}, Col: {1:0.0}", Row, Column);

                    string ImageSize = String.Format("{0}X{1}", imageWidth, imageHeight);
                    //获取值
                    _isXOut = (Row < 0 || Row >= imageHeight);
                    _isYOut = (Column < 0 || Column >= imageWidth);

                    if (!_isXOut && !_isYOut)
                    {
                        if ((int)channel_count == 1)
                        {
                            HTuple grayVal;
                            grayVal=image.GetGrayval((int)Row, (int)Column);                        
                            str_value = String.Format("Val: {0:000.0}", grayVal.I);
                        }
                        else if ((int)channel_count == 3)
                        {
                            double grayValRed, grayValGreen, grayValBlue;

                            HImage _RedChannel, _GreenChannel, _BlueChannel;

                            _RedChannel = image.AccessChannel(1);
                            _GreenChannel = image.AccessChannel(2);
                            _BlueChannel = image.AccessChannel(3);

                            grayValRed = _RedChannel.GetGrayval((int)Row, (int)Column);
                            grayValGreen = _GreenChannel.GetGrayval((int)Row, (int)Column);
                            grayValBlue = _BlueChannel.GetGrayval((int)Row, (int)Column);

                            _RedChannel.Dispose();
                            _GreenChannel.Dispose();
                            _BlueChannel.Dispose();

                            str_value = String.Format("Val: ({0:000.0}, {1:000.0}, {2:000.0})", grayValRed, grayValGreen, grayValBlue);
                        }
                        else
                        {
                            str_value = "";
                        }
                        lblStatus.Text = ImageSize + "    " + str_position + "    " + str_value;
                        lblStatus.Refresh();
                    }
                }
                catch (HalconException ex)
                {
                }
            }
        }

        /// <summary>
        /// 设置显示对象的颜色XLD
        /// </summary>
        /// <param name="Colour"></param>
        /// <returns></returns>
        public bool Set_Colour(string Colour)
        {
            bool result=false;
            try
            {
                hWindowControl.HalconWindow.SetColor(Colour);
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
            }
            return result;
        }

        private void WindowDisplayCtl_Load(object sender, EventArgs e)
        {
            lblStatus.Visible = true;
            HSystem.SetSystem("do_low_error", "false");//少报错
            HSystem.SetSystem("clip_region", "false");//region在图像外不切掉
            HSystem.SetSystem("border_shape_models", "true");//依然匹配边缘的图形
            MouseWheel += new MouseEventHandler(my_MouseWheel);
        }

        private void SaveOriImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string imgFileName = string.Empty;
            sfd.Filter = "所有文件|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(sfd.FileName))
                    return;
                imgFileName = sfd.FileName;
                SaveImage(imgFileName);
            }
        }
        public void SaveImage(string fileName)
        {
            try
            {
                if (HObjectOperations.ObjectValided(image))
                {
                    image.WriteImage("tiff", 0, fileName);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void SaveResultImage_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            string imgFileName;
            sfd.Filter = "PNG图像|*.png|BMP图像|*.bmp|JPEG图像|*.jpg|所有文件|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if (String.IsNullOrEmpty(sfd.FileName))
                    return;
                imgFileName = sfd.FileName;
                SaveWindowDump(imgFileName);
            }
        }
        public void SaveWindowDump(string fileName)
        {
            try
            {
                if (image != null)
                {
                    hWindowControl.HalconWindow.DumpWindow("png best", fileName);
                }
            }
            catch (HalconException ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void MenuILoadImage_Click(object sender, EventArgs e)
        {
            string ImagePath = string.Empty;
            OpenFileDialog openFileDialog2 = new OpenFileDialog();
            openFileDialog2.Title = "请选择图片";
            openFileDialog2.Filter = "(*.jpg,*.png,*.jpg,*.bmp,*.tif)|*.jgp;*.png;*.jpg;*.bmp;*.tif|All files(*.*)|*.*";
            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog2.FileName != "")
                {
                    ImagePath = openFileDialog2.FileName;
                    HImage himage = new HImage(ImagePath);
                    ShowImage(himage);
                }
            }
        }

        private void my_MouseWheel(object sender, MouseEventArgs e)
        {
            Point pt = this.Location;
            int leftBorder = hWindowControl.Location.X;
            int rightBorder = hWindowControl.Location.X + hWindowControl.Size.Width;
            int topBorder = hWindowControl.Location.Y;
            int bottomBorder = hWindowControl.Location.Y + hWindowControl.Size.Height;
            if (e.X > leftBorder && e.X < rightBorder && e.Y > topBorder && e.Y < bottomBorder)
            {
                MouseEventArgs newe = new MouseEventArgs(e.Button, e.Clicks,
                                                     e.X - pt.X, e.Y - pt.Y, e.Delta);
                hWindowControl.HSmartWindowControl_MouseWheel(sender, newe);
            }
        }
    }
}

