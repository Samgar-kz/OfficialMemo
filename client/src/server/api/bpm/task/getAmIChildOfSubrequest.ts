import axios from "@/server/axios";

export default async function getAmIChildOfSubrequest(taskGuid: string) {
  const { data } = await axios.bpmApi.get<boolean>(
    "next/processes/tasks/amIChildOfSubrequest",
    { params: { taskGuid } }
  );
  return data;
}
