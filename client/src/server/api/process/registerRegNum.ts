import axios from "@/server/axios";

export default async function registerRegNum(requestGuid: string) {
  console.log(requestGuid);
  const { data } = await axios.OfficialMemo.post<{
    regNum: string;
    registerDate: Date;
  }>("OfficialMemo/registerRegNum", requestGuid, {
    headers: { "Content-Type": "application/json" },
  });
  return data;
}
