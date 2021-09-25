
namespace Calibration
{
    partial class WindowDisplayCtl
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.lblStatus = new System.Windows.Forms.Label();
            this.hWindowControl = new HalconDotNet.HSmartWindowControl();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuILoadImage = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveOriImage = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveResultImage = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblStatus.Font = new System.Drawing.Font("微软雅黑", 10.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblStatus.ForeColor = System.Drawing.Color.Fuchsia;
            this.lblStatus.Location = new System.Drawing.Point(0, 486);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(0, 25);
            this.lblStatus.TabIndex = 1;
            // 
            // hWindowControl
            // 
            this.hWindowControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.hWindowControl.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.hWindowControl.ContextMenuStrip = this.contextMenuStrip1;
            this.hWindowControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.hWindowControl.HDoubleClickToFitContent = true;
            this.hWindowControl.HDrawingObjectsModifier = HalconDotNet.HSmartWindowControl.DrawingObjectsModifier.None;
            this.hWindowControl.HImagePart = new System.Drawing.Rectangle(0, 0, 640, 480);
            this.hWindowControl.HKeepAspectRatio = true;
            this.hWindowControl.HMoveContent = true;
            this.hWindowControl.HZoomContent = HalconDotNet.HSmartWindowControl.ZoomContent.WheelForwardZoomsIn;
            this.hWindowControl.Location = new System.Drawing.Point(0, 0);
            this.hWindowControl.Margin = new System.Windows.Forms.Padding(0);
            this.hWindowControl.Name = "hWindowControl";
            this.hWindowControl.Size = new System.Drawing.Size(694, 486);
            this.hWindowControl.TabIndex = 2;
            this.hWindowControl.WindowSize = new System.Drawing.Size(694, 486);
            this.hWindowControl.HMouseDown += new HalconDotNet.HMouseEventHandler(this.hWindowControl_HMouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Font = new System.Drawing.Font("Leelawadee UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuILoadImage,
            this.SaveOriImage,
            this.SaveResultImage});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(175, 76);
            // 
            // MenuILoadImage
            // 
            this.MenuILoadImage.Name = "MenuILoadImage";
            this.MenuILoadImage.Size = new System.Drawing.Size(174, 24);
            this.MenuILoadImage.Text = "加载图片文件";
            this.MenuILoadImage.Click += new System.EventHandler(this.MenuILoadImage_Click);
            // 
            // SaveOriImage
            // 
            this.SaveOriImage.Name = "SaveOriImage";
            this.SaveOriImage.Size = new System.Drawing.Size(174, 24);
            this.SaveOriImage.Text = "保存原始图片";
            this.SaveOriImage.Click += new System.EventHandler(this.SaveOriImage_Click);
            // 
            // SaveResultImage
            // 
            this.SaveResultImage.Name = "SaveResultImage";
            this.SaveResultImage.Size = new System.Drawing.Size(174, 24);
            this.SaveResultImage.Text = "保存结果图片";
            this.SaveResultImage.Click += new System.EventHandler(this.SaveResultImage_Click);
            // 
            // WindowDisplayCtl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.hWindowControl);
            this.Controls.Add(this.lblStatus);
            this.Name = "WindowDisplayCtl";
            this.Size = new System.Drawing.Size(694, 511);
            this.Load += new System.EventHandler(this.WindowDisplayCtl_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblStatus;
        private HalconDotNet.HSmartWindowControl hWindowControl;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MenuILoadImage;
        private System.Windows.Forms.ToolStripMenuItem SaveOriImage;
        private System.Windows.Forms.ToolStripMenuItem SaveResultImage;
    }
}
