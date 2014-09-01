using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vevisoft.WindowsAPI;

namespace QQMusicClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSetting_Click(object sender, EventArgs e)
        {
            new FrmSetting().ShowDialog();
        }
        OperateCore core=new OperateCore();
        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Thread.Sleep(5000);
            var handle = core.GetMainForm();
            //SystemWindowsAPI.ShowWindow(handle, 1);
            var handle1 = SystemWindowsAPI.GetTopMostWindow(IntPtr.Zero);
            
            var handle2 = SystemWindowsAPI.GetActiveWindow();
            var handle3 = SystemWindowsAPI.GetTopMostWindow(IntPtr.Zero);
            var handle4 = SystemWindowsAPI.GetForegroundWindow();
            //
            var rect = core.GetFormRect(handle);
            var rect1 = core.GetFormRect(handle1);
            var rect2 = core.GetFormRect(handle2);
            var rect3 = core.GetFormRect(handle3);
            var rect4 = core.GetFormRect(handle4);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var mhook=new MouseHookUtility();
            mhook.OnMouseActivity += mhook_OnMouseActivity;
            mhook.Start();
        }

        void mhook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            textBox2.Text = e.Button.ToString() + "  " + e.X + "," + e.Y;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            core.DoOnce();
        }
    }
}
