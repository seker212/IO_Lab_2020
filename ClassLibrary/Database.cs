using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace TCPServer
{
    public class Database : IDisposable
    {
        SqliteConnection connection;

        public Database()
        {
            connection = new SqliteConnection("Data Source=:memory:");
            connection.Open();
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = @"
                    CREATE TABLE todo(
                        id INTEGER PRIMARY KEY AUTOINCREMENT,
                        task TEXT NOT NULL,
                        isCompleted BOOL NOT NULL
                    )";
                cmd.ExecuteNonQuery();
            }
        }

        public string Get()
        {
            string result = string.Empty;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT id, task, isCompleted FROM todo ORDER BY isCompleted ASC";
                using (var reader = cmd.ExecuteReaderAsync())
                {
                    while(reader.Result.Read())
                        result += reader.Result.GetString(0) + " | " + reader.Result.GetString(1) + " | " + reader.Result.GetString(2) + "\r\n";
                    return result;
                }
            }
        }

        public bool Add(string task)
        {
            if (string.IsNullOrWhiteSpace(task))
            {
                return false;
            }
            else
            {
                string result = string.Empty;
                using (var cmd = connection.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO todo(task, isCompleted) VALUES(@task, false)";
                    cmd.Parameters.AddWithValue("@task", task);
                    cmd.Prepare();
                    cmd.ExecuteNonQueryAsync();
                    return true;
                }
            }
        }

        public void Complete(int taskid)
        {
            string result = string.Empty;
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "UPDATE todo SET isCompleted = true WHERE id = @id";
                cmd.Parameters.AddWithValue("@id", taskid);
                cmd.Prepare();
                cmd.ExecuteNonQueryAsync();
            }
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }
    }
}
