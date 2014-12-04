using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vevisoft.WindowsAPI.Hook;
using mshtml;

namespace Vevisoft.WindowsAPI
{
    public class SystemWindowsAPI
    {
        [DllImport("user32.dll", EntryPoint = "WindowFromPoint")] //调用system目录下的user32.dll动态链接库，并声明应用的过程名称
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

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
        [DllImport("user32.dll", EntryPoint = "GetTopWindow")]
        public static extern IntPtr GetTopWindow(IntPtr hwnd);

        //
        public delegate bool EnumChildWindowsProc(IntPtr hwnd, long lParam);

        [DllImport("user32.dll")]
        public static extern long EnumChildWindows(IntPtr hWndParent, EnumChildWindowsProc lpEnumFunc, long lParam);

        #region 获取窗体信息

        [DllImport("user32.dll")]
        public static extern long GetClassName(IntPtr hwnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("User32.dll", EntryPoint = "GetWindowText")]
        public static extern int GetWindowText(IntPtr hwnd, StringBuilder lpString, int nMaxCount);

        #endregion


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
        public const int WM_SETTEXT = 0x000C;
        public const int EM_SETPASSWORDCHAR = 0x00CC;
        public const int EM_GETPASSWORDCHAR = 0x00D2;
        public const int WM_COPYDATA = 0x004A;
        /// <summary>
        /// Searches for the topmost visible form of your app in all the forms opened in the current Windows session.
        /// </summary>
        /// <param name="hWnd_mainFrm">Handle of the main form</param>
        /// <returns>The Form that is currently TopMost, or null</returns>
        public static IntPtr GetTopMostWindow(IntPtr hWnd_mainFrm)
        {
            //Form frm = null;

            IntPtr hwnd = GetTopWindow((IntPtr) null);
            if (hwnd != IntPtr.Zero)
            {
                while ((!IsWindowVisible(hwnd)) && hwnd != hWnd_mainFrm)
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

        #region 枚举所有窗体

        public delegate bool FindWindowCallBack(IntPtr hwnd, int lParam);

        [DllImport("user32.dll")]
        public static extern int EnumWindows(FindWindowCallBack x, int y);

        #endregion

        [DllImport("user32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hwnd, uint wMsg, int wParam, ref int lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "RegisterWindowMessage")]
        public static extern uint RegisterWindowMessage(string lpString);

        [DllImport("OLEACC.DLL", EntryPoint = "ObjectFromLresult")]
        public static extern int ObjectFromLresult(
            int lResult,
            ref System.Guid riid,
            int wParam,
            [MarshalAs(UnmanagedType.Interface), System.Runtime.InteropServices.In, System.Runtime.InteropServices.Out] ref System.Object ppvObject
            //注意这个函数ObjectFromLresult的声明
            );

        #region 当前窗体

        [DllImport("user32.dll")]
        public static extern IntPtr SetActiveWindow(IntPtr hWnd);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        #endregion

        #region 设置控件的值

        /// <summary>
        /// 设置控件的值，例如textbox
        /// </summary>
        /// <param name="controlHandle"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int SetControlValue(IntPtr controlHandle, string text)
        {
            
            return SendMessage(controlHandle, WM_SETTEXT, IntPtr.Zero, text);
        }
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);
       
        public static int SetPassWordEditValue(IntPtr controlHandle, string password)
        {
            //启动键盘钩子
            var hook = new DebugHook();
            hook.StartHook();
            //获取密码字符
            var charpass = SendMessage(controlHandle, EM_GETPASSWORDCHAR, 0, IntPtr.Zero);
            //string aa = "";
            //if (charpass != 0)
            //{
            //    aa = Marshal.PtrToStringAuto((IntPtr)charpass);
            //    aa = Marshal.PtrToStringAnsi((IntPtr)charpass);
            //    aa = Marshal.PtrToStringUni((IntPtr)charpass);
            //    //aa = Marshal.PtrToStringBSTR((IntPtr)charpass, 1);
            //    aa = Encoding.Default.GetString(new byte[] { Marshal.ReadByte((IntPtr)charpass) });
            //}            
            //取消密码字符
            PostMessage(controlHandle, EM_SETPASSWORDCHAR, 0, IntPtr.Zero);
            //SendMessage(controlHandle, EM_SETPASSWORDCHAR, charpass, IntPtr.Zero);

            //发送文本
            SendMessage(controlHandle, WM_SETTEXT, IntPtr.Zero, password);
            hook.UnHook();
            return 1;
        }
        #endregion

        #region 获取IE控件内的Html

        public static mshtml.IHTMLDocument2 GetHtmlDocument(IntPtr hwnd)
        {
            var domObject = new System.Object();
            int tempInt = 0;
            var guidIEDocument2 = new Guid();
            var WM_Html_GETOBJECT = SystemWindowsAPI.RegisterWindowMessage("WM_Html_GETOBJECT"); //定义一个新的窗口消息
            int W = SystemWindowsAPI.SendMessage(hwnd, WM_Html_GETOBJECT, 0, ref tempInt);
                //注:第二个参数是RegisterWindowMessage函数的返回值
            int lreturn = SystemWindowsAPI.ObjectFromLresult(W, ref guidIEDocument2, 0, ref domObject);
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2) domObject;
            return doc;
        }

        #endregion

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        #region 程序是否响应
        [Flags]
        public enum SendMessageTimeoutFlags : uint
        {
            SMTO_NORMAL = 0x0,
            SMTO_BLOCK = 0x1,
            WM_NULL = 0,
            SMTO_ABORTIFHUNG = 0x2,
            SMTO_NOTIMEOUTIFNOTHUNG = 0x8
        }
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr hWnd,
            uint Msg,
            UIntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags fuFlags,
            uint uTimeout,
            out UIntPtr lpdwResult);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessageTimeout(
            IntPtr windowHandle,
            uint Msg,
            IntPtr wParam,
            IntPtr lParam,
            SendMessageTimeoutFlags flags,
            uint timeout,
            out uint result);

        /* Version specifically setup for use with WM_GETTEXT message */

        [DllImport("user32.dll", EntryPoint = "SendMessageTimeout", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern uint SendMessageTimeoutText(
            IntPtr hWnd,
            int Msg, // Use WM_GETTEXT
            int countOfChars,
            StringBuilder text,
            SendMessageTimeoutFlags flags,
            uint uTImeoutj,
            out IntPtr result);
        //
        public static bool IsExeNotResponse(IntPtr hwnd)
        {
            var lRes = uint.MinValue;
            //Register the message
            var lMsg = RegisterWindowMessage("WM_HTML_GETOBJECT");
            //Get the object
            SendMessageTimeout(hwnd, lMsg, IntPtr.Zero, IntPtr.Zero, SendMessageTimeoutFlags.WM_NULL, 1000, out lRes);
            return lRes == 0;
            //if (lRes != IntPtr.Zero)
            //{
            //    //Get the object from lRes
            //    //htmlDoc = (mshtml.IHTMLDocument)Win32.ObjectFromLresult(lRes, IID_IHTMLDocument, IntPtr.Zero);
            //    //return htmlDoc;
            //}
        }

        public static bool IsExeNotResponse(string appTitle)
        {
            var hwnd = Vevisoft.WindowsAPI.SystemWindowsAPI.FindMainWindowHandle(appTitle, 200, 10);
            if (hwnd == IntPtr.Zero)
                hwnd = Vevisoft.WindowsAPI.SystemWindowsAPI.FindMainWindowHandle(appTitle+"（未响应）", 200, 10);
            if (hwnd != IntPtr.Zero)
                return Vevisoft.WindowsAPI.SystemWindowsAPI.IsExeNotResponse(hwnd);
            //
            return true;
        }
        #endregion
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);
        public static void RefreshTray()
        {
           RefreshTray_XP();
        }
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
        public static void RefreshTray_XP()
        {   const int WM_MOUSEMOVE = 0x200;
            IntPtr k = FindWindow("Shell_TrayWnd", null);
            k = FindWindowEx(k, IntPtr.Zero, "TrayNotifyWnd", null);
            k = FindWindowEx(k, IntPtr.Zero, "SysPager", null);
            k = FindWindowEx(k, IntPtr.Zero, "ToolbarWindow32", null);
            RECT nr = new RECT();

            GetWindowRect((IntPtr)k, ref nr);

            for (int x = 0; x < nr.Right; x = x + 2)
            {
                for (int y = 0; y < nr.Bottom; y = y + 2)
                {
                    SendMessage(k, WM_MOUSEMOVE, 0, MakeLParam(x, y));
                }
            }
        }
        public static void RefreshTray_Win7()
        {
            const int WM_MOUSEMOVE = 0x200;
            IntPtr k = FindWindow("Shell_TrayWnd", null);
            k = FindWindowEx(k, IntPtr.Zero, "TrayNotifyWnd", null);
            k = FindWindowEx(k, IntPtr.Zero, "SysPager", null);
            k = FindWindowEx(k, IntPtr.Zero, "ToolbarWindow32", null);
            RECT nr = new RECT();

            GetWindowRect((IntPtr)k, ref nr);

            for (int x = 0; x < nr.Right; x = x + 2)
            {
                for (int y = 0; y < nr.Bottom; y = y + 2)
                {
                    SendMessage(k, WM_MOUSEMOVE, 0, MakeLParam(x, y));
                }
            }
        }
    }
}
