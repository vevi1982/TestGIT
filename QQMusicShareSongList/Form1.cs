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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            InitQQList();
        }

        private void InitQQList()
        {
            dataGridView1.AutoGenerateColumns = false;
            //var dt = new QQBusiness().GetQQList("");
            //dataGridView1.DataSource = dt;
            dtSource = CreateDatatable();
            dataGridView1.DataSource = dtSource;
        }

        /// <summary>
        /// 获取歌单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 共享歌单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {

        }

        private DataTable dtSource;
        private DataTable CreateDatatable()
        {
            var dt = new DataTable();
            dt.Columns.Add("QQNo");
            dt.Columns.Add("QQPass");
            return dt;
        }
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var opdiag = new OpenFileDialog();
            opdiag.Filter = "Xls文件|*.xls";
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                var dt = Vevisoft.Office.ExcelHelper.ExcelToDataSet(opdiag.FileName, "sheet1").Tables[0];
                qqdicList = new Dictionary<string, string>();
                foreach (DataRow row in dt.Rows)
                    AnaExcelQQ(row[0].ToString());
                var cout = 0;
                string str = "";
                foreach (var key in qqdicList.Keys)
                {
                    cout++;
                    richTextBox1.AppendText(string.Format("{0}  {1}  {2}\r\n", cout, key, qqdicList[key]));
                    var row = dtSource.NewRow();
                    row[0] = key;
                    row[1] = qqdicList[key];
                    dtSource.Rows.Add(row);
                    if (string.IsNullOrEmpty(str))
                        str = key + "(" + key + ")";
                    else str += string.Format(";{0}({0})", key);
                }
                richTextBox1.AppendText(str);
            }
        }

        Dictionary<string, string> qqdicList = new Dictionary<string, string>();
        private void AnaExcelQQ(string excelrow)
        {

            var count = 0;
            var list = excelrow.Split('-', ' ');
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
                }
                else if (count == 1)
                {
                    qqpass = s;
                    break;
                }
            }
            //qq
            if (qqdicList.ContainsKey(qq))
                return;
            qqdicList.Add(qq, qqpass);
        }
    }
}
