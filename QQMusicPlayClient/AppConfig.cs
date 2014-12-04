using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：QQMusicPlayClient
 *文件名：  AppConfig
 *版本号：  V1.0.0.0
 *唯一标识：c6d3abf1-2d6a-4d47-917a-59074cd48d6d
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/3 22:42:16
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/3 22:42:16
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace QQMusicPlayClient
{
    /// <summary>
    /// 预读设置文件的内容
    /// </summary>
    public class AppConfig
    {
        public static string PCName = "";
        //QQ音乐exe地址
        public static string AppPath = "";
        public static int ChangeIPInterval = 3;
        //
        public static bool ChangeIP = false;
        //ADSL 相关参数
        public static string ADSLName = "";
        public static string ADSLUserName = "";
        public static string ADSLPass = "";
        //播放设置
        public static int BeforeClk = 1;
        public static int AfterClk = 3;
        public static DateTime StartTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,9,0,0);
        public static int PlayTime = 5*60 + 15;
        public static int PlaySongNo = 60;
        //
        public static string QQNO = "";
        public static string QQPass = "";
        public static Point CLickPt=new Point(0,0);

        public static void ReadValue()
        {
            
            PCName = ConfigurationManager.AppSettings["PCName"];
            AppPath = ConfigurationManager.AppSettings["AppPath"];
            //
            ChangeIP = ConfigurationManager.AppSettings["ChangeIP"] != "0";
            ADSLName = ConfigurationManager.AppSettings["ADSLName"];
            ADSLUserName = ConfigurationManager.AppSettings["ADSLUserName"];
            ADSLPass = ConfigurationManager.AppSettings["ADSLPass"];
            //时间
            var intValue = GetIntValue("ChangeIPInterval");
            if (intValue != -1)
                ChangeIPInterval = intValue;
            intValue = GetIntValue("BeforeClk");
            if (intValue != -1)
                BeforeClk = intValue;
            intValue = GetIntValue("AfterClk");
            if (intValue != -1)
                AfterClk = intValue;
            intValue = GetIntValue("PlaySongNo");
            if (intValue != -1)
                PlaySongNo = intValue;
            //开始时间，播放时间
            var strValue = ConfigurationManager.AppSettings["AllStartTime"];
            StartTime = GetDateTimeFromString(strValue);
            strValue = ConfigurationManager.AppSettings["PlayTime"];
            PlayTime = GetSecondFromTimeString(strValue);
            //QQ账户
            QQNO = ConfigurationManager.AppSettings["QQNO"];
            QQPass = ConfigurationManager.AppSettings["QQPass"];

            //按钮位置
            var position = ConfigurationManager.AppSettings["QQMusicClientPt"];
            var pt = GetPointFromString(position);
            //
            CLickPt = pt;
        }

        private static Point GetPointFromString(string str)
        {
            var values = str.Split(',');
            if (values.Length != 2)
                return new Point();
            try
            {
                var x = Convert.ToInt32(values[0]);
                var y = Convert.ToInt32(values[1]);
                return new Point(x, y);
            }
            catch (Exception)
            {
                return new Point();
            }
        }

        private static int GetIntValue(string configName)
        {
            var value = ConfigurationManager.AppSettings[configName];
            try
            {
                return Convert.ToInt32(value);
            }
            catch (Exception)
            {
                return -1;
            }
        }

        public static int GetSecondFromTimeString(string value)
        {
            var array = value.Split(':');
            if (array.Length == 2)
            {
                int minute = Convert.ToInt32(array[0]);
                int sec = Convert.ToInt32(array[1]);
                return minute*60 + sec;
            }
            return Convert.ToInt32(value);
        }
        public static string ConvertTimeToString(int seconds)
        {
            var minute = (int) (seconds/60);
            var second = seconds - minute*60;
            return minute + ":" + second;
        }
        public static DateTime GetDateTimeFromString(string value)
        {
            var array = value.Split(':');
            if (array.Length > 1)
            {
                int hour = Convert.ToInt32(array[0]);
                int minu = Convert.ToInt32(array[1]);
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minu, 0);
            }
            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 9, 0, 0);
        }
        public static string ConvertDateToString(DateTime dtime)
        {
            return dtime.Hour + ":" + dtime.Minute;
        }
        public static void SaveValue()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var app = config.AppSettings;
            app.Settings["PCName"].Value = PCName;
            app.Settings["AppPath"].Value = AppPath;
            app.Settings["ChangeIP"].Value = ChangeIP ? "1" : "0";
            app.Settings["ADSLName"].Value = ADSLName;
            app.Settings["ADSLUserName"].Value = ADSLUserName;
            app.Settings["ADSLPass"].Value = ADSLPass;
            //
            app.Settings["BeforeClk"].Value = BeforeClk + "";
            app.Settings["AfterClk"].Value = AfterClk + "";
            app.Settings["PlaySongNo"].Value = PlaySongNo + "";

            app.Settings["AllStartTime"].Value = ConvertDateToString(StartTime);
            app.Settings["PlayTime"].Value = ConvertTimeToString(PlayTime);
            //
            app.Settings["QQNO"].Value = QQNO;
            app.Settings["QQPass"].Value = QQPass;
            app.Settings["QQMusicClientPt"].Value = CLickPt.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件
            ReadValue();
        }
    }
}
