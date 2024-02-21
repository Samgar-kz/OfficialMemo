<script setup lang="ts">
import type { MyDocument } from "@/types/contents/MyDocument";
import { v4 as uuidv4 } from "uuid";
import { inject, onMounted, toRef, watch } from "vue";
import { useField } from "vee-validate";
import { isRequired } from "@/types/utility/ValidationFieldsOf";

const props = withDefaults(
  defineProps<{
    modelValue: MyDocument[] | undefined;
    readonly?: boolean;
    name: string;
    label?: string;
    multiple?: boolean;
  }>(),
  {
    modelValue: [] as any,
    readonly: false,
    multiple: true,
  }
);

const emit = defineEmits(["update:modelValue"]);
const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value, setValue } = useField<MyDocument[]>(
  nameRef,
  undefined,
  {
    validateOnValueUpdate: true,
    label: labelRef,
    syncVModel: true,
  }
);
onMounted(() => setValue(props.modelValue));
const id = uuidv4();

const validationSchema = inject("validationSchema");

watch(value, (newValue) => emit("update:modelValue", newValue));
</script>

<template>
  <div>
    <o-file-input
      v-model="value"
      :readonly="readonly"
      :required-mark="isRequired(validationSchema[name])"
      :multiple="multiple"
    />
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>
