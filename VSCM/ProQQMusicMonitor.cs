using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace VSCM
{
    public class ProQQMusicMonitor
    {
        private System.Timers.Timer timer;

        public string QQMusicPath { get; set; }
        public string MyAPPPath { get; set; }
        public string MyAppName { get; set; }
        public string MyAPPArguments { get; set; }

        //
        public int StartHour { get; set; }
        public int EndHour { get; set; }
        //控制变量
        private bool qmNotResponse = false;
        private bool apNotResponse = false;
        //
        public void Start(int interval)
        {
            timer = new Timer { Interval = interval*1000 };
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            //如果我的APP不存在，那么打开它
            if (!Vevisoft.Utility.ProcessUtility.ProcessExist(MyAppName, MyAPPPath))
                Vevisoft.Utility.ProcessUtility.OpenProjectByProcess(MyAPPPath, MyAPPArguments);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Vevisoft.Log.VeviLog2.WriteLogInfo("00000000");
            if (DateTime.Now.Hour >= EndHour || DateTime.Now.Hour < StartHour)
                return;
            //1.如果我的APP不存在，那么打开它
            if (!Vevisoft.Utility.ProcessUtility.ProcessExist(MyAppName, MyAPPPath))
            {
                Vevisoft.Utility.ProcessUtility.OpenProjectByProcess(MyAPPPath, MyAPPArguments);
                InitParam();
                return;
            }
            var bol1 = !Vevisoft.Utility.ProcessUtility.GetResponseByProcess("QQMusic", QQMusicPath);

            var msg = "QQMusic " + (bol1 ? "not res" : "can res");
            Vevisoft.Log.VeviLog2.WriteLogInfo(msg);
            var bol2 = !Vevisoft.Utility.ProcessUtility.GetResponseByProcess(MyAPPPath, MyAPPArguments);// IsExeNotResponse();
            msg = MyAPPPath + (bol1 ? "not res" : "can res");
             Vevisoft.Log.VeviLog2.WriteLogInfo(msg);
            //如果是2分钟后获取qq音乐程序还是没反应,或者已关闭，那么说明他没反应了至少2分钟。杀死他等着。。
            if (bol1)
            {
                if (qmNotResponse)
                {
                    Vevisoft.Utility.ProcessUtility.KillProcess("QQMusic", QQMusicPath);
                    qmNotResponse = false;
                    //
                    Vevisoft.Log.VeviLog2.WriteLogInfo("11111");
                    //
                    return;
                }
            }
            qmNotResponse = bol1;
            msg = "QQMusic " + (qmNotResponse ? "not res" : "can res") + "1111";
            Vevisoft.Log.VeviLog2.WriteLogInfo(msg);
            //2分钟后 主程序没反应。停止主程序
            if (bol2)
            {
                Vevisoft.Log.VeviLog2.WriteLogInfo("22222");
                if(apNotResponse ){
                Vevisoft.Utility.ProcessUtility.KillProcess(MyAppName, MyAPPPath);
                apNotResponse = false;
                Vevisoft.Log.VeviLog2.WriteLogInfo("333333");
                //
                //如果我的APP不存在，那么打开它
                if (!Vevisoft.Utility.ProcessUtility.ProcessExist(MyAppName, MyAPPPath))
                    Vevisoft.Utility.ProcessUtility.OpenProjectByProcess(MyAPPPath, MyAPPArguments);
                }
            }
            apNotResponse = bol2;
            msg = MyAPPPath + (apNotResponse ? "not res" : "can res ") + "2222";
            Vevisoft.Log.VeviLog2.WriteLogInfo(msg);
        }
        private void InitParam()
        {
            qmNotResponse = false;
            apNotResponse = false;
        }

        public void Stop()
        {
            if(timer!=null)
                timer.Stop();
        }
       

        public static bool IsExeNotResponse(string appTitle)
        {
            var hwnd = Vevisoft.WindowsAPI.SystemWindowsAPI.FindMainWindowHandle(appTitle,200,10);
            if (hwnd != IntPtr.Zero)
                return Vevisoft.WindowsAPI.SystemWindowsAPI.IsExeNotResponse(hwnd);
            //
            return false;
        }
    }
}
