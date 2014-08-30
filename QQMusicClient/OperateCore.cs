using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Vevisoft.WebOperate;

namespace QQMusicClient
{
    /// <summary>
    /// QQ音乐自动下载操作
    /// </summary>
    public class OperateCore
    {
        public void DoOnce()
        {
            //1.改变IP
            #region CHangeIP
            if (AppConfig.ChangeIP)
            {
                var adsl=new AdslDialHelper();

                if (adsl.IsConnectedInternet())
                {
                    //连接上的时候 先断开
                    adsl.StopDailer(AppConfig.ADSLName);
                    while (adsl.IsConnectedInternet())
                    {
                        Thread.Sleep(500);
                    }                    
                }
                //连接ADSL
                adsl.StartDailer(AppConfig.ADSLName, AppConfig.ADSLUserName, AppConfig.ADSLPass);
                while (!adsl.IsConnectedInternet())
                {
                    Thread.Sleep(500);
                }
                //TODO...是否需要判断IP是否重复

            }
            #endregion
            
            //2.打开软件 判断软件是否显示打开并显示？？
            if(string.IsNullOrEmpty(AppConfig.AppPath))
                throw new Exception("没有设置QQ音乐s软件路径");
            if(!File.Exists(AppConfig.AppPath))
                throw new Exception("软件路径错误，找不到软件！");
            //
            System.Diagnostics.Process.Start(AppConfig.AppPath);
            //

        }


    
    }
}
