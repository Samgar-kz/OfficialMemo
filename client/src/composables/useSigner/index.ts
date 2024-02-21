import {
  BPMAgent_CheckRun,
  BPMAgent_OfficeSign,
  BPMAgent_SendIsNotValidReason,
  BPMAgent_SupervisorSign,
  BPMAgent_RegisterDocument,
} from "@/utils/BPMAgentScripts.js";

import type { MyDocument } from "@/types/contents/MyDocument";
import { NCALayerClient } from "ncalayer-js-client";
import type { Model } from "@/types/process/Model";
import type { SignMessageDocument } from "@/types/process/SignModel";
import getDocumentsData, { getPdfData } from "@/server/api/contents/download";
import { ref } from "vue";
import type { DocumentData } from "@/types/DocumentData";

export default function useSigner() {
  async function supervisorSignWithBPMAgent(
    message: Model<any>
  ): Promise<SignMessageDocument> {
    //if (message.data.attachments.length == 0) return "Не подписан";
    const documents = ref<DocumentData[]>(
      await getDocumentsData(message.data.attachments)
    );
    console.log("DocumentsForSign", documents.value);
    console.log("ATTACHMENTS", message.data.attachments);
    documents.value.push(await getPdfData(message.originalDocumentUrl));
    const BPMAgentResult = BPMAgent_SupervisorSign(documents.value);
    console.log(BPMAgentResult);
    const base64str = JSON.parse(BPMAgentResult);
    console.log("supervisorSignWithBPMAgent: ", base64str);
    return base64str;
  }
  async function CheckRun_BPMAgent(error) {
    return BPMAgent_CheckRun(error);
  }
  async function officeSignWithBPMAgent(
    supervisorSignDoc: MyDocument,
    message: Model<any>
  ) {
    // if (!message.data.attachments || message.data.attachments.length == 0) return "Не подписан";
    const documents = await getDocumentsData(message.data.attachments);
    const pdfFile = await getPdfData(message.originalDocumentUrl);
    documents.push(pdfFile);

    if (supervisorSignDoc == null) return "Не подписан";
    const signDocs = ref<MyDocument[]>([]);
    signDocs.value.push(supervisorSignDoc);
    const signDocuments = await getDocumentsData(signDocs.value);
    console.log("signDocuments: ", signDocuments);
    console.log("documents: ", documents);

    const cutIdx = message.data.signData!.signDocument!.name!.indexOf(".");
    const docAuthor = message.data.signData!.signDocument.name.substring(
      0,
      cutIdx
    );

    const base64str = BPMAgent_OfficeSign(
      message.data.regNum,
      docAuthor,
      documents,
      signDocuments
    );
    return base64str;
  }
  const SignIsNotValidReason = function (comment) {
    const data = BPMAgent_SendIsNotValidReason(comment);
    return data;
  };
  async function registerDocument(regNum) {
    const data = BPMAgent_RegisterDocument(regNum);
    return data;
  }
  function stringToB64(str: string) {
    if (!str) return "";
    const codeUnits = new Uint16Array(str.length);
    for (let i = 0; i < codeUnits.length; i++) {
      codeUnits[i] = str.charCodeAt(i);
    }
    const charCodes = new Uint8Array(codeUnits.buffer);

    let result = "";
    for (let i = 0; i < charCodes.byteLength; i++) {
      result += String.fromCharCode(charCodes[i]);
    }
    return btoa(result);
  }

  const ncalayerClient = ref(new NCALayerClient());
  const storageType = ref<string>(NCALayerClient.fileStorageType);

  async function createConnection() {
    try {
      await ncalayerClient.value.connect();
    } catch (error) {
      reportError(error);
      throw error;
    }
  }

  async function signData(data: string) {
    try {
      const base64EncodedSignature =
        await ncalayerClient.value.createCAdESFromBase64(
          storageType.value,
          stringToB64(data),
          "SIGNATURE",
          true
        );
      console.log(base64EncodedSignature);
      // debugger;
      // alert("SIGNED");
      return base64EncodedSignature;
    } catch (error) {
      reportError(error);
      if (error instanceof Error && error.message === "500: action.canceled") {
        error.message = "Отмена подписания";
      }
      throw error;
    }
  }
  return {
    supervisorSignWithBPMAgent,
    officeSignWithBPMAgent,
    createConnection,
    signData,
    CheckRun_BPMAgent,
    SignIsNotValidReason,
    registerDocument,
  };
}
