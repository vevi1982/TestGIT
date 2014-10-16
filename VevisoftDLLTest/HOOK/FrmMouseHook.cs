using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
            IntPtr hwnd = Vevisoft.WindowsAPI.SystemWindowsAPI.WindowFromPoint(e.Location.X, e.Location.Y);
            textBox2.Text = e.Button.ToString() + "  " + e.X + "," + e.Y+":"+hwnd.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var rect = GetFormRect(new IntPtr(int.Parse(textBox3.Text)));
            var msg = string.Format("{0},{1},{2},{3}", rect.Left, rect.Top, rect.Right - rect.Left,
                                    rect.Bottom - rect.Top);
            textBox1.Text = msg;
        }
        /// <summary>
        /// 获取主窗体句柄
        /// </summary>
        /// <returns></returns>
        public IntPtr GetMainForm()
        {
            const string caption = "QQ音乐";//TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 1000, 10);
            return handle;
        }
        /// <summary>
        /// 获取窗体界面位置及大小
        /// </summary>
        /// <param name="handle">窗体句柄</param>
        /// <returns></returns>
        public SystemWindowsAPI.RECT GetFormRect(IntPtr handle)
        {
            var formRec = new SystemWindowsAPI.RECT();
            SystemWindowsAPI.GetWindowRect(handle, ref formRec);
            return formRec;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var a = SystemWindowsAPI.GetHtmlDocument(new IntPtr(int.Parse(textBox3.Text)));
            MessageBox.Show(a.body.innerHTML);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IntPtr handle = SystemWindowsAPI.GetParent(new IntPtr(int.Parse(textBox3.Text)));
            Console.WriteLine(handle);
        }



        private void Forground(object sender, EventArgs e)
        {
            SystemWindowsAPI.SetForegroundWindow(new IntPtr(int.Parse(textBox3.Text)));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread.Sleep(5000);
            textBox3.Text = SystemWindowsAPI.GetForegroundWindow().ToString();
        }

        private void button7_Click(object sender, EventArgs e)
        {
          var bol=  SystemWindowsAPI.IsExeNotResponse(new IntPtr(int.Parse(textBox3.Text)));
            if (bol)
                button7.Text = "Resp 1";
            else button7.Text = "Resp 0";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            textBox3.Text = GetMainForm() + "";
        }
    }
}
