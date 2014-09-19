using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VSCM;

namespace AutoStartApp
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        VSCM.ProQQMusicMonitor monitor=new ProQQMusicMonitor();
        private void button3_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox2.Text) || string.IsNullOrEmpty(textBox5.Text))
            {
                MessageBox.Show("请填写线程名称！");
                return;
            }
            if (string.IsNullOrEmpty(textBox1.Text) || string.IsNullOrEmpty(textBox4.Text))
            {
                MessageBox.Show("请选择程序！");
                return;
            }
            monitor = new ProQQMusicMonitor();
            monitor.MyAPPPath = textBox1.Text;
            monitor.MyAppName = textBox2.Text;
            monitor.MyAPPArguments = textBox3.Text;
            monitor.QQMusicPath = textBox4.Text;
            //
            monitor.Start();
            //
            button3.Enabled = false;
            button4.Enabled = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if(monitor!=null)
                monitor.Stop();
            button3.Enabled = true;
            button4.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = SelectApp();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox4.Text = SelectApp();
        }

        private string SelectApp()
        {
            var opdiag=new OpenFileDialog();
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                return opdiag.FileName;
            }
            return "";
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //this.WindowState = FormWindowState.Normal;
            this.Visible = true;//窗体不可见
            this.notifyIcon1.Visible = false;//托盘图标显示
        }

        private void button5_Click(object sender, EventArgs e)
        {
            //this.WindowState=FormWindowState.Minimized;
            this.Visible = false;//窗体不可见
            this.notifyIcon1.Visible = true;//托盘图标显示
        }

       
    }
}
