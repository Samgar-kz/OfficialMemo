<script setup lang="ts">
import ProcessHistoryTree from "@/components/process-history/ProcessHistoryTree.vue";
import YesNoDialog from "@/components/YesNoDialog.vue";
import useYesNoDialog from "@/composables/useYesNoDialog";
import WideLayout from "@/layouts/WideLayout.vue";
import whoAmI from "@/server/api/employees/whoAmI";
import getApprovalTypes from "@/server/api/handbooks/getApprovalTypes";
import getConfidenceTypes from "@/server/api/handbooks/getConfidenceTypes";
import getLanguages from "@/server/api/handbooks/getLanguages";
import getIndexNomenclatures from "@/server/api/handbooks/getIndexNomenclatures";
import { getOfficialMemoByProcessGuid } from "@/server/api/process/getOfficialMemo";
import { updateByProcessGuid } from "@/server/api/process/update";
import { getProcessInfoByProcessGuid } from "@/server/api/process/getProcessInformations";
import closeWindow from "@/server/closeWindow";
import type { Employee } from "@/types/employees/Employee";
import type { MyDocument } from "@/types/contents/MyDocument";
import type { ConfidenceType } from "@/types/process/ConfidenceType";
import type { LabelValue } from "@/types/process/LabelValue";
import type { CoreData } from "@/types/process/CoreData";
import type { Model } from "@/types/process/Model";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import type { ProcessInfo } from "@/types/shared/ProcessInfo";
import type ValidationFieldsOf from "@/types/utility/ValidationFieldsOf";
import { useStepper, useTitle } from "@vueuse/core";
import Card from "primevue/card";
import { useForm } from "vee-validate";
import { computed, onMounted, reactive, ref, watch } from "vue";
import { useRoute, useRouter } from "vue-router";
import renamePosition from "@/server/api/employees/renamePosition";
import type { IndexNomenclature } from "@/types/process/IndexNomenclature";
import type { Attachment } from "@/features/documents/types";
import getDocumentKind from "@/features/documents/getDocumentKind";
import type { ReceivingResult } from "@/types/process/ReceivingResult";
import getReceivingResults from "@/server/api/process/getReceivingResults";
import loadingOverlay from "@/plugins/vue-loading-overlay";

const title = useTitle("Доработка служебной записки", {
  titleTemplate: "%s | Документооборот",
});

const route = useRoute();
const router = useRouter();

const processGuid = route.params.processGuid as string;

const model = ref<Model<any>>({
  data: {
    attachments: [] as MyDocument[],
    approvalMode: "parallel",
    executor: {} as Employee,
  } as CoreData,
} as Model<any>);

const languages = ref<string[]>();
const confidenceTypes = ref<ConfidenceType[]>();
const approvalTypes = ref<LabelValue[]>();
const indexNomenclatures = ref<IndexNomenclature[]>();
const process = ref<ProcessInfo>({} as ProcessInfo);
const receivingResults = ref<Array<ReceivingResult>>([]);

onMounted(async () => {
  const loader = loadingOverlay.show();
  await fetchData();
  loader.hide();
});

const stepper = useStepper(["regForm", "printForm"]);

const printPageRef = ref<{ extractInnerHtml: () => string }>();
const ynDialog = useYesNoDialog({
  onYes: async () => {
    model.value.htmlDocument = printPageRef.value.extractInnerHtml();

    await updateByProcessGuid(model.value, processGuid);
  },
  onCloseWindow: () => {
    setTimeout(() => {
      router.push({ name: "ViewPage", params: { processGuid: processGuid } });
    }, 2000);
  },
  closeOnSuccess: true,
});

async function fetchData() {
  await Promise.all([
    whoAmI().then((res) => (model.value.data.executor = res)),
    getLanguages().then((res) => (languages.value = res)),
    getIndexNomenclatures().then((res) => (indexNomenclatures.value = res)),
    getConfidenceTypes().then(
      (data) => (confidenceTypes.value = data.filter((obj) => obj.id !== 5))
    ),
    getApprovalTypes().then((res) => (approvalTypes.value = res)),
    getOfficialMemoByProcessGuid(processGuid).then(async (res) => {
      if (res) {
        model.value = res;
        receivingResults.value = await getReceivingResults(
          model.value.data.messageGuid
        );
        model.value.data.signData = undefined;
        model.value.data.dueToDate = new Date(model.value.data.dueToDate);
      }
    }),
    getProcessInfoByProcessGuid(processGuid).then(
      (res) => (process.value = res)
    ),
  ]);

  model.value.data.responsible = model.value.data.executor;
}

const validationSchema: ValidationFieldsOf<CoreData> = reactive({
  initiator: "required",
  language: "required",
  confidenceType: "required",
  signer: "required",
  recipients: "required",
  details: "required",
  subject: "required",
  dueToDate: "required",
  position: "required",
  "executor.localPhone": "required",
  "executor.email": "required",
});
const { validate } = useForm({
  validationSchema: validationSchema,
  keepValuesOnUnmount: true,
});

async function validateAndGoNext() {
  const { valid, errors } = await validate();

  let totalErrors = { ...errors };
  let isValid = valid;

  console.table(totalErrors);

  if (isValid) stepper.goToNext();
}

const updateSigner = function (newValue) {
  if (newValue === undefined || newValue === null) {
    return undefined;
  }
  model.value.data.signer = newValue;
};

const getPropertyName = function () {
  return model.value.data.language?.includes("Қазақша")
    ? "positionKz"
    : model.value.data.language?.includes("Английский")
    ? "positionEn"
    : "positionRu";
};

const currentPosition = computed(() => {
  if (!model.value.data.signer) return undefined;
  return model.value.data.signer[getPropertyName()];
});

const setCurrentPosition = (newValue) => {
  model.value.data.signer[getPropertyName()] = newValue;
};

const selectedItem = ref<ProcessMessage>({} as ProcessMessage);

const attachments = ref<Attachment[]>([]);
const selectedDocument = ref<Attachment>();
watch(model, (value) => {
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
              <documents-list-block
                :items="model.data?.attachments"
                v-model:selectedDocument="selectedDocument"
              />
            </template>
            <div class="tw-col-start-2 tw-rounded-lg tw-bg-white">
              <Transition>
                <div v-if="stepper.isCurrent('regForm')">
                  <card class="tw-w-[210mm]">
                    <template #content>
                      <v-form
                        :schema="validationSchema"
                        class="tw-grid tw-grid-flow-row tw-grid-cols-12 tw-gap-x-4 tw-gap-y-1 tw-pt-4"
                      >
                        <v-text-field
                          label="Инициатор"
                          name="initiator"
                          class="tw-col-span-6"
                          v-model="model.data.executor.name"
                          disabled
                        />
                        <v-text-field
                          label="Внутренний номер"
                          name="executor.localPhone"
                          class="tw-col-span-6"
                          @blur="
                            async () =>
                              await renamePosition(model.data.executor)
                          "
                          v-model="model.data.executor.localPhone"
                          disabled
                        />
                        <v-text-field
                          label="Email"
                          name="executor.email"
                          class="tw-col-span-6"
                          v-model="model.data.executor.email"
                          disabled
                        />

                        <v-select
                          label="Язык документа"
                          name="language"
                          class="required tw-col-span-6"
                          :options="languages"
                          v-model="model.data.language"
                          disabled
                        />

                        <v-select
                          label="Индекс номенклатуры"
                          name="indexNomenclature"
                          class="required tw-col-span-12"
                          :options="indexNomenclatures"
                          v-model="model.data.indexNomenclature"
                          optionLabel="name"
                          optionValue="index"
                          disabled
                        />

                        <v-textarea
                          label="Тема документа"
                          name="subject"
                          class="tw-col-span-12"
                          v-model="model.data.subject"
                          auto-resize
                          disabled
                        />

                        <v-select
                          label="Гриф конфиденциальности"
                          name="confidenceType"
                          class="required tw-col-span-6"
                          :options="confidenceTypes"
                          :optionLabel="
                            model.data.language?.toLowerCase().includes('қаз')
                              ? 'displayTextKz'
                              : 'displayTextRu'
                          "
                          :optionValue="(v: ConfidenceType) => v"
                          v-model="model.data.confidenceType"
                          disabled
                        />

                        <v-number-field
                          label="Количество листов"
                          name="amountPage"
                          class="tw-col-span-6"
                          v-model="model.data.amountPage"
                          disabled
                        />

                        <v-datetime-picker
                          label="Срок исполнения"
                          name="dueToDate"
                          v-model="model.data.dueToDate"
                          class="tw-col-span-6"
                          :minDate="new Date()"
                        />

                        <v-select
                          label="Тип согласования"
                          name="approvalMode"
                          class="required tw-col-span-6"
                          :options="approvalTypes"
                          option-label="title"
                          option-value="value"
                          v-model="model.data.approvalMode"
                          disabled
                        />

                        <v-employee-select
                          label="Согласующие"
                          name="approvers"
                          v-model="model.data.approvers"
                          class="tw-col-span-12"
                          multiple
                          :unselectedEmployees="
                            model.data?.approvalResults?.map(
                              (item) => item.approver
                            )
                          "
                          :readonly="
                            process.schemeId === 'OfficialMemoApprove'
                              ? false
                              : true
                          "
                        />

                        <v-employee-select
                          label="Подписывающий"
                          name="signer"
                          @update:modelValue="updateSigner($event)"
                          :model-value="model.data.signer"
                          class="required tw-col-span-6"
                          readonly
                        />

                        <v-text-field
                          label="Должность"
                          name="position"
                          class="tw-col-span-6"
                          @blur="
                            async () => await renamePosition(model.data.signer)
                          "
                          @update:modelValue="setCurrentPosition($event)"
                          :model-value="currentPosition"
                          disabled
                        />

                        <v-employee-select
                          label="Получатели"
                          name="recipients"
                          v-model="model.data.recipients"
                          class="tw-col-span-12"
                          multiple
                          :unselectedEmployees="
                            receivingResults
                              ? receivingResults?.map((item) => item.receiver)
                              : undefined
                          "
                          :isOnlyEmployee="false"
                        />

                        <v-editor
                          label="Текст СЗ"
                          name="details"
                          class="tw-col-span-full"
                          v-model="model.data.details"
                          readonly
                        />

                        <o-file-input
                          v-model="model.data.attachments"
                          class="tw-col-span-12"
                          name="attachments"
                          disabled
                        />
                      </v-form>
                    </template>
                  </card>
                </div>
                <div v-else-if="stepper.isCurrent('printForm')">
                  <card class="tw-w-[210mm]">
                    <template #content>
                      <print-page
                        v-model:htmlContent="model.htmlDocument"
                        ref="printPageRef"
                      >
                        <default_kz
                          v-if="model?.data?.language == 'Қазақша'"
                          :model="model"
                        />
                        <default_ru
                          v-else-if="model?.data?.language == 'Русский'"
                          :model="model"
                        />
                      </print-page>
                    </template>
                  </card>
                </div>
              </Transition>
            </div>
            <template v-slot:right-aside>
              <actions-block>
                <div class="tw-flex tw-flex-row tw-flex-wrap tw-gap-2">
                  <ul class="hover-ul">
                    <div v-if="stepper.isCurrent('regForm')">
                      <li class="hover-li">
                        <icon-action
                          label="Далее"
                          class="hover-list"
                          @click="validateAndGoNext()"
                        />
                        <icon-action
                          label="Назад"
                          class="hover-list"
                          @click="
                            () => {
                              router.push({
                                name: 'ViewPage',
                                params: { processGuid: processGuid },
                              });
                            }
                          "
                        />
                      </li>
                    </div>
                    <div v-else-if="stepper.isCurrent('printForm')">
                      <li class="hover-li">
                        <yes-no-dialog
                          label="Сохранить"
                          title="Сохранить изменения?"
                          @yes="ynDialog.yes"
                          @no="ynDialog.no"
                          @close="closeWindow"
                          :state="ynDialog.state.value"
                          v-model="ynDialog.show.value"
                          :is-loading="ynDialog.isLoading.value"
                          yesButtonTitle="Сохранить"
                          successMessage="Успешно сохранен"
                          :hideCloseButton="true"
                          :width="400"
                        />
                      </li>
                      <li class="hover-li">
                        <icon-action
                          label="Изменить"
                          class="hover-list"
                          @click="stepper.goToPrevious()"
                        />
                      </li>
                    </div>
                  </ul>
                </div>
              </actions-block>
            </template>
          </adaptive-three-column>
        </o-tab-item>
        <o-tab-item title="Ход исполнения" value="history">
          <process-history-tree
            class="tw-m-2"
            :process-guid="processGuid"
            v-model:selectedItem="selectedItem"
          />
        </o-tab-item>
      </template>
    </o-tab>
  </wide-layout>
</template>
