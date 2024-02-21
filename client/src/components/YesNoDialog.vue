<script setup lang="ts">
import { useVModel } from "@vueuse/core";
import { computed, useAttrs } from "vue";
import type { YesNoDialogState } from "@/composables/useYesNoDialog";
import Dialog from "primevue/dialog";

const props = defineProps<{
  title?: string;
  label?: string;
  state: YesNoDialogState;
  modelValue: boolean;
  width?: number;
  defaultMessage?: string;
  successMessage?: string;
  errorMessage?: string;
  loadingMessage?: string;
  retryButtonTitle?: string;
  yesButtonTitle?: string;
  cancelButtonTitle?: string;
  isLoading: boolean;
  hideCloseButton?: boolean;
}>();

const {
  // defaultMessage = "Сохранить изменения?",
  successMessage = "Успешно сохранено",
  errorMessage = "Произошла ошибка",
  width = 300,
  yesButtonTitle = "Сохранить",
  cancelButtonTitle = "Отменить",
  retryButtonTitle = "Повторить",
  loadingMessage = "Подождите...",
  hideCloseButton = false,
} = props;

const emit = defineEmits(["yes", "no", "update:modelValue", "close"]);

const show = useVModel(props, "modelValue", emit);
const buttonTitle = computed(() => {
  if (props.state === "none" || props.state === "loading")
    return yesButtonTitle;
  if (props.state === "error") return retryButtonTitle;
});
function yes() {
  emit("yes");
}
function no() {
  show.value = false;
  emit("no");
}

const attrs = useAttrs();
</script>

<template>
  <!-- <div v-bind="attrs"> -->
  <slot
    :click="
      () => {
        show = true;
      }
    "
    :label="label"
  >
    <icon-action
      :label="label"
      @click="
        () => {
          show = true;
        }
      "
      class="hover-list"
      v-bind="attrs"
    />
    <!-- <Button
      @click="
        
      "
      v-bind="attrs"
      >{{ label }}</Button
    > -->
  </slot>
  <!-- </div> -->
  <Dialog
    :header="title"
    v-model:visible="show"
    :modal="true"
    :style="{ width: width + 'px' }"
    :closable="!(state === 'success' && hideCloseButton)"
  >
    <template v-if="state === 'none'">
      <slot name="message">
        <div class="!tw-pt-0">
          {{ defaultMessage }}
        </div>
      </slot>
    </template>
    <template v-else-if="isLoading">
      {{ loadingMessage }}
    </template>
    <template v-else-if="state === 'success'">
      <slot name="successMessage" :successMessage="successMessage">
        {{ successMessage }}
      </slot>
    </template>
    <template v-else-if="state === 'error'">
      {{ errorMessage }}
    </template>
    <template v-slot:footer>
      <slot
        name="actions"
        :yes="yes"
        :no="no"
        :state="state"
        :cancelButtonTitle="cancelButtonTitle"
        :buttonTitle="buttonTitle"
      >
        <Button
          class="p-button p-button-secondary"
          style="background: var(--surface-600)"
          v-show="state !== 'success'"
          @click="no"
          type="button"
        >
          {{ cancelButtonTitle }}
        </Button>
        <Button
          v-show="state !== 'success'"
          @click="yes"
          :loading="state === 'loading'"
          loadingIcon="pi pi-spinner pi-spin"
          class="p-button p-button-secondary"
          type="button"
          >{{ buttonTitle }}</Button
        >
        <Button
          v-show="state === 'success' && !hideCloseButton"
          @click="emit('close')"
          class="p-button p-button-secondary"
          >Закрыть</Button
        >
      </slot>
    </template>
  </Dialog>
</template>
