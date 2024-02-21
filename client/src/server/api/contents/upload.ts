import type { MyDocument } from "@/types/contents/MyDocument";
import contentsAxios from "./contentsAxios";

export default async function uploadFile(
  files: FileList
): Promise<MyDocument[]> {
  const formData = new FormData();
  for (let i = 0; i < files.length; ++i) {
    formData.append("files", files[i]);
  }
  const { data } = await contentsAxios.post<MyDocument[]>("/", formData, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  console.log("up");
  return data;
}
