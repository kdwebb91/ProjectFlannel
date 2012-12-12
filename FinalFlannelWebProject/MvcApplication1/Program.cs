using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Data.SQLite;

namespace Flannel
{
    class Program
    {
        public static List<string> Artists = new List<string>()
        {
            "Incubus",
            "Aerosmith",
            "Stone Temple Pilots",
            "Foo Fighters",
            "The Smashing Pumpkins",
            "Nirvana",
            "Mastodon",
            "Red Hot Chili Peppers",
            "The Who",
            "Metallica"
        };

        /* http://en.wikipedia.org/wiki/List_of_popular_music_genres */
        public static List<string> PopularMusicGenres = new List<string>()
        {
            "african",
            "asian",
            "avant-garde",
            "blues",
            "brazilian",
            "comedy",
            "country",
            "easy listening",
            "electronic",
            "modern folk",
            "hip hop",
            "jazz",
            "latin american",
            "pop",
            "r&b",
            "rock",
            "ska"
        };

        public static bool test_FindArtistInDB(List<string> artists)
        {
            bool allArtistsFound = true;
            for (int i = 0; i < artists.Count; i++)
            {
                string artist_id = Metadata.getArtistIdFromName(artists[i]);
                if (string.Equals(artist_id, string.Empty))
                {
                    allArtistsFound = false;
                    Console.Write("ERROR artist not found:\t" + artists[i]);
                }
            }
            return allArtistsFound;
        }

        public static void printHITS(List<string> artists)
        {
            Dictionary<double, string> hubs = new Dictionary<double, string>();
            Dictionary<double, string> auths = new Dictionary<double, string>();
            
            List<double> hubScores = new List<double>();
            List<double> authScores = new List<double>();

            for (int i = 0; i < Artists.Count; i++)
            {
                double hubScore = Hits.GetHubScore(Metadata.getArtistIdFromName(Artists[i]));
                double authScore = Hits.GetAuthScore(Metadata.getArtistIdFromName(Artists[i]));
                hubScores.Add(hubScore);
                authScores.Add(authScore);
                hubs[hubScore] = Artists[i];
                auths[authScore] = Artists[i];
            }

            Console.WriteLine("\nHUBS:\n");
            hubScores.Sort();
            for (int i = hubScores.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(hubs[hubScores[i]] + " (" + hubScores[i] + ")");
            }
            Console.WriteLine("\nAUTHS:\n");
            authScores.Sort();
            for (int i = authScores.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(auths[authScores[i]] + " (" + authScores[i] + ")");
            }
        }

        public static void printFamiliarities(List<string> artists)
        {
            Dictionary<double, string> fams = new Dictionary<double, string>();
            
            List<double> famValues = new List<double>();

            for (int i = 0; i < Artists.Count; i++)
            {
                double famValue = Metadata.GetArtistFamiliarity(Metadata.getArtistIdFromName(Artists[i]));
                famValues.Add(famValue);
                fams[famValue] = Artists[i];
            }

            Console.WriteLine("\nFamiliarity:\n");
            famValues.Sort();
            for (int i = famValues.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(fams[famValues[i]] + " (" + famValues[i] + ")");
            }
        }

        public static void printHotttnessses(List<string> artists)
        {
            Dictionary<double, string> hotttnessses = new Dictionary<double, string>();

            List<double> hotttnesssValues = new List<double>();

            for (int i = 0; i < Artists.Count; i++)
            {
                double hotttnesss = Metadata.GetArtistHotttness(Metadata.getArtistIdFromName(Artists[i]));
                hotttnesssValues.Add(hotttnesss);
                hotttnessses[hotttnesss] = Artists[i];
            }
            
            Console.WriteLine("\nHotttnesss:\n");
            hotttnesssValues.Sort();
            for (int i = hotttnesssValues.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(hotttnessses[hotttnesssValues[i]] + " (" + hotttnesssValues[i] + ")");
            }
        }

        private static bool writeRanksToSQLiteDB(List<string> artistIds)
        {
            bool success = true;
            try
            {
                string path = "Hipstermatic.db";
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                SQLiteConnection.CreateFile(path);
                SQLiteConnection db = new SQLiteConnection(("Data Source=" + path + ";Version=3;").ToString());
                db.Open();
                string szQuery = "CREATE TABLE hipster (artist_id text, hipster_rank int)";
                SQLiteCommand command = new SQLiteCommand(szQuery, db);
                command.ExecuteReader();
                for (int i = 0; i < artistIds.Count; i++)
                {
                    szQuery = "INSERT INTO hipster VALUES ('" + artistIds[i] + "'," + (i+1).ToString() + ")";
                    command = new SQLiteCommand(szQuery, db);
                    command.ExecuteReader();
                }

                szQuery = "SELECT * FROM hipster";
                command = new SQLiteCommand(szQuery, db);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.Write(reader["hipster_rank"].ToString() + " - ");
                    Console.Write(Metadata.getArtistNameFromId(reader["artist_id"].ToString()) + "\n");
                }

                db.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                success = false;
            }
            return success;
        }

        static void Main(string[] args)
        {
            //writeRanksToSQLiteDB(Rec.RankEm(Metadata.GetAllArtistIds().ToList()));

            List<Song> playlist = Rec.GeneratePlaylist(Artists);
            for (int i = 0; i < playlist.Count; i++)
            {
                Console.WriteLine((i+1).ToString() + ".\t" + playlist[i].Title + " - " + playlist[i].Artist);
            }
            Console.Read();
            
            /*
            SQLiteConnection db = new SQLiteConnection("Data Source=i_hope_this_works.db;Version=3;");
            string szQuery = "SELECT sql FROM sqlite_master WHERE tbl_name='songs' AND type='table'";
            //string szQuery = "SELECT song_hotttness FROM songs";
            try
            {
                db.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, db);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader[0].ToString());
                }
                db.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }*/
            Console.Read();
        }
    }
}
