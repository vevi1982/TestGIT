using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace AutoStartApp
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new Button();
            this.button2 = new Button();
            this.label1 = new Label();
            this.label2 = new Label();
            this.textBox1 = new TextBox();
            this.textBox2 = new TextBox();
            this.button3 = new Button();
            this.label3 = new Label();
            this.textBox3 = new TextBox();
            this.checkBox1 = new CheckBox();
            this.label4 = new Label();
            this.textBox4 = new TextBox();
            this.label5 = new Label();
            base.SuspendLayout();
            this.button1.Location = new Point(0x143, 0x29);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x25, 0x17);
            this.button1.TabIndex = 1;
            this.button1.Text = "...";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Location = new Point(0xf5, 0xa9);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x4b, 0x31);
            this.button2.TabIndex = 5;
            this.button2.Text = "开始";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.label1.AutoSize = true;
            this.label1.Location = new Point(0x22, 0x2e);
            this.label1.Name = "label1";
            this.label1.Size = new Size(0x35, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "程序路径";
            this.label2.AutoSize = true;
            this.label2.Location = new Point(0x22, 0x6f);
            this.label2.Name = "label2";
            this.label2.Size = new Size(0x35, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "线程名称";
            this.textBox1.Location = new Point(0x5d, 0x2a);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new Size(0xe0, 0x15);
            this.textBox1.TabIndex = 0;
            this.textBox1.Text = "BaiDuMusicNew.exe";
            this.textBox2.Location = new Point(0x5d, 0x6c);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new Size(0xe0, 0x15);
            this.textBox2.TabIndex = 3;
            this.textBox2.Text = "BaiduMusicNew";
            this.button3.Location = new Point(0x146, 0xa9);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x4b, 0x31);
            this.button3.TabIndex = 6;
            this.button3.Text = "停止";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.button3_Click);
            this.label3.AutoSize = true;
            this.label3.Location = new Point(0x5f, 0x49);
            this.label3.Name = "label3";
            this.label3.Size = new Size(0x1d, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "参数";
            this.textBox3.Location = new Point(130, 70);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new Size(0xbb, 0x15);
            this.textBox3.TabIndex = 2;
            this.textBox3.Text = "-auto";
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = CheckState.Checked;
            this.checkBox1.Location = new Point(0x1a, 0xa2);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new Size(0xca, 0x2b);
            this.checkBox1.TabIndex = 4;
            this.checkBox1.Text = "等待时间后IP不变，断开连接，重启程序(如果次数过多那么重启电脑)";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.label4.AutoSize = true;
            this.label4.Location = new Point(0x22, 0x8b);
            this.label4.Name = "label4";
            this.label4.Size = new Size(0x35, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "等待时间";
            this.label4.TextAlign = ContentAlignment.MiddleLeft;
            this.textBox4.Location = new Point(0x5d, 0x87);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new Size(100, 0x15);
            this.textBox4.TabIndex = 8;
            this.textBox4.Text = "10";
            this.label5.AutoSize = true;
            this.label5.Location = new Point(0xc7, 0x8b);
            this.label5.Name = "label5";
            this.label5.Size = new Size(0x1d, 12);
            this.label5.TabIndex = 7;
            this.label5.Text = "分钟";
            this.label5.TextAlign = ContentAlignment.MiddleLeft;
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            base.ClientSize = new Size(0x19d, 230);
            base.Controls.Add(this.textBox4);
            base.Controls.Add(this.label5);
            base.Controls.Add(this.label4);
            base.Controls.Add(this.checkBox1);
            base.Controls.Add(this.textBox3);
            base.Controls.Add(this.textBox2);
            base.Controls.Add(this.label3);
            base.Controls.Add(this.textBox1);
            base.Controls.Add(this.label2);
            base.Controls.Add(this.label1);
            base.Controls.Add(this.button3);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Name = "Form1";
            this.Text = "自动重启程序";
            base.ResumeLayout(false);
            base.PerformLayout();
        }

        // Fields
        private bool AutoRebot;
        private Button button1;
        private Button button2;
        private Button button3;
        private CheckBox checkBox1;
        private string currentIp = "";
        private int ipCount;
        private Timer IpTimer;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private int restartCount;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private TextBox textBox4;
        private Timer timer;
        private int waitTime = 10;

        #endregion
    }
}

