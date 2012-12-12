using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Rec
    {
        private static string HIPSTER_DB_LOC = "Data Source=C:\\Users\\Kevin\\Documents\\ProjectFlannelDBs\\Hipstermatic.db;Version=3;";
        //private static string HIPSTER_DB_LOC = "Data Source=C:\\Users\\Jeff\\isrhw\\flannel_tracks\\MillionSongSubset\\AdditionalFiles\\Hipstermatic.db;Version=3;";
        private static SQLiteConnection HIPSTER_DB = new SQLiteConnection(HIPSTER_DB_LOC);

        private static List<string> getSortedArtistIds(List<string> artistIds)
        {
            List<string> sortedArtistIds = new List<string>();
            try
            {
                HIPSTER_DB.Open();
                Dictionary<int, string> rankIndex = new Dictionary<int, string>();
                foreach (string artistId in artistIds)
                {
                    string szQuery = "SELECT hipster_rank FROM hipster WHERE artist_id='" + artistId + "'";
                    SQLiteCommand command = new SQLiteCommand(szQuery, HIPSTER_DB);
                    SQLiteDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        rankIndex[Convert.ToInt32(reader[0].ToString())] = artistId;
                    }
                }
                HIPSTER_DB.Close();
                List<int> scores = rankIndex.Keys.ToList();
                scores.Sort();
                foreach (int score in scores)
                {
                    sortedArtistIds.Add(rankIndex[score]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return sortedArtistIds;
        }

        private static double equalWeightScore(List<double> scores)
        {
            double newScore = 0;
            foreach (double score in scores)
            {
                newScore += score * (1.0/scores.Count);
            }
            return newScore;
        }

        public static List<string> RankEm(List<string> artistIds)
        {
            List<string> sortedArtistIds = new List<string>();

            Dictionary<double, List<string>> scoreIndex = new Dictionary<double, List<string>>();
            foreach (string artistId in artistIds)
            {
                double score = ( Hits.GetHubScore(artistId) + (1 - Hits.GetAuthScore(artistId) ) ) / 2;
                if (scoreIndex.ContainsKey(score))
                {
                    scoreIndex[score].Add(artistId);
                }
                else
                {
                    scoreIndex[score] = new List<string>() { artistId };
                }
            }

            Dictionary<string, List<double>> artistScores = new Dictionary<string, List<double>>();
            foreach (double score in scoreIndex.Keys.ToList())
            {
                foreach (string artistId in scoreIndex[score])
                {
                    if (artistScores.ContainsKey(artistId))
                    {
                        artistScores[artistId].Add(score);
                    }
                    else
                    {
                        artistScores[artistId] = new List<double>() { score };
                    }
                }
            }

            Dictionary<double, List<string>> artistScoreIndex = new Dictionary<double, List<string>>();
            foreach (string artistId in artistScores.Keys.ToList())
            {
                double score = equalWeightScore(artistScores[artistId]);
                if (artistScoreIndex.ContainsKey(score))
                {
                    artistScoreIndex[score].Add(artistId);
                }
                else
                {
                    artistScoreIndex[score] = new List<string>() { artistId };
                }
            }

            List<double> scores = scoreIndex.Keys.ToList();
            scores.Sort();
            scores.Reverse();
            foreach (double score in scores)
            {
                foreach (string artistId in artistScoreIndex[score])
                {
                    sortedArtistIds.Add(artistId);
                }
            }

            foreach (string artistId in sortedArtistIds)
            {
                Console.WriteLine(Metadata.getArtistNameFromId(artistId));
            }

            return sortedArtistIds;
        }

        public static List<Song> GeneratePlaylist(List<string> artistNames)
        {
            // names --> ids (TODO: add matcher, e.g. beatles --> The Beatles)
            Console.WriteLine("Matching Artist Names with IDs...");
            List<string> artistIds = new List<string>();
            foreach (string artistName in artistNames)
            {
                artistIds.Add(Metadata.getArtistIdFromName(artistName));
            }
            
            // generate set of all artist and similar artist ids
            Console.WriteLine("Getting Similar Artists...");
            HashSet<string> artistIdPool = new HashSet<string>();
            foreach (string artistId in artistIds)
            {
                List<string> similarArtists = Similarity.GetSimilarArtists(artistId);
                foreach (string similarArtist in similarArtists)
                {
                    artistIdPool.Add(similarArtist);
                }
            }

            // THIS IS THE SLOW PART!!
            // sort by hub score
            Console.WriteLine("Sorting Artists...");
            List<string> sortedArtistIds = getSortedArtistIds(artistIdPool.ToList());

            // get top song from each of the top 10 hubs
            Console.WriteLine("Getting Songs...");
            List<string> topSongs = new List<string>();
            for (int i = 0; i < 10 && i < sortedArtistIds.Count; i++)
            {
                topSongs.Add(Metadata.GetTopSong(sortedArtistIds[i]));
            }

            // generate output
            List<Song> playlist = new List<Song>();
            for (int i = 0; i < topSongs.Count; i++)
            {
                playlist.Add(new Song(Metadata.GetSongTitle(topSongs[i]),Metadata.GetSongArtistName(topSongs[i])));
            }

            return playlist;
        }
    }
}
