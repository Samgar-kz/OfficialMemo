import axios from "@/server/axios";

export default async function deleteOfficialMemo(processGuid: string) {
  await axios.OfficialMemo.post(
    "OfficialMemo/delete?processGuid=" + processGuid
  );
}
