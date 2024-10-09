CREATE TABLE IF NOT EXISTS Position (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS Employee (
    id SERIAL PRIMARY KEY,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    photo VARCHAR,
    position_id INT NOT NULL REFERENCES Position(id),
    phone VARCHAR(20) NOT NULL,
    email VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS Client (
    id SERIAL PRIMARY KEY NOT NULL,
    first_name VARCHAR(100) NOT NULL,
    last_name VARCHAR(100) NOT NULL,
    date_of_birth DATE NOT NULL,
    phone_number VARCHAR(20) NOT NULL,
    password VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS ProductType (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS Manufacturer (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    address VARCHAR(200) NOT NULL,
    phone VARCHAR(20) NOT NULL
);

CREATE TABLE IF NOT EXISTS UnitOfMeasure (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS Product (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    price DECIMAL(10, 2) NOT NULL,
    product_type_id INT NOT NULL REFERENCES ProductType(id),
    manufacturer_id INT NOT NULL REFERENCES Manufacturer(id),
    unit_of_measure_id INT NOT NULL REFERENCES UnitOfMeasure(id),
    quantity INT NOT NULL
);

CREATE TABLE IF NOT EXISTS PromoCode (
    id SERIAL PRIMARY KEY,
    code VARCHAR(10) NOT NULL,
    discount DECIMAL(5, 2) NOT NULL
);


CREATE TABLE IF NOT EXISTS PickupLocation (
    id SERIAL PRIMARY KEY,
    name VARCHAR(200),
    address VARCHAR(200) NOT NULL
);


CREATE TABLE IF NOT EXISTS Cart (
    client_id INT PRIMARY KEY REFERENCES Client(id),
    total_price DECIMAL(10, 2) NOT NULL
);

CREATE TABLE IF NOT EXISTS CartItem (
    id SERIAL PRIMARY KEY,
    product_id INT NOT NULL REFERENCES Product(id),
    quantity INT NOT NULL DEFAULT 1,
    cart_id INT NOT NULL REFERENCES Cart(client_id)
);

CREATE TABLE IF NOT EXISTS Orders (
    id SERIAL PRIMARY KEY,
    client_id INT NOT NULL REFERENCES Client(id),
    order_date TIMESTAMP NOT NULL,
    total_price DECIMAL(10, 2) NOT NULL,
    promo_code_id INT REFERENCES PromoCode(id),
    pickup_location_id INT NOT NULL REFERENCES PickupLocation(id),
    status VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS OrderItem (
    id SERIAL PRIMARY KEY,
    product_id INT NOT NULL REFERENCES Product(id),
    quantity INT NOT NULL DEFAULT 1,
    order_id INT NOT NULL REFERENCES Orders(id)
);

CREATE TABLE IF NOT EXISTS Review (
    id SERIAL PRIMARY KEY,
    client_id INT NOT NULL REFERENCES Client(id),
    rating INT NOT NULL DEFAULT 5,
    text TEXT,
    date TIMESTAMP NOT NULL
);

CREATE TABLE IF NOT EXISTS FAQ (
    id SERIAL PRIMARY KEY,
    question TEXT NOT NULL,
    answer TEXT NOT NULL,
    date_added DATE
);

CREATE TABLE IF NOT EXISTS Job (
    id SERIAL PRIMARY KEY,
    title VARCHAR(200) NOT NULL,
    description TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Action (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT
);

CREATE TABLE IF NOT EXISTS Log (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL REFERENCES Employee(id),
    action_id INT NOT NULL REFERENCES Action(id),
    action_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS Employee_Action (
    employee_id INT NOT NULL REFERENCES Employee(id),
    action_id INT NOT NULL REFERENCES Action(id),
    PRIMARY KEY (employee_id, action_id)
);