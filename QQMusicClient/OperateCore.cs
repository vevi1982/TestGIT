using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Vevisoft.WebOperate;
using Vevisoft.WindowsAPI;

namespace QQMusicClient
{
    /// <summary>
    /// QQ音乐自动下载操作
    /// </summary>
    public class OperateCore
    {
        /// <summary>
        /// 最大延时
        /// </summary>
        public int maxTime = 30;
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

        private const string QQPassErrorMsg = "QQ密码错误！";
        //
        public void DoOnce()
        {
            //1.改变IP

            #region CHangeIP

            if (AppConfig.ChangeIP)
            {
                var adsl = new AdslDialHelper();

                if (adsl.IsConnectedInternet())
                {
                    //连接上的时候 先断开
                    adsl.StopDailer(AppConfig.ADSLName);
                    while (adsl.IsConnectedInternet())
                    {
                        Thread.Sleep(500);
                    }
                }
                //连接ADSL
                adsl.StartDailer(AppConfig.ADSLName, AppConfig.ADSLUserName, AppConfig.ADSLPass);
                while (!adsl.IsConnectedInternet())
                {
                    Thread.Sleep(500);
                }
                //TODO...是否需要判断IP是否重复

            }

            #endregion

            //2.打开软件 判断软件是否显示打开并显示？？
            if (string.IsNullOrEmpty(AppConfig.AppPath))
                throw new Exception("没有设置QQ音乐s软件路径");
            if (!File.Exists(AppConfig.AppPath))
                throw new Exception("软件路径错误，找不到软件！");
            //
            System.Diagnostics.Process.Start(AppConfig.AppPath);
            //
            IntPtr mainHanle = GetMainForm();
            //
            if (mainHanle == IntPtr.Zero)
                throw new Exception("没有获取到主窗体句柄");
            //
            //主窗体加载完成
            Thread.Sleep(2000);
            //
            
            //是否已登录,登录则推出
            QQLogOut(mainHanle);
            //登录QQ
            var successOK = false;
            do
            {
                successOK = QQLoginSuccess(mainHanle);
            } while (!successOK);
            //点击试听列表

            //下载歌曲 并启动判断程序

            //下载完毕，删除歌曲

            //提交 完成


        }
        private bool QQLoginSuccess(IntPtr mainHanle)
        {
            try
            {
                QQLogin(mainHanle);
            }
            catch (Exception e)
            {
                if (e.Message == QQPassErrorMsg)
                    return false;
                else throw e;
            }
            return true;
        }
        private void QQLogin( IntPtr mainHandle)
        {
            //获取QQ号
            var qqlist = GetQQNoAndPass();
            //登录按钮 125 27
            //移动鼠标 到 程序 标题栏
            var mainRect = GetFormRect(mainHandle);
            MouseKeyBoradUtility.SetCursorPos(300 + mainRect.Left, 25 + mainRect.Top);
            //右键点击
            MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            //
            //等待右键菜单弹出
            Thread.Sleep(1000);
            //
            //【登录】 快捷键 L 并且 等待1S
            MouseKeyBoradUtility.KeyInputStringAndNumber("l", 1000);
            //判断是否有 【登录对话框】
            IntPtr msgHandle = SystemWindowsAPI.GetForegroundWindow();
            var count = 0;
            while (msgHandle == mainHandle && count < maxTime)
            {
                Thread.Sleep(1000);
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                count++;
            }
            if (count >= maxTime)
                throw new Exception("等待超时，登录,等待登录框。");
            //QQ号 550,290  密码 550 320  登录 450，400
            //
            MouseKeyBoradUtility.SetCursorPos(550 + mainRect.Left, 290 + mainRect.Top);
            MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            Thread.Sleep(500);
            //输入QQ号
            MouseKeyBoradUtility.KeyInputStringAndNumber(qqlist[0],50);
            //
            MouseKeyBoradUtility.SetCursorPos(550 + mainRect.Left, 320 + mainRect.Top);
            MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //如果有原来的密码，需要删除。所以先按一堆的backspace
            for (int i = 0; i < 18; i++)
            {
                MouseKeyBoradUtility.SendBackSpace();
                Thread.Sleep(50);
            }
            MouseKeyBoradUtility.KeyInputStringAndNumber(qqlist[1], 50);
            //单击登录按钮
            MouseKeyBoradUtility.SetCursorPos(450 + mainRect.Left, 400 + mainRect.Top);
            MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            //此时 可能出现多种情况。
            //1.正常情况 登录框 关闭 登陆完成
            //2.密码错误 
            //3.需要输入验证码
            //超时判断
            count = 0;
            msgHandle = SystemWindowsAPI.GetForegroundWindow();
            while (msgHandle != mainHandle && count < maxTime)
            {
                Thread.Sleep(1000);
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                //安全中心出现
                var safeHandle = GetQQSafeCenterForm();
                if (safeHandle != IntPtr.Zero && safeHandle == msgHandle)
                {
                    if (IsPassWrong(safeHandle))
                    {
                        //关闭安全中心窗体，提交错误QQ并重新获取QQ号密码
                        SystemWindowsAPI.ShowWindow(safeHandle, 0);
                        MouseKeyBoradUtility.KeySendESC();
                        SendPassErrorQQToServer();
                        //
                        throw new Exception(QQPassErrorMsg);
                    }else if (IsNeedVeryCode(safeHandle))
                    {
                        SendNeedVeryCodeQQToServer();
                        //输入验证码
                        SystemWindowsAPI.ShowWindow(safeHandle, 0);
                        InputVeryCode(safeHandle);

                    }
                }
                Console.WriteLine(count);
                count++;
            }
            if (count >= maxTime)
                throw new Exception("等待超时，退出登录,等待更改用户提示框。");

            
        }

        private void SendPassErrorQQToServer()
        {
            //throw new NotImplementedException();
        }

        private void QQLogOut(IntPtr mainHandle)
        {
            //移动鼠标 到 程序 标题栏
            var mainRect = GetFormRect(mainHandle);
            MouseKeyBoradUtility.SetCursorPos(300 + mainRect.Left, 25 + mainRect.Top);
            //右键点击
            MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_RIGHTDOWN | MOUSEEVENTF_RIGHTUP, 0, 0, 0, 0);
            //
            //等待右键菜单弹出
            Thread.Sleep(1000);
            //
            //【更改用户】 快捷键 U 并且 等待1S（eg:如果没有登录，则没有此快捷键）
            MouseKeyBoradUtility.KeyInputStringAndNumber("u", 1000);
            //判断是否有 【更改用户提示框】
            IntPtr msgHandle = SystemWindowsAPI.GetForegroundWindow();
            if (msgHandle == mainHandle)
            {
                //说明没有登录
                return;
            }
            //有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】
            var msgRect = GetFormRect(msgHandle);
            MouseKeyBoradUtility.SetCursorPos(460 + mainRect.Left, 355 + mainRect.Top);
            MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
           //
            msgHandle = SystemWindowsAPI.GetForegroundWindow();//如果当前窗体是主窗体，登录窗体还没有出来
            //超时判断
            var count = 0;
            while (msgHandle == mainHandle&&count<maxTime)
            {
                Thread.Sleep(1000);
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                count++;
            }
            if (count >= maxTime)
                throw new Exception("等待超时，退出登录,等待更改用户提示框。");
            
            //
            msgRect = GetFormRect(msgHandle);
            var loginHandle = GetLoginForm();
            if (loginHandle == msgHandle)
            {
                //关闭它
                MouseKeyBoradUtility.SetCursorPos(598 + mainRect.Left, 204 + mainRect.Top);
                MouseKeyBoradUtility.mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
            }
            //
            //判断是否关闭
            count = 0;
            msgHandle = SystemWindowsAPI.GetForegroundWindow();
            while (msgHandle == mainHandle && count < maxTime)
            {
                Thread.Sleep(1000);
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                count++;
            }
            if (count >= maxTime)
                throw new Exception("等待超时，退出登录,等待登陆框关闭。");
        }

        private bool QQAlreadyLogin()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 获取主窗体句柄
        /// </summary>
        /// <returns></returns>
        public IntPtr GetMainForm()
        {
            const string caption = "QQ音乐";//TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 1000, maxTime);
            return handle;
        }
        /// <summary>
        /// 获取登录窗体句柄
        /// </summary>
        /// <returns></returns>
        public IntPtr GetLoginForm()
        {
            const string caption = "QQ音乐登录";//TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, 10);
            return handle;
        }
        /// <summary>
        /// 获取QQ安全中心 窗体句柄。一般是发生密码错误时。
        /// </summary>
        /// <returns></returns>
        public IntPtr GetQQSafeCenterForm()
        {
            const string caption = "QQ安全中心";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, 10);
            return handle;
        }
        /// <summary>
        /// QQ安全中心 密码错误
        /// </summary>
        /// <param name="safecenterHandle"></param>
        /// <returns></returns>
        public bool IsPassWrong(IntPtr safecenterHandle)
        {
            IntPtr handle = SystemWindowsAPI.FindWindowEx(safecenterHandle, IntPtr.Zero, "Button", "找回密码");

            return handle != IntPtr.Zero;
        }
        /// <summary>
        /// QQ安全中心 需要输入验证码
        /// </summary>
        /// <param name="safeCenterHandle"></param>
        /// <returns></returns>
        public bool IsNeedVeryCode(IntPtr safeCenterHandle)
        {
            IntPtr hwnd1 = SystemWindowsAPI.FindWindowEx(safeCenterHandle, IntPtr.Zero, "Static", "为了您的帐号安全，本次登录需输验证码。");
            return hwnd1 != IntPtr.Zero;              
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
        /// <summary>
        /// 获取QQ号
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetQQNoAndPass()
        {
            var qqList=new string[2];

            qqList[0] = "254430994";
            qqList[1] = "5218246739";

            return qqList;
        }


    
    }
}
