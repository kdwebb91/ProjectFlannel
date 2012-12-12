using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Hits
    {
        private static SQLiteConnection HITS_DB = new SQLiteConnection(DBLocations.HITS_DB_LOC);

        public static List<string> SortByHubScore(List<string> artistIds)
        {
            List<string> sortedArtistIds = new List<string>();
            Dictionary<double, List<string>> scoreIndex = new Dictionary<double, List<string>>();
            foreach (string artistId in artistIds)
            {
                double hubScore = GetHubScore(artistId);
                if (scoreIndex.ContainsKey(hubScore))
                {
                    scoreIndex[hubScore].Add(artistId);
                }
                else
                {
                    scoreIndex[hubScore] = new List<string>(){artistId};
                }
            }
            List<double> scores = scoreIndex.Keys.ToList();
            scores.Sort();
            scores.Reverse();
            foreach (double score in scores)
            {
                foreach (string artistId in scoreIndex[score])
                {
                    sortedArtistIds.Add(artistId);
                }
            }
            return sortedArtistIds;
        }

        public static List<string> SortByAuthScore(List<string> artistIds)
        {
            List<string> sortedArtistIds = new List<string>();
            Dictionary<double, List<string>> scoreIndex = new Dictionary<double, List<string>>();
            foreach (string artistId in artistIds)
            {
                double authScore = GetAuthScore(artistId);
                if (scoreIndex.ContainsKey(authScore))
                {
                    scoreIndex[authScore].Add(artistId);
                }
                else
                {
                    scoreIndex[authScore] = new List<string>() { artistId };
                }
            }
            var scores = scoreIndex.Keys.ToList();
            scores.Sort();
            scores.Reverse();
            foreach (double score in scores)
            {
                foreach (string artistId in scoreIndex[score])
                {
                    sortedArtistIds.Add(artistId);
                }
            }
            return sortedArtistIds;
        }

        public static double GetHubScore(string artistId)
        {
            double score = 0;
            string szQuery = "SELECT hub FROM hits WHERE artist_id='" + artistId + "'";
            try
            {
                HITS_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, HITS_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    score =  Convert.ToDouble(reader["hub"].ToString());
                }
                HITS_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return score;
        }

        public static double GetAuthScore(string artistId)
        {
            double score = 0;
            string szQuery = "SELECT auth FROM hits WHERE artist_id='" + artistId + "'";
            try
            {
                HITS_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, HITS_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    score = Convert.ToDouble(reader["auth"].ToString());
                }
                HITS_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return score;
        }
    }
}
