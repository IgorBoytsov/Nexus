export interface RecoveryPasswordRequest{
    Login: string;
    Verifier: string;
    ClientSalt: string;
    EncryptedDek: string;
    cryptoVersion: number,
}