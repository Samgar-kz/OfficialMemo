<script setup lang="ts">
import type OTabItem from "./OTabItem.vue";
import {
  onBeforeMount,
  onMounted,
  reactive,
  ref,
  useSlots,
  watch,
  type RendererNode,
  type VNode,
} from "vue";
import { useRoute, useRouter } from "vue-router";
const children = ref<typeof OTabItem>();
const tabRoot = ref();
onMounted(() => {
  children.value = tabRoot.value.children;
});

const state = reactive({
  selectedIndex: 0,
  tabs: [] as VNode<RendererNode>[],
  count: 0,
});
const slots = useSlots();

onBeforeMount(() => {
  if (slots.tabs) {
    state.tabs = slots
      .tabs()
      .filter((child) => child.type["__name"] === "OTabItem");
  }
});

const selectedTabIndex = ref(-1);
const selectedTab = ref();
watch(selectedTabIndex, (index: number) => {
  if (!state.tabs?.length) return;
  selectedTabIndex.value = index;
  selectedTab.value = state.tabs[selectedTabIndex.value];

  let newTabValue = selectedTab.value.props.value;
  if (!newTabValue) newTabValue = selectedTabIndex.value.toString();
  router.replace({ ...router.currentRoute, hash: `#${newTabValue}` });
});

const router = useRouter();
const route = useRoute();
onBeforeMount(() => {
  if (slots.tabs) {
    preselectTab(route.hash);
  }
});
watch(
  () => route,
  (newRoute) => {
    preselectTab(newRoute.hash);
  }
);
function preselectTab(hash: string) {
  hash = hash.substring(1);
  if (hash) {
    let preselectedTabIndex = state.tabs.findIndex(
      (t) => t.props.value == hash
    );
    if (preselectedTabIndex === -1) {
      if (!isNaN(parseInt(hash)) && parseInt(hash) < state.tabs.length) {
        preselectedTabIndex = parseInt(hash);
      } else preselectedTabIndex = 0;
    }
    selectedTabIndex.value = preselectedTabIndex;
  } else selectedTabIndex.value = 0;
}
</script>

<template>
  <div class="tw-w-full" ref="tabRoot">
    <div class="top tw-h-12 tw-bg-white">
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
    <slot name="tabs" v-if="false" />
    <slot :tab="selectedTab" :selectedTabIndex="selectedTabIndex">
      <component :is="selectedTab" :key="selectedTabIndex" />
    </slot>
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
