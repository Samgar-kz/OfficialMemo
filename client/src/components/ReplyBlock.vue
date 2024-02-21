<script setup lang="ts">
import type { Reply } from "@/types/Reply";
import Card from "primevue/card";

const props = defineProps<{
  reply: Reply;
  title?: string;
}>();
</script>

<template>
  <card-block :header-height="40" class="tw-h-fit">
    <template v-slot:header>
      <div class="tw-px-4">
        {{ reply?.replyDecisionName ?? title }}
      </div>
    </template>
    <card class="tw-rounded-none">
      <template #header>
        <div class="tw-mx-4 tw-flex tw-justify-between tw-pt-4">
          <div class="tw-flex tw-w-full tw-items-center tw-justify-between">
            <span>{{ reply?.repliedBy?.name }}</span>
            <div
              class="tw-flex tw-rounded-lg tw-px-2 tw-py-1 tw-text-white"
              :class="{
                'tw-bg-green-500': reply?.replyDecision == 'approve',
                'tw-bg-red-600': reply?.replyDecision == 'rework',
              }"
            >
              <span class="tw-self-center">{{ reply.replyDecisionName }} </span>
              <i
                class="pi tw-ml-2 tw-self-center tw-text-sm"
                :class="{
                  'pi-check-circle': reply?.replyDecision == 'approve',
                  'pi-minus-circle': reply?.replyDecision == 'rework',
                }"
              />
            </div>
          </div>
        </div>
      </template>
      <template #content>
        <div class="tw-ml-4 tw-grid tw-grid-cols-[150px_1fr]">
          <span class="text-caption">Дата исполнения:</span>
          <span>{{ new Date(reply?.replyDate).toLocaleString("kk-KZ") }}</span>

          <span class="text-caption">Коментарий:</span>
          <span>{{ reply?.replyComment }}</span>

          <documents-list
            class="tw-col-span-2 tw-mt-2"
            :items="reply?.replyDocuments"
          />
        </div>
      </template>
      <template #footer>
        <slot name="actions" />
      </template>
    </card>
  </card-block>
</template>
