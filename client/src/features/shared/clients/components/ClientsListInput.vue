<script setup lang="ts">
import type { Client } from "@/features/shared/clients/types/Client";
import { useField, type FormValidationResult, type ValidationOptions } from "vee-validate";
import { ref, toRef, watch, onBeforeUpdate } from "vue";

const props = defineProps<{
  modelValue: Client[] | undefined;
  label?: string;
  name: string;
  required?: boolean;
  readonly?: boolean;
}>();

const emit = defineEmits(["update:modelValue"]);

const nameRef = toRef(props, "name");
const labelRef = toRef(props, "label");
const { errorMessage, value: localModelValue } = useField<Client[]>(nameRef, undefined, {
  validateOnValueUpdate: true,
  label: labelRef,
  initialValue: toRef(props, "modelValue"),
  syncVModel: true,
});

watch(localModelValue, (value) => {
  emit("update:modelValue", value);
});

const clientInputRefs = ref<{ validate: (opts?: Partial<ValidationOptions>) => Promise<FormValidationResult<Client>> }[]>([] as any);

async function validate(): Promise<FormValidationResult<Client>> {
  if (!clientInputRefs.value?.length) return {} as any;
  const results = [] as FormValidationResult<Client>[];
  for (let i = 0; i < clientInputRefs.value.length; i++) {
    results.push(await clientInputRefs.value[i].validate());
  }
  const validationResult = results.reduce((prev, current) => {
    return { errors: { ...prev.errors, ...current.errors }, valid: prev.valid && current.valid, results: { ...prev.results, ...current.results } };
  });
  return validationResult;
}

defineExpose({ validate });

function addClient() {
  if (!localModelValue.value) localModelValue.value = [];
  localModelValue.value.push({} as any);
}
function removeClient(index: number) {
  if (index < 0 || index >= localModelValue.value.length) return;
  localModelValue.value.splice(index, 1);
}

function updateFormRef(index: number, el) {
  clientInputRefs.value[index] = el as any;
}

onBeforeUpdate(() => (clientInputRefs.value = [] as any));
</script>

<template>
  <div class="tw-flex tw-flex-col">
    <label class="tw-mb-1 tw-ml-3 tw-text-xs" :class="{ required }">{{ label }}</label>
    <div v-for="(client, index) in localModelValue" :key="index" class="tw-flex tw-flex-col">
      <client-input
        :readonly="readonly"
        :ref="(el) => updateFormRef(index, el)"
        :modelValue="localModelValue[index]"
        @update:modelValue="localModelValue[index] = $event.value"
        :clientKey="index"
        showClientTypeToggle>
        <template v-slot:toolbar>
          <Button label="Удалить" class="tw-w-fit tw-self-end" icon="pi pi-user-minus" @click="removeClient(index)" outlined severity="secondary" />
        </template>
      </client-input>
    </div>
    <Button label="Добавить" class="tw-mt-2 tw-w-fit" icon="pi pi-user-plus" @click="addClient" />
    <small v-if="errorMessage" class="p-error tw-ml-3 tw-mt-2">{{ errorMessage }}</small>
  </div>
</template>

<style>
.list-move,
.list-enter-active {
  transition: 1s ease all;
}
.list-leave-active {
  transition: 1s ease all;
  position: absolute;
}
.list-enter-from,
.list-leave-to {
  opacity: 0;
  transform: scale(0.6);
}

button.list-enter-active,
button.list-leave-active {
  transition: none;
}
</style>
