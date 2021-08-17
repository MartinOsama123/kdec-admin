using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlayerUI
{
    public class AlbumInfo
    {
        public string albumName;
        public string imgPath;

        public AlbumInfo()
        {
        }

        public AlbumInfo(string albumName, string imgPath)
        {
            this.albumName = albumName;
            this.imgPath = imgPath;
        }
    }
}
