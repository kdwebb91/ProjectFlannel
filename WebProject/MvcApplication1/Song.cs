using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Song
    {
        public string Title;
        public string Artist;

        public Song()
        {
            Title = string.Empty;
            Artist = string.Empty;
        }

        public Song(string szTitle,
                string szArtist)
        {
            Title = szTitle;
            Artist = szArtist;
        }
    }
}
