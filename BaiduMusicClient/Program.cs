using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BaiduMusicClient
{
     static class Program
    {
        // Methods
        [STAThread]
        private static void Main(string[] Args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppConfig.GetData();
            try
            {
                Application.Run(StartFormCreator(ParseArgsForFormlabel(Args)));
            }
            catch (Exception exception)
            {
                MessageBox.Show("App Is Exit " + exception.Message);
            }
        }

        private static string ParseArgsForFormlabel(string[] args)
        {
            string str = string.Empty;
            if (args.Length > 0)
            {
                str = args[0];
            }
            return str;
        }

        private static Form StartFormCreator(string Label)
        {
            if (Label.ToLower() == "-auto")
            {
                return new Form1(true);
            }
            return new Form1();
        }
    }

 

}
