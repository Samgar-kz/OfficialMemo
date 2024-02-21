<script setup lang="ts">
import type { DynamicDialogInstance } from "primevue/dynamicdialogoptions";
import { inject, onMounted, ref, type Ref } from "vue";

const dialogRef = inject<Ref<DynamicDialogInstance>>("dialogRef");
const message = ref("");
onMounted(() => {
  message.value = dialogRef.value.data?.message;
});
</script>

<template>
  <div>
    {{ message }}
  </div>
</template>
