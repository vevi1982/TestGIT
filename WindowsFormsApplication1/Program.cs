using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace VersionDebugIncrease
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("工具T035没有设置参数，应该要设为：T035 \"$(ProjectDir)\"");
                Application.Exit();
                return;
            }
            else if (args.Length > 1)
            {
                MessageBox.Show("工具T035有多个参数，是不是你的命令行没带双引号，而项目路径又带空格？");
                Application.Exit();
                return;
            }

            string sPath = args[0].Replace("\"", string.Empty) + "\\Properties\\";
            string sAssemOld = sPath + "AssemblyInfo.old";
            string sAssem = sPath + "AssemblyInfo.cs";
            string sAssemNew = sPath + "AssemblyInfo.new";

            // 检测AssemblyInfo文件是否存在来决定路径是否设置正确
            if (System.IO.File.Exists(sAssem) == false)
            {
                string sInfo =
                    "未检测到文件存在：" + sAssem + "\r\n" +
                    "路径是否有设置错误？";
                MessageBox.Show(sInfo);
                Application.Exit();
                return;
            }

            // TODO: 自行检测文件是否只读等问题
            System.IO.StreamReader oSR = new System.IO.StreamReader(sAssem);
            System.IO.StreamWriter oSW = new System.IO.StreamWriter(
                sAssemNew,
                false,
                oSR.CurrentEncoding);

            string sLine = string.Empty;
            string sNewLine = string.Empty;
            while ((sLine = oSR.ReadLine()) != null)
            {
                if ((sLine.IndexOf("[assembly: AssemblyVersion") == 0) ||
                    (sLine.IndexOf("[assembly: AssemblyFileVersion") == 0))
                {
                    // 找到这两行了，修改它们
                    sNewLine = VerAdd(sLine);
                }
                else
                {
                    sNewLine = sLine;
                }

                oSW.WriteLine(sNewLine);
            }

            oSW.Close();
            oSR.Close();

            // 备份文件删除（只读属性时将会出错）
            if (System.IO.File.Exists(sAssemOld) == true)
            {
                System.IO.File.Delete(sAssemOld);
            }

            // 原文件改名备份（只读属性下允许正常改名）
            System.IO.File.Move(sAssem, sAssemOld);

            // 新文件改为原文件（原只读属性将会丢失）
            System.IO.File.Move(sAssemNew, sAssem);
        }

        /// <summary>
        /// 对输入的字符串, 取其中版本最后部分+1
        /// </summary>
        /// <param name="sLine">输入的字符串，类似：[assembly: AssemblyVersion("1.0.0.4")]</param>
        /// <returns>版本最后部分+1 后的结果</returns>
        private static string VerAdd(string sLine)
        {
            // 定位起始位置与结束位置
            int lPos1 = sLine.IndexOf("(\"");
            if (lPos1 < 0)
            {
                MessageBox.Show("该字符串找不到版本号起始标志(\"：" + sLine);
                Environment.Exit(0);
            }

            int lPos2 = sLine.IndexOf("\")", lPos1);
            if (lPos2 < 0)
            {
                MessageBox.Show("该字符串找不到版本号结束标志\")：" + sLine);
                Environment.Exit(0);
            }

            // TODO: 自行去保证数据正确性，例如：1.0.7 或 1.0.0.7A
            string[] sVer = sLine.Substring(lPos1 + 2, lPos2 - lPos1 - 2).Split('.');
            string sNewLine = sLine.Substring(0, lPos1 + 2) +
                sVer[0] + "." + sVer[1] + "." + sVer[2] + "." + (int.Parse(sVer[3]) + 1).ToString() + "\")]";
            return sNewLine;
        }
    }
}