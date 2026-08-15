import { Injectable } from "@angular/core";
import { CryptoService, CryptoVersion, KeyDerivationService, SecurityUtils, SrpClientService, SrpGroup, SrpKeyDerivationService } from "@crossdyne/security";
import { CryptoConstants } from "../constants/security.constants";
@Injectable({
    providedIn: 'root'
})
export class SrpVerifierService {
    private readonly keyDerivation = new KeyDerivationService();
    private readonly srpKeyDerivationService = new SrpKeyDerivationService();
    private readonly crypto = new CryptoService();
    private readonly srp = new SrpClientService();

    async generateVerifier(
        login: string, 
        password: string, 
        rsaPublicKey: CryptoKey, 
        srpSalt: Uint8Array<ArrayBufferLike>, 
        srpGroup: SrpGroup, 
        cryptoVersion: CryptoVersion) : Promise<{ encryptedVerifier: string, encryptedVerifierWrapKeyBase64: string}> {
        
        const srpAuthHashBytes = await this.srpKeyDerivationService.deriveAuthHashForSrp(login, password, srpSalt, srpGroup, cryptoVersion);
        const srpAuthHashBase64 = SecurityUtils.toBase64(srpAuthHashBytes);
        const verifierBase64 = await this.srp.generateSrpVerifier(srpAuthHashBase64, srpGroup);
        const dekForVerifier = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32

        const encryptedVerifierBase64 = await this.crypto.encryptData(verifierBase64, dekForVerifier, cryptoVersion);

        const encryptedKekForVerifier = await window.crypto.subtle.encrypt(
            { name: "RSA-OAEP" },
            rsaPublicKey,
            dekForVerifier.buffer as ArrayBuffer
        );

        const encryptedVerifierWrapKeyBase64 = SecurityUtils.toBase64(new Uint8Array(encryptedKekForVerifier));

        return { encryptedVerifier: encryptedVerifierBase64, encryptedVerifierWrapKeyBase64 };
    }
}