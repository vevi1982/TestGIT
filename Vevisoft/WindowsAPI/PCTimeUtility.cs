using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：Vevisoft.WindowsAPI
 *文件名：  PCTimeUtility
 *版本号：  V1.0.0.0
 *唯一标识：231ea357-e1f6-400b-88ce-0bd624cef757
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/4 0:06:34
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/4 0:06:34
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace Vevisoft.WindowsAPI
{
    [StructLayout(LayoutKind.Sequential)]
    public struct SystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMiliseconds;
    }
    public class PCTimeUtility
    {
        [DllImport("Kernel32.dll")]
        public static extern bool SetLocalTime(ref SystemTime sysTime);

        public static bool SetSysTime(DateTime setTime)
        {
            bool flag = false;
            SystemTime sysTime = new SystemTime();
            DateTime dt = setTime;
            sysTime.wYear = Convert.ToUInt16(dt.Year);
            sysTime.wMonth = Convert.ToUInt16(dt.Month);
            sysTime.wDay = Convert.ToUInt16(dt.Day);
            sysTime.wHour = Convert.ToUInt16(dt.Hour);
            sysTime.wMinute = Convert.ToUInt16(dt.Minute);
            sysTime.wSecond = Convert.ToUInt16(dt.Second);
            try
            {
                flag = SetLocalTime(ref sysTime);
            }
            catch (Exception e)
            {
                Console.WriteLine("SetSystemDateTime函数执行异常" + e.Message);
            }
            return flag;
        }

        public static string SetSysTime_CMD(DateTime setTime)
        {
            //实例一个Process类，启动一个独立进程 
            var p = new Process();
            //Process类有一个StartInfo属性 
            //设定程序名 
            p.StartInfo.FileName = "cmd.exe";
            //设定程式执行参数    “/C”表示执行完命令后马上退出
            p.StartInfo.Arguments = "/c date " + setTime.ToString("yyyy-MM-dd HH:mm:ss");
            //关闭Shell的使用   
            p.StartInfo.UseShellExecute = false;
            //重定向标准输入      
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            //重定向错误输出   
            p.StartInfo.RedirectStandardError = true;
            //设置不显示doc窗口  
            p.StartInfo.CreateNoWindow = true;
            //启动 
            p.Start();

            //从输出流取得命令执行结果 
            return p.StandardOutput.ReadToEnd();
        }


        public static DateTime  GetDateTimeFromInternet()
        {
            //var c = new TcpClient();
            //c.Connect("www.time.ac.cn", 37);   //连接到国内授时服务器
            //NetworkStream s = c.GetStream();          //读取数据流
            //c.Close();
            //var buf = new byte[4];
            //s.Read(buf, 0, 4);                  //把数据存到数组中
            //uint lTime;
            ////把服务器返回数据转换成1900/1/1 UTC 到现在所经过的秒数
            //lTime = ((uint)buf[0] << 24) + ((uint)buf[1] << 16) + ((uint)buf[2] << 8) + (uint)buf[3];

            ////得到真实的本地时间
            //var datetime = new DateTime(1900, 1, 1, 0, 0, 0, 0);
            //datetime = datetime.AddSeconds(lTime).ToLocalTime();
            //return datetime;

            SNTPTimeClient client = new SNTPTimeClient("http://www.time.ac.cn/", "13");
            client.Connect();
            string strTest = client.ToString();
            //
            Char[] split = new Char[1];
            split[0] = Convert.ToChar("\n");
            string[] temp = strTest.Split(split, 100);

            string strDate = "";
            int intPos = -1;
            for (int i = 0; i < temp.Length - 1; i++)
            {
                strDate = temp[i];
                if (strDate.Substring(0, 10) == "Local time")
                {
                    intPos = 1;
                    break;
                }
                else
                    Console.Write(strDate.Substring(0, 10) + "\n");
            }
            if (intPos < 0)
            {
                throw new Exception("Can't get server time!");
            }
            strDate = strDate.Substring(11);
            return Convert.ToDateTime(strDate);
        }
        public static void CorrectSysTime()
        {
           

            //这里可以显示出时间。有兴趣的朋友可以取消注释看一下效果。
            //MessageBox.Show(datetime.ToLongDateString() + datetime.ToLongTimeString());
        }
    }
}
