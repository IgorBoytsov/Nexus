export interface RecoveryPasswordRequest{
    Login: string;
    Verifier: string;
    ClientSalt: string;
    EncryptedDek: string;
    EncryptionAlgorithm: string;
    Iterations: number;
    KdfType: string;
}