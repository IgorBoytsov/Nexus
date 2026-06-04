SELECT 
    (
        SELECT json_agg(json_build_object('Key', rk.encrypted_value, 'CryptoVersion', rk.crypto_version))
        FROM recovery_keys rk 
        WHERE rk.user_id = u.id
     ) AS RecoveryKeys
FROM users u
WHERE u.login = @login