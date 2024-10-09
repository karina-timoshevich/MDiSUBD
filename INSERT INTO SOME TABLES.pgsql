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
