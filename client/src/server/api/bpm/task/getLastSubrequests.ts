import type { Task } from "@/types/task/Task";
import axios from "@/server/axios";

export default async function getLastSubrequests(processGuid: string) {
  const { data } = await axios.bpmApi.get<Task[]>(
    "next/processes/subrequests/last",
    { params: { processGuid } }
  );
  return data;
}
