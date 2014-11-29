using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Vevisoft.WindowsAPI
{
    public class MouseKeyBoradUtility
    {
        #region MyRegion

        private const int MOUSEEVENTF_MOVE = 0x0001; //  移动鼠标 
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002; // 模拟鼠标左键按下 
        private const int MOUSEEVENTF_LEFTUP = 0x0004; //模拟鼠标左键抬起 
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008; // 模拟鼠标右键按下 
        private const int MOUSEEVENTF_RIGHTUP = 0x0010; // 模拟鼠标右键抬起 
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020; //模拟鼠标中键按下 
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040; // 模拟鼠标中键抬起 
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000; //标示是否采用绝对坐标 

        #endregion

        #region 鼠标消息参数

        private const int WM_MOUSEFIRST = 0x0200; //移动鼠标时发生
        private const int WM_MOUSEMOVE = 0x0200; //移动鼠标时发生，同WM_MOUSEFIRST
        private const int WM_LBUTTONDOWN = 0x0201; //按下鼠标左键
        private const int WM_LBUTTONUP = 0x0202; //释放鼠标左键
        private const int WM_LBUTTONDBLCLK = 0x0203; //双击鼠标左键
        private const int WM_RBUTTONDOWN = 0x0204; //按下鼠标右键
        private const int WM_RBUTTONUP = 0x0205; //释放鼠标右键
        private const int WM_RBUTTONDBLCLK = 0x0206; //双击鼠标右键
        private const int WM_MBUTTONDOWN = 0x0207; //按下鼠标中键
        private const int WM_MBUTTONUP = 0x0208; //释放鼠标中键
        private const int WM_MBUTTONDBLCLK = 0x0209; //双击鼠标中键

        #endregion
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);
        #region 鼠标API

        /// <summary>
        /// 设置鼠标位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        /// <summary>
        /// 模拟点击
        /// </summary>
        /// <param name="dwFlags"></param>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <param name="dwData"></param>
        /// <param name="dwExtraInfo"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        /// <summary>
        /// 鼠标左键单击
        /// </summary>
        public static void MouseLeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// 鼠标右键单击
        /// </summary>
        public static void MouseRightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
        }

        /// <summary>
        /// 发送消息的鼠标单击操作
        /// </summary>
        /// <param name="fromHwnd"></param>
        /// <param name="mousePt"></param>
        public static void MouseLeftClickSendMsg(IntPtr fromHwnd, int x,int y)
        {
            SendMessage(fromHwnd, WM_LBUTTONDOWN, 0, MakeLParam(x, y));
            SendMessage(fromHwnd, WM_LBUTTONUP, 0, MakeLParam(x, y));
        }
        /// <summary>
        /// 发送消息的鼠标右键点击操作
        /// </summary>
        /// <param name="fromHwnd"></param>
        /// <param name="mousePt"></param>
        public static void MouseRightClickPostMsg(IntPtr fromHwnd, int x, int y)
        {
            PostMessage(fromHwnd, WM_RBUTTONDOWN, 0, MakeLParam(x, y));
            PostMessage(fromHwnd, WM_RBUTTONUP, 0, MakeLParam(x, y));
        }
        /// <summary>
        /// 发送消息的鼠标右键点击操作
        /// </summary>
        /// <param name="fromHwnd"></param>
        /// <param name="mousePt"></param>
        public static void MouseLeftClickPostMsg(IntPtr fromHwnd, int x, int y)
        {
            PostMessage(fromHwnd, WM_LBUTTONDOWN, 0, MakeLParam(x, y));
            PostMessage(fromHwnd, WM_LBUTTONUP, 0, MakeLParam(x, y));
        }
        public static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }
        #endregion

        #region 键盘API

        [DllImport("USER32.DLL")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        //导入模拟键盘的方法

        #endregion


        /// <summary>
        /// 键盘输入，只能输入小写字母与数字
        /// </summary>
        /// <param name="valuestr"></param>
        /// <param name="spliteTime">间隔时间ms</param>
        public static void KeyInputStringAndNumber(string valuestr, int spliteTime)
        {
            byte[] src_bytes = Encoding.ASCII.GetBytes(valuestr.ToUpper());
            //
            Vevisoft.Log.VeviLog2.WriteLogInfo("Input Value:"+valuestr);
            for (int i = 0; i < src_bytes.Length; i++)
            {
                Vevisoft.Log.VeviLog2.WriteLogInfo("Input Value:" + src_bytes[i]);
                keybd_event(src_bytes[i], 0, 0, 0);
                keybd_event(src_bytes[i], 0, 2, 0);
                //
                Thread.Sleep(spliteTime);
            }
        }

        public static void KeySendESC()
        {
            //27
            keybd_event(27, 0, 0, 0);
            keybd_event(27, 0, 2, 0);
        }

        public static void KeySendTab()
        {
            keybd_event(0x9, 0, 0, 0);
            keybd_event(0x9, 0, 2, 0);
        }

        public static void KeySendCtrlA()
        {
            keybd_event(0x11, 0, 0, 0);
            keybd_event(65, 0, 0, 0);
            keybd_event(0x11, 0, 2, 0);
            keybd_event(65, 0, 2, 0);
        }

        /// <summary>
        /// 上箭头
        /// </summary>
        public static void KeySendArrowUp()
        {
            keybd_event(38, 0, 0, 0);
            keybd_event(38, 0, 2, 0);
        }

        public static void KeySendArrowDown()
        {
            keybd_event(40, 0, 0, 0);
            keybd_event(40, 0, 2, 0);
        }

        public static void KeySendArrowLeft()
        {
            keybd_event(37, 0, 0, 0);
            keybd_event(37, 0, 2, 0);
        }

        public static void KeySendArrowRight()
        {
            keybd_event(39, 0, 0, 0);
            keybd_event(39, 0, 2, 0);
        }

        public static void KeySendEnter()
        {
            keybd_event(0xD, 0, 0, 0);
            keybd_event(0xD, 0, 2, 0);
        }

        public static void KeySendBackSpace()
        {
            keybd_event(8, 0, 0, 0);
            keybd_event(8, 0, 2, 0);
        }

        public static void KeySendMouseLeft()
        {
            keybd_event(1, 0, 0, 0);
            keybd_event(1, 0, 2, 0);
        }

        public static void KeySendMouseRight()
        {
            keybd_event(2, 0, 0, 0);
            keybd_event(2, 0, 2, 0);
        }

        public static void KeySendCtrlV()
        {
            keybd_event(0x11, 0, 0, 0);
            keybd_event(86, 0, 0, 0);
            keybd_event(0x11, 0, 2, 0);
            keybd_event(86, 0, 2, 0);
        }

        public static void KeySendCtrlC()
        {
            keybd_event(0x11, 0, 0, 0);
            keybd_event(67, 0, 0, 0);
            keybd_event(0x11, 0, 2, 0);
            keybd_event(67, 0, 2, 0);
        }

        public static void KeySendAltF4()
        {
            keybd_event(18, 0, 0, 0);
            keybd_event(115, 0, 0, 0);
            keybd_event(18, 0, 2, 0);
            keybd_event(115, 0, 2, 0);
        }

        /// <summary>
        /// del按键
        /// </summary>
        public static void KeySendDelete()
        {
            keybd_event(46, 0, 0, 0);
            keybd_event(46, 0, 2, 0);
        }
    }
}
