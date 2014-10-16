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
    }
}
