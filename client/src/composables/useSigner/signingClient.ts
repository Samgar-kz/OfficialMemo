import { NCALayerClient } from 'ncalayer-js-client'
// const NCALayerClient = require('ncalayer-js-client');
export default class SigningClient {
  ncalayerClient: NCALayerClient
  storageType: string

  constructor() {
    this.ncalayerClient = new NCALayerClient()
    this.storageType = NCALayerClient.fileStorageType
  }

  async createConnection() {
    try {
      await this.ncalayerClient.connect()
    } catch (error) {
      this.reportError(error)
    }
    return this.ncalayerClient
  }

  setStorageType(token?: string) {
    if (token) {
      this.storageType = token
    } else this.storageType = NCALayerClient.fileStorageType
  }

  async getActiveTokens() {
    try {
      const activeTokens = await this.ncalayerClient.getActiveTokens()
      return activeTokens
    } catch (error) {
      this.reportError(error)
    }
  }

  async signData(data: string) {
    try {
      const base64EncodedSignature =
        await this.ncalayerClient.createCAdESFromBase64(
          this.storageType,
          this.stringToB64(data)
        )
      return base64EncodedSignature
    } catch (error) {
      this.reportError(error)
    }
  }

  private stringToB64(str: string) {
    if (!str) return ''
    const codeUnits = new Uint16Array(str.length)
    for (let i = 0; i < codeUnits.length; i++) {
      codeUnits[i] = str.charCodeAt(i)
    }
    const charCodes = new Uint8Array(codeUnits.buffer)

    let result = ''
    for (let i = 0; i < charCodes.byteLength; i++) {
      result += String.fromCharCode(charCodes[i])
    }
    return btoa(result)
  }

  private b64ToString(b64: string) {
    const binary = atob(b64)
    const bytes = new Uint8Array(binary.length)
    for (let i = 0; i < bytes.length; i++) {
      bytes[i] = binary.charCodeAt(i)
    }
    const charCodes = new Uint16Array(bytes.buffer)
    let result = ''
    for (let i = 0; i < charCodes.length; i++) {
      result += String.fromCharCode(charCodes[i])
    }
    return result
  }
  private reportError(error: unknown) {
    if (error instanceof Error) {
      console.error(error)
      alert(error.message)
    }
  }
}
