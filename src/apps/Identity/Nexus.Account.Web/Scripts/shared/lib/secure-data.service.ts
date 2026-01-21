export class SecureDataService {
    private readonly NONCE_SIZE = 12;
    private readonly TAG_SIZE = 16;
    private readonly ITERATIONS = 600_000;
    private readonly KEY_SIZE = 32;

    private static readonly N = BigInt("0xAC6BDB41324A9A9BF166DE5E1F403D434A6E1B3B94A7E62AC1211858E002C75AD4455C9D19C0A3180296917A376205164043E20144FF485719D181A99EB574671AC58054457ED444A67032EA17D03AD43464D2397449CA593630A670D90D95A78E846A3C8AF80862098D80F33C42ED7059E75225E0A52718E2379369F65B79680A6560B080092EE71986066735A96A7D42E7597116742B02D3A154471B6A23D84E0D642C790D597A2BB7F5A48F734898BDD138C69493E723491959C1B4BD40C91C1C7924F88D046467A006507E781220A80C55A927906A7C6C9C227E674686DD5D1B855D28F0D604E24586C608630B9A34C4808381A54F0D9080A5F90B60187F");
    private static readonly g = BigInt(2);
    private static readonly k = BigInt(3);

    async deriveKeysFromPassword(password: string, salt: Uint8Array): Promise<{ kek: Uint8Array, authHash: string }> {
        const encoder = new TextEncoder();
        const passwordBytes = encoder.encode(password);

        const baseKey = await crypto.subtle.importKey('raw', passwordBytes, 'PBKDF2', false, ['deriveBits', 'deriveKey']);

        const masterKeyBits = await crypto.subtle.deriveBits(
            {
                name: 'PBKDF2',
                salt: salt as any,
                iterations: this.ITERATIONS,
                hash: 'SHA-256'
            },
            baseKey,
            this.KEY_SIZE * 8
        );

        const masterKey = await crypto.subtle.importKey(
            'raw',
            masterKeyBits,
            'HKDF',
            false,
            ['deriveBits']
        );

        const kek = await crypto.subtle.deriveBits(
            {
                name: 'HKDF',
                hash: 'SHA-256',
                salt: new Uint8Array(0),
                info: encoder.encode("AES-GCM-KEK-v1")
            },
            masterKey,
            this.KEY_SIZE * 8
        );

        const authBytes = await crypto.subtle.deriveBits(
            {
                name: 'HKDF',
                hash: 'SHA-256',
                salt: new Uint8Array(0),
                info: encoder.encode("SERVER-AUTH-HASH-v1")
            },
            masterKey,
            this.KEY_SIZE * 8
        );

        return {
            kek: new Uint8Array(kek),
            authHash: this.toBase64(new Uint8Array(authBytes))
        }
    }

    async encryptData<T>(dataModel: T, keyBytes: Uint8Array): Promise<string> {
        const encoder = new TextEncoder();

        let jsonString: string;
        if (dataModel instanceof Uint8Array) {
            jsonString = `"${this.toBase64(dataModel)}"`;
        } else {
            jsonString = JSON.stringify(dataModel);
        }

        const plainBytes = encoder.encode(jsonString);
        const nonce = crypto.getRandomValues(new Uint8Array(this.NONCE_SIZE));

        const cryptoKey = await crypto.subtle.importKey('raw', keyBytes as any, 'AES-GCM', false, ['encrypt']);

        const encryptedContent = await crypto.subtle.encrypt(
            {
                name: 'AES-GCM',
                iv: nonce,
                tagLength: this.TAG_SIZE * 8
            },
            cryptoKey,
            plainBytes
        );

        const result = new Uint8Array(this.NONCE_SIZE + encryptedContent.byteLength);
        result.set(nonce, 0);
        result.set(new Uint8Array(encryptedContent), this.NONCE_SIZE);

        return this.toBase64(result);
    }

    async decryptData<T>(encryptedBase64: string, keyBytes: Uint8Array): Promise<T | null> {
        if (!encryptedBase64) return null;

        const encryptedBytes = this.fromBase64(encryptedBase64);

        if (encryptedBytes.length < this.NONCE_SIZE + this.TAG_SIZE) {
            throw new Error("Недопустимый формат зашифрованных данных");
        }

        const nonce = encryptedBytes.slice(0, this.NONCE_SIZE);
        const ciphertextWithTag = encryptedBytes.slice(this.NONCE_SIZE);

        const cryptoKey = await crypto.subtle.importKey(
            'raw',
            keyBytes as any,
            'AES-GCM',
            false,
            ['decrypt']
        );

        try {
            const decryptedBuffer = await crypto.subtle.decrypt(
                {
                    name: 'AES-GCM',
                    iv: nonce,
                    tagLength: this.TAG_SIZE * 8
                },
                cryptoKey,
                ciphertextWithTag
            );

            const decoder = new TextDecoder();
            const jsonString = decoder.decode(decryptedBuffer);
            return JSON.parse(jsonString) as T;
        } catch (e) {
            console.error("Ошибка дешифрования:", e);
            return null;
        }
    }

    generateRandomBytes = (length: number = 32): Uint8Array => crypto.getRandomValues(new Uint8Array(length)); 

    async generateSrpVerifier(authHash: string): Promise<string> {
        const xBytes = this.fromBase64(authHash);
        const x = BigInt('0x' + Array.from(xBytes).map(b => b.toString(16).padStart(2, '0')).join(''));
        const v = this.expMod(SecureDataService.g, x, SecureDataService.N);

        return v.toString(16);
    }

    async generateSrpProof(password: string, saltBase64: string, B_hex: string): Promise<{ A: string, M1: string, S: string }> {
        const N = SecureDataService.N;
        const g = SecureDataService.g;
        const k = SecureDataService.k;

        const salt = this.fromBase64(saltBase64);
        const { authHash } = await this.deriveKeysFromPassword(password, salt);
        const x = this.bigIntFromBase64(authHash);

        const aBytes = crypto.getRandomValues(new Uint8Array(32));
        const a = BigInt('0x' + Array.from(aBytes).map(b => b.toString(16).padStart(2, '0')).join(''));

        const A = this.expMod(g, a, N);
        const B = BigInt('0x' + B_hex);

        const u = await this.hashBigInt(A, B);

        if (u === BigInt(0)) throw new Error("Недопустимое значение u в SRP");

        const gX = this.expMod(g, x, N);
        const term = (k * gX) % N;
        const base = (B - term + N) % N;
        const exponent = a + (u * x);
        const S = this.expMod(base, exponent, N);

        const M1 = await this.hashBigInt(A, B, S);

        return {
            A: A.toString(16),
            M1: M1.toString(16),
            S: S.toString(16)
        }
    }

    async verifyServerM2(A_hex: string, M1_hex: string, S_hex: string, serverM2_hex: string): Promise<boolean> {
        const A = BigInt('0x' + A_hex);
        const M1 = BigInt('0x' + M1_hex);
        const S = BigInt('0x' + S_hex);

        const computedM2 = await this.hashBigInt(A, M1, S);
        const expectedM2Hex = computedM2.toString(16).toLowerCase().padStart(64, '0');

        return serverM2_hex.toLowerCase().replace(/^0+/, '') === expectedM2Hex.replace(/^0+/, '');
    }

    /*                          
        Публичные вспомогательные методы
    */

    public toBase64 = (bytes: Uint8Array): string => {
        let binary = '';
        const len = bytes.byteLength;
        for (let i = 0; i < len; i++) {
            binary += String.fromCharCode(bytes[i]);
        }
        return window.btoa(binary);
    };

    public fromBase64 = (base64: string): Uint8Array => {
        if (!base64) {
            throw new Error("Передана пустая строка в fromBase64");
        }

        const cleaned = base64.replace(/-/g, '+').replace(/_/g, '/').replace(/\s/g, '');

        try {
            const binary_string = window.atob(cleaned);
            const len = binary_string.length;
            const bytes = new Uint8Array(len);
            for (let i = 0; i < len; i++) {
                bytes[i] = binary_string.charCodeAt(i);
            }
            return bytes;
        } catch (e) {
            console.error("Критическая ошибка Base64:", cleaned, e);
            throw new Error("Некорректная строка Base64.");
        }
    };

    /*
        Приватные вспомогательные методы
    */

    private expMod(base: bigint, exp: bigint, mod: bigint): bigint {
        let res = BigInt(1);
        base = base % mod;
        while (exp > 0) {
            if (exp % BigInt(2) === BigInt(1)) res = (res * base) % mod;
            base = (base * base) % mod;
            exp = exp / BigInt(2);
        }
        return res;
    }

    private bigIntFromBase64(base64: string): bigint {
        const bytes = this.fromBase64(base64);
        return BigInt('0x' + Array.from(bytes).map(b => b.toString(16).padStart(2, '0')).join(''));
    }

    private async hashBigInt(...args: bigint[]): Promise<bigint> {
        const encoder = new TextEncoder();
        const hexString = args.map(a => a.toString(16).toLowerCase().padStart(512, '0')).join('');

        const buffer = await crypto.subtle.digest('SHA-256', encoder.encode(hexString));
        const hashArray = Array.from(new Uint8Array(buffer));
        const hashHex = hashArray.map(b => b.toString(16).padStart(2, '0')).join('');

        return BigInt('0x' + hashHex);
    }
}