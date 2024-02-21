import axios from "@/server/axios";
import type { TaskApproveModel } from "@/types/task/TaskApproveModel";

export default async function taskCancel(model?: TaskApproveModel) {
  await axios.bpmApi.post("next/processes/tasks/cancel", model);
}
