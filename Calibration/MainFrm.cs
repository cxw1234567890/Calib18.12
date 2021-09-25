using Calibration.BaslerSDK;
using System;
using System.Windows.Forms;

namespace Calibration
{
    public partial class MainFrm : Form
    {
        DataClass DC = DataClass.Instance;
        public MainFrm()
        {
            InitializeComponent();
        }

        private void 建工具坐标ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form3 frm3 = GenericSingleton<Form3>.CreateInstrance();
            frm3.Show(this.dockPanel1);
            frm3.Text = "工具坐标";
        }

        private void 九点标定ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form4 frm4 = GenericSingleton<Form4>.CreateInstrance();
            frm4.Show(this.dockPanel1);
            frm4.Text = "九点标定";
        }

        private void MainFrm_Load(object sender, EventArgs e)
        {
            this.IsMdiContainer = true;
            dockPanel1.Dock = DockStyle.Fill;
        }

        private void CamerSetTSItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = GenericSingleton<Form2>.CreateInstrance();
            frm2.Show(this.dockPanel1);
            frm2.Text = "相机设定";
        }

        private void 日志ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Message mes = GenericSingleton<Message>.CreateInstrance();
            mes.Show(this.dockPanel1, WeifenLuo.WinFormsUI.Docking.DockState.DockLeft);
            mes.Text = "运行日志";
        }


        private void MainFrm_FormClosed(object sender, FormClosedEventArgs e)
        {

            foreach (BaslerCamera basler in DC.BaslerList)
            {
                basler.Close();//释放所有的相机
            }
        }
    }
}
