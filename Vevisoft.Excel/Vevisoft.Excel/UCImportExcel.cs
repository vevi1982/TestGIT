using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.Excel.Core;

namespace Vevisoft.Excel
{
    public partial class UCImportExcel : UserControl,DataStructure.IUcokCancel
    {
        readonly ExcelImportCore _importCore=new ExcelImportCore();
        public UCImportExcel()
        {
            InitializeComponent();
        }

        public void InitData(object condition)
        {
            var filename = condition as string;
            if (!string.IsNullOrEmpty(filename))
            {
                txtFileName.Text = filename;
                _importCore.LoadFile(filename);
                if (_importCore.SheetNames != null)
                {
                    comboBox1.Items.AddRange(items: _importCore.SheetNames.ToArray());
                    if (comboBox1.Items.Count > 0)
                        comboBox1.SelectedIndex = 0;
                }
            }
        }

        public object ReturnObject { get; set; }

        public void btnOK_Click()
        {
            if (_importCore.SheetNames != null)
            {
                var sheetName = comboBox1.SelectedItem as string;
                if (_importCore.SheetNames.Contains(sheetName))
                {
                    ReturnObject = _importCore.GetTable(sheetName,true);
                }
            }
            //
            var form = this.FindForm();
            if (form != null) form.DialogResult = DialogResult.OK;
        }

        public void btnCancel_Click()
        {
            var form = this.FindForm();
            if (form != null) form.DialogResult = DialogResult.Cancel;
        }
    }
}
