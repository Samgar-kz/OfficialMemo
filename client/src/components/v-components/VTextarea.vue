<script setup lang="ts">
import { isRequired } from "@/types/utility/ValidationFieldsOf";
import { useField } from "vee-validate";
import { inject, toRef } from "vue";

const props = withDefaults(
  defineProps<{
    modelValue: any;
    name: string;
    autoResize?: boolean;
    label?: string;
    disabled?: boolean;
  }>(),
  {
    autoResize: false,
    modelValue: "",
  }
);
defineEmits<{ "update:modelValue": [value: string] }>();

const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value } = useField<string>(nameRef, undefined, {
  validateOnValueUpdate: true,
  label: labelRef,
  syncVModel: true,
});
const validationSchema = inject("validationSchema");
</script>

<template>
  <OTextarea
    v-model="value"
    :label="label"
    :error-message="errorMessage"
    :autoResize="autoResize"
    :required-mark="isRequired(validationSchema[name])"
    :disabled="disabled"
  />
</template>
