using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Vevisoft.WindowsAPI
{
    public class Shell32Utility
    {
        [DllImport("shell32.dll")]
        public static extern Int32 SHGetDesktopFolder(out IntPtr ppshf);

        /// <summary>
        /// 获得桌面 Shell
        /// </summary>
        //public static IShellFolder GetDesktopFolder(out IntPtr ppshf)
        //{
        //    SHGetDesktopFolder(out ppshf);
        //    Object obj = Marshal.GetObjectForIUnknown(ppshf);
        //    return (IShellFolder)obj;
        //}
    }

    
}
