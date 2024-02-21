<script setup lang="ts">
defineProps<{
  icon?: string;
  label?: string;
}>();
const emit = defineEmits(["click"]);
</script>
<template>
  <div
    @click="emit('click')"
    class="tw-flex tw-w-full tw-cursor-pointer tw-flex-row tw-items-center tw-gap-2 tw-px-4 tw-py-2 tw-text-base tw-font-medium tw-text-gray-500 tw-transition-colors hover:tw-bg-slate-100">
    <slot name="icon"> <i :class="icon" v-if="icon" /> </slot>
    <slot name="default"
      ><span>{{ label }}</span></slot
    >
  </div>
</template>
