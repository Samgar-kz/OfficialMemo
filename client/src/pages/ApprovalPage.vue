<script setup lang="ts">
import ProcessHistoryTree from "@/components/process-history/ProcessHistoryTree.vue";
import ReplyDialog from "@/components/ReplyDialog.vue";
import TaskCreateDialog from "@/components/task/TaskCreateDialog.vue";
import WideLayout from "@/layouts/WideLayout.vue";
import getLastSubrequests from "@/server/api/bpm/task/getLastSubrequests";
import getTask from "@/server/api/bpm/task/getTask";
import taskCancel from "@/server/api/bpm/task/taskCancel";
import taskPerform from "@/server/api/bpm/task/taskPerform";
import officialMemoApprove from "@/server/api/process/approve";
import getOfficialMemo from "@/server/api/process/getOfficialMemo";
import getProcessInformation from "@/server/api/process/getProcessInformations";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";
import type { Task } from "@/types/task/Task";
import type { TaskApproveModel } from "@/types/task/TaskApproveModel";
import type { TaskPerformModel } from "@/types/task/TaskPerformModel";
import { useTitle } from "@vueuse/core";
import { onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import loadingOverlay from "@/plugins/vue-loading-overlay";

const route = useRoute();
const title = useTitle("Согласование служебной записки", {
  titleTemplate: "%s | Документооборот",
});

const requestGuid = route.params.requestGuid as string;
const taskGuid = route.params.taskGuid as string;

const task = ref<Task>(null);

const message = ref<Model<any>>({} as Model<any>);
const process = ref<ProcessInfo>({} as ProcessInfo);
const approvalSubrequests = ref<Task[]>([]);

onMounted(async () => await fetchData());

async function fetchData() {
  const loader = loadingOverlay.show();
  await Promise.all([
    getOfficialMemo(requestGuid).then((res) => (message.value = res)),
    getProcessInformation(requestGuid).then((res) => (process.value = res)),
    getTask(taskGuid).then((res) => (task.value = res)),
  ]);

  approvalSubrequests.value = await getLastSubrequests(
    process.value.processGuid
  );

  message.value.data.regNum &&
    useTitle(
      message.value.data?.regNum?.slice(0, -5) +
        " | " +
        message.value.data.recipients[0].name
    );
  loader.hide();
}

const selectedItem = ref<ProcessMessage>({} as ProcessMessage);

async function approve(model?: TaskPerformModel) {
  console.log(model);

  await officialMemoApprove({
    ...model,
    guid: message.value.data.messageGuid,
    employeeCode: task.value.responsible,
  });
  await taskPerform(model);
  await taskCancel({ taskGuid: model.guid } as TaskApproveModel);
}

async function rework(model?: TaskPerformModel) {
  await officialMemoApprove({
    ...model,
    guid: message.value.data.messageGuid,
    employeeCode: task.value.responsible,
  });
  await taskPerform(model);
}

const attachments = ref<Attachment[]>([]);
const selectedDocument = ref<Attachment>();
watch(message, (value) => {
  attachments.value = [
    { name: value.data?.regNum, url: value.documentUrl, kind: "pdf" },
  ];
  selectedDocument.value = attachments.value[0];

  if (!value.data?.attachments?.length) return;
  value.data?.attachments.forEach((file) => {
    attachments.value.push({ ...file, kind: getDocumentKind(file.name) });
  });
});
</script>

<template>
  <wide-layout>
    <o-tab class="top tw-h-full tw-bg-slate-100">
      <template v-slot:tabs>
        <o-tab-item title="Служебная записка" value="document">
          <adaptive-three-column>
            <template v-slot:left-aside>
              <!-- <official-memo-card :officialMemo="message" /> -->
              <documents-list-block
                :items="attachments"
                v-model:selectedDocument="selectedDocument"
              />
              <approval-results
                v-if="message.data?.approvalResults"
                :approvalResults="message.data?.approvalResults"
              />
            </template>

            <document-viewer :document="selectedDocument" />

            <template v-slot:right-aside>
              <o-actions-menu>
                <reply-dialog
                  :guid="taskGuid"
                  label="Согласовать"
                  title="Согласовать сообщение?"
                  decision="approve"
                  decision-name="Согласовать"
                  performType="task"
                  :send-function="approve"
                  :width="500"
                >
                  <template v-slot:default="{ click, label }">
                    <o-menu-item :label="label" @click="click">
                      <template v-slot:icon
                        ><i-mdi-file-check-outline class="tw-text-primary"
                      /></template>
                    </o-menu-item>
                  </template>
                </reply-dialog>
                <task-create-dialog
                  label="Создать резолюцию"
                  v-if="task?.dueToDate"
                  :default-due-to-date="
                    task?.dueToDate ? new Date(task?.dueToDate) : null
                  "
                  :request-guid="requestGuid"
                  step-name="Исполнение резолюции"
                  :parent-guid="taskGuid"
                  approval-required
                >
                  <template v-slot:default="{ click, label }">
                    <o-menu-item :label="label" @click="click">
                      <template v-slot:icon
                        ><i-mdi-clipboard-plus-outline class="tw-text-primary"
                      /></template>
                    </o-menu-item>
                  </template>
                </task-create-dialog>
                <reply-dialog
                  :guid="taskGuid"
                  label="На доработку"
                  title="Отправить на доработку?"
                  decision="rework"
                  decision-name="Отклонить"
                  performType="task"
                  :send-function="rework"
                  :width="500"
                >
                  <template v-slot:default="{ click, label }">
                    <o-menu-item :label="label" @click="click">
                      <template v-slot:icon
                        ><i-mdi-clipboard-arrow-left-outline
                          class="tw-text-primary"
                      /></template>
                    </o-menu-item>
                  </template>
                </reply-dialog>
              </o-actions-menu>
            </template>
          </adaptive-three-column>
        </o-tab-item>
        <o-tab-item title="Ход исполнения" value="history">
          <div class="tw-m-2 tw-grid tw-grid-cols-[0px_1fr] tw-gap-2">
            <process-history-tree
              class="tw-col-start-2"
              :process-guid="process.processGuid"
              v-model:selectedItem="selectedItem"
            />
          </div>
        </o-tab-item>
      </template>
    </o-tab>
  </wide-layout>
</template>
