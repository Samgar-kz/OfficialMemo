<script setup lang="ts">
import { useVModel } from "@vueuse/core";
import useContents from "@/composables/useContents";
import type { MyDocument } from "@/types/contents/MyDocument";
import Button from "primevue/button";
import { v4 as uuidv4 } from "uuid";
import { watch, watchEffect } from "vue";
import { useToast } from "primevue/usetoast";

const props = withDefaults(
  defineProps<{
    modelValue: MyDocument[] | undefined;
    readonly?: boolean;
    multiple?: boolean;
  }>(),
  { modelValue: () => [], readonly: false, multiple: true }
);
const emit = defineEmits(["update:modelValue"]);
const files = useVModel(props, "modelValue", emit);
const { open, removeFile, errorMessage } = useContents(files);
const toast = useToast();

watchEffect(() => {
  if (errorMessage.value) {
    toast.add({
      severity: "error",
      summary: "Ошибка",
      detail: errorMessage.value,
      life: 3000,
    });
    errorMessage.value = null;
  }
});
const id = uuidv4();
</script>

<template>
  <div class="tw-flex tw-h-auto tw-flex-wrap tw-gap-2 tw-pt-[1rem]" :id="id">
    <file-chip
      v-for="(file, index) in files"
      :key="index"
      :name="file.name"
      :url="file.url"
      @delete="removeFile(file)"
      :readonly="readonly"
    ></file-chip>
    <Button
      icon="pi pi-plus"
      label="Добавить файлы"
      class="p-button-outlined p-button-sm tw-h-[32px] tw-text-primary"
      @click="open({ multiple: multiple })"
      v-if="multiple || !modelValue"
    />
  </div>
</template>
