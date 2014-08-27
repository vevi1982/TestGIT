using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BaiduMusicClient.Models
{
    public class PlayTask
    {
        // Methods
        public PlayTask()
        {
            this.PlayEndHour_PerDay = 0x16;
            this.PlayStartHour_PerDay = 8;
            this.PlayInterval_Night = 0;
            this.PlayInterval_Day = 0;
        }

        // Properties
        public int PlayEndHour_PerDay { get; set; }

        public int PlayInterval_Day { get; set; }

        public int PlayInterval_Night { get; set; }

        public int PlayStartHour_PerDay { get; set; }

        public List<BaiduSongModel> SongList { get; set; }
    }

 

}
