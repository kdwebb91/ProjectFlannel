using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Artist
    {
        public string ArtistId;
        public string ArtistName;

        public Artist()
        {
            ArtistId = string.Empty;
            ArtistName = string.Empty;
        }

        public Artist(string szArtistId,
                string szArtistName)
        {
            ArtistId = szArtistId;
            ArtistName = szArtistName;
        }
    }
}
