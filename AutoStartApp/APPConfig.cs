using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace AutoStartApp
{
    public class APPConfig
    {
        public static string App1Path = "";
        public static string App1Param = "";
        public static string App1ProcessName = "";
        public static string App2Path = "";
        public static string App2ProcessName = "";
        //
        public static int StartHour = 0;
        public static int EndHour = 0;
        public static int TimerInterval = 0;
        public static void ReadFile()
        {
            App1Path = ConfigurationManager.AppSettings["App1Path"];
            App1Param = ConfigurationManager.AppSettings["App1Param"];
            App1ProcessName = ConfigurationManager.AppSettings["App1ProcessName"];
            App2Path = ConfigurationManager.AppSettings["App2Path"];
            App2ProcessName = ConfigurationManager.AppSettings["App2ProcessName"];
            //
            StartHour = GetIntValue("StartHour");
            EndHour = GetIntValue("EndHour");
            TimerInterval = GetIntValue("TimerInterval");
        }


        public static int GetIntValue(string configName)
        {
            try
            {
                var str = ConfigurationManager.AppSettings[configName];
                return int.Parse(str);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }
}
