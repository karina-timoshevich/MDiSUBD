﻿using Npgsql;
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
        public async Task<List<PickupLocation>> GetAllPickupLocations()
        {
            var pickupLocations = new List<PickupLocation>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    // Запрос на выборку всех пунктов выдачи
                    var query = "SELECT Id, Name, Address FROM PickupLocation";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                pickupLocations.Add(new PickupLocation
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Address = reader.GetString(2)
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

            return pickupLocations;
        }
        public async Task<List<Review>> GetReviews()
        {
            var reviews = new List<Review>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "SELECT id, client_id, rating, text, date FROM Review ORDER BY date DESC";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reviews.Add(new Review
                                {
                                    Id = reader.GetInt32(0),
                                    ClientId = reader.GetInt32(1),
                                    Rating = reader.GetInt32(2),
                                    Text = reader.IsDBNull(3) ? null : reader.GetString(3),
                                    Date = reader.GetDateTime(4)
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

            return reviews;
        }
        public async Task<bool> AddReview(int clientId, int rating, string text)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "INSERT INTO Review (client_id, rating, text, date) VALUES (@client_id, @rating, @text, CURRENT_TIMESTAMP)";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("client_id", clientId);
                        cmd.Parameters.AddWithValue("rating", rating);
                        cmd.Parameters.AddWithValue("text", (object)text ?? DBNull.Value);

                        await cmd.ExecuteNonQueryAsync();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }
        public async Task<List<CartItem>> GetCartItemsByClientId(int clientId)
        {
            var cartItems = new List<CartItem>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                SELECT ci.id, ci.product_id, ci.quantity, p.Name AS ProductName, p.Price
                FROM CartItem ci
                JOIN Product p ON ci.product_id = p.id
                WHERE ci.cart_id = @ClientId";

                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientId", clientId);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cartItems.Add(new CartItem
                                {
                                    Id = reader.GetInt32(0),
                                    ProductId = reader.GetInt32(1),
                                    Quantity = reader.GetInt32(2),
                                    ProductName = reader.GetString(3),
                                    Price = reader.GetDecimal(4)
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

            return cartItems;
        }

        public async Task RemoveFromCart(int cartItemId, int clientId)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    Console.WriteLine($"Attempting to remove item from cart. CartItemId: {cartItemId}, ClientId: {clientId}");

                    var query = "DELETE FROM CartItem WHERE product_id = @CartItemId AND cart_id = @ClientId";

                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CartItemId", cartItemId);
                        cmd.Parameters.AddWithValue("@ClientId", clientId);

                        var rowsAffected = await cmd.ExecuteNonQueryAsync();
                
                        Console.WriteLine($"Rows affected: {rowsAffected}");

                        if (rowsAffected == 0)
                        {
                            Console.WriteLine("No rows were deleted. Make sure the item exists and the client_id matches.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        public async Task<Cart> GetCartByClientId(int clientId)
        {
            Cart cart = null;

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "SELECT Id, TotalPrice FROM Cart WHERE CartId = @ClientId";

                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientId", clientId);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                cart = new Cart
                                {
                                    ClientId = reader.GetInt32(0),
                                    TotalPrice = reader.GetDecimal(1)
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

            return cart;
        }
     public async Task UpdateCart(int clientId, int productId, int quantity)
{
    try
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var checkQuery = "SELECT COUNT(*) FROM CartItem WHERE cart_id = @ClientId AND product_id = @ProductId";
            await using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
            {
                checkCmd.Parameters.AddWithValue("@ClientId", clientId);
                checkCmd.Parameters.AddWithValue("@ProductId", productId);

                var count = (long)await checkCmd.ExecuteScalarAsync();
                if (count > 0)
                {
                    var updateQuery = "UPDATE CartItem SET quantity = @Quantity WHERE cart_id = @ClientId AND product_id = @ProductId";
                    await using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                    {
                        updateCmd.Parameters.AddWithValue("@ClientId", clientId);
                        updateCmd.Parameters.AddWithValue("@ProductId", productId);
                        updateCmd.Parameters.AddWithValue("@Quantity", quantity);

                        await updateCmd.ExecuteNonQueryAsync();
                    }
                }
                else
                {
                    var insertQuery = "INSERT INTO CartItem (cart_id, product_id, quantity) VALUES (@ClientId, @ProductId, @Quantity)";
                    await using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                    {
                        insertCmd.Parameters.AddWithValue("@ClientId", clientId);
                        insertCmd.Parameters.AddWithValue("@ProductId", productId);
                        insertCmd.Parameters.AddWithValue("@Quantity", quantity);

                        await insertCmd.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

        public async Task<Product> GetProductById(int productId)
        {
            Product product = null;

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = @"
                SELECT p.Id, p.Name, p.Description, p.Price, p.Quantity,
                       pt.name AS ProductType, m.name AS ManufacturerName, uom.name AS UnitOfMeasure
                FROM Product p
                JOIN ProductType pt ON p.product_type_id = pt.id
                JOIN Manufacturer m ON p.manufacturer_id = m.id
                JOIN UnitOfMeasure uom ON p.unit_of_measure_id = uom.id
                WHERE p.Id = @ProductId";

                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ProductId", productId);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                product = new Product
                                {
                                    Id = reader.GetInt32(0),
                                    Name = reader.GetString(1),
                                    Description = reader.GetString(2),
                                    Price = reader.GetDecimal(3),
                                    Quantity = reader.GetInt32(4),
                                    ProductType = reader.GetString(5),
                                    ManufacturerName = reader.GetString(6),
                                    UnitOfMeasure = reader.GetString(7)
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

            return product;
        }
        public async Task AddProductToCart(int clientId, int productId, int quantity)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var checkCartQuery = "SELECT COUNT(*) FROM Cart WHERE client_id = @ClientId";
                    await using (var checkCartCmd = new NpgsqlCommand(checkCartQuery, conn))
                    {
                        checkCartCmd.Parameters.AddWithValue("@ClientId", clientId);

                        var cartCount = (long)await checkCartCmd.ExecuteScalarAsync();
                        
                        if (cartCount == 0)
                        {
                            var createCartQuery = "INSERT INTO Cart (client_id, total_price) VALUES (@ClientId, 0)";
                            await using (var createCartCmd = new NpgsqlCommand(createCartQuery, conn))
                            {
                                createCartCmd.Parameters.AddWithValue("@ClientId", clientId);
                                await createCartCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }

                    var getCartIdQuery = "SELECT client_id FROM Cart WHERE client_id = @ClientId";
                    int cartId;
                    await using (var getCartIdCmd = new NpgsqlCommand(getCartIdQuery, conn))
                    {
                        getCartIdCmd.Parameters.AddWithValue("@ClientId", clientId);
                        cartId = (int)await getCartIdCmd.ExecuteScalarAsync();
                    }

                    var checkQuery = "SELECT COUNT(*) FROM CartItem WHERE cart_id = @CartId AND product_id = @ProductId";
                    await using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@CartId", cartId);
                        checkCmd.Parameters.AddWithValue("@ProductId", productId);

                        var count = (long)await checkCmd.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            var updateQuery = "UPDATE CartItem SET quantity = quantity + @Quantity WHERE cart_id = @CartId AND product_id = @ProductId";
                            await using (var updateCmd = new NpgsqlCommand(updateQuery, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@CartId", cartId);
                                updateCmd.Parameters.AddWithValue("@ProductId", productId);
                                updateCmd.Parameters.AddWithValue("@Quantity", quantity);

                                await updateCmd.ExecuteNonQueryAsync();
                            }
                        }
                        else
                        {
                            var insertQuery = "INSERT INTO CartItem (cart_id, product_id, quantity) VALUES (@CartId, @ProductId, @Quantity)";
                            await using (var insertCmd = new NpgsqlCommand(insertQuery, conn))
                            {
                                insertCmd.Parameters.AddWithValue("@CartId", cartId);
                                insertCmd.Parameters.AddWithValue("@ProductId", productId);
                                insertCmd.Parameters.AddWithValue("@Quantity", quantity);

                                await insertCmd.ExecuteNonQueryAsync();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
public async Task CreateOrder(int clientId, int pickupLocationId, int? promoCodeId = null)
{
    try
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            await using (var transaction = await conn.BeginTransactionAsync())
            {
                try
                {
                    // Проверяем, существует ли корзина
                    var checkCartQuery = "SELECT COUNT(*) FROM Cart WHERE client_id = @ClientId";
                    await using (var checkCartCmd = new NpgsqlCommand(checkCartQuery, conn, transaction))
                    {
                        checkCartCmd.Parameters.AddWithValue("@ClientId", clientId);

                        var cartCount = (long)await checkCartCmd.ExecuteScalarAsync();
                        if (cartCount == 0)
                        {
                            throw new Exception("Корзина пуста");
                        }
                    }

                    // Получаем данные о корзине
                    var getCartQuery = "SELECT client_id, total_price FROM Cart WHERE client_id = @ClientId";
                    int cartId;
                    decimal totalPrice;
                    await using (var getCartCmd = new NpgsqlCommand(getCartQuery, conn, transaction))
                    {
                        getCartCmd.Parameters.AddWithValue("@ClientId", clientId);
                        await using (var reader = await getCartCmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                cartId = reader.GetInt32(0);
                                totalPrice = reader.GetDecimal(1);
                            }
                            else
                            {
                                throw new Exception("Ошибка при получении корзины");
                            }
                        }
                    }

                    // Создаём заказ
                    var createOrderQuery = @"
                        INSERT INTO Orders (client_id, order_date, total_price, promo_code_id, pickup_location_id, status) 
                        VALUES (@ClientId, @OrderDate, @TotalPrice, @PromoCodeId, @PickupLocationId, 'Pending') 
                        RETURNING id";
                    int orderId;
                    await using (var createOrderCmd = new NpgsqlCommand(createOrderQuery, conn, transaction))
                    {
                        createOrderCmd.Parameters.AddWithValue("@ClientId", clientId);
                        createOrderCmd.Parameters.AddWithValue("@OrderDate", DateTime.UtcNow);
                        createOrderCmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                        createOrderCmd.Parameters.AddWithValue("@PromoCodeId", promoCodeId.HasValue ? (object)promoCodeId.Value : DBNull.Value);
                        createOrderCmd.Parameters.AddWithValue("@PickupLocationId", pickupLocationId);

                        orderId = (int)await createOrderCmd.ExecuteScalarAsync();
                    }

                    // Сохраняем CartItem в память
                    var cartItems = new List<(int ProductId, int Quantity)>();
                    var getCartItemsQuery = "SELECT product_id, quantity FROM CartItem WHERE cart_id = @CartId";
                    await using (var getCartItemsCmd = new NpgsqlCommand(getCartItemsQuery, conn, transaction))
                    {
                        getCartItemsCmd.Parameters.AddWithValue("@CartId", cartId);

                        await using (var reader = await getCartItemsCmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                cartItems.Add((reader.GetInt32(0), reader.GetInt32(1)));
                            }
                        }
                    }

                    // Переносим CartItem в OrderItem
                    foreach (var (productId, quantity) in cartItems)
                    {
                        var insertOrderItemQuery = @"
                            INSERT INTO OrderItem (order_id, product_id, quantity) 
                            VALUES (@OrderId, @ProductId, @Quantity)";
                        await using (var insertOrderItemCmd = new NpgsqlCommand(insertOrderItemQuery, conn, transaction))
                        {
                            insertOrderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
                            insertOrderItemCmd.Parameters.AddWithValue("@ProductId", productId);
                            insertOrderItemCmd.Parameters.AddWithValue("@Quantity", quantity);

                            await insertOrderItemCmd.ExecuteNonQueryAsync();
                        }
                    }

                    // Очищаем корзину клиента
                    var deleteCartItemsQuery = "DELETE FROM CartItem WHERE cart_id = @CartId";
                    await using (var deleteCartItemsCmd = new NpgsqlCommand(deleteCartItemsQuery, conn, transaction))
                    {
                        deleteCartItemsCmd.Parameters.AddWithValue("@CartId", cartId);
                        await deleteCartItemsCmd.ExecuteNonQueryAsync();
                    }

                    var deleteCartQuery = "DELETE FROM Cart WHERE client_id = @ClientId";
                    await using (var deleteCartCmd = new NpgsqlCommand(deleteCartQuery, conn, transaction))
                    {
                        deleteCartCmd.Parameters.AddWithValue("@ClientId", clientId);
                        await deleteCartCmd.ExecuteNonQueryAsync();
                    }

                    // Подтверждаем транзакцию
                    await transaction.CommitAsync();

                    Console.WriteLine($"Заказ успешно оформлен. ID заказа: {orderId}");
                }
                catch (Exception)
                {
                    // Откатываем транзакцию при ошибке
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}


        public async Task<List<PromoCode>> GetAllPromoCodes()
        {
            var promoCodes = new List<PromoCode>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var query = "SELECT Id, Code, Discount FROM PromoCode";
                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                promoCodes.Add(new PromoCode
                                {
                                    Id = reader.GetInt32(0),
                                    Code = reader.GetString(1),
                                    Discount = reader.GetDecimal(2)
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

            return promoCodes;
        }
        public async Task SetEmployeeIdInSession(int employeeId)
        {
            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var setEmployeeIdQuery = "SET app.employee_id TO @EmployeeId";
                    await using (var setEmployeeIdCmd = new NpgsqlCommand(setEmployeeIdQuery, conn))
                    {
                        setEmployeeIdCmd.Parameters.AddWithValue("@EmployeeId", employeeId);
                        await setEmployeeIdCmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting employee ID: {ex.Message}");
            }
        }
        public async Task<List<Orders>> GetOrdersByClientId(int clientId)
        {
            var orders = new List<Orders>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    var getOrdersQuery = @"
                SELECT o.id, o.client_id, o.order_date, o.total_price, o.promo_code_id, o.pickup_location_id, o.status
                FROM Orders o
                WHERE o.client_id = @ClientId
                ORDER BY o.order_date DESC";

                    await using (var cmd = new NpgsqlCommand(getOrdersQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@ClientId", clientId);

                        await using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                var order = new Orders
                                {
                                    Id = reader.GetInt32(0),
                                    ClientId = reader.GetInt32(1),
                                    OrderDate = reader.GetDateTime(2),
                                    TotalPrice = reader.GetDecimal(3),
                                    PromoCodeId = reader.IsDBNull(4) ? null : reader.GetInt32(4),
                                    PickupLocationId = reader.IsDBNull(5) ? null : reader.GetInt32(5),
                                    Status = reader.GetString(6)
                                };
                                orders.Add(order);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            return orders;
        }

    }
}