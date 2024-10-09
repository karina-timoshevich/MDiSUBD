select name, price from product where price > 20;
select * from product;

--insert into product (name, description, price, product_type_id, manufacturer_id, unit_of_measure_id, quantity)
--values ('Laptop ACER', 'your best friend for doing labs', 658.15, 1, 1, 1, 87);

select sum(quantity) from product group by product_type_id;

select sum(quantity) from product;

update product set price=1.01*price where price>(select avg(price) from product);

select * from product;

select first_name, last_name from employee where last_name like  '____';

select name, quantity from product where product_type_id in 
(select product_type_id
from product 
group by product_type_id 
having sum(quantity) > 180 );

select distinct name from manufacturer;
SELECT DISTINCT manufacturer_id from product order by manufacturer_id asc;

select name from position where name <> 'Developer';

select sum(quantity) from product group by manufacturer_id order by manufacturer_id;

select first_name, last_name from employee where last_name like 'E%';
select first_name, last_name from employee where last_name like '_%t';
select first_name, last_name from employee where first_name not like '%l%';

select round(avg(price),2) from product;
select name, price from product where price < (select avg(price) from product);

select min(price) from product;
select max(price) from product;

select round(avg(price),2) from product group by product_type_id;

select name, price from product
where price > ALL (select avg(price) from product group by product_type_id);

select name, price from product
where price < ANY (select avg(price) from product group by product_type_id);