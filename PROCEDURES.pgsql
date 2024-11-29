CREATE OR REPLACE PROCEDURE add_product(
    p_name VARCHAR,
    p_description TEXT,
    p_price DECIMAL(10, 2),
    p_product_type_id INT,
    p_manufacturer_id INT,
    p_unit_of_measure_id INT,
    p_quantity INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Product (name, description, price, product_type_id, manufacturer_id, unit_of_measure_id, quantity)
    VALUES (p_name, p_description, p_price, p_product_type_id, p_manufacturer_id, p_unit_of_measure_id, p_quantity);
END;
$$;

CREATE OR REPLACE PROCEDURE update_product(
    p_id INT,
    p_name VARCHAR,
    p_description TEXT,
    p_price DECIMAL(10, 2),
    p_product_type_id INT,
    p_manufacturer_id INT,
    p_unit_of_measure_id INT,
    p_quantity INT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Product
    SET 
        name = p_name,
        description = p_description,
        price = p_price,
        product_type_id = p_product_type_id,
        manufacturer_id = p_manufacturer_id,
        unit_of_measure_id = p_unit_of_measure_id,
        quantity = p_quantity
    WHERE id = p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE delete_product(p_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM Product WHERE id = p_id;
END;
$$;
-- Удаляем товар с id = 5
--CALL delete_product(25);
select * from product;

CREATE OR REPLACE PROCEDURE update_order_status(
    p_order_id INT,
    p_status VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Orders
    SET status = p_status
    WHERE id = p_order_id;
END;
$$;

CREATE OR REPLACE PROCEDURE add_job(
    p_title VARCHAR,
    p_description TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Job (title, description) VALUES (p_title, p_description);
END;
$$;

CREATE OR REPLACE PROCEDURE update_job(
    p_id INT,
    p_title VARCHAR,
    p_description TEXT
)
LANGUAGE plpgsql
AS $$
BEGIN
    UPDATE Job
    SET 
        title = p_title,
        description = p_description
    WHERE id = p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE delete_job(p_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM Job WHERE id = p_id;
END;
$$;

CREATE OR REPLACE PROCEDURE add_client(
    p_first_name VARCHAR,
    p_last_name VARCHAR,
    p_date_of_birth DATE,
    p_phone_number VARCHAR(20),
    p_password VARCHAR(100)
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO Client (first_name, last_name, date_of_birth, phone_number, password)
    VALUES (p_first_name, p_last_name, p_date_of_birth, p_phone_number, p_password);
END;
$$;

CREATE OR REPLACE PROCEDURE add_pickup_location(
    p_name VARCHAR,
    p_address VARCHAR(200)
)
LANGUAGE plpgsql
AS $$
BEGIN
    INSERT INTO PickupLocation (name, address)
    VALUES (p_name, p_address);
END;
$$;

CREATE OR REPLACE PROCEDURE delete_pickup_location(p_id INT)
LANGUAGE plpgsql
AS $$
BEGIN
    DELETE FROM PickupLocation WHERE id = p_id;
END;
$$;
