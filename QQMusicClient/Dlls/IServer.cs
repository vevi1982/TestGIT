using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Dlls
{
    public interface IServer
    {
        /// <summary>
        /// 获取QQ信息
        /// </summary>
        /// <returns></returns>
        Models.QQInfo GetQQFromServer();
        /// <summary>
        /// 发送心跳
        /// </summary>
        /// <param name="PcName"></param>
        /// <returns></returns>
        bool SendHeart(string PcName);
        /// <summary>
        /// 上传QQ信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateDownLoadResult(Models.QQInfo model);
        /// <summary>
        /// 发送密码错误QQ
        /// </summary>
        /// <param name="qqNo"></param>
        /// <returns></returns>
        bool UpdatePassWrongQQ(string qqNo);
    }
}
