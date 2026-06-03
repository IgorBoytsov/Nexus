export interface GetChangePasswordDataResponse {
    login: string;
    encryptedDek: string;
    dekSalt: string;
    cryptoVersionDek: number;
    asymmetricKeyId: string;
    srvVersion: number;
}