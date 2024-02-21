import type { Reply } from "@/types/Reply";
import axios from "@/server/axios";

export default async function getLastReply(
  requestGuid: string
): Promise<Reply> {
  const { data } = await axios.OfficialMemo.get<Reply>(
    "OfficialMemo/lastReply",
    {
      params: { requestGuid },
    }
  );
  return data;
}
