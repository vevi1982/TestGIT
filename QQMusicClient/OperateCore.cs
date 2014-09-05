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

namespace QQMusicClient
{
    /// <summary>
    /// QQ音乐自动下载操作
    /// </summary>
    public class OperateCore
    {
        private System.Windows.Forms.Timer identifyingTimer;
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
        private IntPtr mainHandler = IntPtr.Zero;
        private IntPtr tmpHandler = IntPtr.Zero;
        public void DoOnce()
        {
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
            if (mainHanle == IntPtr.Zero)
                throw new Exception("没有获取到主窗体句柄");
            mainHandler = mainHanle;
            //
            //主窗体加载完成
            Thread.Sleep(AppConfig.TimeMainFormStart*1000);
            //

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


            Thread.Sleep(1000);
            
            
            //下载歌曲 并启动判断程序
            DownLoadSongs(mainHanle);
            //下载完毕，删除歌曲

            //提交 完成


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

                if (adsl.IsConnectedInternet())
                {
                    //连接上的时候 先断开
                    adsl.StopDailer(AppConfig.ADSLName);
                    while (adsl.IsConnectedInternet())
                    {
                        Thread.Sleep(500);
                    }
                }
                //拨号间隔时间
                Thread.Sleep(AppConfig.ChangeIPInterval);
                //连接ADSL
                adsl.StartDailer(AppConfig.ADSLName, AppConfig.ADSLUserName, AppConfig.ADSLPass);
                while (!adsl.IsConnectedInternet())
                {
                    Thread.Sleep(500);
                }
                //
                if(ValidateIPFromServer(AppConfig.PCName))
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
                QQLogin(mainHanle,qqno,qqpass);
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
        private void QQLogin(IntPtr mainHandle,string qqno,string qqpass)
        {
           
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
            Thread.Sleep(AppConfig.TimeAlertCHangeUser * 1000);
            var count = 0;
            msgHandle = SystemWindowsAPI.GetForegroundWindow();
            while (msgHandle != mainHandle && count < maxTime)
            {
                Thread.Sleep(1000);
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                //安全中心出现，判断5S
                var safeHandle = GetQQSafeCenterForm();
                if (safeHandle != IntPtr.Zero && safeHandle == msgHandle)
                {
                    //判断类型
                    if (IsPassWrong(safeHandle))//密码错误
                    {
                        //关闭安全中心窗体，提交错误QQ并重新获取QQ号密码
                        SystemWindowsAPI.ShowWindow(safeHandle, 0);
                        MouseKeyBoradUtility.KeySendESC();
                        SendPassErrorQQToServer(qqno);
                        //
                        throw new Exception(QQPassErrorMsg);
                    }
                    else if (IsNeedVeryCode(safeHandle))//需要输入验证码
                    {
                        SendNeedVeryCodeQQToServer(qqno);
                        //输入验证码
                        SystemWindowsAPI.ShowWindow(safeHandle, 0);
                        InputVeryCodeOnLogin(safeHandle);
                        //当前窗体是否主窗体？
                        if(!ForgroundIsMain(mainHandle, maxTime))
                            throw new Exception("登录验证码输入错误");
                    }
                }
                Console.WriteLine(count);
                count++;
            }
            if (count >= maxTime)
                throw new Exception("等待超时，退出登录,等待更改用户提示框。");
            //登陆成功！！

        }

        #endregion

        #region 退出登录
        /// <summary>
        /// 退出登录，如果没有登录，那么返回操作
        /// </summary>
        /// <param name="mainHandle"></param>
        private void QQLogOut(IntPtr mainHandle)
        {
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
            if (loginHandle !=IntPtr.Zero)
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
        private void DownLoadSongs(IntPtr mainhandle)
        {
            //1.点击 播放列表
            MouseSetPositonAndLeftClick(mainhandle,PositionInfoQQMusic.MainTryListenButtonPt);
            //
            if (!ForgroundIsMain(mainhandle, 10))
            {
                MouseKeyBoradUtility.KeySendAltF4();
                Thread.Sleep(AppConfig.TimeAlertCHangeUser*1000);
            }
            //2.
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseSetPositonAndLeftClick(mainhandle,PositionInfoQQMusic.MainTryListenPanelFirstSongPt);
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //Ctrl A 全选
            MouseKeyBoradUtility.KeySendCtrlA();
            //
            Thread.Sleep(AppConfig.TimeKeyInterval);
            //
            MouseSetPositonAndLeftClick(mainhandle,PositionInfoQQMusic.MainTryListenPanelDownLoadButtonPt);
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
            var dwlHandle = GetDownLoadDiag();
            if (dwlHandle != IntPtr.Zero)
            {

                Thread.Sleep(500);
                MouseSetPositonAndLeftClick(mainhandle,PositionInfoQQMusic.DownLoadDiagButtonPt);
                //启动计时器 判断验证码输入框是否存在，以及是否下载完成
                Thread.Sleep(AppConfig.TimeMainFormStart);
                StartDownLoadTimer();
                //
            }else throw new Exception("下载对话框没有找到！");
        }
        /// <summary>
        /// 启动下载监视计时器
        /// </summary>
        public void StartDownLoadTimer()
        {
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
                if (!ForgroundIsMain(mainHandler))
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

            }
        }
        public static Bitmap CutScreen(Rectangle rect)
        {
            Bitmap myImage = new Bitmap(rect.Width, rect.Height);
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
            //点击 输入框 获取验证码
            MouseSetPositonAndLeftClick(mainHandler,PositionInfoQQMusic.VeryCodeDownLoadTxtPt);
            var rect = GetFormRect(mainHandler);
            Thread.Sleep(AppConfig.TimeIdCodeLoad*1000);
            //获取验证码
            CutScreen(new Rectangle(
                rect.Left + PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.X,
                  rect.Top+ PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.Y,
                    PositionInfoQQMusic.IDCodeImgSize.Width,
                     PositionInfoQQMusic.IDCodeImgSize.Height)).Save("aa.bmp");
            var idcode =
                Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCode(
                     rect.Left + PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.X,
                    rect.Right + PositionInfoQQMusic.VeryCodeDownLoadImgLeftTopPt.Y,
                    PositionInfoQQMusic.IDCodeImgSize.Width,
                     PositionInfoQQMusic.IDCodeImgSize.Height);
            if (idcode == "1009" || idcode.Length != 4)
                throw new Exception(" 验证码返回失败");
            //输入验证码
            MouseKeyBoradUtility.KeyInputStringAndNumber(idcode,1000);
            //点击确定按钮
            MouseSetPositonAndLeftClick(mainHandler,PositionInfoQQMusic.VeryCodeDownLoadOKPt);
            //判断是否成功。。。。
            //
            if (!ForgroundIsMain(mainHandler, maxTime))
                throw new Exception("验证码输入错误");
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
        /// 下载对话框，点击开始下载
        /// </summary>
        /// <returns></returns>
        public IntPtr GetDownLoadDiag()
        {
            //下载,   TXGuiFoundation
            const string caption = "下载";
            IntPtr handle = SystemWindowsAPI.FindMainWindowHandle(caption, 500, maxTime);
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
            tmpHandler = msgHandle;
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
            return true;
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

        private int qqIdx = 0;

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
                rect.Left + 57,rect.Top + 114,130,50);
            //
            if(veryCode=="1009"||veryCode.Length!=4)
                throw new Exception(" 验证码返回失败");
            //输入验证码
            MouseKeyBoradUtility.KeySendTab();
            Thread.Sleep(300);
            MouseKeyBoradUtility.KeySendTab();
            Thread.Sleep(300);
            MouseKeyBoradUtility.KeyInputStringAndNumber(veryCode,200);
            Thread.Sleep(200);
            MouseKeyBoradUtility.KeySendTab();
            Thread.Sleep(200);
            MouseKeyBoradUtility.KeySendEnter();
        }
        #endregion

    }
}
