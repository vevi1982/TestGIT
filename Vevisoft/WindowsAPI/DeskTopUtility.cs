using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Vevisoft.WindowsAPI
{
    public class DeskTopUtility
    {
        /// <summary>
        /// 发送消息更新工作栏(图标托盘)，当程序使用Process关闭时，托盘图标会保留
        /// </summary>
        public static void RefreshTaskBar()
        {
            IntPtr k = SystemWindowsAPI.FindWindow("Shell_TrayWnd", null);
            k = SystemWindowsAPI.FindWindowEx(k, IntPtr.Zero, "TrayNotifyWnd", null);
            k = SystemWindowsAPI.FindWindowEx(k, IntPtr.Zero, "SysPager", null);
            k = SystemWindowsAPI.FindWindowEx(k, IntPtr.Zero, "ToolbarWindow32", null);
            var nr = new SystemWindowsAPI.RECT();

            SystemWindowsAPI.GetWindowRect((IntPtr)k, ref nr);

            for (int x = 0; x < nr.Right; x = x + 2)
            {
                for (int y = 0; y < nr.Bottom; y = y + 2)
                {
                    SendMessage(k, WM_MOUSEMOVE, 0, MakeLParam(x, y));
                }
            }
        }
        public const int WM_MOUSEMOVE = 0x200;
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
        /// <summary>
        /// The SendMessage API
        /// </summary>
        /// <param name="hWnd">handle to the required window</param>
        /// <param name="msg">the system/Custom message to send</param>
        /// <param name="wParam">first message parameter</param>
        /// <param name="lParam">second message parameter</param>
        /// <returns></returns>
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
    }
}
