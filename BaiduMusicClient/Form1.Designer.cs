using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using BaiduMusicClient.Controler;

namespace BaiduMusicClient
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
            this.button3 = new Button();
            this.button4 = new Button();
            this.button5 = new Button();
            this.button6 = new Button();
            base.SuspendLayout();
            this.button1.Location = new Point(0x5e, 0x67);
            this.button1.Name = "button1";
            this.button1.Size = new Size(0x9d, 0x4c);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new EventHandler(this.button1_Click);
            this.button2.Location = new Point(0x121, 0x67);
            this.button2.Name = "button2";
            this.button2.Size = new Size(0x9d, 0x4c);
            this.button2.TabIndex = 1;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new EventHandler(this.button2_Click);
            this.button3.Location = new Point(0x1a9, 12);
            this.button3.Name = "button3";
            this.button3.Size = new Size(0x4b, 0x17);
            this.button3.TabIndex = 4;
            this.button3.Text = "Tools";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new EventHandler(this.button3_Click);
            this.button4.Location = new Point(0x158, 12);
            this.button4.Name = "button4";
            this.button4.Size = new Size(0x4b, 0x17);
            this.button4.TabIndex = 3;
            this.button4.Text = "GetIP";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new EventHandler(this.button4_Click);
            this.button5.Location = new Point(0x1a9, 0xef);
            this.button5.Name = "button5";
            this.button5.Size = new Size(0x4b, 0x17);
            this.button5.TabIndex = 5;
            this.button5.Text = "Close";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new EventHandler(this.button5_Click);
            this.button6.Location = new Point(0x107, 12);
            this.button6.Name = "button6";
            this.button6.Size = new Size(0x4b, 0x17);
            this.button6.TabIndex = 2;
            this.button6.Text = "Logs";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new EventHandler(this.button6_Click);
            base.AutoScaleDimensions = new SizeF(6f, 12f);
            //base.AutoScaleMode = AutoScaleMode.Font;
            base.ClientSize = new Size(0x200, 0x112);
            base.Controls.Add(this.button5);
            base.Controls.Add(this.button6);
            base.Controls.Add(this.button4);
            base.Controls.Add(this.button3);
            base.Controls.Add(this.button2);
            base.Controls.Add(this.button1);
            base.Name = "Form1";
            this.Text = "百度音乐刷榜";
            base.ResumeLayout(false);
        }

 

       
        private Button button1;
        private Button button2;
        private Button button3;
        private Button button4;
        private Button button5;
        private Button button6;


        #endregion
    }
}

