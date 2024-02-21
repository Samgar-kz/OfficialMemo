<script setup lang="ts">
import Calendar from "primevue/calendar";
import { v4 as uuidv4 } from "uuid";
import { useAttrs } from "vue";

defineProps<{
  modelValue: string | Date | string[] | Date[] | undefined;
  errorMessage?: string | string[];
  label?: string;
  requiredMark?: boolean;
  minDate?: Date;
  maxDate?: Date;
}>();
const emit = defineEmits<{
  "update:modelValue": [value: string | Date | string[] | Date[] | undefined];
}>();
const id = uuidv4();
const attrs = useAttrs();
</script>

<template>
  <div class="field">
    <span class="p-float-label">
      <Calendar
        :minDate="minDate"
        :max-date="maxDate"
        :panel-style="{ width: '300px' }"
        :id="id"
        :modelValue="modelValue"
        @update:modelValue="emit('update:modelValue', $event)"
        class="tw-w-full"
        v-bind="attrs"
      />
      <label :for="id" :class="{ required: requiredMark }">{{ label }}</label>
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>
