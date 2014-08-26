using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vevisoft.WebOperate
{
    public class WebBrowserElement
    {
        // Methods
        public static int GetPicIndex(WebBrowser wbMail, string Src, string Alt)
        {
            for (int i = 0; i < wbMail.Document.Images.Count; i++)
            {
                IHTMLImgElement domElement = (IHTMLImgElement)wbMail.Document.Images[i].DomElement;
                if (Alt == "")
                {
                    if (domElement.src.Contains(Src))
                    {
                        return i;
                    }
                }
                else if (!string.IsNullOrEmpty(domElement.alt) && domElement.alt.Contains(Alt))
                {
                    return i;
                }
            }
            return -1;
        }

        public static Image GetRegCodePic(WebBrowser wbMail, string ImgName, string Src, string Alt)
        {
            IHTMLControlElement domElement;
            HTMLDocument domDocument = (HTMLDocument)wbMail.Document.DomDocument;
            HTMLBody body = (HTMLBody)domDocument.body;
            IHTMLControlRange range = (IHTMLControlRange)body.createControlRange();
            if (ImgName == "")
            {
                int num = GetPicIndex(wbMail, Src, Alt);
                if (num == -1)
                {
                    return null;
                }
                domElement = (IHTMLControlElement)wbMail.Document.Images[num].DomElement;
            }
            else
            {
                domElement = (IHTMLControlElement)wbMail.Document.All[ImgName].DomElement;
            }
            range.add(domElement);
            range.execCommand("Copy", false, null);
            Image image = Clipboard.GetImage();
            Clipboard.Clear();
            return image;
        }
    }


}
