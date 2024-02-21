<script setup lang="ts" generic="TOption, TValue">
import Dropdown from "primevue/dropdown";
import { v4 as uuidv4 } from "uuid";

defineProps<{
  modelValue: any;
  errorMessage?: string | string[];
  label?: string;
  options?: Array<TOption> | undefined;
  optionLabel?: (string & keyof TOption) | ((obj: TOption) => string);
  optionValue?: (string & keyof TOption) | ((obj: TOption) => TValue);
  optionDisabled?: keyof TOption | ((obj: TOption) => boolean);
  optionGroupLabel?: keyof TOption | ((obj: TOption) => string);
  optionGroupChildren?: keyof TOption | ((obj: TOption) => []);
  requiredMark?: boolean;
  editable?: boolean;
}>();
const emit = defineEmits<{ "update:modelValue": [value: TValue] }>();
const id = uuidv4();
</script>

<template>
  <div :class="{ field: label }">
    <span class="p-float-label">
      <Dropdown
        :id="id"
        :modelValue="modelValue"
        @update:modelValue="emit('update:modelValue', $event)"
        :options="options"
        :optionLabel="(optionLabel as any)"
        :optionValue="(optionValue as any)"
        :optionDisabled="(optionDisabled as any)"
        :optionGroupLabel="(optionGroupLabel as any)"
        :optionGroupChildren="(optionGroupChildren as any)"
        class="tw-w-full"
        :editable="editable"
        ><template #option="slotProps">
          <div class="flex align-items-center">
            <span
              v-tooltip.right="{
                value: slotProps.option.name,
                pt: {
                  root: 'tw-max-w-[400px]',
                },
                autoHide: false,
              }"
              :style="{
                width: 'fit-content',
                'max-width': '32vw',
              }"
              class="text-wrap"
            >
              {{ slotProps.option.index }} - {{ slotProps.option.name }}
            </span>
          </div>
        </template></Dropdown
      >
      <label :for="id" :class="{ required: requiredMark }" v-if="label">{{
        label
      }}</label>
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>
