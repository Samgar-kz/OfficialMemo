<script lang="ts" setup>
import { ref } from "vue";
import OverlayPanel from "primevue/overlaypanel";
import { v4 as uuidv4 } from "uuid";
const props = defineProps<{
  title?: string;
  label?: string;
  breakpoints?: {};
  style?: string;
}>();
const panel = ref();
const id = uuidv4();

const emit = defineEmits(["toggle"]);

const toggle = async (event) => {
  emit("toggle", event);
  panel.value.toggle(event);
};

const hide = async () => {
  panel.value.hide();
};
</script>

<template>
  <slot :click="toggle" :hide="hide" :label="label">
    <Button @click="toggle">{{ label }}</Button>
  </slot>
  <!-- </div> -->
  <overlay-panel
    :showCloseIcon="true"
    :style="style"
    :id="'overlay_panel' + id"
    ref="panel"
    append-to="body"
    :header="title"
    :breakpoints="{}"
  >
    <slot name="message"></slot>
  </overlay-panel>
</template>

<style>
.p-overlaypanel-content {
  padding: 15px 0px 0px !important;
}
</style>
