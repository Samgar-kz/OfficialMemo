<script setup lang="ts">
import { renderAsync } from "docx-preview";
import { ref, watch, nextTick } from "vue";
const props = defineProps<{ source: Blob | File }>();

const container = ref<HTMLElement>(null);

watch(
  () => props.source,
  async () => {
    nextTick(
      async () =>
        await renderAsync(props.source, container?.value, null, {
          className: "docx-container",
          breakPages: true,
          ignoreLastRenderedPageBreak: true,
          inWrapper: false,
        })
    );
  },
  { immediate: true }
);
</script>
<template>
  <div ref="container"></div>
</template>

<style>
.docx-container {
  margin-top: 0 !important;
  margin-bottom: 0 !important;
  box-shadow: none !important;
}
</style>
