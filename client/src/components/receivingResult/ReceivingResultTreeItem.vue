<script setup lang="ts">
import type { MyDocument } from "@/types/contents/MyDocument";
import type { Employee } from "@/types/employees/Employee";
import type { ReceivingResult } from "@/types/process/ReceivingResult";
import { onMounted, ref, watch, watchEffect } from "vue";

interface TreeItem {
  item: ReceivingResult;
  messageGuid: string;
  isRoot: boolean;
  siblingsCount: number;
  itemsLength: number;
  level: number;
  isVisible: boolean;
  receiver: Employee;
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

interface ChildrenInfo {
  Guid: string;
  visibleChildrenCount: number;
}

const childrenArr = ref<ChildrenInfo[]>([]);
const childrenInfo = ref<ChildrenInfo>({
  Guid: props.messageGuid,
  visibleChildrenCount: 0,
});

watch(seen, () => {
  childrenInfo.value.visibleChildrenCount = 1;
  updatePrevious2(childrenInfo.value);
});

const fillChildrenArr = function (item: ReceivingResult) {
  item.children?.forEach((_) =>
    childrenArr.value.push({
      Guid: props.messageGuid,
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
  appAndMessDocs.value = [];
  appAndMessDocs.value.concat(props.item.documents ?? []);

  fileCondition.value =
    appAndMessDocs.value?.length > props.item.documents?.length
      ? appAndMessDocs.value?.length > 2
      : props.item.documents?.length > 2;
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
        (childrenInfo.value.visibleChildrenCount ?? 0) +
        props.item.documents?.length;
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

const disableTooltip = ref(false);
</script>

<template>
  <!--  -->
  <tr
    class="cs-tree-node tw-border-collapse tw-border tw-border-b tw-border-gray-500"
  >
    <td
      :style="{
        paddingLeft: item.children
          ? (siblingsCount * 28.7).toString() + 'px'
          : (siblingsCount !== 0 ? siblingsCount * 29.2 : 0).toString() + 'px',
        paddingBottom: (fileCondition ? 20 : 18).toString() + 'px',
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
                    : -12
                  : level === 0
                  ? -17
                  : -19)
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
        class="pi pi-minus node-color"
        style="font-size: 20px"
        :style="{
          transform: 'scale(1,1)',
          marginLeft: 0,
        }"
      ></i>

      <i
        v-if="!item.children"
        class="pi pi-circle-fill ending-circle node-color"
      ></i>
      <button
        v-if="item.children && item.children?.length"
        class="p-treetable-toggler p-link node-color tw-cursor-pointer tw-pr-[16px] tw-text-base"
      >
        <i
          :class="'p-treetable-toggler-icon pi pi-chevron-right right-chevron rotator tw-absolute'"
          :style="{
            transform: 'rotate(' + (seen ? 90 : 0).toString() + 'deg)',
            marginLeft: (!seen ? -4 : 1).toString() + 'px',
            marginTop: (!seen ? -17 : -14).toString() + 'px',
          }"
          @click="seen = !seen"
        />
      </button>
      <span class="tw-ml-[3px] tw-align-super">
        <overlay-executors-table
          v-on:close="disableTooltip = false"
          @click="disableTooltip = true"
          :executor-code="props.receiver.login"
          :executor-name="props.receiver.name"
          :user-name="props.receiver.name"
        />
      </span>
    </td>
    <td>
      {{ item.created ? new Date(item.created).toLocaleString("kk-KZ") : "" }}
    </td>

    <!-- <td>
      <history-attachments
        v-if="appAndMessDocs"
        :threshold="7"
        class="custom-width-webkit-available tw-absolute tw-mb-0 tw-ml-1 tw-font-[10px]"
        :items="appAndMessDocs"
        :wrap="true"
      />
    </td> -->
    <td class="tw-pb-4">
      {{ item.result === "accept" ? "Исполнено" : "" }}
    </td>
    <td>
      {{ item.comment }}
      <!-- <reply-info :item="item" /> -->
    </td>

    <td>
      <history-attachments
        :threshold="20"
        v-if="item.documents"
        class="custom-width-webkit-available tw-mb-0 tw-ml-1 tw-font-[11px]"
        :items="item.documents"
        :wrap="true"
      />
    </td>
  </tr>
  <receiving-result-tree-item
    v-if="item.children && item.children?.length && seen"
    v-for="(child, index) in item.children"
    :key="index"
    :message-guid="messageGuid"
    :item="child"
    :receiver="child.executor"
    :is-root="false"
    :siblings-count="siblingsCount + 1"
    :items-length="item.children?.length"
    :level="index"
    :parent-message="item.comment ?? undefined"
    @update-previous="updatePreviousFromEmit"
    :is-visible="visibility"
  />
  <!--    v-model:selectedItem="selectedItemModel"-->
</template>
