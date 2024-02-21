<script setup lang="ts">
import type { MyDocument } from "@/types/contents/MyDocument";

const props = defineProps<{
  items: MyDocument[];
  threshold: number;
  wrap: Boolean;
}>();
</script>

<template>
  <div
    class="attachments__wrapper_2 tw-text-primary"
    :class="{ 'flex-wrap': wrap }"
    v-if="items && items?.length"
  >
    <div v-for="(attachment, i) in items" :key="i">
      <div class="attachment_item_2 tw-inline-flex tw-mt-0 tw-py-0">
        <slot>
          <a
            v-tooltip.left="{
              value: attachment.name,
              disabled: attachment.name?.length < threshold,
            }"
            :href="attachment.url"
            target="blank"
          >
            <i class="pi pi-file tw-mr-1 tw-align-top tw-text-sm" />
            <span
              class="hover:tw-underline tw-align-top custom-width-webkit-available tw-py-[1px]"
              >{{ attachment.name?.slice(0, threshold) }}</span
            >
          </a>
        </slot>
      </div>
    </div>
  </div>
</template>

<style scoped>
.attachments__wrapper_2 {
  display: flex;
}

.attachments__wrapper_2:not(.flex-wrap) {
  flex-direction: column;
}

.attachments__wrapper_2 {
  flex-wrap: wrap;
}

.attachments__wrapper_2 .attachment_item_2 {
  display: inline-block;
}

.attachments__wrapper_2 .attachment_item_2 {
  border-radius: 4px;
  background-color: #ecf7f8;
}

.attachments__wrapper_2 .attachment_item_2 a {
  text-decoration: none;
}
</style>
