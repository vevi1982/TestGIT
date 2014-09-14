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
        /// <param name="pcName"></param>
        /// <returns></returns>
        bool SendHeart(string pcName);

        /// <summary>
        /// 上传QQ信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ordername"></param>
        /// <returns></returns>
        bool UpdateDownLoadResult(Models.QQInfo model);
        /// <summary>
        /// 发送密码错误QQ
        /// </summary>
        /// <param name="qqNo"></param>
        /// <returns></returns>
        bool UpdatePassWrongQQ(string qqNo);

        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        string GetIP();
        /// <summary>
        /// IP是否重复
        /// </summary>
        /// <returns></returns>
        bool IPIsRepeat(string ip);
    }
}
