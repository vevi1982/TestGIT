using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Vevisoft.WebOperate
{
    public class ADSLHelper
    {
        // Methods
        public static bool ConnectInter(int countTime, int stepTime)
        {
            int num = 0;
            while (num < countTime)
            {
                if (IsConnectedToInternet())
                {
                    return true;
                }
                num += stepTime;
                Thread.Sleep((int)(stepTime * 0x3e8));
            }
            return false;
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        private static extern bool InternetGetConnectedState(ref ConnectionState lpdwFlags, int dwReserved);
        public static bool IsConnectedToInternet()
        {
            ConnectionState lpdwFlags = (ConnectionState)0;
            return InternetGetConnectedState(ref lpdwFlags, 0);
        }

        public static bool LinkAdsl(string adslName)
        {
            RASDisplay display = new RASDisplay();
            try
            {
                if (display.IsConnected)
                {
                    display.Disconnect();
                }
                display.Connect(adslName);
            }
            catch (Exception)
            {
            }
            return display.IsConnected;
        }

        public static bool OnlyLinkAdsl(string adslName)
        {
            RASDisplay display = new RASDisplay();
            if (display.IsConnected)
            {
                return true;
            }
            display.Connect(adslName);
            return display.IsConnected;
        }

        // Nested Types
        private enum ConnectionState
        {
            INTERNET_CONNECTION_CONFIGURED = 0x40,
            INTERNET_CONNECTION_LAN = 2,
            INTERNET_CONNECTION_MODEM = 1,
            INTERNET_CONNECTION_OFFLINE = 0x20,
            INTERNET_CONNECTION_PROXY = 4,
            INTERNET_RAS_INSTALLED = 0x10
        }
    }

    public enum DEL_CACHE_TYPE
    {
        File,
        Cookie
    }


    public class RAS
    {
        // Methods
        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        public static extern int InternetDial(IntPtr hwnd, [In] string lpszConnectoid, uint dwFlags, ref int lpdwConnection, uint dwReserved);
        [DllImport("Rasapi32.dll", EntryPoint = "RasEnumConnectionsA", SetLastError = true)]
        internal static extern int RasEnumConnections(ref RASCONN lprasconn, ref int lpcb, ref int lpcConnections);
        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern uint RasEnumEntries(string reserved, string lpszPhonebook, [In, Out] RasEntryName[] lprasentryname, ref int lpcb, out int lpcEntries);
        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        internal static extern uint RasGetConnectionStatistics(IntPtr hRasConn, [In, Out] RasStats lpStatistics);
        [DllImport("rasapi32.dll", CharSet = CharSet.Auto)]
        public static extern uint RasHangUp(IntPtr hrasconn);
    }


    [StructLayout(LayoutKind.Sequential)]
    public struct RASCONN
    {
        public int dwSize;
        public IntPtr hrasconn;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x101)]
        public string szEntryName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x11)]
        public string szDeviceType;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x81)]
        public string szDeviceName;
    }




    public class RASDisplay
    {
        // Fields
        private bool m_connected = true;
        private IntPtr m_ConnectedRasHandle;
        private string m_ConnectionName;
        private string[] m_ConnectionNames;
        private string m_duration;
        private double m_RX;
        private double m_TX;
        private RasStats status = new RasStats();

        // Methods
        public RASDisplay()
        {
            new RAS();
            RASCONN lprasconn = new RASCONN
            {
                dwSize = Marshal.SizeOf(typeof(RASCONN)),
                hrasconn = IntPtr.Zero
            };
            int lpcb = 0;
            int lpcConnections = 0;
            lpcb = Marshal.SizeOf(typeof(RASCONN));
            if (RAS.RasEnumConnections(ref lprasconn, ref lpcb, ref lpcConnections) != 0)
            {
                this.m_connected = false;
            }
            else
            {
                if (lpcConnections > 0)
                {
                    RasStats lpStatistics = new RasStats();
                    this.m_ConnectedRasHandle = lprasconn.hrasconn;
                    RAS.RasGetConnectionStatistics(lprasconn.hrasconn, lpStatistics);
                    this.m_ConnectionName = lprasconn.szEntryName;
                    int num4 = 0;
                    int num5 = 0;
                    int num6 = 0;
                    num4 = (lpStatistics.dwConnectionDuration / 0x3e8) / 0xe10;
                    num5 = ((lpStatistics.dwConnectionDuration / 0x3e8) / 60) - (num4 * 60);
                    num6 = ((lpStatistics.dwConnectionDuration / 0x3e8) - (num5 * 60)) - (num4 * 0xe10);
                    this.m_duration = string.Concat(new object[] { num4, " hours ", num5, " minutes ", num6, " secs" });
                    this.m_TX = lpStatistics.dwBytesXmited;
                    this.m_RX = lpStatistics.dwBytesRcved;
                }
                else
                {
                    this.m_connected = false;
                }
                int lpcEntries = 1;
                int num8 = 0;
                int num9 = 0;
                RasEntryName[] lprasentryname = null;
                num8 = Marshal.SizeOf(typeof(RasEntryName));
                num9 = lpcEntries * num8;
                lprasentryname = new RasEntryName[lpcEntries];
                lprasentryname[0].dwSize = num8;
                RAS.RasEnumEntries(null, null, lprasentryname, ref num9, out lpcEntries);
                if (lpcEntries > 1)
                {
                    lprasentryname = new RasEntryName[lpcEntries];
                    for (int i = 0; i < lprasentryname.Length; i++)
                    {
                        lprasentryname[i].dwSize = num8;
                    }
                    RAS.RasEnumEntries(null, null, lprasentryname, ref num9, out lpcEntries);
                }
                this.m_ConnectionNames = new string[lprasentryname.Length];
                if (lpcEntries > 0)
                {
                    for (int j = 0; j < lprasentryname.Length; j++)
                    {
                        this.m_ConnectionNames[j] = lprasentryname[j].szEntryName;
                    }
                }
            }
        }

        public int Connect(string Connection)
        {
            int lpdwConnection = 0;
            uint dwFlags = 2;
            return RAS.InternetDial(IntPtr.Zero, Connection, dwFlags, ref lpdwConnection, 0);
        }

        [DllImport("wininet.dll", CharSet = CharSet.Auto)]
        public static extern bool DeleteUrlCacheEntry(DEL_CACHE_TYPE type);
        public void Disconnect()
        {
            RAS.RasHangUp(this.m_ConnectedRasHandle);
        }

        // Properties
        public double BytesReceived
        {
            get
            {
                if (!this.m_connected)
                {
                    return 0.0;
                }
                return this.m_RX;
            }
        }

        public double BytesTransmitted
        {
            get
            {
                if (!this.m_connected)
                {
                    return 0.0;
                }
                return this.m_TX;
            }
        }

        public string ConnectionName
        {
            get
            {
                if (!this.m_connected)
                {
                    return "";
                }
                return this.m_ConnectionName;
            }
        }

        public string[] Connections
        {
            get
            {
                return this.m_ConnectionNames;
            }
        }

        public string Duration
        {
            get
            {
                if (!this.m_connected)
                {
                    return "";
                }
                return this.m_duration;
            }
        }

        public bool IsConnected
        {
            get
            {
                return this.m_connected;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RasEntryName
    {
        public int dwSize;
        public string szEntryName;
    }




    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct RasStats
    {
        public int dwSize;
        public int dwBytesXmited;
        public int dwBytesRcved;
        public int dwFramesXmited;
        public int dwFramesRcved;
        public int dwCrcErr;
        public int dwTimeoutErr;
        public int dwAlignmentErr;
        public int dwHardwareOverrunErr;
        public int dwFramingErr;
        public int dwBufferOverrunErr;
        public int dwCompressionRatioIn;
        public int dwCompressionRatioOut;
        public int dwBps;
        public int dwConnectionDuration;
    }
}
