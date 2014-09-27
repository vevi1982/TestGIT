using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VevisoftDLLTest
{
    public partial class FrmInputDLIDCode : Form
    {
        public FrmInputDLIDCode()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (QQMusicHelper.DownLoadIDCodeHelper.GetDownLoadIdCodeForm() != IntPtr.Zero)
            {
                MessageBox.Show("IDCodeForm,,,");
                //QQMusicHelper.DownLoadIDCodeHelper.GetDownLoadIdCodeFormAndInputIdCode();   
            }
            else
            {
                MessageBox.Show("IDCodeForm!!!,");
            }
            try
            {
                QQMusicHelper.DownLoadIDCodeHelper.GetDownLoadIdCodeFormAndInputIdCode();
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.Message);
            }
            
        }
    }
}
