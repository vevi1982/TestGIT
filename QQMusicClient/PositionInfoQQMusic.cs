using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace QQMusicClient
{
    /// <summary>
    /// 
    /// </summary>
    public class PositionInfoQQMusic
    {
        /// <summary>
        /// 标题栏 中央
        /// </summary>
        public static Point MainCaptionPt = new Point(300, 25);
        /// <summary>
        /// 主界面【图标登录】按钮。点击后 移动鼠标位置 到 0,0 否则会出现提示框
        /// </summary>
        public static Point MainCaptionLoginButtonPt = new Point(30, 30);
        /// <summary>
        /// 【试听列表】按钮位置
        /// </summary>
        public static Point MainTryListenButtonPt = new Point(70, 175);
        /// <summary>
        /// 【试听列表】第一首歌位置
        /// </summary>
        public static Point MainTryListenPanelFirstSongPt=new Point(175,170);
        /// <summary>
        /// 【试听列表】下载按钮位置 ，点击后 下 下 回车。下载弹出框
        /// </summary>
        public static Point MainTryListenPanelDownLoadButtonPt = new Point(380, 105);


       
        
        /// <summary>
        /// 更改用户提示框 关闭按钮
        /// </summary>
        public static Point ChangeUserAlertClosePt = new Point(460, 355);
        

        #region QQ登陆框
        /// <summary>
        /// 登陆框QQ号 输入
        /// </summary>
        public static Point LoginFormText1Pt=new Point(550,290);
        /// <summary>
        /// 登陆框 密码输入
        /// </summary>
        public static Point LoginFormPassPt = new Point(550, 320);
        /// <summary>
        /// 登陆框 【确定】 按钮
        /// </summary>
        public static Point LoginFormOKButtonPt=new Point(450,400);
        /// <summary>
        /// 登陆框 关闭按钮
        /// </summary>
        public static Point LoginFormClosePt = new Point(598, 204);
        #endregion

        #region 下载对话框
        /// <summary>
        /// 【下载对话框】的 【下载到电脑】按钮位置
        /// </summary>
        public static Point DownLoadDiagButtonPt = new Point(460, 430);
        /// <summary>
        /// 【下载对话框】的 关闭 按钮位置
        /// </summary>
        public static Point DownLoadDiagClosePt = new Point(710, 147);

        #endregion

        #region 下载验证码输入
        /// <summary>
        ///【下载验证码输入框】文本框位置
        /// </summary>
        public static Point VeryCodeDownLoadTxtPt=new Point(269,270);
        /// <summary>
        /// 【下载验证码输入框】图片左上
        /// </summary>
        public static Point VeryCodeDownLoadImgLeftTopPt = new Point(319, 230);
        /// <summary>
        /// 【下载验证码输入框】图片右下
        /// </summary>
        public static Size IDCodeImgSize = new Size(110, 60);
        /// <summary>
        /// 【下载验证码输入框】确定按钮
        /// </summary>
        //TODO... 确认
        public static Point VeryCodeDownLoadOKPt=new Point(380,300);
        #endregion
        
    }
}
