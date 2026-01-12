export interface RegisterRequest {
    Login: string,
    UserName: string,
    Password: string,
    ClientSalt: string,
    EncryptedDek: string,
    Email: string,
    Phone: string,
    IdGender: string,
    IdCountry: string
}