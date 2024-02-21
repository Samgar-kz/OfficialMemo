import type { TaskPerformModel } from "@/types/task/TaskPerformModel";
import axios from "@/server/axios";

export default async function taskPerform(model?: TaskPerformModel) {
  await axios.bpmApi.post("next/processes/tasks/perform", model);
}
