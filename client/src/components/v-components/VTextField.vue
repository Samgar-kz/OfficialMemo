<script setup lang="ts">
import useValidationListeners from "@/composables/useValidationListeners";
import { isRequired } from "@/types/utility/ValidationFieldsOf";
import { useField } from "vee-validate";
import { inject, toRef } from "vue";

const props = defineProps<{
  modelValue: any;
  name: string;
  disabled?: boolean;
  label?: string;
}>();
defineEmits<{ "update:modelValue": [value: string] }>();

const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value, handleChange } = useField<string>(
  nameRef,
  undefined,
  {
    validateOnValueUpdate: false,
    label: labelRef,
    syncVModel: true,
  }
);
const validationListeners = useValidationListeners(errorMessage, handleChange);
const validationSchema = inject("validationSchema");
</script>

<template>
  <OTextField
    v-on="validationListeners"
    :model-value="value"
    :error-message="errorMessage"
    :label="label ?? name"
    :required-mark="isRequired(validationSchema[name])"
    :disabled="disabled"
  />
</template>
