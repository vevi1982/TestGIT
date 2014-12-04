using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VevisoftDLLTest.PCControl
{
    public partial class FrmSetSysTime : Form
    {
        public FrmSetSysTime()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Vevisoft.WindowsAPI.PCTimeUtility.SetSysTime(dateTimePicker1.Value);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Vevisoft.WindowsAPI.PCTimeUtility.SetSysTime_CMD(dateTimePicker1.Value);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var dtime = Vevisoft.WindowsAPI.PCTimeUtility.GetDateTimeFromInternet();
           // dateTimePicker1.Value = dtime;
        }
    }
}
