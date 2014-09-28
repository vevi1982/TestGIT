using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vevisoft.ImageRecgnize
{

    /*
     * sunzhenkui my120630
    密保选我的生日，输入 19801026
     */
    /// <summary>
    /// sunzhenkui,my120630,1014  vevisoft shiyuxing521
    /// </summary>
    public class IdentifyingCodeRecg
    {
        public static string GetCodeByUUCodeWeb(string imagePath,int codetype )
        {
            int SoftID = 93025;
            //"2920E48D-6F5F-45F6-AB6E-FFD9E94EB582"
            string SoftKey = "03ff02294b15451e943c1326915a54fc";
            UUCodeWrapper.uu_setSoftInfoA(SoftID, SoftKey);
            UUCodeWrapper.uu_loginA("sunzhenkui", "my120630");
            //ImageConverter converter = new ImageConverter();
            //byte[] source = (byte[]) converter.ConvertTo(bit, typeof (byte[]));
            var sb = new StringBuilder();
            UUCodeWrapper.uu_recognizeByCodeTypeAndPathA(imagePath, codetype, sb);
            if (sb.ToString().Length >= 4)
                return sb.ToString().Substring(sb.Length - 4, 4);
            else return "";
        }
        /// <summary>
        /// 截图 并获取验证码。
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="widht"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetCodeByUUCode(int x, int y, int widht, int height)
        {
            int SoftID = 93025;
            string SoftKey = "03ff02294b15451e943c1326915a54fc";
            UUCodeWrapper.uu_setSoftInfoA(SoftID, SoftKey);
            UUCodeWrapper.uu_loginA("sunzhenkui", "my120630");
            //ImageConverter converter = new ImageConverter();
            //byte[] source = (byte[]) converter.ConvertTo(bit, typeof (byte[]));
            var sb = new StringBuilder();
            UUCodeWrapper.uu_recognizeScreenByCodeTypeA(x, y, widht, height, 1004, sb);
            if (sb.ToString().Length >= 4)
                return sb.ToString().Substring(sb.Length - 4, 4);
            else return "";
        }
    }
}
