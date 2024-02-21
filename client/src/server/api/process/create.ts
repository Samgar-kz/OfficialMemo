import type { Model } from "@/types/process/Model";
import axios from "@/server/axios";

export default async function create(message: Model<any>) {
  const approverCodes: string[] = message.data.approvers
    ?.map((v) => v.login ?? "")
    ?.filter((v) => v);
  const recipientCodes: string[] = message.data.recipients
    ?.map((v) => v.login ?? "")
    ?.filter((v) => v);
  const dto = {
    data: {
      ...message.data,
      approverCodes,
      recipientCodes,
      signerCode: message.data.signer.login ?? "",
      templateName: undefined,
    },
    htmlDocument: message.htmlDocument,
  };
  try {
    const { data } = await axios.OfficialMemo.post("OfficialMemo", dto);
    return data;
  } catch (error) {
    console.error(error);
    throw error;
  }
}
