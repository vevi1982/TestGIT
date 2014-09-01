using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vevisoft.WindowsAPI;

namespace VevisoftDLLTest.HOOK
{
    public partial class FrmMouseHook : Form
    {
        public FrmMouseHook()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var mhook = new MouseHookUtility();
            mhook.OnMouseActivity += mhook_OnMouseActivity;
            mhook.Start();
        }
        void mhook_OnMouseActivity(object sender, MouseEventArgs e)
        {
            textBox2.Text = e.Button.ToString() + "  " + e.X + "," + e.Y;
        }
    }
}
