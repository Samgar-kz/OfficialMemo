import axios from "@/server/axios";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";

export default async function getProcessHistory(
  processGuid: string
): Promise<ProcessMessage[]> {
  const { data } = await axios.OfficialMemo.get<ProcessMessage[]>(
    "processes/messageHistory",
    {
      params: { processGuid },
    }
  );
  return data;
}
