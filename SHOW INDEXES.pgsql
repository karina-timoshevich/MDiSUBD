SELECT
    indexname AS index_name,
    tablename AS table_name,
    indexdef AS index_definition
FROM
    pg_indexes
WHERE
    schemaname = 'public';  


