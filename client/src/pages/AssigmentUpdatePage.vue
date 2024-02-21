<script setup lang="ts">
import TaskResultBlock from "@/components/task/TaskResultBlock.vue";
import WideLayout from "@/layouts/WideLayout.vue";
import getOfficialMemo from "@/server/api/process/getOfficialMemo";
import { useTitle } from "@vueuse/core";
import { onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";

import ProcessHistoryTree from "@/components/process-history/ProcessHistoryTree.vue";
import getTask from "@/server/api/bpm/task/getTask";
import getProcessInformation from "@/server/api/process/getProcessInformations";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";
import type { Task } from "@/types/task/Task";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import loadingOverlay from "@/plugins/vue-loading-overlay";

const title = useTitle("Исполнение резолюции", {
  titleTemplate: "%s | Документооборот",
});

const route = useRoute();
const requestGuid = route.params.requestGuid as string;
const taskGuid = route.params.taskGuid as string;

const message = ref<Model<any>>({} as Model<any>);
const process = ref<ProcessInfo>({} as ProcessInfo);
const task = ref<Task>({} as Task);

onMounted(async () => {
  const loader = loadingOverlay.show();
  await fetchData();
  loader.hide();
});

async function fetchData() {
  await Promise.all([
    getOfficialMemo(requestGuid).then((res) => (message.value = res)),
    getProcessInformation(requestGuid).then((res) => (process.value = res)),
    getTask(taskGuid).then((res) => (task.value = res)),
  ]);

  message.value.data.regNum &&
    useTitle(
      message.value.data?.regNum?.slice(0, -5) +
        " | " +
        message.value.data.recipients[0].name
    );
}

const selectedItem = ref<ProcessMessage>({} as ProcessMessage);

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
        <o-tab-item title="Документ" value="document">
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
              <!-- <receiving-results
                v-if="message.data?.receivingResults"
                :receivingResults="message.data?.receivingResults"
              /> -->
            </template>

            <document-viewer :document="selectedDocument" />

            <template v-slot:right-aside>
              <task-result-block
                @returnNewTask="task = $event"
                :task-guid="taskGuid"
                :title="title"
                :update-task="
                  ['toPerform', 'redirected'].includes(task?.taskStatus)
                "
              >
              </task-result-block>
              <actions-block
                v-if="
                  task?.taskType === 'task' &&
                  ['toPerform', 'redirected'].includes(task?.taskStatus)
                "
              >
                <div class="tw-flex tw-flex-row tw-flex-wrap tw-gap-2">
                  <ul class="hover-ul">
                    <li class="hover-li">
                      <task-create-dialog
                        :default-due-to-date="
                          task?.dueToDate ? new Date(task?.dueToDate) : null
                        "
                        v-if="
                          task?.parentGuid &&
                          ['toPerform', 'redirected'].includes(task?.taskStatus)
                        "
                        label="Создать резолюцию в дополнение к текущему"
                        :request-guid="requestGuid"
                        step-name="Исполнение резолюции"
                        :parent-guid="task?.parentGuid"
                        approval-required
                      />
                    </li>
                    <li class="hover-li">
                      <task-cancel-dialog
                        v-if="
                          task?.dueToDate &&
                          task?.taskType === 'task' &&
                          ['toPerform'].includes(task?.taskStatus)
                        "
                        :default-due-to-date="task?.dueToDate?.toString()"
                        :taskStatus="task?.taskStatus"
                        :taskGuid="taskGuid"
                        :label="'Отменить резолюцию'"
                        :title="'Отменить резолюцию?'"
                        decision="cancel"
                        decision-name="Отменить"
                        approveType="cancel"
                      />
                    </li>
                  </ul>
                </div>
              </actions-block>
            </template>
          </adaptive-three-column>
        </o-tab-item>
        <o-tab-item title="Ход исполнения" value="history">
          <process-history-tree
            class="tw-m-2"
            :process-guid="process.processGuid"
            v-model:selectedItem="selectedItem"
          />
        </o-tab-item>
        <o-tab-item title="Результаты исполнения" value="result">
          <receiving-result-tree
            class="tw-m-2"
            :message-guid="process.messageGuid"
            v-model:selectedItem="selectedItem"
          />
        </o-tab-item>
      </template>
    </o-tab>
  </wide-layout>
</template>
