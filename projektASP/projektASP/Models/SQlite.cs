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
            string Createsql = "CREATE TABLE SampleTable(Col1 VARCHAR(20), Col2 INT)";
            string Createsql1 = "DROP TABLE SampleTable";
            sqlite_cmd = conn.CreateCommand();
            sqlite_cmd.CommandText = Createsql;
            sqlite_cmd.ExecuteNonQuery();
            sqlite_cmd.CommandText = Createsql1;
            sqlite_cmd.ExecuteNonQuery();

        }
    }
}
