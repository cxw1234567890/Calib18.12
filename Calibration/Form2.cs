using Calibration.BaslerSDK;
using HalconDotNet;
using System;
using System.Drawing;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace Calibration
{
    public partial class Form2 : DockContent
    {
        BaslerCamera Basler;
        WindowDisplayCtl hWindow;
        DataClass DC = DataClass.Instance;
        public Form2()
        {
            InitializeComponent();
            hWindow = new WindowDisplayCtl();
            hWindow.Dock = DockStyle.Fill;
            panel1.Controls.Add(hWindow);
        }

        private void Form2_Load(object sender, EventArgs e)
        {         
            AddCamerUser();
            Creat_Cam();
        }

        void Creat_Cam()
        {
            Basler = DC.BaslerList.Find(p => p.UserName ==comboBox1.Text);
            if (Basler == null)
            {
                Basler = new BaslerCamera();
                Basler.UserName = comboBox1.Text;
                DC.BaslerList.Add(Basler);                
            }
            Refr_UI_Stat();
        }

        void AddCamerUser()
        {
            comboBox1.Items.Add("50502696");
            comboBox1.Items.Add("Measure");
            comboBox1.Text = "50502696";//设定默认选项
        }

        private void OpenC_Click(object sender, EventArgs e)
        {
            if (!Basler.Connected)
            {
                Basler.Open();             
            }
            Refr_UI_Stat();
        }

        private void Show(HImage Image)
        {
            if (hWindow.IsHandleCreated)
            {
                hWindow.BeginInvoke(new MethodInvoker(
                () => hWindow.ShowImage(Image))
                                                 );
                /*本来想直接写Lambda表达式的，结果发现报错。因为:对于Control.BeginInvoke()来说，任何的代理类型都是可接受的，
                   这样编译器反而不知道应该用哪个代理去匹配匿名函数了，导致了编译错误的发生。经过测试ThreadStart  MethodInvoker  Action都可以
                   更新界面推荐使用MethodInvoker。
            */
            }
        }
        /// <summary>
        /// 单帧采图
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (Basler.Connected)
            {
                Hwind_Activated();
                Basler.GrabImage();
            }
            else 
            {
                MessageBox.Show("相机未打开");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (Basler.Connected)
            {
                Hwind_Activated();
                Basler.ContinuousGrabImage();
                button3.BackColor = Color.Cyan;
            }
            else
            {
                MessageBox.Show("相机未打开");
            }
        }


        private void button4_Click(object sender, EventArgs e)
        {
            Basler.Stop();
            button3.BackColor = Color.Gray;
        }

        //绑定事件Show通过相机的采图回调函数执行
        private void Hwind_Activated()
        {
            Basler.Clear_Grabevent();
            Basler.EventGrab += Show;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Creat_Cam();
        }

        /// <summary>
        /// 刷新状态
        /// </summary>
        private void Refr_UI_Stat()
        {
            if (Basler != null)
            {
                if (Basler.Connected)
                {
                    OpenC.BackColor = Color.Green;
                    OpenC.Text = "已打开";
                }
                else
                {
                    OpenC.BackColor = Color.Red;
                    OpenC.Text = "打开相机";
                }
                textBox1.Text = Basler.GetExposureTime().ToString();
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (Basler!=null &&Basler.Connected)
                    Basler.SetExposureTime(textBox1.Text);
            }
            catch (Exception )
            {
            }     
        }
    }
}
