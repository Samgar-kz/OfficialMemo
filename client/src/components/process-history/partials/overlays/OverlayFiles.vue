<script lang="ts" setup>
import { v4 as uuidv4 } from "uuid";
import { ref } from "vue";
import OverlayPanel from "primevue/overlaypanel";
import HistoryAttachments from "@/components/process-history/partials/data/HistoryAttachments.vue";
import type { MyDocument } from "@/types/contents/MyDocument";

const panel = ref();
const id = uuidv4();
const activatorId = uuidv4();
const props = defineProps<{
  items: MyDocument[];
}>();
const emit = defineEmits(["toggle", "onItemSelect", "close"]);
const toggle = async (event) => {
  emit("toggle", event);
  panel.value.toggle(event);
};

const close = () => {
  emit("close");
};
</script>

<template>
  <slot
    name="activator"
    :click="toggle"
    :activatorId="'activator-' + activatorId"
  >
    <Button type="button" @click="toggle" />
  </slot>
  <overlay-panel
    @hide="close"
    class="tw-center tw-font-semibold process-overlay tw-min-1000 cs-overlay-panel-relative-position"
    ref="panel"
    appendTo="body"
    :showCloseIcon="true"
    :id="'overlay_panel_' + id"
  >
    <slot>
      <history-attachments
        class="tw-p-4"
        :items="items"
        :enable-overlay="false"
        :threshold="1000"
        :overlay="true"
      />
    </slot>
  </overlay-panel>
</template>

<style>
div.process-overlay > div.p-overlaypanel-content {
  padding: 0px !important;
}

.cs-status-indicator.cs-overlay-panel-relative-position {
  right: 120px !important;
  left: auto !important;
  word-break: break-word;
  white-space: pre-line;
}
.cs-overlay-panel-relative-position::before {
}
.cs-overlay-panel-relative-position::after {
}
.cs-step-name.cs-overlay-panel-relative-position {
  left: 240px !important;
  word-break: break-word;
  white-space: pre-line;
}
.cs-overlay-panel-relative-position {
  max-width: 1050px;
}
</style>
