declare module 'ncalayer-js-client' {
  class NCALayerClient {
    connect: () => Promise<void>;
    getActiveTokens: () => string;
    createCAdESFromBase64: (storageType: string, data: string, keyType = 'SIGNATURE', attach = false) => string
    static fileStorageType: string;

  }
}
