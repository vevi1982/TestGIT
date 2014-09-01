using System;
using System.Collections.Generic;
using System.Configuration;
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
        //
        public static void ReadValue()
        {
            PCName = ConfigurationManager.AppSettings["PCName"];
            AppPath = ConfigurationManager.AppSettings["AppPath"];
            DownLoadPath = ConfigurationManager.AppSettings["DownLoadPath"];
            ChangeIP = ConfigurationManager.AppSettings["ChangeIP"] != "0";

            ADSLName = ConfigurationManager.AppSettings["ADSLName"];
            ADSLUserName = ConfigurationManager.AppSettings["ADSLUserName"];
            ADSLPass = ConfigurationManager.AppSettings["ADSLPass"];
            
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
