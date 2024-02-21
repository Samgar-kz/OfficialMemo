<script setup lang="ts">
import type { Attachment } from "../types";
import VuePdfEmbed from "vue-pdf-embed";
import axios from "axios";
import { useErrorCatchingFn } from "@/features/shared/composables/useErrorCatchingFn";
import useAsyncKeyedState from "@/features/shared/composables/useAsyncKeyedState";
import { onMounted, watch } from "vue";

const props = defineProps<{ document?: Attachment; heightClass?: string }>();

const availableExtensions = ["pdf", "docx", "img", "template"];

const httpClient = axios.create({
  withCredentials: true,
});

async function getDocx(source: string) {
  const { data } = await httpClient.get<ArrayBuffer>(source, {
    responseType: "arraybuffer",
  });
  const blob = new Blob([data], {
    type: "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
  });
  return new File([blob], "filename.docx", { type: blob.type });
}

async function getPdfBase64(source: string) {
  if (!source) return "";

  const { data } = await httpClient.get(source, { responseType: "blob" });
  const result = URL.createObjectURL(data);

  return result;
}

const { state, isLoading, isError, fetch } = useAsyncKeyedState(
  async (url: string) => {
    if (!url) return;
    try {
      if (props.document?.kind === "pdf") return getPdfBase64(url);
      else if (props.document?.kind === "docx") return getDocx(url);
      return undefined;
    } catch {
      return undefined;
    }
  }
);
const { execute: fetchData } = useErrorCatchingFn(
  async () => await fetch(props.document?.url),
  {
    showToast: true,
    errorMessage: "Произошла ошибка при получении данных",
  }
);

onMounted(fetchData);
watch(() => props.document, fetchData);
</script>

<template>
  <div
    class="tw-min-w-[210mm] tw-max-w-[297mm] tw-min-h-[210mm] tw-overflow-clip tw-rounded-lg tw-bg-white tw-drop-shadow-md"
  >
    <ScrollPanel class="tw-h-full">
      <Skeleton v-if="isLoading" height="100%" :class="heightClass" />
      <!-- <span
        v-else-if="!document?.url"
        class="tw-flex tw-w-full tw-flex-row tw-justify-center tw-pt-[30px] tw-font-light"
        >Ошибка</span
      > -->
      <div
        v-else-if="document && !availableExtensions.includes(document.kind)"
        class="tw-flex tw-w-full tw-flex-col tw-items-center tw-pb-[10px] tw-pt-[30px] tw-font-light"
        :class="heightClass"
      >
        <p>
          Тип файла
          {{ document.name.substring(document.name.lastIndexOf(".")) }} не
          поддерживается для просмотра в браузере.
        </p>
        <p>
          <a :href="document.url" class="tw-text-secondary tw-underline"
            >Скачайте</a
          >
          файл, чтобы просмотреть
        </p>
      </div>
      <div
        v-else-if="isError"
        class="tw-flex tw-w-full tw-flex-row tw-items-center tw-pt-[30px]"
        :class="heightClass"
      >
        <a :href="document?.url">{{ document?.name }}</a>
      </div>
      <docx-viewer
        v-else-if="document && document.kind === 'docx'"
        :source="state[document.url]"
        :class="heightClass"
      />
      <vue-pdf-embed
        v-else-if="document && document.kind === 'pdf'"
        text-layer
        :source="state[document.url]"
        :class="heightClass"
      />
      <Image
        v-else-if="document && document.kind === 'img'"
        :src="document.url"
        :alt="document.name"
        preview
        :class="heightClass"
      />
      <div
        v-else-if="document && document.kind === 'template'"
        :class="heightClass"
      >
        <component :is="$slots[document.name]" />
      </div>
    </ScrollPanel>
  </div>
</template>
