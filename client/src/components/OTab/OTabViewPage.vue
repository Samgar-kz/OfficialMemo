<script setup lang="ts">
import type OTabItem from "./OTabItem.vue";
import {
  onBeforeMount,
  onMounted,
  reactive,
  useSlots,
  watch,
  type RendererNode,
  type VNode,
  ref,
} from "vue";
const children = ref<typeof OTabItem>();
const tabRoot = ref();
const selectedTabIndex = ref(-1);
onMounted(() => {
  children.value = tabRoot.value.children;
  SetOldSelectedTabIndexValue();
});
interface TabProps {
  title: string;
}
const props = defineProps({
  processguid: String,
});
const state = reactive({
  selectedIndex: 0,
  tabs: [] as VNode<RendererNode>[],
  count: 0,
});
const slots = useSlots();
onBeforeMount(() => {
  if (slots.default) {
    state.tabs = slots
      .default()
      .filter((child) => child.type["__name"] === "OTabItem");
  }
});
function SetOldSelectedTabIndexValue() {
  if (
    localStorage.OldSelectedTabIndexValue !== undefined &&
    localStorage.OldSelectedTabIndexValue &&
    localStorage.OldProcessGuid !== undefined &&
    localStorage.OldProcessGuid
  ) {
    if (props.processguid === localStorage.OldProcessGuid) {
      selectedTabIndex.value = localStorage.OldSelectedTabIndexValue;
    } else selectedTabIndex.value = 0;
  } else selectedTabIndex.value = 0;
}
function setLocalstorage(processguid, index) {
  localStorage.OldProcessGuid = processguid;
  localStorage.OldSelectedTabIndexValue = index;
  // selectedTab.value=index;
}

const selectedTab = ref();
watch(selectedTabIndex, (index: number, prev: number) => {
  if (!state.tabs?.length) return;
  selectedTabIndex.value = index;
  setLocalstorage(props.processguid, index);
  selectedTab.value = state.tabs[selectedTabIndex.value];
});
</script>

<template>
  <div class="tw-w-full" ref="tabRoot">
    <div class="top tw-h-12">
      <ul
        class="tw-box-border tw-flex tw-h-full tw-border-b-2 tw-border-solid tw-border-[#dee2e6] tw-text-center tw-font-bold tw-text-[#6c757d]"
      >
        <li
          v-for="(tabItem, index) in state.tabs"
          :key="index"
          :class="{
            selected: selectedTabIndex === index,
            unselected: selectedTabIndex !== index,
          }"
          @click="selectedTabIndex = index"
          class="tw-my-auto tw--mb-[2px] tw-h-full tw-cursor-pointer tw-px-4 tw-pt-2 tw-transition-[border-bottom-color] tw-duration-200"
        >
          {{ tabItem.props["title"] }}
        </li>
      </ul>
    </div>
    <!-- <div> -->
    <component :is="selectedTab" :key="selectedTabIndex" />
    <!-- </div> -->
  </div>
</template>

<style>
.selected {
  @apply tw-border-b-2 tw-border-solid tw-border-b-primary;
}
.unselected {
  @apply tw-border-b-transparent tw-pt-2 hover:tw-border-b-2 hover:tw-border-solid hover:tw-border-b-[#adb5bd];
}
</style>
