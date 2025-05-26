using System;
using System.Data.SQLite;
using System.IO;

namespace SleepApp.Helpers
{
    public static class DatabaseHelper
    {
        private static string dbPath = "Data Source=sleepapp.db;Version=3;";

        public static SQLiteConnection GetConnection()
        {
            return new SQLiteConnection(dbPath);
        }

        public static void InitializeDatabase()
        {
            if (!File.Exists("sleepapp.db"))
            {
                SQLiteConnection.CreateFile("sleepapp.db");
            }

            using (var conn = GetConnection())
            {
                conn.Open();
                var cmd = conn.CreateCommand();

                cmd.CommandText = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT NOT NULL UNIQUE,
                        Email TEXT NOT NULL,
                        PasswordHash TEXT NOT NULL
                    );

                    CREATE TABLE IF NOT EXISTS Alarms (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        Time TEXT,
                        IsEnabled INTEGER
                    );

                    CREATE TABLE IF NOT EXISTS SleepSessions (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        UserId INTEGER,
                        StartTime TEXT,
                        EndTime TEXT,
                        DurationMinutes INTEGER
                    );
                ";

                cmd.ExecuteNonQuery();
            }
        }
    }
}
