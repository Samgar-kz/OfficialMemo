<script setup lang="ts">
import { v4 as uuidv4 } from "uuid";
import Editor from "@tinymce/tinymce-vue";
// import CKEditor from "@ckeditor/ckeditor5-vue";
// import Editor from "@/plugins/ckeditor/config";

withDefaults(
  defineProps<{
    modelValue: any;
    autoResize: boolean;
    errorMessage?: string | string[];
    label?: string;
    requiredMark?: boolean;
  }>(),
  {
    autoResize: false,
  }
);
const emit = defineEmits(["update:modelValue"]);

const id = uuidv4();
var dat;
const editor = Editor;
// const ckeditor = CKEditor.component;

// c
</script>

<template>
  <div class="field">
    <span class="p-float-label">
      <!-- <ckeditor
        :id="id"
        :editor="editor"
        :model-value="modelValue"
        @update:model-value="emit('update:modelValue', $event)"
      /> -->
      <Editor
        :id="id"
        :model-value="modelValue"
        initial-value="Once upon a time..."
        @update:model-value="emit('update:modelValue', $event)"
        class="tw-w-full"
        api-key="v3gwk3r7ay2qk6schyr1rhquhpmwc809zxupvca95ebbywv7"
        :init="{
          min_height: 300,
          menubar: false,
          language: 'ru',
          statusbar: false,
          placeholder: label,
          resize: true,
          plugins:
            'preview importcss searchreplace autolink autosave save directionality visualblocks visualchars fullscreen image link media table nonbreaking anchor advlist lists help charmap autoresize',
          toolbar: [
            'lineheight blocks fontfamily fontsize bold italic underline align forecolor backcolor | undo redo strikethrough subscript superscript bullist numlist outdent indent blockquote charmap table link unlink removeformat fullscreen preview',
          ],
          image_caption: true,
          editimage_toolbar:
            'rotateleft rotateright | flipv fliph | editimage imageoptions',
        }"
      />
      <!-- <label :for="id" :class="{ required: requiredMark }">{{ label }}</label> -->
    </span>
    <small v-if="errorMessage" :id="`${id}-help`" class="p-error tw-ml-3">{{
      errorMessage
    }}</small>
  </div>
</template>
