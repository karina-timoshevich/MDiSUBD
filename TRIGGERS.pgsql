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

--над возвратом подумать

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