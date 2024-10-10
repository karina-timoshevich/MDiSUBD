INSERT INTO Position (name, description)
VALUES 
('Manager', 'Manages the team and oversees operations'),
('Salesperson', 'Handles customer relations and sales'),
('Developer', 'Develops and maintains software solutions')
ON CONFLICT (name) DO NOTHING;

INSERT INTO Employee (first_name, last_name, position_id, phone, email)
VALUES 
('Tolik', 'Smit', 1, '+375291234567', 'tolik.smit@gmail.com'),
('Klava', 'Eberman', 2, '+375332189951', 'klava.eberman@gmail.com'),
('Grisha', 'Johnson', 3, '+375256148834', 'grisha.johnson@gmail.com');

INSERT INTO ProductType (name)
VALUES ('Electronics'), ('Clothing'), ('Cosmetics'), ('Food'), ('Books');

INSERT INTO Manufacturer (name, address, phone)
VALUES 
('TechCorporation', '123 Tech Street', '+375298459612'),
('OldBeautiful', '98 OLd street', '+375298478916'),
('FashionisMyProfession', '707 Fashion Avenu', '+375335684526');

INSERT INTO UnitOfMeasure (name)
VALUES ('Piece'), ('Kilogram'), ('Liter'), ('Meter'),('Gram');

INSERT INTO Product (name, description, price, product_type_id, manufacturer_id, unit_of_measure_id, quantity)
VALUES 
('Smartphone', 'Latest model smartphone with 128GB storage', 799.99, 1, 1, 1, 50),
('T-Shirt', '100% cotton T-shirt', 19.99, 2, 2, 1, 200),
('Lipstick', 'Lipstick for kids', 29.99, 3, 3, 1, 150),
('Chocolate Bar', 'Delicious milk chocolate bar', 2.50, 4, 1, 1, 300),
('Novel Book', 'Best-selling novel from popular author', 15.99, 5, 1, 1, 100);

INSERT INTO PromoCode (code, discount)
VALUES 
('SALE10', 10.00), 
('WELCOME5', 5.00), 
('FREESHIP', 0.00), 
('DISCOUNT20', 20.00), 
('HOLIDAY15', 15.00);

INSERT INTO PickupLocation (name, address)
VALUES 
('Main Office', '1 Central Street'), 
('Warehouse', '45 Industrial Avenue'), 
('Store A', '123 Main Street'), 
('Store B', '789 Market Square'), 
('Airport Pickup', 'Airport Terminal 2');

INSERT INTO Client (first_name, last_name, date_of_birth, phone_number, password)
VALUES 
('Alice', 'Cooper', '1990-02-15', '+375291234567', 'password123'),
('Bob', 'Miller', '1985-07-10', '+375332145678', 'qwerty789'),
('Charlie', 'Brown', '1995-05-22', '+375259876543', 'charlie123'),
('Diana', 'Prince', '1988-12-05', '+375291234890', 'diana2020'),
('Edward', 'Norton', '1992-11-30', '+375256789012', 'edward987');

INSERT INTO Cart (client_id, total_price)
VALUES 
(1, 50.00), 
(2, 100.00), 
(3, 75.50), 
(4, 60.00), 
(5, 85.99);

INSERT INTO CartItem (product_id, quantity, cart_id)
VALUES 
(1, 2, 1), 
(2, 5, 2), 
(3, 1, 3), 
(4, 10, 4), 
(5, 2, 5);

INSERT INTO Orders (client_id, order_date, total_price, promo_code_id, pickup_location_id, status)
VALUES 
(1, '2024-10-09 10:00:00', 79.99, 1, 1, 'Processing'), 
(2, '2024-10-08 14:30:00', 120.50, 2, 2, 'Shipped'), 
(3, '2024-10-07 09:15:00', 85.00, 3, 3, 'Delivered'), 
(4, '2024-10-06 16:45:00', 55.00, 4, 4, 'Cancelled'), 
(5, '2024-10-05 11:00:00', 110.99, 5, 5, 'Completed');

INSERT INTO OrderItem (product_id, quantity, order_id)
VALUES 
(1, 1, 1), 
(2, 3, 2), 
(3, 2, 3), 
(4, 5, 4), 
(5, 1, 5);

INSERT INTO Review (client_id, rating, text, date)
VALUES 
(1, 5, 'Excellent product, highly recommend!', '2024-10-01 10:00:00'), 
(2, 4, 'Very good but could be cheaper.', '2024-10-02 14:30:00'), 
(3, 3, 'Average, nothing special.', '2024-10-03 09:15:00'), 
(4, 2, 'Not satisfied, poor quality.', '2024-10-04 16:45:00'), 
(5, 1, 'Terrible experience, would not buy again.', '2024-10-05 11:00:00');

INSERT INTO FAQ (question, answer, date_added)
VALUES 
('How can I track my order?', 'You can track your order using the tracking number provided in the email.', '2024-10-01'),
('What is the return policy?', 'You can return any product within 30 days of delivery.', '2024-10-02'),
('Do you offer international shipping?', 'Yes, we offer worldwide shipping.', '2024-10-03'),
('How can I apply a promo code?', 'You can apply a promo code at checkout.', '2024-10-04'),
('What payment methods are accepted?', 'We accept all major credit cards and PayPal.', '2024-10-05');

INSERT INTO Job (title, description)
VALUES 
('Software Engineer', 'Responsible for developing and maintaining software solutions.'),
('Product Manager', 'Oversees product development from concept to launch.'),
('Data Analyst', 'Analyzes data to provide business insights and improve decision-making.'),
('HR Specialist', 'Handles recruitment, employee relations, and compliance.'),
('Marketing Manager', 'Leads marketing campaigns and promotes brand awareness.');

