<template>
  <span
    v-tooltip.left="{ value: replyTooltip }"
    class="tw-ml-2 tw-align-super"
    v-if="item.replyDate"
  >
    <span v-if="item.userName !== item.executorName">
      {{
        item.userName?.substring(0, item.userName?.lastIndexOf(" ")) +
        " Дата: " +
        (item.replyDate
          ? new Date(item.replyDate)
              .toLocaleString("kk-KZ")
              .replace(/(.*)\D\d+/, "$1")
          : ""
        ).toString()
      }}
      <span class="execute-comment tw-ml-2">
        &nbsp;{{ item.replyComment?.substring(0, 50) }}</span
      >
    </span>
    <span v-else>
      <span>
        &nbsp;{{
          "Дата: " +
            (item.replyDate
              ? new Date(item.replyDate)
                  .toLocaleString("kk-KZ")
                  .replace(/(.*)\D\d+/, "$1")
              : "") ?? ""
        }}</span
      >
      <span class="execute-comment tw-ml-2">{{
        item.replyComment?.substring(0, 50)
      }}</span>
    </span>
  </span>
</template>

<script setup lang="ts">
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import { computed, onMounted, ref } from "vue";
const replyTooltip = ref();
// Define props
const props = defineProps<{
  item: ProcessMessage; // Assuming you have a type for process messages
}>();

// Define computed property to format reply information
const showReplyInfo = computed(() => {
  return props.item.replyDate && props.item.replyComment;
});

const formattedReplyInfo = computed(() => {
  if (showReplyInfo.value) {
    const replyDate = new Date(props.item.replyDate)
      .toLocaleString("kk-KZ")
      .replace(/(.*)\D\d+/, "$1");
    const replyComment = props.item.replyComment.substring(0, 50);
    return `Исполнил(а): ${props.item.userName}\nДата: ${replyDate}\nКомментарий: ${replyComment}`;
  } else {
    return "";
  }
});

onMounted(() => (replyTooltip.value = formatTooltip(props.item)));
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

<!-- Optional styles scoped to this component -->
<style scoped>
.reply-info {
  /* Add your component-specific styles here */
}
</style>
