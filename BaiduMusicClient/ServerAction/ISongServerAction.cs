using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BaiduMusicClient.Models;

namespace BaiduMusicClient.ServerAction
{
    public interface ISongServerAction
    {
        // Methods
        List<BaiduSongModel> GetSongInfoFromServer();
        bool IsIpRepeat();
        void UpdateSuccess(string songID, string pcName);
    }

 

 

}
