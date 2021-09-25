namespace Calibration
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.StopFun = new System.Windows.Forms.Button();
            this.StartFun = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.StopFun);
            this.groupBox1.Controls.Add(this.StartFun);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(602, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(275, 576);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "建工具坐标系";
            // 
            // StopFun
            // 
            this.StopFun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StopFun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.StopFun.FlatAppearance.BorderSize = 0;
            this.StopFun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Magenta;
            this.StopFun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(192)))));
            this.StopFun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StopFun.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StopFun.Location = new System.Drawing.Point(133, 147);
            this.StopFun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StopFun.Name = "StopFun";
            this.StopFun.Size = new System.Drawing.Size(79, 56);
            this.StopFun.TabIndex = 3;
            this.StopFun.Text = "停止";
            this.StopFun.UseVisualStyleBackColor = false;
            this.StopFun.Click += new System.EventHandler(this.StopFun_Click);
            // 
            // StartFun
            // 
            this.StartFun.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartFun.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.StartFun.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.StartFun.FlatAppearance.BorderSize = 0;
            this.StartFun.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Aqua;
            this.StartFun.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.StartFun.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartFun.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartFun.Location = new System.Drawing.Point(133, 56);
            this.StartFun.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StartFun.Name = "StartFun";
            this.StartFun.Size = new System.Drawing.Size(79, 56);
            this.StartFun.TabIndex = 1;
            this.StartFun.Text = "开始";
            this.StartFun.UseVisualStyleBackColor = false;
            this.StartFun.Click += new System.EventHandler(this.StartFun_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(602, 576);
            this.panel1.TabIndex = 8;
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(877, 576);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Form3";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button StopFun;
        private System.Windows.Forms.Button StartFun;
        private System.Windows.Forms.Panel panel1;
    }
}