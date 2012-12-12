using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Similarity
    {
        private static SQLiteConnection SIMILARITY_DB = new SQLiteConnection(DBLocations.SIMILARITY_DB_LOC);

        public static List<string> GetSimilarArtists(string szArtistId)
        {
            List<string> similarArtists = new List<string>();
            string szQuery = "SELECT similar FROM similarity WHERE target='" + szArtistId + "'";
            try
            {
                SIMILARITY_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    similarArtists.Add(reader[0].ToString());
                }
                SIMILARITY_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return similarArtists;
        }

        public static List<string> getSimilarArtists(string szArtistId)
        {
            List<string> similarArtists = new List<string>();
            string szQuery = "SELECT similar FROM similarity WHERE target='" + szArtistId + "'";
            try
            {
                SIMILARITY_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    similarArtists.Add(reader[0].ToString());
                }
                SIMILARITY_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return similarArtists;
        }

        public static double getAverageNumberOfSimilarArtists()
        {
            double avg = 0;
            try
            {
                SIMILARITY_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    szQuery = "SELECT COUNT(similar) FROM similarity WHERE target='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, SIMILARITY_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            avg += Convert.ToInt32(reader[0].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                SIMILARITY_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return avg / Metadata.getNumberOfArtists();
        }
    }
}
