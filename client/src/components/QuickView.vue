<script lang="ts" setup>
import { v4 as uuidv4 } from "uuid";
import { onMounted, ref } from "vue";
import type { Attachment } from "@/features/documents/types";
import DocumentViewer from "@/features/documents/components/DocumentViewer.vue";

import { useToast } from "primevue/usetoast";
import {
  getActionIcon,
  type Icon,
} from "@/server/api/handbooks/getActionIcons";

const visible = ref(false);
const id = uuidv4();

const toast = useToast();
const props = defineProps<{
  document?: Attachment;
}>();

const icon = ref<Icon>({} as Icon);
const iconColor = ref<string>();
onMounted(() => {
  icon.value = getActionIcon("Лупа");
  iconColor.value = icon.value?.color;
});

const show = () => {
  if (!visible.value) {
    toast.add({
      severity: "custom",
      group: "overlay_panel_" + id,
    });
    visible.value = true;
  }
};
</script>

<template>
  <Toast
    class="tw-min-w-fit tw-max-w-[297mm] tw-p-0 tw-opacity-100 tw-absolute"
    position="top-center"
    :group="'overlay_panel_' + id"
    @close="visible = false"
  >
    <template v-slot:message>
      <section
        class="tw-flex tw-w-full tw-bg-black-alpha-90 tw-shadow-2"
        style="border-radius: 10px"
      >
        <document-viewer :document="document" height-class="tw-max-h-[95vh]" />
      </section>
    </template>
  </Toast>
  <span
    class="tw-cursor-pointer tw-pl-1"
    @click="show"
    @mouseover="iconColor = icon.hoverColor"
    @mouseout="iconColor = icon.color"
  >
    <v-icon
      v-if="icon.name"
      :name="icon.name"
      :fill="iconColor"
      :animaton="icon.animation"
      :inverse="icon.inverse"
  /></span>
</template>

<style>
.p-toast-message-content {
  padding: 0 !important;
}
div[data-pc-section="buttoncontainer"] {
  position: fixed;
  right: -32px;
}
</style>
