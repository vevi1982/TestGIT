using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Vevisoft.WindowsAPI;

/************************************************************************************
 * Copyright (c) 2014Microsoft All Rights Reserved.
 * CLR版本： 4.0.30319.17929
 *机器名称：VEVISOFT
 *公司名称：Microsoft
 *命名空间：QQMusicHelper
 *文件名：  QQMusicPlaySongControl
 *版本号：  V1.0.0.0
 *唯一标识：5856997c-1dbc-43c5-a4bd-980fe2ad660a
 *当前的用户域：VEVISOFT
 *创建人：  vevi
 *电子邮箱：
 *创建时间：2014/12/3 23:53:55
 *描述：
 *
 *=====================================================================
 *修改标记
 *修改时间：2014/12/3 23:53:55
 *修改人： Administrator
 *版本号： V1.0.0.0
 *描述：
 *
/************************************************************************************/
namespace QQMusicHelper
{
    public class QQMusicPlaySongControl
    {
        /// <summary>
        /// 开始播放试听列表的歌曲
        /// </summary>
        public static bool StartPlay()
        {
            var hwnd = QQMusicOperateHelper.GetQQMusicHandle();
            if (hwnd == IntPtr.Zero)
                return false;
            //点击试听列表，
            SystemWindowsAPI.SetForegroundWindow(hwnd);
            QQMusicOperateHelper.MouseSetPositonAndLeftClick(hwnd, PositionInfoQQMusic.MaintrySongListBtnPt);
            //等待响应
            while (!SystemWindowsAPI.IsExeNotResponse(hwnd) || !QQMusicOperateHelper.GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
            //点击下一首歌曲
            SystemWindowsAPI.SetForegroundWindow(hwnd);
            QQMusicOperateHelper.MouseSetPositonAndLeftClick(hwnd, PositionInfoQQMusic.MainTryListenPanelPlayAllBtnPt);
            while (!SystemWindowsAPI.IsExeNotResponse(hwnd) || !QQMusicOperateHelper.GetMainResponseByProcess())
            {
                Thread.Sleep(1000);
            }
            return true;
        }
        /// <summary>
        /// 手动拖动到歌曲播放结束位置
        /// </summary>
        /// <returns></returns>
        public static bool PlayEndUserSet()
        {
            var hwnd = QQMusicOperateHelper.GetQQMusicHandle();
            if (hwnd == IntPtr.Zero)
                return false;
            //点击试听列表，
            SystemWindowsAPI.SetForegroundWindow(hwnd);
            QQMusicOperateHelper.MouseSetPositonAndLeftClick(hwnd, PositionInfoQQMusic.MainPlayScrollEnd2S);
            return true;
        }
    }
}
