import getTask from "@/server/api/bpm/task/getTask";
import type { Task } from "@/types/task/Task";
// import type { MaybeRef } from "@vueuse/shared";
import { ref, unref, watch, type Ref } from "vue";

export default function usePreviousTask(task: Ref<Task>) {
  const previousTask = ref<Task>(undefined);
  watch(task, async (newValue) => {
    console.log("Task updated:", newValue);
    let tempTask = unref(newValue);
    while (
      tempTask.previousTaskGuid &&
      tempTask.approvalDecision !== "rework" &&
      tempTask.approvalDecision !== "reject"
    ) {
      tempTask = await getTask(tempTask.previousTaskGuid);
    }
    if (
      tempTask &&
      (tempTask.approvalDecision === "rework" ||
        tempTask.approvalDecision === "reject")
    )
      previousTask.value = tempTask;
    else previousTask.value = undefined;
  });
  return { previousTask };
}
