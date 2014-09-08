using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Dlls
{
    public class ServerToInternet:IServer
    {
        string getQQUrl = "http://i.singmusic.cn:8180/portals/getOneAccount";
        private string heartUrl = "http://i.singmusic.cn:8180/portals/heartbeat?cpName=";

        public Models.QQInfo GetQQFromServer()
        {
            try
            {
                string value = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(getQQUrl, "");
                //eg:{"dayCounter":0,"monthCounter":0,"weekCounter":0,"isUsing":1,"qqNo":"1519580187","qqPassword":"nuosjiao","isEnable":1}
                string qqno = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(value, "qqNo");
                string qqPassword = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(value, "qqPassword");
                //
                var model = new Models.QQInfo() {QQNo = qqno, QQPass = qqPassword};
                return model;
            }
            catch (Exception)
            {
                return null;
            }
        }
        /// <summary>
        /// 发送心条
        /// </summary>
        /// <param name="PcName"></param>
        /// <returns></returns>
        public bool SendHeart(string PcName)
        {
            var url = heartUrl + PcName;
            try
            {
                Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(getQQUrl, "");
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public bool UpdateDownLoadResult(Models.QQInfo model)
        {
            return true;
        }

        public bool UpdatePassWrongQQ(string qqNo)
        {
            return true;
        }
    }
}
