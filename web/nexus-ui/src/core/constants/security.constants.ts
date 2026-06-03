import { CryptoVersion, SrpGroup } from "@crossdyne/security";

export const CryptoConstants = {
    SALT_SIZE_BYTES: 32,
    RECOVERY_KEYS_COUNT: 10,
    KEY_SIZE_BYTES: 32,
    ACTUAL_SRP_GROUP: SrpGroup.Rfc5054_3072,
    ACTUAL_CRYPTO_VERSION: CryptoVersion.V1
} as const;