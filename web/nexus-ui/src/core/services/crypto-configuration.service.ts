import { Injectable } from "@angular/core";
import { CryptoProfile, CryptoProfileRegistry, CryptoService, CryptoVersion, SecurityUtils, SrpContext, SrpContextFactory, SrpGroup } from "@crossdyne/security";
import { CryptoConstants } from "../constants/security.constants";

@Injectable({
    providedIn: 'root'
})
export class CryptoConfigurationService {
    private readonly crypto = new CryptoService();

    async getSrpContext(): Promise<{ srpContext: SrpContext, srpGroup: SrpGroup }> {
        const group = CryptoConstants.ACTUAL_SRP_GROUP;
        const context = await SrpContextFactory.create(group);
        return { srpContext: context, srpGroup: group};
    }

    getCryptoProfile(cryptoVersion: CryptoVersion | null = null): CryptoProfile {
        if (cryptoVersion != null){
            return CryptoProfileRegistry.getProfile(cryptoVersion);
        }

        return CryptoProfileRegistry.getProfile(CryptoConstants.ACTUAL_CRYPTO_VERSION);
    }

    generateSalt(): { rawSalt: Uint8Array, saltBase64: string } {
        const raw = this.crypto.generateRandomBytes(CryptoConstants.SALT_SIZE_BYTES);
        const base64 = SecurityUtils.toBase64(raw);
        return { rawSalt: raw, saltBase64: base64 };
    }
}