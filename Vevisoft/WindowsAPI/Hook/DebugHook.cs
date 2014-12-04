using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：Vevisoft.WindowsAPI.Hook
 *文件名：  DebugHook
 *版本号：  V1.0.0.0
 *唯一标识：3284c496-72c8-4d14-b13f-76341085922e
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/1 20:15:31
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/1 20:15:31
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace Vevisoft.WindowsAPI.Hook
{
    public class DebugHook
    {
        private IntPtr m_Hook = IntPtr.Zero;/// 键盘钩子句柄
        private HookUtility.HookProc m_KeyboardHookProcedure;// 键盘钩子委托实例
        
        public bool StartHook()
        {
            //var pInstance = (IntPtr)4194304;//钩子所在实例的句柄
            if (this.m_Hook == IntPtr.Zero)//如果键盘的句柄为空
            {
                this.m_KeyboardHookProcedure = MyHookProc;//声明一个托管钩子
                this.m_Hook = HookUtility.SetWindowsHookEx((int)HookUtility.HookType.WH_KEYBOARD_LL, m_KeyboardHookProcedure, Marshal.GetHINSTANCE(Assembly.GetExecutingAssembly().GetModules()[0]), 0);
                if (this.m_Hook == IntPtr.Zero)//如果安装失败
                {
                    this.UnHook();//卸载钩子
                    return false;
                }
            }
            //isInstall = true;//安装了钩子
            return true;
        }
        public void UnHook()
        {
            bool retMouse = true;
            if (m_Hook != IntPtr.Zero)
            {
                retMouse = HookUtility.UnhookWindowsHookEx(m_Hook);
                m_Hook = IntPtr.Zero;
            }
            ////如果卸下钩子失败 
            //if (!(retMouse)) throw new Exception("UnhookWindowsHookEx   failed. ");
        }

        private IntPtr MyHookProc(int code, int wparam, IntPtr lparam)
        {
            if (code < 0) return Hook.HookUtility.CallNextHookEx(m_Hook, code, wparam, lparam); //返回，让后面的程序处理该消息           
            
            if (wparam == (decimal)HookUtility.MsgType.WM_CREATE)
            {
                //获取窗体名称 等待。。。

            }
            if (wparam == (decimal) HookUtility.MsgType.WM_SETTEXT)
            {
                
            }
            return IntPtr.Zero;
            return HookUtility.CallNextHookEx(m_Hook, code, wparam, lparam);
        }

        ~DebugHook()
        {
            UnHook();
        }
    }
}
