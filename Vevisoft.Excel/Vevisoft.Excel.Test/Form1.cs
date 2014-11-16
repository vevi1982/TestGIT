using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.Excel.Core;

namespace Vevisoft.Excel.Test
{
    public partial class Form1 : Form
    {
        private readonly ExcelImportCore _importCore;
        public Form1()
        {
            InitializeComponent();
            _importCore = new ExcelImportCore();
            InitDgvData();
        }

        

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var opdiag=new OpenFileDialog();
            tabControl1.TabPages.Clear();
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                _importCore.LoadFile(opdiag.FileName);
                var ds = _importCore.GetAllTables(false);
                //
                for (int i = 0; i < _importCore.SheetNames.Count; i++)
                {
                    var tp=new TabPage {Text= Name = _importCore.SheetNames[i]};
                    tabControl1.TabPages.Add(tp);
                    //添加数据源
                    var dgv=new DataGridView
                        {
                            //AutoGenerateColumns = false,
                            DataSource = ds.Tables[i],
                            Dock = DockStyle.Fill
                        };
                    tp.Controls.Add(dgv);

                }
            }
        }

        private void InitDgvData()
        {
            var dt = new DataTable();
            dt.Columns.Add("C1");
            dt.Columns.Add("C2");
            dt.Columns.Add("C3");
            dt.Columns.Add("C4");
            //
            for (int i = 0; i < 10000; i++)
            {
                var row = dt.NewRow();
                row[0] = i;
                row[1] = i + 1;
                row[2] = i + 2;
                row[3] = i + 3;
                dt.Rows.Add(row);
            }
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = dt;

        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            var dt = dataGridView1.DataSource as DataTable;
            if (dt == null)
                return;
            //
            var exportCore = new ExportExcelCore();
            var sdiag = new SaveFileDialog { Filter = @"Excel File(*.xlsx;*.xls)|*.xlsx;*.xls" };
            if (sdiag.ShowDialog() == DialogResult.OK)
            {
                //exportCore.RenderDataTableToExcel(sdiag.FileName, dt, "Test Export Data To Excel", null);
                exportCore.RenderDataTableToExcelHasTemplate(sdiag.FileName, @"test.xlsx", dt, 3, 1);
            }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            Vevisoft.Excel.Core.ExcelHelper.ExportData();
        }
        
    }
}
