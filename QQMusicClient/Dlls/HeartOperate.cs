using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QQMusicClient.Dlls
{
    public class HeartOperate:IHeart
    {
        private int count = 0;
        public Models.QQInfo currentModel;
        
        public HeartOperate(Models.QQInfo model)
        {
            currentModel =model==null?null: model.Clone() as Models.QQInfo;
        }
        //

        /// <summary>
        /// 判断QQ号，歌单名称，下载数量
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsChangedContent(Models.QQInfo model)
        {
            //在半夜11点至 凌晨1点之间 无条件发送心跳。因为程序此时停止
            if (DateTime.Now.Hour >= 23 && DateTime.Now.Hour <= 1)
            {
                return true;
            }
            if (model == null)
            {
                if (currentModel != null)
                {
                    currentModel = null;
                    return true;
                }
                else return false;
            }
            if (currentModel == null)
            {
                currentModel = model == null ? null : model.Clone() as Models.QQInfo;
                return true;
            }
            //
            if (currentModel.QQNo != model.QQNo || currentModel.CurrentSongOrderName != model.CurrentSongOrderName||
                currentModel.CurrentDownloadCount != model.CurrentDownloadCount)
            {
                currentModel = model == null ? null : model.Clone() as Models.QQInfo;
                count++;
                if (count == 3)
                {
                    
                }
                return true;
            }
            
            //
            return false;
        }
    }
}
