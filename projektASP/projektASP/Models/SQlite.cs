﻿using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using projektASP.Controllers;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;
using System;
using System.Reflection;
using System.Collections.Generic;

namespace projektASP.Models
{
    public class SQlite
    {

        //Öppnar kontakten med databasen
        public static SqliteConnection CreateConnection()
        {

            SqliteConnection sqlite_conn;
            // Create a new database connection:
            sqlite_conn = new SqliteConnection("Data Source=database.db; "); //Version = 3; New = True; Compress = True; 
            // Open the connection:
            try
            {
                sqlite_conn.Open();
            }
            catch (Exception ex)
            {

            }
            return sqlite_conn;
        }

        //Skapar en ny tabell i databasen (används inte)
        public static void CreateTable(SqliteConnection conn)
        {
            SqliteCommand cmd;
            string Createsql = "CREATE TABLE Votes(User TEXT, Artist TEXT)";
            cmd = conn.CreateCommand();
            cmd.CommandText = Createsql;
            cmd.ExecuteNonQuery();
        }


        //Skapa nytt fält (används inte)
        public static void CreateField(SqliteConnection conn)
        {
            SqliteCommand cmd;
            string Createsql = "ALTER TABLE Users ADD Avatar INT";
            cmd = conn.CreateCommand();
            cmd.CommandText = Createsql;
            cmd.ExecuteNonQuery();
        }

        //Tömmer en tabell (ANVÄND INTE OM DU INTE ÄR ISAK)
        public static void EmptyTable()
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "DELETE FROM Votes";
            cmd.ExecuteNonQuery();
		}



        //Lägger till en sidbesökares IP och nuvarande DateTime i Visitors i databasen
        public static void AddVisitor(string userIP)
        {
            SqliteConnection conn = SQlite.CreateConnection();

            //Skriver och skapar Queryn
            SqliteCommand cmd = conn.CreateCommand();
            cmd.CommandText = "INSERT INTO Visitors(IP, VisitationDate) VALUES(@IP, @VisitationDate)"; ;

            //Skapar parametrarna (variablerna)
            SqliteParameter IPparam = cmd.CreateParameter();
            SqliteParameter Dateparam = cmd.CreateParameter();
            IPparam.ParameterName = "IP";
            IPparam.Value = userIP;
            Dateparam.ParameterName = "VisitationDate";
            Dateparam.Value = @DateTime.Now;

            //Lägger in parametrarna
            cmd.Parameters.Add(IPparam);
            cmd.Parameters.Add(Dateparam);

            //Kör Queryn med parametrar
            cmd.ExecuteNonQuery();
        }

        //Returnerar totala antalet besökare (entrys i visitor)
        public static long countTotalVisitors()
        {
            SqliteConnection conn = SQlite.CreateConnection();
            SqliteCommand cmd;
            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Visitors";
            return (long)cmd.ExecuteScalar();
        }

        //Returnerar antalet unika IP-adresser
        public static long countIndividualVisitors()
        {
            SqliteConnection conn = SQlite.CreateConnection();
            SqliteCommand cmd;
            cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT COUNT(DISTINCT IP) FROM Visitors";
            return (long)cmd.ExecuteScalar();
        }

        public static string HashPassword(string username, string password)
        {
			//Genererar ett salt utifrån användarnamnet (mycket säkert jag lovar)
			byte[] salt = Encoding.ASCII.GetBytes(username); 

			// derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
			string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password!,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 100000,
				numBytesRequested: 256 / 8));

            return hashedPassword;
		}



        //Login
        public static bool Login(string username, string password)
        {
            string hashedPassword = HashPassword(username, password);

            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();
                
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            int count = Convert.ToInt32(cmd.ExecuteScalar());

            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Registrera användare
        public static bool Register(string username, string email, string password, int avatar)
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand command = conn.CreateCommand();

			command.CommandText = "SELECT * FROM Users WHERE Username = @Username";
			command.Parameters.AddWithValue("@Username", username);
			var found = command.ExecuteScalar();
            if(found != null)
            {
                Console.WriteLine("Användaren finns redan");
                return false;
            }


			string hashedPassword = HashPassword(username, password);

			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "INSERT INTO Users (Username, Email, Password, Avatar) VALUES (@Username, @Email, @Password, @Avatar)";
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);
            cmd.Parameters.AddWithValue("@Avatar", avatar);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Returnerar att ett element ska döljas om man är inloggad
        public static string HiddenIfLoggedIn(HttpContext httpContext) 
        {
            if (SQlite.GetUsername(httpContext) == null)
            {
                return "block";
            }
            else { return "none"; }
        }

		//Returnerar att ett element ska visas om man är inloggad
		public static string VisibleIfLoggedIn(HttpContext httpContext)
        {

            if (SQlite.GetUsername(httpContext) != null)
            {
                return "block";
            }
            else { return "none"; }
        }

        //Returnerar användarnamnet sparat i en cookie
        public static string GetUsername(HttpContext httpContext)
        {
            return httpContext.Request.Cookies["Username"];
        }

        //Returnerar antalet foruminlägg
        public static long GetTotalPostAmount(string forum)
        {
            SqliteConnection conn = CreateConnection();
			SqliteCommand getIndexCmd = conn.CreateCommand();

            getIndexCmd.CommandText = $"SELECT COUNT(*) FROM {forum}";
            return (long)getIndexCmd.ExecuteScalar();

        }

        //Skapar ett foruminlägg
        public static bool CreatePost(string forum, string title, string text, string username)
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();
			

			cmd.CommandText = $"INSERT INTO {forum} (Postindex, Time, User, Title, Posttext) VALUES (@Postindex, @Time, @User, @Title, @Posttext)";
            cmd.Parameters.AddWithValue("@Postindex", GetTotalPostAmount(forum));
            cmd.Parameters.AddWithValue("@Time", DateTime.Now);
			cmd.Parameters.AddWithValue("@User", username);
			cmd.Parameters.AddWithValue("@Title", title);
			cmd.Parameters.AddWithValue("@Posttext", text);

            int rowsAffected = cmd.ExecuteNonQuery();

			return rowsAffected > 0;
        }
        
        //Returnerar ett inläggs tid
        public static string GetPostTime(string forum, int index)
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = $"SELECT datetime(Time) FROM {forum} WHERE Postindex = @Postindex";
			cmd.Parameters.AddWithValue("@Postindex", index);

            if(cmd.ExecuteScalar() != null)
            {
                return (string)cmd.ExecuteScalar();

            }
            else
            {
				return ("hmmm");
			}

		}

        //Posttext/User/Title
        public static string GetPostInfo(string forum, int index, string info)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = $"SELECT {info} FROM {forum} WHERE Postindex = @Postindex";
            cmd.Parameters.AddWithValue("@Postindex", index);
            return (string)cmd.ExecuteScalar();
        }

        //Returnerar ett inläggs lajks
        public static long GetPostLikes(int index)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT COUNT(*) FROM Likes WHERE Postindex = @Postindex";
			cmd.Parameters.AddWithValue("@Postindex", index);
            return (long)cmd.ExecuteScalar();
        }

        //Returnerar om usern har likat en post
        public static bool GetIfUserLiked(int index, string username)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT COUNT(*) FROM Likes WHERE Postindex = @Postindex AND User = @User";
			cmd.Parameters.AddWithValue("@Postindex", index);
            cmd.Parameters.AddWithValue("@User", username);

            long rowsFound = (long)cmd.ExecuteScalar();
            return rowsFound > 0;
        }

        public static string GetDailyMusic()
        {
            String[] discs = {"Cat", "Mall", "Mellohi", "Stal", "Strad", "Wait", "Chirp", "Pigstep", "Otherside", "Relic"};
            string dayString = @DateTime.Now.ToString("dd");
            char dayChar = dayString[1];
            int dayInt = Int32.Parse(dayChar.ToString());
            return discs[dayInt];
        }

        //Skicka in -1 för denna användare, index för en posts användare
		public static string GetPFP(HttpContext httpContext, string forum, int index)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "SELECT Avatar FROM Users WHERE Username = @Username";

            if (index == -1)
            {
                string name = GetUsername(httpContext);
                if (name != null)
                {
                    cmd.Parameters.AddWithValue("@Username", name);
                    return $"/Pictures/Login-Register/{(long)cmd.ExecuteScalar()}.jpg";
                }
                else { return ""; }
				
            }
            else
            {
				cmd.Parameters.AddWithValue("@Username", GetPostInfo(forum, index, "User"));
			}

            long avatarIndex;
            if(forum == "Forum") { 
                avatarIndex = (long)cmd.ExecuteScalar(); 
            }
            else {
				Random rnd = new Random();
                avatarIndex = rnd.Next(1, 5);
            }
                
			return $"/Pictures/Login-Register/{avatarIndex}.jpg";
		}

        //Sökfunktionen
        public static bool PostSearch(string forum, string searchWord, int index)
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

            searchWord = $"%{searchWord}%";

			cmd.Parameters.AddWithValue("@Search", searchWord);
			cmd.Parameters.AddWithValue("@Postindex", index);

            string[] categories = ["Title", "User", "Posttext"];
            
            foreach(string category in categories)
            {
				cmd.CommandText = $"SELECT COUNT(*) FROM {forum} WHERE {category} LIKE @Search AND Postindex = @Postindex";
				
                if ((long)cmd.ExecuteScalar() > 0)
				{
					Console.WriteLine(index);
					return true;
				}
			}
            return false;
		}

        //Returnerar den nuvarande söktermen
        public static string GetSearchCookie(HttpContext httpContext)
        {
			return httpContext.Request.Cookies["Search"];
		}

        //Likear inlägg i databasen
        public static bool LikePost(int postIndex, string username)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO Likes (Postindex, User) VALUES (@Postindex, @User)";
			cmd.Parameters.AddWithValue("@Postindex", postIndex);
            cmd.Parameters.AddWithValue("@User", username);

            int rowsAffected = cmd.ExecuteNonQuery();

            return rowsAffected > 0;
        }

        //Returnerar om en användare har röstat
		public static bool GetIfUserVoted(string username)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "SELECT COUNT(*) FROM Votes WHERE User = @User";
			cmd.Parameters.AddWithValue("@User", username);

			long rowsFound = (long)cmd.ExecuteScalar();
			return rowsFound > 0;
		}

        //Lägger inskickade användarens röst på inskickade artisten
        public static void AddVote(string artist, string username) 
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "INSERT INTO Votes(Artist, User) VALUES(@Artist, @User)";
			cmd.Parameters.AddWithValue("@Artist", artist);
			cmd.Parameters.AddWithValue("@User", username);

            cmd.ExecuteNonQuery();
		}

        //Räknar en artists röster
		public static string CountVote(string artist)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "SELECT COUNT(*) FROM Votes WHERE ARTIST = @Artist";
			cmd.Parameters.AddWithValue("@Artist", artist);

            if (cmd.ExecuteScalar != null)
            {
                return cmd.ExecuteScalar().ToString();
            } else return "0";
		}

		public static void ChangeVote(string artist, string username)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "UPDATE Votes SET Artist = @Artist WHERE User = @User";
			cmd.Parameters.AddWithValue("@Artist", artist);
			cmd.Parameters.AddWithValue("@User", username);

			cmd.ExecuteNonQuery();
		}
	}
}
