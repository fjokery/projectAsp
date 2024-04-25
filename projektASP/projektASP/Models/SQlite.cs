using Microsoft.AspNetCore.Http;
using Microsoft.Data.Sqlite;
using projektASP.Controllers;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
            string Createsql = "CREATE TABLE Forum(Postindex INTEGER, Time DATETIME, User TEXT, Title TEXT, Posttext TEXT)";
            cmd = conn.CreateCommand();
            cmd.CommandText = Createsql;
            cmd.ExecuteNonQuery();
        }

        //Tömmer en tabell (ANVÄND INTE)
        public static void EmptyTable()
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "DELETE FROM Forum";
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

        public static string HashPassword(string password)
        {
			// Generate a 128-bit salt using a sequence of
			// cryptographically strong random bytes.
			byte[] salt = 1; // divide by 8 to convert bits to bytes

			// derive a 256-bit subkey (use HMACSHA256 with 100,000 iterations)
			string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
				password: password!,
				salt: salt,
				prf: KeyDerivationPrf.HMACSHA256,
				iterationCount: 100000,
				numBytesRequested: 256 / 8));

            return hashedPassword;
		}



        //login
        public static bool Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);

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

        //registerara anv'ndare
        public static bool Register(string username, string email, string password)
        {
            string hashedPassword = HashPassword(password);

			SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", hashedPassword);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Login(username, hashedPassword);
                return true;
            }
            else
            {
                return false;
            }
        }

        //Returnerar om Loginknappen ska visas
        public static string LoginOrUser(HttpContext httpContext) 
        {
            if (SQlite.GetUsername(httpContext) == null)
            {
                return "visible";
            }
            else { return "hidden"; }
        }

        //Returnerar om Logoutknappen ska visas
        public static string ShowLogoutButton(HttpContext httpContext)
        {

            if (SQlite.GetUsername(httpContext) != null)
            {
                return "visible";
            }
            else { return "hidden"; }
        }

        //Returnerar användarnamnet sparat i en cookie
        public static string GetUsername(HttpContext httpContext)
        {
            return httpContext.Request.Cookies["Username"];
        }

        //Returnerar antalet foruminlägg
        public static long GetTotalPostAmount()
        {
            SqliteConnection conn = CreateConnection();
			SqliteCommand getIndexCmd = conn.CreateCommand();

            getIndexCmd.CommandText = "SELECT COUNT(*) FROM Forum";
            return (long)getIndexCmd.ExecuteScalar();

        }

        //Skapar ett foruminlägg
        public static bool CreatePost(string title, string text, string username)
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();
			

			cmd.CommandText = "INSERT INTO Forum (Postindex, Time, User, Title, Posttext) VALUES (@Postindex, @Time, @User, @Title, @Posttext)";
            cmd.Parameters.AddWithValue("@Postindex", GetTotalPostAmount());
            cmd.Parameters.AddWithValue("@Time", DateTime.Now);
			cmd.Parameters.AddWithValue("@User", username);
			cmd.Parameters.AddWithValue("@Title", title);
			cmd.Parameters.AddWithValue("@Posttext", text);

			int rowsAffected = cmd.ExecuteNonQuery();

			return rowsAffected > 0;
        }
        
        //Returnerar ett inläggs tid
        public static string GetPostTime(int index)
        {
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "SELECT datetime(Time) FROM Forum WHERE Postindex = @Postindex";
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

		//Returnerar ett inläggs användarnamn
		public static string GetPostUser(int index)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "SELECT User FROM Forum WHERE Postindex = @Postindex";
			cmd.Parameters.AddWithValue("@Postindex", index);
			return (string)cmd.ExecuteScalar();
		}

		//Returnerar ett inläggs titel
		public static string GetPostTitle(int index)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "SELECT Title FROM Forum WHERE Postindex = @Postindex";
			cmd.Parameters.AddWithValue("@Postindex", index);
			return (string)cmd.ExecuteScalar();
		}

		//Returnerar ett inläggs brödtext
		public static string GetPostText(int index)
		{
			SqliteConnection conn = CreateConnection();
			SqliteCommand cmd = conn.CreateCommand();

			cmd.CommandText = "SELECT Posttext FROM Forum WHERE Postindex = @Postindex";
			cmd.Parameters.AddWithValue("@Postindex", index);
			return (string)cmd.ExecuteScalar();
		}

	}
}
