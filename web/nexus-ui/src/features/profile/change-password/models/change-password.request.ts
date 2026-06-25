export interface ChangePasswordRequest {
    userId: string | null;
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
}