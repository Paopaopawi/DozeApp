using System;
using System.Data.SQLite;
using System.Windows.Forms;
using SleepApp.Helpers;

namespace SleepApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Open a single connection just to set WAL mode
            string dbPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sleepapp.db");
            using (var conn = new SQLiteConnection($"Data Source={dbPath};Version=3;BusyTimeout=5000;"))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            // Initialize SQLite database
            DatabaseHelper.InitializeDatabase();

            Application.Run(new LoginForm()); // We'll create this form next
        }
    }
}
