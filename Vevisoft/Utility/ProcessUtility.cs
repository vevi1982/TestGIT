using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Vevisoft.Utility
{
    public class ProcessUtility
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="startPath"></param>
        public static void KillProcessExists(string name, string startPath)
        {
            Process[] processes = Process.GetProcessesByName(name);
            foreach (Process p in processes)
            {
                if (!string.IsNullOrEmpty(startPath))
                    if (startPath != p.MainModule.FileName)
                        continue;
                p.Kill();
                p.Close();
            }
            //刷新任务栏 托盘区域
            
        }
    }
}
