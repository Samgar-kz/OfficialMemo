import type { ConfidenceType } from "@/types/process/ConfidenceType";
import axios from "@/server/axios";

export default async function getConfidenceTypes() {
  const { data } = await axios.OfficialMemo.get<ConfidenceType[]>(
    "handbooks/confidenceTypes"
  );
  return data;
}
