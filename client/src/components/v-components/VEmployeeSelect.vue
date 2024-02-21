template
<script setup lang="ts">
import type { Employee } from "@/types/employees/Employee";
import { inject, toRef } from "vue";
import { useField } from "vee-validate";
import { isRequired } from "@/types/utility/ValidationFieldsOf";

const props = withDefaults(
  defineProps<{
    modelValue: Array<Employee> | Employee | string | undefined;
    responsible?: Employee | string;
    withResponsible?: boolean;
    name: string;
    label?: string;
    multiple?: boolean;
    readonly?: boolean;
    isOnlyEmployee?: boolean;
    unselectedEmployees?: Array<Employee> | undefined;
  }>(),
  { isOnlyEmployee: true }
);

const emit = defineEmits<{
  "update:modelValue": [value: Array<Employee> | Employee | string | undefined];
  "update:responsible": [value: Employee | string];
  isFocused: [value: true | false];
}>();

const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value } = useField<Array<Employee> | string>(
  nameRef,
  undefined,
  {
    validateOnValueUpdate: true,
    label: labelRef,
    syncVModel: true,
  }
);
const validationSchema = inject("validationSchema");
function consoleLog(item) {
  console.log(item);
}
</script>

<template>
  <OEmployeeSelect
    v-model="value"
    :responsible="responsible"
    @update:responsible="emit('update:responsible', $event)"
    :errorMessage="errorMessage"
    :label="label"
    :multiple="multiple"
    :required-mark="isRequired(validationSchema[name])"
    :readonly="readonly"
    :withResponsible="withResponsible"
    :isOnlyEmployee="isOnlyEmployee"
    :unselectedEmployees="unselectedEmployees"
    @isFocused="
      (child) => {
        emit('isFocused', child);
        consoleLog(child);
      }
    "
  />
</template>
