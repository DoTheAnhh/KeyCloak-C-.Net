export interface RealmResponse {
    realm: string;
    displayName: string;
    enabled: boolean;
}

export interface RealmRequest {
    realm?: string;
    displayName?: string;
    enabled?: boolean;
}