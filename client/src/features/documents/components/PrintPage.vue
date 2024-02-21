<script setup lang="ts">
import { ref } from "vue";
import Button from "primevue/button";
import "@/assets/css/typography.scss";
import { v4 as uuidv4 } from "uuid";
import printJs from "print-js";
import styles from "@/assets/css/main.scss?inline";
import typography from "@/assets/css/typography.scss?inline";
import editor from "quill/dist/quill.core.css?inline";
import type { MyDocument } from "@/types/contents/MyDocument";

withDefaults(
  defineProps<{
    file?: MyDocument;
    htmlContent?: string;
  }>(),
  { file: {} as any }
);
const emit = defineEmits(["update:htmlContent"]);

const container = ref<HTMLElement>({} as HTMLElement);
const innerHTML = ref("");
function updateHtmlContent() {
  innerHTML.value = `<!DOCTYPE html>
    <html>
      <head>
        <meta charset="UTF-8">
      </head>
      <style>
      @page {
            size: A4;
            margin: 0mm;
          } 
          #main_content{
            margin-left: 3cm;
            margin-right: 1cm;
            margin-top: 2cm;
            margin-bottom: 2.5cm;
          }
          
        ${typography}
        ${editor}
        ${styles}

      </style>
      <body>
        ${container?.value.innerHTML}
      </body>
    </html>`;
  emit("update:htmlContent", innerHTML.value);
}

const printSectionId = `print_section_${uuidv4()}`;
function print() {
  const rawHtml = document.getElementById(printSectionId);
  console.log(rawHtml.outerHTML);

  printJs({
    printable: rawHtml.outerHTML,
    type: "raw-html",
    scanStyles: false,
    targetStyles: "*",
    ignoreElements: "print_button_id",
    documentTitle: undefined,
    style: `
      @page {
        size: A4;
        margin: 0mm;
      } 
      #main_content{
        margin-left: 3cm;
        margin-right: 1cm;
        margin-top: 2cm;
        margin-bottom: 2.5cm;
      }
        ${typography}
        ${editor}
        ${styles}

    `,
  });
}
function extractInnerHtml() {
  updateHtmlContent();
  return innerHTML.value;
}
defineExpose({ extractInnerHtml });
</script>

<template>
  <div
    class="print_page__wrapper tw-relative tw-flex tw-min-h-[297mm] tw-flex-col tw-overflow-auto tw-bg-white tw-bg-clip-border"
    :id="printSectionId"
    ref="container"
  >
    <Suspense @resolve="updateHtmlContent">
      <slot />
    </Suspense>
    <Button
      v-show="htmlContent"
      icon="pi pi-print"
      @click="print()"
      class="p-button-rounded p-button-secondary p-button-sm print_button tw-absolute tw-bottom-8 tw-right-[5%]"
      id="print_button_id"
    />
  </div>
</template>

<style>
.print_page__wrapper {
  /* padding: 10mm 15mm 20mm 30mm; */
  print-color-adjust: initial;
  -webkit-print-color-adjust: exact;
}

@page {
  size: A4;
  margin: 0mm;
}
</style>
