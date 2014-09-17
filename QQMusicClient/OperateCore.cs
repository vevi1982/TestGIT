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

    public delegate void ShowInStatusBar(string text);

    public delegate string GetDownLoadIDCOde(IntPtr ieHandle);


    /// <summary>
    /// QQ音乐自动下载操作
    /// </summary>
    public class OperateCore
    {
        public event ShowInStatusBar ShowInStatusBarEvent;
        public event ShowInStatusBar ShowInStatusMonitor;
        public event ShowInStatusBar ShowDownLoadInfo;

        protected virtual void OnShowDownLoadInfo(string text)
        {
            ShowInStatusBar handler = ShowDownLoadInfo;
            if (handler != null) handler(text);
        }

        /// <summary>
        /// 监控器提示信息
        /// </summary>
        /// <param name="text"></param>
        protected virtual void OnShowInStatusMonitor(string text)
        {
            ShowInStatusBar handler = ShowInStatusMonitor;
            if (handler != null) handler(text);
        }

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


        private System.Windows.Forms.Timer identifyingTimer;
        private const string didcInfo = "您下载歌曲的次数过于频繁";

        /// <summary>
        /// 最大延时
        /// </summary>
        public int maxTime = 30;

        /// <summary>
        /// 
        /// </summary>
        public Models.QQInfo qqModel { get; set; }
       

        public Dlls.IServer Server { get; set; }
        /// <summary>
        /// 是否下载完成
        /// </summary>
        public bool IsDownLoadOver = false;

        public IHeart IheartOpe { get; set; }
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


        #region 构造函数
        public OperateCore()
        {
            Server = new ServerToInternet();
            IheartOpe = new HeartOperate(qqModel);
            identifyingTimer = new System.Windows.Forms.Timer { Interval = 10 * 1000 };
            identifyingTimer.Tick += identifyingTimer_Tick;
            //
            WorkThreadFlag = true;
        }
        public OperateCore(IServer server, IHeart heart)
        {
            Server = server;
            IheartOpe = heart;
        }
        #endregion

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

        #region 整体操作

        public bool WorkThreadFlag { get; set; }
        private Thread heartTh;
        private Thread workTh;
        public void DoWork()
        {
            //开启心跳线程。
            //qq 歌单 下载数量 改变的时候 正常心跳，如果没有变化，停发心跳
            //heartTh = new Thread(() =>
            //    {
            //        //心跳程序 统一放到监控timer中
            //        while (IheartOpe.IsChangedContent(qqModel))
            //        {
            //            Server.SendHeart(AppConfig.PCName);
            //            Thread.Sleep(2*60*1000);
            //        }
            //    }) {IsBackground = true};
            //heartTh.Start();
            //工作线程
            workTh = new Thread(() =>
                {
                    //在十一点早凌晨1点之间不进行操作
                    while (WorkThreadFlag)//DateTime.Now.Hour<23&&DateTime.Now.Hour>1)
                    {
                        try
                        {
                            OnShowInStatusBarEvent("Start00000....");
                            GetQQAndDownLoadOperate();
                        }
                        catch (Exception e1)
                        {
                            OnShowInStatusBarEvent(e1.Message);
                            
                        }
                        
                    }
                    OnShowInStatusBarEvent("停止循环下载！");
                }) {IsBackground = true};
            workTh.Start();
        }
        public void Stop()
        {
            try
            {
                //heartTh.Abort();

                workTh.Abort();
               
                
            }
            catch (Exception)
            {
                
                //throw;
            }
            
        }

        public bool WorkThreadIsALive
        {
            get { return workTh != null && workTh.IsAlive; }
        }

        /// <summary>
        /// 获取QQ信息，并下载其所带所有歌单
        /// </summary>
        public void GetQQAndDownLoadOperate()
        {
            OnShowInStatusBarEvent("开始...");
            IsDownLoadOver = false;
            isMoniterStart = false;
            ChangeIP();
            OnShowInStatusBarEvent("更换IP（如果）...");
            var count = 0;
            var model = Server.GetQQFromServer();
            //没有获取就要一直获取，长时间失败 心跳不发
            while (model == null&&count<4)
            {
                count++;
                OnShowInStatusBarEvent("没有获取到QQ");
                Thread.Sleep(1000);
                Application.DoEvents();
                model = Server.GetQQFromServer();
                //throw new Exception("没有获取到QQ");
            }
            if(count>3)
                throw new Exception("没有获取到QQ");
            //
            var orderList = model.SongOrderList.Keys.ToList();
            //for (int i = 0; i < orderList.Count; i++)
            ////foreach (string key in model.SongOrderList.Keys)
            //{
            var i = 0;
                var key = orderList[i];
                try
                {
                    //identifyingTimer.Stop();
                   
                    DoOnce(model,key);
                    //开启监视器堵塞线程？？
                    IsDownLoadOver = false;
                    isMoniterStart = true;
                    //此时还有一个问题，就是如果堵塞时间太长，（下载没变化，那么提交下载数。结束线程。）

                    //StartDownLoadTimer();
                    //
                    //var thmonitor=new Thread(() =>
                    //    {
                    //        while (!IsDownLoadOver)
                    //        {
                    //            //DownLoadInfoMonitor();
                    //            //
                    //            Console.WriteLine("Monitor Thread");
                    //            Thread.Sleep(10*1000);
                    //        }
                    //    });
                    //thmonitor.Start();
                    //thmonitor.Join();
                    while (!IsDownLoadOver)
                    {
                        //DownLoadInfoMonitor();
                        //
                        Console.WriteLine("Monitor Thread");
                        Thread.Sleep(10*1000);
                        Application.DoEvents();
                    }

                }
                catch (Exception e1)
                {
                    SendServerDownInfo();
                    ClearSongFolderAndCloseMain();
                    //throw;
                    Thread.Sleep(5000);
                    OnShowInStatusBarEvent("aaa  "+ e1.Message);
                    Thread.Sleep(5000);
                    qqModel = null;
                }
            //}
        }

        public void StartMonitor_T()
        {
            IsDownLoadOver = false;
            //此时还有一个问题，就是如果堵塞时间太长，（下载没变化，那么提交下载数。结束线程。）
            var thmonitor = new Thread(() =>
            {
                while (!IsDownLoadOver)
                {
                    DownLoadInfoMonitor();
                    //
                    Thread.Sleep(10 * 1000);
                }
            });
            thmonitor.Start();
        }
        #endregion

        #region 单步操作
        /// <summary>
        /// 播放一个歌单
        /// </summary>
        /// <param name="qqinfo"></param>
        /// <param name="songlistName"></param>
        public void DoOnce(Models.QQInfo qqinfo, string songlistName)
        {
            //初始化数据
            if (!qqinfo.SongOrderList.ContainsKey(songlistName))
            {
                OnShowInStatusBarEvent("没有此歌单");
                throw new Exception("没有此歌单");
            }
            qqinfo.CurrentDownloadCount = 0;
            qqinfo.CurrentSongOrderName = songlistName;
            qqinfo.BeginTimeStamp = HttpWebResponseUtility.GetTimeStamp(DateTime.Now);
            qqModel = qqinfo; //
            //
            OnShowDownLoadInfo(string.Format("QQ:{0}.歌单:{1},歌单数量:{2},当前下载:{3}", qqModel.QQNo, qqModel.CurrentSongOrderName, qqModel.SongOrderList[qqModel.CurrentSongOrderName],qqModel.CurrentDownloadCount));
            //
            //0.清理下载文件夹和缓存文件夹
            ClearSongFolderAndCloseMain();
            //1.启动QQ
            IntPtr mainhandle = StartApp();
            mainHandler = mainhandle;
            //2.登录QQ
            LoginQQ(mainhandle, qqinfo);//登录失败则抛出异常
            //3.清理临时歌单与下载列表
            DeleteDownLoadList(mainhandle);
            Thread.Sleep(3000);
            DeleteTrySongList(mainhandle);
            Thread.Sleep(3000);
            //4.点击共享歌单名称
            ClickSongListAndLoadToTryList(mainhandle,songlistName);
            //5.下载歌单
            DownLoadSong_TryList(mainhandle);
            //
            //
            OnShowDownLoadInfo(string.Format("QQ:{0}.歌单:{1},歌单数量:{2},当前下载:{3}", qqModel.QQNo, qqModel.CurrentSongOrderName, qqModel.SongOrderList[qqModel.CurrentSongOrderName], qqModel.CurrentDownloadCount));
            //
        }
        #endregion

        #region 启动程序
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
            mainHandler = mainHanle;
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
            var Count = 0;
            foreach (Process process in System.Diagnostics.Process.GetProcesses())
            {
                if (process.ProcessName == processName)
                    count++;
            }
            return count;
        }
        #endregion


        #endregion

        #region 登录QQ，如果密码错误 ，提交。重新获取QQ登录

        public void LoginQQ(IntPtr mainHanle,Models.QQInfo qqinfo)
        {
            OnShowInStatusBarEvent("查看是否已登录!");
            QQLogOut(mainHanle);
           
            //登录QQ,只有QQ密码错误的时候才重新登录。否则直接重启流程
            var successOK = false;
            do
            {
                OnShowInStatusBarEvent("开始获取QQ！");

                #region 获取QQ
                //获取QQ号
                qqModel = qqinfo;
                OnShowInStatusBarEvent("获取QQ：" + qqModel.QQNo);
                #endregion

                //
                successOK = QQLoginSuccess(mainHanle, qqModel.QQNo, qqModel.QQPass);
                if (!successOK)
                    OnShowInStatusBarEvent(qqModel.QQNo + "密码错误，重新获取QQ！");
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
            Thread.Sleep(AppConfig.TimeAlertCHangeUser * 1000);
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
            //判断是否关闭,即当前窗体是否主窗体
            OnShowInStatusBarEvent("判断是否关闭,即当前窗体是否主窗体！");
            if (!ForgroundIsMain(mainHandle, maxTime))
            {
                OnShowInStatusBarEvent("等待超时，【QQ登陆框】没有关闭。!");
                throw new Exception("等待超时，【QQ登陆框】没有关闭。");
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
            //是否有登录失败，请输入用户名和密码登陆
            if (!ForgroundIsMain(mainHandle))
            {
                msgHandle = SystemWindowsAPI.GetForegroundWindow();
                var strtitle = new StringBuilder(256);
                SystemWindowsAPI.GetWindowText(msgHandle, strtitle, 257);
                if (strtitle.ToString().ToLower() == "error")
                {
                    OnShowInStatusBarEvent("密码错误");
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
                SendNeedVeryCodeQQToServer(CurrentQQNo);
                //输入验证码
                SystemWindowsAPI.SetActiveWindow(safeHandle);
                InputVeryCodeOnLogin(safeHandle);
            }
        }
        /// <summary>
        /// 主程序是否无响应,线程方法
        /// </summary>
        /// <returns></returns>
        public bool GetMainResponseByProcess()
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
        #endregion

       

        #region 清理信息及删除下载列表
        /// <summary>
        /// 清理所有信息
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
        /// 删除歌曲下载文件夹内所有文件
        /// </summary>
        public void ClearSongFolderAndCloseMain()
        {
            //如果QQ音乐打开，那么关闭
            foreach (var p in System.Diagnostics.Process.GetProcesses())
            {
                if (p.ProcessName.Contains("QQMusic") && p.MainModule.FileName.ToLower() == AppConfig.AppPath.ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    p.Kill();
                }
            }
            //删除下载文件
            Vevisoft.IO.Directory.DeleteDirectoryContent(AppConfig.DownLoadPath);
            //删除缓存文件
            Vevisoft.IO.Directory.DeleteDirectoryContent(AppConfig.AppCachePath);
        }

        #region 删除下载列表内容
        /// <summary>
        /// 删除下载列表中所有文件
        /// </summary>
        /// <param name="mainHandle"></param>
        public void DeleteDownLoadList(IntPtr mainHandle)
        {
            if (mainHandle == IntPtr.Zero)
                return;
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
        

        #region 下载歌曲

       
        #region 点击【点歌】按钮，加载共享歌单信息
        private IntPtr songListIEHandle = IntPtr.Zero;
        private bool isContainsSongListIE = false;
        /// <summary>
        /// 加载个人信息页面，并且点击【点歌】加载共享歌单。将此歌单内容添加至临时歌曲列表中
        /// </summary>
        /// <param name="mainHandle"></param>
        /// <returns></returns>
        public bool GetSongListHtml(IntPtr mainHandle)
        {
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
                    OnShowInStatusBarEvent("没有发现【点歌】按钮!");
                    throw new Exception("没有发现【点歌】按钮!");
                }
            }
            return isContainsSongListIE;

        }

        
        #region 遍历窗体寻找 【点歌内容】

        private bool EnumWindowCallBack_SongList(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            Vevisoft.WindowsAPI.SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (!(strclsName.ToString().Trim().ToLower() == "TXGFLayerMask".ToLower() ||
                  strclsName.ToString().Trim().ToLower() == "TXGuiFoundation".ToLower())) //QQ音乐 查找歌单
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
       
        #endregion
        #region 点击【共享歌单名称播放按钮】添加到临时歌曲列表中

        /// <summary>
        /// 点击【共享歌单名称播放按钮】添加到临时歌曲列表中
        /// </summary>
        /// <param name="mainhandle"></param>
        /// <param name="songlistName"></param>
        public void ClickSongListAndLoadToTryList(IntPtr mainhandle, string songlistName)
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
        #endregion

        #region 下载临时列表中的歌曲
        /// <summary>
        /// 下载临时列表中的歌曲
        /// </summary>
        /// <param name="mainhandle"></param>
        public void DownLoadSong_TryList(IntPtr mainhandle)
        {

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
                if (cannotDown)
                {  
                    //此QQ下载数目当天够了，发送服务器
                    Server.UpdateDownLoadResult(qqModel);
                    //
                    Console.WriteLine(QQDownLoadOverLimit);
                    throw new Exception(QQDownLoadOverLimit);
                }
                Thread.Sleep(500);
                MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.DownLoadDiagButtonPt);
                //启动计时器 判断验证码输入框是否存在，以及是否下载完成
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
            //判断10s是否超限
            OnShowInStatusBarEvent("bbbbbbb");
            SystemWindowsAPI.SetForegroundWindow(mainhandle);
            OnShowInStatusBarEvent("dddddddddddd");
            dwlHandle = GetDownLoadDiag(15);
            OnShowInStatusBarEvent("ccccccccc");
            if (dwlHandle != IntPtr.Zero)
            {
                OnShowInStatusBarEvent("eeeeeeeeee");
                if (cannotDown)
                {
                    //此QQ下载数目当天够了，发送服务器
                    Server.UpdateDownLoadResult(qqModel);
                    Console.WriteLine(QQDownLoadOverLimit);
                    throw new Exception(QQDownLoadOverLimit);
                }
                //
               // StartDownLoadTimer();
            }
        }
        #endregion

        #region 发送服务器，下载数量
        public void SendServerDownInfo()
        {
            if (qqModel == null)
                return;
            qqModel.CurrentDownloadCount = GetSongCountFromFolder();
            //
            Server.UpdateDownLoadResult(qqModel);
            //
            OnShowInStatusBarEvent(string.Format("下载,{0}:{1}:{2}", qqModel.QQNo, qqModel.CurrentSongOrderName,
                                                 qqModel.CurrentDownloadCount));
        }
        #endregion

        #region 下载监视器，监视下载数量，监视下载验证码，输入下载验证码
        /// <summary>
        /// 启动下载监视计时器
        /// </summary>
        public void StartDownLoadTimer()
        {
            IsDownLoadOver = false;
            mainHandler = GetMainForm();
           
            OnShowInStatusBarEvent("开始下载");
            identifyingTimer.Start();
        }
        /// <summary>
        /// 判断是否有【下载验证码输入框】，判断下载是否完成！！
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void identifyingTimer_Tick(object sender, EventArgs e)
        {
            Console.WriteLine("Timer");
            identifyingTimer.Stop();
            ////
            DownLoadInfoMonitor();
            //
            identifyingTimer.Start();
        }
        /// <summary>
        /// 是否启动下载数量监视
        /// </summary>
        private bool isMoniterStart = false;
        /// <summary>
        /// 下载监视，下载数量是否完成
        /// </summary>
        public void DownLoadInfoMonitor()
        {
            if (!isMoniterStart)
                return;
            //
            var songCount = GetSongCountFromFolder();
            //更新下载数
            qqModel.CurrentDownloadCount = songCount;
            //判断下载是否完成
            if (songCount == qqModel.SongOrderList[qqModel.CurrentSongOrderName])
            {
                //下载完成。。。
                SendServerDownInfo();
                //清楚下载缓存，及列表
                ClearALlInfos(mainHandler);
                //
                OnShowInStatusBarEvent(qqModel.CurrentSongOrderName + "下载完成," + qqModel.CurrentDownloadCount);
                IsDownLoadOver = true;
                return;
            }
            else
            {
                OnShowDownLoadInfo(string.Format("QQ:{0}.歌单:{1},歌单数量:{2},当前下载:{3}", qqModel.QQNo, qqModel.CurrentSongOrderName, qqModel.SongOrderList[qqModel.CurrentSongOrderName], qqModel.CurrentDownloadCount));
                OnShowInStatusBarEvent("已下载" + songCount);
            }
        }
        public void JudgeDownLoadIDCodeAndInput()
        {
            try
            {
                //OnShowInStatusMonitor("查找下载验证码窗体");
                if (GetDownLoadIdCodeDiagExist())
                {
                    OnShowInStatusMonitor("下载验证码窗体出现");
                    //当前窗体不是主窗体，那么 默认 就是输入验证码窗体 弹出
                    InputDownLoadIdentifyingCode();
                }
            }
            catch (Exception e1)
            {
                OnShowInStatusMonitor("下载验证码窗体" + e1.Message);
            }
        }
        /// <summary>
        /// 输入下载对话框的验证码
        /// </summary>
        private void InputDownLoadIdentifyingCode()
        {
            //var mainhandle = GetMainForm();          
            //Thread.Sleep(3000);
            //
            //GetDownLoadIdCodeDiagExist();
            var idcode = GetIDCodeDownLoad(didcIEHandler);
            //var idcode = OnGetDownLoadIdcOdeEvent(didcIEHandler);
            OnShowInStatusMonitor("验证码" + idcode);
            //
            Console.WriteLine(idcode);
           
            mshtml.IHTMLDocument2 id = GetHtmlDocument(didcIEHandler);   
                //
            if (idcode == "1009" || idcode.Length != 4)
            {
                OnShowInStatusMonitor("验证码返回失败");
                id.parentWindow.execScript("showVcode()", "javascript");
                throw new Exception(" 验证码返回失败");
            }
            //            
            id.parentWindow.execScript("showElement('id_verify')", "javascript");
            var idtext = id.all.item("vcode", 0) as IHTMLElement;
            if (idtext != null)
            {
                OnShowInStatusMonitor("输入验证码" + idcode);
                idtext.setAttribute("value", idcode);
            }
            else
                OnShowInStatusMonitor("没有找到输入验证码" + idcode);
            //var btnok = id.all.item("", 0) as IHTMLElement;
            foreach ( IHTMLElement btnok in id.links)
            {
                if (btnok.innerHTML.Contains("确认"))
                {
                    OnShowInStatusMonitor("点击确认按钮" + idcode);
                    btnok.click();
                    break;
                }
            }
            var errortip = id.all.item("error_tips", 0) as IHTMLElement;
            if (errortip != null && errortip.innerHTML.Contains("错误"))
            {
                OnShowInStatusMonitor("验证码错误" + idcode);
                id.parentWindow.execScript("showVcode()", "javascript");
            }
        }
        private string GetIDCodeDownLoad(IntPtr didcIeHandle)
        {
            mshtml.IHTMLDocument2 id = GetHtmlDocument(didcIeHandle);
            if (id == null)
                return "";
            OnShowInStatusMonitor("获取验证码图片");
            IHTMLControlElement img =
                id.images.Cast<IHTMLElement>().Where(item => item.id == "imgVerify").Cast<IHTMLControlElement>().FirstOrDefault();
            if (img != null)
            {
                IHTMLControlRange range = (IHTMLControlRange)((HTMLBody)id.body).createControlRange();
                range.add(img);
                bool bol1 = range.execCommand("Copy", false, null);
                if (Clipboard.ContainsImage())
                {
                    OnShowInStatusMonitor("保存验证码图片");
                    Clipboard.GetImage().Save(@"c:\bb.bmp");
                    OnShowInStatusMonitor("保存验证码图片成功");
                    return Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCodeWeb(@"c:\bb.bmp", 1004);
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
            //缓存下载量
            var catchCount = 0;
            var catchPath = AppConfig.AppCachePath;
            if (catchPath.EndsWith("\\"))
               catchPath.Substring(0, AppConfig.AppCachePath.Length - 1);
            var QMDL = string.Format("{0}\\QMDL", AppConfig.AppCachePath);
            if (Directory.Exists(QMDL))
            {
                string[] files = Directory.GetFiles(QMDL);
                catchCount += files.Count(file => file.EndsWith(".cache"));
            }
            //复制到下载目录的下载量
            var downCount = 0;
            if (Directory.Exists(AppConfig.DownLoadPath))
                downCount = Directory.GetFiles(AppConfig.DownLoadPath).Length;
            //
            return Math.Max(catchCount, downCount);
        }

        #endregion
       

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
                if (!string.IsNullOrEmpty(faTitle.ToString()) && faTitle.ToString().Contains("下载"))
                {
                    //属于下载对话框的子窗体。查找IE
                    var strclsName = new StringBuilder(256);
                    SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
                    var strTitle = new StringBuilder(256);
                    SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
                    if (strclsName.ToString().Trim().ToLower() != "TXGFLayerMask".ToLower())
                        return true;
                    IntPtr chHandle = SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null,null);
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
                        didcIEHandler = SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
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
                        qqModel.SongOrderList[qqModel.CurrentSongOrderName] = GetSongCount(str);
                            // GetSongCount(str);
                        OnShowInStatusBarEvent("歌单总数为" + qqModel.SongOrderList[qqModel.CurrentSongOrderName]);
                        OnShowInStatusMonitor("歌单总数为" + qqModel.SongOrderList[qqModel.CurrentSongOrderName]);
                        cannotDown = str.Contains("下载数量已达到上限");
                    }
                }
            }
            return true;
        }

        public int GetSongCOuntByliClassName(string html)
        {
            //data-downtype  查看此字段包含的次数
            return System.Text.RegularExpressions.Regex.Matches(html, "data-downtype").Count;
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
            int idx = str.IndexOf("首",idx2);
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
        /// <summary>
        /// 查找下载验证框是否存在
        /// </summary>
        /// <returns></returns>
        public bool GetDownLoadIdCodeDiagExist()
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
            SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (!(strclsName.ToString().Trim().ToLower() == "TXGFLayerMask".ToLower()||
                strclsName.ToString().Trim().ToLower() == "TXGuiFoundation".ToLower()))//QQ音乐 查找歌单
                return true;
            count++;
            //richTextBox1.AppendText(string.Format("{3}---Title:{0};  ClassName:{1};  Hwnd:{2}\r\n", strTitle, strclsName, hwnd.ToString(), count));
            //
            IntPtr chHandle = SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null,null);
            if (chHandle != IntPtr.Zero)
            {
                try
                {
                    strclsName = new StringBuilder(256);
                    SystemWindowsAPI.GetClassName(chHandle, strclsName, 257);
                    strTitle = new StringBuilder(256);
                    SystemWindowsAPI.GetWindowText(chHandle, strTitle, 257);
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
                    string cookies = id.cookie;
                    if (str != null && str.Contains("请输入验证码"))
                    {
                        OnShowInStatusMonitor("下载验证码窗体IE句柄");
                        didcIEHandler = didcIEHandler1;
                        isexistDownLoadIDCodeDiag = str.Contains(didcInfo);
                        OnShowInStatusMonitor("下载验证码窗体IE句柄" + (isexistDownLoadIDCodeDiag ? "111" : "0000"));
                        //
                    }
                }
                catch 
                {
                    
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
        private bool ValidateIPFromServer(string ip)
        {
            ip = Server.GetIP();
            OnShowInStatusBarEvent("当前IP"+ip);
            return Server.IPIsRepeat(ip);
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

        string[] qqno = new string[] {  "1062457275", "1064073775", "1064057103", "1061930934" };
        string[] qqpass = new string[] {  "xd1550000", "xd1550000", "xd1550000", "xd1550000" };
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
        public virtual void UpdateSuccessToServer()
        {
            
        }

        public virtual void SetQQDownLoadEnough()
        {
            
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
            OnShowInStatusBarEvent("QQIDCode:" + veryCode);
            Console.WriteLine("QQIDCode:" + veryCode);
            //
            if (veryCode == "1009" || veryCode.Length != 4)
            {
                OnShowInStatusBarEvent("验证码返回失败" + veryCode);
                throw new Exception(" 验证码返回失败");
            }
            //输入验证码
            IntPtr editHandle = SystemWindowsAPI.FindWindowEx(safeHandle, IntPtr.Zero, "Edit", null);
            if (editHandle != IntPtr.Zero)
            {
                OnShowInStatusBarEvent("SetControlValue");
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


        #region 测试方法
        public void DownLoadSongsBySongListName(IntPtr mainhandle, string songlistName)
        {
            if (GetSongListHtml(mainhandle))
            {
                Thread.Sleep(4000);
                //
                if (isContainsSOngListAndClick(songlistName))
                {
                    Thread.Sleep(2000);
                    //
                    MouseSetPositonAndLeftClick(mainhandle, PositionInfoQQMusic.MaintrySongListBtnPt);

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
                    //
                    //等待响应
                    while (!SystemWindowsAPI.IsExeNotResponse(mainhandle)
                        || !GetMainResponseByProcess())
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
                    else
                    {
                        OnShowInStatusBarEvent("下载对话框没有找到");
                        throw new Exception("下载对话框没有找到");
                    }
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
                else
                {
                    OnShowInStatusBarEvent("没有共享点歌内容");
                    throw new Exception("没有共享点歌内容");
                }
            }
            else
            {
                OnShowInStatusBarEvent("没有共享点歌");
                throw new Exception("没有共享点歌");
            }
        }
        #endregion
        #region 无用方法
        private void DoOnce()
        {
            ClearSongFolderAndCloseMain();
            //1.改变IP

            #region CHangeIP

            ChangeIP();

            #endregion

            OnShowInStatusBarEvent("开始启动QQ音乐!");
            //
            IntPtr mainHanle = StartApp();
            OnShowInStatusBarEvent("启动完成！");
            Thread.Sleep(1000);
            //
            OnShowInStatusBarEvent("启动登陆！");
            LoginQQ(mainHanle, qqModel);
            //
            Thread.Sleep(1000);
            OnShowInStatusBarEvent("开始下载歌曲！");
            //下载歌曲 并启动判断程序
            DownLoadSongsBySongListName(mainHanle, "AAA");
            //下载完毕，删除歌曲
            while (!IsDownLoadOver)
            {
                Thread.Sleep(1000);
                Application.DoEvents();
            }
            //
            ClearALlInfos(mainHanle);
            IsDownLoadOver = false;
        }

        /*
         * 点击 播放列表
         * 点击 歌曲列表区域
         * Ctrl A
         * 【下载】按钮，（键盘）下，（键盘）下，回车
         * 判断下载窗体 点击 下载到电脑 按钮
         */

        /// <summary>
        /// 点击播放列表，下载歌曲
        /// </summary>
        /// <param name="mainhandle"></param>
        private void DownLoadSongs(IntPtr mainhandle)
        {
            SystemWindowsAPI.SetForegroundWindow(mainhandle);
            DeleteTrySongList(mainhandle);
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
            else
            {
                OnShowInStatusBarEvent("下载对话框没有找到");
                throw new Exception("下载对话框没有找到");
            }
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

        #endregion

        public int SendHeartFailedCount { get; set; }
        public bool IsInfoChanged { get { return IheartOpe.IsChangedContent(qqModel); } }
        /// <summary>
        /// 发送心跳
        /// </summary>
        /// <param name="p"></param>
        internal void SendHeart(string p)
        {
            if (IheartOpe.IsChangedContent(qqModel))
            {
                SendHeartFailedCount = 0;
                OnShowInStatusMonitor("发送心跳!");
                Server.SendHeart(p);
            }
            else
            {
                SendHeartFailedCount++;
                OnShowInStatusMonitor("没有发送心跳! " + SendHeartFailedCount);
            }
        }
    }
}
