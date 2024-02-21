import type { PerformModel } from "@/types/PerformModel";
import axios from "@/server/axios";

export default async function officialMemoApprove(model: PerformModel) {
  await axios.OfficialMemo.post("OfficialMemo/approve", model);
}
export async function officialMemoReceive(model: PerformModel) {
  await axios.OfficialMemo.post("OfficialMemo/receive", model);
}
