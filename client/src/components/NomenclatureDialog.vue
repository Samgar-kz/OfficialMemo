<script setup lang="ts">
import { onMounted, ref, watch } from "vue";
import YesNoDialog from "./YesNoDialog.vue";
import useYesNoDialog from "@/composables/useYesNoDialog";
import { useVModel } from "@vueuse/core";
import getIndexNomenclatures from "@/server/api/handbooks/getIndexNomenclatures";
import type { IndexNomenclature } from "@/types/process/IndexNomenclature";

const props = withDefaults(
  defineProps<{
    modelValue: string | undefined;
    label: string;
    width: number;
  }>(),
  {
    width: 500,
  }
);

const indexNomenclatures = ref<IndexNomenclature[]>();

const emit = defineEmits([
  "success",
  "update:modelValue",
  "update:reply",
  "yes",
  "no",
]);

const ynDialog = useYesNoDialog({
  onYes: () => {},
  // onCloseWindow: closeWindow,
  // closeOnSuccess: true,
});

const departments = ref();
const indexes = ref();
const index = ref();
const selectedDep = ref();
const selectedIndex = ref();
onMounted(async () => {
  indexNomenclatures.value = await getIndexNomenclatures();

  departments.value = Array.from(
    new Set(indexNomenclatures.value.map((obj) => obj.department))
  ).map((department) => {
    return indexNomenclatures.value.find((obj) => obj.department === department)
      .department;
  });
});

function saveValue() {
  emit("update:modelValue", index);
  ynDialog.show.value = false;
  ynDialog.state.value = "none";
}

function setIndex(event) {
  index.value = event;
}

watch(selectedDep, (val) => {
  indexes.value = indexNomenclatures.value.filter(
    (obj) => obj.department === val
  );
});
</script>

<template>
  <yes-no-dialog
    :title="label"
    :label="label"
    :state="ynDialog.state.value"
    v-model="ynDialog.show.value"
    @yes="saveValue"
    @no="ynDialog.no"
    :is-loading="ynDialog.isLoading.value"
    :width="width"
  >
    <template v-slot:message>
      <v-select
        label="Департамент"
        name="department"
        class="required tw-col-span-6"
        @update:modelValue="() => (selectedIndex = null)"
        :options="departments"
        v-model="selectedDep"
      />
      <o-select-index
        label="Заголовок дела"
        name="indexes"
        class="required tw-col-span-6"
        :options="indexes"
        optionLabel="name"
        optionValue="index"
        @update:modelValue="setIndex($event)"
        v-model="selectedIndex"
      />
    </template>
    <template v-slot:default="{ click: click1, label }">
      <slot :click="click1" :label="label"> </slot>
    </template>
  </yes-no-dialog>
</template>
