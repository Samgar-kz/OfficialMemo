<script setup lang="ts">
import ActionsBlock from "@/components/ActionsBlock.vue";
import TaskApproveDialog from "@/components/task/TaskApproveDialog.vue";
import TaskResultBlock from "@/components/task/TaskResultBlock.vue";
import WideLayout from "@/layouts/WideLayout.vue";
import getOfficialMemo from "@/server/api/process/getOfficialMemo";
import getProcessInformation from "@/server/api/process/getProcessInformations";
import closeWindow from "@/server/closeWindow";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";
import { useTitle } from "@vueuse/core";
import { onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import loadingOverlay from "@/plugins/vue-loading-overlay";

useTitle("Принятие ответа по резолюции", {
  titleTemplate: "%s | Документооборот",
});

const route = useRoute();
const requestGuid = route.params.requestGuid as string;
const taskGuid = route.params.taskGuid as string;

const message = ref<Model<any>>({} as Model<any>);

onMounted(async () => {
  const loader = loadingOverlay.show();
  await fetchData();
  loader.hide();
});

async function fetchData() {
  await Promise.all([
    getOfficialMemo(requestGuid).then((res) => (message.value = res)),
    getProcessInformation(requestGuid).then((res) => (process.value = res)),
  ]);

  message.value.data.regNum &&
    useTitle(
      message.value.data?.regNum?.slice(0, -5) +
        " | " +
        message.value.data.recipients[0].name
    );
}

const process = ref<ProcessInfo>({} as ProcessInfo);
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
              <task-result-block :task-guid="taskGuid" title="Резолюция" />
              <actions-block>
                <div class="tw-flex tw-flex-row tw-flex-wrap tw-gap-2">
                  <ul class="hover-ul">
                    <li class="hover-li">
                      <task-approve-dialog
                        :taskGuid="taskGuid"
                        :message-guid="message?.data?.messageGuid"
                        label="Принять"
                        title="Принять ответ"
                        decision="accept"
                        decision-name="Принять ответ"
                        defaultPerformAfterApprove
                        :isPerform="process.schemeId === 'OfficialMemoPerform'"
                        approveType="approve"
                        @success="closeWindow"
                      />
                    </li>
                    <li class="hover-li">
                      <task-approve-dialog
                        :taskGuid="taskGuid"
                        label="На доработку"
                        title="Отправить на доработку?"
                        decision="rework"
                        decision-name="На доработку"
                        approveType="rework"
                        @success="closeWindow"
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
