import { Injectable } from "@angular/core";
import { CryptoProfile, CryptoService, KeyDerivationService, SecurityUtils } from "@crossdyne/security";
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
        profile: CryptoProfile
    ): Promise<{ rawDek: Uint8Array, encryptedDekBase64: string }> {
         const { kek } = await this.keyDerivation.deriveKeysFromPassword(login, password, salt);
         
         const dek = this.crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
         const encryptedDek = await this.crypto.encryptData(dek, kek, profile.aesGcmOptions);

         return { rawDek: dek, encryptedDekBase64: encryptedDek };
    }

    async reEncryptDekWithNewPassword(
        login: string,
        oldPassword: string,
        newPassword: string,
        storageDekSalt: string,
        storageEncryptedDek: string,
        newDekSalt: Uint8Array<ArrayBufferLike>,
        profile: CryptoProfile,
    ): Promise<string> {
        const storageDekSaltBytes = SecurityUtils.fromBase64(storageDekSalt);
        
        const { kek: oldKek } = await this.keyDerivation.deriveKeysFromPassword(login, oldPassword, storageDekSaltBytes, profile.kdfOptions);
        const decryptedDek = await this.crypto.decryptData<Uint8Array>(storageEncryptedDek, oldKek, profile.aesGcmOptions, true);
       
        const { kek: newKek } = await this.keyDerivation.deriveKeysFromPassword(login, newPassword, newDekSalt, profile.kdfOptions);
        const reEncryptedDek = await this.crypto.encryptData(decryptedDek!, newKek, profile.aesGcmOptions);

        return reEncryptedDek;
    } 

    async reEncryptExistingDek(
        login: string,
        newPassword: string,
        dek: string,
        dekSalt: Uint8Array<ArrayBufferLike>,
        profile: CryptoProfile
    ) : Promise<{ rawDek: Uint8Array, encryptedDekBase64: string }> {
        const { kek } = await this.keyDerivation.deriveKeysFromPassword(login!, newPassword, dekSalt);
        const rawDekBytes = SecurityUtils.fromBase64(dek);
        const encryptedDek = await this.crypto.encryptData(rawDekBytes, kek, profile.aesGcmOptions);

        return { rawDek: rawDekBytes, encryptedDekBase64: encryptedDek };
    }

}