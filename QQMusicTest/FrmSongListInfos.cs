using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using QQMusicHelper.GetInfo;
using Vevisoft.Excel.Core;

namespace QQMusicTest
{
    public partial class FrmSongListInfos : Form
    {
        public FrmSongListInfos()
        {
            InitializeComponent();
            InitData();
        }

        private void InitData()
        {
            var dt = new DataTable();
            var model = new QQMusicHelper.GetInfo.SongListInfo();
            Type typeModel = model.GetType();
            foreach (PropertyInfo propertyInfo in typeModel.GetProperties())
            {
                var column = new DataColumn(propertyInfo.Name) {DataType = propertyInfo.PropertyType};
                dt.Columns.Add(column);
            }
            dataGridView1.DataSource = dt;
        }

        private void AnalyseSongList(string jsonText)
        {
            var dt = dataGridView1.DataSource as DataTable;
            if (dt == null)
                return;
            var modelList = QQMusicHelper.GetInfo.SongListUtility.GetAllSongOrderInfos(jsonText);
            foreach (SongListInfo model in modelList)
            {
                dt.TableName = model.orderName;
                var newrow = dt.NewRow();
                Type p = model.GetType();
                foreach (PropertyInfo propertyInfo in p.GetProperties())
                {
                    if (dt.Columns.Contains(propertyInfo.Name))
                       newrow[propertyInfo.Name] = propertyInfo.GetValue(model, null);
                        
                }
                dt.Rows.Add(newrow);
            }
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var diag = MessageBox.Show("是否清楚所有数据?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (diag != DialogResult.Yes)
                return;
            var dt = dataGridView1.DataSource as DataTable;
            if(dt!=null)
                dt.Rows.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(richTextBox1.Text.Trim()))
            {
                AnalyseSongList(richTextBox1.Text);
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var dt = dataGridView1.DataSource as DataTable;
            if (dt == null)
                return;
            var sdiag=new SaveFileDialog {Filter = "Excel文件*.xlsx|*.xlsx"};
            if (sdiag.ShowDialog() == DialogResult.OK)
            {
                var core = new ExportExcelCore();
                core.RenderDataTableToExcel(sdiag.FileName, dt, "", null);    
            }
            
        }
    }
}
