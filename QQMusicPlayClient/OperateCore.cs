using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using QQMusicHelper;
using Vevisoft.WebOperate;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：QQMusicPlayClient
 *文件名：  OperateCore
 *版本号：  V1.0.0.0
 *唯一标识：99bb75d8-fd25-4605-a2ef-81b81e4673ec
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/3 23:47:30
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/3 23:47:30
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace QQMusicPlayClient
{
    public delegate void ShowInStatusBar(string text);
    public class OperateCore
    {
        private object _threadLockObj=new object();

        private bool mainThreadRun = true;

        private Thread _workThread;
        //
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



        #endregion

        public void StartWork()
        {
            //需要获取实际时间控制停止与否
            if (_workThread != null && _workThread.IsAlive)
                _workThread.Abort();
            if (_workThread != null)
                while (_workThread.IsAlive)
                {
                    Thread.Sleep(1000);
                }
            //确保线程是结束的
            _workThread = new Thread(() =>
                {
                    try
                    {
                        while (mainThreadRun)
                        {
                            try
                            {
                                PlayOneQQ();
                            }
                            catch (Exception e1)
                            {
                                if (e1.Message.StartsWith("可能密码错误"))
                                {
                                    //throw;
                                }
                                if (e1.Message.StartsWith("密码错误"))
                                {
                                    passWrongTimes = 0;
                                    //throw;
                                }
                                OnShowStepEvent(e1.Message);
                                Vevisoft.Log.VeviLog2.WriteLogInfo(e1.Message);
                            }
                        }
                    }
                    catch (Exception e2)
                    {
                        OnShowStepEvent(e2.Message);
                        Vevisoft.Log.VeviLog2.WriteLogInfo(e2.Message);
                        //throw;
                    }
                    

                });
            _workThread.Start();

        }

        public void StopWork()
        {
            _workThread.Abort();
            while (_workThread.IsAlive)
            {
                Thread.Sleep(1000);
            }
        }

        public void ChangeIp()
        {
            try
            {
                ADSLHelper.LinkAdsl(AppConfig.ADSLName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            while (WebUtility.IsIPRepeat())
            {
                Thread.Sleep((int) (AppConfig.ChangeIPInterval*0x3e8));
                ADSLHelper.LinkAdsl(AppConfig.ADSLName);
            }
            //this.Count = 0;
        }

        /// <summary>
        /// 播放一个QQ
        /// </summary>
        public void PlayOneQQ()
        {
            if (AppConfig.ChangeIP)
            {
                OnShowStepEvent("更换IP");
                //todo...
                ChangeIp();
            }
            //
            var wordTime = WebUtility.GetWordDateTime();
            //11.30后停止
            if (AppConfig.CLickPt.X == 300)
                if (wordTime.Hour > 22 && wordTime.Minute > 30)
                {
                    Thread.Sleep(200*1000);
                    return;
                }
            //同步当前的开始时间
            Vevisoft.WindowsAPI.PCTimeUtility.SetSysTime(wordTime);
            AppConfig.ReadValue(); //更新appconifg
            //AppConfig.StartTime = AppConfig.StartTime.AddDays(1);
            //设置开始时间
            Thread.Sleep(1000);
            OnShowStepEvent("设置系统时间");
            Vevisoft.WindowsAPI.PCTimeUtility.SetSysTime(AppConfig.StartTime);
            OnShowStepEvent(AppConfig.StartTime.ToString("yyyy/MM/dd hh:mm:ss"));
            Thread.Sleep(1000);
            //获取QQ号码,临时使用固定号码
            Step_StartExeAndLogin(AppConfig.QQNO, AppConfig.QQPass);

            OnShowStepEvent("开始播放歌曲!");
            var playSongsCount = 0;
            Step_StartPlay();
            while (playSongsCount < AppConfig.PlaySongNo)
            {
                Step_PlayOneSong();
                OnShowStepEvent("播放歌曲" + (playSongsCount + 1));
                playSongsCount++;
            }
            OnShowStepEvent("播放结束!");
            //关闭QQ音乐
            QQMusicOperateHelper.CloseQQMusicExe(AppConfig.AppPath);
            //
            //todo  提交数据到服务器

        }

        private int passWrongTimes = 0;

        public void Step_StartExeAndLogin(string QQNo, string QQPass)
        {
            //1.启动QQMusic
            OnShowStepEvent("启动QQMusic");
            var bol1= QQMusicOperateHelper.StartQQMusic(AppConfig.AppPath, "");
            if(bol1)
            OnShowStepEvent("启动QQMusic完成");
            else OnShowStepEvent("启动QQMusic没有完成！！！");
            //2.退出登录
            //OnShowStepEvent("退出登录");
            //QQMusicOperateHelper.LogOutQQMusic();
            //2.登录
            OnShowStepEvent("登录");
            try
            {
                OnShowHeartEvent(QQNo + ";" + QQPass);

                QQMusicOperateHelper.LoginSmart(QQNo, QQPass, passWrongTimes);
                //Thread.Sleep(1000);
                //if (!QQMusicOperateHelper.JudgeLoginCorrect(QQNo))
                //    throw new Exception("登录失败!");
            }
            catch (Exception e1)
            {
                Vevisoft.Log.VeviLog2.WriteLogInfo(e1.Message);
                if (passWrongTimes < 3)
                {
                    if (e1.Message == QQMusicOperateHelper.QQPassErrorMsg)
                    {
                        passWrongTimes++;
                        throw new Exception("可能密码错误" + passWrongTimes);
                    }
                    throw;
                }
                else
                {
                    //Server.UpdatePassWrongQQ(qqinfo.QQNo);
                    OnShowErrorEvent("密码错误");
                    throw;
                }
            }
            OnShowStepEvent("登录成功!");
        }

        /// <summary>
        /// 开始播放歌曲
        /// </summary>
        public void Step_StartPlay()
        {
            //点击试听列表，点击开始播放按钮
            QQMusicHelper.QQMusicPlaySongControl.StartPlay();
            Thread.Sleep(AppConfig.BeforeClk*1000);
        }
        /// <summary>
        /// 播放一首歌
        /// </summary>
        public void Step_PlayOneSong()
        {
            //修改系统时间
            var currentTime = DateTime.Now.AddSeconds(AppConfig.PlayTime);
            Vevisoft.WindowsAPI.PCTimeUtility.SetSysTime(currentTime);
            //扯动滚动条
            QQMusicPlaySongControl.PlayEndUserSet();
            Thread.Sleep(AppConfig.AfterClk*1000);
            //播放下一条
        }
    }
}
