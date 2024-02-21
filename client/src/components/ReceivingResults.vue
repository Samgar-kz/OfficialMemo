<script setup lang="ts">
import type { ReceivingResult } from "@/types/process/ReceivingResult";
import Accordion from "primevue/accordion";
import AccordionTab from "primevue/accordiontab";

defineProps<{
  receivingResults?: ReceivingResult[];
}>();
</script>

<template>
  <card-block class="approval-result">
    <template v-slot:header>
      <div class="tw-px-4">Результаты исполнения</div>
    </template>
    <div
      v-if="!receivingResults?.length"
      class="tw-w-full tw-py-3 tw-text-center"
    >
      <span class="tw-font-thin">Нет исполнении</span>
    </div>
    <Accordion class="tw-divide-y-2" v-else>
      <AccordionTab
        v-for="(subrequest, index) in receivingResults"
        :key="index"
      >
        <template v-slot:header>
          <div class="tw-flex tw-w-full tw-items-center tw-justify-between">
            <span>{{ subrequest.receiver?.name }}</span>
            <div
              class="tw-flex tw-rounded-lg tw-px-2 tw-py-1 tw-text-white"
              :class="{
                'tw-bg-green-500': subrequest.result == 'accept',
              }"
            >
              <span class="tw-self-center">Исполнено </span>
              <i
                class="pi tw-ml-2 tw-self-center tw-text-sm"
                :class="{
                  'pi-check-circle': subrequest.result == 'accept',
                }"
              />
            </div>
          </div>
        </template>
        <div class="tw-ml-4 tw-grid tw-grid-cols-[150px_1fr]">
          <!-- <span class="tw-text-sm tw-font-semibold">Дата запроса:</span>
          <span>{{ new Date(subrequest.created).toLocaleString("kk-KZ") }}</span> -->

          <!-- <span class="tw-text-sm tw-font-semibold" v-if="subrequest.dueToDate">Срок исполнения:</span>
          <span v-if="subrequest.dueToDate">{{ new Date(subrequest.dueToDate).toLocaleString("kk-KZ") }}</span> -->
          <span class="tw-text-sm tw-font-semibold">Дата исполнения:</span>
          <span>{{
            new Date(subrequest.created).toLocaleString("kk-KZ")
          }}</span>

          <span class="tw-text-sm tw-font-semibold">Коментарий:</span>
          <span>{{ subrequest.comment }}</span>

          <documents-list class="tw-col-span-2" :items="subrequest.documents" />

          <!-- <template v-if="subrequest.replyDate">
            <divider class="tw-col-span-2 tw-my-4 tw--ml-4" />

            <span class="tw-text-sm tw-font-semibold">Ответ:</span>
            <span>{{ subrequest.replyDecisionName }}</span>

            <span class="tw-text-sm tw-font-semibold">Исполнитель:</span>
            <span>{{ subrequest.repliedByName }}</span>

            <span class="tw-text-sm tw-font-semibold">Дата исполнения:</span>
            <span>{{ new Date(subrequest.replyDate).toLocaleString("kk-KZ") }}</span>

            <template v-if="subrequest.replyComment">
              <span class="tw-text-sm tw-font-semibold">Коментарий:</span>
              <p v-for="(line, index) in subrequest.replyComment.split('\n')" :key="index">{{ line }}</p>
            </template>

            <documents-list class="tw-col-span-2" :items="subrequest.replyDocuments" />
          </template> -->
        </div>
      </AccordionTab>
    </Accordion>
  </card-block>
</template>

<style lang="scss">
.approval-result .p-card-body {
  padding: 0 !important;
}
.approval-result .p-accordion-content {
  border-width: 0px;
}

.approval-result .p-accordion .p-accordion-tab:last-child {
  margin-bottom: 0;
}

.approval-result .p-accordion-header {
  .p-accordion-header-link {
    background-color: transparent;
    border-width: 0px;
  }
}
</style>
