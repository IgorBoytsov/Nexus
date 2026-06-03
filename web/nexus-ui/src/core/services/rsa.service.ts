import { inject, Injectable } from "@angular/core";
import { firstValueFrom } from "rxjs";
import { CryptoApi } from "../clients/crypto.api";
import { SecurityUtils } from "@crossdyne/security";

@Injectable({
    providedIn: 'root'
})
export class RsaService {
    private cryptoApi = inject(CryptoApi);

    async getPublicKey() : Promise<CryptoKey> {
        const publicKeyResponse = await firstValueFrom(this.cryptoApi.getPublicKey());

        const firstParse = JSON.parse(publicKeyResponse.publicKey);
        const publicKeyBase64 = typeof firstParse === 'string' ? firstParse : firstParse.publicKey
        const binaryKey = SecurityUtils.fromBase64(publicKeyBase64);

        const rsaPublicKey = await window.crypto.subtle.importKey(
            "spki",
            binaryKey.buffer as ArrayBuffer, {
                name: "RSA-OAEP",
                hash: "SHA-256"
            },
            false,
            ["encrypt"]
        );

        return rsaPublicKey;
    }
}