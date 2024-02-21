import type { Task } from "@/types/task/Task";
import axios from "@/server/axios";

export default async function getTasks(requestGuid: string) {
  const { data } = await axios.bpmApi.get<Array<Task>>(
    "next/processes/tasks/byRequest",
    { params: { requestGuid } }
  );
  return data;
}
