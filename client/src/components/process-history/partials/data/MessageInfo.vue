<script setup lang="ts">
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import { ref, computed, onMounted } from "vue";
const messageTooltip = ref();
let messageComment = ref("");
const disableTooltip = ref(false);
// Define props
const props = defineProps<{
  item: ProcessMessage;
  parentApprovalDecision?: string;
  parentApprovalComment?: string;
}>();

function formatMessageComment(item) {
  const sendComment = !item.messageComment?.includes(item.stepName)
    ? item.messageComment ?? ""
    : "";
  return `${new Date(item.messageDate)
    .toLocaleString("kk-KZ")
    .replace(/(.*)\D\d+/, "$1")} ${
    props.parentApprovalDecision === "reject"
      ? props.parentApprovalComment ?? sendComment
      : sendComment
  }`;
}
onMounted(() => {
  messageTooltip.value = formatTooltip(props.item);

  const formattedComment = formatMessageComment(props.item);
  messageComment.value = formattedComment.substring(0, 82);
});
function formatTooltip(item) {
  let tooltip = "";
  tooltip += `Исполнил(а): ${item.userName}\n`;
  if (item.userName !== item.executorName) {
    tooltip += `От имени: ${item.executorName}\n`;
  }
  tooltip += `Дата и время ${
    item.messageStatus === "canceled" ? "отмены: " : "исполнения: "
  }${new Date(item.replyDate).toLocaleString("kk-KZ")}\n`;
  tooltip += item.replyComment
    ? `Текст ${
        item.messageStatus === "canceled" ? "отмены: " : "исполнения: "
      }${item.replyComment}\n`
    : "";
  return tooltip;
}
</script>

<template>
  <span
    class="tw-ml-[3px] tw-align-super"
    v-tooltip.right="{
      value: messageTooltip,
      disabled: disableTooltip,
    }"
  >
    <span
      v-if="item.approverName?.length > 0 || item.initiatorName?.length > 0"
    >
      {{
        (() => {
          const longestName =
            item.approverName.length > 0
              ? item.approverName
              : item.initiatorName;
          return longestName?.substring(0, longestName?.lastIndexOf(" "));
        })()
      }}
      <span class="tw-pb-[0px] tw-pl-[4px] tw-pr-[17px] tw-pt-[0px]"
        ><span class="arrow-right">&#10141;</span>&nbsp;</span
      ></span
    >
    <overlay-executors-table
      v-on:close="disableTooltip = false"
      @click="disableTooltip = true"
      :executor-code="item.executorCode"
      :executor-name="item.executorName"
      :user-name="item.userName"
    >
    </overlay-executors-table>
    <span>
      {{
        item.dueToDate
          ? ", Срок: " + new Date(item.dueToDate).toLocaleDateString("kk-KZ")
          : ""
      }}
    </span>
    <span class="message-comment">{{ messageComment }}</span>
  </span>
</template>
