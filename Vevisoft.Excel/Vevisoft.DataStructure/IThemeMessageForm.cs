using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vevisoft.DataStructure
{
    public interface IThemeMessageForm
    {
        void InitData(IUcokCancel control, string title);
        /// <summary>
        /// 返回信息,需要拆箱
        /// </summary>
        object ReturnObject { get; set; }
    }
}
