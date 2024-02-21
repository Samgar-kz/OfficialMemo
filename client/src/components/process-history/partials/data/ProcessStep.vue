<script setup lang="ts">
import { onMounted, ref } from "vue";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";

const link = import.meta.env.VITE_BASE_URL;
const props = defineProps<{
  item: ProcessMessage;
  processGuid: string;
}>();
const isNewProcess = ref(false);
onMounted(() => {
  if (
    props.item.stepName
      .toLowerCase()
      ?.includes(props.item.regNum?.toLowerCase())
  ) {
    isNewProcess.value =
      (props.processGuid && props.processGuid) !== props.item.processGuid;
  }
});
</script>

<template>
  <overlay-process-step-block overlay-panel-class="cs-step-name" :step="item">
    <template #activator="{ click }">
      <span
        @click="(event) => !isNewProcess && click(event)"
        class="tw-cursor-pointer tw-align-super hover:tw-text-secondary"
      >
        <span
          v-if="isNewProcess"
          v-tooltip.top="{
            value: isNewProcess
              ? 'Переход на страницу процесса'
              : 'Вы уже находитесь на странице данного процесса',
            disabled: !item.stepName
              .toLowerCase()
              ?.includes(props.item.regNum?.toLowerCase()),
          }"
        >
          <a :href="link + '/' + item.processGuid + '/view'">
            {{ item.stepName }}</a
          >
        </span>
        <span v-else class="tw-relative tw-flex">
          <span
            v-for="(text, index) in item.stepName?.split('. ', 3)"
            v-bind:key="index"
            class="tw-absolute tw-w-max tw-overflow-hidden"
            :style="{ marginTop: (-21 + index * 12)?.toString() + 'px' }"
            >{{ text }}</span
          >
        </span></span
      >
    </template>
  </overlay-process-step-block>
</template>
