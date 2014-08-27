using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using BaiduMusicClient.Core;
using BaiduMusicClient.Models;
using BaiduMusicClient.ServerAction;
using Vevisoft.Log;
using Vevisoft.Utility.Web;
using Vevisoft.WebOperate;

namespace BaiduMusicClient.Controler
{
    public delegate BaiDuMusicForIE PlayBaiduMusicDel(BaiduSongModel song);
    public delegate BaiDuMusicForIE PlayBaiduMusicStepTwoDel(BaiDuMusicForIE playdbm);


 

    public class FactoryForAdsl : IWorkFactory
    {
        // Fields
        private AdslUtility adslutility = new AdslUtility(AppConfig.AdslName);
        private List<BaiDuMusicForIE> bdmList = new List<BaiDuMusicForIE>();
        private int Count;
        private int ipCount;
        private bool isRun;
        private readonly ISongServerAction serverAction;
        private List<BaiduSongModel> songList = new List<BaiduSongModel>();
        private int StepOneCount;

        // Methods
        public FactoryForAdsl(ISongServerAction server)
        {
            this.serverAction = server;
            this.PcName = "Cp96";
            this.PlayInterval = 5;
        }

        private void AsyncFuncComplete(IAsyncResult ar)
        {
            this.Count++;
            if (ar != null)
            {
                Console.WriteLine((ar.AsyncState as PlayBaiduMusicDel).EndInvoke(ar));
                if (this.Count == this.songList.Count)
                {
                    for (int i = 0; i < this.PlayInterval; i++)
                    {
                        Thread.Sleep(0x3e8);
                    }
                    this.PlayStepTwo();
                }
            }
        }

        private void AsyncFunCompleteTwo(IAsyncResult ar)
        {
            this.StepOneCount++;
            if (ar != null)
            {
                (ar.AsyncState as PlayBaiduMusicStepTwoDel).EndInvoke(ar);
            }
        }

        public void ChangeIp()
        {
            try
            {
                ADSLHelper.LinkAdsl(AppConfig.AdslName);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
            while (this.serverAction.IsIpRepeat())
            {
                Thread.Sleep((int)(AppConfig.AdslIntervalTime * 0x3e8));
                ADSLHelper.LinkAdsl(AppConfig.AdslName);
            }
            this.Count = 0;
        }

        public void ChangeIp(WebProxy proxy)
        {
            throw new NotImplementedException();
        }

        public WebProxy GetWebProxy()
        {
            throw new NotImplementedException();
        }

        private void PlayOnce()
        {
            while (this.isRun)
            {
                if ((this.Count >= this.songList.Count) && (this.StepOneCount >= this.songList.Count))
                {
                    this.Count = 0;
                    this.StepOneCount = 0;
                    this.songList = this.serverAction.GetSongInfoFromServer();
                    this.bdmList = new List<BaiDuMusicForIE>();
                    this.ChangeIp();
                    this.PlayStepOne();
                }
            }
        }

        private BaiDuMusicForIE PlaySongStepOne(BaiduSongModel song)
        {
            try
            {
                BaiDuMusicForIE item = new BaiDuMusicForIE(song, this.TimeDecrease);
                this.bdmList.Add(item);
                item.SearchAndPlaySong();
                return item;
            }
            catch (Exception exception)
            {
                VeviLog2.WriteLogInfo("刷新歌曲" + song.SongName + "失败！" + exception.Message);
                return null;
            }
        }

        private BaiDuMusicForIE PlaySongStepTwo(BaiDuMusicForIE bdm)
        {
            if (bdm == null)
            {
                return null;
            }
            try
            {
                bdm.Play60AndCompleteStates();
                this.serverAction.UpdateSuccess(bdm.songInfo.SongBaiduId, this.PcName);
                VeviLog2.WriteSuccessLog(bdm.songInfo.SongName + " 成功!");
                VeviLog2.WriteLogInfo("刷新歌曲" + bdm.songInfo.SongName + "成功！");
                return bdm;
            }
            catch (Exception exception)
            {
                VeviLog2.WriteLogInfo("刷新歌曲" + bdm.songInfo.SongName + "失败！" + exception.Message);
                return null;
            }
        }

        private void PlayStepOne()
        {
            this.Count = 0;
            foreach (BaiduSongModel model in this.songList)
            {
                PlayBaiduMusicDel del = new PlayBaiduMusicDel(this.PlaySongStepOne);
                del.BeginInvoke(model, new AsyncCallback(this.AsyncFuncComplete), del);
            }
        }

        private void PlayStepTwo()
        {
            this.StepOneCount = 0;
            foreach (BaiDuMusicForIE rie in this.bdmList)
            {
                PlayBaiduMusicStepTwoDel del = new PlayBaiduMusicStepTwoDel(this.PlaySongStepTwo);
                del.BeginInvoke(rie, new AsyncCallback(this.AsyncFunCompleteTwo), del);
            }
        }

        public void StartWork()
        {
            this.Count = 0;
            this.StepOneCount = 0;
            this.songList = new List<BaiduSongModel>();
            this.isRun = true;
            VeviLog2.WriteLogInfo("Start Work!");
            ThreadPool.QueueUserWorkItem(state => this.PlayOnce());
        }

        public void StopWork()
        {
            this.isRun = false;
            this.Count = 0;
        }

        // Properties
        public string AdslName { get; set; }

        public string PcName { get; set; }

        public int PlayInterval { get; set; }

        public bool TimeDecrease { get; set; }
    }

 

}
