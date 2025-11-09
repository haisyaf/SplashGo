using System;
using System.Windows;
using Npgsql;
using DotNetEnv;

class Program
{
    static void Main()
    {
        // Load the .env file
        Env.Load();

        // Read connection string
        string connectionString = Environment.GetEnvironmentVariable("NEON_CONNECTION");

        try
        {
            using (var conn = new NpgsqlConnection(connectionString))
            {
                conn.Open();
                MessageBox.Show("✅ Successfully connected!");
                using (var cmd = new NpgsqlCommand("SELECT version();", conn))
                {
                    Console.WriteLine("PostgreSQL Version: " + cmd.ExecuteScalar());
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("X FAILED connected!");
            Console.WriteLine(ex.Message);
        }

        Console.ReadLine();
    }
}
