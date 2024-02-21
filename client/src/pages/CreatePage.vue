<script setup lang="ts">
import templates from "@/printPages";
import useYesNoDialog from "@/composables/useYesNoDialog";
import renamePosition from "@/server/api/employees/renamePosition";
import whoAmI from "@/server/api/employees/whoAmI";
import getApprovalTypes from "@/server/api/handbooks/getApprovalTypes";
import getConfidenceTypes from "@/server/api/handbooks/getConfidenceTypes";
import getLanguages from "@/server/api/handbooks/getLanguages";
import getIndexNomenclatures from "@/server/api/handbooks/getIndexNomenclatures";
import create from "@/server/api/process/create";
import closeWindow from "@/server/closeWindow";
import type { Employee } from "@/types/employees/Employee";
import type { MyDocument } from "@/types/contents/MyDocument";
import type { ConfidenceType } from "@/types/process/ConfidenceType";
import type { LabelValue } from "@/types/process/LabelValue";
import type { CoreData } from "@/types/process/CoreData";
import type { Model } from "@/types/process/Model";
import type ValidationFieldsOf from "@/types/utility/ValidationFieldsOf";
import { useStepper, useTitle } from "@vueuse/core";
import Card from "primevue/card";
import { useForm } from "vee-validate";
import { computed, onMounted, reactive, ref, watchEffect } from "vue";
import { useRoute } from "vue-router";
import addDays from "@/utils/getDate";
import type { IndexNomenclature } from "@/types/process/IndexNomenclature";
import WideLayout from "@/layouts/WideLayout.vue";
import { useToast } from "primevue/usetoast";
import useAsyncKeyedState from "@/features/shared/composables/useAsyncKeyedState";
import { useErrorCatchingFn } from "@/features/shared/composables/useErrorCatchingFn";
import loadingOverlay from "@/plugins/vue-loading-overlay";

const title = useTitle("Создание служебной записки", {
  titleTemplate: "%s | Документооборот",
});

const stepper = useStepper(["regForm", "printForm"]);

const languages = ref<string[]>();
const confidenceTypes = ref<ConfidenceType[]>();
const indexNomenclatures = ref<IndexNomenclature[]>();
const approvalTypes = ref<LabelValue[]>();
const isFocused = ref<boolean>(false);

const printPageRef = ref<{ extractInnerHtml: () => string }>();
const registeredNum = ref("");
const ynDialog = useYesNoDialog({
  onYes: async () => {
    model.value.htmlDocument = printPageRef.value.extractInnerHtml();
    registeredNum.value = await create(model.value);
  },
  onCloseWindow: closeWindow,
  closeOnSuccess: false,
});

const route = useRoute();

const model = ref<Model<any>>({
  data: { executor: {} } as CoreData,
} as Model<any>);
async function modelDefaults(): Promise<Model<any>> {
  const defaults: CoreData = {
    attachments: undefined as MyDocument[],
    approvalMode: "parallel",
    executor: {} as Employee,
    approvers: undefined as Employee[],
    recipients: undefined as Employee[],
    confidenceType: undefined,
    subject: undefined,
    dueToDate: await addDays(new Date(), 15, {
      dayType: "calendarDay",
      addRule: "inclusive",
    }),
    amountPage: undefined,
    initiator: undefined,
    language: "Русский",
    indexNomenclature: undefined,
    signer: undefined as Employee,
    responsible: undefined as Employee,
    details: undefined,
    signData: undefined,
    approvalResults: [],
  };
  return {
    data: defaults,
  } as Model<any>;
}

// async function fetchData() {
//   await Promise.all([
//     whoAmI().then((res) => (model.value.data.executor = res)),
//     getLanguages().then((res) => (languages.value = res)),
//     getIndexNomenclatures().then((res) => (indexNomenclatures.value = res)),
//     getConfidenceTypes().then(
//       (data) => (confidenceTypes.value = data.filter((obj) => obj.id !== 5))
//     ),
//     getApprovalTypes().then((res) => (approvalTypes.value = res)),
//   ]);

//   model.value.data.responsible = model.value.data.executor;
//   model.value.data.confidenceType = confidenceTypes.value[3];
// }

const sleepNow = (delay) =>
  new Promise((resolve) => setTimeout(resolve, delay));

const { state, isLoading, isError, fetch } = useAsyncKeyedState(
  async (messageGuid: string) => {
    const loader = loadingOverlay.show();
    const fetchDataWithTimeout = async () => {
      try {
        await Promise.all([
          whoAmI().then((res) => (model.value.data.executor = res)),
          getLanguages().then((res) => (languages.value = res)),
          getIndexNomenclatures().then(
            (res) => (indexNomenclatures.value = res)
          ),
          getConfidenceTypes().then(
            (data) =>
              (confidenceTypes.value = data.filter((obj) => obj.id !== 5))
          ),
          getApprovalTypes().then((res) => (approvalTypes.value = res)),
        ]);

        model.value.data.responsible = model.value.data.executor;
        model.value.data.confidenceType = confidenceTypes.value[3];
        model.value.data.indexNomenclature = indexNomenclatures.value[0].name;
        loader.hide();
      } catch (error) {
        // Handle errors
        console.error("Error fetching data:", error);
        loader.hide();
      }
    };

    // Set a timeout for fetchData
    const timeout = setTimeout(() => {
      console.log("Timeout occurred, reloading the window...");
      window.location.reload(); // Reload the window after the timeout
    }, 3000);

    // Clear timeout after fetching data
    await fetchDataWithTimeout();
    clearTimeout(timeout);
    return undefined;
  }
);

const { execute: fetchData } = useErrorCatchingFn(
  async () => await fetch(null),
  {
    showToast: true,
    errorMessage: "Произошла ошибка при получении данных",
  }
);

// watch(isLoading, (val) => {
//   console.log(val);

//   if (!val) loader.hide();
// });

onMounted(async () => {
  model.value = await modelDefaults();
  await fetchData();
});

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
  validateOnMount: false,
  keepValuesOnUnmount: true,
});

const toast = useToast();
async function validateAndGoNext() {
  const { valid, errors } = await validate();
  let totalErrors = { ...errors };
  let isValid = valid;

  console.table(totalErrors);

  if (isValid) {
    model.value.data.details = repleceTabChar(model.value.data?.details);
    stepper.goToNext();
  } else {
    toast.add({
      severity: "error",
      summary: "Ошибка",
      detail: "Заполните все обязательные поля",
      life: 3000,
    });
  }
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

const emit = defineEmits(["update:modelValue"]);

function repleceTabChar(str) {
  return str.replace(/\t/g, "&nbsp;&nbsp;&nbsp;&nbsp;");
}

watchEffect(() => {
  console.log(model.value.data.attachments);
});
</script>

<template>
  <wide-layout class="tw-bg-slate-100">
    <div
      class="tw-grid tw-grid-cols-[min-content] tw-justify-center tw-gap-4 tw-px-4 tw-py-4 xl:tw-grid-cols-[1fr_auto] 2xl:tw-grid-cols-[4fr_auto_5fr]"
    >
      <div class="tw-flex tw-flex-col tw-gap-4"></div>
      <div class="tw-flex tw-flex-col tw-gap-4">
        <form-block :title="title" class="tw-w-[220mm]">
          <div v-if="stepper.isCurrent('regForm')">
            <card>
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
                      async () => await renamePosition(model.data.executor)
                    "
                    v-model="model.data.executor.localPhone"
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
                  />
                  <o-select-index
                    label="Индекс номенклатуры"
                    name="indexNomenclature"
                    class="tw-col-span-10"
                    :options="indexNomenclatures"
                    editable
                    optionLabel="name"
                    optionValue="index"
                    v-model="model.data.indexNomenclature"
                  />
                  <nomenclature-dialog
                    class="tw-col-span-2"
                    label="Фильтр"
                    v-model="model.data.indexNomenclature"
                    :width="700"
                    ><template v-slot:default="{ click, label }">
                      <Button
                        :label="label"
                        :severity="label"
                        @click="click"
                        class="tw-w-max tw-mt-[25px] tw-w-[118px] p-button-outlined p-button-sm"
                        icon="pi pi-sliders-h"
                        icon-pos="right" /></template
                  ></nomenclature-dialog>

                  <v-textarea
                    label="Тема документа"
                    name="subject"
                    class="tw-col-span-12"
                    v-model="model.data.subject"
                    auto-resize
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
                  />

                  <v-number-field
                    label="Количество листов"
                    name="amountPage"
                    class="tw-col-span-6"
                    v-model="model.data.amountPage"
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
                    class="tw-col-span-6"
                    :options="approvalTypes"
                    option-label="title"
                    option-value="value"
                    v-model="model.data.approvalMode"
                  />

                  <v-employee-select
                    label="Согласующие"
                    name="approvers"
                    v-model="model.data.approvers"
                    class="tw-col-span-12"
                    multiple
                  />

                  <v-employee-select
                    label="Подписывающий"
                    name="signer"
                    @update:modelValue="updateSigner($event)"
                    :model-value="model.data.signer"
                    class="required tw-col-span-6"
                  />
                  <v-text-field
                    label="Должность"
                    name="position"
                    class="tw-col-span-6"
                    @blur="async () => await renamePosition(model.data.signer)"
                    @update:modelValue="setCurrentPosition($event)"
                    :model-value="currentPosition"
                  />

                  <group-list
                    class="tw-col-span-full"
                    :executors="model.data.recipients"
                    :userLogin="model.data.executor"
                    @update-executors="
                      (child) => (model.data.recipients = child)
                    "
                    :isFocused="isFocused"
                  />

                  <v-employee-select
                    label="Получатели"
                    name="recipients"
                    v-model="model.data.recipients"
                    class="tw-col-span-12"
                    multiple
                    :isOnlyEmployee="false"
                    @isFocused="
                      (child) => {
                        isFocused = child;
                      }
                    "
                  />

                  <v-editor
                    label="Текст СЗ"
                    name="details"
                    class="tw-col-span-full"
                    v-model="model.data.details"
                  />

                  <o-file-input
                    v-model="model.data.attachments"
                    class="tw-col-span-12"
                    name="attachments"
                  />
                </v-form>
              </template>
            </card>
          </div>

          <div v-else-if="stepper.isCurrent('printForm')">
            <card>
              <template #content>
                <print-page
                  v-model:htmlContent="model.htmlDocument"
                  ref="printPageRef"
                >
                  <component
                    :is="templates.printPage(model.data.language)"
                    :model="model"
                  />
                </print-page>
              </template>
            </card>
          </div>
        </form-block>
      </div>
      <div class="tw-flex tw-flex-col tw-gap-4">
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
                </li>
              </div>
              <div v-else-if="stepper.isCurrent('printForm')">
                <li class="hover-li">
                  <yes-no-dialog
                    label="Отправить"
                    title="Вы действительно хотите отправить служебную записку?"
                    @yes="ynDialog.yes"
                    @no="ynDialog.no"
                    @close="closeWindow"
                    :state="ynDialog.state.value"
                    v-model="ynDialog.show.value"
                    :is-loading="ynDialog.isLoading.value"
                    yesButtonTitle="Отправить"
                    successMessage="Успешно отправлено"
                    :width="400"
                    ><template v-slot:successMessage="{ successMessage }">
                      <p>{{ successMessage }}</p>
                      <!-- <p> -->
                      <span>Рег. номер: </span>
                      <copy-on-click-text
                        :value="registeredNum"
                      ></copy-on-click-text>
                      <!-- </p> -->
                    </template></yes-no-dialog
                  >
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
      </div>
    </div>
  </wide-layout>
</template>
