import { ref, type Ref, computed, watch } from "vue";

export type YesNoDialogState = "none" | "loading" | "success" | "error";

export default function useYesNoDialog(options: {
  onYes: Function;
  show?: Ref<boolean>;
  onNo?: Function;
  closeOnSuccess?: boolean;
  onCloseWindow?: Function;
  onValidate?: Function;
}) {
  const { onYes, onNo, closeOnSuccess, onCloseWindow, onValidate } = options;
  const state = ref<YesNoDialogState>("none");
  const show = options.show ?? ref(false);

  const isLoading = computed(() => state.value === "loading");

  async function yes() {
    try {
      if (state.value !== "none" && state.value !== "error") return;
      if (onValidate) {
        let { valid } = await onValidate();
        if (!valid) return;
      }
      state.value = "loading";
      await onYes();
      state.value = "success";
      if (closeOnSuccess && onCloseWindow) {
        onCloseWindow();
      }
    } catch (error) {
      console.error(error);
      state.value = "error";
    }
  }

  function no() {
    show.value = false;
    if (onNo) onNo();
  }
  watch(show, () => {
    if (!show.value) state.value = "none";
  });
  return { show, state, yes, no, isLoading };
}
