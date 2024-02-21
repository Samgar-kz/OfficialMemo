import type { PerformModel } from "@/types/PerformModel";
import axios from "@/server/axios";
import type { SignModel } from "@/types/process/SignModel";

export async function reviewRework(model: PerformModel) {
  await axios.OfficialMemo.post("OfficialMemo/rework", { replyDto: model });
}

export async function reviewPerform(model: PerformModel) {
  await axios.OfficialMemo.post("OfficialMemo/close", { replyDto: model });
}

export async function registerSign(message: SignModel) {
  await axios.OfficialMemo.post("OfficialMemo/registerSign", message);
}
export async function getSupervisorSign(guid: string) {
  const { data } = await axios.OfficialMemo.get<SignModel>(
    "OfficialMemo/signature?requestGuid=" + guid
  );
  return data;
}
