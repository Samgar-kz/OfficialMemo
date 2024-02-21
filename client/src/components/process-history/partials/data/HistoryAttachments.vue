<script setup lang="ts">
import type { MyDocument } from "@/types/contents/MyDocument";
import OverlayFiles from "@/components/process-history/partials/overlays/OverlayFiles.vue";
import getDocumentKind from "@/features/documents/getDocumentKind";

const props = withDefaults(
  defineProps<{
    items: MyDocument[];
    threshold: number;
    wrap?: boolean;
    enableOverlay?: boolean;
    overlay?: boolean;
  }>(),
  { enableOverlay: true }
);
</script>

<template>
  <div
    class="attachments__wrapper_2 tw-text-primary"
    v-if="items && items?.length"
  >
    <div v-for="(attachment, i) in items" :key="i">
      <div v-show="i < 2 || overlay">
        <div class="attachment_item_2 tw-inline-flex tw-mt-0 tw-py-0">
          <slot>
            <span
              class="tw-ml-[-14px]"
              v-if="items.length - 2 > 0 && i === 0 && enableOverlay"
            >
              <overlay-files :items="items">
                <template #activator="{ click, activatorId }">
                  <span
                    :ref="activatorId"
                    @click="click"
                    class="tw-cursor-pointer hover:tw-text-secondary"
                  >
                    +{{ items.length - 2 }}
                  </span>
                </template>
              </overlay-files>
            </span>

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
            <quick-view
              :document="{
                ...attachment,
                kind: getDocumentKind(attachment.name),
              }"
            />
          </slot>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.attachments__wrapper_2 {
  /* display: flex; */
}

.attachments__wrapper_2:not(.flex-wrap) {
  flex-direction: column;
}

.attachments__wrapper_2 {
  /* flex-wrap: wrap; */
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
