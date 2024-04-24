using Azure;
using Microsoft.Data.Sqlite;
using projektASP.Controllers;
using System.Runtime.InteropServices;

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
            string Createsql = "CREATE TABLE Users(Username TEXT, Email TEXT, Password TEXT)";
            cmd = conn.CreateCommand();
            cmd.CommandText = Createsql;
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

        //login
        public static bool Login(string username, string password)
        {
            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();
                
            cmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = @Username AND Password = @Password";
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Password", password);

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
            SqliteConnection conn = CreateConnection();
            SqliteCommand cmd = conn.CreateCommand();

            cmd.CommandText = "INSERT INTO Users (Username, Email, Password) VALUES (@Username, @Email, @Password)";
            cmd.Parameters.AddWithValue("@Username", username);
            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Login(username, password);
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string LoginOrUser() 
        {
            string username = new HomeController.GetUsername();
            if (HomeController.GetUsername() == null)
            {
                return "visible";
            }
            else { return "hidden"; }
        }

        public static string ShowLogoutButton()
        {
            if (loggedInUser != null)
            {
                return "visible";
            }
            else { return "hidden"; }
        }

    }
}
