export interface RegisterRequest {
    Login: string,
    UserName: string,
    Verifier: string,
    ClientSalt: string,
    EncryptedDek: string,
    EncryptionAlgorithm: string,
    Iterations: number,
    KdfType: string,
    Email: string,
    Phone: string,
    IdGender: string,
    IdCountry: string
}