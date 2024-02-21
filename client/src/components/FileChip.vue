<script setup lang="ts">
  const props = defineProps<{
    name: string;
    url?: string;
  }>();
  const emit = defineEmits(["delete"]);
</script>

<template>
  <!-- <div class="tw-inline-block tw-bg-[#ecf7f8] tw-rounded-full tw-text-xs tw-h-10 "> -->
  <a :href="url" target="blank" class="o-chip">
    <!-- <v-icon size="small" class="tw-mr-1 tw-ml-3 ">mdi-attachment</v-icon> -->
    <i class="pi pi-file" />
    <span class="o-chip-text">
      {{ props.name }}
    </span>
    <i class="pi pi-trash" @click.prevent="emit('delete')" />
    <!-- <v-btn icon="mdi-delete" size="small" variant="plain" @click.prevent="emit('delete')"></v-btn> -->
  </a>
  <!-- </div> -->
</template>

<style scoped>
.o-chip {
  @apply tw-no-underline tw-leading-4 tw-bg-[#ecf7f8] tw-rounded-lg tw-cursor-pointer tw-p-[0.5rem] tw-w-fit;
}
.o-chip .o-chip-text {
  @apply tw-py-[0.16em] tw-pl-[0.3em] tw-pr-[0.5em] hover:tw-underline tw-text-primary;
}
</style>
