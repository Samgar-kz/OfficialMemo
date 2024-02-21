<script setup lang="ts">
import { onMounted, ref } from "vue";
import YesNoDialog from "../YesNoDialog.vue";
import type { TaskRedirectModel } from "@/types/task/TaskRedirectModel";
import useYesNoDialog from "@/composables/useYesNoDialog";
import closeWindow from "@/server/closeWindow";
import taskRedirect from "@/server/api/bpm/task/taskRedirect";

import { getActionIcon } from "@/server/api/handbooks/getActionIcons";
import type { Icon } from "@/server/api/handbooks/getActionIcons";

const iconColor = ref<string>();

const icon = ref<Icon>({} as Icon);
onMounted(() => {
  icon.value = getActionIcon("Определить другого исполнителя");
  iconColor.value = icon.value?.color;
});

const props = defineProps({
  taskGuid: String,
  modelValue: {
    type: Boolean,
    default: false,
  },
  title: String,
  stepName: String,
  parentGuid: String,
  approvalRequired: Boolean,
});
defineEmits(["success", "update:modelValue", "yes", "no"]);

const model = ref<TaskRedirectModel>({} as TaskRedirectModel);

const ynDialog = useYesNoDialog({
  onYes: async () =>
    taskRedirect({
      ...model.value,
      taskGuid: props.taskGuid ?? "",
    }),
  onCloseWindow: closeWindow,
  closeOnSuccess: true,
});
</script>

<template>
  <yes-no-dialog
    title="Определить другого исполнителя"
    label="Определить другого исполнителя"
    :state="ynDialog.state.value"
    v-model="ynDialog.show.value"
    :width="500"
    @yes="ynDialog.yes"
    @no="ynDialog.no"
    :is-loading="ynDialog.isLoading.value">
    <template v-slot:message>
      <o-employee-select v-model="model.redirectTo" label="Исполнитель" class="tw-col-span-9 tw-w-full" />
      <o-textarea label="Сообщение" v-model="model.replyComment" auto-resize />
      <o-file-input v-model="model.replyDocuments" />
    </template>

    <template v-slot:default="{ click: click1, label }">
      <span>
        <slot :click="click1" :label="label">
          <span class="hover-list" @click="click1" @mouseover="iconColor = icon.hoverColor" @mouseout="iconColor = icon.color">
            <v-icon v-if="icon.name" :name="icon.name" :fill="iconColor" :animaton="icon.animation" :inverse="icon.inverse" />
            {{ label }}</span
          >
        </slot></span
      >
    </template>
  </yes-no-dialog>
</template>
