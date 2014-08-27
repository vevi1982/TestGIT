using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace BaiduMusicClient
{
    public class AppConfig
    {
        // Fields
        public static int AdslIntervalTime = 3;
        public static string AdslName = "adsl";
        public static string AppType = "0";
        public static int EndHour = 0;
        public static string PcName = "CP96";
        public static int PlayTimes = 5;
        public static string ServerType = "0";
        public static string Songs = "";
        public static int StartHour = 0;
        public static bool TimeDecrease = true;

        // Methods
        public static void GetData()
        {
            AdslName = ConfigurationManager.AppSettings["AdslName"];
            PcName = ConfigurationManager.AppSettings["PcName"];
            TimeDecrease = ConfigurationManager.AppSettings["TimeDecrease"] != "0";
            int? intValue = GetIntValue("PlayTimes");
            PlayTimes = !intValue.HasValue ? PlayTimes : intValue.Value;
            AppType = ConfigurationManager.AppSettings["AppType"];
            ServerType = ConfigurationManager.AppSettings["ServerType"];
            intValue = GetIntValue("StartHour");
            StartHour = !intValue.HasValue ? StartHour : intValue.Value;
            intValue = GetIntValue("EndHour");
            EndHour = !intValue.HasValue ? EndHour : intValue.Value;
            Songs = ConfigurationManager.AppSettings["Songs"];
            AdslIntervalTime = Convert.ToInt32(ConfigurationManager.AppSettings["AdslIntervalTime"]);
        }

        private static int? GetIntValue(string name)
        {
            string s = ConfigurationManager.AppSettings[name];
            try
            {
                return new int?(int.Parse(s));
            }
            catch
            {
                return null;
            }
        }
    }


}
