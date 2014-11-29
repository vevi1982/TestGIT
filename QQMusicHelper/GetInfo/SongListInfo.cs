using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：QQMusicHelper.GetInfo
 *文件名：  SongListInfo
 *版本号：  V1.0.0.0
 *唯一标识：748fda90-c130-4831-add8-9b92c74e6b57
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/11/26 10:53:26
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/11/26 10:53:26
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace QQMusicHelper.GetInfo
{
    public class SongListInfo
    {
        public string album_mid { get; set; }
        public int diskid { get; set; }
        public string diskname { get; set; }
        public int id { get; set; }
        public int playtime { get; set; }
        public string singer_mid { get; set; }
        public int singerid { get; set; }
        public string singername { get; set; }
        public string song_mid { get; set; }
        public string songname { get; set; }
        public int type { get; set; }
        public string url { get; set; }
        public int i { get; set; }
        public int songListID { get; set; }
        public string orderName { get; set; }
    }

    public class SongListUtility
    {
        /*
         * {"i":"1","type":"13","id":"7045277","song_mid":"004e0npp1k89hp ",
         * "songname":"背叛我 ","singerid":"91864","singer_mid":"003bZUnL4d5OAU ",
         * "singername":"王刚 ","url":"http://stream9.qqmusic.qq.com/19045277.wma",
         * "diskid":"642489","album_mid":"001S5ePm2ZpfKH ","diskname":"王刚单曲集",
         * "playtime":"277"}
         */
        public static SongListInfo GetSongInfos(string songInfoText, string orderName)
        {
            var model = new SongListInfo {orderName = orderName};
            Type p = model.GetType();
            foreach (PropertyInfo propertyInfo in p.GetProperties())
            {
                var value = GetJsonParameter(songInfoText.ToLower(), propertyInfo.Name.ToLower());
                if (value != null)
                {
                    if(propertyInfo.PropertyType==typeof(string))
                        propertyInfo.SetValue(model,value,null);
                    else if (propertyInfo.PropertyType == typeof (int))
                    {
                        var intvalue = 0;
                        if(int.TryParse(value,out intvalue))
                            propertyInfo.SetValue(model,intvalue,null);
                    }
                }
            }
            //获取歌曲在
            model.songListID = model.i;
            //
            return model;
        }

        private static string GetJsonParameter(string jsontext, string paraName)
        {
            var namestr = string.Format("\"{0}\":", paraName);
            int idx = jsontext.IndexOf(namestr, System.StringComparison.Ordinal);
            if (idx < 0)
                return null;
            var idx2 = jsontext.IndexOf(",", idx, System.StringComparison.Ordinal);
            if (idx2 < 0)
                idx2 = jsontext.Length - 1;
            var value = jsontext.Substring(idx + namestr.Length, idx2 - idx - namestr.Length);
            return value.Replace("\"", "").Trim();
        }

        public static List<SongListInfo> GetAllSongOrderInfos(string jsonText)
        {
            var modelList = new List<SongListInfo>();
            var ordername = GetJsonParameter(jsonText, "Title");
            if (ordername != null)
            {
                var startStr = "\"SongList\":[";
                int idx = jsonText.IndexOf(startStr, System.StringComparison.Ordinal);
                if (idx < 0)
                    return null;
                idx += startStr.Length;
                int idx2 = jsonText.IndexOf("]", idx, System.StringComparison.Ordinal);
                if (idx2 < 0)
                    return null;
                var songlistinfoText = jsonText.Substring(idx + 1, idx2 - idx - 2);
                var array = songlistinfoText.Split(new string[] {"{", "}"}, StringSplitOptions.RemoveEmptyEntries);
                modelList.AddRange(from s in array where s.Length > 2 select GetSongInfos(s, ordername));
            }
            return modelList;
        }
    }
}
