import type { Task } from "@/types/task/Task";
import axios from "@/server/axios";

export default async function getTask(taskGuid: string) {
  const { data } = await axios.bpmApi.get<Task>("next/processes/tasks", {
    params: { taskGuid },
  });
  return data;
}
