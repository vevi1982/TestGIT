using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Vevisoft.Excel;
using Vevisoft.Excel.Core;
using Vevisoft.Theme.Properties;
using GridView = DevExpress.XtraGrid.Views.Grid.GridView;

namespace Vevisoft.Theme
{
    public class GridViewOperate:Vevisoft.DataStructure.IGridViewOperate
    {

        public GridViewOperate(GridControl gctl)
        {
            BindingControl = gctl;
            gctl.ContextMenuStrip = CreateContextMenu();
            if(DefaultGridView!=null)
                ShowRowNumber(DefaultGridView);
        }
        #region 显示行号
        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="gridview"></param>
        public void ShowRowNumber(GridView gridview)
        {
            gridview.IndicatorWidth = 40;
            //此代码只有在动态创建的时候起作用
            gridview.CustomDrawRowIndicator += gridview_CustomDrawRowIndicator;
        }

        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gridview_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1) + "";
            }
        }

        #endregion
        public object BindingControl
        {get; set; }

        public GridView DefaultGridView
        {
            get
            {
                if (BindingControl != null)
                {
                    return ((GridControl)BindingControl).DefaultView as GridView; 
                }
                else return null;
            }
        }

        private ContextMenuStrip CreateContextMenu()
        {
            var cmenu=new ContextMenuStrip();
            var item = new ToolStripMenuItem
                {
                    Text = "导出Excel文件(&E)", Image = Properties.Resources.export
                };
            item.Click += (object sender, EventArgs e) => ExportExcelData();
            
            cmenu.Items.Add(item);
            //
            var item2 = new ToolStripMenuItem
                {
                    Text = "导入Excel文件(&I)", Image = Properties.Resources.import
                };
            item2.Click += (object sender, EventArgs e) => ImportExcelData();
          
            cmenu.Items.Add(item2);
            //
            cmenu.Items.Add(new ToolStripSeparator());
            //添加
            var itemadd = new ToolStripMenuItem
            {
                Text = "添加(&A)",
                //Image = Properties.Resources.Delete
            };
            itemadd.Click += (object sender, EventArgs e) => AddItem();

            cmenu.Items.Add(itemadd);
            //删除
            var item3 = new ToolStripMenuItem
                {
                    Text = "删除(&D)", Image = Properties.Resources.Delete
                };
            item3.Click += (object sender, EventArgs e) => DeleteItem();
           
            cmenu.Items.Add(item3);
            //保存
            var item4 = new ToolStripMenuItem
                {
                    Text = "保存数据(&S)", Image = Properties.Resources.save
                };
            item4.Click += (object sender, EventArgs e) => SaveData();
            
            cmenu.Items.Add(item4);
            //
            return cmenu;
        }

        #region 操作

        /// <summary>
        /// 导出Excel数据
        /// </summary>
        public void ExportExcelData()
        {
            if (DefaultGridView == null)
                return;
            var dt = DefaultGridView.DataSource as DataTable;
            if (dt == null)
            {
                var dv = DefaultGridView.DataSource as DataView;
                if (dv == null)
                    return;
                dt = dv.Table;
            }
            var columnNameList = (from GridColumn column in DefaultGridView.Columns
                                  where column.Visible
                                  where !string.IsNullOrEmpty(column.FieldName) &&
                                  !string.IsNullOrEmpty(column.Caption)
                                  select column).ToDictionary(column => column.FieldName,
                                 column => column.Caption);
            //
            var sdiag = new SaveFileDialog
            {
                Title = Resources.SaveFileTitle,
                Filter = Resources.ExcelDiagFilter
            };
            if (sdiag.ShowDialog() == DialogResult.OK)
            {
                DialogHelper.ShowWattingForm(DefaultGridView.GridControl.FindForm());
                var export = new ExportExcelCore();
                export.RenderDataTableToExcel(sdiag.FileName, dt, "", columnNameList);
                DialogHelper.CloaseWattingForm();
            }

        }
        

        /// <summary>
        /// 导入Excel数据
        /// </summary>
        public void ImportExcelData()
        {
            var opdiag = new OpenFileDialog { Title = Resources.SelectExcelFile, Filter = Resources.ExcelDiagFilter };
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                DialogHelper.ShowWattingForm(DefaultGridView.GridControl.FindForm());
                var ctrl = new UCImportExcel();
                ctrl.InitData(opdiag.FileName);
                //Close Wait Form
                DialogHelper.CloaseWattingForm();
                if (DialogHelper.ShowMessageForm(ctrl, "导入数据") == DialogResult.OK)
                {
                    var dt = ctrl.ReturnObject as DataTable;
                    if (dt != null)
                    {
                        DefaultGridView.GridControl.DataSource = dt;
                        //dt.RowChanging += dt_RowChanging;
                    }
                }
            }
        }

        /// <summary>
        /// 查找
        /// </summary>
        public void FindText()
        {
            XtraMessageBox.Show("暂未实现？", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        /// <summary>
        /// 替换
        /// </summary>
        public void ReplaceText()
        {
            XtraMessageBox.Show("暂未实现？", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 添加记录
        /// </summary>
        public void AddItem()
        {
            if (DefaultGridView == null)
                return;
            DefaultGridView.AddNewRow();
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        public void DeleteItem()
        {
            if (DefaultGridView == null)
                return;
            if (XtraMessageBox.Show("你确定要删除选中的记录吗？", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var intiSelectRowCount = DefaultGridView.SelectedRowsCount;
                if (intiSelectRowCount > 0)
                {
                    DefaultGridView.DeleteSelectedRows();
                }
            }
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        public void SaveData()
        {

        }
        #endregion

        public object DataSource
        {
            get { return DefaultGridView.GridControl.DataSource; }
            set { DefaultGridView.GridControl.DataSource = value;
                var dt = value as DataTable;
                if(dt!=null)
                    dt.RowChanging+=dt_RowChanging;
            }
        }
        /// <summary>
        /// DataSouce是否发生改变
        /// </summary>
        public bool DataChanged { get; set; }

        private void dt_RowChanging(object sender, DataRowChangeEventArgs e)
        {
            DataChanged = (e.Action == DataRowAction.Change) || (e.Action == DataRowAction.Add) ||
                          (e.Action == DataRowAction.Delete);

            //
            if (DataChanged)
                DefaultGridView.GroupPanelText = "Changed!!!!";
        }
    }
}
