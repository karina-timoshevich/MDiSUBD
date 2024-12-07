ALTER TABLE Client
ADD COLUMN email VARCHAR(100) UNIQUE;

ALTER TABLE Employee
ADD COLUMN password VARCHAR(100);

UPDATE Client
SET email = CONCAT('client', id, '@example.com');

UPDATE Employee
SET password = 'temporary_password';

ALTER TABLE Client
ALTER COLUMN email SET NOT NULL;

ALTER TABLE Employee
ALTER COLUMN password SET NOT NULL;

ALTER TABLE Client
ADD CONSTRAINT unique_email_client UNIQUE (email);

ALTER TABLE Employee
ADD CONSTRAINT unique_email_employee UNIQUE (email);

SELECT 
    table_name,
    column_name,
    data_type
FROM 
    information_schema.columns
WHERE 
    table_schema = 'public'
ORDER BY 
    table_name, ordinal_position;


