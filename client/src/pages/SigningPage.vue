<script setup lang="ts">
import ProcessHistoryTree from "@/components/process-history/ProcessHistoryTree.vue";
import ReplyDialog from "@/components/ReplyDialog.vue";
import SuccessModal from "@/components/SuccessModal.vue";
import useSigner from "@/composables/useSigner";
import WideLayout from "@/layouts/WideLayout.vue";
import getOfficialMemo from "@/server/api/process/getOfficialMemo";
import offMemoSign, {
  offMemoSignRework,
} from "@/server/api/process/offMemoSign";
import getProcessInformation from "@/server/api/process/getProcessInformations";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";
import { useTitle } from "@vueuse/core";
import Button from "primevue/button";
import Checkbox from "primevue/checkbox";
import { useDialog } from "primevue/usedialog";
import { h, inject, onMounted, ref, watch } from "vue";
import { useRoute } from "vue-router";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import closeWindow from "@/server/closeWindow";
import loadingOverlay from "@/plugins/vue-loading-overlay";

useTitle("Подписание служебной записки", {
  titleTemplate: "%s | Документооборот",
});

const route = useRoute();
const requestGuid = route.params.requestGuid as string;

const process = ref<ProcessInfo>({} as ProcessInfo);

const message = ref<Model<any>>({} as Model<any>);
const signer = useSigner();
const withECP = ref<boolean>(true);
const errorMessage = ref<string>();
type Status = "success" | "loading" | "error" | "hide";
const signStatus = ref<Status>("hide");

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
const successSignDialog = useDialog();
// const dialogRef = inject('dialogRef');

// const closeDialog = () => {
//     dialogRef.value.close();
// }

async function sign() {
  signStatus.value = "loading";
  errorMessage.value = null;
  const data = JSON.stringify(message.value);
  let signature = null;
  if (withECP.value)
    signature = await signer
      .supervisorSignWithBPMAgent(message.value)
      .catch(async (ex) => {
        errorMessage.value = await signer.CheckRun_BPMAgent(ex.message);
        signStatus.value = "error";
        if (!errorMessage.value) {
          errorMessage.value =
            "В вашем устройстве отсутствует необходимое для подписания программное обеспечение TumarCSP";
        }
        return null;
      });
  if (signature === null && withECP.value) {
    // const SignWithoutEcpDialog = successSignDialog.open(SuccessModal, {
    //   props: {
    //     header:
    //       errorMessage.value +
    //       ".\nВы в любом случае хотите подписать документ?",
    //     modal: false,
    //   },
    //   templates: {
    //     footer: () => {
    //       return [
    //         h(Button, {
    //           label: "Закрыть",
    //           onClick: () => SignWithoutEcpDialog.close(),
    //         }),
    //         h(Button, {
    //           label: "Все равно подписать",
    //           onClick: async () => {
    //             await offMemoSign({
    //               data,
    //               requestGuid,
    //               signature: signature?.data ?? "",
    //               registerSignature: null,
    //               signDocument: { name: signature?.name ?? "", url: "" },
    //               signType: "HandWritten",
    //             });
    //           },
    //         }),
    //       ];
    //     },
    //   },
    //   data: {
    //     message: "",
    //   },
    // });
    window.alert(errorMessage.value);
    errorMessage.value = "Произошла ошибка при подписании";
    throw "Unknown error";
  }

  try {
    await offMemoSign({
      data,
      requestGuid,
      signature: signature?.data ?? "",
      registerSignature: null,
      signDocument: { name: signature?.name ?? "", url: "", size: 0 },
      signType: withECP.value ? "Digital" : "HandWritten",
    });
    signStatus.value = "success";
  } catch (error) {
    signStatus.value = "error";

    successSignDialog.open(SuccessModal, {
      props: {
        header: "Уведомление о подписи руководителя",
        modal: false,
      },
      data: {
        message: `Произошло ошибка: ${error}`,
      },
    });
  }

  if (signStatus.value === "success")
    successSignDialog.open(SuccessModal, {
      props: {
        header: "Уведомление о подписи руководителя",
        modal: false,
      },
      templates: {
        footer: () => {
          return [
            h(Button, { label: "Закрыть", onClick: () => closeWindow() }),
          ];
        },
      },
      data: {
        message: "Успешно подписано",
      },
    });
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
            </template>

            <document-viewer :document="selectedDocument" />

            <template v-slot:right-aside>
              <actions-block>
                <div class="tw-flex tw-flex-row tw-flex-wrap tw-gap-2">
                  <div
                    class="tw-text-center tw-w-full tw-mt-3 sign-checkbox-label"
                  >
                    <Checkbox
                      v-model="withECP"
                      inputId="withECP"
                      :binary="true"
                    />
                    <label for="withECP"
                      >&nbsp; Использовать ЭЦП при подписании</label
                    >
                  </div>
                  <ul class="hover-ul">
                    <li class="hover-li">
                      <icon-action
                        class="hover-list"
                        label="Подписать"
                        @click="sign"
                      >
                        <template #secondIcon v-if="signStatus !== 'hide'">
                          <i
                            class="tw-ml-2 tw-align-top tw-text-base"
                            :class="{
                              'pi pi-spin pi-spinner': signStatus === 'loading',
                              'pi pi-check': signStatus === 'success',
                              'pi pi-times': signStatus === 'error',
                            }"
                          ></i>
                        </template>
                      </icon-action>
                      <div
                        class="tw-flex tw-flex-row tw-justify-between tw-gap-4"
                        :class="{ 'tw-pb-1': errorMessage }"
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
                        :sendFunction="offMemoSignRework"
                        :width="500"
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
      </template>
    </o-tab>
  </wide-layout>
</template>
<style>
.sign-checkbox-input {
  width: 14px;
  height: 14px;
  margin-top: 8px;
  margin-left: 8px;
  vertical-align: revert;
}

.sign-checkbox-label {
  vertical-align: -webkit-baseline-middle;
}
.error-text-red {
  position: absolute;
  font-size: 12px;
  color: red;
}
</style>
