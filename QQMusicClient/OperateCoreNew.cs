using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QQMusicClient.Dlls;
using Vevisoft.WebOperate;
using Vevisoft.WindowsAPI;
using mshtml;

namespace QQMusicClient
{
    /// <summary>
    /// QQ音乐下载程序，核心组件
    /// </summary>
    public class OperateCoreNew
    {
        public static readonly string QQPassErrorMsg = "QQ密码错误";

        #region 日志输出
        public event ShowInStatusBar ShowLog;

        protected virtual void OnShowLog(string text)
        {
            ShowInStatusBar handler = ShowLog;
            if (handler != null) handler(text);
        }

        public event ShowInStatusBar ShowInStatusBarEvent;
        public event ShowInStatusBar ShowInStatusMonitor;
        public event ShowInStatusBar ShowDownLoadInfo;
        /// <summary>
        /// 显示关于下载的信息
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnShowDownLoadInfo(string text)
        {
            ShowInStatusBar handler = ShowDownLoadInfo;
            if (handler != null) handler(text);
        }

        /// <summary>
        /// 监控器线程的信息提示信息
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnShowInStatusMonitor(string text)
        {
            ShowInStatusBar handler = ShowInStatusMonitor;
            if (handler != null) handler(text);
        }
        /// <summary>
        /// 显示Status中的信息,指的是步骤操作
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnShowInStatusBarEvent(string text)
        {
            ShowInStatusBar handler = ShowInStatusBarEvent;
            if (handler != null) handler(text);
        }

        public event GetDownLoadIDCOde GetDownLoadIDCOdeEvent;

        protected virtual string OnGetDownLoadIdcOdeEvent(IntPtr iehandle)
        {
            GetDownLoadIDCOde handler = GetDownLoadIDCOdeEvent;
            if (handler != null)
                return handler(iehandle);
            return "";
        }

        #endregion
        //共享变量
        #region 共享变量
        //是否必须为static形式。static能确保内存地址单一。
        /// <summary>
        /// QQ信息
        /// </summary>
        private static Models.QQInfo qqModel;
        /// <summary>
        /// 发送心跳失败的计数
        /// </summary>
        private static int FailedSendHeartCount = 0;
        /// <summary>
        /// 是否下载完成
        /// </summary>
        private static bool IsDownLoadOver = false;
        
        #endregion



        //子操作


        #region 清理缓存信息以及关闭QQ音乐
        /// <summary>
        /// 清理QQ音乐列表信息及删除所有文件
        /// </summary>
        /// <param name="mainHandle"></param>
        public void ClearALlInfos(IntPtr mainHandle)
        {
            try
            {
                DeleteTrySongList(mainHandle);
                DeleteDownLoadList(mainHandle);
                ClearSongFolderAndCloseMain();
            }
            catch (Exception)
            {


            }

        }
         /// <summary>
        /// 删除歌曲下载文件夹内所有文件，关闭QQMusic
        /// </summary>
        public void ClearSongFolderAndCloseMain()
        {
            try
            {
                //如果QQ音乐打开，那么关闭
                Vevisoft.Utility.ProcessUtility.KillProcess("QQMusic", AppConfig.AppPath);
                //删除下载文件
                Vevisoft.IO.Directory.DeleteDirectoryContent(AppConfig.DownLoadPath);
                //删除缓存文件
                Vevisoft.IO.Directory.DeleteDirectoryContent(AppConfig.AppCachePath);
            }
            catch (Exception)
            {
            }

        }

        #region 清理QQ音乐列表信息

        /// <summary>
        /// 删除下载列表中所有文件
        /// </summary>
        /// <param name="mainHandle"></param>
        public void DeleteDownLoadList(IntPtr mainHandle)
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
            Thread.Sleep(AppConfig.TimeMainFormStart);
            //
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainTryListenPanelFirstSongPt);
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
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
        public void DeleteTrySongList(IntPtr mainHandle)
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
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseKeyBoradUtility.KeySendDelete();
            //
            while (!SystemWindowsAPI.IsExeNotResponse(mainHandle) || !GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
        }

        #endregion
        #endregion

        #region 启动QQ,登录QQ，如果密码错误 ，提交。重新获取QQ登录
        /// <summary>
        /// 启动QQ音乐程序
        /// </summary>
        /// <returns></returns>
        public IntPtr StartApp()
        {
            //2.打开软件 判断软件是否显示打开并显示？？
            if (string.IsNullOrEmpty(AppConfig.AppPath))
            {
                OnShowInStatusBarEvent("没有设置QQ音乐s软件路径！");
                throw new Exception("没有设置QQ音乐s软件路径");
            }
            if (!File.Exists(AppConfig.AppPath))
            {
                OnShowInStatusBarEvent("软件路径错误，找不到软件！");
                throw new Exception("软件路径错误，找不到软件！");
            }
            //
            Process.Start(AppConfig.AppPath);
            //
            OnShowInStatusBarEvent("等待线程启动完全！");
            //
            WaitMainStartOver();
            //
            OnShowInStatusBarEvent("获取主窗体句柄！");
            IntPtr mainHanle = GetMainForm();
            
            //
            if (mainHanle == IntPtr.Zero)
            {
                OnShowInStatusBarEvent("没有获取到主窗体句柄！");
                throw new Exception("没有获取到主窗体句柄");
            }
            //
            while (!SystemWindowsAPI.IsExeNotResponse(mainHanle) || !GetMainResponseByProcess())
            {
                Application.DoEvents();
                Thread.Sleep(1000);
            }
            //
            OnShowInStatusBarEvent("主窗体加载完成！");
            //主窗体加载完成
            Thread.Sleep(AppConfig.TimeMainFormStart * 1000);
            return mainHanle;
        }

        public void LoginQQ(IntPtr mainHanle,string qqno,string qqpass)
        {
            OnShowInStatusBarEvent("查看是否已登录!");
            QQLogOut(mainHanle);
           
            //登录QQ,只有QQ密码错误的时候才重新登录。否则直接重启流程
            var successOK = false;
            do
            {
                //
                successOK = QQLoginSuccess(mainHanle, qqno, qqpass);
                if (!successOK)
                    OnShowInStatusBarEvent(qqno + "密码错误，重新获取QQ！");
            } while (!successOK);
        }

        #region 退出登录

        /// <summary>
        /// 退出登录，如果没有登录，那么返回操作
        /// </summary>
        /// <param name="mainHandle"></param>
        private void QQLogOut(IntPtr mainHandle)
        {
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //移动鼠标 到 程序 标题栏
            ClickChangeUser(mainHandle);
            //
            //判断是否有 【更改用户提示框】
            Thread.Sleep(AppConfig.TimeAlertCHangeUser*1000);
            IntPtr msgHandle = SystemWindowsAPI.GetForegroundWindow();
            if (msgHandle == mainHandle)
            {
                //说明 弹出框 说明没有登录，直接返回
                OnShowInStatusBarEvent("没有发现【弹出框】 说明没有登录，直接返回!");
                return;
            }
            OnShowInStatusBarEvent("有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】!");
            //有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】
            // 关闭【更改用户提示框】
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.ChangeUserAlertClosePt);
            //
            //【QQ登录窗体】是否出现
            OnShowInStatusBarEvent("判断【登陆窗体】是否出现!");
            var loginHandle = GetLoginForm();
            if (loginHandle != IntPtr.Zero)
            {
                OnShowInStatusBarEvent("关闭【登陆窗体】!");
                //关闭登录窗体
                MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormClosePt);
            }
            else
            {
                OnShowInStatusBarEvent("【登陆窗体】等待超时!");
                throw new Exception("等待超时，【更改用户提示框】没有出现。");
            }
            //
        }

        #endregion
        #region 登录QQ

        /// <summary>
        /// 登录，密码错误返回false
        /// </summary>
        /// <param name="mainHanle"></param>
        /// <returns></returns>
        private bool QQLoginSuccess(IntPtr mainHanle, string qqno, string qqpass)
        {
            try
            {
                QQLogin(mainHanle, qqno, qqpass);
            }
            catch (Exception e)
            {
                //密码错误，重新开始
                if (e.Message == QQPassErrorMsg)
                    return false;
                //验证码输入错误
                else throw e;
            }
            return true;
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="mainHandle"></param>
        private void QQLogin(IntPtr mainHandle, string qqno, string qqpass)
        {
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
                OnShowInStatusBarEvent("等待超时，登录,【QQ登录框】没有出现！");
                throw new Exception("等待超时，登录,【QQ登录框】没有出现。");
            }
            //
            Thread.Sleep(AppConfig.TimeAlertCHangeUser * 1000);
            InputPass(mainHandle, qqno, qqpass);
            //此时 可能出现多种情况。
            //1.正常情况 登录框 关闭 登陆完成
            //2.密码错误 
            //3.需要输入验证码
            //超时判断
            Thread.Sleep(AppConfig.TimeAlertCHangeUser * 1000);
            var count = 0;
            var safeHandle = GetQQSafeCenterForm(10);
            while (safeHandle != IntPtr.Zero && count < 3)
            {
                //安全中心出现，判断10S
                //处理一次
                DealWithQQSafeForm(safeHandle, qqno);
                OnShowInStatusBarEvent("QQ安全中心" + count);
                Console.WriteLine("QQ安全中心" + count);
                count++;
                safeHandle = GetQQSafeCenterForm(10);
            }
            if (count >= 3)
            {
                OnShowInStatusBarEvent("处理错误，退出登录,处理QQ安全中心失败。");
                throw new Exception("处理错误，退出登录,处理QQ安全中心失败。");
            }
            return;
            //登陆成功！！
        }

        /// <summary>
        ///安全中心 窗体是否还存在，存在则说明验证码输入错误。
        ///如果正确，密码错误那么还是出现安全中心.只不过是提示密码错误
        ///当前窗体是否主窗体？    
        /// </summary>
        /// <param name="safeHandle"></param>
        /// <param name="CurrentQQNo"></param>
        private void DealWithQQSafeForm(IntPtr safeHandle, string CurrentQQNo)
        {
            OnShowInStatusBarEvent("安全中心出现");
            Console.WriteLine("QQ安全中心出现");
            //判断类型
            if (IsPassWrong(safeHandle)) //密码错误
            {
                Console.WriteLine("密码错误");
                //关闭安全中心窗体，提交错误QQ并重新获取QQ号密码
                SystemWindowsAPI.SetActiveWindow(safeHandle);
                MouseKeyBoradUtility.KeySendESC();
                SendPassErrorQQToServer(CurrentQQNo);
                //
                throw new Exception(QQPassErrorMsg);
            }
            else if (IsNeedVeryCode(safeHandle)) //需要输入验证码
            {
                OnShowInStatusBarEvent("登录需要验证码");
                Console.WriteLine("登录需要验证码");
                //SendNeedVeryCodeQQToServer(CurrentQQNo);
                //输入验证码
                SystemWindowsAPI.SetActiveWindow(safeHandle);
                //InputVeryCodeOnLogin(safeHandle);
            }
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
        /// 右键 更改用户
        /// </summary>
        /// <param name="mainHandle"></param>
        private void ClickChangeUser(IntPtr mainHandle)
        {
            //移动鼠标 到 程序 标题栏
            SetMousePosition(mainHandle, PositionInfoQQMusic.MainCaptionPt);
            //右键点击
            MouseKeyBoradUtility.MouseRightClick();
            //
            //等待右键菜单弹出
            Thread.Sleep(AppConfig.TimeContextMenu * 1000);
            //【更改用户】 快捷键 U 并且 等待1S（eg:如果没有登录，则没有此快捷键）
            MouseKeyBoradUtility.KeyInputStringAndNumber("u", 1000);
        }
        /// <summary>
        /// QQ号密码输入，点击登录按钮
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="qqno"></param>
        /// <param name="pass"></param>
        private static void InputPass(IntPtr mainHandle, string qqno, string pass)
        {
            //
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormText1Pt);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            Thread.Sleep(500);
            //输入QQ号
            MouseKeyBoradUtility.KeyInputStringAndNumber(qqno, 50);
            //
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormPassPt);
            //如果有原来的密码，需要删除。所以先按一堆的backspace
            for (int i = 0; i < 18; i++)
            {
                MouseKeyBoradUtility.KeySendBackSpace();
                Thread.Sleep(50);
            }
            MouseKeyBoradUtility.KeyInputStringAndNumber(pass, 50);
            //单击登录按钮
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormOKButtonPt);
        }
        #endregion

        /// <summary>
        /// 发送错误QQ给服务器
        /// </summary>
        /// <param name="CurrentQQNo"></param>
        private void SendPassErrorQQToServer(string CurrentQQNo)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 打开歌单
        
        /// <summary>
        /// 点击【共享歌单名称播放按钮】添加到临时歌曲列表中
        /// </summary>
        /// <param name="mainhandle"></param>
        /// <param name="songlistName"></param>
        public  void ClickSongListAndLoadToTryList(IntPtr mainhandle, string songlistName)
        {
            var count = 5;
            var clickSuccess = false;
            if (GetSongListHtml(mainhandle))
            {
                do
                {
                    clickSuccess = isContainsSOngListAndClick(songlistName);
                    if (!clickSuccess)
                    {
                        ClickGetSongListbtn("歌单");
                        Thread.Sleep(2000);
                        ClickGetSongListbtn("点歌");
                        Thread.Sleep(2000);
                    }
                    count--;
                    Thread.Sleep(4000);
                } while (!clickSuccess && count > 0);
                if (!clickSuccess && count < 1)
                {
                    OnShowInStatusBarEvent("没有发现【共享歌单】" + songlistName);
                    throw new Exception("没有发现【共享歌单】" + songlistName);
                }
            }
        }
        /// <summary>
        /// 点击【点歌】按钮，加载共享歌单信息
        /// </summary>
        /// <returns></returns>
        public bool ClickGetSongListbtn(string btnName)
        {
            var id = GetHtmlDocument(songListIEHandle);
            if (id == null)
                return false;
            var str = id.body.innerHTML;
            foreach (IHTMLElement link in id.links)
            {
                if (!string.IsNullOrEmpty(link.innerHTML) && link.innerHTML.Contains(btnName))
                {
                    link.click();
                    return true;
                }
            }
            return false;
        }
         /// <summary>
        /// 是否存在此歌单，存在则点击
        /// </summary>
        /// <param name="songlistName"></param>
        /// <returns></returns>
        public bool isContainsSOngListAndClick(string songlistName)
        {
            var id = GetHtmlDocument(songListIEHandle);
            if (id == null)
                return false;
            var str = id.body.innerHTML;
            foreach (IHTMLElement link in id.links)
            {
                if (!string.IsNullOrEmpty(link.innerHTML)&&link.innerHTML.Contains("《"+songlistName))
                {
                    link.click();
                    Thread.Sleep(1000);
                    return true;
                }
            }
            //没有此歌单，查看是否有下一页，在下一页中查找
             foreach (IHTMLElement link in id.links)
             {
                 if (!string.IsNullOrEmpty(link.title) && link.title.Contains("下一页"))
                 {
                     link.click();
                     Thread.Sleep(4000);
                     return isContainsSOngListAndClick(songlistName);
                 }
             }
             return false;
        }

        private  bool isContainsSongListIE = false;
        private  IntPtr songListIEHandle = IntPtr.Zero;
        /// <summary>
        /// 加载个人信息页面，并且点击【点歌】加载共享歌单。将此歌单内容添加至临时歌曲列表中
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <returns></returns>
        public  bool GetSongListHtml(IntPtr mainHandle)
        {
            //初始化
            isContainsSongListIE = false;
            songListIEHandle = IntPtr.Zero;
            //
            SystemWindowsAPI.SetForegroundWindow(mainHandle);
            //点击头像
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.MainCaptionLoginButtonPt);
            MouseSetPositonAndLeftClick(mainHandle, new Point(1, 1));
            //等待加载
            Thread.Sleep(4000);
            //需要遍历所有窗体
            SystemWindowsAPI.FindWindowCallBack callback_songlist = EnumWindowCallBack_SongList;
            SystemWindowsAPI.EnumWindows(callback_songlist, 0);
            //
            var count = 5;
            var clickSuccess = false;
            if (isContainsSongListIE)
            {
                do
                {
                    ClickGetSongListbtn("歌单");
                    Thread.Sleep(1000);
                    clickSuccess = ClickGetSongListbtn("点歌");
                    count--;
                    //此时可能是没有加载完全，等待加载
                    Thread.Sleep(4000);
                } while (!clickSuccess && count > 0);
                //
                if (!clickSuccess && count < 1)
                {
                    //OnShowInStatusBarEvent("没有发现【点歌】按钮!");
                    throw new Exception("没有发现【点歌】按钮!");
                }
            }
            return isContainsSongListIE;

        }
        private  bool EnumWindowCallBack_SongList(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (!(strclsName.ToString().Trim().ToLower() == "TXGFLayerMask".ToLower() ||
                  strclsName.ToString().Trim().ToLower() == "TXGuiFoundation".ToLower())) //QQ音乐 查找歌单
                return true;

            //richTextBox1.AppendText(string.Format("{3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n", strTitle, strclsName, hwnd.ToString(), count));
            //
            IntPtr chHandle = Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null,
                                                                                null);
            if (chHandle != IntPtr.Zero)
            {
                strclsName = new StringBuilder(256);
                Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(chHandle, strclsName, 257);
                strTitle = new StringBuilder(256);
                Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(chHandle, strTitle, 257);
                //
                IntPtr chHandle2 = SystemWindowsAPI.FindWindowEx(chHandle, IntPtr.Zero, null, null);
                IntPtr chHandle3 = SystemWindowsAPI.FindWindowEx(chHandle2, IntPtr.Zero, null, null);
                var didcIEHandler1 = SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
                if (didcIEHandler1 == IntPtr.Zero)
                    return true;
                var id = GetHtmlDocument(didcIEHandler1);
                if (id == null)
                    return true;
                var str = id.body.innerHTML;
                if (str == null) return true;
                string cookies = id.cookie;
                if (str.Contains("音乐基因"))
                {
                    OnShowInStatusBarEvent("歌单验证码窗体IE句柄");
                    songListIEHandle = didcIEHandler1;
                    isContainsSongListIE = str.Contains("点歌");
                }
            }
            return true;

        }
        #endregion

        #region 下载歌曲，获取下载数等。。。
        /// <summary>
        /// 下载临时列表中的歌曲
        /// </summary>
        /// <param name="mainhandle"></param>
        public void DownLoadSong_TryList(IntPtr mainhandle)
        {
            //点击临时歌单
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
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MainTryListenPanelFirstSongPt);
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MaintrySongListDownLoadButtonPt);
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseKeyBoradUtility.KeySendArrowDown();
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseKeyBoradUtility.KeySendArrowDown();
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseKeyBoradUtility.KeySendEnter();
            #endregion
            //
            Thread.Sleep(2000);
            OnlyDownLoadSongs(mainhandle);
        }
        /// <summary>
        /// 等待下载弹出框，下载歌曲
        /// </summary>
        /// <param name="mainhandle"></param>
        public void OnlyDownLoadSongs(IntPtr mainhandle)
        {
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(mainhandle)
                || !GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
            //
            //等待下载对话框，判断是否超过限制
            var dwlHandle = GetDownLoadDiag(30);
            if (dwlHandle != IntPtr.Zero)
            {
                SetDownLoadNum();
                Thread.Sleep(500);
                MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.DownLoadDiagButtonPt);               
            }
            else
            {
                OnShowInStatusBarEvent("下载对话框没有找到");
                throw new Exception("下载对话框没有找到");
            }
            OnShowInStatusBarEvent("aaaaaaaaaaa");
            //
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(mainhandle) || !GetMainResponseByProcess())
                Thread.Sleep(1000);
           
        }

        private void SetDownLoadNum()
        {
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowGetDownLoadDiagCallBack;
            SystemWindowsAPI.EnumWindows(callback, 0);
           
        }
        private bool EnumWindowGetDownLoadDiagCallBack(IntPtr hwnd, int lParam)
        {
            IntPtr fatherHwnd = SystemWindowsAPI.GetParent(hwnd);
            if (fatherHwnd != IntPtr.Zero)
            {
                //TXGFLayerMask
                var faTitle = new StringBuilder(256);
                SystemWindowsAPI.GetWindowText(fatherHwnd, faTitle, 257);
                if (!string.IsNullOrEmpty(faTitle.ToString()) && faTitle.ToString().Contains("下载"))
                {
                    //属于下载对话框的子窗体。查找IE
                    var strclsName = new StringBuilder(256);
                    SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
                    var strTitle = new StringBuilder(256);
                    SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
                    if (strclsName.ToString().Trim().ToLower() != "TXGFLayerMask".ToLower())
                        return true;
                    IntPtr chHandle = SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null, null);
                    if (chHandle != IntPtr.Zero)
                    {
                        //strclsName = new StringBuilder(256);
                        //Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(chHandle, strclsName, 257);
                        //strTitle = new StringBuilder(256);
                        //Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(chHandle, strTitle, 257);

                        //SystemWindowsAPI.ShowWindow(hwnd,0);
                        //richTextBox1.AppendText(string.Format("   {3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n", strTitle,
                        //                                       strclsName, hwnd.ToString(), count + ".1"));
                        IntPtr chHandle2 = SystemWindowsAPI.FindWindowEx(chHandle, IntPtr.Zero, null, null);
                        IntPtr chHandle3 = SystemWindowsAPI.FindWindowEx(chHandle2, IntPtr.Zero, null, null);
                        var didcIEHandler = SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
                        if (didcIEHandler == IntPtr.Zero)
                            return true;
                        var id = GetHtmlDocument(didcIEHandler);
                        if (id == null)
                            return true;
                        var str = id.body.innerHTML;
                        if (string.IsNullOrEmpty(str))
                            return true;
                        if (qqModel == null)
                            return true;
                        //

                        //downloadCookie = id.cookie;

                        qqModel.SongOrderList[qqModel.CurrentSongOrderName] = GetSongCount(str);
                        //获取剩余数量
                        //GetDownLoadInfoFromTecentServer(qqModel);
                        //var count1 = 0;
                        //var count2 = 0;
                        if (qqModel.SongOrderList[qqModel.CurrentSongOrderName] > qqModel.RemainNum)
                        {
                            //取消选择
                            var elements2 =
                                id.all.Cast<mshtml.HTMLDivElement>()
                                  .Where(
                                      item =>
                                      (item.className == "checkbox checkbox_press js_nl" ||
                                       item.className == "checkbox js_nl checkbox_press"));
                            foreach (HTMLDivElement htmlDivElement in elements2)
                            {
                                Console.WriteLine(htmlDivElement.outerHTML);
                                htmlDivElement.click();
                                var str2 = htmlDivElement.innerHTML;
                                Console.WriteLine(htmlDivElement.outerHTML);
                            }
                            //var songlistDivEle =
                            //    id.all.Cast<IHTMLElement>().FirstOrDefault(item => item.id == "id_songlist") as IHTMLElement;
                            //if (titleelement != null)
                            //    titleelement.innerHTML = "您共选择了1首歌曲";

                            //剩余数没有歌单数量多。那么减少下载数量                           
                            //
                            //选择足够数量
                            //checkbox js_nl

                            var elements3 =
                                id.all.Cast<mshtml.HTMLSpanElement>()
                                  .Where(item => item.className == "checkbox js_nl");
                            bool isfirst = true;
                            //剩余下载数
                            var count = qqModel.RemainNum;
                            //
                            foreach (HTMLSpanElement htmlDivElement in elements3)
                            {
                                if (count > 0)
                                {
                                    if (htmlDivElement
                                            .parentElement.parentElement
                                            .parentElement.parentElement.id == "id_songlist")
                                    {
                                        Console.WriteLine(htmlDivElement.outerHTML);
                                        htmlDivElement.click();
                                        var str2 = htmlDivElement.innerHTML;
                                        Console.WriteLine(htmlDivElement.outerHTML);
                                        count--;
                                    }
                                }
                            }
                            //重新给下载歌单数复制
                            qqModel.SongOrderList[qqModel.CurrentSongOrderName] = qqModel.RemainNum;
                        }
                        //
                        // GetSongCount(str);
                        OnShowInStatusBarEvent("歌单总数为" + qqModel.SongOrderList[qqModel.CurrentSongOrderName]);
                        OnShowInStatusMonitor("歌单总数为" + qqModel.SongOrderList[qqModel.CurrentSongOrderName]);
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 获取歌单歌曲总数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public int GetSongCount(string str)
        {
            //str.LastIndexOf()
            if (!str.Contains("js_selnum"))
                return 0;
            var idx2 = str.IndexOf("js_selnum");
            //str = str.Substring(idx2);
            int idx = str.IndexOf("首", idx2);
            if (idx < 0)
                return 0;
            var intvalue = str.Substring(idx2, idx - idx2);
            try
            {
                return GetIntFromString(intvalue);
            }
            catch (Exception)
            {
            }
            return 0;
        }

     
        #endregion

        #region 是否启动完全,判断线程
        /// <summary>
        /// 等待QQ音乐启动完毕,10S时间
        /// </summary>
        public void WaitMainStartOver()
        {
            //
            var count = 10;

            while (GetProcessCount("QQMusicExternal") < 2 && count > 0)
            {
                count--;
                Thread.Sleep(1000);
            }
            if (count <= 0)
            {
                OnShowInStatusBarEvent("启动没有完成！");
                throw new Exception("启动没有完成！");
            }
        }

        private int GetProcessCount(string processName)
        {
            return Process.GetProcesses().Count(process => process.ProcessName.ToLower() == processName.ToLower());
        }

        #endregion
        #region 线程是否 没反应

        /// <summary>
        /// 主程序是否无响应,线程方法
        /// </summary>
        /// <returns>true有反应 false没反应</returns>
        public bool GetMainResponseByProcess()
        {
            return Vevisoft.Utility.ProcessUtility.GetResponseByProcess("QQMusic", AppConfig.AppPath);
        }

        #endregion
        #region 公用方法

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

        /// <summary>
        /// 移动鼠标到相应的位置
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="relativePt"></param>
        public static void SetMousePosition(IntPtr mainHandle, Point relativePt)
        {
            //移动鼠标 到 (eg 程序 标题栏)
            var mainRect = GetFormRect(mainHandle);
            MouseKeyBoradUtility.SetCursorPos(relativePt.X + mainRect.Left, relativePt.Y + mainRect.Top);
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
        }

        /// <summary>
        /// 获取IHTMLDocument2对象
        /// </summary>
        /// <param name="hwnd">IE句柄</param>
        /// <returns></returns>
        public mshtml.IHTMLDocument2 GetHtmlDocument(IntPtr hwnd)
        {
            var domObject = new System.Object();
            var tempInt = 0;
            var guidIEDocument2 = new Guid();
            var WM_Html_GETOBJECT = SystemWindowsAPI.RegisterWindowMessage("WM_Html_GETOBJECT");//定义一个新的窗口消息
            var W = SystemWindowsAPI.SendMessage(hwnd, WM_Html_GETOBJECT, 0, ref tempInt);//注:第二个参数是RegisterWindowMessage函数的返回值
            var lreturn = SystemWindowsAPI.ObjectFromLresult(W, ref guidIEDocument2, 0, ref domObject);
            var doc = (mshtml.IHTMLDocument2)domObject;
            return doc;
        }

        public static int GetIntFromString(string str)
        {
            int number = 0;
            string num = null;
            foreach (char item in str)
            {
                if (item >= 48 && item <= 58)
                {
                    num += item;
                }
            }
            number = int.Parse(num);
            return number;
        }
        #endregion
        #region 获取窗体
        
        /// <summary>
        /// 获取主窗体句柄
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetMainForm()
        {
            const string caption = "QQ音乐"; //TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 1000, 30);
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
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, 30);
            if(handle==IntPtr.Zero)
                handle = SystemWindowsAPI.FindMainWindowHandle(caption2, 500, 30);
            return handle;
        }

        /// <summary>
        /// 下载对话框是否存在
        /// </summary>
        /// <returns></returns>
        public  static IntPtr GetDownLoadDiag(int delay)
        {
            //下载,   TXGuiFoundation
            const string caption = "下载";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, delay*2);
            return handle;
        }
        /// <summary>
        /// 获取QQ安全中心 窗体句柄。一般是发生密码错误时。
        /// </summary>
        /// <returns></returns>
        public IntPtr GetQQSafeCenterForm(int delay)
        {
            const string caption = "QQ安全中心";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, delay * 2);
            return handle;
        }

        #endregion
    }
}
