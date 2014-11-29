using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vevisoft.WindowsAPI;
using mshtml;

namespace QQMusicHelper
{
    /// <summary>
    /// 获取QQ音乐用户信息,包含共享歌单信息
    /// </summary>
    public class QQMusicUserInfo
    {
        private static bool isExistDiag = false;
        private static IntPtr IEHandler = IntPtr.Zero;
        private static string htmlFlag = "您下载歌曲的次数过于频繁";

        public static string LoginUserNo = "";

        /// <summary>
        /// 查找Form并且点击歌单
        /// </summary>
        /// <param name="songlistName"></param>
        /// <returns></returns>
        public static bool FindFormAndCLickBtn(string songlistName)
        {
            IntPtr hwnd = GetUserInfoForm();
            if (hwnd != IntPtr.Zero)
               return GetSongListAndClick(songlistName, hwnd);
            return false;
        }
        /// <summary>
        /// 查找Form的IE
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetUserInfoForm()
        {
            LoginUserNo = "";
            isExistDiag = false;
            IEHandler = IntPtr.Zero;
            //需要遍历所有窗体
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowCallBack;
            if (SystemWindowsAPI.EnumWindows(callback, 0) == 0)
            {
                //throw new Exception("枚举窗体函数失败！"); //函数枚举失败
            }
            //
            return IEHandler;
        }

        private static bool EnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (!(strclsName.ToString().Trim().ToLower() == "TXGFLayerMask".ToLower() ||
                  strclsName.ToString().Trim().ToLower() == "TXGuiFoundation".ToLower())) //QQ音乐 查找歌单
                return true;
            //
            IntPtr chHandle = SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null,null);
            if (chHandle != IntPtr.Zero)
            {
                strclsName = new StringBuilder(256);
                SystemWindowsAPI.GetClassName(chHandle, strclsName, 257);
                strTitle = new StringBuilder(256);
                SystemWindowsAPI.GetWindowText(chHandle, strTitle, 257);
                //
                IntPtr chHandle2 = SystemWindowsAPI.FindWindowEx(chHandle, IntPtr.Zero, null, null);
                IntPtr chHandle3 = SystemWindowsAPI.FindWindowEx(chHandle2, IntPtr.Zero, null, null);
                var didcIeHandler1 = SystemWindowsAPI.FindWindowEx(chHandle3, IntPtr.Zero, null, null);
                if (didcIeHandler1 == IntPtr.Zero)
                    return true;
                var id = IEAPI.GetHtmlDocumentObject(didcIeHandler1);
                if (id == null)
                    return true;
                //
                LoginUserNo = GetQQno(id.cookie);
                //
                Vevisoft.Log.VeviLog2.WriteLogInfo("LoginQQNO:"+LoginUserNo);
                //
                var str = id.body.innerHTML;
                if (str == null) return true;
                //string cookies = id.cookie;
                if (str.Contains("音乐基因"))
                {
                    //用户已登录
                    IEHandler = didcIeHandler1;
                    isExistDiag = str.Contains("点歌");
                    //return false;//遍历结束
                }
            }
            return true;
        }

        private static string GetQQno(string p)
        {
            var tmp = "qqmusic_uin=";
            int idx = p.IndexOf(tmp);
            int idx2 = p.IndexOf(";", idx);
            return p.Substring(idx + tmp.Length, idx2 - idx-tmp.Length).Trim();
        }


        public static bool GetSongListAndClick(string songlistName,IntPtr ieHwnd)
        {
            var count = 5;
            var clickSuccess = false;
            if (ClickTabBtn(ieHwnd))
            {
                do
                {
                    clickSuccess = IsContainsSongListAndClick(songlistName, ieHwnd);
                    if (!clickSuccess)
                    {
                        ClickGetSongListbtn("歌单",ieHwnd);
                        Thread.Sleep(2000);
                        ClickGetSongListbtn("点歌", ieHwnd);
                        Thread.Sleep(2000);
                    }
                    count--;
                    Thread.Sleep(4000);
                } while (!clickSuccess && count > 0);
                if (!clickSuccess && count < 1)
                {
                    //OnShowInStatusBarEvent("没有发现【共享歌单】" + songlistName);
                    throw new Exception("没有加载共享歌单：" + songlistName);
                }
            }
            return clickSuccess;
        }
        /// <summary>
        /// 点击【点歌】tab的按钮
        /// </summary>
        /// <param name="ieHwnd"></param>
        /// <returns></returns>
        public static bool ClickTabBtn(IntPtr ieHwnd)
        {
            var count = 5;
            var clickSuccess = false;
            do
            {
                ClickGetSongListbtn("歌单",ieHwnd);
                Thread.Sleep(1000);
                clickSuccess = ClickGetSongListbtn("点歌",ieHwnd);
                count--;
                //此时可能是没有加载完全，等待加载
                Thread.Sleep(4000);
            } while (!clickSuccess && count > 0);
            //
            if (!clickSuccess && count < 1)
            {
                //OnShowInStatusBarEvent("没有发现【点歌】按钮!");
                throw new Exception("没有发现【点歌】按钮!");
            }
            return clickSuccess;
        }

        /// <summary>
        /// 打开点歌tab后，是否存在歌单，存在则点击
        /// </summary>
        /// <param name="songlistName"></param>
        /// <param name="ieHwnd"></param>
        /// <returns></returns>
        public static bool IsContainsSongListAndClick(string songlistName,IntPtr ieHwnd)
        {
            var id = IEAPI.GetHtmlDocumentObject(ieHwnd);
            if (id == null)
                return false;
            var str = id.body.innerHTML;
            foreach (IHTMLElement link in id.links)
            {
                if (!string.IsNullOrEmpty(link.innerHTML) && link.innerHTML.Contains("《" + songlistName))
                {
                    link.click();
                    Thread.Sleep(1000);
                    return true;
                }
            }
            //没有此歌单，查看是否有下一页，在下一页中查找
            foreach (IHTMLElement link in id.links)
            {
                if (!string.IsNullOrEmpty(link.title) && link.title.Contains("下一页"))
                {
                    link.click();
                    Thread.Sleep(4000);
                    return IsContainsSongListAndClick(songlistName, ieHwnd);
                }
            }
            return false;
        }

        /// <summary>
        /// 点击按钮
        /// </summary>
        /// <returns>有没有此按钮</returns>
        public static bool ClickGetSongListbtn(string btnName,IntPtr ieHwnd)
        {
            var id = IEAPI.GetHtmlDocumentObject(ieHwnd);
            if (id == null)
                return false;
            var str = id.body.innerHTML;
            foreach (IHTMLElement link in id.links.Cast<IHTMLElement>().Where(link => !string.IsNullOrEmpty(link.innerHTML) && link.innerHTML.Contains(btnName)))
            {
                link.click();
                return true;
            }
            return false;
        }
    }
}
