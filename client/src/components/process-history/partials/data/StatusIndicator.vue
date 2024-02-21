<script setup lang="ts">
import { computed } from "vue";

type Status = { status: string; name: string; color: string; tooltip: string };
const props = withDefaults(
  defineProps<{
    status: string;
    childCount:number|undefined;
    replyApprovalDecisions:any[]|undefined;
    allowedStatuses?: {};
    defaultStatus?: string;
    tooltipPosition?: string;
  }>(),
  {
    tooltipPosition: "right",
    allowedStatuses: () => {
      return {
        approved: {
          name: "bi-circle-fill",
          color: "#5fb558",
          tooltip: "Исполнено",
        },
        completed: {
          name: "bi-circle-fill",
          color: "#5fb558",
          tooltip: "Исполнено",
        },
        redirected: {
          name: "bi-pause-circle-fill",
          color: "#5fb558",
          tooltip: "Перенаправлен",
        },
        toPerform: {
          name: "bi-play-circle-fill",
          color: "#fa972b",
          tooltip: "На исполнении",
        },
        error: {
          name: "pi pi-times-circle",
          color: "red",
          tooltip: "Ошибка",
        },
        toApprove: {
          name: "bi-circle-fill",
          color: "#fa972b",
          tooltip: "На проверке",
        },
        inWaitingPerform: {
          name: "bi-pause-circle-fill",
          color: "#5fb558",
          tooltip: "В ожидании исполнения",
        },
        canceled: {
          name: "oi-circle-clash",
          color: "red",
          tooltip: "Отменен",
        },
        reject: {
          name: "bi-play-circle-fill",
          color: "#fa972b",
          inverse: true,
          tooltip: "Отменен",
        },
        rework: {
          name: "io-reload-circle",
          color: "#9A247C",
          tooltip: "Решение: на доработку",
        },
        undefinedStatus: {
          name: "ri-question-line",
          color: "yellow",
          tooltip: "Неизвестный статус:",
        },
      };
    },
  }
);

const iconValue = computed(() => props.allowedStatuses[getStatus(props.status)] ?? props.allowedStatuses["undefinedStatus"]);

function getStatus(innerStatus) {
  let result = innerStatus;
  if (innerStatus === "toPerform" && props.childCount > 0) {
    result = "inWaitingPerform";
  }
  if (["completed","redirected"].includes(innerStatus) && props.replyApprovalDecisions.includes('rework')) {
    result = 'rework';
  }
  return result;
}
</script>

<template>
  <span style="vertical-align: text-bottom; cursor: pointer">
    <span v-tooltip.right="{ value: iconValue.tooltip  }"> <v-icon :inverse="iconValue?.inverse" :name="iconValue.name" :fill="iconValue.color" /></span>
  </span>
</template>
