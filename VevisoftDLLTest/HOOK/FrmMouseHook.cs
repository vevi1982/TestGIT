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

        private void button1_Click(object sender, EventArgs e)
        {
            var rect = GetFormRect(GetMainForm());
            var msg = string.Format("{0},{1},{1},{3}", rect.Left, rect.Top, rect.Right - rect.Left,
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
    }
}
