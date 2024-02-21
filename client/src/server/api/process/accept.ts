import type { PerformModel } from "@/types/PerformModel";
import axios from "@/server/axios";

export async function accept(model?: PerformModel) {
  await axios.OfficialMemo.post("OfficialMemo/accept", {
    replyDto: {
      ...model,
      requestGuid: model.guid,
    },
  });
}

export async function acceptRework(model: PerformModel) {
  await axios.OfficialMemo.post("OfficialMemo/rework", {
    replyDto: model,
  });
}
