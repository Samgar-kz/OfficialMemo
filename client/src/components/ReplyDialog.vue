<script setup lang="ts">
import { onMounted, ref } from "vue";
import YesNoDialog from "./YesNoDialog.vue";
import useYesNoDialog from "@/composables/useYesNoDialog";
import closeWindow from "@/server/closeWindow";
import type { PerformModel } from "@/types/PerformModel";
import type { ReplyDecision } from "@/types/Reply";
import { getActionIcon } from "@/server/api/handbooks/getActionIcons";
import type { Icon } from "@/server/api/handbooks/getActionIcons";
import { useVModel } from "@vueuse/core";

type PerformType = "request" | "task";
const props = withDefaults(
  defineProps<{
    guid: string;
    decision: ReplyDecision;
    decisionName: string;
    modelValue?: boolean;
    reply?: PerformModel;
    title: string;
    label: string;
    performType: PerformType;
    sendFunction: (model: PerformModel) => Promise<any>;
    width: number;
    meta?: string;
  }>(),
  {
    performType: "request",
    width: 500,
    reply: () => ({} as PerformModel),
  }
);

const iconColor = ref<string>();

const emit = defineEmits([
  "success",
  "update:modelValue",
  "update:reply",
  "yes",
  "no",
]);

const model = useVModel(props, "reply", emit, { passive: true });
const icon = ref<Icon>({} as Icon);

const ynDialog = useYesNoDialog({
  onYes: async () =>
    props.sendFunction
      ? await props.sendFunction({
          ...model.value,
          replyDecision: props.decision,
          replyDecisionName: props.decisionName ?? "",
          [`${props.performType}Guid`]: props.guid,
          guid: props.guid,
          meta: props.meta,
        })
      : await Promise.resolve(),
  onCloseWindow: closeWindow,
  closeOnSuccess: true,
});
onMounted(() => {
  icon.value = getActionIcon(props.label);
  iconColor.value = icon.value?.color;
});
</script>

<template>
  <yes-no-dialog
    :title="title"
    :label="label"
    :state="ynDialog.state.value"
    v-model="ynDialog.show.value"
    @yes="ynDialog.yes"
    @no="ynDialog.no"
    :is-loading="ynDialog.isLoading.value"
    :width="width"
  >
    <template v-slot:message>
      <o-textarea label="Сообщение" v-model="model.replyComment" auto-resize />
      <o-file-input v-model="model.replyDocuments" />
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
