<script lang="ts" setup>
import { onMounted, ref } from "vue";
import type { Field } from "@/types/shared/Field";
import type { Executor } from "@/types/Executor";

import getExecutorsList from "@/server/api/bpm/users/getExecutorsList";
function isRole(text) {
  return !text?.toLowerCase()?.includes("hcsbk");
}
function getOverlayNoDataTitle(code: string, name: string) {
  return (
    (isRole(code) ? "К роли " : "У пользователя ") +
    '"' +
    name +
    '" ' +
    (isRole(code) ? "не привязаны пользователи " : "нет заместителя")
  );
}
const executorsList = ref<Executor[]>([{}] as Executor[]);

let OverlayTableTitle = ref<string>();
const fields = ref<Field[]>([
  {
    value: "code",
    name: "Код пользователя",
    style: "width: 30%",
    class: "tw-italic tw-text-secondary tw-font-medium",
    sortable: true,
  },
  {
    value: "fio",
    name: "ФИО",
    style: "width: 40%",
    sortable: true,
  },
  {
    value: "title",
    name: "Область замещения",
    style: "width: 30%",
    sortable: false,
  },
]);
const props = defineProps<{
  executorCode: string;
  executorName: string;
  userName: string;
}>();
const emit = defineEmits(["toggle", "onItemSelect", "close"]);
onMounted(() => {
  if (!isRole(props.executorCode)) {
    OverlayTableTitle.value = `Заместители пользователя "' ${props.executorName} "`;
    fields.value = fields.value.slice(0, -1);
  } else OverlayTableTitle.value = props.executorName;
});
</script>
<template>
  <overlay-table
    v-on:close="emit('close')"
    :title="OverlayTableTitle"
    :no-data-text="getOverlayNoDataTitle(executorCode, executorName)"
    @toggle="async () => (executorsList = await getExecutorsList(executorCode))"
    :fields="fields"
    :items="executorsList"
  >
    <template v-slot:activator="{ click: click }">
      <span class="original-executor" @click="click">
        {{
          isRole(executorCode)
            ? executorName
            : userName && !executorName
            ? isRole(userName)
            : executorName?.substring(0, executorName?.lastIndexOf(" "))
        }}
      </span>
    </template>
  </overlay-table>
</template>

<style>
.p-overlaypanel-content {
  padding: 15px 0px 0px !important;
}
</style>
