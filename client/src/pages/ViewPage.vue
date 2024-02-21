<script setup lang="ts">
import WideLayout from "@/layouts/WideLayout.vue";
import {
  getOfficialMemoByProcessGuid,
  getOfficialMemoByRegNum,
} from "@/server/api/process/getOfficialMemo";
// import revokeOutgoingMessage from "@/server/api/outgoing/revokeOutMessage";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import { useTitle } from "@vueuse/core";
import { ref, onMounted, watch, onBeforeMount } from "vue";
import { useRoute } from "vue-router";
import useConfirmDialog from "@/features/confirmationService/useConfirmDialog";
import deleteOutgoingMessage from "@/server/api/process/deleteOfficialMemo";
import checkObjectRight from "@/server/api/accessRight/checkObjectRight";
import whoAmI from "@/server/api/employees/whoAmI";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import { getProcessStatus } from "@/server/api/process/getProcessInformations";
import type { Employee } from "@/types/employees/Employee";
import loadingOverlay from "@/plugins/vue-loading-overlay";

const route = useRoute();
const processGuid = ref<string>(route.params.processGuid as string);
const regNum = route.params.regNum as string;
// const status = route.params.status as string;

const showDeleteLink = ref<boolean>();
const showEditLink = ref<boolean>();
const processStatus = ref<string>();
const currentUser = ref<Employee>();
const message = ref<Model<any>>({} as Model<any>);
useTitle("Просмотр исходящего письма", {
  titleTemplate: "%s | Документооборот",
});
onMounted(async () => {
  const loader = loadingOverlay.show();
  await fetchData();
  loader.hide();
});
async function fetchData() {
  await Promise.all([
    processGuid.value
      ? getOfficialMemoByProcessGuid(processGuid.value).then(
          (res) => (message.value = res)
        )
      : getOfficialMemoByRegNum(regNum).then((res) => (message.value = res)),
    getProcessStatus(processGuid.value).then(
      (res) => (processStatus.value = res)
    ),
    whoAmI().then((res) => (currentUser.value = res)),
  ]);

  message.value?.data.regNum &&
    useTitle(
      `${message.value?.data?.regNum.slice(0, -5)} | ${
        message.value?.data?.recipients[0].name
      }`
    );

  showEditLink.value =
    message.value?.data?.executor?.login === currentUser.value?.login &&
    processStatus.value === "Started";

  showDeleteLink.value =
    message.value?.data?.executor?.login === currentUser.value?.login;
}

const selectedItem = ref<ProcessMessage>({} as ProcessMessage);

const confirmDialog = useConfirmDialog();
// async function confirmRevoke() {
//   confirmDialog.require({
//     title: "Отозвать исходящий документ?",
//     onConfirm: async () => {
//       await revokeOutgoingMessage(processGuid.value);
//     },
//     confirmLabel: "Отозвать",
//     rejectLabel: "Отмена",
//   });
// }

async function confirmDelete() {
  confirmDialog.require({
    title: "Отправить в архив",
    message: `Вы точно хотите отправить в архив служебной запиской ${message.value?.data?.regNum}?`,
    onConfirm: async () => {
      await deleteOutgoingMessage(processGuid.value);
    },
    confirmLabel: "Отправить",
    rejectLabel: "Отмена",
  });
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
              <o-actions-menu>
                <!-- <o-menu-item label="Отозвать документ" @click="confirmRevoke">
                  <template v-slot:icon><i-mdi-undo class="tw-text-primary" /></template>
                </o-menu-item> -->
                <router-link
                  v-if="showEditLink"
                  :to="{
                    name: 'EditPage',
                    params: { processGuid: processGuid },
                  }"
                  custom
                  v-slot="{ navigate }"
                >
                  <o-menu-item label="Редактировать" @click="navigate">
                    <template v-slot:icon
                      ><i-mdi-clipboard-edit-outline
                        class="tw-text-primary" /></template
                  ></o-menu-item>
                </router-link>
                <o-menu-item
                  label="Отправить в архив"
                  @click="confirmDelete"
                  v-if="showDeleteLink"
                >
                  <template v-slot:icon
                    ><i-mdi-trash-can-outline class="tw-text-primary"
                  /></template>
                </o-menu-item>
              </o-actions-menu>
            </template>
          </adaptive-three-column>
        </o-tab-item>
        <o-tab-item title="Ход исполнения" value="history">
          <process-history-tree
            class="tw-m-2"
            v-show="processGuid"
            :process-guid="processGuid"
            v-model:selectedItem="selectedItem"
          />
        </o-tab-item>
        <o-tab-item title="Результаты исполнения" value="result">
          <receiving-result-tree
            class="tw-m-2"
            :message-guid="message.data?.messageGuid"
            v-model:selectedItem="selectedItem"
          />
        </o-tab-item>
      </template>
    </o-tab>
  </wide-layout>
</template>
