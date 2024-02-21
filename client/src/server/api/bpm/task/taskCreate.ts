import axios from "@/server/axios";
import type { TaskCreateModel } from "@/types/task/TaskCreateModel";

export default async function taskCreate(model?: TaskCreateModel) {
  const awaits = model?.executors.map((executor) =>
    axios.bpmApi.post("next/processes/tasks", {
      ...model,
      executorCode: executor?.login,
      executor: undefined,
    })
  );
  if (awaits) await Promise.all(awaits);
}
