using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QQMusicPlayClient
{
    public partial class FrmSetting : Form
    {
        public FrmSetting()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            txtPcName.Text = AppConfig.PCName;
            checkBox1.Checked = AppConfig.ChangeIP;
            txtPath.Text = AppConfig.AppPath;
            txtAdslName.Text = AppConfig.ADSLName;
            txtUserName.Text = AppConfig.ADSLUserName;
            txtAdslPass.Text = AppConfig.ADSLPass;
            txtBClk.Text = AppConfig.BeforeClk + "";
            txtAClk.Text = AppConfig.AfterClk + "";
            txtPlaySongCount.Text = AppConfig.PlaySongNo + "";
            //
            txtStartTime.Text = AppConfig.ConvertDateToString(AppConfig.StartTime);
            txtPlayTime.Text = AppConfig.ConvertTimeToString(AppConfig.PlayTime);
            //
            txtQQNo.Text = AppConfig.QQNO;
            txtQQPass.Text = AppConfig.QQPass;
            //
        }

        private void WriteData()
        {
            AppConfig.PCName = txtPcName.Text;
            AppConfig.ChangeIP = checkBox1.Checked;
            AppConfig.AppPath = txtPath.Text;
            AppConfig.ADSLName = txtAdslName.Text;
            AppConfig.ADSLUserName = txtUserName.Text;
            AppConfig.ADSLPass = txtAdslPass.Text;
            AppConfig.BeforeClk = Convert.ToInt32(txtBClk.Text);
            AppConfig.AfterClk = Convert.ToInt32(txtAClk.Text);
            AppConfig.PlaySongNo = Convert.ToInt32(txtPlaySongCount.Text);
            //
            if(!string.IsNullOrEmpty(txtStartTime.Text.Trim()))
            AppConfig.StartTime = AppConfig.GetDateTimeFromString(txtStartTime.Text);
            if (!string.IsNullOrEmpty(txtPlayTime.Text.Trim()))
            AppConfig.PlayTime = AppConfig.GetSecondFromTimeString(txtPlayTime.Text);
            //
            AppConfig.QQNO = txtQQNo.Text;
            AppConfig.QQPass = txtQQPass.Text;
            //
            AppConfig.SaveValue();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WriteData();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var opdiag = new OpenFileDialog();
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = opdiag.FileName;
            }
        }
    }
}
