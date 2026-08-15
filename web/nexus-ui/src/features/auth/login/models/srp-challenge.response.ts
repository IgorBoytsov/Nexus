export interface SrpChallengeResponse {
    salt: string;
    b: string;
    srpVersion: number;
    srpCryptoVersion: number;
}