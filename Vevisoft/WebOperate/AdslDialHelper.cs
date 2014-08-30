using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Vevisoft.WebOperate
{
    public class AdslDialHelper
    {
        private int Desc;
        private static Mutex mutex = new Mutex();
        private Process dailer = new Process();
        //  Creating the extern function...
        /*
         * 经测试在win7下不准确，网线连接上网，测试为没联网desc=18
         */
        [DllImport("wininet.dll")]
        private static extern bool InternetGetConnectedState(out int Description, int ReservedValue);


        //  Creating a  function that  uses the API function. .. 
        //If out parameter returns 18 then fail, if 81 then success 

        public void IsConnectedToInternet()
        {
            InternetGetConnectedState(out Desc, 0);
        }
        public bool IsConnectedInternet()
        {
            InternetGetConnectedState(out Desc, 0);
            return Desc == 81;
        }
        public void StopDailer(string adslName)
        {
            //Process.Start("close.bat");
            while (Desc == 81)
            {
                lock (dailer)
                {
                    if (!IsAlive("rasdial"))
                    {
                        mutex.WaitOne();
                        dailer.StartInfo.UseShellExecute = false;
                        dailer.StartInfo.FileName = "rasdial.exe";
                        dailer.StartInfo.CreateNoWindow = true;
                        dailer.StartInfo.Arguments = adslName + " /disconnect";
                        dailer.Start();
                        //
                        Thread.Sleep(1000);
                        mutex.ReleaseMutex();
                    }
                    //Dailer.WaitForExit(1000); 
                    //Dailer.Close();
                }
                IsConnectedToInternet();
            }
            dailer.Close();
        }

        public void StartDailer(string adslName, string username, string pass)
        {
            
            //var dailer= Process.Start("connect.bat");
            //Mutex.WaitAny(dailer);
            while (Desc != 81)
            {
                lock (dailer)
                {
                    if (!IsAlive("rasdial"))
                    {
                        mutex.WaitOne();
                        dailer.StartInfo.UseShellExecute = false;
                        dailer.StartInfo.FileName = "rasdial.exe";
                        dailer.StartInfo.CreateNoWindow = true;
                        dailer.StartInfo.Arguments = adslName + " " + username + " " + pass;
                        dailer.Start();
                        mutex.ReleaseMutex();
                    }
                    //Dailer.WaitForExit(1000); 
                    //Dailer.Close(); 
                    //
                    Thread.Sleep(1000);
                }
                IsConnectedToInternet();
            }
            dailer.Close();
        }

        private bool IsAlive(string name)
        {
            Process[] ps = Process.GetProcessesByName(name);
            if (ps.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
