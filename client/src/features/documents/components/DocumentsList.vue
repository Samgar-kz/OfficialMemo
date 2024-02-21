<script setup lang="ts">
import { computed } from "vue";
import type { MyDocument } from "@/types/contents/MyDocument";
import getDocumentKind from "@/features/documents/getDocumentKind";

const props = defineProps<{
  items: MyDocument[] | undefined;
  zipName?: string;
  wrap?: boolean;
}>();

const zipUrl = computed(() => {
  const ids = props.items.map((doc) => {
    return new URL(doc.url).searchParams.get("id");
  });
  if (!ids?.length) return "";
  const url = new URL(import.meta.env.VITE_CONTENTS_API_URL + "/asZip");
  for (let i = 0; i < ids.length; i++) {
    url.searchParams.append("ids", ids[i]);
  }
  if (props.zipName) url.searchParams.append("name", props.zipName);
  return url.toString();
});
</script>

<template>
  <div
    class="attachments__wrapper tw-text-primary"
    :class="{ 'flex-wrap': wrap }"
    v-if="items && items?.length"
  >
    <div v-for="(attachment, i) in items" :key="i">
      <div class="attachment_item">
        <slot>
          <a :href="attachment.url" target="blank">
            <i class="pi pi-file tw-mr-1" />
            <span class="tw-text-sm hover:tw-underline">{{
              attachment.name
            }}</span>
          </a>
          <quick-view
            :document="{
              ...attachment,
              kind: getDocumentKind(attachment.name),
            }"
          />
        </slot>
      </div>
    </div>
    <div v-if="items?.length > 1">
      <div class="attachment_item">
        <a :href="zipUrl" target="blank">
          <i class="pi pi-file tw-mr-1" />
          <span class="tw-text-sm hover:tw-underline">Скачать все</span>
        </a>
      </div>
    </div>
  </div>
</template>

<style scoped>
.attachments__wrapper {
  display: flex;
  font-size: 0.87em;
}
.attachments__wrapper:not(.flex-wrap) {
  flex-direction: column;
}
.attachments__wrapper {
  flex-wrap: wrap;
}
.attachments__wrapper .attachment_item {
  margin-right: 8px;
  margin-top: 4px;
  display: inline-block;
}
.attachments__wrapper .attachment_item {
  border-radius: 4px;
  background-color: #ecf7f8;
  padding: 0.16em 0.3em;
}
.attachments__wrapper .attachment_item a {
  text-decoration: none;
}
</style>
