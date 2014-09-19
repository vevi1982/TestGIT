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

        //控制变量
        private bool qmNotResponse = false;
        private bool apNotResponse = false;
        //
        public void Start()
        {
            timer=new Timer {Interval = 10*1000};
            timer.Elapsed += timer_Elapsed;
            timer.Start();
            //如果我的APP不存在，那么打开它
            if (!Vevisoft.Utility.ProcessUtility.ProcessExist(MyAppName, MyAPPPath))
                Vevisoft.Utility.ProcessUtility.OpenProjectByProcess(MyAPPPath, MyAPPArguments);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            //如果我的APP不存在，那么打开它
            if (!Vevisoft.Utility.ProcessUtility.ProcessExist(MyAppName, MyAPPPath))
            {
                Vevisoft.Utility.ProcessUtility.OpenProjectByProcess(MyAPPPath, MyAPPArguments);
                return;
            }
            var bol1 = !Vevisoft.Utility.ProcessUtility.GetResponseByProcess("QQMusic", QQMusicPath);
            var bol2 = !Vevisoft.Utility.ProcessUtility.GetResponseByProcess(MyAppName, MyAPPPath);
            //如果是2分钟后获取qq音乐程序还是没反应，那么说明他没反应了至少2分钟。杀死他等着。。
            if (qmNotResponse && bol1)
            {
                Vevisoft.Utility.ProcessUtility.KillProcess("QQMusic", QQMusicPath);
                return;
            }
            qmNotResponse = bol1;
            //2分钟后 主程序没反应。停止主程序
            if (apNotResponse && bol2)
            {
                Vevisoft.Utility.ProcessUtility.KillProcess(MyAppName, QQMusicPath);
            }
            apNotResponse = bol2;
        }


        public void Stop()
        {
            if(timer!=null)
                timer.Stop();
        }
       
    }
}
