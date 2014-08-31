using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            this.Visible = false;
            core.DoOnce();
            label1.Text = core.Msg;
            this.Visible = true;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            long handleValue = Convert.ToInt64(textBox1.Text.Trim());
            core.SHowFormRectByHandle(handleValue);
            label1.Text = core.Msg;
        }
    }
}
