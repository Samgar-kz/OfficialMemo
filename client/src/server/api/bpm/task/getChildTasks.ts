import type { Task } from "@/types/task/Task";
import axios from "@/server/axios";

export default async function getChildTasks(parentGuid: string) {
  const { data } = await axios.bpmApi.get<Task[]>(
    "next/processes/tasks/child-tasks",
    { params: { parentGuid } }
  );
  return data;
}
