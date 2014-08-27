using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaiduMusicClient.ServerAction;

namespace BaiduMusicClient.Controler
{
   public class FatoryControler
{
    // Fields
    private static IWorkFactory _currentFactory;

    // Methods
    public static IWorkFactory CreateFactory(string condition)
    {
        if (_currentFactory == null)
        {
            string str;
            ISongServerAction server = null;
            if (((str = AppConfig.ServerType) != null) && (str == "0"))
            {
                server = new LocalServer();
            }
            else
            {
                server = new LocalServer();
            }
            if (AppConfig.AppType == "0")
            {
                FactoryForAdsl adsl = new FactoryForAdsl(server) {
                    AdslName = AppConfig.AdslName,
                    PcName = AppConfig.PcName,
                    PlayInterval = AppConfig.PlayTimes,
                    TimeDecrease = AppConfig.TimeDecrease
                };
                _currentFactory = adsl;
            }
        }
        return _currentFactory;
    }
}

 
 

}
