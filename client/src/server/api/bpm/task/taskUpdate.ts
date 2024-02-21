import type { TaskUpdateModel } from "@/types/task/TaskUpdateModel";
import axios from "@/server/axios";

export default async function taskUpdate(model?: TaskUpdateModel) {
  await axios.bpmApi.post("next/processes/tasks/update", model);
}
