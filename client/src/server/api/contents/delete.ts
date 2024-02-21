import contentsAxios from "./contentsAxios";

export default async function deleteFile(fileUrl: string) {
  const { data } = await contentsAxios.delete(fileUrl);
  return data;
}
