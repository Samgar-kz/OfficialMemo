<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import closeWindow from "@/server/closeWindow";
import type { TaskApproveModel } from "@/types/task/TaskApproveModel";
import taskApprove from "@/server/api/bpm/task/taskApprove";
import taskRework from "@/server/api/bpm/task/taskRework";
import getAmIChildOfSubrequest from "@/server/api/bpm/task/getAmIChildOfSubrequest";
import type { MyDocument } from "@/types/contents/MyDocument";
import type { Task } from "@/types/task/Task";
import getTask from "@/server/api/bpm/task/getTask";
import Dialog from "primevue/dialog";
import ConfirmDialog from "primevue/confirmdialog";
import { getActionIcon } from "@/server/api/handbooks/getActionIcons";
import type { Icon } from "@/server/api/handbooks/getActionIcons";
import { officialMemoReceive } from "@/server/api/process/approve";

type ReplyDecision = "accept" | "approve" | "rework";
type ApproveType = "approve" | "rework";

const props = withDefaults(
  defineProps<{
    taskGuid: string;
    messageGuid?: string;
    decision: ReplyDecision;
    defaultPerformAfterApprove?: boolean;
    isPerform?: boolean;
    decisionName: string;
    modelValue?: boolean;
    title?: string;
    label?: string;
    approveType?: ApproveType;
    taskResults?: Task[];
  }>(),
  { approveType: "approve", defaultPerformAfterApprove: false }
);

const iconColor = ref<string>();

const icon = ref<Icon>({} as Icon);
onMounted(async () => {
  icon.value = getActionIcon(props.label);
  iconColor.value = icon.value.color;
  console.log(props.isPerform);
  // await fetchData();
});
const emit = defineEmits(["success", "update:modelValue", "yes", "no"]);

const model = ref<TaskApproveModel>({
  approvalDocuments: [] as MyDocument[],
  appendReplyToParent: true,
} as TaskApproveModel);

type states =
  | "none"
  | "approved"
  | "rejected"
  | "performed"
  | "error"
  | "loading";
const state = ref<states>("none");

async function approve() {
  if (props.approveType !== "approve") return;
  try {
    state.value = "loading";
    await taskApprove({ ...model.value, taskGuid: props.taskGuid });
    state.value = "approved";
    if (childSub.value && props.isPerform && model.value.appendReplyToParent) {
      await officialMemoReceive({
        guid: props.messageGuid,
        replyDecision: "accept",
        replyDecisionName: "Исполнено",
        replyComment: task.value.replyComment,
        replyDocuments: task.value.replyDocuments,
        employeeCode: task.value.initiator,
        executorCode: task.value.responsible,
        isRoot: false,
      });
    }

    emit("success");
  } catch (error) {
    console.error(error);
    state.value = "error";
  }
}

async function rework() {
  if (props.approveType !== "rework") return;
  try {
    state.value = "loading";
    await taskRework({ ...model.value, taskGuid: props.taskGuid });
    state.value = "rejected";
    emit("success");
  } catch (error) {
    console.error(error);
    state.value = "error";
  }
}

function cancel() {
  if (state.value !== "none" && state.value !== "error") return;
  visible.value = false;
  state.value = "none";
}

const task = ref<Task>({} as Task);
const showPerformAfterApprove = ref(false);
const childSub = ref(false);
async function fetchData() {
  task.value = await getTask(props.taskGuid);
  model.value.performAfterApprove = props.defaultPerformAfterApprove;

  childSub.value = await getAmIChildOfSubrequest(props.taskGuid);
  console.log(childSub.value);
  console.log(props.isPerform);

  showPerformAfterApprove.value = !(childSub.value && !props.isPerform);
  if (!showPerformAfterApprove.value) {
    model.value.performAfterApprove = false;
    model.value.appendReplyToParent = false;
  } else {
    console.log("asdasd");
  }
}
onMounted(() => fetchData());
watch(
  () => props.isPerform,
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
    :header="title"
    v-model:visible="visible"
    modal
    class="sm:tw-w-full md:tw-w-[70%] lg:tw-w-[50%]"
  >
    <div>
      <div class="tw-flex tw-flex-col tw-gap-4" v-if="state !== 'approved'">
        <o-textarea
          label="Сообщение"
          v-model="model.approvalComment"
          auto-resize
        />
        <o-file-input v-model="model.approvalDocuments" />
        <div v-if="props.approveType !== 'rework' && showPerformAfterApprove">
          <o-checkbox
            v-model="model.appendReplyToParent"
            label="Перенести отчет в свою КИ"
          />
          <o-checkbox
            v-model="model.performAfterApprove"
            label="Закрыть свою КИ"
          />
        </div>
      </div>
      <p v-else>Успешно согласовано</p>
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
        v-show="
          state === 'approved' || state === 'performed' || state === 'rejected'
        "
        @click="closeWindow"
        class="p-button p-button-secondary"
        >Закрыть</Button
      >

      <Button
        v-if="approveType === 'approve'"
        v-show="state !== 'approved'"
        @click="approve"
        :loading="state === 'loading'"
        loadingIcon="pi pi-spinner pi-spin"
        class="p-button p-button-primary"
        type="button"
        >Принять
      </Button>
      <Button
        v-if="approveType === 'rework'"
        v-show="state !== 'rejected'"
        @click="rework"
        :loading="state === 'loading'"
        loadingIcon="pi pi-spinner pi-spin"
        class="p-button p-button-primary"
        type="button"
        >Отправить на доработку</Button
      >
    </template>
  </Dialog>
</template>
