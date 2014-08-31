using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Vevisoft.WindowsAPI
{
    public class SystemWindowsAPI
    {
        //需调用API函数
        //需在开头引入命名空间
        //using System.Runtime.InteropServices;
        //获取当前窗口句柄:GetForegroundWindow()
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "GetWindow", SetLastError = true)]
        public static extern IntPtr GetNextWindow(IntPtr hwnd, [MarshalAs(UnmanagedType.U4)] int wFlag);

        //返回值类型是IntPtr,即为当前获得焦点窗口的句柄
        //使用方法 : IntPtr myPtr=GetForegroundWindow();
        //获取到该窗口句柄后,可以对该窗口进行操作.比如,关闭该窗口或在该窗口隐藏后,使其显示
        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int ShowWindow(IntPtr hwnd, int nCmdShow);

        //其中ShowWindow(IntPtr hwnd, int nCmdShow);
        //nCmdShow的含义
        //0 关闭窗口
        //1 正常大小显示窗口
        //2 最小化窗口
        //3 最大化窗口
        //使用实例: ShowWindow(myPtr, 0);
        //获取窗口大小及位置:需要调用方法GetWindowRect(IntPtr hWnd, ref RECT lpRect)
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left; //最左坐标
            public int Top; //最上坐标
            public int Right; //最右坐标
            public int Bottom; //最下坐标
        }

        [DllImport("user32.dll", EntryPoint = "FindWindow", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", EntryPoint = "FindWindowEx", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass,
                                                 string lpszWindow);
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        //
        [DllImport("user32", EntryPoint = "GetTopWindow")]
        public static extern IntPtr GetTopWindow(IntPtr hwnd);

        //
        public delegate bool EnumChildWindowsProc(IntPtr hwnd, long lParam);

        [DllImport("user32.dll")]
        public static extern long EnumChildWindows(IntPtr hWndParent, EnumChildWindowsProc lpEnumFunc, long lParam);

        [DllImport("user32.dll")]
        public static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        public static IntPtr FindMainWindowHandle(string caption, int delay, int maxTries)
        {
            IntPtr mwh = IntPtr.Zero;
            bool formFound = false;
            int attempts = 0;
            while (!formFound && attempts < maxTries)
            {
                if (mwh == IntPtr.Zero)
                {
                    Console.WriteLine("Form not yet found");
                    Thread.Sleep(delay);
                    ++attempts;
                    mwh = FindWindow(null, caption);
                }
                else
                {
                    Console.WriteLine("Form has been found");
                    formFound = true;
                }
            }

            if (mwh == IntPtr.Zero)
                return IntPtr.Zero;
            else
                return mwh;
        }

        public static IntPtr FindWindowByIndex(IntPtr hwndParent, int index)
        {
            if (index == 0)
            {
                return hwndParent;
            }
            else
            {
                int ct = 0;
                IntPtr result = IntPtr.Zero;
                do
                {
                    result = FindWindowEx(hwndParent, result, null, null);
                    if (result != IntPtr.Zero)
                    {
                        ++ct;
                    }
                } while (ct < index && result != IntPtr.Zero);
                return result;
            }
        }



        public static bool EumWinChiPro(IntPtr hWnd, long lParam)
        {
            var s = new StringBuilder(256);
            GetClassName(hWnd, s, 257);
            string ss = s.ToString();
            ss = ss.Trim();
            return true;

        }


       
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        public string GetActiveWindowTitle()
        {
            const int nChars = 256;
            var Buff = new StringBuilder(nChars);
            IntPtr handle = GetForegroundWindow();

            if (GetWindowText(handle, Buff, nChars) > 0)
            {
                return Buff.ToString();
            }
            return null;
        }
        public const int GW_HWNDNEXT = 2; // The next window is below the specified window
        public const int GW_HWNDPREV = 3; // The previous window is ab
        /// <summary>
        /// Searches for the topmost visible form of your app in all the forms opened in the current Windows session.
        /// </summary>
        /// <param name="hWnd_mainFrm">Handle of the main form</param>
        /// <returns>The Form that is currently TopMost, or null</returns>
        public static IntPtr GetTopMostWindow(IntPtr hWnd_mainFrm)
        {
            //Form frm = null;

            IntPtr hwnd = GetTopWindow((IntPtr)null);
            if (hwnd != IntPtr.Zero)
            {
                while ((!IsWindowVisible(hwnd) ) && hwnd != hWnd_mainFrm)
                {
                    // Get next window under the current handler
                    hwnd = GetNextWindow(hwnd, GW_HWNDNEXT);

                    try
                    {
                        //frm = (Form)Form.FromHandle(hwnd);
                    }
                    catch
                    {
                        // Weird behaviour: In some cases, trying to cast to a Form a handle of an object 
                        // that isn't a form will just return null. In other cases, will throw an exception.
                    }
                }
            }

            return hwnd;
        }
    }
}
