SELECT 
    u.login as Login, 
    d.encrypted_value as EncryptedDek, 
    d.salt as DekSalt, 
    d.crypto_version as CryptoVersionDek, 
    ua.srp_asymmetric_key_id as AsymmetricKeyId
FROM users u
JOIN deks d ON d.user_id = u.id
JOIN user_authenticators ua ON d.user_id = u.id
WHERE d.dek_type = 1 AND ua."Method" = 1 AND u.id = @userId