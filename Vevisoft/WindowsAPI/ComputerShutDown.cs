using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Vevisoft.WindowsAPI
{
    public class ComputerShutDown
    {
        // Fields
        internal const int EWX_FORCE = 4;
        internal const int EWX_FORCEIFHUNG = 0x10;
        internal const int EWX_LOGOFF = 0;
        internal const int EWX_POWEROFF = 8;
        internal const int EWX_REBOOT = 2;
        internal const int EWX_SHUTDOWN = 1;
        internal const int SE_PRIVILEGE_ENABLED = 2;
        internal const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";
        internal const int TOKEN_ADJUST_PRIVILEGES = 0x20;
        internal const int TOKEN_QUERY = 8;

        // Methods
        [DllImport("advapi32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);
        private static void DoExitWin(int DoFlag)
        {
            TokPriv1Luid luid;
            IntPtr currentProcess = GetCurrentProcess();
            IntPtr zero = IntPtr.Zero;
            OpenProcessToken(currentProcess, 40, ref zero);
            luid.Count = 1;
            luid.Luid = 0L;
            luid.Attr = 2;
            LookupPrivilegeValue(null, "SeShutdownPrivilege", ref luid.Luid);
            AdjustTokenPrivileges(zero, false, ref luid, 0, IntPtr.Zero, IntPtr.Zero);
            ExitWindowsEx(DoFlag, 0);
        }

        [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool ExitWindowsEx(int DoFlag, int rea);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        internal static extern IntPtr GetCurrentProcess();
        public static void LogOff()
        {
            DoExitWin(4);
        }

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);
        [DllImport("advapi32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);
        public static void PowerOff()
        {
            DoExitWin(12);
        }

        public static void Reboot()
        {
            DoExitWin(6);
        }

        // Nested Types
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct TokPriv1Luid
        {
            public int Count;
            public long Luid;
            public int Attr;
        }
    }


}
