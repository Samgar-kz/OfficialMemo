<script setup lang="ts" generic="TOption, TValue">
import { isRequired } from "@/types/utility/ValidationFieldsOf";
import { useField } from "vee-validate";
import { inject, toRef } from "vue";

const props = defineProps<{
  modelValue: TValue;
  name: string;
  label?: string;
  options?: Array<TOption> | undefined;
  optionLabel?: InnerType;
  optionValue?: (string & keyof TOption) | ((obj: TOption) => TValue);
  optionDisabled?: (string & keyof TOption) | ((obj: TOption) => boolean);
  optionGroupLabel?: (string & keyof TOption) | ((obj: TOption) => string);
  optionGroupChildren?: keyof TOption | ((obj: TOption) => []);
  disabled?: boolean;
  editable?: boolean;
}>();
defineEmits<{ (e: "update:modelValue", value: TValue) }>();

const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
// eslint-disable-next-line vue/no-setup-props-destructure
const { errorMessage, value, handleChange } = useField<TValue>(
  nameRef,
  undefined,
  {
    validateOnValueUpdate: true,
    label: labelRef,
    initialValue: props.modelValue,
    syncVModel: true,
  }
);
const validationSchema = inject("validationSchema");
type InnerType = string & keyof TOption;
</script>

<template>
  <OSelect
    @update:model-value="handleChange"
    :model-value="value"
    :label="label"
    :errorMessage="errorMessage"
    :options="options"
    :optionLabel="(optionLabel as any)"
    :optionValue="(optionValue as any)"
    :optionDisabled="(optionDisabled as any)"
    :optionGroupLabel="(optionGroupLabel as any)"
    :optionGroupChildren="(optionGroupChildren as any)"
    :disabled="disabled"
    :editable="editable"
    :required-mark="isRequired(validationSchema[name])"
  />
</template>
