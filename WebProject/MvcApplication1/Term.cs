using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Term
    {
        private static SQLiteConnection TERM_DB = new SQLiteConnection(DBLocations.TERM_DB_LOC);

        public static List<string> GetGenresFromArtist(string szArtistId)
        {
            List<string> artistGenres = new List<string>();
            string szQuery = "SELECT term FROM artist_term WHERE artist_id='" + szArtistId + "'";
            try
            {
                TERM_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artistGenres.Add(reader[0].ToString());
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return artistGenres;
        }

        public static List<string> GetArtistsFromGenre(string szGenre)
        {
            List<string> genreArtists = new List<string>();
            string szQuery = "SELECT artist_id FROM artist_term WHERE term='" + szGenre + "'";
            try
            {
                TERM_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genreArtists.Add(reader[0].ToString());
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return genreArtists;
        }

        public static int getNumberOfGenres()
        {
            int numberOfGenres = 0;
            string szQuery = "SELECT COUNT(term) FROM terms";
            try
            {
                TERM_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numberOfGenres = Convert.ToInt32(reader[0].ToString());
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfGenres;
        }

        public static int getNumberOfMainGenres()
        {
            int numberOfGenres = 0;
            try
            {
                TERM_DB.Open();
                List<string> genres = new List<string>();
                string szQuery = "SELECT term FROM terms";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genres.Add(reader["term"].ToString());
                }
                numberOfGenres = genres.Count;
                for (int i = 0; i < genres.Count; i++)
                {
                    int genLen = genres[i].Length;
                    szQuery = "SELECT COUNT(term) FROM terms WHERE SUBSTR(term,1," + genLen + ")='" + genres[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, TERM_DB);
                        reader = command.ExecuteReader();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Genre " + genres[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                    if (reader.Read())
                    {
                        int count = Convert.ToInt32(reader[0].ToString());
                        numberOfGenres -= (count - 1);
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfGenres;
        }

        public static double getGenresPerArtist()
        {
            int genresPerArtist = 0;
            try
            {
                TERM_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    int genLen = artists[i].Length;
                    szQuery = "SELECT COUNT(term) FROM artist_term WHERE artist_id='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, TERM_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            genresPerArtist += Convert.ToInt32(reader[0].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Convert.ToDouble(genresPerArtist) / Metadata.getNumberOfArtists();
        }

        public static double getArtistsPerGenre()
        {
            int artistsPerGenre = 0;
            try
            {
                TERM_DB.Open();
                List<string> genres = new List<string>();
                string szQuery = "SELECT term FROM terms";
                SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    genres.Add(reader["term"].ToString());
                }
                for (int i = 0; i < genres.Count; i++)
                {
                    int genLen = genres[i].Length;
                    szQuery = "SELECT COUNT(artist_id) FROM artist_term WHERE term='" + genres[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, TERM_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            artistsPerGenre += Convert.ToInt32(reader[0].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Genre " + genres[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return Convert.ToDouble(artistsPerGenre) / getNumberOfGenres();
        }

        private static Dictionary<string, int> getData()
        {
            Dictionary<string, int> data = new Dictionary<string, int>();
            try
            {
                TERM_DB.Open();
                for (int i = 0; i < Program.PopularMusicGenres.Count; i++)
                {
                    string szQuery = "SELECT COUNT(artist_id) FROM artist_term WHERE term='" + Program.PopularMusicGenres[i] + "'";
                    try
                    {
                        SQLiteCommand command = new SQLiteCommand(szQuery, TERM_DB);
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            data.Add(Program.PopularMusicGenres[i], Convert.ToInt32(reader[0].ToString()));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Genre " + Program.PopularMusicGenres[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                TERM_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return data;
        }

        public static void testGetGenreArtists()
        {
            Console.WriteLine("\n-----------------------");
            Console.WriteLine("GET GENRE ARTISTS");
            Console.WriteLine("-----------------------");
            while (true)
            {
                Console.Write("\nEnter Genre:\t");
                string szGenre = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(szGenre))
                {
                    break;
                }
                List<string> genreArtists = GetArtistsFromGenre(szGenre);
                Console.WriteLine('\n' + "The genre '" + szGenre + "' includes to the following " + genreArtists.Count.ToString() + " artist(s):");
                for (int i = 0; i < genreArtists.Count; i++)
                {
                    Console.WriteLine(Metadata.getArtistNameFromId(genreArtists[i]));
                }
            }
        }

        public static void testGetArtistGenres()
        {
            Console.WriteLine("\n-----------------------");
            Console.WriteLine("GET ARTIST GENRES");
            Console.WriteLine("-----------------------");
            while (true)
            {
                Console.Write("\nEnter Artist Name:\t");
                string szArtistName = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(szArtistName))
                {
                    break;
                }
                string szArtistId = Metadata.getArtistIdFromName(szArtistName);
                if (string.IsNullOrWhiteSpace(szArtistId))
                {
                    Console.WriteLine("ERROR: Artist not found!");
                    continue;
                }
                List<string> artistGenres = GetGenresFromArtist(szArtistId);
                Console.WriteLine('\n' + szArtistName + " belongs to the following " + artistGenres.Count.ToString() + " genre(s):");
                for (int i = 0; i < artistGenres.Count; i++)
                {
                    Console.WriteLine(artistGenres[i]);
                }
            }
        }
    }
}
