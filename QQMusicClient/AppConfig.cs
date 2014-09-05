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

        public static int SongListCount = 800;
        public static int TimeIdCodeLoad = 1;
        //
       public static Point MainTryListenButtonPt=new Point(50,310);
        public static Point MainTryListenPanelFirstSongPt=new Point(170,240);
        public static Point MainTryListenPanelDownLoadButtonPt=new Point(300,170);
        #endregion

        public static void ReadValue()
        {
            PCName = ConfigurationManager.AppSettings["PCName"];
            AppPath = ConfigurationManager.AppSettings["AppPath"];
            DownLoadPath = ConfigurationManager.AppSettings["DownLoadPath"];
            ChangeIP = ConfigurationManager.AppSettings["ChangeIP"] != "0";

            ADSLName = ConfigurationManager.AppSettings["ADSLName"];
            ADSLUserName = ConfigurationManager.AppSettings["ADSLUserName"];
            ADSLPass = ConfigurationManager.AppSettings["ADSLPass"];
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
            var value = 0;

        }

        private static Point GetPointFromString(string str)
        {
            var values = str.Split(',');
            if(values.Length!=2)
                return new Point();
            try
            {
                var x = Convert.ToInt32(values[0]);
                var y = Convert.ToInt32(values[1]);
                return new Point(x,y);
            }
            catch (Exception)
            {
                return new Point();
            }
        }
        public static void SaveValue()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var app = config.AppSettings;
            app.Settings["PCName"].Value = PCName;
            app.Settings["AppPath"].Value = AppPath;
            app.Settings["DownLoadPath"].Value = DownLoadPath;
            
            app.Settings["ChangeIP"].Value = ChangeIP ? "1" : "0";
            app.Settings["ADSLName"].Value = ADSLName;
            app.Settings["ADSLUserName"].Value = ADSLUserName;
            app.Settings["ADSLPass"].Value = ADSLPass;
            config.Save(ConfigurationSaveMode.Modified);
        }
    }
}
