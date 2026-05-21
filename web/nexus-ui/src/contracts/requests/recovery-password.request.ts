export interface RecoveryPasswordRequest{
    login: string;
    verifier: string;
    clientSalt: string;
    encryptedDek: string;
    cryptoVersion: number,
    srpVersion: number,
    encryptedVerifierWrapKey: string, 
    keyWrapVersion: number, 
    asymmetricKeyId: string,
    recoveryKeys: Array<{encryptedValue: string, cryptoVersion: number}>
}