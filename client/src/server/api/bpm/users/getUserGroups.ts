import axios from "@/server/axios";
export default async function getUserGroups() {
  const { data } = await axios.bpmApi.get("User/getUserGroups");
  return data;
}
