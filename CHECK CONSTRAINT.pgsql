SELECT conname
FROM pg_constraint
WHERE conrelid = 'Position'::regclass;
