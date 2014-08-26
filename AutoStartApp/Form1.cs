using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.Log;
using Vevisoft.WebOperate;
using Vevisoft.WindowsAPI;

namespace AutoStartApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.AutoRebot = ConfigurationManager.AppSettings["AutoRebot"] != "0";
            this.textBox1.Text = ConfigurationManager.AppSettings["AppName"];
            this.textBox3.Text = ConfigurationManager.AppSettings["AppPara"];
            this.textBox2.Text = ConfigurationManager.AppSettings["AppProcessName"];
            var timer = new Timer
            {
                Enabled = true,
                Interval = 0x3e8
            };
            this.timer = timer;
            this.timer.Stop();
            this.timer.Tick += new EventHandler(this.timer_Tick);
            var timer2 = new Timer
            {
                Enabled = true,
                Interval = 0xea60
            };
            this.IpTimer = timer2;
            this.IpTimer.Stop();
            this.IpTimer.Tick += new EventHandler(this.IpTimer_Tick);
            if (ConfigurationManager.AppSettings["AutoRun"] != "0")
            {
                this.button2_Click(null, null);
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                this.textBox1.Text = dialog.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.timer.Start();
            if (this.checkBox1.Checked)
            {
                this.IpTimer.Start();
            }
            this.button2.Enabled = false;
            this.button3.Enabled = true;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.timer.Stop();
            this.IpTimer.Stop();
            this.button2.Enabled = true;
            this.button3.Enabled = false;
        }
        private void IpTimer_Tick(object sender, EventArgs e)
        {
            if ((this.restartCount > 600) && this.AutoRebot)
            {
                VeviLog2.WriteIPLog("Reboot");
                ComputerShutDown.Reboot();
            }
            else
            {
                string iPFromInternet = "";
                try
                {
                    this.waitTime = Convert.ToInt32(this.textBox4.Text);
                    iPFromInternet = HttpWebResponseUtility.GetIPFromInternet();
                }
                catch (Exception)
                {
                }
                if (iPFromInternet != this.currentIp)
                {
                    this.currentIp = iPFromInternet;
                }
                else
                {
                    this.ipCount++;
                }
                if (this.ipCount >= this.waitTime)
                {
                    foreach (Process process in Process.GetProcesses())
                    {
                        if (process.ProcessName.ToLower() == this.textBox2.Text.Trim().ToLower())
                        {
                            process.Kill();
                        }
                    }
                    ADSLHelper.LinkAdsl("");
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if ((!string.IsNullOrEmpty(this.textBox1.Text.Trim()) && !string.IsNullOrEmpty(this.textBox2.Text.Trim())) && !Process.GetProcesses().Any<Process>(t => (t.ProcessName.ToLower() == this.textBox2.Text.Trim().ToLower())))
            {
                if (!string.IsNullOrEmpty(this.textBox3.Text.Trim()))
                {
                    Process.Start(this.textBox1.Text, this.textBox3.Text);
                }
                else
                {
                    Process.Start(this.textBox1.Text);
                }
                this.restartCount++;
            }
        }

       
    }
}
