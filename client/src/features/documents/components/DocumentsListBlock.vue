<script setup lang="ts">
import { computed } from "vue";
import type { Attachment } from "../types";

const props = defineProps<{
  items: Attachment[] | undefined;
  zipName?: string;
  selectedDocument?: Attachment;
}>();

const emit = defineEmits<{
  "update:selectedDocument": [value: Attachment];
}>();

const zipUrl = computed(() => {
  if (!props.items?.length) return "";
  const ids = props.items
    .filter((doc) => doc.kind !== "template")
    .map((doc) => {
      return new URL(doc.url).searchParams.get("id");
    });
  if (!ids?.length) return "";
  const url = new URL(import.meta.env.VITE_CONTENTS_API_URL + "/asZip");
  for (let i = 0; i < ids.length; i++) {
    url.searchParams.append("ids", ids[i]);
  }
  if (props.zipName) url.searchParams.append("name", props.zipName);
  return url.toString();
});

function download(url) {
  const anchor = document.createElement("a");
  anchor.href = url;
  anchor.download = url.split("/").pop();
  document.body.appendChild(anchor);
  anchor.click();
  document.body.removeChild(anchor);
}
</script>

<template>
  <card-block :header-height="40" class="tw-h-fit tw-bg-white">
    <template v-slot:header>
      <div class="tw-flex tw-w-full tw-items-center">
        <div class="tw-px-4">Документы</div>
        <div class="tw-flex-1" />
        <Button text rounded @click="download(zipUrl)">
          <template v-slot>
            <i-mdi-folder-download-outline class="tw-text-white" />
          </template>
        </Button>
      </div>
    </template>
    <div class="tw-overflow-auto tw-rounded-none">
      <Listbox
        v-if="items?.length"
        :modelValue="selectedDocument"
        @update:modelValue="emit('update:selectedDocument', $event as any)"
        :options="items"
        dataKey="url"
        :optionLabel="(v) => v.name"
        :pt="{
          root: {
            class: [
              'tw-bg-white tw-border-0 tw-transition-colors tw-duration-200 tw-ease-in-out tw-rounded-none',
              'tw-w-full',
            ],
          },
          list: 'tw-list-none tw-m-0',
          item: ({ context }) => ({
            class: [
              'tw-cursor-pointer tw-font-normal tw-overflow-hidden tw-relative tw-whitespace-nowrap',
              'tw-border-0 tw-transition-shadow tw-duration-200 tw-rounded-none',
              'hover:tw-text-gray-700 hover:tw-bg-gray-200',
              {
                'tw-text-gray-700': !context.focused && !context.selected,
                'tw-bg-gray-300 tw-text-gray-700':
                  context.focused && !context.selected,
                'tw-bg-blue-400 tw-text-blue-700':
                  context.focused && context.selected,
                'tw-bg-blue-50 tw-text-blue-700':
                  !context.focused && context.selected,
              },
            ],
          }),
        }"
      >
        <template v-slot:option="{ option }">
          <div
            class="tw-group tw-flex tw-w-full tw-items-center"
            @click.prevent="
              if (option === selectedDocument) $event.stopPropagation();
            "
          >
            <div
              class="tw-block tw-max-w-2xl tw-flex-shrink tw-truncate xl:tw-max-w-md"
            >
              {{ option.name }}
            </div>
            <div class="tw-flex-1" />
            <div
              v-if="option.kind !== 'template'"
              :class="[
                'hover:tw-text-pr tw-invisible tw-my-auto tw-rounded-full tw-p-2 group-hover:!tw-visible',
                {
                  '!tw-visible hover:tw-bg-blue-500/20 active:tw-bg-blue-500/30':
                    option === selectedDocument,
                  'hover:tw-bg-gray-400/20 active:tw-bg-gray-400/30':
                    option !== selectedDocument,
                },
              ]"
              @click="if (option.kind !== 'template') download(option.url);"
            >
              <i-mdi-download-outline />
            </div>
          </div>
        </template>
      </Listbox>
      <div v-else class="tw-w-full tw-py-3 tw-text-center">
        <span class="tw-font-thin">Нет вложении</span>
      </div>
    </div>
  </card-block>
</template>
