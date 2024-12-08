CREATE OR REPLACE FUNCTION decrease_product_quantity_on_order()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Product
    SET quantity = quantity - NEW.quantity
    WHERE id = NEW.product_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_decrease_product_quantity_on_order
AFTER INSERT ON OrderItem
FOR EACH ROW
EXECUTE FUNCTION decrease_product_quantity_on_order();

CREATE OR REPLACE FUNCTION increase_product_quantity_on_order_delete()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Product
    SET quantity = quantity + OLD.quantity
    WHERE id = OLD.product_id;

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_increase_product_quantity_on_order_delete
AFTER DELETE ON OrderItem
FOR EACH ROW
EXECUTE FUNCTION increase_product_quantity_on_order_delete();

CREATE OR REPLACE FUNCTION recalculate_order_total_price()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Orders
    SET total_price = (
        SELECT SUM(oi.quantity * p.price)
        FROM OrderItem oi
        JOIN Product p ON oi.product_id = p.id
        WHERE oi.order_id = NEW.order_id
    )
    WHERE id = NEW.order_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER trigger_recalculate_order_total_price_after_insert
AFTER INSERT ON OrderItem
FOR EACH ROW
EXECUTE FUNCTION recalculate_order_total_price();

CREATE OR REPLACE FUNCTION recalculate_order_total_price_on_delete()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Orders
    SET total_price = (
        SELECT COALESCE(SUM(oi.quantity * p.price), 0)
        FROM OrderItem oi
        JOIN Product p ON oi.product_id = p.id
        WHERE oi.order_id = OLD.order_id
    )
    WHERE id = OLD.order_id;

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_recalculate_order_total_price_after_delete
AFTER DELETE ON OrderItem
FOR EACH ROW
EXECUTE FUNCTION recalculate_order_total_price_on_delete();

CREATE OR REPLACE FUNCTION log_product_addition()
RETURNS TRIGGER AS $$
BEGIN
    IF current_setting('app.employee_id', true) IS NOT NULL THEN
        INSERT INTO Log (employee_id, action_id, action_date)
        VALUES (current_setting('app.employee_id')::INT, 
                (SELECT id FROM Action WHERE name = 'Add Product'), 
                CURRENT_TIMESTAMP);
    END IF;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_log_product_addition
AFTER INSERT ON Product
FOR EACH ROW
EXECUTE FUNCTION log_product_addition();

SELECT * FROM Log;

CREATE OR REPLACE FUNCTION log_product_update()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO Log (employee_id, action_id, action_date)
    VALUES (current_setting('app.employee_id')::INT, 
            (SELECT id FROM Action WHERE name = 'Update Product'), 
            CURRENT_TIMESTAMP);

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;
CREATE TRIGGER trigger_log_product_update
AFTER UPDATE ON Product
FOR EACH ROW
EXECUTE FUNCTION log_product_update();

CREATE OR REPLACE FUNCTION log_product_deletion()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO Log (employee_id, action_id, action_date)
    VALUES (current_setting('app.employee_id')::INT, 
            (SELECT id FROM Action WHERE name = 'Delete Product'), 
            CURRENT_TIMESTAMP);

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_log_product_deletion
AFTER DELETE ON Product
FOR EACH ROW
EXECUTE FUNCTION log_product_deletion();

CREATE OR REPLACE FUNCTION log_order_status_update()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO Log (employee_id, action_id, action_date)
    VALUES (current_setting('app.employee_id')::INT, 
            (SELECT id FROM Action WHERE name = 'Update Order Status'), 
            CURRENT_TIMESTAMP);

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_log_order_status_update
AFTER UPDATE OF status ON Orders
FOR EACH ROW
EXECUTE FUNCTION log_order_status_update();

CREATE OR REPLACE FUNCTION log_job_addition()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO Log (employee_id, action_id, action_date)
    VALUES (current_setting('app.employee_id')::INT, 
            (SELECT id FROM Action WHERE name = 'Add Job'), 
            CURRENT_TIMESTAMP);

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_log_job_addition
AFTER INSERT ON Job
FOR EACH ROW
EXECUTE FUNCTION log_job_addition();

CREATE OR REPLACE FUNCTION log_job_deletion()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO Log (employee_id, action_id, action_date)
    VALUES (current_setting('app.employee_id')::INT, 
            (SELECT id FROM Action WHERE name = 'Delete Job'), 
            CURRENT_TIMESTAMP);

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_log_job_deletion
AFTER DELETE ON Job
FOR EACH ROW
EXECUTE FUNCTION log_job_deletion();

CREATE OR REPLACE FUNCTION recalculate_cart_total_price_on_insert()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Cart
    SET total_price = (
        SELECT COALESCE(SUM(ci.quantity * p.price), 0)
        FROM CartItem ci
        JOIN Product p ON ci.product_id = p.id
        WHERE ci.cart_id = NEW.cart_id
    )
    WHERE client_id = NEW.cart_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_recalculate_cart_total_price_after_insert
AFTER INSERT ON CartItem
FOR EACH ROW
EXECUTE FUNCTION recalculate_cart_total_price_on_insert();


CREATE OR REPLACE FUNCTION recalculate_cart_total_price_on_update()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Cart
    SET total_price = (
        SELECT COALESCE(SUM(ci.quantity * p.price), 0)
        FROM CartItem ci
        JOIN Product p ON ci.product_id = p.id
        WHERE ci.cart_id = NEW.cart_id
    )
    WHERE client_id = NEW.cart_id;

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_recalculate_cart_total_price_after_update
AFTER UPDATE ON CartItem
FOR EACH ROW
EXECUTE FUNCTION recalculate_cart_total_price_on_update();


CREATE OR REPLACE FUNCTION recalculate_cart_total_price_on_delete()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE Cart
    SET total_price = (
        SELECT COALESCE(SUM(ci.quantity * p.price), 0)
        FROM CartItem ci
        JOIN Product p ON ci.product_id = p.id
        WHERE ci.cart_id = OLD.cart_id
    )
    WHERE client_id = OLD.cart_id;

    RETURN OLD;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER trigger_recalculate_cart_total_price_after_delete
AFTER DELETE ON CartItem
FOR EACH ROW
EXECUTE FUNCTION recalculate_cart_total_price_on_delete();
