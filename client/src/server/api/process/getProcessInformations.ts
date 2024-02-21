import axios from "@/server/axios";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";

export default async function getProcessInformation(requestGuid: string) {
  const { data } = await axios.OfficialMemo.get<ProcessInfo>("processes", {
    params: { requestGuid },
  });
  return data;
}

export async function getProcessInfoByProcessGuid(processGuid: string) {
  const { data } = await axios.OfficialMemo.get<ProcessInfo>(
    "processes/getProcessInfoByProcessGuid",
    {
      params: { processGuid },
    }
  );
  return data;
}
export async function getProcessStatus(processGuid: string) {
  const { data } = await axios.OfficialMemo.get<string>(
    "processes/getProcessStatus",
    {
      params: { processGuid },
    }
  );
  return data;
}
