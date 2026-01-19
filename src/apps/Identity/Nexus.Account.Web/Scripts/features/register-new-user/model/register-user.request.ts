export interface RegisterRequest {
    Login: string,
    UserName: string,
    Verifier: string,
    ClientSalt: string,
    EncryptedDek: string,
    Email: string,
    Phone: string,
    IdGender: string,
    IdCountry: string
}