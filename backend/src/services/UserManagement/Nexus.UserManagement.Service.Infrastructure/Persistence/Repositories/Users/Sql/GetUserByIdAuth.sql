SELECT
    u.id::text as Id,
    u.login as Login,
    d.encrypted_value as EncryptedDek,
    d.crypto_version as DekVersion,
    ua.srp_encrypted_verifier as EncryptedVerifier,
    ua.srp_salt as ClientSalt,
    ua.srp_version as SrpVersion,
    ua."srp_encrypted_verifier_wrapKey" as EncryptedVerifierWrapKey,
    ua.srp_key_wrap_version as KeyWrapVersion,
    ua.srp_asymmetric_key_id as AsymmetricKeyId,
    (
        SELECT json_agg(r.name)
        FROM user_roles ur
        JOIN roles r ON ur.role_id = r.id
        WHERE ur.user_id = u.id
    ) as Roles
FROM users u 
JOIN deks d ON d.user_id = u.id
JOIN user_authenticators ua ON ua.user_id = u.id
WHERE u.id = @userId 
    AND d.dek_type = 1 
    AND ua."Method" = 1