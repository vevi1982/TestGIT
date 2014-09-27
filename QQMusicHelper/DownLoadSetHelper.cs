using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vevisoft.WindowsAPI;
using mshtml;

namespace QQMusicHelper
{
    public class DownLoadSetHelper
    {
        private static bool isExistDiag = false;
        private static IntPtr IEHandler = IntPtr.Zero;
        public static string downloadCookie = "";

        /// <summary>
        /// 获取下载窗体
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetDownLoadForm()
        {
            downloadCookie = "";
            //
            isExistDiag = false;
            IEHandler = IntPtr.Zero;
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowCallBack;
            if (SystemWindowsAPI.EnumWindows(callback, 0) == 0)
            {
                // throw new Exception("枚举窗体函数失败！"); //函数枚举失败   
            }
            //
            return IEHandler;
        }

        private static bool EnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            IntPtr fatherHwnd = SystemWindowsAPI.GetParent(hwnd);
            if (fatherHwnd != IntPtr.Zero)
            {
                //TXGFLayerMask
                var faTitle = new StringBuilder(256);
                SystemWindowsAPI.GetWindowText(fatherHwnd, faTitle, 257);
                if (!string.IsNullOrEmpty(faTitle.ToString()) && faTitle.ToString().Contains("下载"))
                {
                    //属于下载对话框的子窗体。查找IE
                    var strclsName = new StringBuilder(256);
                    SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
                    var strTitle = new StringBuilder(256);
                    SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
                    if (strclsName.ToString().Trim().ToLower() != "TXGFLayerMask".ToLower())
                        return true;
                    IntPtr chHandle = SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null, null);
                    if (chHandle != IntPtr.Zero)
                    {
                        IntPtr chHandle2 = SystemWindowsAPI.FindWindowEx(chHandle, IntPtr.Zero, null, null);
                        IntPtr chHandle3 = SystemWindowsAPI.FindWindowEx(chHandle2, IntPtr.Zero, null, null);
                        IEHandler = SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
                        if (IEHandler == IntPtr.Zero)
                            return true;
                        var id = IEAPI.GetHtmlDocumentObject(IEHandler);
                        if (id == null)
                            return true;
                        var str = id.body.innerHTML;
                        if (string.IsNullOrEmpty(str))
                            return true;
                        //找到窗体
                        isExistDiag = true;
                        //return false;
                    }
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="qqno"></param>
        /// <param name="remain"></param>
        /// <param name="songlistCount">歌单数量</param>
        /// <param name="downCount">下载数量</param>
        /// <returns></returns>
        public static bool SetDownNum(string qqno, out int songlistCount,out int downCount)
        {
            var dlInfo = DownLoadInfoHelper.GetDownLoadInfo(qqno);
            //
            var id = IEAPI.GetHtmlDocumentObject(IEHandler);
            downloadCookie = id.cookie;

            songlistCount = GetSongCount(id.body.innerHTML);
            //获取剩余数量
            downCount = songlistCount;

            if (songlistCount > dlInfo.Remain)
            {
                downCount = dlInfo.Remain;
                //取消选择
                var elements2 =
                    id.all.Cast<HTMLDivElement>()
                      .Where(
                          item =>
                          (item.className == "checkbox checkbox_press js_nl" ||
                           item.className == "checkbox js_nl checkbox_press"));
                foreach (HTMLDivElement htmlDivElement in elements2)
                {
                    Console.WriteLine(htmlDivElement.outerHTML);
                    htmlDivElement.click();
                    var str2 = htmlDivElement.innerHTML;
                    Console.WriteLine(htmlDivElement.outerHTML);
                }
                var elements3 =
                    id.all.Cast<mshtml.HTMLSpanElement>()
                      .Where(item => item.className == "checkbox js_nl");
                bool isfirst = true;
                //剩余下载数
                var count = dlInfo.Remain;
                //
                foreach (HTMLSpanElement htmlDivElement in elements3)
                {
                    if (count > 0)
                    {
                        if (htmlDivElement
                                .parentElement.parentElement
                                .parentElement.parentElement.id == "id_songlist")
                        {
                            Console.WriteLine(htmlDivElement.outerHTML);
                            htmlDivElement.click();
                            var str2 = htmlDivElement.innerHTML;
                            Console.WriteLine(htmlDivElement.outerHTML);
                            count--;
                        }
                    }
                }
            }
            //点击下载按钮 <a class="mod_bigbtn" href="javascript:;"><i class="icon_down"></i>下载到电脑<i class="bg"></i></a>
            foreach (IHTMLElement btnok in id.links.Cast<IHTMLElement>().Where(btnok => btnok.className.ToLower()=="mod_bigbtn"))
            {
                btnok.click();
                break;
            }
            //确定是否超限？？如果两个客户端同时下载一个Q，会出现这种问题
            //TODO.....
            //
            return true;
        }
        #region Others 
        /// <summary>
        /// 获取歌单歌曲总数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private static int GetSongCount(string str)
        {
            //str.LastIndexOf()
            if (!str.Contains("js_selnum"))
                return 0;
            var idx2 = str.IndexOf("js_selnum", System.StringComparison.Ordinal);
            //str = str.Substring(idx2);
            int idx = str.IndexOf("首", idx2, System.StringComparison.Ordinal);
            if (idx < 0)
                return 0;
            var intvalue = str.Substring(idx2, idx - idx2);
            try
            {
                return GetIntFromString(intvalue);
            }
            catch (Exception)
            {
                return 0;
            }
            return 0;
        }

        private static int GetIntFromString(string str)
        {
            int number = 0;
            string num = str.Where(item => item >= 48 && item <= 58).Aggregate<char, string>(null, (current, item) => current + item);
            number = int.Parse(num);
            return number;
        }
        #endregion
        
    }

}
