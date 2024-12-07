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

        public async Task<List<Product>> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Запрос на выборку всех продуктов с дополнительными данными (тип, производитель и т.д.)
                    var query = @"
                SELECT 
                    p.Id, p.Name, p.Description, p.Price, p.Quantity,
                    pt.name AS ProductType, 
                    m.name AS ManufacturerName, 
                    uom.name AS UnitOfMeasure
                FROM Product p
                JOIN ProductType pt ON p.product_type_id = pt.id
                JOIN Manufacturer m ON p.manufacturer_id = m.id
                JOIN UnitOfMeasure uom ON p.unit_of_measure_id = uom.id";

                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                products.Add(new Product
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4),
                                    ProductType = reader.GetString(5),
                                    ManufacturerName = reader.GetString(6),
                                    UnitOfMeasure = reader.GetString(7)
                                });
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


        public async Task<Client> GetClientByEmail(string email)
        {
            Client client = null;

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query =
                        "SELECT id, first_name, last_name, phone_number, email, password FROM client WHERE email = @Email";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                client = new Client
                                {
                                    Id = (int)reader["id"],
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    PhoneNumber = reader["phone_number"].ToString(),
                                    Password = reader["password"].ToString(),
                                    Email = reader["email"].ToString()

                                };
                                Console.WriteLine($"Client found: {client.FirstName} {client.LastName}");

                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return client;
        }

        public async Task<Employee> GetEmployeeByEmail(string email)
        {
            Employee employee = null;

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Запрос для получения сотрудника по email
                    var query = "SELECT id, first_name, last_name, email, password FROM employee WHERE email = @Email";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                employee = new Employee
                                {
                                    Id = (int)reader["id"],
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return employee;
        }

        public async Task<Employee> GetEmployeeByEmailAndPassword(string email, string password)
        {
            Employee employee = null;
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Запрос для получения сотрудника по email и password
                    await using (var cmd = new NpgsqlCommand(
                                     "SELECT id, first_name, last_name, phone, email, password FROM Employee WHERE email = @email AND password = @password",
                                     conn))
                    {
                        cmd.Parameters.AddWithValue("email", email);
                        cmd.Parameters.AddWithValue("password", password);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                employee = new Employee
                                {
                                    Id = (int)reader["id"],
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    Phone = reader["phone"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return employee;
        }

        public async Task<Client> GetClientByEmailAndPassword(string email, string password)
        {
            Client client = null;
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Запрос для получения клиента по email и password
                    await using (var cmd = new NpgsqlCommand(
                                     "SELECT id, first_name, last_name, phone_number, email, password FROM Client WHERE email = @email AND password = @password",
                                     conn))
                    {
                        cmd.Parameters.AddWithValue("email", email);
                        cmd.Parameters.AddWithValue("password", password);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                client = new Client
                                {
                                    Id = (int)reader["id"],
                                    FirstName = reader["first_name"].ToString(),
                                    LastName = reader["last_name"].ToString(),
                                    PhoneNumber = reader["phone_number"].ToString(),
                                    Email = reader["email"].ToString(),
                                    Password = reader["password"].ToString()
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return client;
        }

        public async Task AddClient(Client client)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "CALL public.add_client(@FirstName, @LastName, @DateOfBirth, @PhoneNumber, @Password, @Email)";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        Console.WriteLine($"FirstName: {client.FirstName}");
                        Console.WriteLine($"LastName: {client.LastName}");
                        Console.WriteLine($"DateOfBirth: {client.DateOfBirth}");
                        Console.WriteLine($"PhoneNumber: {client.PhoneNumber}");
                        Console.WriteLine($"Password: {client.Password}");
                        Console.WriteLine($"Email: {client.Email}");
                        
                        cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", client.LastName);
                        cmd.Parameters.AddWithValue("@DateOfBirth", client.DateOfBirth.HasValue ? client.DateOfBirth.Value.Date : (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Password", client.Password);
                        cmd.Parameters.AddWithValue("@Email", client.Email ?? (object)DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}