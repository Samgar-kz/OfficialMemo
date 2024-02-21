<script setup lang="ts">
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import Card from "primevue/card";
import Divider from "primevue/divider";
import { ref } from "vue";
import DocumentsList from "@/features/documents/components/DocumentsList.vue";

const props = defineProps<{
  step: ProcessMessage;
}>();
const statuses = ref({
  completed: "Исполнено",
  approved: "Ответ принят",
  toApprove: "Принятие ответа",
  toPerform: "На исполнении",
  withChildren: "В ожидании исполнения",
  createdStep: "Создан",
});
</script>

<template>
  <card-block :header-height="40" class="tw-max-w-[800px]">
    <template v-slot:header>
      <div class="tw-px-4">
        {{ step.stepName }}
      </div>
    </template>
    <card class="tw-rounded-none">
      <template #content>
        <div class="tw-grid tw-grid-cols-[auto_auto]">
          <span class="tw-text-sm tw-font-semibold" v-show="step.initiatorName"
            >Инициатор:</span
          >
          <span v-show="step.initiatorName">{{ step.initiatorName }}</span>

          <span class="tw-text-sm tw-font-semibold">{{
            step.messageType === "task" ? "Тип поручения:" : "Тип запроса:"
          }}</span>
          <span class="cs-overflow-cLass">{{ step.stepName }}</span>

          <span class="tw-text-sm tw-font-semibold">Статус:</span>
          <span>{{
            step.messageStatusName ?? statuses[step.messageStatus]
          }}</span>

          <divider  class="tw-m-2"/>

          <p class="tw-col-span-2 tw-mb-2 tw--ml-2">Сообщение:</p>

          <span class="tw-text-sm tw-font-semibold">Исполнитель:</span>
          <span>{{ step.executorName }}</span>

          <span class="tw-text-sm tw-font-semibold tw-mr-4"
            >Дата поступления:</span
          >
          <span>{{ new Date(step.messageDate)?.toLocaleString("kk-KZ") }}</span>
          <template v-if="step.dueToDate">
            <span class="tw-text-sm tw-font-semibold tw-mr-4"
              >Срок исполнения:</span
            >
            <span>{{ new Date(step.dueToDate)?.toLocaleString("kk-KZ") }}</span>
          </template>
          <span class="tw-text-sm tw-font-semibold">Вложения:</span>
          <span>
            <documents-list
              v-if="step.messageDocuments?.length"
              class="tw-col-span-full"
              :items="step.messageDocuments"
            />
          </span>

          <template v-if="step.stepName != step.messageComment">
            <p
              class="tw-col-span-2 tw-mt-2 tw--mb-2 tw-text-center tw-font-semibold"
            >
              Комментарий:
            </p>
            <p
              class="tw-col-span-2 tw-mt-2 tw--mb-2 tw-text-justify tw-break-normal"
            >
              {{ step.messageComment }}
            </p>
          </template>

          <divider v-if="step.replyDate"  class="tw-m-2"/>

          <template v-if="step.replyDate">
            <p class="tw-col-span-2 tw-mb-2 tw--ml-2">Исполнение:</p>
            <template>
              <span class="tw-text-sm tw-font-semibold tw-mr-4"
                >Дата исполнения:</span
              >
              <span>{{
                new Date(step.replyDate ?? "").toLocaleString("kk-KZ")
              }}</span>
            </template>

            <template
              v-if="step.replyDate && step.userName != step.executorName"
            >
              <span class="tw-text-sm tw-font-semibold">Исполнил:</span>
              <span>{{ step.userName }}</span>
            </template>

            <template v-if="step.replyDecisionName">
              <span class="tw-text-sm tw-font-semibold">Решение:</span>
              <span>{{ step.replyDecisionName }}</span>
            </template>
            <span class="tw-text-sm tw-font-semibold">Вложения:</span>
            <span>
              <documents-list
                v-if="step.replyDocuments?.length > 0"
                class="tw-col-span-full"
                :items="step.replyDocuments"
              />
            </span>

            <p
              class="tw-col-span-2 tw-mt-2 tw--mb-2 tw-text-center tw-font-semibold"
              v-if="step.replyComment"
            >
              Комментарий:
            </p>
            <p
              class="tw-col-span-2 tw-mt-2 tw--mb-2 tw-text-justify tw-break-normal"
            >
              {{ step.replyComment }}
            </p>
          </template>

          <divider v-if="step.approvalDate" class="tw-m-2"/>
          <template v-if="step.approvalDate">
            <p class="tw-col-span-2 tw-mb-2 tw--ml-2">Принятие ответа:</p>

            <span class="tw-text-sm tw-font-semibold">Принял(а) ответ:</span>
            <span>{{ step.approverName }}</span>

            <span class="tw-text-sm tw-font-semibold tw-mr-4"
              >Дата принятия ответа:</span
            >
            <span>{{
              new Date(step.approvalDate ?? "").toLocaleString("kk-KZ")
            }}</span>

            <span class="tw-text-sm tw-font-semibold">Решение:</span>
            <span>{{ step.approvalDecisionName }}</span>

            <span class="tw-text-sm tw-font-semibold">Вложения:</span>
            <span>
              <documents-list
                v-if="step.approvalDocuments?.length > 0"
                class="tw-col-span-full"
                :items="step.approvalDocuments"
              />
            </span>

            <p
              class="tw-col-span-2 tw-mt-2 tw--mb-2 tw-text-center tw-font-semibold"
              v-if="step.approvalComment"
            >
              Комментарий:
            </p>
            <p
              class="tw-col-span-2 tw-mt-2 tw--mb-2 tw-text-justify tw-break-normal"
            >
              {{ step.approvalComment }}
            </p>
          </template>
        </div>
      </template>
      <template #footer>
        <div class="tw-flex tw-flex-row tw-gap-2">
          <slot name="actions" />
        </div>
      </template>
    </card>
  </card-block>
</template>

<style scoped>
.cs-overflow-cLass {
  max-height: 6rem;
  width: 300px;
  overflow: hidden;
  white-space: nowrap;
  text-overflow: ellipsis;
  display: block;
}
</style>
