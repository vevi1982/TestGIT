using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QQMusicClient.Dlls;
using QQMusicHelper;
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
        public event ShowInStatusBar ShowHeartEvent;

        protected virtual void OnShowHeartEvent(string text)
        {
            ShowInStatusBar handler = ShowHeartEvent;
            if (handler != null) handler(text);
        }

        public event ShowInStatusBar ShowStepEvent;

        protected virtual void OnShowStepEvent(string text)
        {
            ShowInStatusBar handler = ShowStepEvent;
            if (handler != null) handler(text);
        }

        public event ShowInStatusBar ShowErrorEvent;

        protected virtual void OnShowErrorEvent(string text)
        {
            ShowInStatusBar handler = ShowErrorEvent;
            if (handler != null) handler(text);
        }

        public event ShowInStatusBar ShowDownLoadInfo;

        protected virtual void OnShowDownLoadInfo(string text)
        {
            ShowInStatusBar handler = ShowDownLoadInfo;
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
        private  Models.QQInfo qqModel;
        /// <summary>
        /// 发送心跳失败的计数
        /// </summary>
        int FailedSendHeartCount = 0;
        /// <summary>
        /// 是否下载完成
        /// </summary>
        private  bool IsDownLoadOver = false;

        /// <summary>
        /// 服务器选项
        /// </summary>
        public IServer Server { get; set; }

        public IHeart Heart { get; set; }

        public OperateCoreNew()
        {
            Server = new ServerToInternet();
            Heart = new HeartOperate(qqModel);
            //
            MainThreadFlag = true;
        }

        #endregion

        public static bool MainThreadFlag = true;
        private Thread workThread;

        public void StartWorkThread()
        {
            MainThreadFlag = true;
            var update = false;
            workThread = new Thread(() =>
                {
                    try
                    {
                        while (MainThreadFlag)
                        {
                            OnShowStepEvent("开始下载！");
                            //0点开始，晚上11点结束
                            var endtime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 50, 0);
                            var starttime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 00, 30, 0);
                            if (DateTime.Compare(DateTime.Now,starttime)>0&&DateTime.Compare(endtime,DateTime.Now)>0)
                            {
                                try
                                {
                                    update = false;
                                    DoOnQQMusicDownload();
                                }
                                catch (Exception e1)
                                {
                                    Vevisoft.Log.VeviLog2.WriteLogInfo("000  " + e1.Message);
                                    if (qqModel != null)
                                    {
                                        lock (qqModel)
                                        {
                                            GetQQInfo();
                                        }
                                        OnShowStepEvent("讲下载数提交到服务器中。" + (qqModel.OriRemain - qqModel.RemainNum));
                                        Server.UpdateDownLoadResult(qqModel);
                                        //上传歌单下载数
                                        Server.UpdateDownLoadOrder(qqModel);
                                        OnShowHeartEvent("上传下载歌单数" + qqModel.CurrentDownloadCount);
                                    }
                                    //关闭并清理下载信息
                                    QQMusicOperateHelper.CloseAndCLearAll(AppConfig.AppPath, AppConfig.DownLoadPath, AppConfig.AppCachePath);
                                    update = true;
                                }
                            }
                            else
                            {
                                OnShowErrorEvent("时间限制");
                                Thread.Sleep(1000*10);
                            }
                        }
                    }
                    catch (ThreadAbortException e2)
                    {
                        if (!update)
                            if (qqModel != null)
                            {
                                lock (qqModel)
                                {
                                    GetQQInfo();
                                }
                                //关闭并清理下载信息
                                QQMusicOperateHelper.CloseAndCLearAll(AppConfig.AppPath, AppConfig.DownLoadPath, AppConfig.AppCachePath);
                                OnShowStepEvent("讲下载数提交到服务器中。" + (qqModel.OriRemain - qqModel.RemainNum));
                                Server.UpdateDownLoadResult(qqModel);
                            }
                    }
                    
                });
            workThread.IsBackground = true;
            workThread.Start();
        }

        public void StopMainThread()
        {
            MainThreadFlag = false;
        }

        public void AbortMainThread()
        {
            workThread.Abort();
        }
        /// <summary>
        /// 下载一个QQ 800首歌
        /// </summary>
        public void DoOnQQMusicDownload()
        {
            OnShowStepEvent("获取QQ");
            if(qqModel!=null)
            lock (qqModel)
            {
                qqModel = Server.GetQQFromServer();
            }
            else qqModel = Server.GetQQFromServer();
            if (qqModel == null)
            {
                OnShowErrorEvent("无法获取QQ");
                throw new Exception("无法获取QQ");
            }
            OnShowStepEvent("获取到QQ："+qqModel.QQNo);
            var qqdlinfo = DownLoadInfoHelper.GetDownLoadInfo(qqModel.QQNo);
            lock (qqModel)
            {
                qqModel.DLCount = qqModel.OriRemain = qqModel.RemainNum = qqdlinfo.Remain;
                IsDownLoadOver = false;
                FailedSendHeartCount = 0;
                //qqModel.QQPass += "1";
            }
            if (qqModel.RemainNum == 0)
            {
                //使此QQ不可用
                Server.UpdatePassWrongQQ(qqModel.QQNo);
                Thread.Sleep(2*1000);
                return;
            }
            //
            var songlistNames = qqModel.SongOrderList.Keys.ToArray();
            //将此QQ的剩余下载量下完
            while (qqModel.RemainNum != 0)
            {
                for (int i = 0; i < songlistNames.Length; i++)
                {
                    try
                    {
                        lock (qqModel)
                        {
                            qqModel.CurrentSongOrderName = songlistNames[i];
                            GetQQInfo();
                            IsDownLoadOver = false;
                            FailedSendHeartCount = 0;
                        }
                        if (qqModel.RemainNum == 0)
                            break;
                        //下载操作
                        DoOnce(qqModel, qqModel.CurrentSongOrderName);
                        //等待下载结束
                        while (!IsDownLoadOver && FailedSendHeartCount < 1)
                        {
                            Thread.Sleep(10*1000);
                            lock (qqModel)
                            {
                                qqModel.CurrentDownloadCount = GetSongCountFromFolder();
                                IsDownLoadOver = qqModel.CurrentDownloadCount ==
                                                 qqModel.SongOrderList[qqModel.CurrentSongOrderName];
                                if(!IsDownLoadOver)
                                    if (!QQMusicOperateHelper.IsQQMusicStart())
                                    {
                                        IsDownLoadOver = true;
                                        OnShowErrorEvent("QQ音乐已经关闭" + qqModel.CurrentSongOrderName);    
                                        Thread.Sleep(2000);
                                    }
                                ShowDownLoadLogInfo();
                            }
                            //如果超过11:30分，那么抛出异常。退出
                            if (DateTime.Now.Hour == 23 && DateTime.Now.Minute >= 55)
                            {
                                throw new Exception("已经半夜了，该休息了。老板");
                            }
                            //if (IsDownLoadOver)
                            //    break;
                        }
                        OnShowStepEvent("下载完成或中断" + qqModel.CurrentSongOrderName);                     
                        //
                        lock (qqModel)
                        {
                            OnShowStepEvent("获取实际下载数" + qqModel.CurrentSongOrderName); 
                            GetQQInfo();
                            //上传歌单下载数
                            Server.UpdateDownLoadOrder(qqModel);
                            OnShowHeartEvent("上传下载歌单数" + qqModel.CurrentDownloadCount);
                        }
                        //关闭并清理下载信息
                        QQMusicOperateHelper.CloseAndCLearAll(AppConfig.AppPath,AppConfig.DownLoadPath,AppConfig.AppCachePath);
                    }
                    catch (Exception e1)
                    {
                        Vevisoft.Log.VeviLog2.WriteLogInfo("111  " + e1.Message);
                        OnShowErrorEvent(e1.Message);
                        //上传歌单下载数
                        Server.UpdateDownLoadOrder(qqModel);
                        OnShowHeartEvent("上传下载歌单数"+qqModel.CurrentDownloadCount);
                        if (e1.Message == QQMusicOperateHelper.QQPassErrorMsg || e1.Message.StartsWith("已经半夜了"))
                        {
                            OnShowErrorEvent(e1.Message);
                            throw e1;
                        }
                        if (e1.Message.StartsWith("OleAut reported a type mismatch"))
                        {
                            throw e1;
                        }
                    }
                }
                //判断下载数，如果下载数<100.那么可能是歌单为分享，提交错误，更换QQ
                lock (qqModel)
                {
                    if (qqModel.DownLoadNum < 101)
                    {
                        //提交错误QQ
                        Server.UpdatePassWrongQQ(qqModel.QQNo);
                        Vevisoft.Log.VeviLog2.WriteLogInfo("QQ歌单未分享" + qqModel.QQNo);
                        throw new Exception("QQ歌单未分享" + qqModel.QQNo);
                    }    
                }
                
            }
            //提交到服务器中
            lock (qqModel)
            {
                GetQQInfo();
            }
            OnShowStepEvent("讲下载数提交到服务器中。"+(qqModel.OriRemain-qqModel.RemainNum));
            Server.UpdateDownLoadResult(qqModel);
        }

        /// <summary>
        /// 下载一次歌单
        /// </summary>
        /// <param name="qqinfo"></param>
        /// <param name="songlistName"></param>
        public void DoOnce(Models.QQInfo qqinfo, string songlistName)
        {
            QQMusicOperateHelper.ClearSongFolderAndCloseMain(AppConfig.AppPath,AppConfig.DownLoadPath,AppConfig.AppCachePath);
            Thread.Sleep(1000);
            //1.启动QQMusic
            OnShowStepEvent("启动QQMusic");
            QQMusicOperateHelper.StartQQMusic(AppConfig.AppPath, "");
            //2.退出登录
            OnShowStepEvent("退出登录");
            QQMusicOperateHelper.LogOutQQMusic();
            //2.登录
            OnShowStepEvent("登录");
            try
            {
                QQMusicOperateHelper.LoginQQ(qqinfo.QQNo, qqinfo.QQPass);
            }
            catch (Exception e1)
            {
                if (e1.Message == QQMusicOperateHelper.QQPassErrorMsg)
                {
                    Server.UpdatePassWrongQQ(qqinfo.QQNo);
                    OnShowStepEvent("密码错误");
                }
                throw;
            }
            //3.清理下载列表等
            OnShowStepEvent("清理下载列表等");
            QQMusicOperateHelper.ClearAllQQMusicList();
            //4.点击QQ音乐用户操作
            OnShowStepEvent("点击QQ音乐用户操作");
            QQMusicOperateHelper.GetUserInfo();
            Thread.Sleep(4000);
            //5.选择歌曲列表
            QQMusicUserInfo.FindFormAndCLickBtn(songlistName);
            Thread.Sleep(2000);
            //6.点击下载按钮
            OnShowStepEvent("点击下载按钮");
            QQMusicOperateHelper.DownLoadTryListSongs();
            //7.设置下载内容。如果下载数少的话，那么少下载一些
            OnShowStepEvent("等待下载窗体出现。");
            var hwnd = DownLoadSetHelper.GetDownLoadForm();
            var count = 5;
            while (hwnd == IntPtr.Zero && count > 0)
            {
                OnShowStepEvent("等待下载窗体出现。" + (5 - count));
                hwnd = DownLoadSetHelper.GetDownLoadForm();
                Thread.Sleep(2000);
                count--;
            }
            //8.设置下载内容
            OnShowStepEvent("设置下载歌单内容。");
            var songlistCount = 0;
            var downCount = 0;
            DownLoadSetHelper.SetDownNum(qqinfo.QQNo, out songlistCount, out downCount);
            lock (qqModel)
            {
                qqinfo.SongOrderList[songlistName] = downCount;
                //输出
                GetQQInfo();
            }
            //9.等待下载完成
            OnShowStepEvent("等待下载完成。");
        }

        private void GetQQInfo()
        {
            if (qqModel == null)
                return;
            var qqdlinfo = DownLoadInfoHelper.GetDownLoadInfo(qqModel.QQNo);
            lock (qqModel)
            {
                qqModel.RemainNum = qqdlinfo.Remain;
                qqModel.DownLoadNum = qqdlinfo.Dl;
            }
            ShowDownLoadLogInfo();  
        }

        private void ShowDownLoadLogInfo()
        {
            lock (qqModel)
            {
                if (qqModel != null && qqModel.CurrentSongOrderName != null)
                    OnShowDownLoadInfo(
                        string.Format("QQ:{0}.\r\n歌单:{1},\r\n歌单数量:{2},\r\n当前下载:{3},\r\n已记录:{4},\r\n已下载:{5},\r\n剩余:{6},",
                                      qqModel.QQNo, qqModel.CurrentSongOrderName,
                                      qqModel.SongOrderList[qqModel.CurrentSongOrderName], qqModel.CurrentDownloadCount,
                                      qqModel.DayCounter, qqModel.DownLoadNum, qqModel.RemainNum));
                OnShowErrorEvent("显示下载信息。");
            }
        }

        /// <summary>
        /// 获取下载数量
        /// </summary>
        /// <returns></returns>
        private int GetSongCountFromFolder()
        {
            //缓存下载量
            OnShowStepEvent("获取下载数" ); 
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
            foreach (string file in Directory.GetFiles(AppConfig.DownLoadPath))
            {
                if (!file.EndsWith("mp3"))
                    downCount--;
            }
            OnShowStepEvent(string.Format("获取下载数({0},{1})", downCount, catchCount));            
            return downCount;
            //
            return Math.Max(catchCount, downCount);
        }


        /// <summary>
        /// 发送心跳,自动判断数据是否改变
        /// </summary>
        public void SendHeartToServer()
        {
            lock (qqModel)
            {
                if (Heart.IsChangedContent(qqModel))
                {
                    Server.SendHeart(AppConfig.PCName);
                    OnShowHeartEvent("发送心跳");
                }
                else
                {                    
                    FailedSendHeartCount++;
                    if (FailedSendHeartCount > 2)
                        IsDownLoadOver = true;
                    OnShowHeartEvent("没有发送心跳" + FailedSendHeartCount);
                }
            }
        }
    }
}
