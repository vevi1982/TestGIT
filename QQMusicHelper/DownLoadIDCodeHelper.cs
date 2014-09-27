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
    /// qq音乐下载验证码窗体,获取并输入验证码
    /// </summary>
    public class DownLoadIDCodeHelper
    {
        private static bool isExistDiag = false;
        private static IntPtr IEHandler = IntPtr.Zero;
        private static string htmlFlag = "您下载歌曲的次数过于频繁";

        /// <summary>
        /// 获取下载验证码窗体
        /// </summary>
        /// <returns></returns>
        public static IntPtr GetDownLoadIdCodeForm()
        {
            isExistDiag = false;
            IEHandler = IntPtr.Zero;
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowCallBack;
            if (SystemWindowsAPI.EnumWindows(callback, 0) == 0)
            {
                //throw new Exception("枚举窗体函数失败！"); //函数枚举失败
            }
            //
            return IEHandler;
        }

        /// <summary>
        /// 查找下载验证码窗体，如果有那么输入验证码
        /// </summary>
        public static void GetDownLoadIdCodeFormAndInputIdCode()
        {
            isExistDiag = false;
            IEHandler = IntPtr.Zero;
            SystemWindowsAPI.FindWindowCallBack callback = EnumWindowCallBack;
            if (SystemWindowsAPI.EnumWindows(callback, 0) == 0)
            {
                //throw new Exception("枚举窗体函数失败！"); //函数枚举失败
            }
            //
            if (IEHandler != IntPtr.Zero && isExistDiag)
            {
                var code = SaveIdImgAndGetCode();
                InputVeryCode(code, IEHandler);
            }
            //return isexistDownLoadIDCodeDiag;
        }

        private static bool EnumWindowCallBack(IntPtr hwnd, int lParam)
        {
            var strclsName = new StringBuilder(256);
            SystemWindowsAPI.GetClassName(hwnd, strclsName, 257);
            var strTitle = new StringBuilder(256);
            SystemWindowsAPI.GetWindowText(hwnd, strTitle, 257);
            if (!(strclsName.ToString().Trim().ToLower() == "TXGFLayerMask".ToLower() ||
                strclsName.ToString().Trim().ToLower() == "TXGuiFoundation".ToLower()))//QQ音乐 查找歌单
                return true;
            //
            IntPtr chHandle = SystemWindowsAPI.FindWindowEx(hwnd, IntPtr.Zero, null, null);
            if (chHandle != IntPtr.Zero)
            {
                try
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
                    var str = id.body.innerHTML;
                    //string cookies = id.cookie;
                    if (str != null && str.Contains("请输入验证码"))
                    {
                        //OnShowInStatusMonitor("下载验证码窗体IE句柄");
                        IEHandler = didcIeHandler1;
                        isExistDiag = str.Contains(htmlFlag);
                        //找到下载验证码对话框
                        //return false;
                        //OnShowInStatusMonitor("下载验证码窗体IE句柄" + (isexistDownLoadIDCodeDiag ? "111" : "0000"));
                        //
                    }
                }
                catch
                {
                    return true;
                }

            }
            return true;
        }
        /// <summary>
        /// 保存验证码图片
        /// </summary>
        /// <returns></returns>
        private static string SaveIdImgAndGetCode()
        {
            const string path = @"c:\bb.bmp";
            if(!IEAPI.GetIDCodeDownLoad(IEHandler, "imgVerify", path))
                throw new Exception("没有获取到图片");
            return Vevisoft.ImageRecgnize.IdentifyingCodeRecg.GetCodeByUUCodeWeb(@"c:\bb.bmp", 1004);
        }
        /// <summary>
        /// 输入验证码,并确定
        /// </summary>
        /// <param name="code"></param>
        /// <param name="iehwnd"></param>
        /// <returns></returns>
        private static bool InputVeryCode(string code,IntPtr iehwnd)
        {
            var iedoc = IEAPI.GetHtmlDocumentObject(iehwnd);
            //
            iedoc.parentWindow.execScript("showElement('id_verify')", "javascript");
            var idtext = iedoc.all.item("vcode", 0) as IHTMLElement;
            if (idtext != null)
            {
                idtext.setAttribute("value", code);
            }
            else
                throw new Exception("没有找到输入验证码框" + code);
            //var btnok = id.all.item("", 0) as IHTMLElement;
            foreach (IHTMLElement btnok in iedoc.links)
            {
                if (btnok.innerHTML.Contains("确认"))
                {
                    btnok.click();
                    break;
                }
            }
            //确认 是否正确
            Thread.Sleep(1000);
            //
            var errortip = iedoc.all.item("error_tips", 0) as IHTMLElement;
            if (errortip != null && errortip.innerHTML.Contains("错误"))
                return false;
            return true;
        }
    }
}
