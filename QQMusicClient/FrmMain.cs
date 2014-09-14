using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.WindowsAPI;
using mshtml;

namespace QQMusicClient
{
    
    public partial class FrmMain : Form
    {
        OperateCore core = new OperateCore();
        System.Windows.Forms.Timer idTimer=new Timer();
        public FrmMain()
        {
            InitializeComponent();
            core.ShowInStatusBarEvent += core_ShowInStatusBarEvent;
            core.GetDownLoadIDCOdeEvent += core_GetDownLoadIDCOdeEvent;
            idTimer.Interval = 10*1000;
            idTimer.Tick += idTimer_Tick;
            idTimer.Start();
        }

        void idTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine(" Main Timer");
            if(core!=null)
                core.JudgeDownLoadIDCodeAndInput();
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
                this.Invoke(new ShowInStatusBar(ShowTaskInfo),text);
            }
            else toolStripStatusLabel1.Text = text;
        }
        public void ShowTaskInfo(string text)
        {
            toolStripStatusLabel1.Text = text;
            statusStrip1.Refresh();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = true;
            //
            core.DoWork();
            //
            //core.StartMonitor_T();
            //core.StartDownLoadTimer();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button2.Enabled = false;
            button1.Enabled = true;
            core.Stop();
        }

        private void button3_Click(object sender, EventArgs e)
        {
           if( new FrmSetting().ShowDialog()==DialogResult.OK)
               AppConfig.ReadValue();
        }
    }
}
