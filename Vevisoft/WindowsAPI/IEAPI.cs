using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mshtml;

namespace Vevisoft.WindowsAPI
{
    public class IEAPI
    {
        /// <summary>
        /// 通过IE句柄，获取msthml的HTMLDocument对象
        /// </summary>
        /// <param name="IEhwnd"></param>
        /// <returns></returns>
        public static mshtml.IHTMLDocument2 GetHtmlDocumentObject(IntPtr IEhwnd)
        {
            var domObject = new System.Object();
            int tempInt = 0;
            var guidIEDocument2 = new Guid();
            var WM_Html_GETOBJECT = SystemWindowsAPI.RegisterWindowMessage("WM_Html_GETOBJECT"); //定义一个新的窗口消息
            int W = SystemWindowsAPI.SendMessage(IEhwnd, WM_Html_GETOBJECT, 0, ref tempInt);
                //注:第二个参数是RegisterWindowMessage函数的返回值
            int lreturn = SystemWindowsAPI.ObjectFromLresult(W, ref guidIEDocument2, 0, ref domObject);
            mshtml.IHTMLDocument2 doc = (mshtml.IHTMLDocument2) domObject;
            return doc;
        }
        /// <summary>
        /// 保存IE中验证码图片到硬盘中
        /// </summary>
        /// <param name="didcIeHandle"></param>
        /// <param name="imgHtmlID"></param>
        /// <param name="filename">图片保存地址</param>
        /// <returns></returns>
        public  static bool GetIDCodeDownLoad(IntPtr didcIeHandle,string imgHtmlID,string filename)
        {
            mshtml.IHTMLDocument2 id = GetHtmlDocumentObject(didcIeHandle);
            if (id == null)
                return false;
            //OnShowInStatusMonitor("获取验证码图片");
            var img =
                id.images.Cast<IHTMLElement>().Where(item => item.id == imgHtmlID).Cast<IHTMLControlElement>().FirstOrDefault();
            if (img != null)
            {
                var range = (IHTMLControlRange)((HTMLBody)id.body).createControlRange();
                range.add(img);
                bool bol1 = range.execCommand("Copy", false, null);
                if (Clipboard.ContainsImage())
                {
                    //OnShowInStatusMonitor("保存验证码图片");
                    var image = Clipboard.GetImage();
                    if (image != null) image.Save(filename);
                    else return false;
                    return true;
                }
            }
            return false;
        }
    }
}
