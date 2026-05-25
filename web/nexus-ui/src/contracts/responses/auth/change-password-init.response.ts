export interface ChangePasswordInitResponse {
    login: string;
    encryptedDek: string;
    cryptoVersionDek: number;
    clientSalt: string;
    asymmetricKeyId: string;
}