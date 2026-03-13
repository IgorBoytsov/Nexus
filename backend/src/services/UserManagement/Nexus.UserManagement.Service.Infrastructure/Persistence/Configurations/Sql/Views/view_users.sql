CREATE OR REPLACE VIEW "V_Users" AS 
SELECT
    u."Id",
    u."Login",
    u."UserName",
    u."Email",
    u."Phone",
    u."DateRegistration",
 
    u."IdStatus",
    s."Name" AS "StatusName",
    
    u."IdRole",
    r."Name" AS "RoleName",
    
    u."IdGender",
    g."Name" AS "GenderName",
    
    u."IdCountry",
    c."Name" AS "CountryName"
    
FROM "Users" AS u
LEFT JOIN "Statuses"  AS s ON u."IdStatus" = s."Id" 
LEFT JOIN "Roles"     AS r ON u."IdRole" = r."Id"
LEFT JOIN "Genders"   AS g ON u."IdGender" = g."Id"
LEFT JOIN "Countries" AS c ON u."IdCountry" = c."Id";