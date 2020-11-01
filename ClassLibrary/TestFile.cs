using System;
using Microsoft.Data.Sqlite;

namespace Program
{
    public class Test
    {
        static public void test()
        {
            string cs = "Data Source=:memory:";
            string stm = "SELECT SQLITE_VERSION()";

            using (var con = new SqliteConnection(cs))
            {
                SQLitePCL.Batteries.Init();
                con.Open();

                using (var cmd = new SqliteCommand(stm, con)) 
                {
                    string version = cmd.ExecuteScalar().ToString();

                    Console.WriteLine($"SQLite version: {version}");
               
                }

            }
        }
    }
}