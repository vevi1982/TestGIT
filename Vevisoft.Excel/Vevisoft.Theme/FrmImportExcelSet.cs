using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using Vevisoft.Excel;
using Vevisoft.Excel.Core;
using Vevisoft.Theme.Properties;

namespace Vevisoft.Theme
{
    public partial class FrmImportExcelSet : FrmDialog
    {
        private FrmImportExcelSet()
        {
            InitializeComponent();
        }

        private string dbTableName = "";
        public FrmImportExcelSet(string tableName):this()
        {
            dbTableName = tableName;
            InitControls();
        }
        private void InitControls()
        {
            comboBoxEdit1.Enabled = false;
            comboBoxEdit1.Text = "";
            comboBoxEdit1.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
            comboBoxEdit1.Properties.Items.Clear();
            //
            gridControlVs1.DataSource = null;
            gridControlVs1.Enabled = false;
            //
            _excelColNameList = new List<string>();
            //
            var dt = DBUtility.DgvColInfoXmlHelper.ReadInfos(dbTableName);
            //此时不应该显示Excel列的内容
            if (dt.Columns.Contains("ExcelColNameTmp"))
                dt.Columns.Remove("ExcelColNameTmp");
            dt.Columns["ExcelColName"].ColumnName = "ExcelColNameTmp";
            dt.Columns.Add("ExcelColName");
            gridControlVs1.DataSource = dt;
            //
            repositoryItemComboBox1.TextEditStyle = TextEditStyles.DisableTextEditor;
            repositoryItemComboBox1.EditValueChanging += repositoryItemComboBox1_EditValueChanging;
            
        }

        void repositoryItemComboBox1_EditValueChanging(object sender, DevExpress.XtraEditors.Controls.ChangingEventArgs e)
        {
            if (e.NewValue != e.OldValue)
            {
                var dt = gridControlVs1.DataSource as DataTable;
                if (dt != null)
                {
                    var rows = dt.Select(string.Format("ExcelColName='{0}'", e.NewValue));
                    if (rows.Length > 0)
                    {
                        XtraMessageBox.Show("【Excel字段】" + e.NewValue + "不能对应两个数据库字段!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        e.Cancel = true;
                    }
                }
            }

        }

        readonly ExcelImportCore _importCore = new ExcelImportCore();
        List<string> _excelColNameList=new List<string>();
        private void buttonEdit1_Click(object sender, EventArgs e)
        {
            _excelColNameList = new List<string>();
            comboBoxEdit1.Properties.Items.Clear();
            var opdiag = new OpenFileDialog { Title = Resources.SelectExcelFile, Filter = Resources.ExcelDiagFilter };
            if (opdiag.ShowDialog() == DialogResult.OK)
            {
                ClearCurrentSet();
                comboBoxEdit1.SelectedIndex = -1;
                //
                var filename = opdiag.FileName;
                buttonEdit1.Text = filename;
                DialogHelper.ShowWattingForm(this);
                _importCore.LoadFile(filename);
                if (_importCore.SheetNames != null)
                {
                    comboBoxEdit1.Properties.Items.AddRange(items: _importCore.SheetNames.ToArray());
                    if (comboBoxEdit1.Properties.Items.Count > 0)
                    {
                        comboBoxEdit1.SelectedIndex = 0;
                        comboBoxEdit1.Enabled = true;
                        gridControlVs1.Enabled = true;
                        //comboBoxEdit1_SelectedIndexChanged(null, null);
                    }
                }
                DialogHelper.CloaseWattingForm();
            }
        }

        private void comboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxEdit1.SelectedItem == null)
                return;
            var sheetName = comboBoxEdit1.SelectedItem.ToString();
            if (string.IsNullOrEmpty(sheetName))
                return;
            DataSource = _importCore.GetTable(sheetName, true);
            if (DataSource == null)
            {
                ClearCurrentSet();
                return;
            }
            _excelColNameList=new List<string>();
            foreach (DataColumn column in DataSource.Columns)
            {
                if(!_excelColNameList.Contains(column.ColumnName))
                    _excelColNameList.Add(column.ColumnName);
            }
            //绑定列的数据源
            repositoryItemComboBox1.Items.Clear();
            repositoryItemComboBox1.Items.AddRange(_excelColNameList.ToArray());
            //
            SetBindingData(_excelColNameList);
        }
        private void SetBindingData(List<string> excelColNameList)
        {
            var dt = gridControlVs1.DataSource as DataTable;
            if (dt != null)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var excelName = row["ExcelColNameTmp"].ToString();
                    if (excelColNameList.Contains(excelName))
                        row["ExcelColName"] = excelName;
                }
            }
        }

        private void ClearCurrentSet()
        {
            var dt = gridControlVs1.DataSource as DataTable;
            if (dt != null)
            {
                //此时不应该显示Excel列的内容
                if (dt.Columns.Contains("ExcelColName"))
                    dt.Columns.Remove("ExcelColName");
                dt.Columns.Add("ExcelColName");
                gridControlVs1.RefreshDataSource();
            }
        }

        private bool CheckDataIsCorrect()
        {
            var dt = gridControlVs1.DataSource as DataTable;
            if (dt == null)
                return true;
            //查看是否有重复对应的情况
            //查看是否有没有对应的情况
            foreach (DataRow row in dt.Rows)
            {
                if (string.IsNullOrEmpty(row["ExcelColName"].ToString()))
                {
                    XtraMessageBox.Show("【数据库字段】与【Excel字段】必须全部对应才能导入数据!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
                var excelName = row["ExcelColName"].ToString();
                var rows = dt.Select(string.Format("ExcelColName='{0}'", excelName));
                if (rows.Length > 1)
                {
                    XtraMessageBox.Show("【Excel字段】"+excelName+"不能对应两个数据库字段!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }
            //保存对应关系文件
            DBUtility.DgvColInfoXmlHelper.WriteInfos(dbTableName, dt);
            return true;
        }

        /// <summary>
        /// 导入的数据源
        /// </summary>
        public DataTable DataSource { get; set; }
        /// <summary>
        /// 导入数据时是否覆盖
        /// </summary>
        public bool IsReplace{get { return checkEdit1.Checked; }}

        private void btnImport_Click(object sender, EventArgs e)
        {
            if (CheckDataIsCorrect())
            {
                //保存数据    
                this.DialogResult = DialogResult.OK;
            }
        }

        private void checkEdit2_CheckedChanged(object sender, EventArgs e)
        {
            checkEdit1.Checked = !checkEdit2.Checked;
        }

        private void checkEdit1_CheckedChanged(object sender, EventArgs e)
        {
            checkEdit2.Checked = !checkEdit1.Checked;
        }
    }
}
