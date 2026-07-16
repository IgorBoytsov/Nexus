import { Injectable } from "@angular/core";
import { CryptoService, CryptoVersion, SecurityUtils } from "@crossdyne/security";
import { CryptoConstants } from "../constants/security.constants";

@Injectable({
    providedIn: 'root'
})
export class RecoveryKeyService {

    async generateKeys(
        crypto: CryptoService, 
        dek: Uint8Array<ArrayBufferLike>, 
        countKeys: number, 
        cryptoVersion: CryptoVersion
    ) : Promise<{recoveryKeysForDisplay: string[], recoveryAssets: Array<{encryptedDek: string, rowKey: Uint8Array, version: CryptoVersion}>}>{
        
        let recoveryKeysDisplay: string[] = [];
        let recoveryAssets: Array<{encryptedDek: string, rowKey: Uint8Array, version: CryptoVersion}> = [];

         for (let index = 0; index < countKeys; index++) {
            const rowKey = crypto.generateRandomBytes(CryptoConstants.KEY_SIZE_BYTES); // 32
            const encryptedDek = await crypto.encryptData(dek, rowKey, cryptoVersion);
            recoveryKeysDisplay.push(SecurityUtils.toBase64(rowKey));
            recoveryAssets.push({encryptedDek: encryptedDek, rowKey: rowKey, version: cryptoVersion })
        }

        return { recoveryKeysForDisplay: recoveryKeysDisplay, recoveryAssets: recoveryAssets }
    }
}