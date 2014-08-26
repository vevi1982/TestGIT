using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Vevisoft.Log
{
    public class VeviLog2
{
    // Fields
    private static Dictionary<string, List<string>> logNameAndInfoList = new Dictionary<string, List<string>>();

    // Methods
    static VeviLog2()
    {
        ThreadPool.QueueUserWorkItem(state => DoWriteLog());
    }

    public static void DoWriteLog()
    {
        while (true)
        {
            if (logNameAndInfoList == null)
            {
                logNameAndInfoList = new Dictionary<string, List<string>>();
            }
            if (logNameAndInfoList.Count > 0)
            {
                lock (logNameAndInfoList)
                {
                    foreach (string str in logNameAndInfoList.Keys)
                    {
                        if (logNameAndInfoList[str].Count > 0)
                        {
                            WriteLogThread(str, logNameAndInfoList[str]);
                            logNameAndInfoList[str].Clear();
                        }
                    }
                }
            }
            Thread.Sleep(200);
        }
    }

    public static void WriteIPLog(string logText)
    {
        WriteToLog("IPLog", logText);
    }

    public static void WriteLogInfo(string logText)
    {
        WriteToLog("InfoLog", logText);
    }

    public static void WriteLogThread(string logName, List<string> textList)
    {
        string fileName = logName + ".txt";
        var info = new FileInfo(fileName);
        if (File.Exists(fileName) && (info.Length > 0xf4240L))
        {
            File.Copy(fileName, "log" + DateTime.Now.Ticks + ".txt");
            File.Delete(fileName);
        }
        using (var writer = new StreamWriter(fileName, true))
        {
            foreach (string str2 in textList)
            {
                writer.Write(str2 + "====");
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }
    }

    public static void WriteSuccessLog(string logText)
    {
        WriteToLog("SuccessIPlog", logText);
    }

    private static void WriteToLog(string logname, string logtext)
    {
        lock (logNameAndInfoList)
        {
            if (!logNameAndInfoList.ContainsKey(logname))
            {
                logNameAndInfoList.Add(logname, new List<string>());
            }
            logNameAndInfoList[logname].Add(logtext);
        }
    }
}


}
