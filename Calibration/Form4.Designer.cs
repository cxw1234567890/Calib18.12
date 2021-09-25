namespace Calibration
{
    partial class Form4
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
            this.panel2 = new System.Windows.Forms.Panel();
            this.StartCli = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.ConnRobot = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ConnRobot);
            this.panel2.Controls.Add(this.StartCli);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel2.Location = new System.Drawing.Point(777, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(246, 750);
            this.panel2.TabIndex = 18;
            // 
            // StartCli
            // 
            this.StartCli.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.StartCli.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.StartCli.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.StartCli.FlatAppearance.BorderSize = 0;
            this.StartCli.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartCli.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StartCli.Location = new System.Drawing.Point(80, 228);
            this.StartCli.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.StartCli.Name = "StartCli";
            this.StartCli.Size = new System.Drawing.Size(79, 66);
            this.StartCli.TabIndex = 14;
            this.StartCli.Text = "开始标定";
            this.StartCli.UseVisualStyleBackColor = false;
            this.StartCli.Click += new System.EventHandler(this.StartCli_Click);
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(777, 750);
            this.panel1.TabIndex = 19;
            // 
            // ConnRobot
            // 
            this.ConnRobot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnRobot.BackColor = System.Drawing.Color.Silver;
            this.ConnRobot.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ConnRobot.FlatAppearance.BorderSize = 0;
            this.ConnRobot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ConnRobot.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.ConnRobot.Location = new System.Drawing.Point(80, 136);
            this.ConnRobot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.ConnRobot.Name = "ConnRobot";
            this.ConnRobot.Size = new System.Drawing.Size(79, 66);
            this.ConnRobot.TabIndex = 15;
            this.ConnRobot.Text = "连接Robot";
            this.ConnRobot.UseVisualStyleBackColor = false;
            this.ConnRobot.Click += new System.EventHandler(this.ConnRobot_Click);
            // 
            // Form4
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1023, 750);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Name = "Form4";
            this.Text = "Form4";
            this.Load += new System.EventHandler(this.Form4_Load);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button StartCli;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button ConnRobot;
    }
}