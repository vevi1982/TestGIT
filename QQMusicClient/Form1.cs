using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QQMusicClient.Dlls;
using QQMusicClient.Models;
using Vevisoft.WindowsAPI;

namespace QQMusicClient
{
    public partial class Form1 : Form
    {
        OperateCore core = new OperateCore();
        public Form1()
        {
            InitializeComponent();
            core.Server=new ServerToInternet();
            core.ShowInStatusBarEvent += core_ShowInStatusBarEvent;
            //
            new ServerToInternet().GetQQFromServer();
        }

        void core_ShowInStatusBarEvent(string text)
        {
            toolStripStatusLabel1.Text = text;
            statusStrip1.Refresh();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            new FrmSetting().ShowDialog();
        }

       
        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                //core.DoOnce();
                //core.StartDownLoadTimer();
            }
            catch (Exception e1)
            {
                if(e1.Message==OperateCore.QQDownLoadOverLimit||e1.Message==OperateCore.QQPassErrorMsg)
                    btnStart_Click(null,null);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            try
            {
                core.StartDownLoadTimer();
            }
            catch (Exception e1)
            {
                if (e1.Message == OperateCore.QQDownLoadOverLimit || e1.Message == OperateCore.QQPassErrorMsg)
                    btnStart_Click(null, null);
            }
        }

        private IntPtr mainHandle = IntPtr.Zero;
        /// <summary>
        /// 启动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            mainHandle = core.StartApp();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetMainHandle();
            core.DeleteDownLoadList(mainHandle);
        }

        private void button3_Click(object sender, EventArgs e)
        {
           SetMainHandle();
            core.GetSongListHtml(mainHandle);
        }

        private void button6_Click(object sender, EventArgs e)
        {
           SetMainHandle();
            var songlistname = "1-xin";
            core.isContainsSOngListAndClick(songlistname);
        }


        private void SetMainHandle()
        {
            if (mainHandle == IntPtr.Zero)
                mainHandle = new IntPtr(int.Parse(textBox1.Text));
        }

        private void button9_Click(object sender, EventArgs e)
        {
            SetMainHandle();
            core.DeleteTrySongList(mainHandle);
            core.DeleteDownLoadList(mainHandle);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            SetMainHandle();
            core.DownLoadSongsBySongListName(mainHandle,"新建歌单");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            SetMainHandle();
            core.LoginQQ(mainHandle,new QQInfo(){QQNo="254430994",QQPass="52182467391"});
        }

        private void button10_Click(object sender, EventArgs e)
        {
            SetMainHandle();
            core.ClearALlInfos(mainHandle);
        }

    }
}
