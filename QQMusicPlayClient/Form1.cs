﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QQMusicPlayClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            SetLocation(this);
            InitCoreObj();
        }
        /// <summary>
        /// 设置启动自动放在左下角
        /// </summary>
        /// <param name="frm"></param>
        private void SetLocation(Form frm)
        {
            Rectangle ScreenArea = System.Windows.Forms.Screen.GetWorkingArea(this);
            //左下角显示
            frm.StartPosition = FormStartPosition.Manual;
            frm.Location = new Point(0, ScreenArea.Height - frm.Height);
        }

        private void InitCoreObj()
        {
            if (_core == null)
            {
                _core=new OperateCore();
                //
                _core.ShowStepEvent += core_ShowInStatusBarEvent;
                _core.ShowHeartEvent += core_ShowInStatusMonitor;
                _core.ShowErrorEvent += core_ShowInStatusMonitor;
            }
        }
        void core_ShowInStatusMonitor(string text)
        {
            if (statusStrip1.InvokeRequired)
            {
                this.BeginInvoke(new ShowInStatusBar(ShowTaskInfo2), text);
            }
            else toolStripStatusLabel3.Text = text;
        }

        private void ShowTaskInfo2(string text)
        {
            toolStripStatusLabel3.Text = text;
            statusStrip1.Refresh();
        }

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

        private void btnSet_Click(object sender, EventArgs e)
        {
            new FrmSetting().ShowDialog();
        }

        private OperateCore _core;
        private void btnStart_Click(object sender, EventArgs e)
        {
            if(_core==null)
                _core=new OperateCore();
            _core.StartWork();
            //
            btnStart.Enabled = false;
            btnStop.Enabled = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            if(_core!=null)
                _core.StopWork();
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(_core!=null)
                _core.ChangeIp();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var str = WebUtility.GetWordDateTime().ToString("yyyy/MM/dd hh:mm:ss");
            MessageBox.Show(str);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Vevisoft.WindowsAPI.PCTimeUtility.SetSysTime(AppConfig.StartTime);

        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(WebUtility.GetMd5Encrypt("cp01qqmusic"));
        }

        private void button5_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(AppConfig.AppPath);
        }

        private void button6_Click(object sender, EventArgs e)
        {
           var process= System.Diagnostics.Process.Start(AppConfig.AppPath,"");
            if (process == null)
                core_ShowInStatusMonitor("线程为空！");
           while (!process.Responding)
           {
               Thread.Sleep(1000);
               core_ShowInStatusMonitor("线程没反应！");
           }
           core_ShowInStatusMonitor("线程有了反应！");
            //
           var count = 10;

           while (GetProcessCount("QQMusicExternal") < 2 && count > 0)
           {
               count--;
               Thread.Sleep(1000);
               core_ShowInStatusMonitor("启动没有完成！");
           }
          
           if (count <= 0)
               core_ShowInStatusMonitor("启动没有完成！");
           else core_ShowInStatusMonitor("启动完成！");
        }
        private static int GetProcessCount(string processName)
        {
            return System.Diagnostics.Process.GetProcesses().Count(process => process.ProcessName == processName);
        }
    }
}
