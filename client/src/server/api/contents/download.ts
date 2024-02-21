import type { DocumentData } from "@/types/DocumentData";
import type { MyDocument } from "@/types/contents/MyDocument";
import { ref } from "vue";
import contentsAxios from "./contentsAxios";

export default async function getDocumentsData(
  documents: MyDocument[]
): Promise<DocumentData[]> {
  const files: DocumentData[] = [];
  if (documents?.length) {
    for (let i = 0; i < documents.length; i++) {
      await getDocumentAsBase64(documents[i].url).then((base64) => {
        files.push({ name: documents[i].name, data: base64 });
      });
    }
  }
  return files;
}
async function getDocumentAsBase64(url: string): Promise<string> {
  const urlStr = url.substring(url.lastIndexOf("/api") + 4);
  const { data } = await contentsAxios.get<string>("/getBase64" + urlStr);
  console.log(data);
  return data;
}
export async function getPdfData(documentUrl: string): Promise<DocumentData> {
  const file = ref<DocumentData>();
  if (documentUrl != null) {
    const base64 = await getDocumentAsBase64(documentUrl);
    file.value = { name: "Document.pdf", data: base64 };
  }
  return file.value;
}
