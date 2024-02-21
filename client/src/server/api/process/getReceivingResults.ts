import axios from "@/server/axios";
import type { ReceivingResult } from "@/types/process/ReceivingResult";

export default async function getReceivingResults(messageGuid: string) {
  const { data } = await axios.OfficialMemo.get<ReceivingResult[]>(
    "officialMemo/receivingResults",
    {
      params: { messageGuid },
    }
  );
  return data;
}
