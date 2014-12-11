using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：QQMusicPlayClient
 *文件名：  WebUtility
 *版本号：  V1.0.0.0
 *唯一标识：140daedd-c364-4792-8955-730241fa7e38
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/4 21:39:09
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/4 21:39:09
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace QQMusicPlayClient
{
    public class WebUtility
    {
        static string IPUrl="http://q.singmusic.cn:8180/portals/isRepeatIP";
        private static string GetTimeUrl = "http://q.singmusic.cn:8180/getTime.jsp";
        public static bool IsIPRepeat()
        {
            string value = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(IPUrl, "");
            return Convert.ToBoolean(value);
        }

        public static DateTime GetWordDateTime()
        {
            var value = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(GetTimeUrl, "");
            //毫秒数？？
            var dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(value.Trim()+"0000");
            var toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow); ;
        }

        public static string GetMd5Encrypt(string value)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5 = MD5.Create();
            byte[] result = md5.ComputeHash(Encoding.UTF8.GetBytes(value));
            var md5value = "";
            foreach (byte b in result)
                md5value += b.ToString("x2");
            return md5value;
        }

        public static void SendHeart()
        {
            try
            {
                var url = "http://q.singmusic.cn:8180/portals/heartbeat?cpName=" + AppConfig.PCName;
                Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(url, "");
            }
            catch (Exception)
            {
            }
        }

        public static bool IsPcExist()
        {
            var url = "http://q.singmusic.cn:8180/portals/login?cpName=" + AppConfig.PCName;
            var result = Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(url, "");
            return Convert.ToBoolean(result);
        }
    }
}
