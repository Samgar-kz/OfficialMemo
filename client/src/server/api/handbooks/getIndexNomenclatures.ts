import type { IndexNomenclature } from "@/types/process/IndexNomenclature";
import axios from "@/server/axios";

export default async function getIndexNomenclatures() {
  const { data } = await axios.OfficialMemo.get<IndexNomenclature[]>(
    "handbooks/indexNomenclatures"
  );
  return data;
}
