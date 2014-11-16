using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using Vevisoft.DBUtility;
using Vevisoft.Excel;
using Vevisoft.Excel.Core;
using Vevisoft.Theme.Properties;

namespace Vevisoft.Theme
{
    /// <summary>
    /// 用于基础数据的表格显示与保存数据
    /// </summary>
    public class GridViewOperate_BaseData : DataStructure.IGridViewOperate
    {
        /// <summary>
        /// 数据库字段与显示字段对应
        /// </summary>
        Dictionary<string,string> showColumnList=new Dictionary<string, string>();
        /// <summary>
        /// 数据库字段与导入Excel列名对应
        /// </summary>
        Dictionary<string,string> importColumnList=new Dictionary<string, string>();
        //
        private GridViewOperate_BaseData(GridControl gctl)
        {
            BindingControl = gctl;
            gctl.ContextMenuStrip = CreateContextMenu();
        }
        private GridViewOperate_BaseData(GridControl gctl, Dictionary<string, string> showColList,
                                        Dictionary<string, string> importColList) : this(gctl)
        {
            showColumnList = showColList;
            importColumnList = importColList;
        }

        public string _dbTableName;
        public GridViewOperate_BaseData(GridControl gctl, string dbTableName) : this(gctl)
        {
            _dbTableName = dbTableName;
        }

        #region 数据模型,加载数据，导入数据的集合
        /*需要获取的数据为：
         * 数据库字段名，显示名称，导入对应的名称
         */
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
        /// <summary>
        /// 创建右键菜单
        /// </summary>
        /// <returns></returns>
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
            if (!string.IsNullOrEmpty(_dbTableName))
                ImportExcelDataToChange(_dbTableName);
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
            DBUtility.DbHelperSQL.UpdateDataTable(_dbTableName,DataSource as DataTable);
        }
        #endregion
        /// <summary>
        /// 初始化数据
        /// </summary>
        public void InitData()
        {
            //
            var sql = "select * from " + _dbTableName;
            var dt = DBUtility.DbHelperSQL.Query(sql).Tables[0];
            DataSource = dt;
        }
        /// <summary>
        /// 导入Excel数据文件
        /// </summary>
        private void ImportExcelDataToChange(string dbTableName)
        {
            var frmdiag = new FrmImportExcelSet(dbTableName);
            if (frmdiag.ShowDialog() == DialogResult.OK)
            {
                ImportExcelDataToChange(frmdiag.DataSource,frmdiag.IsReplace);
            }
        }
        private void ImportExcelDataToChange(DataTable dtSource, bool isReplace)
        {
            var dtData = DataSource as DataTable;
            if (dtData == null)
                return;
            var dtSet =
            DBUtility.DgvColInfoXmlHelper.ReadInfos(_dbTableName);

            if (dtSource.Rows.Count > DBUtility.ParallelSet.StartParallelCount)
            {
                //并行计算 进行导入
                System.Threading.Tasks.Parallel.For(0, dtSource.Rows.Count, (i) =>
                    {
                        var row = dtSource.Rows[i];
                        ImportOneData(row,dtData,dtSet,isReplace);
                    });
            }
            else
            {
                //普通导入
                for (int i = 0; i < dtSource.Rows.Count; i++)
                {
                    var row = dtSource.Rows[i];
                    ImportOneData(row, dtData, dtSet, isReplace);
                }
            }
        }
        private void ImportOneData(DataRow iptRow, DataTable dtData, DataTable dtSet,bool isReplace)
        {
            DataRow row = null;
            if (isReplace)
            {
                //查找是否有相同的列？
                var selectFilter = "";
                foreach (DataRow setrow in dtSet.Rows)
                {
                    if (Convert.ToBoolean(setrow["IsPri"]))
                    {
                        var colName = setrow[0].ToString();
                        var excelColName = setrow[2].ToString();
                        var subFilter = GetRowCellValueToFilter(colName, excelColName, iptRow);
                        if (!string.IsNullOrEmpty(subFilter))
                            if (!string.IsNullOrEmpty(selectFilter))
                                selectFilter += ";" + subFilter;
                            else selectFilter = subFilter;
                    }
                }
                //
                var rows = dtData.Select(selectFilter);
                if (rows.Length > 0)
                    row = rows[0];
                else
                {
                    row = dtData.NewRow();
                    dtData.Rows.Add(row);
                }
            }
            else
            {
                row = dtData.NewRow();
                dtData.Rows.Add(row);
            }
            //设置 覆盖行或者新添行的值
            foreach (DataRow setRow in dtSet.Rows)
            {
                row[setRow[0].ToString()] = iptRow[setRow[2].ToString()];
            }
            
        }
        private string GetRowCellValueToFilter(string colName,string excelColName ,DataRow excelRow)
        {
            if (excelRow.Table.Columns.Contains(excelColName))
            {
                if (excelRow.Table.Columns[excelColName].DataType == typeof(string) ||
                    excelRow.Table.Columns[excelColName].DataType == typeof(DateTime))
                    return string.Format("{0} = '{1}'", colName, excelRow[excelColName]);// "'" + row[colName] + "'";    
                if (excelRow.Table.Columns[excelColName].DataType == typeof(int) || excelRow.Table.Columns[colName].DataType == typeof(float) ||
                    excelRow.Table.Columns[excelColName].DataType == typeof(decimal))
                    if (!string.IsNullOrEmpty(excelRow[excelColName].ToString()))
                        return string.Format("{0} = {1}", colName, excelRow[excelColName]);
            }
            return "";
        }
        

        public object DataSource
        {
            get { return DefaultGridView.GridControl.DataSource; }
            set
            {
                DefaultGridView.GridControl.DataSource = value;
                var dt = value as DataTable;
                if (dt != null)
                    dt.RowChanging += dt_RowChanging;
                //test
                //showColumnList.Add("C1","死爱意");
               SetColumnHeaderText();
                //判断是否更改
                DefaultGridView.CellValueChanging+=DefaultGridView_CellValueChanging;
                
            }
        }
        private void SetColumnHeaderText()
        {
            var dt = DBUtility.DgvColInfoXmlHelper.ReadInfos(_dbTableName);
            foreach (GridColumn column in DefaultGridView.Columns)
            {
                var rows = dt.Select(string.Format("DBColName='{0}'", column.FieldName));
                if (rows.Length > 0)
                {
                    column.Caption = rows[0]["ShowColName"].ToString();
                }
            }
        }
        /// <summary>
        /// 数据是否变化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DefaultGridView_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
        {
            var dt = DataSource as DataTable;
            if (dt == null)
                return;
            if (e.Value != dt.Rows[e.RowHandle][e.Column.FieldName])
                DataChanged = true;
            //
            if (DataChanged)
                DefaultGridView.GroupPanelText = "Changed!!!!";
        }
        /// <summary>
        /// DataSouce是否发生改变
        /// </summary>
        public bool DataChanged { get; set; }
        /// <summary>
        /// 数据是否添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
