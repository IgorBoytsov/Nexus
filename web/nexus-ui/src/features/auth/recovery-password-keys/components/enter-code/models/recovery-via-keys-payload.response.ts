export interface RecoveryViaKeysPayloadResponse {
    recoveryKeys: Array<RecoveryKeysResponse>;
}

export interface RecoveryKeysResponse {
    key: string;
    cryptoVersion: number;
}