using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraSplashScreen;

namespace Vevisoft.Theme
{
    public class DialogHelper
    {
        public static DialogResult ShowMessageForm(Control dataControl, string title)
        {
            return new FrmMessage(dataControl, title).ShowDialog();
        }

        #region 等待窗体
        public static void ShowWattingForm(Form frm)
        {
            SplashScreenManager.ShowForm(frm, typeof(WaitForm1), true, true, false);
        }

        public static void CloaseWattingForm()
        {
            //Close Wait Form
            SplashScreenManager.CloseForm(false);
        }
        #endregion
        
    }
}
