<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import OverlayPanelDialog from "../OverlayPanelDialog.vue";
import type {
  TaskUpdateModel,
  updateObject,
} from "@/types/task/TaskUpdateModel";
import taskUpdate from "@/server/api/bpm/task/taskUpdate";
import type ValidationFieldsOf from "@/types/utility/ValidationFieldsOf";
import type { Employee } from "@/types/employees/Employee";
const props = withDefaults(
  defineProps<{
    taskGuid: string;
    updateObject?: updateObject;
    modelValue?: boolean;
    title?: string;
    defaultDueToDate?: Date;
  }>(),
  {
    title: "Изменить резолюцию",
    modelValue: false,
  }
);
const emit = defineEmits([
  "success",
  "update:modelValue",
  "yes",
  "no",
  "updateTask",
]);
const executor = ref<Employee>();
const dueToDate = ref<Date>();
const model = ref<TaskUpdateModel>({} as TaskUpdateModel);
const validationSchema_dueToDate: ValidationFieldsOf<TaskUpdateModel> = {
  dueToDate: "required|notBeforeTodayInclusive",
};
const validationSchema_executor: ValidationFieldsOf<TaskUpdateModel> = {
  executor: "required",
};

watch(dueToDate, (newValue) => {
  model.value.dueToDate = newValue;
  updateTask();
});
watch(executor, (newExecutor) => {
  model.value.executor = newExecutor?.login;
  model.value.executor?.length > 0 && updateTask();
});
onMounted(
  () =>
    (model.value.dueToDate = props.defaultDueToDate
      ? props.defaultDueToDate
      : new Date())
);

const overlayPanel = ref();
const updateTask = () => {
  overlayPanel.value.click();
  props.taskGuid &&
    taskUpdate({
      ...model.value,
      taskGuid: props.taskGuid ?? "",
      executor: executor?.value?.login,
      updateObject: props.updateObject,
    }).then(() => {
      emit("updateTask");
    });
};
</script>

<template>
  <overlay-panel-dialog
    :title="title"
    label="Изменить"
    :breakpoints="{ '460px': '75vw' }"
  >
    <template #default="{ click: click1 }">
      <span
        ref="overlayPanel"
        class="tw-ml-2 tw-cursor-pointer tw-underline"
        style="color: var(--secondary-color)"
        @click="click1"
        >{{ title }}</span
      >
    </template>
    <template v-slot:message>
      <v-form
        :schema="validationSchema_dueToDate"
        class="tw-flex tw-flex-col"
        v-if="updateObject === 'DueToDate'"
      >
        <v-datetime-picker
          name="dueToDate"
          label="Срок исполнения"
          v-model="dueToDate"
          :min-date="new Date()"
          class="tw-col-span-4"
        />
      </v-form>
      <v-form
        :schema="validationSchema_executor"
        class="tw-flex tw-flex-col"
        v-if="updateObject === 'Executor'"
      >
        <v-employee-select
          name="executor"
          v-model="executor"
          label="Новый исполнитель"
          class="tw-col-span-9 tw-w-full"
        />
      </v-form>
    </template>
  </overlay-panel-dialog>
</template>
