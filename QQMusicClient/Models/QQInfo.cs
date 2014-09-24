using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Models
{
    public class QQInfo:ICloneable
    {
        public QQInfo()
        {
            BeginTimeStamp = Vevisoft.WebOperate.HttpWebResponseUtility.GetTimeStamp(DateTime.Now);
            PcName = AppConfig.PCName;
            SongOrderList=new Dictionary<string, int>();
            DLCount = 0;
        }

        public string QQNo { get; set; }
        public string QQPass { get; set; }
        public string PcName { get; set; }

        /// <summary>
        /// 歌单名称,下载数
        /// </summary>
        public Dictionary<string, int> SongOrderList { get; set; }

        ///// <summary>
        ///// 歌单索引
        ///// </summary>
        //public int SongListID { get; set; }
        ///// <summary>
        ///// 歌单数量
        ///// </summary>
        //public int SongListCount { get; set; }

        public string BeginTimeStamp { get; set; }

        public string EndTimeStamp { get; set; }

        /// <summary>
        /// 当日已下载数量
        /// </summary>
        public int DownLoadNum { get; set; }
        /// <summary>
        /// 当日剩余下载数量
        /// </summary>
        public int RemainNum { get; set; }

        public int DayCounter { get; set; }
        /// <summary>
        /// 下载次数
        /// </summary>
        public int DLCount { get; set; }
        //
        //状态参数
        /// <summary>
        /// 当前歌单总数
        /// </summary>
        public int CurrentDownloadCount { get; set; }
        /// <summary>
        /// 当前歌单的名称
        /// </summary>
        public string CurrentSongOrderName { get; set; }


        public object Clone()
        {
            var model = new Models.QQInfo();
            model.QQNo = QQNo;
            model.QQPass = QQPass;
            model.CurrentSongOrderName = CurrentSongOrderName;
            model.CurrentDownloadCount = CurrentDownloadCount;
            model.BeginTimeStamp = BeginTimeStamp;
            model.EndTimeStamp = EndTimeStamp;
            foreach (string key in SongOrderList.Keys)
            {
                model.SongOrderList.Add(key,SongOrderList[key]);
            }
            return model;
        }
    }
}
