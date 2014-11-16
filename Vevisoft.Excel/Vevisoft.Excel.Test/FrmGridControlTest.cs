using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using VeviSoft.Controls;
using Vevisoft.Theme;

namespace Vevisoft.Excel.Test
{
    public partial class FrmGridControlTest : Form
    {
        public FrmGridControlTest()
        {
            InitializeComponent();
            //
            //gridControlVs2 = new GridControlVs { Dock = DockStyle.Fill };
            //this.Controls.Add(gridControlVs2);
            var gop = new GridViewOperate_BaseData(gridControlVs2, "QCMLB");// {BindingControl = gridControlVs2};
            gridControlVs2.GridOperate = gop;
            //
            gop.InitData();
        }
        

        public void InitData()
        {
            var sql = "select * from temp";
            var dt= DBUtility.DbHelperSQL.Query(sql).Tables[0];
            dt.Columns.Add("aa");
            dt.Columns.Remove("C2");
            gridControlVs2.GridOperate.DataSource = dt;
        }
        private void SaveToDB()
        {
            DBUtility.DbHelperSQL.UpdateDataTable("temp", gridControlVs2.DataSource as DataTable);
        }
        private void barButtonItem1_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControlVs2.GridOperate.ImportExcelData();
        }

        private void barButtonItem2_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControlVs2.GridOperate.ExportExcelData();
        }

        private void barButtonItem3_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e)
        {
            gridControlVs2.GridOperate.DeleteItem();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            InitData();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            SaveToDB();
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            //xml文档写入测试
            var showColList = new Dictionary<string, string>();
            var importColList = new Dictionary<string, string>();
            showColList.Add("C1","水电费");
            importColList.Add("C1", "水电费");
            showColList.Add("C2", "werkl");
            importColList.Add("C2","sfjpo");
            var dt = DBUtility.DgvColInfoXmlHelper.CreateTableStructure();
            var row = dt.NewRow();
            row[0] = "C1";
            row[1] = "水电费";
            row[2] = "";
            row[3] = true;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row[0] = "C2";
            row[1] = "水电费1";
            row[2] = "";
            row[3] = true;
            dt.Rows.Add(row);
            row = dt.NewRow();
            row[0] = "C3";
            row[1] = "水电费2";
            row[2] = "";
            row[3] = true;
            dt.Rows.Add(row);
            DBUtility.DgvColInfoXmlHelper.WriteInfos("Test",dt);
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            var showColList = new Dictionary<string, string>();
            var importColList = new Dictionary<string, string>();
            var dt = DBUtility.DgvColInfoXmlHelper.ReadInfos("Test");
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            var frm = new FrmImportExcelSet("Test");
            if (frm.ShowDialog() == DialogResult.OK)
            {

            }
        }


    }
}
