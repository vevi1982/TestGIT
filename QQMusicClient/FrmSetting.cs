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
    public partial class FrmSetting : Form
    {
        public FrmSetting()
        {
            InitializeComponent();
            //
            LoadData();
        }
        private void LoadData()
        {
            txtPcName.Text = AppConfig.PCName;
            checkBox1.Checked = AppConfig.ChangeIP;
            //
            txtPath.Text = AppConfig.AppPath;
            //
            txtAdslName.Text = AppConfig.ADSLName;
            txtUserName.Text = AppConfig.ADSLUserName;
            txtAdslPass.Text = AppConfig.ADSLPass;

        }
        private void SaveData()
        {
            AppConfig.PCName = txtPcName.Text.Trim();
            AppConfig.ChangeIP = checkBox1.Checked;
            AppConfig.AppPath = txtPath.Text.Trim();
            AppConfig.ADSLName = txtAdslName.Text.Trim();
            AppConfig.ADSLUserName = txtUserName.Text.Trim();
            AppConfig.ADSLPass = txtAdslPass.Text.Trim();
            //
            AppConfig.SaveValue();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveData();
            DialogResult = DialogResult.OK;
        }
        /// <summary>
        /// 浏览文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
