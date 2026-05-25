export interface ChangePasswordRequest {
    userId: string | null;
    verifier: string;
    clientSalt: string; 
    encryptedDek: string;
    cryptoVersion: number; 
    srpVersion: number;
    encryptedVerifierWrapKey: string; 
    keyWrapVersion: number; 
    asymmetricKeyId: string;
}