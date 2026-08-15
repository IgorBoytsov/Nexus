SELECT
    d.salt as ClientSalt,
    d.encrypted_value as EncryptedDek,
    d.crypto_version as CryptoVersion,
    u.login as Login
FROM users u 
JOIN deks d ON d.user_id = u.id
JOIN user_authenticators ua ON ua.user_id = u.id
WHERE u.id = @userId
    AND d.dek_type = 1 
    AND ua.method = 1