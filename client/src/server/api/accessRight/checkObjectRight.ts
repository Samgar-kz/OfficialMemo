import accessRightAxios from "./accessRightAxios";

export default async function checkObjectRight(typeId: string, objectId: string, rightId: string): Promise<boolean> {
  const { data } = await accessRightAxios.get<boolean>("/checkObjectRight", {
    params: { typeId, objectId, rightId },
  });
  return data;
}
