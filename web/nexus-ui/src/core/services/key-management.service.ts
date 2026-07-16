import { Injectable } from "@angular/core";
import { CryptoService, CryptoVersion, KeyDerivationService, SecurityUtils } from "@crossdyne/security";
import { CryptoConstants } from "../constants/security.constants";

@Injectable({
    providedIn: 'root'
})
export class KeyManagementService {
    private keyDerivation = new KeyDerivationService();
    private crypto = new CryptoService();

    async generateAndEncryptDek(
        login: string, 
        password: string, 
        salt: Uint8Array<ArrayBufferLike>,
        cryptoVersion: CryptoVersion
    ): Promise<{ rawDek: Uint8Array, encryptedDekBase64: string }> {
         const { kek } = await this.keyDerivation.deriveKeysFromPassword(login, password, salt, cryptoVersion);
         
         const dek = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
         const encryptedDek = await this.crypto.encryptData(dek, kek, cryptoVersion);

         return { rawDek: dek, encryptedDekBase64: encryptedDek };
    }

    async reEncryptDekWithNewPassword(
        login: string,
        oldPassword: string,
        newPassword: string,
        storageDekSalt: string,
        storageEncryptedDek: string,
        newDekSalt: Uint8Array<ArrayBufferLike>,
        cryptoVersion: CryptoVersion,
    ): Promise<string> {
        const storageDekSaltBytes = SecurityUtils.fromBase64(storageDekSalt);
        
        const { kek: oldKek } = await this.keyDerivation.deriveKeysFromPassword(login, oldPassword, storageDekSaltBytes, cryptoVersion);
        const decryptedDek = await this.crypto.decryptData<Uint8Array>(storageEncryptedDek, oldKek, true);
       
        const { kek: newKek } = await this.keyDerivation.deriveKeysFromPassword(login, newPassword, newDekSalt, cryptoVersion);
        const reEncryptedDek = await this.crypto.encryptData(decryptedDek!, newKek, cryptoVersion);

        return reEncryptedDek;
    } 

    async reEncryptExistingDek(
        login: string,
        newPassword: string,
        dek: string,
        dekSalt: Uint8Array<ArrayBufferLike>,
        cryptoVersion: CryptoVersion
    ) : Promise<{ rawDek: Uint8Array, encryptedDekBase64: string }> {
        const { kek } = await this.keyDerivation.deriveKeysFromPassword(login!, newPassword, dekSalt, cryptoVersion);
        const rawDekBytes = SecurityUtils.fromBase64(dek);
        const encryptedDek = await this.crypto.encryptData(rawDekBytes, kek, cryptoVersion);

        return { rawDek: rawDekBytes, encryptedDekBase64: encryptedDek };
    }

}