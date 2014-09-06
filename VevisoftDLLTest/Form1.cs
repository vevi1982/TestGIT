using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VevisoftDLLTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            new ADSL.FrmDialup().Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var opdiag=new OpenFileDialog();
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = opdiag.FileName;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox1.Text.Trim()))
            {
                Process.Start(textBox1.Text.Trim());
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            new HOOK.FrmMouseHook().Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            new IdentifyingCode.FrmCode().Show();
        }
    }
}
