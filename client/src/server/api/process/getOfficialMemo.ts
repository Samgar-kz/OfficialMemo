import type { Model } from "@/types/process/Model";
import axios from "@/server/axios";

export default async function getOfficialMemo(
  requestGuid: string
): Promise<Model<any>> {
  try {
    const { data } = await axios.OfficialMemo.get<Model<any>>(
      "OfficialMemo/byRequestGuid",
      {
        params: { requestGuid },
      }
    );
    return data;
  } catch (error) {
    console.error("Error in getOfficialMemo:", error);
    return Promise.resolve({} as Model<any>);
  }
}

export async function getOfficialMemoByProcessGuid(
  processGuid: string
): Promise<Model<any>> {
  try {
    const { data } = await axios.OfficialMemo.get<Model<any>>(
      "OfficialMemo/byProcessGuid",
      {
        params: { processGuid },
      }
    );
    return data;
  } catch (error) {
    console.error("Error in getOfficialMemoByProcessGuid:", error);
    return Promise.resolve({} as Model<any>);
  }
}

export async function getOfficialMemoByRegNum(
  regNum: string
): Promise<Model<any>> {
  try {
    const { data } = await axios.OfficialMemo.get<Model<any>>(
      "OfficialMemo/byRegNum",
      {
        params: { regNum },
      }
    );
    return data;
  } catch (error) {
    console.error("Error in getOfficialMemoByRegNum:", error);
    return Promise.resolve({} as Model<any>);
  }
}
