import { Injectable } from "@angular/core";
import { CryptoService, CryptoVersion, SecurityUtils, SrpContext, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { CryptoConstants } from "../constants/security.constants";

@Injectable({
    providedIn: 'root'
})
export class CryptoConfigurationService {
    private readonly crypto = new CryptoService();

    async getSrpGroup(): Promise<SrpGroup> {
        return CryptoConstants.ACTUAL_SRP_GROUP;
    }

    getCryptoVersion(): CryptoVersion {
        return CryptoConstants.ACTUAL_CRYPTO_VERSION;
    }

    generateSalt(): { rawSalt: Uint8Array, saltBase64: string } {
        const raw = this.crypto.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES);
        const base64 = SecurityUtils.toBase64(raw);
        return { rawSalt: raw, saltBase64: base64 };
    }
}