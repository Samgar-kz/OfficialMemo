<script setup lang="ts">
import getProcessHistory from "@/server/api/processHistory/getProcessHistory";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import { onMounted, ref, watchEffect } from "vue";
import "@/assets/css/history-tree.css";
import useAsyncKeyedState from "@/features/shared/composables/useAsyncKeyedState";
import { useErrorCatchingFn } from "@/features/shared/composables/useErrorCatchingFn";

const props = defineProps<{
  processGuid?: string;
}>();
const items = ref<Array<ProcessMessage>>([]);
const firstItem = ref<Array<ProcessMessage>>([]);

const previousStepCount = ref<number>(0);

const visibility = ref(items?.value?.length < 20);
const ShowAll = ref(true);

const { state, isLoading, isError, fetch } = useAsyncKeyedState(
  async (processGuid: string) => {
    if (!processGuid) return;

    await getProcessHistory(processGuid).then((res) => (items.value = res));

    previousStepCount.value = items.value?.length || 0;
    firstItem.value = items.value?.slice(0, 1) || [];

    return undefined;
  }
);

const { execute: fetchData } = useErrorCatchingFn(
  async () => await fetch(props.processGuid),
  {
    showToast: true,
    errorMessage: "Произошла ошибка при получении данных",
  }
);

watchEffect(() => {
  if (props.processGuid) fetchData();
});

// async function fetchData(processGuid: string) {
//   if (!processGuid) return;
//   isLoading.value = true;
//     isError.value = false;
//     try {
//       items.value = await getProcessHistory(processGuid);
//     } catch (error) {

//     }

//   previousStepCount.value = items.value.length;
//   if (items.value) firstItem.value = items.value.slice(0, 1);
// }
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
            <td class="tw-w-40 tw-pb-2">Вложения</td>
            <!-- <td class="tw-pb-2" style="padding-left: 10px">Время поступления</td> -->
            <!-- <td class="tw-pb-2" style="padding-left:10px">Срок исполнения</td> -->
            <td class="tw-w-[430px] tw-pb-2">Тип запроса</td>
            <td class="tw-pb-1 tw-text-center"><span>Статус</span></td>
            <!-- <td class="tw-pb-2" style="padding-left: 10px">Дата исполнения</td>
        <td class="tw-pb-2">Текст исполнения</td> -->
            <td class="tw-pb-2 tw-pl-[30px]">Информация об исполнении</td>
            <td class="tw-pb-2">Вложения исполнения</td>
          </tr>
        </thead>
        <tbody>
          <tr class="tw-h-[50vh]" v-if="isLoading">
            <Skeleton
              class="!tw-absolute tw-h-[50vh]"
              width="100%"
              height="100%"
            />
          </tr>
          <process-history-tree-item
            v-else
            v-for="(item, index) in ShowAll ? items : firstItem"
            :key="index"
            :process-guid="processGuid"
            :item="item"
            :items-length="(ShowAll ? items : firstItem).length"
            :level="index"
            :is-root="true"
            :siblings-count="0"
            :parent-message="item.messageComment ?? undefined"
            :parent-executor="item.executorCode"
            :is-visible="visibility"
            @update-show="updateShow"
            @update-previous="previousStepCount = items.length"
          />
        </tbody>
      </table>
    </div>
  </div>
</template>
