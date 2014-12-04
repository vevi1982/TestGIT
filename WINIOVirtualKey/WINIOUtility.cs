using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：WINIOVirtualKey
 *文件名：  WINIOUtility
 *版本号：  V1.0.0.0
 *唯一标识：eaaae915-1ee0-4815-be24-7c41e72d1e23
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/1 13:30:56
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/1 13:30:56
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace WINIOVirtualKey
{
    public class WINIOUtility
    {
        public const int KBC_KEY_CMD = 0x64;//输入键盘按下消息的端口
        public const int KBC_KEY_DATA = 0x60;//输入键盘弹起消息的端口
        //[DllImport("WinIo64.dll")]
        //public static extern bool InitializeWinIo();

        //[DllImport("WinIo64.dll")]
        //public static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal,byte bSize);

        //[DllImport("WinIo64.dll")]
        //public static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal,byte bSize);

        //[DllImport("WinIo64.dll")]
        //public static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize,IntPtr PhysicalMemoryHandle);
        //[DllImport("WinIo64.dll")]
        //public static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle,byte pbLinAddr);
        //[DllImport("WinIo64.dll")]
        //public static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        //[DllImport("WinIo64.dll")]
        //public static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        //[DllImport("WinIo64.dll")]
        //public static extern void ShutdownWinIo();
        //[DllImport("user32.dll")]
        //public static extern int MapVirtualKey(uint Ucode, uint uMapType);
        [DllImport("WinIo64.dll")]
        private static extern bool InitializeWinIo();
        [DllImport("WinIo64.dll")]
        private static extern bool GetPortVal(IntPtr wPortAddr, out int pdwPortVal, byte bSize);
        [DllImport("WinIo64.dll")]
        private static extern bool SetPortVal(uint wPortAddr, IntPtr dwPortVal, byte bSize);
        [DllImport("WinIo64.dll")]
        private static extern byte MapPhysToLin(byte pbPhysAddr, uint dwPhysSize, IntPtr PhysicalMemoryHandle);
        [DllImport("WinIo64.dll")]
        private static extern bool UnmapPhysicalMemory(IntPtr PhysicalMemoryHandle, byte pbLinAddr);
        [DllImport("WinIo64.dll")]
        private static extern bool GetPhysLong(IntPtr pbPhysAddr, byte pdwPhysVal);
        [DllImport("WinIo64.dll")]
        private static extern bool SetPhysLong(IntPtr pbPhysAddr, byte dwPhysVal);
        [DllImport("WinIo64.dll")]
        private static extern void ShutdownWinIo();
        [DllImport("user32.dll")]
        private static extern int MapVirtualKey(uint Ucode, uint uMapType);
        //
        //初始化，安装驱动？

        public bool Initialize()
        {
            if (InitializeWinIo())
            {
                KBCWait4IBE();
                return true;
            }

            else
            {
                //MessageBox.Show("failed");
                return false;
            }
        }

        //应该是调用结束要用的，卸载驱动？

        public void Shutdown()
        {
            ShutdownWinIo();
            KBCWait4IBE();
        }



        ///等待键盘缓冲区为空

        public void KBCWait4IBE()
        {
            int dwVal = 0;
            do
            {
                bool flag = GetPortVal((IntPtr)0x64, out dwVal, 1);
            }
            //while ((dwVal &amp;amp; 0x2) &amp;gt; 0);
            while ((dwVal & 0x2) > 0);

        }

        /// 模拟键盘标按下

        public void KeyDown(Keys vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr) btScancode, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            var ret = SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
        }

        /// 模拟键盘弹出

        public void KeyUp(Keys vKeyCoad)
        {
            int btScancode = 0;
            btScancode = MapVirtualKey((uint)vKeyCoad, 0);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)0x60, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            KBCWait4IBE();
            SetPortVal(KBC_KEY_DATA, (IntPtr)(btScancode | 0x80), 1);
        }

        /// 模拟一次按键
        public void KeyDownUp(Keys vKeyCoad)
        {
            KeyDown(vKeyCoad);
            Thread.Sleep(100);
            KeyUp(vKeyCoad);
        }

        public void InputKeyValues(string value)
        {
            //KeyDownUp(Keys.Tab);
            Thread.Sleep(1000);
            var array = value.ToUpper().ToCharArray();
            foreach (char key in array)
            {
                KBCWait4IBE();
                var theKey = (Keys)Convert.ToInt32(key);
                KeyDown(theKey);
            }
            
        }
        public void TestA()
        {
            KeyDown(Keys.A);
            //KeyUp(Keys.A);
        }
        private uint VK_NUMLOCK = 0x90;
        public void NumLockKeyClick()
        {
            //
            KeyDown(Keys.NumLock);
            //
            SetPortVal(KBC_KEY_CMD, (IntPtr)0xD2, 1);
            SetPortVal(KBC_KEY_DATA, (IntPtr)MapVirtualKey(VK_NUMLOCK, 0), 1);
            Thread.Sleep(500);
            SetPortVal(KBC_KEY_CMD, (IntPtr) 0xD2, 1);
            SetPortVal(KBC_KEY_DATA, (IntPtr)(MapVirtualKey(VK_NUMLOCK, 0)|0x80), 1);
            //ShutdownWinIo();
            //

        }
    }
}
