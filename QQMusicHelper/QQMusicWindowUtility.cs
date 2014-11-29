using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vevisoft.WindowsAPI;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：QQMusicHelper
 *文件名：  QQMusicWindowUtility
 *版本号：  V1.0.0.0
 *唯一标识：f981c337-c62f-406b-98aa-22b3d8150ce8
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/11/24 16:34:30
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/11/24 16:34:30
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace QQMusicHelper
{
    public class QQMusicWindowUtility
    {
        public const string MainFormText = "QQ音乐";
        public const string LoginFormText = "QQ音乐登录";
        //
        public static IntPtr MainFormHandle = IntPtr.Zero;
        public static IntPtr LoginFormHandle;
        //
        public static bool MainHandleChange = false;
        public static bool LoginHandleChange = false;
        //
        /// <summary>
        /// 获取窗体
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetQQMusicForm()
        {
            MainHandleChange = false;
            LoginHandleChange = false;
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowCallBack;
            if (SystemWindowsAPI.EnumWindows(callback, 0) == 0)
            {
                //throw new Exception("枚举窗体函数失败！"); //函数枚举失败
            }
            //
            return MainFormHandle;
        }

        private static bool EnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (strTitle.ToString() == MainFormText)
            {
                if (MainFormHandle != hwnd)
                {
                    //主窗体
                    MainFormHandle = hwnd;
                    MainHandleChange = true;
                }
            }
            if (strTitle.ToString() == LoginFormText)
            {if (LoginFormHandle != hwnd)
            {
                LoginFormHandle = hwnd;
                LoginHandleChange = true;
            }
            }

            return true;
        }
    }
}
