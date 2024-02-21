<script setup lang="ts">
import { v4 as uuidv4 } from "uuid";
import InputText from "primevue/inputtext";

defineProps<{
  modelValue: any;
  label?: string;
  errorMessage?: string | string[] | undefined;
  requiredMark?: boolean;
  disabled?: boolean;
}>();
const emit = defineEmits(["update:modelValue", "blur", "input"]);

const id = uuidv4();
</script>

<template>
  <div class="field">
    <span class="p-float-label">
      <InputText
        :id="id"
        type="text"
        :model-value="modelValue"
        @update:model-value="emit('update:modelValue', $event)"
        @input="emit('input', $event)"
        @blur="emit('blur', $event)"
        class="tw-w-full"
        :disabled="disabled" />
      <label :for="id" :class="{ required: requiredMark }">{{ label }}</label>
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{ errorMessage }}</small>
  </div>
</template>
