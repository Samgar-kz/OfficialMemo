import type { MyDocument } from "@/types/contents/MyDocument";

interface SignModel {
  requestGuid: string;
  signature: string;
  registerSignature: string | null;
  signDocument: MyDocument;
  data: string;
  signType: string;
}
// export enum signType{
//   Digital ,
//   HandWritten
// }
interface SignMessageDocument {
  name: string;
  data: string;
}

export type { SignModel, SignMessageDocument };
