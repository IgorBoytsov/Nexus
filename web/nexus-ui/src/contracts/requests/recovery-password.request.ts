export interface RecoveryPasswordRequest{
    login: string;
    // Srp
    encryptedVerifier: string;
    srpSalt: string;
    srpVersion: number,
    encryptedVerifierWrapKey: string, 
    keyWrapVersion: number, 
    asymmetricKeyId: string,
    // Dek 
    encryptedDek: string;
    dekSalt: string;
    cryptoVersion: number;
    // RecoveryKeys
    recoveryKeys: Array<{encryptedValue: string, cryptoVersion: number}>;
}