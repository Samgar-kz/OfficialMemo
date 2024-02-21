<script setup lang="ts">
import { onMounted, ref, watchEffect } from "vue";
import YesNoDialog from "../YesNoDialog.vue";
import type { TaskCreateModel } from "@/types/task/TaskCreateModel";
import useYesNoDialog from "@/composables/useYesNoDialog";
import closeWindow from "@/server/closeWindow";
import taskCreate from "@/server/api/bpm/task/taskCreate";
import type { MyDocument } from "@/types/contents/MyDocument";
import { type FormOptions, useForm } from "vee-validate";
import type ValidationFieldsOf from "@/types/utility/ValidationFieldsOf";
import type { Employee } from "@/types/employees/Employee";
import { getActionIcon } from "@/server/api/handbooks/getActionIcons";
import type { Icon } from "@/server/api/handbooks/getActionIcons";
import GroupList from "../GroupList.vue";

const props = withDefaults(
  defineProps<{
    requestGuid: string;
    modelValue?: boolean;
    title?: string;
    stepName: string;
    parentGuid: string;
    approvalRequired: boolean;
    label?: string;
    defaultDueToDate?: Date;
    defaultExecutor?: Employee;
  }>(),
  {
    title: "Создать резолюцию",
    modelValue: false,
    label: "Создать резолюцию",
  }
);
defineEmits(["success", "update:modelValue", "yes", "no"]);

const iconColor = ref<string>();
const isFocused = ref<boolean>(false);
const icon = ref<Icon>({} as Icon);
onMounted(() => {
  icon.value = getActionIcon(props.label ?? "Создать резолюцию");
  iconColor.value = icon.value?.color;
});
const model = ref<TaskCreateModel>({
  requestGuid: props.requestGuid,
  documents: [] as MyDocument[],
} as TaskCreateModel);
const validationSchema: ValidationFieldsOf<TaskCreateModel> = {
  executors: "required",
  dueToDate: "required",
  // dueToDate: "required|notBeforeTodayInclusive",
};

const { validate } = useForm({
  validationSchema: validationSchema,
  validateOnMount: false,
  initialErrors: undefined,
  initialValues: { executors: null },
} as FormOptions<TaskCreateModel>);
const ynDialog = useYesNoDialog({
  onYes: async () => {
    await taskCreate({
      ...model.value,
      stepName: props.stepName,
      parentGuid: props.parentGuid,
      approvalRequired: props.approvalRequired,
      approverSelf: props.approvalRequired,
    });
  },
  onCloseWindow: closeWindow,
  closeOnSuccess: false,
  onValidate: validate,
});

watchEffect(() => {
  if (props.defaultDueToDate) {
    model.value.dueToDate = new Date(props.defaultDueToDate);
  }
});

function consoleLog(item) {
  console.log(item);
}
</script>

<template>
  <yes-no-dialog
    :title="title"
    :label="label"
    :state="ynDialog.state.value"
    v-model="ynDialog.show.value"
    :width="500"
    @yes="ynDialog.yes"
    @no="ynDialog.no"
    :is-loading="ynDialog.isLoading.value"
    @close="closeWindow"
  >
    <template v-slot:message>
      <group-list
        :executors="model.executors"
        @update-executors="(child) => (model.executors = child)"
        :isFocused="isFocused"
      />
      <v-form :schema="validationSchema" class="tw-flex tw-flex-col">
        <v-employee-select
          name="executors"
          v-model="model.executors"
          label="Исполнители"
          class="tw-col-span-9 tw-w-full"
          multiple
          @isFocused="
            (child) => {
              isFocused = child;
            }
          "
        />
        <v-datetime-picker
          name="dueToDate"
          label="Срок исполнения"
          v-model="model.dueToDate"
          :min-date="new Date()"
          :max-date="model.dueToDate"
          class="tw-col-span-4"
        />
        <v-textarea
          name="taskComment"
          label="Сообщение"
          v-model="model.taskComment"
          auto-resize
        />
        <v-file-input name="documents" v-model="model.documents" />
      </v-form>
    </template>
    <template v-slot:default="{ click: click1, label }">
      <span>
        <slot :click="click1" :label="label">
          <span
            class="hover-list"
            @click="click1"
            @mouseover="iconColor = icon.hoverColor"
            @mouseout="iconColor = icon.color"
          >
            <v-icon
              v-if="icon.name"
              :name="icon.name"
              :fill="iconColor"
              :animaton="icon.animation"
              :inverse="icon.inverse"
            />
            {{ label }}</span
          >
        </slot></span
      >
    </template>
  </yes-no-dialog>
</template>
