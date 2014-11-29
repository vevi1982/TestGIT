using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Vevisoft.WindowsAPI;

namespace QQMusicHelper
{
    public class QQMusicOperateHelper
    {
        /// <summary>
        /// 跑出的错误信息，需要处理
        /// </summary>
        public static readonly string QQPassErrorMsg = "QQ密码错误！";

        #region 启动QQMusic
        /// <summary>
        /// 启动QQ音乐
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static bool StartQQMusic(string filepath,string argument)
        {
            Vevisoft.Utility.ProcessUtility.KillProcess("QQMusic", filepath);
            if (!File.Exists(filepath))
            {
                //OnShowInStatusBarEvent("软件路径错误，找不到软件！");
                throw new Exception("软件路径错误，找不到软件！");
            }
            var process = System.Diagnostics.Process.Start(filepath, argument);
            if (process == null)
                return false;
            while (!process.Responding)
            {
                Thread.Sleep(1000);
            } Thread.Sleep(1000);
            WaitMainStartOver();
            return true;
        }
        /// <summary>
        /// 等待QQ音乐启动完毕,QQMusicExternal为2个线程的时候。超时为10S
        /// </summary>
        public static void WaitMainStartOver()
        {
            //
            var count = 10;

            while (GetProcessCount("QQMusicExternal") < 2 && count > 0)
            {
                count--;
                Thread.Sleep(1000);
            }
            if (count <= 0)
                throw new Exception("启动没有完成！");
        }
        private static int GetProcessCount(string processName)
        {
            return System.Diagnostics.Process.GetProcesses().Count(process => process.ProcessName == processName);
        }

        public static bool IsQQMusicStart()
        {
            return GetProcessCount("QQMusic") == 1;
        }
        #endregion

        #region 注销QQ
        private static void OnlyLogOut()
        {
            var mainHandle = GetQQMusicHandle();
            if (mainHandle == IntPtr.Zero)
                return;
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //移动鼠标 到 程序 标题栏
            ClickChangeUser(mainHandle);
            //
            //判断是否有 【更改用户提示框】
            Thread.Sleep(2 * 1000);
            //IntPtr msgHandle = SystemWindowsAPI.GetForegroundWindow();
            //if (msgHandle == mainHandle)
            //{
            //    //说明 弹出框 说明没有登录，直接返回
            //    //OnShowInStatusBarEvent("没有发现【弹出框】 说明没有登录，直接返回!");
            //    return;
            //}
            //OnShowInStatusBarEvent("有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】!");
            //有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】
            // 关闭【更改用户提示框】
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.ChangeUserAlertClosePt);
        }
        /// <summary>
        /// 退出登录，如果没有登录，那么返回操作
        /// </summary>
        /// <param name="mainHandle"></param>
        public static void LogOutQQMusic()
        {
            var mainHandle = GetQQMusicHandle();
            if (mainHandle == IntPtr.Zero)
                return;
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //移动鼠标 到 程序 标题栏
            ClickChangeUser(mainHandle);
            //
            //判断是否有 【更改用户提示框】
            Thread.Sleep(2 * 1000);
            IntPtr msgHandle = SystemWindowsAPI.GetForegroundWindow();
            if (msgHandle == mainHandle)
            {
                //说明 弹出框 说明没有登录，直接返回
                //OnShowInStatusBarEvent("没有发现【弹出框】 说明没有登录，直接返回!");
                return;
            }
            //OnShowInStatusBarEvent("有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】!");
            //有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】
            // 关闭【更改用户提示框】
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.ChangeUserAlertClosePt);
            //
            //【QQ登录窗体】是否出现
            //OnShowInStatusBarEvent("判断【登陆窗体】是否出现!");
            var loginHandle = GetLoginForm();
            if (loginHandle != IntPtr.Zero)
            {
                //OnShowInStatusBarEvent("关闭【登陆窗体】!");
                //关闭登录窗体
                MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormClosePt);
            }
            else
            {
                //OnShowInStatusBarEvent("【登陆窗体】等待超时!");
                throw new Exception("等待超时，【更改用户提示框】没有出现。");
            }
            //
            //判断是否关闭,即当前窗体是否主窗体
            //OnShowInStatusBarEvent("判断是否关闭,即当前窗体是否主窗体！");
            //if (!ForgroundIsMain(mainHandle, maxTime))
            //{
            //    OnShowInStatusBarEvent("等待超时，【QQ登陆框】没有关闭。!");
            //    throw new Exception("等待超时，【QQ登陆框】没有关闭。");
            //}

        }
        #endregion

        #region 登录QQ

        public static bool LoginSmart(string qqno, string qqpass,int passwrongtimes)
        {
            var mainHandle = GetQQMusicHandle();
            if (mainHandle == IntPtr.Zero)
                return false;
            //点击左上登录，查看是否有登录窗体
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            MouseSetPositonAndLeftClick(mainHandle,PositionInfoQQMusic.MainCaptionLoginButtonPt);
            MouseSetPositonAndLeftClick(mainHandle,new Point(1,1));
            ////等待加载完成。。。。
            //Thread.Sleep(5*1000);
            //等待验证登录窗体  【登录对话框】
            IntPtr msgHandle = GetLoginForm();
            //查找15S后无果，跑出异常
            if (msgHandle == IntPtr.Zero)
            {
                //窗体没有出现，是否已登录？？
                QQMusicUserInfo.GetUserInfoForm();
                //已登录，查看登录账号                
                //是当前账号，返回
                if (QQMusicUserInfo.LoginUserNo == qqno)
                {
                    //不用登陆了
                    return true;
                }
                //不是当前账号，退出登录
                OnlyLogOut();
            }
            //开始登陆
            msgHandle = GetLoginForm();
            if(msgHandle==IntPtr.Zero)
                throw new Exception("登陆框没有出现!!");
            //
            Thread.Sleep(1000);
            //输入用户名密码
            InputPassByDiag(msgHandle, qqno, qqpass, passwrongtimes);
            //此时 可能出现多种情况。
            //1.正常情况 登录框 关闭 登陆完成
            //2.密码错误 
            //3.需要输入验证码
            //4.账号密码没有输入正确，登陆框还是存在
            //超时判断
            
            //Thread.Sleep(AppConfig.TimeAlertCHangeUser * 1000);
            var count = 0;
            var safeHandle = GetQQSafeCenterForm();
            while (safeHandle != IntPtr.Zero && count < 3)
            {
                //安全中心出现，判断10S
                //处理一次
                DealWithQQSafeForm(safeHandle, qqno);
                //OnShowInStatusBarEvent("QQ安全中心" + count);
                Console.WriteLine("QQ安全中心" + count);
                count++;
                safeHandle = GetQQSafeCenterForm();
            }
            if (count >= 3)
            {
                //OnShowInStatusBarEvent("处理错误，退出登录,处理QQ安全中心失败。");
                throw new Exception("处理错误，退出登录,处理QQ安全中心失败。");
            }
            //判断登陆框是否还存在，如果不存在，那么登陆成功，如果存在那么失败
            msgHandle = GetLoginForm();
            if(msgHandle!=IntPtr.Zero)
                throw new Exception("登陆失败！！");
            return true;
        }

        /// <summary>
        /// 登录QQ音乐
        /// </summary>
        /// <param name="qqno"></param>
        /// <param name="qqpass"></param>
        /// <returns></returns>
        public static bool LoginQQ(string qqno, string qqpass)
        {
            var mainHandle = GetQQMusicHandle();
            if (mainHandle == IntPtr.Zero)
                return false;
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //点击 标题栏 图标
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainCaptionLoginButtonPt);
            //点击后移动鼠标位置 以防止出现【用户信息框】
            MouseSetPositonAndLeftClick(mainHandle, new Point(1, 1));
            //            
            //判断是否有 【登录对话框】
            IntPtr msgHandle = GetLoginForm();
            //查找5S后无果，跑出异常
            if (msgHandle == IntPtr.Zero)
            {
                //OnShowInStatusBarEvent("等待超时，登录,【QQ登录框】没有出现！");
                throw new Exception("等待超时，登录,【QQ登录框】没有出现。");
            }
            //
            Thread.Sleep(1000);
            //输入用户名密码
            InputPass(mainHandle, qqno, qqpass);
            //此时 可能出现多种情况。
            //1.正常情况 登录框 关闭 登陆完成
            //2.密码错误 
            //3.需要输入验证码
            //超时判断
            //Thread.Sleep(AppConfig.TimeAlertCHangeUser * 1000);
            var count = 0;
            var safeHandle = GetQQSafeCenterForm();
            while (safeHandle != IntPtr.Zero && count < 3)
            {
                //安全中心出现，判断10S
                //处理一次
                DealWithQQSafeForm(safeHandle, qqno);
                //OnShowInStatusBarEvent("QQ安全中心" + count);
                Console.WriteLine("QQ安全中心" + count);
                count++;
                safeHandle = GetQQSafeCenterForm();
            }
            if (count >= 3)
            {
                //OnShowInStatusBarEvent("处理错误，退出登录,处理QQ安全中心失败。");
                throw new Exception("处理错误，退出登录,处理QQ安全中心失败。");
            }
            return true;
        }

        #region 安全中心处理
          /// <summary>
        ///安全中心 窗体是否还存在，存在则说明验证码输入错误。
        ///如果正确，密码错误那么还是出现安全中心.只不过是提示密码错误
        ///当前窗体是否主窗体？    
        /// </summary>
        /// <param name="safeHandle"></param>
        /// <param name="CurrentQQNo"></param>
        private static void DealWithQQSafeForm(IntPtr safeHandle, string CurrentQQNo)
        {
            //OnShowInStatusBarEvent("安全中心出现");
            Console.WriteLine("QQ安全中心出现");
            //判断类型
            if (IsPassWrong(safeHandle)) //密码错误
            {
                Console.WriteLine("密码错误");
                //关闭安全中心窗体，提交错误QQ并重新获取QQ号密码
                SystemWindowsAPI.SetActiveWindow(safeHandle);
                MouseKeyBoradUtility.KeySendESC();
                //SendPassErrorQQToServer(CurrentQQNo);
                //
                throw new Exception(QQPassErrorMsg);
            }
            else if (IsNeedVeryCode(safeHandle)) //需要输入验证码
            {
                //OnShowInStatusBarEvent("登录需要验证码");
                Console.WriteLine("登录需要验证码");
                //SendNeedVeryCodeQQToServer(CurrentQQNo);
                //输入验证码
                SystemWindowsAPI.SetActiveWindow(safeHandle);
                InputVeryCodeOnLogin(safeHandle);
            }
        }
        #endregion

        #region 输入用户名密码
        /// <summary>
        /// QQ号密码输入，点击登录按钮
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="qqno"></param>
        /// <param name="pass"></param>
        private static void InputPass(IntPtr mainHandle, string qqno, string pass)
        {  
            //var mainHandle = GetQQMusicHandle();
            //SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormText1Pt);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            Thread.Sleep(500);
            //输入QQ号
            MouseKeyBoradUtility.KeyInputStringAndNumber(qqno, 50);
            //
            MouseKeyBoradUtility.KeySendTab();
            //MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormPassPt);
            //如果有原来的密码，需要删除。所以先按一堆的backspace
            for (int i = 0; i < 18; i++)
            {
                MouseKeyBoradUtility.KeySendBackSpace();
                Thread.Sleep(50);
            }
            Vevisoft.Log.VeviLog2.WriteLogInfo(qqno + ";" + pass);
            MouseKeyBoradUtility.KeyInputStringAndNumber(pass, 50);
            //单击登录按钮
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormOKButtonPt);
        }

        private static void InputPassByDiag(IntPtr loginHandle, string qqno, string pass,int count)
        {
            //
            MouseSetPositonAndLeftClick(loginHandle, PositionInfoQQMusic.LoginFormSelfUserPt);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            Thread.Sleep(500);
            //
            var interval = 50;
            //第二次输入为500ms
            if (count == 1)
                interval = 500;
            if (count > 1)
                interval = 500*count;
            //输入QQ号
            MouseKeyBoradUtility.KeyInputStringAndNumber(qqno, interval);
            //  
            MouseKeyBoradUtility.KeySendTab();
            MouseSetPositonAndLeftClick(loginHandle, PositionInfoQQMusic.LoginFormSelfPassPt);
            //MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormPassPt);
            //如果有原来的密码，需要删除。所以先按一堆的backspace
            for (int i = 0; i < 18; i++)
            {
                MouseKeyBoradUtility.KeySendBackSpace();
                Thread.Sleep(interval);
            }
            Vevisoft.Log.VeviLog2.WriteLogInfo(qqno + ";" + pass);
            
            //单击登录按钮
            //var aa = pass.ToCharArray();
            //foreach (char c in aa)
            //{
            //    Vevisoft.Log.VeviLog2.WriteLogInfo(logText: "Pass:"+c.ToString());
            //    MouseSetPositonAndLeftClick(loginHandle, PositionInfoQQMusic.LoginFormSelfPassPt);
                 
            //}
            MouseKeyBoradUtility.KeyInputStringAndNumber(pass, interval);   
            MouseSetPositonAndLeftClick(loginHandle, PositionInfoQQMusic.LoginFormSelfOkBtnPt);    
       
        }
        #endregion

        #region 鼠标移动并单击
        /// <summary>
        /// 右键 更改用户
        /// </summary>
        /// <param name="mainHandle"></param>
        private static void ClickChangeUser(IntPtr mainHandle)
        {
            //移动鼠标 到 程序 标题栏
            SetMousePosition(mainHandle, PositionInfoQQMusic.MainCaptionPt);
            //右键点击
            MouseKeyBoradUtility.MouseRightClick();
            //MouseKeyBoradUtility.MouseRightClickPostMsg(mainHandle, PositionInfoQQMusic.MainCaptionPt.X, PositionInfoQQMusic.MainCaptionPt.Y);
            //
            //等待右键菜单弹出
            Thread.Sleep(2 * 1000);
            //【更改用户】 快捷键 U 并且 等待1S（eg:如果没有登录，则没有此快捷键）
            MouseKeyBoradUtility.KeyInputStringAndNumber("u", 1000);
        }
        /// <summary>
        /// 移动鼠标 并单击
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="relativePt"></param>
        public static void MouseSetPositonAndLeftClick(IntPtr mainHandle, Point relativePt)
        {
            SetMousePosition(mainHandle, relativePt);
            MouseKeyBoradUtility.MouseLeftClick();
            //MouseKeyBoradUtility.MouseRightClickPostMsg(mainHandle, relativePt.X, relativePt.Y);
        }
        /// <summary>
        /// 移动鼠标到相应的位置
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="relativePt"></param>
        public static void SetMousePosition(IntPtr mainHandle, Point relativePt)
        {
            //是窗体在最前
            Vevisoft.WindowsAPI.SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //移动鼠标 到 程序 标题栏
            var mainRect = GetFormRect(mainHandle);
            MouseKeyBoradUtility.SetCursorPos(relativePt.X + mainRect.Left, relativePt.Y + mainRect.Top);
        }
        /// <summary>
        /// 获取窗体界面位置及大小
        /// </summary>
        /// <param name="handle">窗体句柄</param>
        /// <returns></returns>
        public static SystemWindowsAPI.RECT GetFormRect(IntPtr handle)
        {
            var formRec = new SystemWindowsAPI.RECT();
            SystemWindowsAPI.GetWindowRect(handle, ref formRec);
            return formRec;
        }
        #endregion

        #region 安全中心判断 是密码错误还是需要验证码
        /// <summary>
        /// QQ安全中心 密码错误
        /// </summary>
        /// <param name="safecenterHandle"></param>
        /// <returns></returns>
        public static bool IsPassWrong(IntPtr safecenterHandle)
        {
            IntPtr handle = SystemWindowsAPI.FindWindowEx(safecenterHandle, IntPtr.Zero, "Button", "找回密码");

            return handle != IntPtr.Zero;
        }

        /// <summary>
        /// QQ安全中心 需要输入验证码
        /// </summary>
        /// <param name="safeCenterHandle"></param>
        /// <returns></returns>
        public static bool IsNeedVeryCode(IntPtr safeCenterHandle)
        {
            IntPtr hwnd1 = SystemWindowsAPI.FindWindowEx(safeCenterHandle, IntPtr.Zero, "Static", "为了您的帐号安全，本次登录需输验证码。");
            return hwnd1 != IntPtr.Zero;
        }

        #endregion

        #region 输入验证码
        /// <summary>
        /// 登录的时候 输入 验证码
        /// </summary>
        /// <param name="safeHandle"></param>
        private static void InputVeryCodeOnLogin(IntPtr safeHandle)
        {
            SystemWindowsAPI.SetForegroundWindow(safeHandle);
            var rect = GetFormRect(safeHandle);
            var veryCode = Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCode(
                rect.Left + PositionInfoQQMusic.IDCodeSafeImgLeftTopPt.X,
                rect.Top + PositionInfoQQMusic.IDCodeSafeImgLeftTopPt.Y
                , PositionInfoQQMusic.IDCodeSafeImgSize.Width, PositionInfoQQMusic.IDCodeSafeImgSize.Height);
            //OnShowInStatusBarEvent("QQIDCode:" + veryCode);
            Console.WriteLine("QQIDCode:" + veryCode);
            //
            if (veryCode == "1009" || veryCode.Length != 4)
            {
                //OnShowInStatusBarEvent("验证码返回失败" + veryCode);
                throw new Exception(" 验证码返回失败");
            }
            //输入验证码
            IntPtr editHandle = SystemWindowsAPI.FindWindowEx(safeHandle, IntPtr.Zero, "Edit", null);
            if (editHandle != IntPtr.Zero)
            {
                //OnShowInStatusBarEvent("SetControlValue");
                Console.WriteLine("SetControlValue");
                SystemWindowsAPI.SetControlValue(editHandle, veryCode);
            }
            else
            {
                MouseSetPositonAndLeftClick(safeHandle, PositionInfoQQMusic.IDCodeSafeTextPt);
                Thread.Sleep(300);
                //防止有输入内容
                MouseKeyBoradUtility.KeySendCtrlA();
                Thread.Sleep(300);
                MouseKeyBoradUtility.KeySendBackSpace();
                Thread.Sleep(300);
                //输入验证码
                MouseKeyBoradUtility.KeyInputStringAndNumber(veryCode, 200);
                Console.WriteLine("KeyEvent");
            }

            Thread.Sleep(200);
            //点击确认按钮
            MouseSetPositonAndLeftClick(safeHandle, PositionInfoQQMusic.IDCodeSafeOKPt);
        }
        #endregion

        #region 获取窗体句柄
        /// <summary>
        /// 获取QQ安全中心 窗体句柄。一般是发生密码错误时。10S
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetQQSafeCenterForm()
        {
            return GetQQSafeCenterForm(5);
        }

        /// <summary>
        /// 获取QQ安全中心 窗体句柄。一般是发生密码错误时。
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetQQSafeCenterForm(int delay)
        {
            const string caption = "QQ安全中心";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, delay * 2);
            return handle;
        }
        /// <summary>
        /// 获取登录窗体句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetLoginForm()
        {
            //QQ音乐快速登录
            string caption2 = "QQ音乐快速登录";
            const string caption = "QQ音乐登录"; //TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, 20);
            //if (handle == IntPtr.Zero)
            //    handle = SystemWindowsAPI.FindMainWindowHandle(caption2, 500, 30);
            return handle;
        }

        /// <summary>
        /// 获取QQ音乐的主程序句柄,没有则抛出异常
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetQQMusicHandle()
        {
            const string caption = "QQ音乐"; //TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 1000, 30);
            //if (handle == IntPtr.Zero)
            //    throw new Exception("QQ音乐没有启动!");
            return handle;
        }
        #endregion
        #endregion

        #region 点击显示用户信息，及共享歌单信息
        public static void GetUserInfo()
        {
            var mainHandle = GetQQMusicHandle();
            if (mainHandle == IntPtr.Zero)
                return;
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //点击 标题栏 图标
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainCaptionLoginButtonPt);
            MouseSetPositonAndLeftClick(mainHandle, new Point(1, 1));
        }
        #endregion

        #region 下载是试听列表中的所有歌曲,只有操作.不点击 确定按钮
        /// <summary>
        /// 试听列表中点击 下载按钮 选择第二个选项（HQ）下载
        /// </summary>
        public static void DownLoadTryListSongs()
        {
            IntPtr mainhandle = GetQQMusicHandle();
            if (mainhandle == IntPtr.Zero)
                return;
            SystemWindowsAPI.SetForegroundWindow(mainhandle);
            //
             //
             MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MaintrySongListBtnPt);
             //等待加载
             Thread.Sleep(2000);
             while (!SystemWindowsAPI.IsExeNotResponse(mainhandle)
                 || !GetMainResponseByProcess())
             {
                 Thread.Sleep(1000);
             }
             //
             #region 点击下载按钮
             Thread.Sleep(500);
             //
             MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MainTryListenPanelFirstSongPt);
             //
             Thread.Sleep(500);
             //Ctrl A 全选
             MouseKeyBoradUtility.KeySendCtrlA();
             //
             Thread.Sleep(500);
             //
             MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MaintrySongListDownLoadButtonPt);
             //
             Thread.Sleep(500);
             //
             MouseKeyBoradUtility.KeySendArrowDown();
             //
             Thread.Sleep(500);
             //
             MouseKeyBoradUtility.KeySendArrowDown();
             //
             Thread.Sleep(500);
             //
             MouseKeyBoradUtility.KeySendEnter();
             #endregion
             //
             Thread.Sleep(2000);
        }
        #endregion

        #region 清理试听列表 下载列表
        /// <summary>
        /// 关闭程序，清除所有缓存及下载列表
        /// </summary>
        /// <param name="qqmusicPath"></param>
        /// <param name="dlPath"></param>
        /// <param name="catchPath"></param>
        public static void CloseAndCLearAll(string qqmusicPath, string dlPath, string catchPath)
        {
            //删除之前发送下载数据到服务器中
            //SendSongDataToServer(dlPath);
            ClearAllQQMusicList();
            ClearSongFolderAndCloseMain(qqmusicPath, dlPath, catchPath);
        }
        public static string GetPostSongData(string dlPath,string ordername)
        {
            var postdata = new StringBuilder();
            postdata.Append("[");
            var dir = new DirectoryInfo(dlPath);
            var fileCount = 0;
            if (dir.Exists)
            {
                foreach (FileInfo file in dir.GetFiles())
                {
                    //{id:"aaa",songName:"tong",singerName:"bb",orderName:"aaa",counter:1}
                    if (!file.Extension.ToLower().EndsWith("mp3"))
                        continue;
                    var songName = file.Name.Substring(0,file.Name.Length-file.Extension.Length);
                    var array = songName.Split('-');
                    if (array.Length == 2)
                    {
                        fileCount++;
                        if (postdata.ToString().Length > 1)
                            postdata.Append(",");
                        postdata.Append("{");
                        postdata.Append(
                            string.Format(
                                "\"id\":\"{0}\",\"songName\":\"{1}\",\"singerName\":\"{2}\",\"orderName\":\"{3}\",\"counter\":\"1\"",
                                songName, array[0].Trim(), array[1].Trim(), ordername));
                        postdata.Append("}");
                    }
                }
            }

            postdata.Append("]");
            if (fileCount == 0)
                return "";
            return postdata.ToString();
        }
        public static string SendSongDataToServer(string postdata)
        {
            if (!string.IsNullOrEmpty(postdata))
            {
                //发送数据
                var PostUrl = "http://i.singmusic.cn:8180/portals/setMusicCounter";
                var httpparm = new Vevisoft.Utility.Web.HttpParam(PostUrl);
                httpparm.Accpt = "application/json, text/javascript, */*";
                //httpparm.AccptEncoding = "utf-8";
                using (var response=Vevisoft.Utility.Web.HttpResponseUtility.CreatePostjsonResponse(httpparm,postdata.ToString()))
                {
                    var responseStr = Vevisoft.Utility.Web.HttpResponseUtility.GetPostResponseTextFromResponse(response);
                    return responseStr;
                }
            }
            return "";
        }
        /// <summary>
        /// 获取下载歌曲信息并发送到服务器记录数据
        /// </summary>
        /// <param name="dlPath"></param>
        /// <param name="ordername"></param>
        public static void GetSongInfoAndSendToServer(string dlPath, string ordername)
        {
            var postdata = GetPostSongData(dlPath, ordername);
            if (!string.IsNullOrEmpty(postdata))
                SendSongDataToServer(postdata);
        }

       

        /// <summary>
        /// 删除歌曲下载文件夹内所有文件，关闭QQMusic
        /// </summary>
        public static void ClearSongFolderAndCloseMain(string qqmusicPath, string dlPath,string catchPath)
        {
            try
            {
                //如果QQ音乐打开，那么关闭
                Vevisoft.Utility.ProcessUtility.KillProcess("QQMusic", qqmusicPath);
                //删除下载文件
                Vevisoft.IO.Directory.DeleteDirectoryContent(dlPath);
                //删除缓存文件
                Vevisoft.IO.Directory.DeleteDirectoryContent(catchPath);
            }
            catch (Exception)
            {
            }

        }
        /// <summary>
        /// 清理试听列表 下载列表
        /// </summary>
        public static void ClearAllQQMusicList()
        {
            IntPtr hwnd = GetQQMusicHandle();
            if (hwnd == IntPtr.Zero)
                return;
            SystemWindowsAPI.SetForegroundWindow(hwnd);
            DeleteTrySongList(hwnd);
            DeleteDownLoadList(hwnd);
        }

        /// <summary>
        /// 删除下载列表中所有文件
        /// </summary>
        /// <param name="mainHandle"></param>
        public static void DeleteDownLoadList(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
                return;
            //点击下载列表 按钮
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainDownLoadBtnPt);
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(mainHandle) || !GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
            //2.
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainDownLoadBtnPt);
            Thread.Sleep(1000);
            //
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainTryListenPanelFirstSongPt);
            //
            Thread.Sleep(1000);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            //
            Thread.Sleep(1000);
            //
            MouseKeyBoradUtility.KeySendDelete();
            //
            while (!SystemWindowsAPI.IsExeNotResponse(mainHandle) || !GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 清空试听列表中的内容
        /// </summary>
        /// <param name="mainHandle"></param>
        public static void DeleteTrySongList(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
                return;
            //点击下载列表 按钮
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MaintrySongListBtnPt);
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(mainHandle) || !GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
            Thread.Sleep(1000);
            //2.
           MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainTryListenPanelFirstSongPt);
            //
            Thread.Sleep(1000);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            //
            Thread.Sleep(1000);
            //
            MouseKeyBoradUtility.KeySendDelete();
            //
            while (!SystemWindowsAPI.IsExeNotResponse(mainHandle) || !GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 主程序是否无响应,线程方法
        /// </summary>
        /// <returns></returns>
        public static bool GetMainResponseByProcess()
        {
            foreach (var p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.ProcessName.ToLower()=="QQMusic".ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    return p.Responding;
                }
            }
            return false;
        }

        #endregion
    }

}
