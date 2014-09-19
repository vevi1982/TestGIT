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
        /// 主程序是否无响应,线程方法
        /// </summary>
        /// <returns></returns>
        public static bool GetResponseByProcess(string processName, string appPath)
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.ToLower().Contains(processName.ToLower())
                    && p.MainModule.FileName.ToLower() == appPath.ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    return p.Responding;
                }
            }
            return false;
        }

        /// <summary>
        /// 杀死线程
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        public static bool KillProcess(string processName, string appPath)
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.ToLower().Contains(processName.ToLower()) && p.MainModule.FileName.ToLower() == appPath.ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    p.Kill();
                    p.Close();
                }
            }
            //托盘区清理


            return false;
        }

        /// <summary>
        /// 线程是否存在
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="appPath"></param>
        /// <returns></returns>
        public static bool ProcessExist(string processName, string appPath)
        {
            foreach (var p in Process.GetProcesses())
            {
                if (p.ProcessName.ToLower().Contains(processName.ToLower()) && p.MainModule.FileName.ToLower() == appPath.ToLower())
                {
                    Console.WriteLine(p.StartInfo.FileName);
                    return true;
                }
            }
            return false;
        }


        public static bool OpenProjectByProcess(string appPath, string arguments)
        {
            Process p = null;
            p = !string.IsNullOrEmpty(arguments) ? Process.Start(appPath, arguments) : Process.Start(appPath);
            return p != null;
        }

      
    }
}
