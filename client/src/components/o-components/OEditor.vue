<script setup lang="ts">
import { v4 as uuidv4 } from "uuid";

withDefaults(
  defineProps<{
    modelValue: any;
    autoResize: boolean;
    errorMessage?: string | string[];
    label?: string;
    requiredMark?: boolean;
    readonly?: boolean;
  }>(),
  {
    autoResize: false,
  }
);

function removeLinks(event) {
  const parser = new DOMParser();
  const doc = parser.parseFromString(event?.htmlValue, "text/html");

  const links = doc.querySelectorAll("a");

  if (links.length) {
    links.forEach((link) => {
      const textNode = doc.createTextNode(link.textContent);
      link.parentNode.replaceChild(textNode, link);
    });

    const htmlWithoutLinks = doc.body.innerHTML;

    const editor = document.querySelector(".ql-editor");
    editor.innerHTML = htmlWithoutLinks;
  }
}

const emit = defineEmits(["update:modelValue"]);

const id = uuidv4();
</script>

<template>
  <div class="field">
    <span class="p-float-label">
      <Editor
        :id="id"
        :model-value="modelValue"
        @update:model-value="emit('update:modelValue', $event)"
        class="tw-w-full"
        editorStyle="height: 320px"
        :readonly="readonly"
        @text-change="removeLinks($event)"
      >
        <template v-slot:toolbar>
          <span class="ql-formats"
            ><select class="ql-header">
              <option value="1">Заголовок</option>
              <option value="2">Подзаголовок</option>
              <option value="0">Обычный</option>
            </select>

            <select class="ql-font">
              <option></option>
              <option value="serif"></option>
              <option value="monospace"></option></select
          ></span>
          <span class="ql-formats">
            <button v-tooltip.bottom="'Жирный'" class="ql-bold"></button>
            <button v-tooltip.bottom="'Курсив'" class="ql-italic"></button>
            <button
              v-tooltip.bottom="'Подчеркивание'"
              class="ql-underline"
            ></button>
          </span>

          <span class="ql-formats"
            ><select class="ql-color">
              <option selected="true"></option>
              <option value="#e60000"></option>
              <option value="#ff9900"></option>
              <option value="#ffff00"></option>
              <option value="#008a00"></option>
              <option value="#0066cc"></option>
              <option value="#9933ff"></option>
              <option value="#ffffff"></option>
              <option value="#facccc"></option>
              <option value="#ffebcc"></option>
              <option value="#ffffcc"></option>
              <option value="#cce8cc"></option>
              <option value="#cce0f5"></option>
              <option value="#ebd6ff"></option>
              <option value="#bbbbbb"></option>
              <option value="#f06666"></option>
              <option value="#ffc266"></option>
              <option value="#ffff66"></option>
              <option value="#66b966"></option>
              <option value="#66a3e0"></option>
              <option value="#c285ff"></option>
              <option value="#888888"></option>
              <option value="#a10000"></option>
              <option value="#b26b00"></option>
              <option value="#b2b200"></option>
              <option value="#006100"></option>
              <option value="#0047b2"></option>
              <option value="#6b24b2"></option>
              <option value="#444444"></option>
              <option value="#5c0000"></option>
              <option value="#663d00"></option>
              <option value="#666600"></option>
              <option value="#003700"></option>
              <option value="#002966"></option>
              <option value="#3d1466"></option></select
            ><select class="ql-background">
              <option value="#000000"></option>
              <option value="#e60000"></option>
              <option value="#ff9900"></option>
              <option value="#ffff00"></option>
              <option value="#008a00"></option>
              <option value="#0066cc"></option>
              <option value="#9933ff"></option>
              <option selected="true"></option>
              <option value="#facccc"></option>
              <option value="#ffebcc"></option>
              <option value="#ffffcc"></option>
              <option value="#cce8cc"></option>
              <option value="#cce0f5"></option>
              <option value="#ebd6ff"></option>
              <option value="#bbbbbb"></option>
              <option value="#f06666"></option>
              <option value="#ffc266"></option>
              <option value="#ffff66"></option>
              <option value="#66b966"></option>
              <option value="#66a3e0"></option>
              <option value="#c285ff"></option>
              <option value="#888888"></option>
              <option value="#a10000"></option>
              <option value="#b26b00"></option>
              <option value="#b2b200"></option>
              <option value="#006100"></option>
              <option value="#0047b2"></option>
              <option value="#6b24b2"></option>
              <option value="#444444"></option>
              <option value="#5c0000"></option>
              <option value="#663d00"></option>
              <option value="#666600"></option>
              <option value="#003700"></option>
              <option value="#002966"></option>
              <option value="#3d1466"></option></select
          ></span>
          <span class="ql-formats" data-pc-section="formats"
            ><button
              v-tooltip.bottom="'Нумерация'"
              class="ql-list"
              value="ordered"
            ></button
            ><button
              v-tooltip.bottom="'Маркеры'"
              class="ql-list"
              value="bullet"
            ></button
            ><select class="ql-align">
              <option defaultvalue=""></option>
              <option value="center"></option>
              <option value="right"></option>
              <option value="justify"></option></select
          ></span>
          <span class="ql-formats"
            ><button
              v-tooltip.bottom="'Рисунки'"
              class="ql-image"
            ></button></span></template
      ></Editor>
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>
