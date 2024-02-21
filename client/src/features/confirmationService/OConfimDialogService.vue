<script setup lang="ts">
import closeWindow from "@/server/closeWindow";
import { useEventBus } from "@vueuse/core";
import type { ConfirmationDialogOptions, ConfirmationStates } from "./types";
import { onUnmounted, ref, watch } from "vue";

const bus = useEventBus<ConfirmationDialogOptions>("confirmationServiceBus");
const localOptions = ref<ConfirmationDialogOptions>(
  {} as ConfirmationDialogOptions
);
const unsubscribe = bus.on((options) => {
  localOptions.value = options;
  visible.value = true;
});
onUnmounted(unsubscribe);

const visible = ref(false);

const state = ref<ConfirmationStates>("none");

async function confirm() {
  try {
    if (localOptions.value.onConfirm) {
      state.value = "loading";
      await Promise.resolve(localOptions.value.onConfirm());
      state.value = "confirmed";
    }
  } catch (error) {
    state.value = "error";
    errorMessage.value = error.message;
    console.error("Error while confirming", error);
    if (localOptions.value?.onRejectError) localOptions.value?.onRejectError();
  }
}

async function reject() {
  try {
    if (localOptions.value.onReject) {
      state.value = "loading";
      await Promise.resolve(localOptions.value.onReject());
      state.value = "rejected";
    }
    visible.value = false;
  } catch (error) {
    state.value = "error";
    errorMessage.value = error.message;
    console.error("Error while rejecting:", error);
    if (localOptions.value?.onRejectError) localOptions.value?.onRejectError();
  }
}

function reset() {
  state.value = "none";
}

watch(visible, (v) => console.log("visible canged to", v));

const errorMessage = ref("");
</script>

<template>
  <Dialog
    v-model:visible="visible"
    :header="localOptions.title"
    modal
    @hide="reset()"
  >
    <div>
      <p>{{ localOptions.message }}</p>
      <div class="tw-text-sm tw-italic tw-text-error" v-if="state === 'error'">
        <p>Ошибка{{ errorMessage ? ":" : "" }}</p>
        <p class="tw-pl-2">{{ errorMessage }}</p>
      </div>
    </div>
    <template v-slot:header>
      <span class="tw-pr-4 tw-text-lg">{{ localOptions.title }}</span>
    </template>
    <template v-slot:footer>
      <Button
        v-show="state !== 'loading' && state !== 'none' && state !== 'error'"
        @click="closeWindow"
        class="p-button p-button-secondary"
        >Закрыть</Button
      >
      <Button
        v-show="state !== 'confirmed'"
        :disabled="state === 'loading'"
        @click="reject"
        type="button"
        class="p-button p-button-secondary"
        >{{ localOptions.rejectLabel }}</Button
      >
      <Button
        v-show="state !== 'rejected' && state !== 'confirmed'"
        @click="confirm"
        :loading="state === 'loading'"
        :disabled="state === 'loading'"
        loadingIcon="pi pi-spinner pi-spin"
        class="p-button p-button-primary"
        type="button"
        >{{ localOptions.confirmLabel }}</Button
      >
    </template>
  </Dialog>
</template>
