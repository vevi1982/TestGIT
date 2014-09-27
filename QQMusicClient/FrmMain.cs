using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QQMusicClient.Dlls;
using Vevisoft.WindowsAPI;
using mshtml;
using Timer = System.Windows.Forms.Timer;

namespace QQMusicClient
{

    public partial class FrmMain : Form
    {
        OperateCore core = new OperateCore();
        Timer idTimer = new Timer();
        public FrmMain()
        {
            InitializeComponent();
            core.ShowInStatusBarEvent += core_ShowInStatusBarEvent;
            core.ShowInStatusMonitor += core_ShowInStatusMonitor;
            core.GetDownLoadIDCOdeEvent += core_GetDownLoadIDCOdeEvent;
            core.ShowDownLoadInfo += core_ShowDownLoadInfo;

            core.ShowLog += core_ShowLog;
            idTimer.Interval = 10 * 1000;
            idTimer.Tick += idTimer_Tick;

        }


        public FrmMain(bool auto):this()
        {

            button1_Click(null, null);    
        }

        void core_ShowLog(string text)
        {
            if (label1.InvokeRequired)
            {
                this.BeginInvoke(new ShowInStatusBar(ShowTaskInfo4), text);
            }
            else label1.Text = text;
        }

        private void ShowTaskInfo4(string text)
        {
            label1.Text = text;
        }

        void core_ShowDownLoadInfo(string text)
        {
            if (richTextBox1.InvokeRequired)
            {
                this.BeginInvoke(new ShowInStatusBar(ShowTaskInfo3), text);
            }
            else richTextBox1.Text = text;
        }

        private void ShowTaskInfo3(string text)
        {
            richTextBox1.Text = text;
        }

        void core_ShowInStatusMonitor(string text)
        {
            if (statusStrip1.InvokeRequired)
            {
                this.BeginInvoke(new ShowInStatusBar(ShowTaskInfo2), text);
            }
            else toolStripStatusLabel4.Text = text;
        }

        private void ShowTaskInfo2(string text)
        {
            toolStripStatusLabel4.Text = text;
            statusStrip1.Refresh();
        }

        private int timerCount = 0;
        /// <summary>
        /// 四个职责：
        /// 1.发送心跳
        /// 2.判断是否下载完成
        /// 3.填写下载验证码
        /// 4.4分钟内没有变化，那么重新开始下载流程
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void idTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Main Timer Monitor");
            try
            {
                
                //发送心跳，两分钟
                if (timerCount >= 2*60*1000/idTimer.Interval)
                {
                    if (core != null)
                        core.SendHeart(AppConfig.PCName);
                    timerCount = 0;
                }
                else
                    timerCount++;
                //2个周期内下载数没有变化，那么从新开始下载
                if (core != null && ((OperateCore.SendHeartFailedCount > 0) /*||!core.GetMainResponseByProcess()*/))
                {
                    //button2_Click(null, null);

                    //button1_Click(null, null);
                    ////
                    //return;
                }

                //判断下载是否完成
                if (core != null)
                    core.DownLoadInfoMonitor();
                //10秒判断一次
                if (core != null)
                    core.JudgeDownLoadIDCodeAndInput();
            }
            catch (Exception e1)
            {
                ShowTaskInfo("监视器:"+e1.Message);
            }
            
        }
        #region 获取验证码
        public mshtml.IHTMLDocument2 GetHtmlDocument(IntPtr hwnd)
        {
            var domObject = new System.Object();
            int tempInt = 0;
            var guidIEDocument2 = new Guid();
            var WM_Html_GETOBJECT = SystemWindowsAPI.RegisterWindowMessage("WM_Html_GETOBJECT");//定义一个新的窗口消息
            int W = SystemWindowsAPI.SendMessage(hwnd, WM_Html_GETOBJECT, 0, ref tempInt);//注:第二个参数是RegisterWindowMessage函数的返回值
            int lreturn = SystemWindowsAPI.ObjectFromLresult(W, ref guidIEDocument2, 0, ref domObject);
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)domObject;
            return doc;
        }
        string core_GetDownLoadIDCOdeEvent(IntPtr didcIeHandle)
        {
            mshtml.IHTMLDocument2 id = GetHtmlDocument(didcIeHandle);
            if (id == null)
                return "";
            core_ShowInStatusBarEvent("获取验证码图片");
            IHTMLControlElement img =
                id.images.Cast<IHTMLElement>().Where(item => item.id == "imgVerify").Cast<IHTMLControlElement>().FirstOrDefault();
            if (img != null)
            {
                IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)id.body).createControlRange();
                range.add(img);
                bool bol1 = range.execCommand("Copy", false, null);
                if (Clipboard.ContainsImage())
                {
                    core_ShowInStatusBarEvent("保存验证码图片");
                    Clipboard.GetImage().Save(@"c:\bb.bmp");
                    return Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCodeWeb(@"c:\bb.bmp", 1014);
                }
            }
            return "";
        }
        #endregion


        void core_ShowInStatusBarEvent(string text)
        {
            if (statusStrip1.InvokeRequired)
            {
                this.BeginInvoke(new ShowInStatusBar(ShowTaskInfo), text);
            }
            else toolStripStatusLabel1.Text = text;
        }
        public void ShowTaskInfo(string text)
        {
            toolStripStatusLabel1.Text = text;
            //statusStrip1.Refresh();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            //Thread t=new Thread(() =>
            //    {
            //        while (true)
            //        {
            //            Thread.Sleep(100);
            //        }
            //    });
           
            //t.Start();
            //t.Join();
            button1.Enabled = false;
            button2.Enabled = true;
            button4.Enabled = true;
            //
            timerCount = 0;
            OperateCore.SendHeartFailedCount = 0;
            core.DoWork();
            idTimer.Start();

            core.StartMonitor_T();
            core.StartDownLoadTimer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            core.Stop();
            idTimer.Stop();
            //while (core.WorkThreadIsALive)
            //{
            Thread.Sleep(3000);
            
            //}
            button2.Enabled = false;
            button1.Enabled = true;
            button4.Enabled = false;

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (new FrmSetting().ShowDialog() == DialogResult.OK)
                AppConfig.ReadValue();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (core != null)
                core.WorkThreadFlag = false;
            button4.Enabled = false;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(textBox1.Text.Trim()))
                return;
            var s=new TencentServer();
            var model = new Models.QQInfo();
            model.QQNo = textBox1.Text;
            MessageBox.Show(TencentServer.GetDownLoadInfoStrFromTencentServer(model, ""));
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            
        }
    }
}
