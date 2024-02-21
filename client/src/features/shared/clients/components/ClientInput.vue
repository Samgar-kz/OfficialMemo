<script setup lang="ts">
import type ValidationFieldsOf from "@/types/utility/ValidationFieldsOf";
import { useForm } from "vee-validate";
import { ref, reactive, computed, watch, onMounted } from "vue";
import type { Client, EntityClient } from "../types/Client";
import { searchClientsByName } from "../api/searchClientsByName";
import type { AutoCompleteCompleteEvent } from "primevue/autocomplete";
import getClientTypes from "@/server/api/handbooks/getClientTypes";
import type { LabelValue } from "@/types/outgoing/LabelValue";
import type { ClientType } from "@/features/inquiries/types";

const props = defineProps<{
  modelValue: Client | EntityClient | undefined;
  errorMessage?: string;
  required?: boolean;
  showClientTypeToggle?: boolean;
  clientKey: any;
  readonly?: boolean;
  disabled?: boolean;
  defaultClientType?: ClientType;
}>();
const emit = defineEmits(["update:modelValue", "cancel"]);

const localModelValue = ref<Client & EntityClient>({} as Client);
watch(
  () => props.modelValue,
  (newValue) => {
    if (typeof newValue !== "object") return;
    localModelValue.value = props.modelValue ?? ({} as Client);
  },
  { immediate: true }
);
watch(
  localModelValue,
  (value) => {
    emit("update:modelValue", { value, key: props.clientKey });
  },
  { deep: true }
);

const validationSchema: ValidationFieldsOf<Client> = reactive({
  name: "required",
  identityNumber: computed(() => (localModelValue.value?.clientType === "physic" ? "required" : "") + "|numeric|min:12|max:12"),
  phone: "phone",
  email: "email",
});
const { validate } = useForm({ validationSchema: validationSchema, keepValuesOnUnmount: true, initialValues: toRef(props, "modelValue") });
defineExpose({ validate });

const suggestions = ref<(Client | EntityClient)[]>();
async function search(e: AutoCompleteCompleteEvent) {
  suggestions.value = await searchClientsByName(e.query, localModelValue.value.clientType);
}
function updateClient(client: Client | EntityClient | string) {
  if (typeof client === "object") localModelValue.value = client;
  else if (typeof client === "string") localModelValue.value.name = client;
}

const clientTypes = ref<LabelValue[]>([]);
onMounted(async () => {
  clientTypes.value = await getClientTypes();
  localModelValue.value.clientType = props.defaultClientType ?? props.modelValue?.clientType ?? (clientTypes.value[0].value as ClientType);
});
</script>
<template>
  <v-form :schema="validationSchema" ref="clientInputForm" class="tw-grid tw-w-full tw-grid-cols-2 tw-gap-x-4 tw-gap-y-1 tw-rounded-md tw-py-2">
    <div class="tw-col-span-full tw-flex tw-flex-row tw-items-center tw-gap-2">
      <o-toggle-button
        :items="clientTypes"
        name="clientType"
        v-model="localModelValue.clientType"
        dataKey="value"
        :item-value="(v) => v.value"
        unselectable
        class="tw-self-start"
        v-if="showClientTypeToggle"
        :disabled="disabled" />
      <div class="tw-flex tw-flex-grow tw-flex-row tw-justify-end tw-gap-2"><slot name="toolbar"></slot></div>
    </div>
    <v-auto-complete
      :model-value="localModelValue.name"
      @update:model-value="updateClient"
      :suggestions="suggestions"
      @complete="search"
      :label="localModelValue.clientType === 'physic' ? 'Имя' : 'Наименование'"
      name="name"
      :disabled="disabled">
      <template v-slot:option="{ option }">
        <client-chip :model="option" />
      </template>
    </v-auto-complete>
    <v-text-field
      v-model="localModelValue.identityNumber"
      :label="localModelValue.clientType === 'physic' ? 'ИИН' : 'БИН'"
      name="identityNumber"
      :disabled="disabled" />
    <v-input-mask v-model="localModelValue.phone" label="Телефон" name="phone" mask="+7 (999) 999 9999" slot-char="_" :disabled="disabled" />
    <v-text-field v-model="localModelValue.email" label="Email" name="email" :disabled="disabled" />
    <v-text-field
      v-model="(localModelValue as EntityClient).email2"
      label="Допольнительный Email"
      name="email2"
      :disabled="disabled"
      v-if="localModelValue.clientType === 'entity'" />
    <o-checkbox
      :readonly="readonly"
      :disabled="disabled"
      label="Подключен к ЕСЭДО"
      v-model="localModelValue.hasEsedo"
      v-if="localModelValue.clientType === 'entity'"
      class="tw-mt-6 tw-self-center" />
    <v-text-field v-model="localModelValue.address" label="Адрес" name="address" class="tw-col-span-full" :disabled="disabled" />
    <!-- <small v-if="errorMessage" class="p-error tw-ml-3">{{ errorMessage }}</small> -->
  </v-form>
</template>
