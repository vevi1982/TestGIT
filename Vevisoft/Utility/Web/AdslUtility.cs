using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Shell32;

namespace Vevisoft.Utility.Web
{
    public class AdslUtility
    {
        // Fields
        private string AdslName = "宽带连接";
        private string adslPasswd = "";
        private string adslUserName = "";
        private FolderItemVerb connection;
        private FolderItemVerb disconnect;

        // Methods
        public AdslUtility(string AdslName)
        {
            this.AdslName = AdslName;
        }

        private bool IsConnectInternet()
        {
            PingReply reply = new Ping().Send("www.baidu.com", 100);
            if (reply == null)
            {
                throw new ArgumentNullException("Ping Command is Error!!");
            }
            return (reply.Status == IPStatus.Success);
        }

        public void ReConnect(int interval)
        {
            this.SetNetworkAdapter();
            if (this.disconnect != null)
            {
                this.disconnect.DoIt();
                this.SetNetworkAdapter();
                while (this.connection == null)
                {
                    this.SetNetworkAdapter();
                    Thread.Sleep(500);
                }
                for (int i = 0; i < interval; i++)
                {
                    Thread.Sleep(0x3e8);
                }
                this.connection.DoIt();
            }
            else
            {
                this.connection.DoIt();
                this.SetNetworkAdapter();
            }
            while (!this.IsConnectInternet())
            {
                Thread.Sleep(500);
            }
        }

        private void SetNetworkAdapter()
        {
            string str = "断开(&O)";
            string str2 = "连接(&O)";
            string str3 = "网络连接";
            string adslName = this.AdslName;
            Folder folder = ((Shell)Activator.CreateInstance(Type.GetTypeFromCLSID(new Guid("13709620-C279-11CE-A49E-444553540000")))).NameSpace(ShellSpecialFolderConstants.ssfCONTROLS);
            try
            {
                int count = folder.Items().Count;
                foreach (object obj2 in folder.Items())
                {
                    obj2.ToString();
                }
                foreach (FolderItem item in folder.Items())
                {
                    if (item.Name == str3)
                    {
                        foreach (FolderItem item2 in ((Folder)item.GetFolder).Items())
                        {
                            if (item2.Name.IndexOf(adslName) > -1)
                            {
                                foreach (FolderItemVerb verb in item2.Verbs())
                                {
                                    if (verb.Name == str)
                                    {
                                        this.disconnect = verb;
                                        return;
                                    }
                                    if (verb.Name == str2)
                                    {
                                        this.connection = verb;
                                        return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
            }
        }
    }

 

}
