SELECT
    d.salt as ClientSalt,
    d.encrypted_value as EncryptedDek
FROM users u 
JOIN deks d ON d.user_id = u.id
JOIN user_authenticators ua ON ua.user_id = u.id
WHERE u.login = @login
    AND d.dek_type = 1 
    AND ua."Method" = 1