using System;

namespace QQMusicClient.Dlls
{
    public class ServerToInternet:IServer
    {
        private const string GetIPUrl = "http://i.singmusic.cn:8180/getIP.jsp";
        private const string ValidateIPUrl = "http://i.singmusic.cn:8180/portals/isRepeatIP?ip={0}";

        private const string GetQQUrl = "http://i.singmusic.cn:8180/portals/getOneAccount";
        private const string HeartUrl = "http://i.singmusic.cn:8180/portals/heartbeat?cpName={0}";

        private const string ResultUrl =
            "http://i.singmusic.cn:8180/portals/setDownloadCounter?qqNo={0}&cpName={1}&increment={2}&beginTime={3}&endTime={4}&orderName={5}";

        private const string WrongUrl = "http://i.singmusic.cn:8180/portals/setAccountStatus?qqNo={0}&status=0";
        public Models.QQInfo GetQQFromServer()
        {
            //测试代码
            //var tmpmodel = new Models.QQInfo {QQNo = "1062457275", QQPass = "xd1550000"};
            //tmpmodel.SongOrderList.Add("1409200101", 200);
            //tmpmodel.SongOrderList.Add("1409200102", 200);
            //tmpmodel.SongOrderList.Add("1409200103", 200);
            //tmpmodel.SongOrderList.Add("1409200104", 200);
            //return tmpmodel;
            //
            try
            {
                string value = Vevisoft.Utility.Web.HttpResponseUtility.GetJsonStringFromUrlByGet(GetQQUrl, "");
                //eg:{"dayCounter":0,"monthCounter":0,"weekCounter":0,"isUsing":1,"qqNo":"1519580187","qqPassword":"nuosjiao","isEnable":1}
                string qqno = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(value, "qqNo");
                string qqPassword = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(value, "qqPassword");
                //
                var dayCounter = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(value, "dayCounter");
                //分解Order
                var orders = Vevisoft.Utility.Web.HttpResponseUtility.GetSubJsonStr(value, "orderName");
                var model = new Models.QQInfo {QQNo = qqno, QQPass = qqPassword};
                if (!string.IsNullOrEmpty(dayCounter))
                    model.DayCounter = int.Parse(dayCounter);
                foreach (var order in orders)
                {
                    if (order == null)
                        continue;
                    if (string.IsNullOrEmpty(order))
                        continue;
                    if (order.Length < 5)
                        continue;
                    //
                    string orname = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(order, "orderName");
                    if (model.SongOrderList.ContainsKey(orname))
                        continue;
                    int orderCount = 0;
                    var countstr = Vevisoft.Utility.Web.HttpResponseUtility.GetValueFromJson(order, "musicNum");
                    if (!string.IsNullOrEmpty(countstr))
                        orderCount = Convert.ToInt32(countstr);
                    model.SongOrderList.Add(orname, orderCount);
                }
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
        /// <param name="pcName"></param>
        /// <returns></returns>
        public bool SendHeart(string pcName)
        {
            var url = string.Format(HeartUrl, pcName);
            try
            {
                Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(url, "");
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 上传播放数
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool UpdateDownLoadResult(Models.QQInfo model)
        {
            if (model == null)
                return false;
            var updateNo = 0;
            //
            updateNo = model.OriRemain  - model.RemainNum;

            string endstamp = Vevisoft.WebOperate.HttpWebResponseUtility.GetTimeStamp(DateTime.Now);
            string url = string.Format(ResultUrl, model.QQNo, model.PcName, updateNo,
                                       model.BeginTimeStamp, endstamp, model.CurrentSongOrderName);
            try
            {
                Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(url, "");
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// QQ错误，暂时停用此QQ
        /// </summary>
        /// <param name="qqNo"></param>
        /// <returns></returns>
        public bool UpdatePassWrongQQ(string qqNo)
        {
            //TODO....
            var url = string.Format(WrongUrl, qqNo);
            string result;
            try
            {
                result = Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(url, "");
            }
            catch (Exception)
            {
                return false;
            }
            return Convert.ToBoolean(result);
        }


        public string GetIP()
        {
            try
            {
                return Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(GetIPUrl, "");
            }
            catch (Exception)
            {
                return "";
            }
        }

        public bool IPIsRepeat(string ip)
        {
            //return false;
            var url = string.Format(ValidateIPUrl, ip);
            string result;
            try
            {
                result = Vevisoft.Utility.Web.HttpResponseUtility.GetHtmlStringFromUrlByGet(url, "");
            }
            catch (Exception)
            {
                return false;
            }
            return Convert.ToBoolean(result);
        }

    }
}
