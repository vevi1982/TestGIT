using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraSplashScreen;
using VeviSoft.Controls.Properties;
using Vevisoft.DataStructure;

namespace VeviSoft.Controls
{
    public partial class GridControlVs : DevExpress.XtraGrid.GridControl
    {
        public GridControlVs()
        {
            InitializeComponent();
            //
            DefaultViewChanged += GridControlVs_DefaultViewChanged;
        }
        
        public GridControlVs(IContainer container)
        {
            container.Add(this);

            InitializeComponent();

        }

        public GridView DefaultGridView
        {
            get
            {
                if (DefaultView == null)
                    return null;
                return DefaultView as DevExpress.XtraGrid.Views.Grid.GridView;
            }
        }
        /// <summary>
        /// 操作类
        /// </summary>
        public IGridViewOperate GridOperate { get; set; }
        /// <summary>
        /// 预定义设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void GridControlVs_DefaultViewChanged(object sender, EventArgs e)
        {
            //
            if (DefaultGridView != null)
            {
                SetGridView(DefaultGridView);
                //ShowRowNumber(DefaultGridView);
                DefaultGridView.GroupPanelText = Resources.Vevisoft;
            }
        }

        private void SetGridView(GridView gridview)
        {
            //禁用GridControl中列头的过滤器
            gridview.OptionsCustomization.AllowFilter = false;
            //显示行号
            ShowRowNumber(gridview);
            //各列头禁止移动
            gridview.OptionsCustomization.AllowColumnMoving = false;
            //各列头禁止排序gridview.OptionsCustomization.AllowSort = false;
            //中单击列标题弹出右键菜单
            gridview.OptionsMenu.EnableColumnMenu = false;
            //奇数偶数行差异
            gridview.OptionsView.EnableAppearanceOddRow = true;
            gridview.OptionsView.EnableAppearanceEvenRow = true;
            //
            gridview.OptionsBehavior.EditorShowMode = EditorShowMode.Click; 

        }

        #region 设置列标题居中
        /// <summary>
        /// 设置列标题居中
        /// </summary>
        /// <param name="column"></param>
        private void SetColumnTextCenter(DevExpress.XtraGrid.Columns.GridColumn column)
        {
            column.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
            column.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
        }
        #endregion
        
        
        #region 显示行号
        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="gridview"></param>
        public void ShowRowNumber(GridView gridview)
        {
            gridview.IndicatorWidth = 40;
            //此代码只有在动态创建的时候起作用
            gridview.CustomDrawRowIndicator += gridview_CustomDrawRowIndicator;
        }

        /// <summary>
        /// 显示行号
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void gridview_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1) + "";
            }
        }

        #endregion

      

        
    }
}
