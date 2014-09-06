using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VevisoftDLLTest.WindowsForm
{
    public partial class FrmAPIWindowsTest : Form
    {
        public FrmAPIWindowsTest()
        {
            InitializeComponent();
        }

        private int count = 0;
        public void GetAllWindows()
        {
            count = 0;
            Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowCallBack myCallBack =
                EnumWindowCallBack;

            Vevisoft.WindowsAPI.SystemWindowsAPI.EnumWindows(myCallBack, 0);
        }

        private bool EnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (strclsName.ToString().Trim().ToLower() != "TXGFLayerMask".ToLower())
                return true;

            count++;
            richTextBox1.AppendText(string.Format("{3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n",strTitle,strclsName,hwnd.ToString(),count));
            //
            IntPtr chHandle = Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null,
                                                                                null);
            if (chHandle != IntPtr.Zero)
            {
                strclsName = new StringBuilder(256);
                Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(chHandle, strclsName, 257);
                strTitle = new StringBuilder(256);
                Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(chHandle, strTitle, 257);

                richTextBox1.AppendText(string.Format("   {3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n", strTitle,
                                                      strclsName, hwnd.ToString(), count + ".1"));
            }
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GetAllWindows();
        }
    }
}
