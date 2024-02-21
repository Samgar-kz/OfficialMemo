<script setup lang="ts">
import { isRequired } from "@/types/utility/ValidationFieldsOf";
import { useField } from "vee-validate";
import { inject, toRef } from "vue";

const props = defineProps<{
  modelValue: number | undefined;
  name: string;
  label?: string;
  disabled?: boolean;
}>();
defineEmits<{ "update:modelValue": [value: number] }>();

const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value } = useField<number>(nameRef, undefined, {
  validateOnValueUpdate: true,
  label: labelRef,
  syncVModel: true,
});
const validationSchema = inject("validationSchema");
</script>

<template>
  <ONumberField
    v-model="value"
    :error-message="errorMessage"
    :label="label"
    :required-mark="isRequired(validationSchema[name])"
    :disabled="disabled"
  />
</template>
