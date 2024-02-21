<script setup lang="ts">
import type { Employee } from "@/types/employees/Employee";
import AutoComplete, {
  type AutoCompleteCompleteEvent,
} from "primevue/autocomplete";
import { v4 as uuidv4 } from "uuid";
import { nextTick, ref } from "vue";
import searchByName from "@/server/api/employees/searchByName";
import { useToast } from "primevue/usetoast";

const props = defineProps<{
  modelValue: Array<Employee> | Employee | string | undefined;
  responsible?: Employee | string;
  withResponsible?: boolean;
  label?: string;
  multiple?: boolean;
  readonly?: boolean;
  requiredMark?: boolean;
  errorMessage?: string;
  isOnlyEmployee?: boolean;
  unselectedEmployees?: Array<Employee> | undefined;
}>();
const emit = defineEmits<{
  "update:modelValue": [value: Array<Employee> | Employee | string | undefined];
  isFocused: [value: true | false];
}>();

const id = uuidv4();
const filteredEmployees = ref<Employee[]>();

async function searchEmployee(e: AutoCompleteCompleteEvent) {
  filteredEmployees.value = await searchByName(e.query, props.isOnlyEmployee);
}

const root = ref<HTMLElement>();

const updateModel = ref<any>();
function updateModelValue(event) {
  if (props.unselectedEmployees) {
    updateModel.value = event;
  } else update(event);
}
const toast = useToast();
function unselectEmployee(event) {
  if (props.unselectedEmployees) {
    if (
      props.unselectedEmployees?.find(
        (item) => item.login === event.value.login
      )
    ) {
      const errTxt = function () {
        if (props.label.includes("Согласующие"))
          return "согласующего, поскольку он/она уже согласовал(а) служебную записку!";
        else if (props.label.includes("Получатели"))
          return "исполнителя, поскольку он/она уже исполнил(а) служебную записку!";
      };
      toast.add({
        severity: "error",
        summary: "Ошибка",
        detail: "Вы не можете удалить этого " + errTxt(),
        life: 3000,
      });
    } else {
      update(updateModel.value);
    }
  }
}

function update(event) {
  emit("update:modelValue", event);
}

function selectEmployee(event) {
  if (props.unselectedEmployees) update(updateModel.value);
}
</script>

<template>
  <div class="field employee-select" ref="root">
    <span class="p-float-label">
      <AutoComplete
        :id="id"
        :modelValue="modelValue"
        @update:modelValue="updateModelValue($event)"
        :suggestions="filteredEmployees"
        @complete="searchEmployee($event)"
        @item-unselect="unselectEmployee($event)"
        @item-select="selectEmployee($event)"
        @focus="
          () => {
            emit('isFocused', true);
          }
        "
        class="tw-w-full"
        inputClass="tw-w-full"
        optionLabel="name"
        :optionValue="(v) => v"
        forceSelection
        :multiple="multiple"
        :minLength="3"
        :disabled="readonly"
        :pt="{ token: 'tw-gap-2', removeTokenIcon: 'tw-cursor-pointer' }"
      >
      </AutoComplete>
      <label :for="id" :class="{ required: requiredMark }">{{ label }}</label>
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>

<style lang="scss">
.employee-select
  .p-autocomplete-multiple-container
  .p-autocomplete-token:not(.resp) {
  @apply tw-bg-primary-100;
}
.employee-select .p-autocomplete-multiple-container .p-autocomplete-token.resp {
  @apply tw-bg-primary-200;
}
</style>
