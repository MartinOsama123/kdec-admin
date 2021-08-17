using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerUI
{
    public class SongInfo
    {
        public int id;
        public String albumName;
        public String lang;
        public String songName;

        public SongInfo()
        {
        }

        public SongInfo(string albumName, string lang, string songName)
        {
            this.albumName = albumName;
            this.lang = lang;
            this.songName = songName;
        }
    }
}
