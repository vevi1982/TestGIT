using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VeviSoft.Controls
{
    public partial class Form1 : Form
    {
        private GridControlVs gridControlVs1;
        public Form1()
        {
            InitializeComponent();
            //
            gridControlVs1=new GridControlVs {Dock = DockStyle.Fill};
            this.Controls.Add(gridControlVs1);
            //
            //gridControlVs1.DefaultGridView.CustomDrawRowIndicator += gridControlVs1.gridview_CustomDrawRowIndicator;

            //var dt = new DataTable();
            //dt.Columns.Add("C1");
            //dt.Columns.Add("C2");
            //dt.Columns.Add("C3");
            //dt.Columns.Add("C4");
            ////
            //for (int i = 0; i < 10000; i++)
            //{
            //    var row = dt.NewRow();
            //    row[0] = i;
            //    row[1] = i + 1;
            //    row[2] = i + 2;
            //    row[3] = i + 3;
            //    dt.Rows.Add(row);
            //}
            //gridControlVs1.DataSource = dt;
        }

      

        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControlVs1.ImportExcelData();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControlVs1.ExportExcelData();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControlVs1.DeleteItem();
        }
        
    }
}
