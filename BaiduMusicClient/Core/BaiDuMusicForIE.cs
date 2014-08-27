using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using BaiduMusicClient.Models;
using Vevisoft.Log;
using Vevisoft.WebOperate;

namespace BaiduMusicClient.Core
{
    public class BaiDuMusicForIE
{
    // Fields
    private string baiduidCookie;
    public static readonly string BaiduMusicBoxURL = "http://play.baidu.com";
    public static readonly string BaiduMusicSearchURL = "http://music.baidu.com/search?key=";
    public static readonly string BaiduMusicSongUrl = "http://music.baidu.com/song/";
    public static readonly string BaiduMusicUrl = "http://music.baidu.com/";
    public static readonly string BaiduSongInfoURL = "http://play.baidu.com/data/music/songinfo";
    public static readonly string BaiduSongLinkURL = "http://play.baidu.com/data/music/songlink";
    public static readonly string BaiduStatageURL = "http://play.baidu.com/statage";
    private int count;
    private int count2;
    private int getsongTimes;
    private int SongLinkTimes;
    private bool TimeDecrease;
    private int visitTimes;
    private WebProxy wbproxy;

    // Methods
    public BaiDuMusicForIE(BaiduSongModel song, bool timedec)
    {
        this.TimeDecrease = true;
        this.count = 3;
        this.SongLinkTimes = 3;
        this.count2 = 3;
        this.getsongTimes = 3;
        this.visitTimes = 3;
        this.TimeDecrease = timedec;
        this.songInfo = song;
    }

    public BaiDuMusicForIE(BaiduSongModel song, WebProxy proxy) : this(song, false)
    {
        this.wbproxy = proxy;
    }

    private string GetBAIDUIDCookie(string url)
    {
        string str = "";
        try
        {
            using (HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponseProxy(url, null, null, "", "", "", this.wbproxy))
            {
                if (response.StatusCode.ToString().Contains("OK"))
                {
                    string str2 = response.Headers.ToString();
                    int startIndex = str2.LastIndexOf("BAIDUID");
                    if (startIndex > 0)
                    {
                        int index = str2.IndexOf(";", startIndex);
                        str = str2.Substring(startIndex, index - startIndex);
                    }
                }
            }
            this.count = 3;
        }
        catch
        {
            this.count--;
            if (this.count <= 0)
            {
                throw new Exception("can not visit from baidumusicbox");
            }
            this.GetBAIDUIDCookie(url);
        }
        VeviLog2.WriteLogInfo("获取BaiduCookie :" + str);
        return str;
    }

    public string GetDateTimeString()
    {
        DateTime now = DateTime.Now;
        if (this.TimeDecrease)
        {
            now = now.AddMinutes(-1.0);
        }
        return now.ToString("yyyy-MM-dd-HH:mm:ss");
    }

    public string GetDateTimeStringNow()
    {
        return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
    }

    public string GetDateTimeTicks()
    {
        DateTime now = DateTime.Now;
        if (this.TimeDecrease)
        {
            now = now.AddMinutes(-1.0);
        }
        return HttpWebResponseUtility.GetTimeTIcks(now);
    }

    public string GetDateTimeTicksNow()
    {
        return HttpWebResponseUtility.GetTimeTIcks(DateTime.Now);
    }

    private void GetRefanent(string baiduCookie)
    {
        string url = BaiduMusicBoxURL + string.Format("?__m=mboxCtrl.playSong&__a={0}&__o=/song/{0}||playBtn&fr=-1||-1", this.songInfo.SongBaiduId);
        string refer = "http://music.baidu.com/song/" + this.songInfo.SongBaiduId;
        try
        {
            using (HttpWebResponse response = HttpWebResponseUtility.CreateGetHttpResponse(url, null, null, baiduCookie, refer, ""))
            {
                if (response.StatusCode.ToString().Contains("OK"))
                {
                    VeviLog2.WriteLogInfo("WorkThread is Start BaiduMusicBoxURL 11");
                    string str3 = "mbox.verTag";
                    string postResponseTextFromResponse = HttpWebResponseUtility.GetPostResponseTextFromResponse(response);
                    int index = postResponseTextFromResponse.IndexOf(str3);
                    if (index < 0)
                    {
                        throw new Exception("获取百度网页play.baidu.com失败！mbox.verTag获取失败！");
                    }
                    int num2 = postResponseTextFromResponse.IndexOf(";", (int) (index + str3.Length));
                    if (num2 < 0)
                    {
                        throw new Exception("获取百度网页play.baidu.com失败！mbox.verTag 的值获取失败！");
                    }
                    this.songInfo.refagent = postResponseTextFromResponse.Substring(index + str3.Length, (num2 - index) - str3.Length).Replace("=", "").Replace("'", "").Replace(";", "").Replace(str3, "").Trim();
                    this.count = 3;
                }
            }
        }
        catch
        {
            if (this.count <= 0)
            {
                throw new Exception("Get refgent Error!!");
            }
            this.count--;
            this.GetRefanent(baiduCookie);
        }
    }

    public bool GetSongInfo(BaiduSongModel SongInfo, string baiduidcookie)
    {
        string songBaiduId = this.songInfo.SongBaiduId;
        string referer = "http://play.baidu.com/?__m=mboxCtrl.playSong&__a=" + SongInfo.SongBaiduId + "&__o=/song/" + SongInfo.SongBaiduId + "||playBtn";
        songBaiduId = "songIds=" + songBaiduId;
        try
        {
            if (this.getsongTimes == 3)
            {
                using (HttpWebResponse response = HttpWebResponseUtility.CreatePostJsonHttpResponse(BaiduSongInfoURL, songBaiduId, null, baiduidcookie, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)", null, referer, this.wbproxy))
                {
                    VeviLog2.WriteLogInfo("Post Get Result!!");
                    this.getsongTimes = 3;
                    string postResponseTextFromResponse = HttpWebResponseUtility.GetPostResponseTextFromResponse(response);
                    if (!postResponseTextFromResponse.Contains("\"errorCode\":22000"))
                    {
                        goto Label_042B;
                    }
                    int index = postResponseTextFromResponse.IndexOf("[", StringComparison.Ordinal);
                    int num2 = postResponseTextFromResponse.IndexOf("]", StringComparison.Ordinal);
                    string[] strArray = postResponseTextFromResponse.Substring(index, num2 - index).Replace("\"", "").Split(new char[] { '{', '}' });
                    Dictionary<string, string> dictionary = new Dictionary<string, string>();
                    foreach (string str4 in strArray)
                    {
                        string str5 = "";
                        string str6 = "";
                        string[] strArray2 = str4.Split(new char[] { ',', ':' });
                        for (int i = 0; i < strArray2.Length; i++)
                        {
                            string str7 = strArray2[i];
                            if (str7.ToLower() == "songid")
                            {
                                str5 = strArray2[i + 1];
                            }
                            if (str7.ToLower() == "artistid")
                            {
                                str6 = strArray2[i + 1];
                            }
                        }
                        if (!string.IsNullOrEmpty(str5))
                        {
                            if (!dictionary.ContainsKey(str5))
                            {
                                dictionary.Add(str5, str6);
                            }
                            else
                            {
                                dictionary[str5] = str6;
                            }
                        }
                    }
                    if (dictionary.ContainsKey(this.songInfo.SongBaiduId))
                    {
                        this.songInfo.ArtistId = dictionary[this.songInfo.SongBaiduId];
                    }
                    return true;
                }
            }
            using (HttpWebResponse response2 = HttpWebResponseUtility.CreatePostJsonHttpResponse(BaiduSongInfoURL, songBaiduId, null, baiduidcookie, "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)", null, referer, null))
            {
                VeviLog2.WriteLogInfo("Post Get NOOOOOO Result!!");
                this.getsongTimes = 3;
                string str8 = HttpWebResponseUtility.GetPostResponseTextFromResponse(response2);
                if (str8.Contains("\"errorCode\":22000"))
                {
                    int startIndex = str8.IndexOf("[", StringComparison.Ordinal);
                    int num5 = str8.IndexOf("]", StringComparison.Ordinal);
                    string[] strArray3 = str8.Substring(startIndex, num5 - startIndex).Replace("\"", "").Split(new char[] { '{', '}' });
                    Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
                    foreach (string str9 in strArray3)
                    {
                        string str10 = "";
                        string str11 = "";
                        string[] strArray4 = str9.Split(new char[] { ',', ':' });
                        for (int j = 0; j < strArray4.Length; j++)
                        {
                            string str12 = strArray4[j];
                            if (str12.ToLower() == "songid")
                            {
                                str10 = strArray4[j + 1];
                            }
                            if (str12.ToLower() == "artistid")
                            {
                                str11 = strArray4[j + 1];
                            }
                        }
                        if (!string.IsNullOrEmpty(str10))
                        {
                            if (!dictionary2.ContainsKey(str10))
                            {
                                dictionary2.Add(str10, str11);
                            }
                            else
                            {
                                dictionary2[str10] = str11;
                            }
                        }
                    }
                    if (dictionary2.ContainsKey(this.songInfo.SongBaiduId))
                    {
                        this.songInfo.ArtistId = dictionary2[this.songInfo.SongBaiduId];
                    }
                    return true;
                }
            }
        }
        catch
        {
            this.getsongTimes--;
            if (this.getsongTimes <= 0)
            {
                throw new Exception("Time Out SongInfo " + BaiduSongInfoURL);
            }
            return this.GetSongInfo(this.songInfo, baiduidcookie);
        }
    Label_042B:
        return false;
    }

    private void GetSongLinkUrl(BaiduSongModel songinfo, string baiduidCookie)
    {
        string songBaiduId = songinfo.SongBaiduId;
        songBaiduId = "songIds=" + songBaiduId;
        try
        {
            if (this.SongLinkTimes == 3)
            {
                using (HttpWebResponse response = HttpWebResponseUtility.CreatePostJsonHttpResponse(BaiduSongLinkURL, songBaiduId, null, baiduidCookie, null, null, BaiduMusicBoxURL, this.wbproxy))
                {
                    VeviLog2.WriteLogInfo("Post Get  result!!");
                    this.SongLinkTimes = 3;
                    string postResponseTextFromResponse = HttpWebResponseUtility.GetPostResponseTextFromResponse(response);
                    if (postResponseTextFromResponse.Contains("\"errorCode\":22000"))
                    {
                        int index = postResponseTextFromResponse.IndexOf("[", StringComparison.Ordinal);
                        int num2 = postResponseTextFromResponse.IndexOf("]", StringComparison.Ordinal);
                        postResponseTextFromResponse = postResponseTextFromResponse.Substring(index, num2 - index);
                        songinfo.SongName = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.SongName);
                        songinfo.albumID = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "albumId");
                        songinfo.albumName = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "albumName");
                        songinfo.albumName = this.unicodetogb(songinfo.albumName);
                        songinfo.albumName = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.albumName);
                        songinfo.queryId = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "queryId");
                        songinfo.status = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "status");
                        songinfo.ArtistId = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "artistId");
                        songinfo.singerName = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "artistName");
                        songinfo.singerName = this.unicodetogb(songinfo.singerName);
                        songinfo.singerName = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.singerName);
                        songinfo.lrcLink = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "lrcLink");
                        songinfo.lrcLink = songinfo.lrcLink.Replace(@"\", "");
                        songinfo.lrcLink = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.lrcLink);
                        songinfo.LinkCode = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "linkCode");
                        songinfo.downLoadUrl = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "songLink");
                        songinfo.downLoadUrl = songinfo.downLoadUrl.Replace(@"\", "");
                        songinfo.downLoadUrl = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.downLoadUrl);
                        songinfo.format = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "format");
                        songinfo.time = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "time");
                        songinfo.songSize = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "size");
                        songinfo.rate = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "rate");
                        songinfo.songVersion = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "version");
                        songinfo.copyType = HttpWebResponseUtility.GetJsonValue(postResponseTextFromResponse, "copyType");
                    }
                    return;
                }
            }
            using (HttpWebResponse response2 = HttpWebResponseUtility.CreatePostJsonHttpResponse(BaiduSongLinkURL, songBaiduId, null, baiduidCookie, null, null, BaiduMusicBoxURL, null))
            {
                VeviLog2.WriteLogInfo("Post Get NOOO result!!");
                this.SongLinkTimes = 3;
                string jsonResult = HttpWebResponseUtility.GetPostResponseTextFromResponse(response2);
                if (jsonResult.Contains("\"errorCode\":22000"))
                {
                    int startIndex = jsonResult.IndexOf("[", StringComparison.Ordinal);
                    int num4 = jsonResult.IndexOf("]", StringComparison.Ordinal);
                    jsonResult = jsonResult.Substring(startIndex, num4 - startIndex);
                    songinfo.SongName = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.SongName);
                    songinfo.albumID = HttpWebResponseUtility.GetJsonValue(jsonResult, "albumId");
                    songinfo.albumName = HttpWebResponseUtility.GetJsonValue(jsonResult, "albumName");
                    songinfo.albumName = this.unicodetogb(songinfo.albumName);
                    songinfo.albumName = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.albumName);
                    songinfo.queryId = HttpWebResponseUtility.GetJsonValue(jsonResult, "queryId");
                    songinfo.status = HttpWebResponseUtility.GetJsonValue(jsonResult, "status");
                    songinfo.ArtistId = HttpWebResponseUtility.GetJsonValue(jsonResult, "artistId");
                    songinfo.singerName = HttpWebResponseUtility.GetJsonValue(jsonResult, "artistName");
                    songinfo.singerName = this.unicodetogb(songinfo.singerName);
                    songinfo.singerName = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.singerName);
                    songinfo.lrcLink = HttpWebResponseUtility.GetJsonValue(jsonResult, "lrcLink");
                    songinfo.lrcLink = songinfo.lrcLink.Replace(@"\", "");
                    songinfo.lrcLink = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.lrcLink);
                    songinfo.LinkCode = HttpWebResponseUtility.GetJsonValue(jsonResult, "linkCode");
                    songinfo.downLoadUrl = HttpWebResponseUtility.GetJsonValue(jsonResult, "songLink");
                    songinfo.downLoadUrl = songinfo.downLoadUrl.Replace(@"\", "");
                    songinfo.downLoadUrl = HttpWebResponseUtility.GetUrlEncodeValue(songinfo.downLoadUrl);
                    songinfo.format = HttpWebResponseUtility.GetJsonValue(jsonResult, "format");
                    songinfo.time = HttpWebResponseUtility.GetJsonValue(jsonResult, "time");
                    songinfo.songSize = HttpWebResponseUtility.GetJsonValue(jsonResult, "size");
                    songinfo.rate = HttpWebResponseUtility.GetJsonValue(jsonResult, "rate");
                    songinfo.songVersion = HttpWebResponseUtility.GetJsonValue(jsonResult, "version");
                    songinfo.copyType = HttpWebResponseUtility.GetJsonValue(jsonResult, "copyType");
                }
            }
        }
        catch
        {
            this.SongLinkTimes--;
            if (this.SongLinkTimes <= 0)
            {
                throw new Exception("Time Out SongLink...");
            }
            this.GetSongLinkUrl(this.songInfo, baiduidCookie);
        }
    }

    public void Play60AndCompleteStates()
    {
        this.SendBaiduButtonClick(this.songInfo, this.baiduidCookie, "60play");
        VeviLog2.WriteLogInfo("WorkThread is Start play60");
        this.SendBaiduButtonClick(this.songInfo, this.baiduidCookie, "playcomplete");
        this.SendBaiduButtonClick(this.songInfo, this.baiduidCookie, "playend");
    }

    public void SearchAndPlaySong()
    {
        this.baiduidCookie = this.GetBAIDUIDCookie(BaiduMusicUrl);
        string urlEncodeValue = HttpWebResponseUtility.GetUrlEncodeValue(this.GetDateTimeString());
        string refer = "http://music.baidu.com/";
        string str3 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=tracesrc&ref=music_web&tracesrc=-1%7C%7C-1&inittime=" + urlEncodeValue;
        string str4 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=resolution&sw=1366&sh=768&ref=music_web";
        string str5 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=exposure&page=home_box&expoitem=3&ref=music_web";
        List<string> list = new List<string> {
            str3,
            str4,
            str5
        };
        foreach (string str6 in list)
        {
            this.VisiteWebAddress(str6, this.baiduidCookie, refer, "");
        }
        string str7 = HttpWebResponseUtility.GetUrlEncodeValue(this.songInfo.SongName);
        string str8 = HttpWebResponseUtility.GetUrlEncodeValue(this.songInfo.SongName);
        str7 = str8;
        string url = BaiduMusicSearchURL + str8;
        this.VisiteWebAddress(url, this.baiduidCookie, "http://music.baidu.com/", "");
        urlEncodeValue = HttpWebResponseUtility.GetUrlEncodeValue(this.GetDateTimeString());
        str3 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=clicksearch&ref=music_web&key=" + str7 + "&search_res=1&page_type=first&page_num=1&sub=song";
        str4 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=clicksearch&page=searchresult&sub=song&logtype=exposure_pv&key=" + str7 + "&ref=music_web";
        string str10 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=tracesrc&ref=music_web&tracesrc=-1%7C%7C-1&inittime=" + urlEncodeValue;
        string str11 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=exposure&expoitem=targetbanner_album&ref=music_web";
        string str12 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=resolution&sw=1366&sh=768&ref=music_web";
        List<string> list2 = new List<string> {
            str3,
            str4,
            str10,
            str11,
            str12
        };
        foreach (string str13 in list2)
        {
            this.VisiteWebAddress(str13, this.baiduidCookie, url, "");
        }
        Thread.Sleep(0x3e8);
        string str14 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=clicksearch&page=searchresult&sub=song&logtype=click_pv&key=" + str7 + "&pos=firstsong&linktype=ico&resourcetype=0&song_id=" + this.songInfo.SongBaiduId + "&original_song_id=" + this.songInfo.SongBaiduId + "&act=audio&u_tag=unfold_0&ref=music_web";
        string str15 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=clicksearch&page=searchresult&sub=song&logtype=click_pv&key=" + str7 + "&pos=resultcontent&linktype=ico&resourcetype=0&song_id=" + this.songInfo.SongBaiduId + "&original_song_id=" + this.songInfo.SongBaiduId + "&act=audio&u_tag=unfold_0&ref=music_web";
        string str16 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=clicksearch&page=searchresult&sub=song&logtype=click_num&key=" + str7 + "&pos=resultcontent&linktype=ico&resourcetype=0&song_id=" + this.songInfo.SongBaiduId + "&original_song_id=" + this.songInfo.SongBaiduId + "&act=audio&u_tag=unfold_0&page_num=1&position=0&ref=music_web";
        string str17 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=siteToBox&operation=playSong&id=" + this.songInfo.SongBaiduId + "&ref=music_web";
        string str18 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + this.GetDateTimeTicks() + "&type=click&pos=play_song&page=m_search_song&ref=music_web";
        list2 = new List<string> {
            str14,
            str15,
            str16,
            str17,
            str18
        };
        foreach (string str19 in list2)
        {
            this.VisiteWebAddress(str19, this.baiduidCookie, url, "");
        }
        url = BaiduMusicSongUrl + this.songInfo.SongBaiduId;
        refer = "http://music.baidu.com/search?key=" + str7;
        this.VisiteWebAddress(url, this.baiduidCookie, refer, "");
        url = BaiduMusicBoxURL + string.Format("?__m=mboxCtrl.playSong&__a={0}&__o=/song/{0}||playBtn&fr=-1||-1", this.songInfo.SongBaiduId);
        refer = "http://music.baidu.com/song/" + this.songInfo.SongBaiduId;
        string str20 = "nega_" + DateTime.Now.ToString("yyyyMMddHHmm");
        this.GetRefanent(this.baiduidCookie);
        string str21 = "http://play.baidu.com/?__m=mboxCtrl.playSong&__a=" + this.songInfo.SongBaiduId + "&__o=/song/" + this.songInfo.SongBaiduId + "||playBtn&fr=-1||-1";
        string str22 = "http://nsclick.baidu.com/v.gif?pid=304&url=&type=locallistsongnum&r" + new Random().NextDouble().ToString() + "=1&ref=tingbox&refagent=" + str20 + "&num=1&flow=ad_voice_1";
        string str23 = str22.Replace("http://nsclick.baidu.com/v.gif", "http://log.music.baidu.com/r.gif");
        string str24 = "http://nsclick.baidu.com/v.gif?pid=304&url=&type=showlist&r" + new Random().NextDouble().ToString() + "=1&ref=tingbox&refagent=" + str20 + "&list=list_temp&flow=ad_voice_1";
        string str25 = str24.Replace("http://nsclick.baidu.com/v.gif", "http://log.music.baidu.com/r.gif");
        string str26 = "http://nsclick.baidu.com/v.gif?pid=304&url=&type=fav_status&r" + new Random().NextDouble().ToString() + "=1&ref=tingbox&refagent=" + str20 + "&list=list_temp&list_status=0&flow=ad_voice_1";
        string str27 = str26.Replace("http://nsclick.baidu.com/v.gif", "http://log.music.baidu.com/r.gif");
        list2 = new List<string> {
            str22,
            str23,
            str24,
            str25,
            str26,
            str27
        };
        foreach (string str28 in list2)
        {
            this.VisiteWebAddress(str28, this.baiduidCookie, str21, "");
        }
        this.GetSongInfo(this.songInfo, this.baiduidCookie);
        this.SendBaiduButtonClick(this.songInfo, this.baiduidCookie, "playstart");
        this.GetSongLinkUrl(this.songInfo, this.baiduidCookie);
        VeviLog2.WriteLogInfo("WorkThread is Start playstart");
        this.SendBaiduButtonClick(this.songInfo, this.baiduidCookie, "load");
        this.SendBaiduButtonClick(this.songInfo, this.baiduidCookie, "playsong100ms");
    }

    private void SendBaiduButtonClick(BaiduSongModel songInfo, string baiduCookie, string type)
    {
        string str = "/song/" + songInfo.SongBaiduId + "||playBtn||-1||-1";
        string url = "http://nsclick.baidu.com/v.gif?pid=304&url=&";
        string str25 = type;
        if (str25 != null)
        {
            if (!(str25 == "playstart"))
            {
                if (str25 == "load")
                {
                    url = string.Format("http://nsclick.baidu.com/v.gif?pid=304&url=&type={0}&song_id={1}&song_title={2}&singer_id={3}&singer_name={4}&album_id={5}&album_name={6}&copytype={7}&format={8}&queryid={9}&rate={10}&relatestatus=0&resourcetype=0&songsize={11}&song_version={12}&linkCode={13}&lrcLink={14}&time={15}&status={16}&from={20}&start2load=-1&source=web&song_type=0&songlist_id=%2Fsong%2F{1}&songlist_name=undefined&r{17}=1&ref=tingbox&flow=ad_voice_1&refagent={18}&username=&vip_type=0&userid=0&list=list_temp&link={19}&prestatus=playstart", new object[] { 
                        type, songInfo.SongBaiduId, songInfo.SongName, songInfo.ArtistId, songInfo.singerName, songInfo.albumID, songInfo.albumName, songInfo.copyType, songInfo.format, songInfo.queryId, songInfo.rate, songInfo.songSize, songInfo.songVersion, songInfo.LinkCode, songInfo.lrcLink, songInfo.time, 
                        songInfo.status, new Random().NextDouble().ToString("F16"), songInfo.refagent, songInfo.downLoadUrl, str
                     });
                }
                else if (str25 == "playsong100ms")
                {
                    url = string.Format("http://nsclick.baidu.com/v.gif?pid=304&url=&type={0}&song_id={1}&song_title={2}&singer_id={3}&singer_name={4}&album_id={5}&album_name={6}&copytype={7}&format={8}&queryid={9}&rate={10}&relatestatus=0&resourcetype=0&songsize={11}&song_version={12}&linkCode={13}&lrcLink={14}&time={15}&status={16}&from={22}&start2load=-1&source=web&song_type=0&songlist_id=%2Fsong%2F{1}&songlist_name=undefined&r{17}=1&ref=tingbox&flow=ad_voice_1&refagent={18}&username=&vip_type=0&userid=0&list=list_temp&fromload={20}&fromstart={21}&link={19}&module=private_btn", new object[] { 
                        type, songInfo.SongBaiduId, songInfo.SongName, songInfo.ArtistId, songInfo.singerName, songInfo.albumID, songInfo.albumName, songInfo.copyType, songInfo.format, songInfo.queryId, songInfo.rate, songInfo.songSize, songInfo.songVersion, songInfo.LinkCode, songInfo.lrcLink, songInfo.time, 
                        songInfo.status, new Random().NextDouble().ToString("F16"), songInfo.refagent, songInfo.downLoadUrl, new Random().Next(0x3e8), new Random().Next(0x3e8), str
                     });
                }
                else if (str25 == "60play")
                {
                    url = string.Format("http://nsclick.baidu.com/v.gif?pid=304&url=&type={0}&song_id={1}&song_title={2}&singer_id={3}&singer_name={4}&album_id={5}&album_name={6}&copytype={7}&format={8}&queryid={9}&rate={10}&relatestatus=0&resourcetype=0&songsize={11}&song_version={12}&linkCode={13}&lrcLink={14}&time={15}&status={16}&from=%2Fsong%2F{1}%7C%7CplayBtn%7C%7C-1%7C%7C-1&start2load=-1&source=web&song_type=0&songlist_id=%2Fsong%2F{1}&songlist_name=undefined&r{17}=1&ref=tingbox&flow=ad_voice_1&refagent={18}&username=&vip_type=0&userid=0&list=list_temp&position=60046&link={19}&usertype=lowflow&prestatus=playsong100ms&module=private_btn", new object[] { 
                        type, songInfo.SongBaiduId, songInfo.SongName, songInfo.ArtistId, songInfo.singerName, songInfo.albumID, songInfo.albumName, songInfo.copyType, songInfo.format, songInfo.queryId, songInfo.rate, songInfo.songSize, songInfo.songVersion, songInfo.LinkCode, songInfo.lrcLink, songInfo.time, 
                        songInfo.status, new Random().NextDouble().ToString("F16"), songInfo.refagent, songInfo.downLoadUrl
                     });
                }
                else if (str25 == "playcomplete")
                {
                    url = string.Format("http://nsclick.baidu.com/v.gif?pid=304&url=&type={0}&song_id={1}&song_title={2}&singer_id={3}&singer_name={4}&album_id={5}&album_name={6}&copytype={7}&format={8}&queryid={9}&rate={10}&relatestatus=0&resourcetype=0&songsize={11}&song_version={12}&linkCode={13}&lrcLink={14}&time={15}&status={16}&from={19}&start2load=-1&source=web&song_type=0&songlist_id=%2Fsong%2F{1}&songlist_name=undefined&r{17}=1&ref=tingbox&flow=ad_voice_1&refagent={18}&username=&vip_type=0&userid=0&list=list_temp&prestatus=60play", new object[] { 
                        type, songInfo.SongBaiduId, songInfo.SongName, songInfo.ArtistId, songInfo.singerName, songInfo.albumID, songInfo.albumName, songInfo.copyType, songInfo.format, songInfo.queryId, songInfo.rate, songInfo.songSize, songInfo.songVersion, songInfo.LinkCode, songInfo.lrcLink, songInfo.time, 
                        songInfo.status, new Random().NextDouble().ToString("F16"), songInfo.refagent, str
                     });
                }
                else if (str25 == "playend")
                {
                    url = string.Format("http://nsclick.baidu.com/v.gif?pid=304&url=&type={0}&song_id={1}&song_title={2}&singer_id={3}&singer_name={4}&album_id={5}&album_name={6}&copytype={7}&format={8}&queryid={9}&rate={10}&relatestatus=0&resourcetype=0&songsize={11}&song_version={12}&linkCode={13}&lrcLink={14}&time={15}&status={16}&from={20}&start2load=-1&source=web&song_type=0&songlist_id=%2Fsong%2F{1}&songlist_name=undefined&r{17}=1&ref=tingbox&flow=ad_voice_1&refagent={18}&username=&vip_type=0&userid=0&list=list_temp&position=255164&auto=1&load2play=816&start2play=1457&fromload=255881&fromstart=256523&buftime=0&buftotal=685&isclosed=0&bufp=100&bufc=4418&link={19}&songdur=255000&prestatus=playcomplete&open2play=1190&open2load=358&open2close=-1", new object[] { 
                        type, songInfo.SongBaiduId, songInfo.SongName, songInfo.ArtistId, songInfo.singerName, songInfo.albumID, songInfo.albumName, songInfo.copyType, songInfo.format, songInfo.queryId, songInfo.rate, songInfo.songSize, songInfo.songVersion, songInfo.LinkCode, songInfo.lrcLink, songInfo.time, 
                        songInfo.status, new Random().NextDouble().ToString("F16"), songInfo.refagent, songInfo.downLoadUrl, str
                     });
                }
            }
            else
            {
                url = string.Format("http://nsclick.baidu.com/v.gif?pid=304&url=&type={0}&song_id={1}&song_title={2}&singer_id={3}&singer_name={4}&album_id={5}&album_name={6}&copytype={7}&format={8}&queryid={9}&rate={10}&relatestatus=0&resourcetype=0&songsize={11}&song_version={12}&linkCode={13}&lrcLink={14}&time={15}&status={16}&from={19}&start2load=-1&source=web&song_type=0&songlist_id=%2Fsong%2F{1}&songlist_name=undefined&r{17}=1&ref=tingbox&flow=ad_voice_1&refagent={18}&username=&vip_type=0&userid=0&usertype=lowflow&auto=0", new object[] { 
                    type, songInfo.SongBaiduId, songInfo.SongName, songInfo.ArtistId, songInfo.singerName, songInfo.albumID, songInfo.albumName, songInfo.copyType, songInfo.format, songInfo.queryId, songInfo.rate, songInfo.songSize, songInfo.songVersion, songInfo.LinkCode, songInfo.lrcLink, songInfo.time, 
                    songInfo.status, new Random().NextDouble().ToString("F16"), songInfo.refagent, str
                 });
            }
        }
        string refer = "http://play.baidu.com/?__m=mboxCtrl.playSong&__a=" + songInfo.SongBaiduId + "&__o=/song/" + songInfo.SongBaiduId + "||playBtn&fr=-1||-1";
        if (type == "60play")
        {
            string str4 = "http://play.baidu.com/?__m=mboxCtrl.playSong&__a=" + songInfo.SongBaiduId + "&__o=/song/" + songInfo.SongBaiduId + "||playBtn&fr=-1||-1";
            string dateTimeTicksNow = this.GetDateTimeTicksNow();
            string urlEncodeValue = HttpWebResponseUtility.GetUrlEncodeValue(this.GetDateTimeStringNow());
            string str7 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + dateTimeTicksNow + "&type=tracesrc&ref=tingbox&tracesrc=-1%7C%7C-1&inittime=" + urlEncodeValue;
            this.VisiteWebAddress(str7, baiduCookie, str4, "");
        }
        this.VisiteWebAddress(url, baiduCookie, refer, "");
        string str8 = url.Replace("http://nsclick.baidu.com/v.gif", "http://log.music.baidu.com/r.gif");
        this.VisiteWebAddress(str8, baiduCookie, refer, "");
        if (type == "load")
        {
            Random random = new Random();
            string str9 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=firstsong&r", new Random().NextDouble().ToString("F16"), "=1&ref=tingbox&flow=ad_voice_1&refagent=", songInfo.refagent, "&openTimestamp=", this.GetDateTimeTicks(), "&open2ready=", random.Next(0x3e8, 0xbb8), "&open2load=", random.Next(0x3e8, 0xbb8), "&open2play=", random.Next(0x3e8, 0xbb8) });
            this.VisiteWebAddress(str9, baiduCookie, refer, "");
            string dateTimeTicks = this.GetDateTimeTicks();
            string str11 = HttpWebResponseUtility.GetUrlEncodeValue(this.GetDateTimeString());
            str9 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + dateTimeTicks + "&type=tracesrc&ref=tingbox&tracesrc=-1%7C%7C-1&inittime=" + str11;
            this.VisiteWebAddress(str9, baiduCookie, refer, "");
        }
        if (type == "60play")
        {
            this.SendStatageInfo(songInfo, baiduCookie);
        }
        if (type == "playsong100ms")
        {
            string str12 = this.GetDateTimeTicks();
            string str13 = HttpWebResponseUtility.GetUrlEncodeValue(this.GetDateTimeString());
            string str14 = "554963";
            string refagent = songInfo.refagent;
            string str16 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=exposure&r", new Random().NextDouble(), "=1&ref=tingbox&refagent=", refagent, "&page=tingbox&expoitem=tingbox&flow=ad_voice_1" });
            string str17 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=exposure&r", new Random().NextDouble(), "=1&ref=tingbox&flow=ad_voice_1&refagent=", refagent, "&page=tingbox&expoitem=tingbox" });
            string str18 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=exposure&r", new Random().NextDouble(), "=1&ref=tingbox&flow=ad_voice_1&refagent=", refagent, "&expoitem=skin_moren&page=tingbox" });
            string str19 = "http://nsclick.baidu.com/v.gif?pid=304&url=&v=1.0.0&r=" + str12 + "&type=tracesrc&ref=tingbox&tracesrc=-1%7C%7C-1&inittime=" + str13;
            string str20 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=swfloaded&r", new Random().NextDouble(), "=1&ref=tingbox&flow=ad_voice_1&refagent=", refagent, "&swfVersion=12.0.0.0&time=185&browser=" });
            string str21 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=ad_req&r", new Random().NextDouble(), "=1&ref=tingbox&flow=ad_voice_1&refagent=", refagent, "&page=tingbox&ad_id=", str14 });
            string str22 = string.Concat(new object[] { "http://nsclick.baidu.com/v.gif?pid=304&url=&type=ad_exposure&r", new Random().NextDouble(), "=1&ref=tingbox&flow=ad_voice_1&refagent=", refagent, "&page=tingbox&ad_id=", str14 });
            List<string> list = new List<string> {
                str16,
                str17,
                str18,
                str19,
                str20,
                str21,
                str22
            };
            string str23 = "http://play.baidu.com/?__m=mboxCtrl.playSong&__a=" + songInfo.SongBaiduId + "&__o=/song/" + songInfo.SongBaiduId + "||playBtn&fr=-1||-1";
            foreach (string str24 in list)
            {
                this.VisiteWebAddress(str24, baiduCookie, str23, "");
            }
        }
    }

    public bool SendStatageInfo(BaiduSongModel songInfo, string baiduidcookie)
    {
        bool flag = true;
        try
        {
            string referer = "http://play.baidu.com/?__m=mboxCtrl.playSong&__a=" + songInfo.SongBaiduId + "&__o=/song/" + songInfo.SongBaiduId + "||playBtn";
            string postData = "songid=" + songInfo.SongBaiduId + "&singerid=" + songInfo.ArtistId;
            if (this.count2 == 3)
            {
                using (HttpWebResponse response = HttpWebResponseUtility.CreatePostJsonHttpResponse(BaiduStatageURL, postData, null, baiduidcookie, null, null, referer, this.wbproxy))
                {
                    if (HttpWebResponseUtility.GetPostResponseTextFromResponse(response) == "0")
                    {
                        this.count2 = 3;
                    }
                    else
                    {
                        flag = false;
                    }
                    goto Label_0124;
                }
            }
            using (HttpWebResponse response2 = HttpWebResponseUtility.CreatePostJsonHttpResponse(BaiduStatageURL, postData, null, baiduidcookie, null, null, referer, null))
            {
                if (HttpWebResponseUtility.GetPostResponseTextFromResponse(response2) == "0")
                {
                    this.count2 = 3;
                }
                else
                {
                    flag = false;
                }
            }
        }
        catch (Exception)
        {
            this.count2--;
            if (this.count2 > 0)
            {
                this.SendStatageInfo(songInfo, baiduidcookie);
            }
        }
    Label_0124:
        VeviLog2.WriteLogInfo("Send Stage over");
        return flag;
    }

    public string unicodetogb(string text)
    {
        MatchCollection matchs = Regex.Matches(text, @"\\u([\w]{4})");
        string str = text.Replace(@"\u", "");
        char[] chArray = new char[matchs.Count];
        for (int i = 0; i < chArray.Length; i++)
        {
            chArray[i] = (char) Convert.ToInt32(str.Substring(i * 4, 4), 0x10);
        }
        return new string(chArray);
    }

    public void VisiteWebAddress(string url, string cookieStr, string refer, string acceptencoding)
    {
        try
        {
            using (HttpWebResponseUtility.CreateGetHttpResponseProxy(url, null, null, cookieStr, refer, "", this.wbproxy))
            {
                this.visitTimes = 3;
            }
        }
        catch
        {
            if (this.visitTimes <= 0)
            {
                throw new Exception("time out " + url);
            }
            this.visitTimes--;
            this.VisiteWebAddress(url, cookieStr, refer, acceptencoding);
        }
    }

    // Properties
    public BaiduSongModel songInfo { get; set; }
}

 
 

}
