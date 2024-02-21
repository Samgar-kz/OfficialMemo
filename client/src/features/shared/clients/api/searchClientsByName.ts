import axios from "@/server/axios";
import type { ClientType } from "@/features/inquiries/types";
import type { Client, EntityClient } from "../types/Client";

export async function searchClientsByName(name: string, clientType: ClientType | "both" | undefined = "both") {
  const { data, status } = await axios.get<(Client | EntityClient)[]>("clients/search", { params: { name, clientType } });
  if (status === 200) return data;
  return [];
}
