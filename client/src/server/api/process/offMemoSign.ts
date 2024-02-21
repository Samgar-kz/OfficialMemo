import type { SignModel } from "@/types/process/SignModel";
import type { PerformModel } from "@/types/PerformModel";
import axios from "@/server/axios";

export default async function offMemoSign(model: SignModel) {
  try {
    await axios.OfficialMemo.post("OfficialMemo/sign", model);
  } catch (error) {
    throw error;
  }
}
export async function offMemoSignRework(model: PerformModel) {
  try {
    await axios.OfficialMemo.post("OfficialMemo/rework", {
      replyDto: model,
    });
  } catch (error) {
    throw error;
  }
}
