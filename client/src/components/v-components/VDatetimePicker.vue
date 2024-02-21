<script setup lang="ts">
import { inject, toRef, useAttrs } from "vue";
import { useField } from "vee-validate";
import { isRequired } from "@/types/utility/ValidationFieldsOf";

const props = defineProps<{
  modelValue: string | Date | string[] | Date[] | undefined;
  name: string;
  label?: string;
  minDate?: Date;
  maxDate?: Date;
}>();
defineEmits<{
  "update:modelValue": [value: string | Date | string[] | Date[] | undefined];
}>();

const attrs = useAttrs();
const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value } = useField<string | Date | string[] | Date[]>(
  nameRef,
  undefined,
  {
    validateOnValueUpdate: true,
    label: labelRef,
    syncVModel: true,
  }
);
const validationSchema = inject("validationSchema");
</script>

<template>
  <o-datetime-picker
    v-model="value"
    v-bind="attrs"
    :label="label"
    :errorMessage="errorMessage"
    :minDate="minDate"
    :max-date="maxDate"
    :required-mark="isRequired(validationSchema[name])"
  />
</template>
