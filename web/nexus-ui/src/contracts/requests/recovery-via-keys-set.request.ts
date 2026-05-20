export interface RecoveryViaKeysSetRequest {
    login: string,
    verifier: string,
    clientSalt: string,
    encryptedVerifierWrapKey: string ,
    cryptoVersion: number,
    srpVersion: number,
    encryptedDek: string, 
    keyWrapVersion: number,
    asymmetricKeyId: string,
    recoveryKeys: Array<RecoveryKeyRequestData>;
}

export interface RecoveryKeyRequestData {
    encryptedValue: string;
    cryptoVersion: number;
}