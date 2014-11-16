using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vevisoft.Theme
{
    public partial class FrmMessage : DevExpress.XtraEditors.XtraForm
    {
        private DataStructure.IUcokCancel structure;
        public FrmMessage()
        {
            InitializeComponent();
        }

        public FrmMessage(Control control, string title) : this()
        {
            this.Text = title;

            structure = control as DataStructure.IUcokCancel;

            var addHeight = Math.Max(100,60+ control.Height);
            var wid = Math.Max(175, control.Width);
            this.Size = new Size(wid, addHeight);
            control.Dock=DockStyle.Top;
            
            this.Controls.Add(control);
            //
            if (structure != null)
            {
                btnOK.Click += (object sender, EventArgs e1) => structure.btnOK_Click();
                simpleButton2.Click += (object sender, EventArgs e1) => structure.btnCancel_Click();
            }
        }
        /// <summary>
        /// 返回的数据
        /// </summary>
        public object ReturnObject
        {
            get
            {
                if (structure != null)
                    return structure.ReturnObject;
                return null;
            }
        }
    }
}
