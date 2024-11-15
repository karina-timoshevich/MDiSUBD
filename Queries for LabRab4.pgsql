--Нужно получить список всех сотрудников, чьи фамилии начинаются на букву "S",
-- а их должность не является "Developer"
select * from employee;
select * from position;
select first_name, last_name, position_id from employee
where last_name like 'S%' and position_id not in
(select id from position where name = 'Developer') ;
select id from position where name = 'Manager';

--Получить список продуктов, которые имеют количество на складе больше 50 
--и цену ниже среднего значения по всем продуктам.
select * from product;
select name, price, quantity from product
where quantity>50 and price< (select round(avg(price),2) from product);
select round(avg(price),2) from product;

--Найти клиентов, которые имеют заказы с использованием не 1 промокода
select * from orders;
select * from client;
select first_name, last_name from client
where id in 
(select client_id from orders 
where promo_code_id>1);

--Найти продукты, произведённые только теми производителями, 
--которые имеют номер телефона, начинающийся с '+37529'
select * from manufacturer;
select * from product;
select name, manufacturer_id from product
where manufacturer_id in 
(select id from manufacturer
where phone like '+37529%');

--Получить список всех заказов, у которых сумма превышает среднюю сумму 
--по всем заказам, и статус не равен "Cancelled"
select * from orders;
select id, total_price, status from orders
where total_price < (select round(avg(total_price),2) from orders) and status <> 'Cancelled';
select round(avg(total_price),2) from orders;

--Напишите запрос, который выбирает название и цену продуктов, цена которых 
--превышает или равна средней цене для их типа
select * from product;
select round(avg(price),2), product_type_id from product
group by product_type_id;

select name, price, product_type_id from product p
where price <= 
(select round(avg(price),2) from product 
where product_type_id = p.product_type_id);

--Запрос с подзапросом для получения данных о продуктах со стоимостью
--ниже максимальной по их типу
select name, price, product_type_id 
from product p
where price <> (select max(price) 
from product
where p.product_type_id = product_type_id);

--Напишите запрос, который возвращает список всех заказов 
--и соответствующих данных о клиентах
select c.first_name, c.last_name, o.total_price, o.id from client c
inner join orders o on o.client_id = c.id;

--Получите список всех клиентов и, если есть, их заказы
select c.first_name, c.last_name, o.total_price, o.id from client c
left join orders o on o.client_id = c.id;

--Получите список всех клиентов и заказов
select c.first_name, c.last_name, o.total_price, o.id from client c
full outer join orders o on o.client_id = c.id;

--Создайте запрос для получения всех возможных комбинаций 
--работников и позиций
select e.first_name, e.last_name, p.name from employee e
cross join position p;

--Напишите запрос, который возвращает список всех пар продуктов, 
--произведённых одним и тем же производителем
select p1.name as name1, p2.name as name2
from product p1
join product p2 on p1.manufacturer_id = p2.manufacturer_id and p1.id <> p2.id;

--Получить список клиентов, которые сделали заказы, 
--и суммарную стоимость всех их заказов
select sum(o.total_price) as total_spent, c.first_name, c.last_name from orders o
inner join client c on o.client_id = c.id
group by c.id;

--Получить список продуктов и их производителей, но только для тех продуктов,
--по которым были сделаны заказы
select product.name, manufacturer.name from product
inner join manufacturer on product.manufacturer_id = manufacturer.id
inner join orderitem on orderitem.product_id = product.id
group by product.id, manufacturer.id;

--Получить список клиентов, которые сделали заказы, и только те заказы, 
--которые не отменены
select * from orders;
select client.first_name, client.last_name, orders.id, orders.status from client
inner join orders on client.id = orders.client_id
where orders.status <> 'Cancelled';

--Получить список продуктов, их производителей и статус запасов 
--(достаточно ли на складе)
select product.name, manufacturer.name,
case
    when product.quantity=0 then 'need to replenish supplies'
    when product.quantity<100 then 'will end soon'
    else 'enough for now'
end as stock_status
from product inner join manufacturer on manufacturer.id = product.manufacturer_id;

--Найти производителей и продукты, у которых средняя цена
--выше определенного значения
select product.name, product.price, manufacturer.name from product
inner join manufacturer on product.manufacturer_id = manufacturer.id
where price > 100;

--Напишите запрос, который возвращает список категорий продуктов и общее 
--количество продуктов в каждой категории, где количество продуктов превышает 10
select * from product;
select product_type_id, count(*) as product_count 
from product 
group by product_type_id 
having count(*) < 10;

--Найдите среднюю цену продуктов по каждому типу продукта
select producttype.name, round(avg(price),2) 
from  product
inner join producttype on product.product_type_id = producttype.id
group by  producttype.name;

--Посчитайте, сколько заказов (Orders) было сделано каждым клиентом, и выведите 
--информацию по каждому заказу, добавив колонку с общим количеством заказов клиента.
select orders.id as order_id, first_name, last_name,
count(orders.id) over(partition by client_id) as orders_per_client
from orders inner join client on orders.client_id = client.id;

--Для каждого клиента найдите дату самого первого и самого последнего заказа
select client_id,
first_value(order_date) over(partition by client_id order by order_date asc) as first_order_date,
last_value(order_date) over(partition by client_id order by order_date asc ROWS BETWEEN UNBOUNDED PRECEDING AND UNBOUNDED FOLLOWING) as last_order_date
from orders;

select * from orders;
--Для каждого заказа найдите дату предыдущего и следующего заказа клиента
select client_id, order_date,
lag(order_date) over(partition by client_id order by order_date) as previous_order_date,
lead(order_date) over(partition by client_id order by order_date) as next_order_date
from orders;

--Для каждого производителя подсчитайте количество товаров, которые они производят, 
--и количество товаров на складе
select * from product;
select m.name,
count(p.id) over(partition by p.manufacturer_id) as total_products,
sum(p.quantity) over(partition by p.manufacturer_id) as total_quantity
from product p
inner join manufacturer m on m.id = p.manufacturer_id;

--использовать различные виды ранжирования
select id as order_id, client_id, order_date,
rank() over(order by client_id asc) as rank_by_client,
dense_rank() over (partition by client_id order by order_date asc) as dense_rank_by_client,
row_number() over (order by id asc) as row_number_by_order
from orders;

--Получите список всех клиентов и сотрудников, указав их имена и роли 
--("Client" или "Employee") в едином списке
select first_name, last_name,
'Client' as role 
from client
UNION
select first_name, last_name, 
'Employee' as role
from employee;

--Выведите список клиентов, у которых есть товары в корзине, 
--вместе с их именами и фамилиями
select c.id, c.first_name, c.last_name 
from client c 
where exists (
    select 1 
    from CartItem ci 
    where ci.cart_id = c.id
);

explain
select m.name, 
       count(p.id) as total_products, 
       sum(p.quantity) as total_quantity
from Product p
inner join Manufacturer m on m.id = p.manufacturer_id
group by m.name;

explain analyze
select m.name, 
       count(p.id) as total_products, 
       sum(p.quantity) as total_quantity
from Product p
inner join Manufacturer m on m.id = p.manufacturer_id
group by m.name;

create table TempTable (
    id serial PRIMARY KEY,
    name varchar(50),
    quantity int
);

insert into TempTable (name, quantity)
select name, quantity
from product
where quantity > 100;

select * from TempTable;
drop table TempTable;

/*
INSERT INTO CartItem (product_id, quantity, cart_id)
VALUES 
(1, 3, 1), 
(3, 4, 3),  
(5, 2, 5), 
(2, 3, 2),  
(4, 7, 4);  

INSERT INTO Orders (client_id, order_date, total_price, promo_code_id, pickup_location_id, status)
VALUES 
(1, '2024-10-10 14:00:00', 159.99, 1, 2, 'Shipped'), 
(3, '2024-10-10 15:00:00', 120.00, 2, 2, 'Shipped'), 
(5, '2024-10-10 16:00:00', 80.00, 3, 3, 'Shipped'), 
(2, '2024-10-11 12:00:00', 55.00, 4, 4, 'Shipped'),
(4, '2024-10-12 17:00:00', 85.00, 5, 5, 'Shipped');
INSERT INTO OrderItem (product_id, quantity, order_id)
VALUES 
(1, 3, 6),  
(2, 3, 7),  
(5, 2, 8),  
(4, 7, 9),  
(3, 4, 10);  

INSERT INTO CartItem (product_id, quantity, cart_id)
VALUES 
(1, 2, 3);
INSERT INTO Orders (client_id, order_date, total_price, promo_code_id, pickup_location_id, status)
VALUES 
(3, '2024-10-10 14:00:00', 159.99, 1, 2, 'Shipped');
INSERT INTO OrderItem (product_id, quantity, order_id)
VALUES 
(1, 2, 9);
*/

select first_name, last_name, name, product_count
from (
    select
        c.first_name, 
        c.last_name, 
        p.name, 
        count(oi.product_id) as product_count,
      row_number() over(partition by c.id order by count(oi.product_id)  DESC) as row_num
    from 
        client c
    join orders o on c.id = o.client_id
    join OrderItem oi ON o.id = oi.order_id
    join product p ON oi.product_id = p.id
    group by c.id, p.id
) as ranked
where row_num = 1
order by ranked.first_name, ranked.last_name;