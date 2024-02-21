<script setup lang="ts">
import Checkbox from "primevue/checkbox";
import { v4 as uuidv4 } from "uuid";

withDefaults(
  defineProps<{
    modelValue: boolean;
    label?: string;
    requiredMark?: boolean;
    readonly?: boolean;
    disabled?: boolean;
  }>(),
  {
    modelValue: false,
    readonly: false,
  }
);
const emit = defineEmits(["update:modelValue"]);
const id = uuidv4();
</script>

<template>
  <div class="field-checkbox">
    <Checkbox
      :readonly="readonly"
      :disabled="disabled"
      :inputId="id"
      :model-value="modelValue"
      @update:model-value="emit('update:modelValue', $event)"
      :binary="true" />
    <label :for="id" class="tw-pl-2" :class="{ required: requiredMark }">{{ label }}</label>
  </div>
</template>
