<script setup lang="ts">
import getReceivingResults from "@/server/api/process/getReceivingResults";
import { onBeforeMount, ref, watchEffect } from "vue";
import "@/assets/css/history-tree.css";
import type { ReceivingResult } from "@/types/process/ReceivingResult";
import useAsyncKeyedState from "@/features/shared/composables/useAsyncKeyedState";
import { useErrorCatchingFn } from "@/features/shared/composables/useErrorCatchingFn";

const props = defineProps<{
  messageGuid?: string;
}>();
const items = ref<Array<ReceivingResult>>([]);

const previousStepCount = ref<number>(0);

const visibility = ref(items?.value?.length < 20);
const ShowAll = ref(true);

// async function fetchData(messageGuid: string) {
//   if (!messageGuid) return;
//   items.value = await getReceivingResults(messageGuid);
//   previousStepCount.value = items.value.length;
// }

const { state, isLoading, isError, fetch } = useAsyncKeyedState(
  async (messageGuid: string) => {
    if (!messageGuid) return;
    items.value = await getReceivingResults(messageGuid);
    previousStepCount.value = items.value?.length;

    return undefined;
  }
);

onBeforeMount(() => {
  isLoading.value = true;
});

const { execute: fetchData } = useErrorCatchingFn(
  async () => await fetch(props.messageGuid),
  {
    showToast: true,
    errorMessage: "Произошла ошибка при получении данных",
  }
);
watchEffect(() => {
  if (props.messageGuid) fetchData();
});

function setVisibility(value: boolean) {
  visibility.value = !value;
}
function updateShow(showValue: boolean) {
  ShowAll.value = showValue;
}
</script>

<template>
  <div>
    <span
      v-tooltip.right="{ value: !visibility ? 'Раскрыть всё' : 'Свернуть всё' }"
      class="tw-ml-[20px]"
    >
      <v-icon
        @click="setVisibility(visibility)"
        fill="var(--magenta-color)"
        class="tw-cursor-pointer"
        :name="'ri-node-tree'"
      />
    </span>
    <div class="tw-text-xs tw-mt-4">
      <table
        class="tw-w-[99%] tw-absolute"
        style="vertical-align: inherit !important"
      >
        <thead>
          <tr
            class="tw-border-gray-500 tw-border-collapse tw-border tw-border-b tw-font-bold"
          >
            <td class="tw-pb-2 tw-pl-[30px]">Исполнители</td>
            <td class="tw-pb-2">Дата исполнения</td>
            <!-- <td class="tw-pb-2" style="padding-left: 10px">Время поступления</td> -->
            <!-- <td class="tw-pb-2" style="padding-left:10px">Срок исполнения</td> -->
            <!-- <td class="tw-w-[430px] tw-pb-2">Тип запроса</td> -->
            <!-- <td class="tw-pb-2" style="padding-left: 10px">Дата исполнения</td>
        <td class="tw-pb-2">Текст исполнения</td> -->
            <td class="tw-pb-2">Информация об исполнении</td>
            <td class="tw-pb-2">Текст исполнения</td>
            <td class="tw-pb-2">Вложения исполнения</td>
          </tr>
        </thead>
        <tbody>
          <tr class="tw-h-[50vh]" v-if="isLoading">
            <Skeleton class="!tw-absolute" width="100%" height="100%" />
          </tr>
          <tr v-else-if="!items && !isLoading" class="!tw-pl-5">
            <div
              class="tw-absolute tw-top-[20vh] tw-left-[45vw] tw-flex tw-flex-col tw-justify-center tw-items-center tw-text-[20px] tw-opacity-70"
            >
              <v-icon
                name="md-warningamber-round"
                fill="var(--secondary-color)"
                class="tw-text-center tw-h-[100px] tw-w-[100px]"
              />
              <br />
              <span>Нет информации</span>
            </div>
          </tr>
          <receiving-result-tree-item
            v-else
            v-for="(item, index) in items"
            :key="index"
            :message-guid="messageGuid"
            :item="item"
            :receiver="item.receiver"
            :items-length="items.length"
            :level="index"
            :is-root="true"
            :siblings-count="0"
            :is-visible="visibility"
            @update-show="updateShow"
            @update-previous="previousStepCount = items.length"
          />
        </tbody>
      </table>
    </div>
  </div>
</template>
