using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QQMusicTest
{
    public partial class FrmSendSongDataToServer : Form
    {
        public FrmSendSongDataToServer()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var fdiag=new FolderBrowserDialog();
            if (fdiag.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fdiag.SelectedPath;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                var str = QQMusicHelper.QQMusicOperateHelper.GetPostSongData(textBox1.Text, textBox2.Text.Trim());
                richTextBox1.Text = str;
            }
            

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            richTextBox2.Text = QQMusicHelper.QQMusicOperateHelper.SendSongDataToServer(richTextBox1.Text);
        }
    }
}
