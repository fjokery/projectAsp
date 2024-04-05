using Microsoft.Data.Sqlite;

namespace projektASP.Models
{
    public class SQlite
    {


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

        public static void AddVisitor(string userIP)
        {
            SqliteConnection conn = SQlite.CreateConnection();
            SqliteCommand sqlite_cmd;
            string Createsql = "INSERT INTO Visitors(IP, VisitationDate) VALUES(@IP, @VisitationDate)";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            SqliteParameter IPparam = sqlite_cmd.CreateParameter();
            SqliteParameter Dateparam = sqlite_cmd.CreateParameter();
            IPparam.ParameterName = "IP";
            IPparam.Value = userIP;
            Dateparam.ParameterName = "VisitationDate";
            Dateparam.Value = @DateTime.Now;
            sqlite_cmd.Parameters.Add(IPparam);
            sqlite_cmd.Parameters.Add(Dateparam);
            sqlite_cmd.ExecuteNonQuery();
        }

        public static long countVisitors()
        {
            SqliteConnection conn = SQlite.CreateConnection();
            SqliteCommand sqlite_cmd;
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = "SELECT COUNT(*) FROM Visitors";
            return (long)sqlite_cmd.ExecuteScalar();
        }


    }
}
