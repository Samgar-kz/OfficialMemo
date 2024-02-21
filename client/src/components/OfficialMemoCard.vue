<script setup lang="ts">
import getLanguages from "@/server/api/handbooks/getLanguages";
import type { Language } from "@/types/process/Language";
import type { Model } from "@/types/process/Model";
import { ref, onMounted, watch, computed } from "vue";

type OutgoingMessageChangeModel = {
  language: Language;
};

const props = defineProps<{
  officialMemo: Model<any>;
}>();

const changeModel = ref<OutgoingMessageChangeModel>({} as any);

function updateChangeModel() {
  changeModel.value = {
    language: props.officialMemo?.data?.language,
  };
}
onMounted(() => updateChangeModel());
watch(
  () => props,
  () => updateChangeModel()
);

const languages = ref<Language[]>();
onMounted(async () => {
  languages.value = await getLanguages();
});
</script>

<template>
  <card-block :header="officialMemo.data?.regNum">
    <div
      class="tw-grid tw-grid-cols-[auto_1fr] tw-gap-x-4 tw-px-4 tw-py-2 tw-text-gray-500"
    >
      <span class="tw-col-start-1 tw-text-sm tw-font-semibold">Инициатор:</span>
      <span>{{ officialMemo.data?.executor?.name }}</span>

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Получатели:</span
      >
      <span
        class="tw-col-start-2"
        v-for="(recipient, _ri) in officialMemo.data?.recipients"
        :key="_ri"
        >{{ recipient?.name }}</span
      >

      <span class="tw-self-center tw-text-sm tw-font-semibold">Язык:</span>
      <span>{{ officialMemo.data?.language }}</span>
      <!-- <o-editable-field :modelValue="props.officialMemo?.data?.language" @update:modelValue="updateLanguage($event)">
        <template v-slot:display="{ modelValue }">
          <span>{{ modelValue }}</span>
        </template>
        <template v-slot:editor="{ value, updateValue }">
          <o-select name="language" class="required tw-col-span-6" :options="languages" :modelValue="value" @update:modelValue="updateValue($event)" />
        </template>
      </o-editable-field> -->

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Гриф конфиденциальности:</span
      >
      <span>{{ officialMemo.data?.confidenceType?.displayTextRu }}</span>

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Индекс номенклатуры:</span
      >
      <span>{{ officialMemo.data?.indexNomenclature }}</span>

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Тема документа:</span
      >
      <span>{{ officialMemo.data?.subject }}</span>

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Тип согласования:</span
      >
      <span
        >{{
          officialMemo.data?.approvalMode === "parallel"
            ? "Параллельное"
            : "Последовательное"
        }}
      </span>

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Согласующие:</span
      >
      <span
        class="tw-col-start-2"
        v-for="(approver, _ai) in officialMemo.data?.approvers"
        :key="_ai"
        >{{ approver?.name }}</span
      >

      <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Подписывающий:</span
      >
      <span class="tw-col-start-2">{{ officialMemo.data?.signer?.name }}</span>

      <!-- <span class="tw-col-start-1 tw-text-sm tw-font-semibold"
        >Краткое содержание:</span
      >
      <div class="tw-col-span-full tw-pl-4">
        <span>{{ officialMemo.data?.subject }}</span>
      </div> -->
    </div>
  </card-block>
</template>
