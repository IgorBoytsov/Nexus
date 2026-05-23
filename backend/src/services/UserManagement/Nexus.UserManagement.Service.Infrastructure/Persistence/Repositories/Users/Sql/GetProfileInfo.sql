SELECT 
    login as Login, 
    email as Email
FROM users u
WHERE u.id = @userId