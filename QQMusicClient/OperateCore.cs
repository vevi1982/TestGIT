using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Vevisoft.WebOperate;
using Vevisoft.WindowsAPI;
using mshtml;

namespace QQMusicClient
{
    /// <summary>
    /// QQ音乐自动下载操作
    /// </summary>
    public class OperateCore
    {
        private System.Windows.Forms.Timer identifyingTimer;
        private const string didcInfo = "您下载歌曲的次数过于频繁，请输入验证码后继续下载！";

        /// <summary>
        /// 最大延时
        /// </summary>
        public int maxTime = 30;

        /// <summary>
        /// 是否下载完成
        /// </summary>
        public bool IsDownLoadOver = false;

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

        //非程序错误，需要启动下一步骤的错误
        public static readonly string QQPassErrorMsg = "QQ密码错误！";
        public static readonly string QQDownLoadOverLimit = "下载超限";
        //
        private IntPtr mainHandler = IntPtr.Zero;

        public void DoOnce()
        {
            ClearSongFolderAndCloseMain();
            //1.改变IP

            #region CHangeIP

            ChangeIP();

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
          
            //
            if (mainHanle == IntPtr.Zero)
                throw new Exception("没有获取到主窗体句柄");
            mainHandler = mainHanle;
            //
            while (!SystemWindowsAPI.IsExeNotResponse(mainHanle))
            {
                Thread.Sleep(1000);
            }
            //
            //主窗体加载完成
            Thread.Sleep(AppConfig.TimeMainFormStart*1000);
           
            //是否已登录,登录则退出
            QQLogOut(mainHanle);
            //登录QQ,只有QQ密码错误的时候才重新登录。否则直接重启流程
            var successOK = false;
            do
            {
                #region 获取QQ
                //获取QQ号
                var qqlist = GetQQNoAndPass();
                #endregion
                //
                successOK = QQLoginSuccess(mainHanle, qqlist[0], qqlist[1]);
            } while (!successOK);
            //
            Thread.Sleep(1000);
            //下载歌曲 并启动判断程序
            DownLoadSongs(mainHanle);
            //下载完毕，删除歌曲
            while (!IsDownLoadOver)
            {
                Thread.Sleep(1000);
                Application.DoEvents();
            }
            //
            ClearSongFolderAndCloseMain();

        }

        #region 更改IP

        /// <summary>
        /// 更改IP
        /// </summary>
        private void ChangeIP()
        {

            if (AppConfig.ChangeIP)
            {
                var adsl = new AdslDialHelper();

                if (adsl.IsConnectToInternetByPing("www.baidu.com"))
                {
                    //连接上的时候 先断开
                    adsl.StopDailer(AppConfig.ADSLName);
                    while (adsl.IsConnectToInternetByPing("www.baidu.com"))
                    {
                        Thread.Sleep(500);
                    }
                }
                //拨号间隔时间
                Thread.Sleep(AppConfig.ChangeIPInterval);
                //连接ADSL
                adsl.StartDailer(AppConfig.ADSLName, AppConfig.ADSLUserName, AppConfig.ADSLPass);
                while (!adsl.IsConnectToInternetByPing("www.baidu.com"))
                {
                    Thread.Sleep(500);
                }
                //
                if (ValidateIPFromServer(AppConfig.PCName))
                    ChangeIP();
            }

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
                throw new Exception("等待超时，登录,【QQ登录框】没有出现。");
            //
            Thread.Sleep(AppConfig.TimeAlertCHangeUser*1000);
            InputPass(mainHandle, qqno, qqpass);
            //此时 可能出现多种情况。
            //1.正常情况 登录框 关闭 登陆完成
            //2.密码错误 
            //3.需要输入验证码
            //超时判断
            Thread.Sleep(AppConfig.TimeAlertCHangeUser*1000);
            var count = 0;
            var safeHandle = GetQQSafeCenterForm(10);
            while (safeHandle != IntPtr.Zero && count < 3)
            {
                //安全中心出现，判断10S
                //处理一次
                DealWithQQSafeForm(safeHandle, qqno);
                Console.WriteLine("QQ安全中心" + count);
                count++;
                safeHandle = GetQQSafeCenterForm(10);
            }
            if (count >= 3)
                throw new Exception("处理错误，退出登录,处理QQ安全中心失败。");
            //是否有登录失败，请输入用户名和密码登陆
            if (!ForgroundIsMain(mainHandle))
            {
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                var strtitle = new StringBuilder(256);
                SystemWindowsAPI.GetWindowText(msgHandle, strtitle, 257);
                if (strtitle.ToString().ToLower() == "error")
                {
                    Console.WriteLine("密码错误");
                    //关闭安全中心窗体，提交错误QQ并重新获取QQ号密码
                    SystemWindowsAPI.SetActiveWindow(safeHandle);
                    MouseKeyBoradUtility.KeySendESC();
                    SendPassErrorQQToServer(qqno);
                    //
                    throw new Exception(QQPassErrorMsg);
                    //MouseKeyBoradUtility.KeySendEnter();
                    ////
                    //Thread.Sleep(1000);
                    ////
                    ////关闭登录窗体
                    //MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormClosePt);
                }
            }
            //登陆成功！！
            
        }

        /// <summary>
        /// 删除歌曲下载文件夹内所有文件
        /// </summary>
        public void ClearSongFolderAndCloseMain()
        {
            var dir = new DirectoryInfo(AppConfig.DownLoadPath);
            foreach (FileInfo f in dir.GetFiles())
                f.Delete();
            //如果QQ音乐打开，那么关闭
            foreach (var p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.ProcessName.Contains("QQMusic") && p.MainModule.FileName.ToLower() == AppConfig.AppPath.ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    p.Kill();
                }
            }
        }

        private bool GetMainResponseByProcess()
        {
            foreach (var p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.ProcessName.Contains("QQMusic") && p.MainModule.FileName.ToLower() == AppConfig.AppPath.ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    return p.Responding;
                }
            }
            return false;
        }

        #endregion

        #region 删除下载列表内容
        private void DeleteDownLoadList(IntPtr mainHandle)
        {
            //点击下载列表 按钮
            MouseSetPositonAndLeftClick(mainHandle,PositionInfoQQMusic.MainDownLoadBtnPt);
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(mainHandle)||!GetMainResponseByProcess())
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
             while (!SystemWindowsAPI.IsExeNotResponse(mainHandle)||!GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
        }
        #endregion
        /// <summary>
        ///安全中心 窗体是否还存在，存在则说明验证码输入错误。
        ///如果正确，密码错误那么还是出现安全中心.只不过是提示密码错误
        ///当前窗体是否主窗体？    
        /// </summary>
        /// <param name="safeHandle"></param>
        /// <param name="CurrentQQNo"></param>
        private void DealWithQQSafeForm(IntPtr safeHandle, string CurrentQQNo)
        {
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
                Console.WriteLine("登录需要验证码");
                SendNeedVeryCodeQQToServer(CurrentQQNo);
                //输入验证码
                SystemWindowsAPI.SetActiveWindow(safeHandle);
                InputVeryCodeOnLogin(safeHandle);
            }
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
                return;
            }
            //有【更改用户提示框】,那么 鼠标移动 左键单击 【关闭】
            // 关闭【更改用户提示框】
            MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.ChangeUserAlertClosePt);
            //
            //【QQ登录窗体】是否出现
            var loginHandle = GetLoginForm();
            if (loginHandle != IntPtr.Zero)
            {
                //关闭登录窗体
                MouseSetPositonAndLeftClick(mainHandle, PositionInfoQQMusic.LoginFormClosePt);
            }
            else
                throw new Exception("等待超时，【更改用户提示框】没有出现。");
            //
            //判断是否关闭,即当前窗体是否主窗体
            if (!ForgroundIsMain(mainHandle, maxTime))
                throw new Exception("等待超时，【QQ登陆框】没有关闭。");

        }

        #endregion

        #region 下载歌曲

        /*
         * 点击 播放列表
         * 点击 歌曲列表区域
         * Ctrl A
         * 【下载】按钮，（键盘）下，（键盘）下，回车
         * 判断下载窗体 点击 下载到电脑 按钮
         */

        /// <summary>
        /// 下载歌曲
        /// </summary>
        /// <param name="mainhandle"></param>
        private void DownLoadSongs(IntPtr mainhandle)
        {
            SystemWindowsAPI.SetForegroundWindow(mainhandle);
            DeleteDownLoadList(mainhandle);
            SystemWindowsAPI.SetForegroundWindow(mainhandle);
            //1.点击 播放列表
            MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MainTryListenButtonPt);
            //
            if (!ForgroundIsMain(mainhandle, 10))
            {
                MouseKeyBoradUtility.KeySendAltF4();
                Thread.Sleep(AppConfig.TimeAlertCHangeUser*1000);
            }
            //2.
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
            MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MainTryListenPanelDownLoadButtonPt);
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
            //
            //等待响应
             while (!SystemWindowsAPI.IsExeNotResponse(mainhandle)
                 ||!GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
            //等待下载对话框，判断是否超过限制
            var dwlHandle = GetDownLoadDiag(30);
            if (dwlHandle != IntPtr.Zero)
            {
                if (cannotDown)
                {
                    Console.WriteLine(QQDownLoadOverLimit);
                    throw new Exception(QQDownLoadOverLimit);
                }
                Thread.Sleep(500);
                MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.DownLoadDiagButtonPt);
                //启动计时器 判断验证码输入框是否存在，以及是否下载完成
            }
            else throw new Exception("下载对话框没有找到");
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(mainhandle) || !GetMainResponseByProcess())
                Thread.Sleep(1000);
            //判断10s是否超限
            SystemWindowsAPI.SetForegroundWindow(mainhandle);
            dwlHandle = GetDownLoadDiag(15);
            if (dwlHandle != IntPtr.Zero)
            {
                if (cannotDown)
                {
                    Console.WriteLine(QQDownLoadOverLimit);
                    throw new Exception(QQDownLoadOverLimit);
                }
                //
                StartDownLoadTimer();
            }

        }

    /// <summary>
        /// 启动下载监视计时器
        /// </summary>
        public void StartDownLoadTimer()
        {
            IsDownLoadOver = false;
            mainHandler = GetMainForm();
            identifyingTimer = new System.Windows.Forms.Timer { Interval = 10 * 1000 };
            identifyingTimer.Tick += identifyingTimer_Tick;
            identifyingTimer.Start();
        }
        /// <summary>
        /// 判断是否有【下载验证码输入框】，判断下载是否完成！！
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void identifyingTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (GetDownLoadIdCodeDiagExist())
                {
                    //当前窗体不是主窗体，那么 默认 就是输入验证码窗体 弹出
                    InputDownLoadIdentifyingCode();

                }
            }
            catch (Exception)
            {

                
            }

            //
            if (GetSongCountFromFolder() == AppConfig.SongListCount)
            {
                //下载完成。。。
                IsDownLoadOver = true;
            }
        }
        public static Bitmap CutScreen(Rectangle rect)
        {
            var myImage = new Bitmap(rect.Width, rect.Height);
            Graphics g = Graphics.FromImage(myImage);
            g.CopyFromScreen(new Point(rect.X, rect.Y), new Point(0, 0), new Size(rect.Width, rect.Height));
            IntPtr dc1 = g.GetHdc();
            g.ReleaseHdc(dc1);
            return myImage;
        }
        /// <summary>
        /// 输入下载对话框的验证码
        /// </summary>
        private void InputDownLoadIdentifyingCode()
        {
            var mainhandle = GetMainForm();
            //点击 输入框 获取验证码
            //MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.VeryCodeDownLoadTxtPt);
            //var rect = GetFormRect(mainHandler);
            //Thread.Sleep(AppConfig.TimeIdCodeLoad*1000);
            //获取验证码
            //CutScreen(new Rectangle(
            //    rect.Left + PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.X,
            //      rect.Top+ PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.Y,
            //        PositionInfoQQMusic.IDCodeImgSize.Width,
            //         PositionInfoQQMusic.IDCodeImgSize.Height)).Save("aa.bmp");
            //var idcode =
            //    Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCode(
            //         rect.Left + PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.X,
            //        rect.Top + PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.Y,
            //        PositionInfoQQMusic.IDCodeImgSize.Width,
            //         PositionInfoQQMusic.IDCodeImgSize.Height);
            //
            //for (int i = 0; i < 4; i++)
            //    MouseKeyBoradUtility.KeySendBackSpace();

            var idcode = GetIDCodeDownLoad(didcIEHandler);
            //
            Console.WriteLine(idcode);
            mshtml.IHTMLDocument2 id = GetHtmlDocument(didcIEHandler);   
                //
            if (idcode == "1009" || idcode.Length != 4)
            {
                id.parentWindow.execScript("showVcode()", "javascript");
                throw new Exception(" 验证码返回失败");
            }
            //
                    
            id.parentWindow.execScript("showElement('id_verify')", "javascript");
            var idtext = id.all.item("vcode", 0) as IHTMLElement;
            idtext.setAttribute("value", idcode);
            //var btnok = id.all.item("", 0) as IHTMLElement;
            foreach ( IHTMLElement btnok in id.links)
            {
                if (btnok.innerHTML.Contains("确认"))
                {
                    btnok.click();
                    break;
                }
            }
            var errortip = id.all.item("error_tips", 0) as IHTMLElement;
            if(errortip.innerHTML.Contains("错误"))
                id.parentWindow.execScript("showVcode()", "javascript");
            //btnok.click();

            //输入验证码
            //MouseKeyBoradUtility.KeyInputStringAndNumber(idcode,50);
            ////点击确定按钮
            //MouseSetPositonAndLeftClick(mainHandler,PositionInfoQQMusic.VeryCodeDownLoadOKPt);
            //判断是否成功。。。。
            //
            if (!ForgroundIsMain(mainHandler, maxTime))
                throw new Exception("验证码输入错误");
        }
        private string GetIDCodeDownLoad(IntPtr didcIeHandle)
        {
            mshtml.IHTMLDocument2 id = GetHtmlDocument(didcIeHandle);
            if (id == null)
                return "";
            IHTMLControlElement img =
                id.images.Cast<IHTMLElement>().Where(item => item.id == "imgVerify").Cast<IHTMLControlElement>().FirstOrDefault();
            if (img != null)
            {
                IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)id.body).createControlRange();
                range.add(img);
                range.execCommand("Copy", false, null);
                if (Clipboard.ContainsImage())
                {
                    Clipboard.GetImage().Save(@"c:\bb.bmp");
                    return Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCodeWeb(@"c:\bb.bmp", 1014);
                }
            }
            return "";
        }
        /// <summary>
        /// 获取下载数量
        /// </summary>
        /// <returns></returns>
        private int GetSongCountFromFolder()
        {
            return Directory.Exists(AppConfig.DownLoadPath) ? 
                Directory.GetFiles(AppConfig.DownLoadPath).Length : 0;
        }

        #endregion

        #region 获取相关窗体

        /// <summary>
        /// 获取主窗体句柄
        /// </summary>
        /// <returns></returns>
        public IntPtr GetMainForm()
        {
            const string caption = "QQ音乐"; //TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 1000, maxTime);
            return handle;
        }

        /// <summary>
        /// 获取登录窗体句柄
        /// </summary>
        /// <returns></returns>
        public IntPtr GetLoginForm()
        {
            //QQ音乐快速登录
            string caption2 = "QQ音乐快速登录";
            const string caption = "QQ音乐登录"; //TXGuiFoundation
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, maxTime);
            if(handle==IntPtr.Zero)
                handle = SystemWindowsAPI.FindMainWindowHandle(caption2, 500, maxTime);
            return handle;
        }

        /// <summary>
        /// 下载对话框是否存在
        /// </summary>
        /// <returns></returns>
        public IntPtr GetDownLoadDiag(int delay)
        {
            //下载,   TXGuiFoundation
            const string caption = "下载";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, delay*2);
            if (handle != IntPtr.Zero)
                IsConNotDownLoadSong();
            return handle;
        }
        /// <summary>
        /// 是否下载数量已达到上限
        /// </summary>
        private bool cannotDown = false;
        /// <summary>
        /// 是否下载数量已达到上限
        /// </summary>
        /// <returns></returns>
        public bool IsConNotDownLoadSong()
        {
            cannotDown = false;
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowGetDownLoadDiagCallBack;
            SystemWindowsAPI.EnumWindows(callback, 0);
            return cannotDown;
        }
        private bool EnumWindowGetDownLoadDiagCallBack(IntPtr hwnd, int lParam)
        {
            IntPtr fatherHwnd = SystemWindowsAPI.GetParent(hwnd);
            if (fatherHwnd != IntPtr.Zero)
            {
                //TXGFLayerMask
                var faTitle = new StringBuilder(256);
                SystemWindowsAPI.GetWindowText(fatherHwnd, faTitle, 257);
                if (faTitle.ToString().Contains("下载"))
                {
                    //属于下载对话框的子窗体。查找IE
                    var strclsName = new StringBuilder(256);
                    Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
                    var strTitle = new StringBuilder(256);
                    Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
                    if (strclsName.ToString().Trim().ToLower() != "TXGFLayerMask".ToLower())
                        return true;
                    IntPtr chHandle = Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null,
                                                                                null);
                    if (chHandle != IntPtr.Zero)
                    {
                        strclsName = new StringBuilder(256);
                        Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(chHandle, strclsName, 257);
                        strTitle = new StringBuilder(256);
                        Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(chHandle, strTitle, 257);

                        //SystemWindowsAPI.ShowWindow(hwnd,0);
                        //richTextBox1.AppendText(string.Format("   {3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n", strTitle,
                        //                                       strclsName, hwnd.ToString(), count + ".1"));
                        IntPtr chHandle2 = Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowEx(chHandle, IntPtr.Zero, null, null);
                        IntPtr chHandle3 = Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowEx(chHandle2, IntPtr.Zero, null, null);
                        didcIEHandler = Vevisoft.WindowsAPI.SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
                        mshtml.IHTMLDocument2 id = GetHtmlDocument(didcIEHandler);
                        var str = id.body.innerHTML;
                        cannotDown = str.Contains("下载数量已达到上限");
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 获取QQ安全中心 窗体句柄。一般是发生密码错误时。10S
        /// </summary>
        /// <returns></returns>
        public IntPtr GetQQSafeCenterForm()
        {
            return GetQQSafeCenterForm(10);
        }

        /// <summary>
        /// 获取QQ安全中心 窗体句柄。一般是发生密码错误时。
        /// </summary>
        /// <returns></returns>
        public IntPtr GetQQSafeCenterForm(int delay)
        {
            const string caption = "QQ安全中心";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, delay*2);
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

        #endregion

        #region 用户相关操作

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
            Thread.Sleep(AppConfig.TimeContextMenu*1000);
            //【更改用户】 快捷键 U 并且 等待1S（eg:如果没有登录，则没有此快捷键）
            MouseKeyBoradUtility.KeyInputStringAndNumber("u", 1000);
        }
        /// <summary>
        /// QQ号密码输入，点击登录按钮
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="qqno"></param>
        /// <param name="pass"></param>
        private void InputPass(IntPtr mainHandle, string qqno, string pass)
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

        private IntPtr handler1 = IntPtr.Zero;
        private IntPtr handler2 = IntPtr.Zero;
        private IntPtr handler3 = IntPtr.Zero;
        private IntPtr handler4 = IntPtr.Zero;
        /// <summary>
        /// 当前窗体是否主窗体
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <returns></returns>
        private bool ForgroundIsMain(IntPtr mainHandle)
        {
            var msgHandle = SystemWindowsAPI.GetForegroundWindow();
            handler1 = SystemWindowsAPI.GetTopMostWindow(mainHandler);
            handler2 = SystemWindowsAPI.GetTopWindow(mainHandler);
            handler3 = SystemWindowsAPI.GetTopWindow(IntPtr.Zero);
            handler4 = SystemWindowsAPI.GetTopMostWindow(IntPtr.Zero);
            return msgHandle == mainHandle;
        }
        /// <summary>
        /// 当前窗体是否主窗体
        /// </summary>
        /// <param name="mainHandle">当前窗体Handler</param>
        /// <param name="delayTime">判断时间</param>
        /// <returns></returns>
        private bool ForgroundIsMain(IntPtr mainHandle, int delayTime)
        {
            var count = 0;
            while (!ForgroundIsMain(mainHandle) && count < delayTime)
            {
                Thread.Sleep(1000);
                count++;
            }
            if (count >= maxTime)
                return false;
            return true;
        }
        //
        private int count = 0;
        private bool isexistDownLoadIDCodeDiag = false;
        private bool GetDownLoadIdCodeDiagExist()
        {
            count = 0;
            isexistDownLoadIDCodeDiag = false;
            didcIEHandler = IntPtr.Zero;
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowCallBack;
            SystemWindowsAPI.EnumWindows(callback, 0);
            //
            return isexistDownLoadIDCodeDiag;
        }

        private IntPtr didcIEHandler = IntPtr.Zero;

        private bool EnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (strclsName.ToString().Trim().ToLower() != "TXGFLayerMask".ToLower())
                return true;

            count++;
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

                //SystemWindowsAPI.ShowWindow(hwnd,0);
                //richTextBox1.AppendText(string.Format("   {3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n", strTitle,
                //                                       strclsName, hwnd.ToString(), count + ".1"));
                IntPtr chHandle2 = SystemWindowsAPI.FindWindowEx(chHandle, IntPtr.Zero, null, null);
                IntPtr chHandle3 = SystemWindowsAPI.FindWindowEx(chHandle2, IntPtr.Zero, null, null);
                var  didcIEHandler1 = SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
                var id = GetHtmlDocument(didcIEHandler1);
                var str = id.body.innerHTML;
                if (str.Contains("请输入验证码"))
                {
                    didcIEHandler = didcIEHandler1;
                    isexistDownLoadIDCodeDiag = str.Contains(didcInfo);
                }

            }
            return true;
            
        }
        public mshtml.IHTMLDocument2 GetHtmlDocument(IntPtr hwnd)
        {
            var domObject = new System.Object();
            int tempInt = 0;
            var guidIEDocument2 = new Guid();
            var WM_Html_GETOBJECT = SystemWindowsAPI.RegisterWindowMessage("WM_Html_GETOBJECT");//定义一个新的窗口消息
            int W = SystemWindowsAPI.SendMessage(hwnd, WM_Html_GETOBJECT, 0, ref tempInt);//注:第二个参数是RegisterWindowMessage函数的返回值
            int lreturn = SystemWindowsAPI.ObjectFromLresult(W, ref guidIEDocument2, 0, ref domObject);
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2)domObject;
            return doc;
        }

        #endregion

        #region 公用方法

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
        /// 移动鼠标到相应的位置
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="relativePt"></param>
        public void SetMousePosition(IntPtr mainHandle, Point relativePt)
        {
            //移动鼠标 到 程序 标题栏
            var mainRect = GetFormRect(mainHandle);
            MouseKeyBoradUtility.SetCursorPos(relativePt.X + mainRect.Left, relativePt.Y + mainRect.Top);
        }

        /// <summary>
        /// 移动鼠标 并单击
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <param name="relativePt"></param>
        public void MouseSetPositonAndLeftClick(IntPtr mainHandle, Point relativePt)
        {
            SetMousePosition(mainHandle, relativePt);
            MouseKeyBoradUtility.MouseLeftClick();
        }

        #endregion

        #region 与服务器交互  提交密码错误 需要输入验证码QQ；获取下一个QQ
        /// <summary>
        /// IP是否重复
        /// </summary>
        /// <param name="pcname"></param>
        /// <returns></returns>
        private bool ValidateIPFromServer(string pcname)
        {
            //TODO...
            return false;
        }
        /// <summary>
        /// 发送需要验证码的QQ给服务器
        /// </summary>
        private void SendNeedVeryCodeQQToServer(string qqno)
        {
            //TODO...
        }

        /// <summary>
        /// 发送密码错误的QQ给服务器
        /// </summary>
        private void SendPassErrorQQToServer(string qqno)
        {
            //TODO...
        }

        private int qqIdx = 1;

        string[] qqno = new string[] { "2242362305", "1519580187", "1063715267", "1062457275", "1064073775", "1064057103", "1061930934" };
        string[] qqpass = new string[] { "hangbeic", "nuosjiao", "xd1550000", "xd1550000", "xd1550000", "xd1550000", "xd1550000" };
        /// <summary>
        /// 获取QQ号
        /// </summary>
        /// <returns></returns>
        public virtual string[] GetQQNoAndPass()
        {
            var qqList = new string[2];
            qqList[0] = qqno[qqIdx];
            qqList[1] = qqpass[qqIdx];
            qqIdx++;
            if (qqIdx > qqno.Length - 1)
                qqIdx = 0;
            return qqList;
        }

        /// <summary>
        /// 此QQ操作成功！
        /// </summary>
        /// <param name="qqno"></param>
        public virtual void UpdateSuccessToServer(string qqno)
        {
            //TODO...
        }
        #endregion

        #region 关于验证码

        #region 下载频繁 输入后继续下载

        //ps 269 270   verycode rect 319,230  431,280
        //【确认】 按钮380 605


        #endregion

        /// <summary>
        /// 登录的时候 输入 验证码
        /// </summary>
        /// <param name="safeHandle"></param>
        private void InputVeryCodeOnLogin(IntPtr safeHandle)
        {
            var rect = GetFormRect(safeHandle);
            var veryCode = Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCode(
                rect.Left + PositionInfoQQMusic.IDCodeSafeImgLeftTopPt.X,
                rect.Top + PositionInfoQQMusic.IDCodeSafeImgLeftTopPt.Y
                , PositionInfoQQMusic.IDCodeSafeImgSize.Width, PositionInfoQQMusic.IDCodeSafeImgSize.Height);
            Console.WriteLine("QQID:" + veryCode);
            //
            if (veryCode == "1009" || veryCode.Length != 4)
                throw new Exception(" 验证码返回失败");
            //输入验证码
            IntPtr editHandle = SystemWindowsAPI.FindWindowEx(safeHandle, IntPtr.Zero, "Edit", null);
            if (editHandle != IntPtr.Zero)
            {
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

    }
}
