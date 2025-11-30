using Npgsql;
using SplashGoJunpro.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetEnv;

namespace SplashGoJunpro.Api.Data
{
    public class NeonDB
    {
        // Private properties → detail koneksi tersembunyi
        private string Host { get; set; }
        private string User { get; set; }
        private string Password { get; set; }
        private string Database { get; set; }
        private string Port { get; set; }

        // Constructor → otomatis baca dari .env
        public NeonDB()
        {
            Env.Load(); // load file .env

            Host = Environment.GetEnvironmentVariable("NEON_HOST");
            User = Environment.GetEnvironmentVariable("NEON_USER");
            Password = Environment.GetEnvironmentVariable("NEON_PASSWORD");
            Database = Environment.GetEnvironmentVariable("NEON_DATABASE");
            Port = Environment.GetEnvironmentVariable("NEON_PORT") ?? "5432";
        }

        // Private method → connection string
        private string GetConnectionString()
        {
            return $"Host={Host};Username={User};Password={Password};Database={Database};SSL Mode=Require;Trust Server Certificate=true;";
        }

        // Method SELECT with PARAMETERS → safer (prevents SQL injection)
        public async Task<List<Dictionary<string, object>>> QueryAsync(string sql, Dictionary<string, object> parameters)
        {
            var results = new List<Dictionary<string, object>>();
            var conn = new NpgsqlConnection(GetConnectionString());

            try
            {
                await conn.OpenAsync();

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    // Add parameters to query
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var row = new Dictionary<string, object>();
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                row[reader.GetName(i)] = reader[i];
                            }
                            results.Add(row);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in QueryAsync (params): {ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return results;
        }

        // Method for INSERT/UPDATE/DELETE query : return affected rows
        public async Task<int> ExecuteAsync(string sql, Dictionary<string, object> parameters)
        {
            int affected = 0;
            var conn = new NpgsqlConnection(GetConnectionString());

            try
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    foreach (var param in parameters)
                    {
                        cmd.Parameters.AddWithValue(param.Key, param.Value);
                    }

                    affected = await cmd.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ExecuteAsync: {ex.Message}");
            }
            finally
            {
                conn.Close();
            }

            return affected;
        }

    }
}
