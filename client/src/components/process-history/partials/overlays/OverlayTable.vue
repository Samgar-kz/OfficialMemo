<template>
  <slot name="activator" :click="toggle" :label="label">
    <Button type="button" :name="label" @click="toggle" />
  </slot>
  <overlay-panel
    @hide="close"
    class="tw-font-semibold"
    ref="panel"
    appendTo="div"
    :showCloseIcon="true"
    :id="'overlay_panel' + id"
  >
    <slot
      name="content"
      :items="items"
      :fields="fields"
      v-if="items?.length > 0"
    >
      <div
        class="p-panel-header tw-mb-2 tw-justify-center tw-text-center tw-text-[14px] tw-px-5"
      >
        {{ title }}
      </div>
      <data-table
        v-if="items?.length > 0 && fields?.length > 0"
        :value="items"
        :rowsPerPageOptions="[5, 10, 25, 50, 100]"
        responsiveLayout="scroll"
        :paginator="items?.length > 5 ? true : false"
        :rows="5"
        class="tw-text-[12px]"
        :breakpoints="{ '960px': '75vw' }"
        paginatorTemplate="FirstPageLink PrevPageLink PageLinks NextPageLink LastPageLink RowsPerPageDropdown"
      >
        <Column
          v-for="(field, index) in fields"
          v-bind:key="index"
          :field="field.value"
          :header="field.name"
          :sortable="field.sortable"
          :style="field.style"
          :class="field.class"
        />
      </data-table>
    </slot>
    <slot v-else name="noDataTitle" :noDataText="noDataText">
      <div
        class="p-panel-header tw-mb-2 tw-justify-center tw-pl-4 tw-pr-4 tw-text-center tw-text-[14px]"
      >
        {{ noDataText }}
      </div>
    </slot>
  </overlay-panel>
</template>

<script lang="ts" setup>
import { v4 as uuidv4 } from "uuid";
import { ref } from "vue";
import OverlayPanel from "primevue/overlaypanel";
import type { Field } from "@/types/shared/Field";

const panel = ref();
type responsiveLayout = "stack" | "scroll";
type selectionMode = "single" | "multiple";
const id = uuidv4();
const props = defineProps<{
  fields: Field[];
  items: any[];
  selectionMode?: selectionMode;
  paginator?: boolean;
  showCloseIcon?: { type: boolean; default: true };
  rows?: number;
  title?: string;
  label?: string;
  noDataText?: string;
  breakpoints?: number;
  responsiveLayout?: { type: responsiveLayout; default: "stack" };
}>();
const emit = defineEmits(["toggle", "close"]);
const toggle = async (event) => {
  emit("toggle", event);
  panel.value.toggle(event);
};

const close = () => {
  emit("close");
};
</script>

<style>
.p-overlaypanel-content {
  padding: 15px 0px 0px !important;
}
</style>
