using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.DBUtility;

namespace QQMusicUpdateDL
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            var core=new MySqlDBCOre();
            var dt = core.GetAccountInfo();
            dataGridView1.DataSource = dt;
            //
             if(!string.IsNullOrEmpty(MysqlHelper.ErrorString))
            MessageBox.Show(MysqlHelper.ErrorString);
        }
    }
}
