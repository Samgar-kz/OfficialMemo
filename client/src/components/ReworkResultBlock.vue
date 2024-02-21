<script setup lang="ts">
import CardBlock from "@/components/CardBlock.vue";
import Card from "primevue/card";
import Divider from "primevue/divider";
import type { ApprovalResult } from "@/types/process/ApprovalResult";

const props = defineProps<{
  title: string;
  approvalResults?: ApprovalResult[];
}>();
</script>

<template>
  <card-block>
    <template v-slot:header>
      <div class="tw-px-4">
        {{ title }}
      </div>
    </template>
    <card class="tw-rounded-none">
      <!-- <template #header>
        <div class="tw-flex tw-justify-between tw-px-4 tw-py-2">
          <span class="tw-text-base">{{ task.responsibleName }}</span>
          <slot name="replaceExecutor">
            <task-update-dialog
              v-if="updateTask"
              :task-guid="taskGuid"
              title="Поменять исполнителя"
              class="tw-bg-secondary"
              update-object="Executor"
              @update-task="fetchData" />
          </slot>
          <span
            class="tw-flex tw-items-center tw-justify-center tw-rounded-md tw-px-2 tw-text-center tw-text-xs tw-text-white"
            :class="{
              'bg-success': task.replyDecision == 'accept' || task.replyDecision == 'approve',
              'bg-error': task.replyDecision == 'reject' || task.replyDecision == 'rework',
            }"
            >{{ task.replyDecisionName }}</span
          >
        </div>
      </template> -->
      <template #content>
        <div
          v-for="(subrequest, index) in approvalResults"
          :key="index"
          class="tw-ml-4 tw-mt-4 tw-grid tw-grid-cols-[150px_1fr]"
        >
          <Divider
            class="tw-col-span-2 tw-mb-1 divider-text"
            align="left"
            type="solid"
            ><b>На доработку</b></Divider
          >
          <!-- <p class="tw-col-span-2 tw--ml-2 tw-mb-2">На доработку:</p> -->

          <span class="tw-text-sm tw-font-semibold">Инициатор:</span>
          <span>{{ subrequest.approver?.name }}</span>

          <span class="tw-text-sm tw-font-semibold">Дата:</span>
          <span>{{
            new Date(subrequest.created).toLocaleString("kk-KZ")
          }}</span>

          <span class="tw-text-sm tw-font-semibold">Коментарий:</span>
          <span>{{ subrequest.comment }}</span>

          <documents-list class="tw-col-span-2" :items="subrequest.documents" />
        </div>
      </template>

      <template #footer>
        <slot name="actions" />
      </template>
    </card>
  </card-block>
</template>
