export interface RegisterRequest {
    login: string;
    userName: string;
    verifier: string;
    clientSalt: string;
    encryptedDek: string;
    cryptoVersion: number,
    srpVersion: number,
    encryptedVerifierWrapKey: string, 
    keyWrapVersion: number, 
    asymmetricKeyId: string,
    email: string;
    idGender: string | null;
    idCountry: string | null;
    recoveryKeys: Array<{encryptedValue: string, cryptoVersion: number}>
}