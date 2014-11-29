using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace AutoStartApp
{
    public partial class FrmMusicMonitor : Form
    {
        Timer timer=new Timer();
        public FrmMusicMonitor()
        {
            InitializeComponent();
            button1_Click(null,null);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            timer=new Timer {Interval = int.Parse(textBox1.Text)};
            timer.Tick += timer_Tick;
            timer.Start();
        }

        private bool respons = true;
        void timer_Tick(object sender, EventArgs e)
        {
            bool bol1 = GetProcessIsResponsing();
            if (!bol1)
            {
                if (!respons)
                {
                    respons = true;
                    KillProcess();
                }
                else respons = bol1;
            }
            else respons = bol1;
            //
            if (!GetQQMusicClientExist())
            {
                //重新启动下载程序
                System.Diagnostics.Process.Start(APPConfig.App1Path, "auto");
            }
            //
            QQMusicHelper.QQMusicWindowUtility.GetQQMusicForm();
            if (QQMusicHelper.QQMusicWindowUtility.LoginHandleChange )
            {
                Vevisoft.Log.VeviLog2.WriteLogInfo("Login Form has Changed "+QQMusicHelper.QQMusicWindowUtility.LoginFormHandle);
            }
            if (QQMusicHelper.QQMusicWindowUtility.MainHandleChange)
            {
                Vevisoft.Log.VeviLog2.WriteLogInfo("Main Form has Changed " + QQMusicHelper.QQMusicWindowUtility.MainFormHandle);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer.Stop();
            button1.Enabled = true;
            button2.Enabled = false;
        }
        private bool GetQQMusicClientExist()
        {
            var title = "QQMusicClient";
            foreach (var process in System.Diagnostics.Process.GetProcesses())
            {
                if (process.ProcessName == title)
                {
                    return true;
                }
            }
            return false;
        }
        private bool GetProcessIsResponsing()
        {
            foreach (var process in System.Diagnostics.Process.GetProcesses())
            {
                if (process.ProcessName == "QQMusic")
                {
                    return process.Responding;
                }
            }
            //没有找到此线程，可能是尚未启动，返回true
            return true;
        }


        private void KillProcess()
        {
            foreach (var process in System.Diagnostics.Process.GetProcesses())
            {
                if (process.ProcessName == "QQMusic")
                {
                   process.Kill();
                    process.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var diag = MessageBox.Show("是否重新启动计算机？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (diag != DialogResult.Yes)
                return;
            Vevisoft.WindowsAPI.ComputerShutDown.Reboot();
        }
    }
}
