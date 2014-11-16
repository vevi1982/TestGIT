using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vevisoft.DataStructure
{
    /// <summary>
    /// 用于
    /// </summary>
    public interface IUcokCancel
    {
        /// <summary>
        /// 返回信息,需要拆箱
        /// </summary>
        object ReturnObject { get; set; }

        void InitData(object condition);

        void btnOK_Click();

        void btnCancel_Click();
    }
}
