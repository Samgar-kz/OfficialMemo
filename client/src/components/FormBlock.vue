<script setup lang="ts">
import CardBlock from "@/components/CardBlock.vue";

defineProps<{ title?: string }>();
</script>

<template>
  <card-block :header-height="60">
    <template v-slot:header>
      <img src="@/assets/img/logo_inverted.svg" class="tw-h-[36px] tw-ml-4" />
      <div class="tw-w-full tw-text-white tw-text-base">{{ title }}</div>
    </template>
    <slot />
  </card-block>
</template>
