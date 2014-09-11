using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Dlls
{
    public interface  IHeart
    {
        /// <summary>
        /// 判断QQ号，歌单名称，下载数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool IsChangedContent(Models.QQInfo model);
    }
}
