<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import closeWindow from "@/server/closeWindow";
import type { TaskApproveModel } from "@/types/task/TaskApproveModel";
import type { MyDocument } from "@/types/contents/MyDocument";
import type { Task } from "@/types/task/Task";
import getTask from "@/server/api/bpm/task/getTask";
import Dialog from "primevue/dialog";
import ConfirmDialog from "primevue/confirmdialog";
import taskCancel from "@/server/api/bpm/task/taskCancel";
import { getActionIcon } from "@/server/api/handbooks/getActionIcons";
import type { Icon } from "@/server/api/handbooks/getActionIcons";

const props = defineProps<{
  taskGuid: string;
  modelValue?: boolean;
  label?: string;
}>();
defineEmits(["success", "update:modelValue", "yes", "no"]);

const model = ref<TaskApproveModel>({
  approvalDocuments: [] as MyDocument[],
} as TaskApproveModel);

type states = "none" | "canceled" | "error" | "loading";
const state = ref<states>("none");

function cancel() {
  if (state.value !== "none" && state.value !== "error") return;
  visible.value = false;
  state.value = "none";
}
async function cancelTask() {
  try {
    state.value = "loading";
    await taskCancel({ ...model.value, taskGuid: props.taskGuid });
    state.value = "canceled";
  } catch (error) {
    console.error(error);
    state.value = "error";
  }
}

const iconColor = ref<string>();

const icon = ref<Icon>({} as Icon);
onMounted(() => {
  icon.value = getActionIcon(props.label);
  iconColor.value = icon.value.color;
});

const task = ref<Task>({} as Task);
async function fetchData() {
  task.value = await getTask(props.taskGuid);
}
onMounted(() => fetchData());
watch(
  () => props.taskGuid,
  () => fetchData()
);

const visible = ref(false);
</script>

<template>
  <ConfirmDialog />
  <slot
    :click="
      () => {
        visible = true;
      }
    "
    :label="label"
  >
    <span
      class="hover-list"
      @click="
        () => {
          visible = true;
        }
      "
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
  </slot>

  <Dialog
    :header="
      state === 'error' ? 'Ошибка при отмене резолюции' : 'Отменить резолюцию'
    "
    v-model:visible="visible"
    modal
    class="sm:tw-w-full md:tw-w-[70%] lg:tw-w-[50%]"
  >
    <div class="tw-flex tw-flex-col tw-gap-4" v-if="state !== 'canceled'">
      <o-textarea
        label="Сообщение"
        v-model="model.approvalComment"
        auto-resize
      />
    </div>
    <template v-slot:footer>
      <Button
        v-show="state === 'none' || state === 'error'"
        @click="cancel"
        type="button"
        class="p-button p-button-secondary"
        >Отменить</Button
      >
      <Button
        v-show="state === 'canceled'"
        @click="closeWindow"
        class="p-button p-button-secondary"
        >Закрыть</Button
      >
      <Button
        v-show="state !== 'canceled'"
        @click="cancelTask"
        :loading="state === 'loading'"
        loadingIcon="pi pi-spinner pi-spin"
        class="p-button p-button-primary ф"
        type="button"
        >Отменить резолюцию</Button
      >
    </template>
  </Dialog>
</template>
