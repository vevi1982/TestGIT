using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaiduMusicClient.Models;
using Vevisoft.Log;
using Vevisoft.Utility.Web;

namespace BaiduMusicClient.ServerAction
{
    public class LocalServer : ISongServerAction
    {
        // Fields
        private List<string> CurrentDayIPList = new List<string>();
        private DateTime? dt = null;
        private AdslUtility utility = new AdslUtility(AppConfig.AdslName);

        // Methods
        public List<BaiduSongModel> GetSongInfoFromServer()
        {
            List<BaiduSongModel> list = new List<BaiduSongModel>();
            foreach (string str in AppConfig.Songs.Split(new char[] { ';' }))
            {
                string[] strArray2 = str.Split(new char[] { ',' });
                if (strArray2.Length == 3)
                {
                    BaiduSongModel item = new BaiduSongModel
                    {
                        SongBaiduId = strArray2[0],
                        SongName = strArray2[1],
                        singerName = strArray2[2]
                    };
                    list.Add(item);
                }
            }
            return list;
        }

        public bool IsIpRepeat()
        {
            string iPFromBaidu = HttpResponseUtility.GetIPFromBaidu();
            VeviLog2.WriteIPLog(iPFromBaidu);
            if (!this.dt.HasValue)
            {
                this.CurrentDayIPList.Add(iPFromBaidu);
                this.dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                return false;
            }
            DateTime time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            if (time.CompareTo(this.dt) != 0)
            {
                this.CurrentDayIPList = new List<string>();
                this.dt = new DateTime?(time);
                this.CurrentDayIPList.Add(iPFromBaidu);
                return false;
            }
            bool flag = this.CurrentDayIPList.Contains(iPFromBaidu);
            if (!flag)
            {
                this.CurrentDayIPList.Add(iPFromBaidu);
            }
            return flag;
        }

        public void UpdateSuccess(string songID, string pcName)
        {
        }
    }


}
