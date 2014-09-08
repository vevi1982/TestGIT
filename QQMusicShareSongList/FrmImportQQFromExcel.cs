using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QQMusicShareSongList
{
    public partial class FrmImportQQFromExcel : Form
    {
        public FrmImportQQFromExcel()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var opdiag=new OpenFileDialog();
            opdiag.Filter = "Xls文件|*.xls";
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = opdiag.FileName;
                var dt = Vevisoft.Office.ExcelHelper.ExcelToDataSet(opdiag.FileName, "sheet1").Tables[0];
                 qqdicList = new Dictionary<string, string>();
                foreach (DataRow row in dt.Rows)
                    AnaExcelQQ(row[0].ToString());
                var cout = 0;

                foreach (var key in qqdicList.Keys)
                    richTextBox1.AppendText(string.Format("{0}  {1}  {2}",cout,key,qqdicList[key]));
            }

        }


        Dictionary<string,string> qqdicList=new Dictionary<string, string>();
        private void AnaExcelQQ(string excelrow)
        {
           
            var count = 0;
            var list = excelrow.Split('-',' ');
            var qq = "";
            var qqpass = "";
            foreach (string s in list)
            {
                if (string.IsNullOrEmpty(s))
                    continue;
                if (count == 0)
                {
                    
                    qq = s;
                    count++;
                }else if (count == 1)
                {
                    qqpass = s;
                    break;
                }
            }
            //qq
            if (qqdicList.ContainsKey(qq))
                return;
            qqdicList.Add(qq,qqpass);
        }

        public void AddTODB()
        {
            //QQBusiness bus=new QQBusiness();
            //foreach (string  key in qqdicList.Keys)
            //{
            //    if (!bus.ExistQQ(key))
            //        bus.InsertQQList(key, qqdicList[key]);
            //}
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //AddTODB();
            //DialogResult = DialogResult.OK;
        }
    }
}
