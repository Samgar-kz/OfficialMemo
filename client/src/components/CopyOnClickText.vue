<script setup lang="ts">
import { useTimeoutFn } from '@vueuse/shared';
import { ref } from 'vue';

const props = defineProps<{
  value: string
}>();
const copied = ref(false);

function copy(event: Event) {
  if (navigator.clipboard && window.isSecureContext) {
    navigator.clipboard.writeText(props.value);
  } else {
    let textArea = document.createElement("textarea");
    textArea.value = props.value;
    textArea.style.position = "absolute";
    textArea.style.opacity = "0";
    document.body.appendChild(textArea);
    textArea.focus();
    textArea.select();
    document.execCommand('copy');
    textArea.remove();
  }
  // navigator.clipboard.writeText(props.value);
  copied.value = true;
  setTimeout(() => {
    copied.value = false;
  }, 30000);
}
</script>


<template>
  <div class="tw-inline-flex tw-flex-row tw-align-middle tw-items-center">
    <span>{{ value }}</span>
    <Button class="tw-ml-2 tw-my-0 p-button-rounded p-button-text" :icon="copied ? 'pi pi-check-circle' : 'pi pi-copy'" @click="copy" size="x-small"></Button>
  </div>
</template>
