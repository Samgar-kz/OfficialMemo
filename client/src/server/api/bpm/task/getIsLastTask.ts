import axios from "@/server/axios";

export default async function getIsLastTask(taskGuid: string) {
  const { data } = await axios.bpmApi.get<boolean>(
    "next/processes/tasks/isLastTask",
    { params: { taskGuid } }
  );
  return data;
}
