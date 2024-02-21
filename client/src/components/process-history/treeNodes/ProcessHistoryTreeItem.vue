<script setup lang="ts">
import type { MyDocument } from "@/types/contents/MyDocument";
import type { ProcessMessage } from "@/types/processHistory/ProcessMessage";
import { onMounted, ref, watch, watchEffect } from "vue";

interface TreeItem {
  item: ProcessMessage;
  processGuid: string;
  isRoot: boolean;
  siblingsCount: number;
  itemsLength: number;
  level: number;
  parentMessage: string;
  isVisible: boolean;
  parentExecutor?: string;
  parentApprovalDecision?: string;
  parentApprovalComment?: string;
}
const props = defineProps<TreeItem>();

const emit = defineEmits([
  "update:selectedItem",
  "updatePrevious",
  "updateShow",
]);
const seen = ref<Boolean>();
const visibility = ref(false);
const appAndMessDocs = ref<MyDocument[]>([]);
const showAll = ref(true);
const isNewProcess = ref(false);
const stepName = ref("");

interface ChildrenInfo {
  Guid: string;
  visibleChildrenCount: number;
}

const childrenArr = ref<ChildrenInfo[]>([]);
const childrenInfo = ref<ChildrenInfo>({
  Guid:
    props.item.taskGuid ?? props.item.requestGuid ?? props.item?.messageGuid,
  visibleChildrenCount: 0,
});

watch(seen, () => {
  childrenInfo.value.visibleChildrenCount = 1;
  updatePrevious2(childrenInfo.value);
});

const fillChildrenArr = function (item: ProcessMessage) {
  item.children?.forEach((_) =>
    childrenArr.value.push({
      Guid: _.taskGuid ?? _.messageGuid,
      visibleChildrenCount: 1,
    })
  );
};

function updatePreviousFromEmit(step: {
  Guid: string;
  visibleChildrenCount: number;
}) {
  updatePrevious2(step);
}

function updatePrevious2(step: { Guid: string; visibleChildrenCount: number }) {
  let visibleChildCount = 1;
  if (props.item.children?.length > 0) {
    const child = childrenArr.value.find((_) => _.Guid === step.Guid);
    if (child) {
      child.visibleChildrenCount = step.visibleChildrenCount;
    }
  }

  childrenArr.value?.forEach(
    (item) => (visibleChildCount += item.visibleChildrenCount)
  );
  // console.log(visibleChildCount)
  // console.log(childrenArr.value)
  if (seen.value) childrenInfo.value.visibleChildrenCount = visibleChildCount;
  else if (props.itemsLength - 1 === props.level)
    childrenInfo.value.visibleChildrenCount = 1;
  else childrenInfo.value.visibleChildrenCount = 0;
  if (!props.isRoot) emit("updatePrevious", childrenInfo.value);
}

const fileCondition = ref(false);

watchEffect(() => {
  if (
    props.item.stepName
      .toLowerCase()
      ?.includes(props.item.regNum?.toLowerCase())
  ) {
    isNewProcess.value =
      props.processGuid && props.processGuid !== props.item.processGuid;
  }

  stepName.value = props.item.stepName;
  appAndMessDocs.value = [];
  appAndMessDocs.value
    .concat(props.item.messageDocuments ?? [])
    .concat(props.item.approvalDocuments ?? []);

  fileCondition.value =
    appAndMessDocs.value?.length > props.item.replyDocuments?.length
      ? appAndMessDocs.value?.length > 2
      : props.item.replyDocuments?.length > 2;
});

onMounted(() => {
  fillChildrenArr(props.item);
  updateVisibleChildCount(props.isVisible);
});

watchEffect(() => {
  visibility.value = props.isVisible;
});

function updateVisibleChildCount(value) {
  if (props.item.children) {
    seen.value = value;
    if (props.level != length - 1)
      childrenInfo.value.visibleChildrenCount =
          (childrenInfo.value.visibleChildrenCount ?? 0) + props.item.replyDocuments?.length;
  }
}

watch(visibility, (value) => {
  updateVisibleChildCount(value);
});

function updateShowing() {
  showAll.value = !showAll.value;
  if (props.itemsLength > 1) emit("updateShow", false);
  else emit("updateShow", true);
}
</script>

<template>
  <!--  -->
  <tr
    class="cs-tree-node tw-border-collapse tw-border tw-border-b tw-border-gray-500"
  >
    <td
      class="tw-pb-4"
      :style="{
        paddingLeft: item.children
          ? (siblingsCount * 28.7).toString() + 'px'
          : (siblingsCount !== 0 ? siblingsCount * 29.2 : 0).toString() + 'px',
      }"
    >
      <div v-if="level !== itemsLength - 1" class="tw-mt-1">
        <span
          class="tree-basis node-color"
          v-for="index in (childrenInfo.visibleChildrenCount < 2
            ? childrenInfo.visibleChildrenCount + 1
            : childrenInfo.visibleChildrenCount) +
          (level === itemsLength - 1 ? -1 : 0)"
          :key="index"
          :style="{
            marginTop:
              (
                index * 36 +
                (fileCondition || childrenInfo.visibleChildrenCount > 1
                  ? index > 13
                    ? 34 + index
                    : level === itemsLength - 1 &&
                      index === childrenInfo.visibleChildrenCount - 1
                    ? -12
                    : -24
                  : level === 0
                  ? -17
                  : -21)
              ).toString() + 'px',
            marginLeft: !isRoot ? '-3px' : '-4px',
            transform: index > 3 ? 'scale(1,' + Math.sqrt(index + 5) + ')' : '',
          }"
          :class="
            fileCondition || childrenInfo.visibleChildrenCount > 1
              ? 'long-scale'
              : 'standard-scale'
          "
          >|</span
        >
      </div>

      <span
        class="tree-down node-color tw-ml-[26.4px]"
        :style="{
          transform:
            'scale(1,' +
            (item.children?.length > 1 ? 1.3 : !fileCondition ? 1 : 1.3) +
            ')',
          marginTop:
            (
              24 + (item.children?.length > 1 ? 3 : !fileCondition ? -1 : 2)
            )?.toString() + 'px',
        }"
        v-if="item.children && seen"
        >|</span
      >
      <i
        v-if="isRoot && level === 0 ? showAll : true"
        class="pi pi-minus node-color"
        style="font-size: 20px"
        :style="{
          transform: (isRoot && level === 0 ? 'scale(0.5,' : 'scale(1,') + '1)',
          marginLeft: isRoot && level === 0 ? '-5px' : 0,
        }"
      ></i>

      <i
        v-if="!item.children && !(isRoot && level === 0)"
        class="pi pi-circle-fill ending-circle node-color"
      ></i>
      <button
        v-if="
          (item.children && item.children?.length) || (isRoot && level === 0)
        "
        class="p-treetable-toggler p-link node-color tw-cursor-pointer tw-pr-[16px] tw-text-base"
      >
        <i
          :class="'p-treetable-toggler-icon pi pi-chevron-right right-chevron rotator tw-absolute'"
          :style="{
            transform:
              'rotate(' +
              ((isRoot && level === 0 ? showAll : seen) ? 90 : 0).toString() +
              'deg)',
            marginLeft:
              (!(isRoot && level === 0 ? showAll : seen) ? -4 : 1).toString() +
              'px',
            marginTop:
              (!(isRoot && level === 0 ? showAll : seen)
                ? -17
                : -14
              ).toString() + 'px',
          }"
          @click="isRoot && level === 0 ? updateShowing() : (seen = !seen)"
        />
      </button>
      <message-info
        :item="item"
        :parent-approval-comment="parentApprovalComment"
        :parent-approval-decision="parentApprovalDecision"
      />
    </td>

    <td
    >
      <history-attachments
        v-if="appAndMessDocs"
        :threshold="7"
        class="custom-width-webkit-available tw-absolute tw-mb-0 tw-ml-1 tw-font-[10px]"
        :items="appAndMessDocs"
        :wrap="true"
      />
    </td>
    <td class="tw-pb-4">
      <overlay-process-step-block
        overlay-panel-class="cs-step-name"
        :step="item"
      >
        <template #activator="{ click, activatorId }">
          <span
            :ref="activatorId"
            @click="(event) => !isNewProcess && click(event)"
            class="tw-cursor-pointer hover:tw-text-secondary"
          >
            <process-step :item="item" :process-guid="processGuid" />
          </span>
        </template>
      </overlay-process-step-block>
    </td>

    <td class="tw-text-center">
      <overlay-process-step-block
        overlay-panel-class="cs-status-indicator"
        :step="item"
      >
        <template #activator="{ click, activatorId }">
          <status-indicator
            :ref="activatorId"
            :child-count="item.childCount"
            :reply-approval-decisions="[
              item.replyDecision,
              item.approvalDecision,
            ]"
            :status="item.messageStatus"
            @click="click"
          />
        </template>
      </overlay-process-step-block>
    </td>
    <td>
      <reply-info :item="item" />
    </td>

    <td>
      <history-attachments
        :threshold="20"
        v-if="item.replyDocuments"
        class="custom-width-webkit-available tw-absolute tw-mb-0 tw-ml-1 tw-font-[11px]"
        :items="item.replyDocuments"
        :wrap="true"
      />
    </td>
  </tr>
  <process-history-tree-item
    v-if="item.children && item.children?.length && seen"
    v-for="(child, index) in item.children"
    :key="index"
    :process-guid="processGuid"
    :item="child"
    :is-root="false"
    :siblings-count="siblingsCount + 1"
    :items-length="item.children?.length"
    :level="index"
    :parent-message="item.messageComment ?? undefined"
    @update-previous="updatePreviousFromEmit"
    :parent-executor="item.executorName"
    :parent-approval-comment="item.approvalComment ? item.approvalComment : ''"
    :parent-approval-decision="
      item.approvalDecision ? item.approvalDecision : ''
    "
    :is-visible="visibility"
  />
  <!--    v-model:selectedItem="selectedItemModel"-->
</template>
