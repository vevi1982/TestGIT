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
using Vevisoft.WindowsAPI.Hook;
using WINIOVirtualKey;

namespace QQMusicTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            QQMusicHelper.QQMusicOperateHelper.LogOutQQMusic();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var IEHandler =new IntPtr(Convert.ToInt32(textBox1.Text,16));
            if (IEHandler == IntPtr.Zero)
                return;
            var id = IEAPI.GetHtmlDocumentObject(IEHandler);
            if (id == null)
                return;
            var str = id.body.innerHTML;
            richTextBox1.Clear();
            richTextBox1.AppendText("Html:" + "\r\n");
            richTextBox1.AppendText(str + "\r\n");
            richTextBox1.AppendText("Cookies:"+"\r\n");
            richTextBox1.AppendText(id.cookie);
            //
            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var handle = new IntPtr(Convert.ToInt32(textBox2.Text, 16));
            int x = Convert.ToInt32(textBox3.Text);
            int y = Convert.ToInt32(textBox4.Text);
            //
            MouseKeyBoradUtility.MouseLeftClickPostMsg(handle, x, y);
            
        }

        private void button5_Click(object sender, EventArgs e)
        {
            var handle = new IntPtr(Convert.ToInt32(textBox2.Text, 16));
            int x = Convert.ToInt32(textBox3.Text);
            int y = Convert.ToInt32(textBox4.Text);
            //
            MouseKeyBoradUtility.MouseRightClickPostMsg(handle, x, y);
        }
        /// <summary>
        /// SetText
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button6_Click(object sender, EventArgs e)
        {
            var handle = new IntPtr(Convert.ToInt32(textBox5.Text, 16));
            var text = textBox6.Text;
            //
            MouseKeyBoradUtility.InputSendKeyMessage(handle,text);
            //SystemWindowsAPI.SetPassWordEditValue(handle, text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new FrmSongListInfos().Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new FrmSendSongDataToServer().Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox7.Focus();
            Thread.Sleep(1*1000);
            var winio=new WINIOUtility();
            if (winio.Initialize())
                winio.InputKeyValues(textBox8.Text);
            else MessageBox.Show("WINIO初始化失败!");
            winio.TestA();
            //
            winio.Shutdown();
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox8_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            SendKeys.Send("NUMLOCK");
            var winio = new WINIOUtility();
            if (winio.Initialize())
                winio.NumLockKeyClick();
            else MessageBox.Show("WINIO初始化失败!");
        }

        private void textBox7_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var winio = new WINIOUtility();

            if (!winio.Initialize())
               MessageBox.Show("WINIO初始化失败!");
            winio.TestA();
            winio.Shutdown();
        }

        private DebugHook keyhook;
        private void button11_Click(object sender, EventArgs e)
        {
            //var handle = new IntPtr(Convert.ToInt32(textBox5.Text, 16));
            var text = textBox6.Text;
            keyhook = new DebugHook();
            keyhook.StartHook();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            keyhook.UnHook();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            //Thread.Sleep(2*1000);
            var text = textBox6.Text;
            
            //var handle = new IntPtr(Convert.ToInt32(textBox5.Text, 16));
            //MouseKeyBoradUtility.InputStringSendMessage(handle, text);
            var handle = QQMusicHelper.QQMusicOperateHelper.GetLoginForm();
            var editHandle = QQMusicHelper.QQMusicOperateHelper.FindQQLoginPassEditHandle(handle);
            MouseKeyBoradUtility.InputStringSendMessage(editHandle, text);
        }

        private void button14_Click(object sender, EventArgs e)
        {
            QQMusicHelper.QQMusicPlaySongControl.StartPlay();
        }

        private void button15_Click(object sender, EventArgs e)
        {
            QQMusicHelper.QQMusicPlaySongControl.PlayEndUserSet();
        }
    }
}
