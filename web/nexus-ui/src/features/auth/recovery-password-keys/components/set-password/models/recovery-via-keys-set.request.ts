export interface RecoveryViaKeysSetRequest {
    login: string;
    // Srp
    encryptedVerifier: string;
    srpSalt: string; 
    srpVersion: number;
    newCryptoVersion: number;
    encryptedVerifierWrapKey: string; 
    keyWrapVersion: number; 
    asymmetricKeyId: string;
    // Dek
    encryptedDek: string;
    dekSalt: string;
    cryptoVersion: number; 
    // Recovery Keys
    recoveryKeys: Array<RecoveryKeyRequestData>;
}

export interface RecoveryKeyRequestData {
    encryptedValue: string;
    cryptoVersion: number;
}