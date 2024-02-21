<script setup lang="ts">
withDefaults(
  defineProps<{
    header?: string;
    width?: number;
    headerColor?: Colors;
  }>(),
  { headerColor: "primary" }
);
</script>

<script lang="ts">
type Colors = "primary" | "secondary";
export type { Colors };
</script>

<template>
  <div
    class="t-flex t-flex-col tw-scroll-mb-10 tw-overflow-hidden tw-rounded tw-shadow-md"
  >
    <div
      class="card_block__header tw-flex tw-h-[60px] tw-flex-row tw-items-center tw-gap-6 tw-text-white"
      :class="[headerColor === 'primary' ? 'tw-bg-primary' : 'tw-bg-secondary']"
    >
      <slot name="header" :header="header">
        <div class="tw-px-4">{{ header }}</div>
      </slot>
    </div>
    <div class="card_block__body">
      <slot />
    </div>
  </div>
</template>

<style>
.card_block__body {
  @apply tw-bg-white;
}
.card_block__body > div > .p-card {
  border-radius: 0;
}
</style>
