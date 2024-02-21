import axios from "@/server/axios";
import type { Executor } from "@/types/Executor";

export default async function getExecutorsList(
  executorCode: string
): Promise<Executor[]> {
  const { data } = await axios.bpmApi.get<Executor[]>("User/GetExecutorList", {
    params: { executor: executorCode },
  });
  return data;
}
