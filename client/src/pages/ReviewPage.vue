<script setup lang="ts">
import ProcessHistoryTree from "@/components/process-history/ProcessHistoryTree.vue";
import ReplyDialog from "@/components/ReplyDialog.vue";
import SuccessModal from "@/components/SuccessModal.vue";
import useSigner from "@/composables/useSigner";
import WideLayout from "@/layouts/WideLayout.vue";
import getLastSubrequests from "@/server/api/bpm/task/getLastSubrequests";
import getOfficialMemo from "@/server/api/process/getOfficialMemo";
import {
  getSupervisorSign,
  registerSign,
  reviewPerform,
  reviewRework,
} from "@/server/api/process/review";
import registerRegNum from "@/server/api/process/registerRegNum";
import getProcessInformation from "@/server/api/process/getProcessInformations";
import type { SignModel } from "@/types/process/SignModel";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";
import type { Task } from "@/types/task/Task";
import { useTitle } from "@vueuse/core";
import Button from "primevue/button";
import { useDialog } from "primevue/usedialog";
import { h, onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import loadingOverlay from "@/plugins/vue-loading-overlay";

useTitle("Регистратор. Проверка служебной записки", {
  titleTemplate: "%s | Документооборот",
});

const route = useRoute();
const requestGuid = route.params.requestGuid as string;

const message = ref<Model<any>>({} as Model<any>);
const mustSign = ref<Boolean>(false);
type Status = "success" | "loading" | "error" | "hide";
const signStatus = ref<Status>("hide");
const process = ref<ProcessInfo>({} as ProcessInfo);
const signer = useSigner();
const approvalSubrequests = ref<Task[]>([]);
const errorMessage = ref<string>();
onMounted(async () => {
  const loader = loadingOverlay.show();
  await fetchData();
  loader.hide();
});
const successRegisterSignDialog = useDialog();

async function sign() {
  signStatus.value = "loading";
  const signature = ref<SignModel>();
  signature.value = await getSupervisorSign(requestGuid);
  console.log("signature.value.signDocument: ", signature.value.signDocument);
  console.log("message.value: ", message.value);
  signature.value.registerSignature = await signer
    .officeSignWithBPMAgent(signature.value.signDocument, message.value)
    .catch(async (ex) => {
      signStatus.value = "error";
      errorMessage.value = await signer.CheckRun_BPMAgent(ex.message);
      if (!errorMessage.value) {
        errorMessage.value =
          "В вашем устройстве отсутствует необходимое для подписания программное обеспечение TumarCSP";
      }
      return null;
    });
  signature.value.requestGuid = requestGuid;
  if (
    signature.value.registerSignature === "" ||
    signature.value.registerSignature === null
  ) {
    window.alert(errorMessage.value);
    errorMessage.value = "Произошла ошибка при подписании";
    throw "Unknown error";
  }
  await registerSign(signature.value)
    .catch(() => (signStatus.value = "error"))
    .then(() => (signStatus.value = "success"))
    .then(() => (signStatus.value = "success"));

  const dialogRef = successRegisterSignDialog.open(SuccessModal, {
    props: {
      header: "Уведомление о подписи канцелярии",
      modal: false,
    },
    templates: {
      footer: () => {
        return [
          h(Button, {
            label: "Закрыть",
            onClick: () => {
              dialogRef.close();
            },
          }),
        ];
      },
    },
    data: {
      message: "Успешно подписано",
    },
  });
}
async function fetchData() {
  await Promise.all([
    getOfficialMemo(requestGuid).then((res) => (message.value = res)),
    getProcessInformation(requestGuid).then((res) => (process.value = res)),
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
  message.value.data.recipients.forEach(() => {
    if (message.value.data?.signData?.signature) {
      mustSign.value = true;
      return;
    }
  });
}

const selectedItem = ref<ProcessMessage>({} as ProcessMessage);
const successRegisterDialog = useDialog();

async function register() {
  const { regNum, registerDate } = await registerRegNum(requestGuid);
  await fetchData();
  const dialogRef = successRegisterDialog.open(SuccessModal, {
    props: {
      header: "Успешно зарегистрировано",
      modal: false,
    },
    templates: {
      footer: () => {
        return [
          h(Button, { label: "Закрыть", onClick: () => dialogRef.close() }),
        ];
      },
    },
    data: {
      message: `${regNum} от ${new Date(registerDate).toLocaleDateString(
        "kk-KZ"
      )}`,
    },
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
              <!-- <receiving-results  v-if="message.data?.receivingResults" 
              :receivingResults="message.data?.receivingResults" /> -->
            </template>

            <document-viewer :document="selectedDocument" />

            <template v-slot:right-aside>
              <actions-block class="tw-mt-[0px]">
                <div class="tw-flex tw-flex-row tw-flex-wrap tw-gap-2">
                  <ul class="hover-ul">
                    <li class="hover-li">
                      <icon-action
                        class="hover-list"
                        label="Подписать"
                        v-if="mustSign"
                        @click="sign"
                      >
                        <template #secondIcon v-if="signStatus !== 'hide'">
                          <i
                            :class="{
                              'pi pi-spin pi-spinner': signStatus === 'loading',
                              'pi pi-check': signStatus === 'success',
                              'pi pi-times': signStatus === 'error',
                            }"
                            style="font-size: 2rem"
                          ></i>
                        </template>
                      </icon-action>
                    </li>
                    <!-- <li class="hover-li">
                      <icon-action
                        label="Зарегистрировать"
                        class="hover-list"
                        @click="register()"
                      />
                    </li> -->
                    <li class="hover-li">
                      <reply-dialog
                        :guid="requestGuid"
                        label="Отправить"
                        title="Отправить служебную записку?"
                        decision="accept"
                        decision-name="Отправить служебную записку"
                        performType="request"
                        :send-function="reviewPerform"
                        :width="500"
                      />
                    </li>
                    <li class="hover-li">
                      <reply-dialog
                        :guid="requestGuid"
                        label="На доработку"
                        title="Отправить на доработку?"
                        decision="rework"
                        decision-name="На доработку"
                        performType="request"
                        :sendFunction="reviewRework"
                        :width="500"
                      />
                    </li>
                  </ul>

                  <div :class="{ 'tw-pb-1': errorMessage }">
                    <span class="error-text-red">{{ errorMessage }}</span>
                  </div>
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
        <!-- <o-tab-item title="Результаты исполнения" value="result">
          <receiving-result-tree
            class="tw-m-2"
            :message-guid="process.messageGuid"
            v-model:selectedItem="selectedItem"
          />
        </o-tab-item> -->
      </template>
    </o-tab>
  </wide-layout>
</template>
<style>
.error-text-red {
  position: absolute;
  font-size: 12px;
  color: red;
}
</style>
{ guid: requestGuid }
