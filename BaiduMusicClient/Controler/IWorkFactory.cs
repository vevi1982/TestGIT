using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace BaiduMusicClient.Controler
{
    public interface IWorkFactory
    {
        // Methods
        void ChangeIp();
        void ChangeIp(WebProxy proxy);
        WebProxy GetWebProxy();
        void StartWork();
        void StopWork();
    }

 

 

}
