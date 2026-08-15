SELECT 
    login as Login, 
    user_name as UserName,
    email as Email,
    date_registration as DateRegistration,
    avatar_key as AvatarS3Key
FROM users u
WHERE u.id = @userId