using Microsoft.Data.Sqlite;

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

            SqliteCommand sqlite_cmd;
            string Createsql = "CREATE TABLE Visitors(IP VARCHAR(45), VisitationDate DATETIME)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            //sqlite_cmd.CommandText = Createsql1;
            //sqlite_cmd.ExecuteNonQuery();

        }

        //Lägger till en sidbesökares IP och nuvarande DateTime i Visitors i databasen
        public static void AddVisitor(string userIP)
        {
            SqliteConnection conn = SQlite.CreateConnection();

            //Skriver och skapar Queryn
            SqliteCommand sqlite_cmd;
            string Createsql = "INSERT INTO Visitors(IP, VisitationDate) VALUES(@IP, @VisitationDate)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;

            //Skapar parametrarna (variablerna)
            SqliteParameter IPparam = sqlite_cmd.CreateParameter();
            SqliteParameter Dateparam = sqlite_cmd.CreateParameter();
            IPparam.ParameterName = "IP";
            IPparam.Value = userIP;
            Dateparam.ParameterName = "VisitationDate";
            Dateparam.Value = @DateTime.Now;

            //Lägger in parametrarna
            sqlite_cmd.Parameters.Add(IPparam);
            sqlite_cmd.Parameters.Add(Dateparam);

            //Kör Queryn med parametrar
            sqlite_cmd.ExecuteNonQuery();
        }

        //Returnerar totala antalet besökare (entrys i visitor)
        public static long countTotalVisitors()
        {
            SqliteConnection conn = SQlite.CreateConnection();
            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT COUNT(*) FROM Visitors";
            return (long)sqlite_cmd.ExecuteScalar();
        }

        //Returnerar antalet unika IP-adresser
        public static long countIndividualVisitors()
        {
            SqliteConnection conn = SQlite.CreateConnection();
            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT COUNT(DISTINCT IP) FROM Visitors";
            return (long)sqlite_cmd.ExecuteScalar();
        }

    }
}
