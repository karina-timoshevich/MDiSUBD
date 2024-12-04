using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;
using LabRab6_MDiSUBD_Timoshevich.Models;
using LabRab6_MDiSUBD_Timoshevich.Entities;

namespace LabRab6_MDiSUBD_Timoshevich.Services
{
    public class DbService
    {
        private readonly string _connectionString;

        public DbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetValue<string>("ConnectionString") 
                                ?? throw new ArgumentNullException("ConnectionString is not configured");
        }

        // Метод для получения списка продуктов
        public async Task<List<Product>> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();
                    
                    // Запрос для получения продуктов
                    await using (var cmd = new NpgsqlCommand("SELECT id, name, price FROM product", conn))
                    {
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var product = new Product
                                {
                                    Id = (int)reader["id"],
                                    Name = reader["name"].ToString(),
                                    Price = (decimal)reader["price"]
                                };

                                products.Add(product);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return products;
        }
    }
}