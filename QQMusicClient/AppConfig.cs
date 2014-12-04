using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Text;

namespace QQMusicClient
{
    /// <summary>
    /// 预读设置文件的内容
    /// </summary>
    public class AppConfig
    {
        public static string PCName = "";
        //
        public static string AppPath = "";
        public static string DownLoadPath = "";
        public static string AppCachePath = "";
        //
        public static bool ChangeIP = false;
        //
        public static string ADSLName = "";
        public static string ADSLUserName = "";
        public static string ADSLPass = "";

        public static int ChangeIPInterval = 3;
        //

        #region 操作间隔时间

        public static int TimeMainFormStart = 2;
        public static int TimeContextMenu = 1;
        public static int TimeAlertCHangeUser = 1;
        public static int TimeKeyInterval = 500;
        public static int TimeDownLoadListLoad = 30;
        public static int TimeDownLoadListDel = 10;
        public static int TimeIdCodeLoad = 5;
        public static int SongListCount = 800;
        //
        public static Point MainTryListenButtonPt = new Point(50, 310);
        public static Point MainTryListenPanelFirstSongPt = new Point(170, 240);
        public static Point MainTryListenPanelDownLoadButtonPt = new Point(300, 170);

        #endregion

        public static void ReadValue()
        {
            PCName = ConfigurationManager.AppSettings["PCName"];
            AppPath = ConfigurationManager.AppSettings["AppPath"];
            DownLoadPath = ConfigurationManager.AppSettings["DownLoadPath"];
            AppCachePath = ConfigurationManager.AppSettings["AppCachePath"];
            ChangeIP = ConfigurationManager.AppSettings["ChangeIP"] != "0";

            ADSLName = ConfigurationManager.AppSettings["ADSLName"];
            ADSLUserName = ConfigurationManager.AppSettings["ADSLUserName"];
            ADSLPass = ConfigurationManager.AppSettings["ADSLPass"];
            //时间
            var intValue = GetIntValue("TimeMainFormStart");
            if (intValue != -1)
                TimeMainFormStart = intValue;
            intValue = GetIntValue("TimeContextMenu");
            if (intValue != -1)
                TimeContextMenu = intValue;
            intValue = GetIntValue("TimeAlertCHangeUser");
            if (intValue != -1)
                TimeAlertCHangeUser = intValue;
            intValue = GetIntValue("TimeKeyInterval");
            if (intValue != -1)
                TimeKeyInterval = intValue;
            intValue = GetIntValue("TimeDownLoadListLoad");
            if (intValue != -1)
                TimeDownLoadListLoad = intValue;
            intValue = GetIntValue("TimeDownLoadListDel");
            if (intValue != -1)
                TimeDownLoadListDel = intValue;
            //intValue = GetIntValue("TimeMainFormStart");
            //if (intValue != -1)
            //    TimeMainFormStart = intValue;
            intValue = GetIntValue("TimeIdCodeLoad");
            if (intValue != -1)
                TimeIdCodeLoad = intValue;
            intValue = GetIntValue("SongListCount");
            if (intValue != -1)
                SongListCount = intValue;
            //按钮位置
            var position = ConfigurationManager.AppSettings["MainTryListenButtonPt"];
            var pt = GetPointFromString(position);
            if (pt.X != 0 || pt.Y != 0)
                PositionInfoQQMusic.MainTryListenButtonPt = MainTryListenButtonPt = pt;
            position = ConfigurationManager.AppSettings["MainTryListenPanelFirstSongPt"];
            pt = GetPointFromString(position);
            if (pt.X != 0 || pt.Y != 0)
                PositionInfoQQMusic.MainTryListenPanelFirstSongPt = MainTryListenPanelFirstSongPt = pt;
            position = ConfigurationManager.AppSettings["MainTryListenPanelDownLoadButtonPt"];
            pt = GetPointFromString(position);
            if (pt.X != 0 || pt.Y != 0)
                PositionInfoQQMusic.MainTryListenPanelDownLoadButtonPt = MainTryListenPanelDownLoadButtonPt = pt;
            //

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

        public static void SaveValue()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var app = config.AppSettings;
            app.Settings["PCName"].Value = PCName;
            app.Settings["AppPath"].Value = AppPath;
            app.Settings["DownLoadPath"].Value = DownLoadPath;
            app.Settings["AppCachePath"].Value = AppCachePath;
            app.Settings["ChangeIP"].Value = ChangeIP ? "1" : "0";
            app.Settings["ADSLName"].Value = ADSLName;
            app.Settings["ADSLUserName"].Value = ADSLUserName;
            app.Settings["ADSLPass"].Value = ADSLPass;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");//重新加载新的配置文件
            ReadValue();
        }
    }
}
