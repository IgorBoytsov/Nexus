export interface RegisterRequest {
    login: string;
    userName: string;
    verifier: string;
    clientSalt: string;
    encryptedDek: string;
    encryptionAlgorithm: string;
    iterations: number;
    kdfType: string;
    email: string;
    phone: string | null;
    idGender: string | null;
    idCountry: string | null;
}