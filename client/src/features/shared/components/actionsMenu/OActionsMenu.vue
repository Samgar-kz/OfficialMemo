<script setup lang="ts">
withDefaults(defineProps<{ header?: string }>(), { header: "Доступные действия" });
</script>
<template>
  <card-block header-color="secondary" :header="header">
    <div class="tw-flex tw-flex-col tw-pb-1">
      <slot name="default">
        <div class="tw-my-3 tw-w-full tw-text-center">
          <span class="tw-font-thin">Нет доступных действий</span>
        </div>
      </slot>
    </div>
  </card-block>
</template>
