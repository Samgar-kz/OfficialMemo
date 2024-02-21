<script setup lang="ts">
defineProps<{
  title: string;
  value?: string;
}>();
</script>

<template>
  <slot />
</template>
