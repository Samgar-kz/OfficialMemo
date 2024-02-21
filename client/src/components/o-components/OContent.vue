<script setup lang="ts">
import type { TabViewChangeEvent } from "primevue/tabview";
import { ref, onBeforeMount, watch } from "vue";
import TabView from "primevue/tabview";
import WideLayout from "@/layouts/WideLayout.vue";
import { useRoute, useRouter } from "vue-router";

defineProps<{
  processGuid: string;
}>();

const indexTab = ref<number>();
const router = useRouter();
const route = useRoute();

onBeforeMount(() => {
  indexTab.value = localStorage.selectedIndex
    ? Number(localStorage.selectedIndex)
    : 0;
  preselectTab(route.hash);
});

function tabChange(e: TabViewChangeEvent) {
  localStorage.selectedIndex = e.index;
  indexTab.value = e.index;
}

watch(indexTab, (val) => {
  router.replace({
    ...router.currentRoute,
    hash: val === 0 ? "#document" : "#history",
  });
});

function preselectTab(hash: string) {
  hash = hash.substring(1);
  if (hash) {
    indexTab.value = hash === "document" ? 0 : 1;
  }
}
</script>

<template>
  <wide-layout>
    <TabView
      ref="tabview"
      class="top tw-h-full"
      @tab-change="tabChange($event)"
      :active-index="indexTab"
    >
      <TabPanel header="Документ">
        <div
          class="tw-grid tw-grid-cols-[4fr_5fr_4fr] tw-px-4 tw-gap-4 tw-py-4 tw-bg-slate-100"
        >
          <slot />
        </div>
      </TabPanel>
      <TabPanel header="Ход исполнения">
        <div class="tw-grid tw-grid-cols-[0px_6fr] tw-gap-2 tw-m-2">
          <process-history-tree
            class="tw-col-start-2"
            :process-guid="processGuid"
          />
        </div>
      </TabPanel>
    </TabView>
  </wide-layout>
</template>
