using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Vevisoft.DataStructure
{
        /// <summary>
        /// 用于菜单或者右键命令，用于操作datagridview数据显示控件
        /// </summary>
        public interface IGridViewOperate
        {
            /// <summary>
            /// 操作的GridView,需拆箱
            /// </summary>
            object BindingControl { get; set; }
            object DataSource { get; set; }
            #region 操作
            /// <summary>
            /// 导出到Excel
            /// </summary>
            void ExportExcelData();
            /// <summary>
            /// 导入Excel数据
            /// </summary>
            void ImportExcelData();
            /// <summary>
            /// 查找
            /// </summary>
            void FindText();
            /// <summary>
            /// 替换
            /// </summary>
            void ReplaceText();
            /// <summary>
            /// 添加记录
            /// </summary>
            void AddItem();
            /// <summary>
            /// 删除记录
            /// </summary>
            void DeleteItem();
            /// <summary>
            /// 保存数据
            /// </summary>
            void SaveData();
            #endregion
           
        }
}
