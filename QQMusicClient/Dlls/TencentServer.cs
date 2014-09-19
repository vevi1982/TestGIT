using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Dlls
{
   public  class TencentServer
   {
       private static string URL_DownLoadInfo =
           "http://s.plcloud.music.qq.com/fcgi-bin/fcg_vip_down_lmt.fcg?uin={0}&outCharset=utf-8&rnd={1}&g_tk=1276264180&loginUin={0}&hostUin=0&format=jsonp&inCharset=GB2312&notice=0&platform=miniframe&jsonpCallback=MusicJsonCallback&needNewCode=0";
       /// <summary>
       /// 
       /// </summary>
       /// <param name="qqmodel"></param>
       /// <param name="cookies"></param>
       /// <returns>是否成功!</returns>
       public static int GetDownLoadInfoFromTencentServer(Models.QQInfo qqmodel,string cookies)
       {
           if (qqmodel == null)
               return -1;
           qqmodel.DownLoadNum = 0;
           qqmodel.RemainNum = 0;
           //
           cookies =
               "PATH=/; ts_uid=1616588603; ts_refer=qqmusic.qq.com/fcgi-bin/qm_gopage.fcg; ts_last=music.qq.com/miniframe/static/profile/profile.html; qm_method=1; qm_hideuin=0; qqmusic_gtime=0; qqmusic_gkey=357D7DF1C62D0CC88CCF9957550F340C9EC5CD01C461D6AA; qqmusic_guid=B00E60A22A3CBD6EBF82F03129A4184A; qqmusic_version=11; qqmusic_miniversion=22; qqmusic_fromtag=3; ac=1,013,008; pgv_pvid=8660477544; qqmusic_privatekey=70E82AF208036D891DA3052F721EDEB261E6C13F7A8CEB40; qqmusic_key=33128A88574C5F6F2FE86BB211792ED21FE28A70703FF51223ED031516C98BBA; qqmusic_uin=254430994; qq_version=http%3A%2F%2Fmusic.qq.com%2Fminiframe%2Fclient%2Fdownload.html%3Fhkey%3D%7B64F84CF9-A0E1-47AA-A6B1-67A4428A76F3%7D%26nDownloadTo%3D2; qqmusic_pageurl=http%3A%2F%2Fmusic.qq.com%2Fminiframe%2Fclient%2Fdownload.html%3Fhkey%3D%7B64F84CF9-A0E1-47AA-A6B1-67A4428A76F3%7D%26nDownloadTo%3D2; qmipflag=254430994_1";

           //
           var timestamp = Vevisoft.WebOperate.HttpWebResponseUtility.GetTimeTIcks(DateTime.Now);
           var url = string.Format(URL_DownLoadInfo, qqmodel.QQNo, timestamp);
           string resulut= Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(url, cookies);
           //
           if (!resulut.Contains("成功接收"))
               return - 1;
           var downNo = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(resulut, "dl");
           var remainNo = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(resulut, "remain");
           //remain
           
           if (string.IsNullOrEmpty(downNo) || string.IsNullOrEmpty(remainNo))
               return -1;
           qqmodel.DownLoadNum = int.Parse(downNo);
           qqmodel.RemainNum = int.Parse(remainNo);
           return int.Parse(downNo);
       }

       public static string GetDownLoadInfoStrFromTencentServer(Models.QQInfo qqmodel, string cookies)
       {
           if (qqmodel == null)
               return "";
           qqmodel.DownLoadNum = 0;
           qqmodel.RemainNum = 0;
           //
           cookies =
               "PATH=/; ts_uid=1616588603; ts_refer=qqmusic.qq.com/fcgi-bin/qm_gopage.fcg; ts_last=music.qq.com/miniframe/static/profile/profile.html; qm_method=1; qm_hideuin=0; qqmusic_gtime=0; qqmusic_gkey=357D7DF1C62D0CC88CCF9957550F340C9EC5CD01C461D6AA; qqmusic_guid=B00E60A22A3CBD6EBF82F03129A4184A; qqmusic_version=11; qqmusic_miniversion=22; qqmusic_fromtag=3; ac=1,013,008; pgv_pvid=8660477544; qqmusic_privatekey=70E82AF208036D891DA3052F721EDEB261E6C13F7A8CEB40; qqmusic_key=33128A88574C5F6F2FE86BB211792ED21FE28A70703FF51223ED031516C98BBA; qqmusic_uin=254430994; qq_version=http%3A%2F%2Fmusic.qq.com%2Fminiframe%2Fclient%2Fdownload.html%3Fhkey%3D%7B64F84CF9-A0E1-47AA-A6B1-67A4428A76F3%7D%26nDownloadTo%3D2; qqmusic_pageurl=http%3A%2F%2Fmusic.qq.com%2Fminiframe%2Fclient%2Fdownload.html%3Fhkey%3D%7B64F84CF9-A0E1-47AA-A6B1-67A4428A76F3%7D%26nDownloadTo%3D2; qmipflag=254430994_1";

           //
           var timestamp = Vevisoft.WebOperate.HttpWebResponseUtility.GetTimeTIcks(DateTime.Now);
           var url = string.Format(URL_DownLoadInfo, qqmodel.QQNo, timestamp);
           string resulut = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(url, cookies);
           //
           return resulut;
           //if (!resulut.Contains("成功接收"))
           //    return -1;
           //var downNo = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(resulut, "dl");
           //var remainNo = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(resulut, "remain");
           ////remain

           //if (string.IsNullOrEmpty(downNo) || string.IsNullOrEmpty(remainNo))
           //    return -1;
           //qqmodel.DownLoadNum = int.Parse(downNo);
           //qqmodel.RemainNum = int.Parse(remainNo);
           //return int.Parse(downNo);
       }
    }
}
