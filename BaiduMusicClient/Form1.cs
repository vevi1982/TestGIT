using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BaiduMusicClient.Controler;
using Vevisoft.Utility.Web;
using Vevisoft.WebOperate;

namespace BaiduMusicClient
{
    public partial class Form1 : Form
    { // Fields
        private IWorkFactory _fac;
        // Methods
        public Form1()
        {
            this.InitializeComponent();
        }

        public Form1(bool auto)
            : this()
        {
            this.button1_Click(null, null);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = false;
            this.button2.Enabled = true;
            try
            {
                this._fac = FatoryControler.CreateFactory("");
                this._fac.StartWork();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.button1.Enabled = true;
            this.button2.Enabled = false;
            if (this._fac != null)
            {
                this._fac.StopWork();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            AdslUtility utility = new AdslUtility("宽带连接");
            try
            {
                utility.ReConnect(1);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            MessageBox.Show(HttpWebResponseUtility.GetIPFromInternet());
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void button6_Click(object sender, EventArgs e)
        {
        }

        


    }
}
