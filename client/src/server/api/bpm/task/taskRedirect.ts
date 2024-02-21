import axios from "@/server/axios";
import type { TaskRedirectModel } from "@/types/task/TaskRedirectModel";

export default async function taskRedirect(model?: TaskRedirectModel) {
  await axios.bpmApi.post("next/processes/tasks/redirect", {
    taskGuid: model?.taskGuid,
    redirectTo: model?.redirectTo.login,
    replyComment: model?.replyComment,
    replyDocuments: model?.replyDocuments,
  });
}
