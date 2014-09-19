using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace QQMusicClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppConfig.ReadValue();
            if (Args.Length == 0)
                Application.Run(new FrmMain());
            else Application.Run(new FrmMain(true));
        }
    }
}
