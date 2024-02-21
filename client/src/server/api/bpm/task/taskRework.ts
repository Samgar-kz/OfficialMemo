import axios from "@/server/axios";
import type { TaskApproveModel } from "@/types/task/TaskApproveModel";

export default async function taskRework(model?: TaskApproveModel) {
  await axios.bpmApi.post("next/processes/tasks/rework", model);
}
