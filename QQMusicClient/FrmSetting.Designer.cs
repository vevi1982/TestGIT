namespace QQMusicClient
{
    partial class FrmSetting
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
            this.button1 = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtAdslPass = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAdslName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.txtPcName = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtDownLoadPath = new System.Windows.Forms.TextBox();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.txtDownLoadPath);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtPath);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 53);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(371, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "QQMusicExePath";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(329, 18);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(36, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(89, 20);
            this.txtPath.Name = "txtPath";
            this.txtPath.ReadOnly = true;
            this.txtPath.Size = new System.Drawing.Size(234, 21);
            this.txtPath.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ClientExePath";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtAdslPass);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtUserName);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.txtAdslName);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox2.Location = new System.Drawing.Point(0, 141);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(371, 111);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ADSLSetting";
            // 
            // txtAdslPass
            // 
            this.txtAdslPass.Location = new System.Drawing.Point(65, 77);
            this.txtAdslPass.Name = "txtAdslPass";
            this.txtAdslPass.Size = new System.Drawing.Size(294, 21);
            this.txtAdslPass.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 77);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 2;
            this.label4.Text = "ADSLPass";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(65, 50);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(294, 21);
            this.txtUserName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 50);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 2;
            this.label3.Text = "UserName";
            // 
            // txtAdslName
            // 
            this.txtAdslName.Location = new System.Drawing.Point(65, 23);
            this.txtAdslName.Name = "txtAdslName";
            this.txtAdslName.Size = new System.Drawing.Size(294, 21);
            this.txtAdslName.TabIndex = 3;
            this.txtAdslName.Text = "宽带连接";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "ADSLName";
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(215, 285);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 2;
            this.button2.Text = "&OK";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button3.Location = new System.Drawing.Point(296, 285);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 2;
            this.button3.Text = "&Cancel";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.checkBox1);
            this.groupBox3.Controls.Add(this.txtPcName);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(371, 53);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ClientInfo";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(262, 22);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(96, 16);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "是否ADSL拨号";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // txtPcName
            // 
            this.txtPcName.Location = new System.Drawing.Point(77, 20);
            this.txtPcName.Name = "txtPcName";
            this.txtPcName.Size = new System.Drawing.Size(168, 21);
            this.txtPcName.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 23);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 12);
            this.label5.TabIndex = 2;
            this.label5.Text = "ClientName";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 50);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "DownLoadPath";
            // 
            // txtDownLoadPath
            // 
            this.txtDownLoadPath.Location = new System.Drawing.Point(89, 47);
            this.txtDownLoadPath.Name = "txtDownLoadPath";
            this.txtDownLoadPath.ReadOnly = true;
            this.txtDownLoadPath.Size = new System.Drawing.Size(234, 21);
            this.txtDownLoadPath.TabIndex = 3;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(329, 45);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(36, 23);
            this.button4.TabIndex = 1;
            this.button4.Text = "...";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // FrmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(371, 320);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Name = "FrmSetting";
            this.Text = "FrmSetting";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtAdslPass;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAdslName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtPcName;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.TextBox txtDownLoadPath;
        private System.Windows.Forms.Label label6;
    }
}