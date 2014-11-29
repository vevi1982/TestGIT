using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.WindowsAPI;

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
            SystemWindowsAPI.SetPassWordEditValue(handle, text);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new FrmSongListInfos().Show();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            new FrmSendSongDataToServer().Show();
        }
    }
}
