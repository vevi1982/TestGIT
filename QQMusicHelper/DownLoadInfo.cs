using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicHelper
{
    public class DownLoadInfoHelper
    {
        private static readonly  string URL_DownLoadInfo =
          "http://s.plcloud.music.qq.com/fcgi-bin/fcg_vip_down_lmt.fcg?uin={0}&outCharset=utf-8&rnd={1}&g_tk=1276264180&loginUin={0}&hostUin=0&format=jsonp&inCharset=GB2312&notice=0&platform=miniframe&jsonpCallback=MusicJsonCallback&needNewCode=0";
        private static readonly string cookies =
                  "PATH=/; ts_uid=1616588603; ts_refer=qqmusic.qq.com/fcgi-bin/qm_gopage.fcg; ts_last=music.qq.com/miniframe/static/profile/profile.html; qm_method=1; qm_hideuin=0; qqmusic_gtime=0; qqmusic_gkey=357D7DF1C62D0CC88CCF9957550F340C9EC5CD01C461D6AA; qqmusic_guid=B00E60A22A3CBD6EBF82F03129A4184A; qqmusic_version=11; qqmusic_miniversion=22; qqmusic_fromtag=3; ac=1,013,008; pgv_pvid=8660477544; qqmusic_privatekey=70E82AF208036D891DA3052F721EDEB261E6C13F7A8CEB40; qqmusic_key=33128A88574C5F6F2FE86BB211792ED21FE28A70703FF51223ED031516C98BBA; qqmusic_uin=254430994; qq_version=http%3A%2F%2Fmusic.qq.com%2Fminiframe%2Fclient%2Fdownload.html%3Fhkey%3D%7B64F84CF9-A0E1-47AA-A6B1-67A4428A76F3%7D%26nDownloadTo%3D2; qqmusic_pageurl=http%3A%2F%2Fmusic.qq.com%2Fminiframe%2Fclient%2Fdownload.html%3Fhkey%3D%7B64F84CF9-A0E1-47AA-A6B1-67A4428A76F3%7D%26nDownloadTo%3D2; qmipflag=254430994_1";

        public static DownLoadInfo GetDownLoadInfo(string qqno)
        {
            var dinfo = new DownLoadInfo(qqno);
            //获取信息
            var timestamp = Vevisoft.WebOperate.HttpWebResponseUtility.GetTimeTIcks(DateTime.Now);
            var url = string.Format(URL_DownLoadInfo, qqno, timestamp);
            string resulut = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(url, cookies);
            //
            GetDownLoadInfo(resulut, dinfo);
            return dinfo;
        }
        /// <summary>
        /// 通过返回Json字符串获取信息
        /// </summary>
        /// <param name="jsonStr"></param>
        /// <param name="info"></param>
        private static void GetDownLoadInfo(string jsonStr,DownLoadInfo info)
        {
            if (!jsonStr.Contains("成功接收"))
            {
                info.IsSuccess=false;
                return;
            }
            var downNo = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(jsonStr, "dl");
            var remainNo = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(jsonStr, "remain");
            var level = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(jsonStr, "level");
            var ret = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(jsonStr, "ret");
            //remain

            if (string.IsNullOrEmpty(downNo) || string.IsNullOrEmpty(remainNo))
                throw new Exception("获取QQ音乐下载信息出错。空字符串无法转化为数字！");
            info.Dl = int.Parse(downNo);
            info.Remain= int.Parse(remainNo);
            info.Ret = int.Parse(ret);
            info.Level = int.Parse(level);
        }
    }
    /// <summary>
    /// qq音乐下载信息获取
    /// </summary>
    public class DownLoadInfo
    {
        
        public DownLoadInfo(string qqno)
        {
            QQNo = qqno;
            IsSuccess = false;
        }
        /// <summary>
        /// QQ号码
        /// </summary>
        public string QQNo { get; set; }
        /// <summary>
        /// 是否获取成功
        /// </summary>
        public bool IsSuccess { get; set; }


        public int Ret { get; set; }
        /// <summary>
        /// 0当天，3周，4月
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// 下载数
        /// </summary>
        public int Dl { get; set; }
        /// <summary>
        /// 剩余数
        /// </summary>
        public int Remain { get; set; }

        public override string ToString()
        {
            return string.Format("{0}::Ret:{1};Level{2};DL:{3};Remain:{4}\r\n", QQNo, Ret, Level, Dl, Remain);
        }
    }
}
