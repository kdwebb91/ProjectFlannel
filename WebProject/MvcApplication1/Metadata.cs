using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Flannel
{
    class Metadata
    {
        private static SQLiteConnection METADATA_DB = new SQLiteConnection(DBLocations.METADATA_DB_LOC);

        public static string GetSongArtistName(string szSongId)
        {
            string szArtistName = string.Empty;
            string szQuery = "SELECT artist_name FROM songs WHERE song_id='" + szSongId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistName = reader["artist_name"].ToString();
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szArtistName;
        }

        public static string GetSongTitle(string szSongId)
        {
            string szSongTitle = string.Empty;
            string szQuery = "SELECT * FROM songs WHERE song_id='" + szSongId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szSongTitle = reader["title"].ToString();
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szSongTitle;
        }

        public static string GetTopSong(string szArtistId)
        {
            string szSongName = string.Empty;
            double maxRating = 0;
            string szQuery = "SELECT * FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // NEED TO ADD song_hotttnesss TO subset_track_metadata.db
                    double songHotttnesss = 1; // Convert.ToDouble(reader["song_hotttnesss"].ToString());
                    if (songHotttnesss > maxRating)
                    {
                        szSongName = reader["song_id"].ToString();
                        maxRating = songHotttnesss;
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szSongName;
        }

        public static string getArtistIdFromName(string szArtistName)
        {
            string szArtistId = string.Empty;
            string szQuery = "SELECT artist_id FROM songs WHERE artist_name='" + szArtistName + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistId = reader["artist_id"].ToString();
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szArtistId;
        }

        public static string getArtistNameFromId(string szArtistId)
        {
            string szArtistName = string.Empty;
            string szQuery = "SELECT artist_name FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    szArtistName = reader["artist_name"].ToString();
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return szArtistName;
        }

        public static int getNumberOfArtists()
        {
            int numberOfArtists = 0;
            string szQuery = "SELECT COUNT(artist_id) FROM artists";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numberOfArtists = Convert.ToInt32(reader[0].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numberOfArtists;
        }

        public static void getMostFamiliarArtist()
        {
            List<double> fam = new List<double>() { 0, 0, 0 };
            List<string> artist = new List<string>() { "", "", "" };
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    szQuery = "SELECT artist_familiarity FROM songs WHERE artist_id='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            double temp = Convert.ToDouble(reader[0].ToString());
                            if (temp > fam[0])
                            {
                                fam[0] = temp;
                                fam.Sort();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                for (int i = 0; i < fam.Count; i++)
                {
                    szQuery = "SELECT artist_id FROM songs WHERE artist_familiarity='" + fam[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string temp = reader["artist_id"].ToString();
                            if (artist.Contains(temp))
                            {
                                continue;
                            }
                            else
                            {
                                artist[i] = temp;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            for (int i = fam.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(getArtistNameFromId(artist[i]));
                Console.WriteLine(fam[i]);
            }
        }

        public static void getLeastFamiliarArtist()
        {
            List<double> fam = new List<double>() { 1, 1, 1 };
            List<string> artist = new List<string>() { "", "", "" };
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    szQuery = "SELECT artist_familiarity FROM songs WHERE artist_id='" + artists[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            double temp = Convert.ToDouble(reader[0].ToString());
                            if (temp < fam[fam.Count - 1])
                            {
                                fam[fam.Count - 1] = temp;
                                fam.Sort();
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Artist " + artists[i] + " threw an exception.");
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                for (int i = 0; i < fam.Count; i++)
                {
                    szQuery = "SELECT artist_id FROM songs WHERE artist_familiarity='" + fam[i] + "'";
                    try
                    {
                        command = new SQLiteCommand(szQuery, METADATA_DB);
                        reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            string temp = reader["artist_id"].ToString();
                            if (artist.Contains(temp))
                            {
                                continue;
                            }
                            else
                            {
                                artist[i] = temp;
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                        continue;
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            for (int i = fam.Count - 1; i >= 0; i--)
            {
                Console.WriteLine(getArtistNameFromId(artist[i]));
                Console.WriteLine(fam[i]);
            }
        }

        public static int getNumberOfSongs(string szArtistId)
        {
            int numSongs = 0;
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                string szQuery = "SELECT COUNT(song_id) FROM songs WHERE artist_id='" + szArtistId + "'";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    numSongs = Convert.ToInt32(reader[0].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return numSongs;
        }

        public static double getNumberOfSongsPerArtist()
        {
            double avgNumSongs = 0;
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                List<string> artists = new List<string>();
                string szQuery = "SELECT artist_id FROM artists";
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artists.Add(reader["artist_id"].ToString());
                }
                for (int i = 0; i < artists.Count; i++)
                {
                    avgNumSongs += getNumberOfSongs(artists[i]);
                }
                avgNumSongs /= artists.Count;
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return avgNumSongs;
        }

        public static double GetArtistFamiliarity(string szArtistId)
        {
            double artist_familiarity = 0;
            string szQuery = "SELECT artist_familiarity FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    artist_familiarity = Convert.ToDouble(reader["artist_familiarity"].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return artist_familiarity;
        }

        public static List<string> SortByArtistFamiliarity(List<string> artistIds)
        {
            List<string> sortedArtistIds = new List<string>();
            Dictionary<double, List<string>> scoreIndex = new Dictionary<double, List<string>>();
            foreach (string artistId in artistIds)
            {
                double fam = GetArtistFamiliarity(artistId);
                if (scoreIndex.ContainsKey(fam))
                {
                    scoreIndex[fam].Add(artistId);
                }
                else
                {
                    scoreIndex[fam] = new List<string>() { artistId };
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

        public static double GetArtistHotttness(string szArtistId)
        {
            double artist_hotttnesss = 0;
            string szQuery = "SELECT artist_hotttnesss FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    artist_hotttnesss = Convert.ToDouble(reader["artist_hotttnesss"].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return artist_hotttnesss;
        }
        /*
        public static List<Song> GetSongsFromArtist(string szArtistId)
        {
            List<Song> songs = new List<Song>();
            string szQuery = "SELECT * FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    songs.Add(new Song(reader["track_id"].ToString(),
                                        reader["title"].ToString(),
                                        reader["release"].ToString(),
                                        reader["duration"].ToString(),
                                        reader["year"].ToString(),
                                        new Artist(reader["artist_id"].ToString(),
                                            reader["artist_name"].ToString(),
                                            Convert.ToDouble(reader["artist_familiarity"].ToString()),
                                            Convert.ToDouble(reader["artist_hotttnesss"].ToString()))));
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return songs;
        }

        public static Artist CreateArtist(string szArtistId)
        {
            Artist artist = null;
            string szQuery = "SELECT * FROM songs WHERE artist_id='" + szArtistId + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                METADATA_DB.Close();
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    artist = new Artist(reader["artist_id"].ToString(),
                                            reader["artist_name"].ToString(),
                                            Convert.ToDouble(reader["artist_familiarity"].ToString()),
                                            Convert.ToDouble(reader["artist_hotttnesss"].ToString()));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return artist;
        }
        */
        public static HashSet<string> GetAllArtistIds()
        {
            HashSet<string> artistIds = new HashSet<string>();
            string szQuery = "SELECT artist_id FROM songs";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    artistIds.Add(reader["artist_id"].ToString());
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return artistIds;
        }

        public static HashSet<string> GetAllArtistNames()
        {
            HashSet<string> artistIds = GetAllArtistIds();
            HashSet<string> artistNames = new HashSet<string>();
            
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                foreach (string artistId in artistIds)
                {
                    try
                    {
                        string szQuery = "SELECT artist_name FROM songs WHERE artist_id='" + artistId + "'";
                        SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                        SQLiteDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            artistNames.Add(reader["artist_name"].ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return artistNames;
        }

        public static void PrintMatchingArtists(string szArtistName)
        {
            HashSet<string> artistNames = Metadata.GetAllArtistNames();
            HashSet<string> possibleMatches = new HashSet<string>();
            bool matchFound = false;
            string query = szArtistName;
            string matchingArtist = string.Empty;
            foreach (string artistName in artistNames)
            {
                if (artistName.Equals(query, StringComparison.OrdinalIgnoreCase))
                {
                    matchingArtist = artistName;
                    matchFound = true;
                    break;
                }
                else if (artistName.ToLower().Contains(query))
                {
                    possibleMatches.Add(artistName);
                }
            }
            if (matchFound == false && possibleMatches.Count == 1)
            {
                matchingArtist = possibleMatches.FirstOrDefault();
                matchFound = true;
            }
            if (matchFound == true)
            {
                Console.WriteLine(query + " == " + matchingArtist);
            }
            else if (possibleMatches.Count > 1)
            {
                Console.WriteLine(query + " possible matches:");
                foreach (string match in possibleMatches)
                {
                    Console.WriteLine("\t" + match);
                }
            }
            else
            {
                Console.WriteLine("No matches found for " + query + "!");
            }
        }

        public static bool IsInDB(string szArtistName)
        {
            bool isInDB = false;
            string szQuery = "SELECT artist_id FROM songs WHERE artist_name='" + szArtistName + "'";
            try
            {
                METADATA_DB.Close();METADATA_DB.Open();
                SQLiteCommand command = new SQLiteCommand(szQuery, METADATA_DB);
                SQLiteDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    if (!string.Equals(reader["artist_id"].ToString(), string.Empty))
                    {
                        isInDB = true;
                    }
                }
                METADATA_DB.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return isInDB;
        }
    }
}
