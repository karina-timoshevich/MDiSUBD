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
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DbService(IConfiguration configuration,IHttpContextAccessor httpContextAccessor)
        {
            _connectionString = configuration.GetValue<string>("ConnectionString")
                                ?? throw new ArgumentNullException("ConnectionString is not configured");
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CheckEmployeeIdInSession()
        {
            
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
        
                var query = "SHOW app.employee_id;";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    Console.WriteLine($"Current app.employee_id: {result}");
            
                    if (result == null || result.ToString() == "")
                    {
                        Console.WriteLine("Error: app.employee_id is not set or is empty.");
                    }
                }
            }
        }
        public async Task<List<Product>> GetAllProducts()
        {
            var products = new List<Product>();

            try
            {
                await using (var conn = new NpgsqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

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

                    var query = "SELECT id, first_name, last_name, email, password, position_id FROM Employee WHERE email = @Email";
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
                                    Password = reader["password"].ToString(),
                                    PositionId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5) 

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

                    var query =
                        "CALL public.add_client(@FirstName, @LastName, @DateOfBirth, @PhoneNumber, @Password, @Email)";
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
                        cmd.Parameters.AddWithValue("@DateOfBirth",
                            client.DateOfBirth.HasValue ? client.DateOfBirth.Value.Date : (object)DBNull.Value);
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

                    var query =
                        "INSERT INTO Review (client_id, rating, text, date) VALUES (@client_id, @rating, @text, CURRENT_TIMESTAMP)";
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

                    Console.WriteLine(
                        $"Attempting to remove item from cart. CartItemId: {cartItemId}, ClientId: {clientId}");

                    var query = "DELETE FROM CartItem WHERE product_id = @CartItemId AND cart_id = @ClientId";

                    await using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CartItemId", cartItemId);
                        cmd.Parameters.AddWithValue("@ClientId", clientId);

                        var rowsAffected = await cmd.ExecuteNonQueryAsync();

                        Console.WriteLine($"Rows affected: {rowsAffected}");

                        if (rowsAffected == 0)
                        {
                            Console.WriteLine(
                                "No rows were deleted. Make sure the item exists and the client_id matches.");
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
                    var checkQuery =
                        "SELECT COUNT(*) FROM CartItem WHERE cart_id = @ClientId AND product_id = @ProductId";
                    await using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@ClientId", clientId);
                        checkCmd.Parameters.AddWithValue("@ProductId", productId);

                        var count = (long)await checkCmd.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            var updateQuery =
                                "UPDATE CartItem SET quantity = @Quantity WHERE cart_id = @ClientId AND product_id = @ProductId";
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
                            var insertQuery =
                                "INSERT INTO CartItem (cart_id, product_id, quantity) VALUES (@ClientId, @ProductId, @Quantity)";
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

                    var checkQuery =
                        "SELECT COUNT(*) FROM CartItem WHERE cart_id = @CartId AND product_id = @ProductId";
                    await using (var checkCmd = new NpgsqlCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@CartId", cartId);
                        checkCmd.Parameters.AddWithValue("@ProductId", productId);

                        var count = (long)await checkCmd.ExecuteScalarAsync();
                        if (count > 0)
                        {
                            var updateQuery =
                                "UPDATE CartItem SET quantity = quantity + @Quantity WHERE cart_id = @CartId AND product_id = @ProductId";
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
                            var insertQuery =
                                "INSERT INTO CartItem (cart_id, product_id, quantity) VALUES (@CartId, @ProductId, @Quantity)";
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

        public async Task CreateOrder(int clientId, int pickupLocationId, int? promoCodeId, decimal totalPrice)
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

                            var getCartQuery = "SELECT client_id, total_price FROM Cart WHERE client_id = @ClientId";
                            int cartId;
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
                                createOrderCmd.Parameters.AddWithValue("@PromoCodeId",
                                    promoCodeId.HasValue ? (object)promoCodeId.Value : DBNull.Value);
                                createOrderCmd.Parameters.AddWithValue("@PickupLocationId", pickupLocationId);

                                orderId = (int)await createOrderCmd.ExecuteScalarAsync();
                            }


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

                            foreach (var (productId, quantity) in cartItems)
                            {
                                var insertOrderItemQuery = @"
                            INSERT INTO OrderItem (order_id, product_id, quantity) 
                            VALUES (@OrderId, @ProductId, @Quantity)";
                                await using (var insertOrderItemCmd =
                                             new NpgsqlCommand(insertOrderItemQuery, conn, transaction))
                                {
                                    insertOrderItemCmd.Parameters.AddWithValue("@OrderId", orderId);
                                    insertOrderItemCmd.Parameters.AddWithValue("@ProductId", productId);
                                    insertOrderItemCmd.Parameters.AddWithValue("@Quantity", quantity);

                                    await insertOrderItemCmd.ExecuteNonQueryAsync();
                                }
                            }

                            var deleteCartItemsQuery = "DELETE FROM CartItem WHERE cart_id = @CartId";
                            await using (var deleteCartItemsCmd =
                                         new NpgsqlCommand(deleteCartItemsQuery, conn, transaction))
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

                            await transaction.CommitAsync();

                            Console.WriteLine($"Заказ успешно оформлен. ID заказа: {orderId}");
                        }
                        catch (Exception)
                        {
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
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = $"SET LOCAL app.employee_id = {employeeId};";
                var queryCheck = "SHOW app.employee_id;";
                await using (var cmdCheck = new NpgsqlCommand(queryCheck, conn))
                {
                    var result = await cmdCheck.ExecuteScalarAsync();
                    Console.WriteLine($"Current from db app.employee_id: {result}");
                }

                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
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

        public async Task<decimal> GetCartTotalPrice(int clientId)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT total_price FROM Cart WHERE client_id = @ClientId";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    return (decimal)await cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task UpdateCartTotalPrice(int clientId, decimal totalPrice)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
               
                var query = "UPDATE Cart SET total_price = @TotalPrice WHERE client_id = @ClientId";

                Console.WriteLine(
                    $"Executing query: {query}, Parameters: ClientId = {clientId}, TotalPrice = {totalPrice}");

                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@TotalPrice", totalPrice);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();

                    Console.WriteLine($"Rows affected: {rowsAffected}");

                    if (rowsAffected == 0)
                    {
                        Console.WriteLine("No rows updated.");
                    }
                }
            }
        }

        public async Task<decimal> GetPromoCodeDiscount(int promoCodeId)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT discount FROM PromoCode WHERE id = @PromoCodeId";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@PromoCodeId", promoCodeId);
                    return (decimal)await cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task<bool> UpdateCartItemQuantity(int clientId, int productId, int quantity)
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                Console.WriteLine(
                    $"Connection opened for ClientId: {clientId}, ProductId: {productId}, Quantity: {quantity}");

                if (quantity < 1)
                {
                    var deleteQuery =
                        "DELETE FROM CartItem WHERE cart_id = (SELECT id FROM Cart WHERE client_id = @ClientId) AND product_id = @ProductId";
                    await using (var deleteCmd = new NpgsqlCommand(deleteQuery, conn))
                    {
                        deleteCmd.Parameters.AddWithValue("@ClientId", clientId);
                        deleteCmd.Parameters.AddWithValue("@ProductId", productId);
                        var rowsDeleted = await deleteCmd.ExecuteNonQueryAsync();
                        Console.WriteLine($"Rows deleted: {rowsDeleted}");
                        return rowsDeleted > 0;
                    }
                }

                var updateQuery =
                    "UPDATE CartItem SET quantity = @Quantity WHERE cart_id = @ClientId AND product_id = @ProductId";
                Console.WriteLine($"Executing update query: {updateQuery}");
                await using (var cmd = new NpgsqlCommand(updateQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@Quantity", quantity);
                    cmd.Parameters.AddWithValue("@ClientId", clientId);
                    cmd.Parameters.AddWithValue("@ProductId", productId);

                    Console.WriteLine(
                        $"Parameters: Quantity = {quantity}, ClientId = {clientId}, ProductId = {productId}");

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"Rows affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
        }

        public async Task<List<Orders>> GetOrdersForWorkerAsync()
        {
            var orders = new List<Orders>();

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query =
                    "SELECT id, client_id, order_date, total_price, promo_code_id, pickup_location_id, status FROM Orders";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            orders.Add(new Orders
                            {
                                Id = reader.GetInt32(0),
                                ClientId = reader.GetInt32(1),
                                OrderDate = reader.GetDateTime(2),
                                TotalPrice = reader.GetDecimal(3),
                                PromoCodeId = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4),
                                PickupLocationId = reader.GetInt32(5),
                                Status = reader.GetString(6)
                            });
                        }
                    }
                }
            }

            return orders;
        }

        public async Task<bool> UpdateOrderStatusAsync(int orderId, string newStatus)
        {
            var employeeId = _httpContextAccessor.HttpContext?.Session.GetInt32("EmployeeId");

           
            if (!employeeId.HasValue)
            {
                Console.WriteLine("Employee ID is not set in session.");
                return false;
            }

            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                Console.WriteLine($"Executing SQL update for OrderId: {orderId}, Status: {newStatus}");
                var setEmployeeIdQuery = $"SET app.employee_id = {employeeId};";
                await using (var setCmd = new NpgsqlCommand(setEmployeeIdQuery, conn))
                {
                    await setCmd.ExecuteNonQueryAsync();
                }
                var query = "UPDATE Orders SET status = @Status WHERE id = @OrderId";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Status", newStatus);
                    cmd.Parameters.AddWithValue("@OrderId", orderId);

                    var rowsAffected = await cmd.ExecuteNonQueryAsync();
                    Console.WriteLine($"Rows affected гзв: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
        }
 public async Task<List<Job>> GetAllJobsAsync()
    {
        var jobs = new List<Job>();

        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "SELECT id, title, description FROM Job";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            jobs.Add(new Job
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Description = reader.GetString(2)
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

        return jobs;
    }

    public async Task AddJobAsync(string title, string description)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var employeeId = _httpContextAccessor.HttpContext?.Session.GetInt32("EmployeeId");

                if (!employeeId.HasValue)
                {
                    Console.WriteLine("Employee ID is not set in session.");
                    return;
                }
                var setEmployeeIdQuery = $"SET app.employee_id = {employeeId.Value};";
                await using (var setCmd = new NpgsqlCommand(setEmployeeIdQuery, conn))
                {
                    await setCmd.ExecuteNonQueryAsync();
                }

                var query = "INSERT INTO Job (title, description) VALUES (@Title, @Description)";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    public async Task UpdateJobAsync(int id, string title, string description)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                var query = "UPDATE Job SET title = @Title, description = @Description WHERE id = @Id";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@Title", title);
                    cmd.Parameters.AddWithValue("@Description", description);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
    public async Task<string> GetUserRoleByEmailAsync(string email)
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();

            var clientQuery = "SELECT COUNT(*) FROM Client WHERE email = @Email";
            await using (var clientCmd = new NpgsqlCommand(clientQuery, conn))
            {
                clientCmd.Parameters.AddWithValue("@Email", email);
                var isClient = (long)await clientCmd.ExecuteScalarAsync() > 0;
                if (isClient) return "Client";
            }

            var employeeQuery = "SELECT position_id FROM Employee WHERE email = @Email";
            await using (var employeeCmd = new NpgsqlCommand(employeeQuery, conn))
            {
                employeeCmd.Parameters.AddWithValue("@Email", email);
                var positionId = await employeeCmd.ExecuteScalarAsync();

                if (positionId != null)
                {
                    return (int)positionId == 8 ? "Admin" : "Worker";
                }
            }
        }

        return "Unknown";
    }
    public async Task<bool> AddFAQAsync(string question, string answer)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "INSERT INTO FAQ (question, answer, date_added) VALUES (@Question, @Answer, @DateAdded)";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Question", question);
                    cmd.Parameters.AddWithValue("@Answer", answer);
                    cmd.Parameters.AddWithValue("@DateAdded", DateTime.UtcNow);

                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0; 
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> UpdateClientAsync(int id, Client client)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
    
                var query = "UPDATE Client SET first_name = @FirstName, last_name = @LastName, date_of_birth = @DateOfBirth, " +
                            "phone_number = @PhoneNumber, password = @Password, email = @Email WHERE id = @Id";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.Parameters.AddWithValue("@FirstName", client.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", client.LastName);
                    cmd.Parameters.AddWithValue("@DateOfBirth", client.DateOfBirth);
                    cmd.Parameters.AddWithValue("@PhoneNumber", client.PhoneNumber);
                    cmd.Parameters.AddWithValue("@Password", client.Password);
                    cmd.Parameters.AddWithValue("@Email", client.Email);
    
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> DeleteClientAsync(int id)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
    
                var query = "DELETE FROM Client WHERE id = @Id";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    var result = await cmd.ExecuteNonQueryAsync();
                    return result > 0;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return false;
        }
    }
    public async Task<List<Client>> GetAllClientsAsync()
    {
        var clients = new List<Client>();

        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT id, first_name, last_name, date_of_birth, phone_number, password, email FROM Client";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            clients.Add(new Client
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                DateOfBirth = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                PhoneNumber = reader.GetString(4),
                                Password = reader.GetString(5),
                                Email = reader.GetString(6)
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

        return clients;
    }
    public async Task<Client> GetClientByIdAsync(int id)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = "SELECT id, first_name, last_name, date_of_birth, phone_number, password, email FROM Client WHERE id = @Id";
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new Client
                            {
                                Id = reader.GetInt32(0),
                                FirstName = reader.GetString(1),
                                LastName = reader.GetString(2),
                                DateOfBirth = reader.IsDBNull(3) ? null : reader.GetDateTime(3),
                                PhoneNumber = reader.GetString(4),
                                Password = reader.GetString(5),
                                Email = reader.GetString(6)
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return null; 
        }
    }

    public async Task<bool> AddProduct(Product product)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = @"
                INSERT INTO Product (name, description, price, product_type_id, manufacturer_id, unit_of_measure_id, quantity) 
                VALUES (@name, @description, @price, @product_type_id, @manufacturer_id, @unit_of_measure_id, @quantity)";
            
                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("name", product.Name);
                    cmd.Parameters.AddWithValue("description", (object)product.Description ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("price", product.Price);
                    cmd.Parameters.AddWithValue("product_type_id", product.ProductTypeId); 
                    cmd.Parameters.AddWithValue("manufacturer_id", product.ManufacturerId); 
                    cmd.Parameters.AddWithValue("unit_of_measure_id", product.UnitOfMeasureId); 
                    cmd.Parameters.AddWithValue("quantity", product.Quantity);

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
  public async Task<bool> UpdateProduct(Product product)
{
    try
    {
        Console.WriteLine("Updating product in the database...");
        Console.WriteLine($"Product data: Id={product.Id}, Name={product.Name}, Description={product.Description}, Price={product.Price}, ProductTypeId={product.ProductTypeId}, ManufacturerId={product.ManufacturerId}, UnitOfMeasureId={product.UnitOfMeasureId}, Quantity={product.Quantity}");

        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            
            Console.WriteLine("Connection to database opened.");

            var query = @"
                UPDATE Product 
                SET name = @name, description = @description, price = @price, 
                    product_type_id = @product_type_id, manufacturer_id = @manufacturer_id, 
                    unit_of_measure_id = @unit_of_measure_id, quantity = @quantity
                WHERE id = @id";

            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("id", product.Id);
                cmd.Parameters.AddWithValue("name", product.Name);
                cmd.Parameters.AddWithValue("description", (object)product.Description ?? DBNull.Value);
                cmd.Parameters.AddWithValue("price", product.Price);
                cmd.Parameters.AddWithValue("product_type_id", product.ProductTypeId);
                cmd.Parameters.AddWithValue("manufacturer_id", product.ManufacturerId);
                cmd.Parameters.AddWithValue("unit_of_measure_id", product.UnitOfMeasureId);
                cmd.Parameters.AddWithValue("quantity", product.Quantity);

                Console.WriteLine("Executing query...");
                await cmd.ExecuteNonQueryAsync();
                Console.WriteLine("Query executed successfully.");
            }
        }

        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error occurred while updating product: {ex.Message}");
        return false;
    }
}

    public async Task<bool> DeleteProduct(int id)
    {
        try
        {
            await using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                var query = @"DELETE FROM Product WHERE id = @id";

                await using (var cmd = new NpgsqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("id", id);
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
public async Task<IEnumerable<ProductType>> GetProductTypes()
{
    var productTypes = new List<ProductType>();

    await using (var conn = new NpgsqlConnection(_connectionString))
    {
        await conn.OpenAsync();
        var query = "SELECT id, name FROM ProductType";

        await using (var cmd = new NpgsqlCommand(query, conn))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    productTypes.Add(new ProductType
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
    }

    return productTypes;
}

public async Task<IEnumerable<Manufacturer>> GetManufacturers()
{
    var manufacturers = new List<Manufacturer>();

    await using (var conn = new NpgsqlConnection(_connectionString))
    {
        await conn.OpenAsync();
        var query = "SELECT id, name, address, phone FROM Manufacturer";

        await using (var cmd = new NpgsqlCommand(query, conn))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    manufacturers.Add(new Manufacturer
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1),
                        Address = reader.GetString(2),
                        Phone = reader.GetString(3)
                    });
                }
            }
        }
    }

    return manufacturers;
}

public async Task<IEnumerable<UnitOfMeasure>> GetUnitsOfMeasure()
{
    var units = new List<UnitOfMeasure>();

    await using (var conn = new NpgsqlConnection(_connectionString))
    {
        await conn.OpenAsync();
        var query = "SELECT id, name FROM UnitOfMeasure";

        await using (var cmd = new NpgsqlCommand(query, conn))
        {
            await using (var reader = await cmd.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    units.Add(new UnitOfMeasure
                    {
                        Id = reader.GetInt32(0),
                        Name = reader.GetString(1)
                    });
                }
            }
        }
    }

    return units;
}
public async Task<List<FAQ>> GetFAQsAsync()
{
    var faqs = new List<FAQ>();

    try
    {
        await using (var conn = new NpgsqlConnection(_connectionString))
        {
            await conn.OpenAsync();
            var query = "SELECT id, question, answer, date_added FROM FAQ ORDER BY date_added DESC";
            await using (var cmd = new NpgsqlCommand(query, conn))
            {
                await using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        faqs.Add(new FAQ
                        {
                            Id = reader.GetInt32(0),
                            Question = reader.GetString(1),
                            Answer = reader.GetString(2),
                            DateAdded = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3)
                        });
                    }
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching FAQs: {ex.Message}");
    }

    return faqs;
}
    }
}