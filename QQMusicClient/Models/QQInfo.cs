using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Models
{
    public class QQInfo
    {
        public QQInfo()
        {
            BeginTime = DateTime.Now;
            PCName = AppConfig.PCName;
        }

        public string QQNo { get; set; }
        public string QQPass { get; set; }
        public string PCName { get; set; }
        /// <summary>
        /// 歌单名称
        /// </summary>
        public string SongListName { get; set; }
        /// <summary>
        /// 歌单索引
        /// </summary>
        public int SongListID { get; set; }
        /// <summary>
        /// 歌单数量
        /// </summary>
        public int SongListCount { get; set; }

        public DateTime BeginTime { get; set; }

        public DateTime EndTime { get; set; }

        public int DownLoadNo { get; set; }
    }
}
