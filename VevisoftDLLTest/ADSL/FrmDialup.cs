using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vevisoft.WebOperate;

namespace VevisoftDLLTest.ADSL
{
    public partial class FrmDialup : Form
    {
        public FrmDialup()
        {
            InitializeComponent();
        }
        AdslDialHelper adslhelper = new AdslDialHelper();
        private void button1_Click(object sender, EventArgs e)
        {
            //
            var adslname = textBox1.Text.Trim();
            var username = textBox2.Text.Trim();
            var pass = textBox3.Text.Trim();
            //
            if (string.IsNullOrEmpty(adslname) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("请填写内容!");
                return;
            }
           
            int count = 10;
            //start dial
            if (checkBox1.Checked)
            {
                adslhelper.StopDailer(adslname);
                while (!adslhelper.IsConnectedInternet()&&count>0)
                {
                    adslhelper.StartDailer(adslname, username, pass);
                    Thread.Sleep(500);
                    richTextBox1.AppendText(Vevisoft.Utility.Web.HttpResponseUtility.GetIPFromBaidu()+"\r\n");
                    adslhelper.StopDailer(adslname);
                    count--;
                }
            }
            else
            {
                adslhelper.StartDailer(adslname, username, pass);
                Thread.Sleep(500);
                richTextBox1.AppendText(Vevisoft.Utility.Web.HttpResponseUtility.GetIPFromBaidu() + "\r\n");
                   
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (adslhelper.IsConnectedInternet())
            {
                MessageBox.Show("yes!");
            }
            else MessageBox.Show("no!");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            adslhelper.StopDailer(textBox1.Text.Trim());
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //
            var adslname = textBox1.Text.Trim();
            var username = textBox2.Text.Trim();
            var pass = textBox3.Text.Trim();
            //
            if (string.IsNullOrEmpty(adslname) ||
                string.IsNullOrEmpty(username) ||
                string.IsNullOrEmpty(pass))
            {
                MessageBox.Show("请填写内容!");
                return;
            }
            adslhelper.StartDailer(adslname, username, pass);
        }
    }
}
