export interface RegisterRequest {
    // Общая информация об аккаунте
    login: string;
    userName: string;
    email: string;
    idGender: string | null;
    idCountry: string | null;
    // Srp
    encryptedVerifier: string;
    srpSalt: string;
    srpVersion: number;
    encryptedVerifierWrapKey: string;
    keyWrapVersion: number; 
    asymmetricKeyId: string;
    // Dek
    encryptedDek: string;
    dekSalt: string;
    cryptoVersion: number;
    // RecoveryKeys
    recoveryKeys: Array<{encryptedValue: string, cryptoVersion: number}>;
}