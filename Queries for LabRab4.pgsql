--Нужно получить список всех сотрудников, чьи фамилии начинаются на букву "S",
-- а их должность не является "Developer"

select * from employee;
select * from position;
select first_name, last_name, position_id from employee
where last_name like 'S%' and position_id not in
(select id from position where name = 'Developer') ;
select id from position where name = 'Manager'