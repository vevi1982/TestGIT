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
        #endregion

        #region 键盘API
         [DllImport("USER32.DLL")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);  //导入模拟键盘的方法

        #endregion


         /// <summary>
         /// 键盘输入，只能输入小写字母与数字
         /// </summary>
         /// <param name="valuestr"></param>
         /// <param name="spliteTime">间隔时间ms</param>
         public static void KeyInputStringAndNumber(string valuestr, int spliteTime)
         {
             byte[] src_bytes = Encoding.ASCII.GetBytes(valuestr.ToUpper());
             for (int i = 0; i < src_bytes.Length; i++)
             {
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
         public static void SendEnter()
         {
             keybd_event(0xD, 0, 0, 0);
             keybd_event(0xD, 0, 2, 0);
         }
         public static void SendBackSpace()
         {
             keybd_event(8, 0, 0, 0);
             keybd_event(8, 0, 2, 0);
         }
         public static void SendMouseLeft()
         {
             keybd_event(1, 0, 0, 0);
             keybd_event(1, 0, 2, 0);
         }
         public static void SendMouseRight()
         {
             keybd_event(2, 0, 0, 0);
             keybd_event(2, 0, 2, 0);
         }
         public static void SendCtrlV()
         {
             keybd_event(0x11, 0, 0, 0);
             keybd_event(86, 0, 0, 0);
             keybd_event(0x11, 0, 2, 0);
             keybd_event(86, 0, 2, 0);
         }
         public static void SendCtrlC()
         {
             keybd_event(0x11, 0, 0, 0);
             keybd_event(67, 0, 0, 0);
             keybd_event(0x11, 0, 2, 0);
             keybd_event(67, 0, 2, 0);
         }
    }
}
