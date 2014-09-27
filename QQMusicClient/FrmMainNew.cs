using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QQMusicClient.Dlls;

namespace QQMusicClient
{
    public partial class FrmMainNew : Form
    {
        private OperateCoreNew core;
        Timer idTimer = new Timer();
        public FrmMainNew()
        {
            InitializeComponent();
            core=new OperateCoreNew();
            core.ShowDownLoadInfo += core_ShowDownLoadInfo;
            core.ShowStepEvent += core_ShowStepEvent;
            core.ShowErrorEvent += core_ShowErrorEvent;
            core.ShowHeartEvent += core_ShowHeartEvent;
            //
            idTimer.Interval = 10 * 1000;
            idTimer.Tick += idTimer_Tick;
        }

        private int timerCount = 0;
        private void idTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Main Timer Monitor");
            try
            {

                //发送心跳，两分钟
                if (timerCount >= 2 * 60 * 1000 / idTimer.Interval)
                {
                    if (core != null)
                        core.SendHeartToServer();
                    timerCount = 0;
                }
                else
                    timerCount++;
                //2个周期内下载数没有变化，那么从新开始下载
                //if (core != null )//&& ((OperateCore.SendHeartFailedCount > 0) /*||!core.GetMainResponseByProcess()*/))
                //{
                //    //button2_Click(null, null);

                //    //button1_Click(null, null);
                //    ////
                //    //return;
                //}

                //判断下载是否完成
                //if (core != null)
                //    core.DownLoadInfoMonitor();
                //10秒判断一次
                try
                {
                    QQMusicHelper.DownLoadIDCodeHelper.GetDownLoadIdCodeFormAndInputIdCode();
                }
                catch (Exception e1)
                {
                    ShowTaskInfo2(e1.Message);
                }
                
            }
            catch (Exception e1)
            {
                ShowTaskInfo2("监视器:" + e1.Message);
            }
        }

        public FrmMainNew(bool auto)
            : this()
        {

            button1_Click(null, null);    
        }
        #region Log显示
        void core_ShowHeartEvent(string text)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.BeginInvoke(new ShowInStatusBar(ShowTaskInfo4), text);
            }
            else toolStripStatusLabel3.Text = text;
        }

        private void ShowTaskInfo4(string text)
        {
            toolStripStatusLabel3.Text = text;
        }

        void core_ShowErrorEvent(string text)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.BeginInvoke(new ShowInStatusBar(ShowTaskInfo2), text);
            }
            else toolStripStatusLabel2.Text = text;
        }

        private void ShowTaskInfo2(string text)
        {
            toolStripStatusLabel2.Text = text;
        }

        void core_ShowStepEvent(string text)
        {
            if (statusStrip1.InvokeRequired)
            {
                statusStrip1.BeginInvoke(new ShowInStatusBar(ShowTaskInfo1), text);
            }
            else toolStripStatusLabel1.Text = text;
        }

        private void ShowTaskInfo1(string text)
        {
            toolStripStatusLabel1.Text = text;
        }

        void core_ShowDownLoadInfo(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new ShowInStatusBar(ShowTaskInfo), text);
            }
            else richTextBox1.Text = text;
        }

        private void ShowTaskInfo(string text)
        {
            richTextBox1.Text = text;
        }
        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            core.StartWorkThread();
            idTimer.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            
            //
            core.AbortMainThread();
            idTimer.Stop();
            //
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            core.StopMainThread();
            button1.Enabled = true;
            button2.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text.Trim()))
                return;
            var s = new TencentServer();
            var model = new Models.QQInfo {QQNo = textBox1.Text};
            MessageBox.Show(TencentServer.GetDownLoadInfoStrFromTencentServer(model, ""));
        }
    }
}
