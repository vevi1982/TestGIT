﻿using System;
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
            Application.Run(Args.Length == 0 ? new FrmMainNew() : new FrmMainNew(true));
        }
    }
}
