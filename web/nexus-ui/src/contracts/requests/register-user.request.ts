export interface RegisterRequest {
    login: string;
    userName: string;
    verifier: string;
    clientSalt: string;
    encryptedDek: string;
    cryptoVersion: number,
    email: string;
    idGender: string | null;
    idCountry: string | null;
}