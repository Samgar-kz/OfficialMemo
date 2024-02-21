<script setup lang="ts">
import { v4 as uuidv4 } from "uuid";
import Textarea from "primevue/textarea";

withDefaults(
  defineProps<{
    modelValue: any;
    autoResize: boolean;
    errorMessage?: string | string[];
    label?: string;
    requiredMark?: boolean;
    disabled?: boolean;
  }>(),
  {
    autoResize: false,
  }
);
const emit = defineEmits(["update:modelValue"]);

const id = uuidv4();
</script>

<template>
  <div class="field">
    <span class="p-float-label">
      <Textarea
        :id="id"
        type="text"
        :model-value="modelValue"
        @update:model-value="emit('update:modelValue', $event)"
        class="tw-w-full"
        :autoResize="autoResize"
        :disabled="disabled"
      />
      <label :for="id" :class="{ required: requiredMark }">{{ label }}</label>
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>
