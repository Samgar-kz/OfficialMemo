import { useEventBus } from "@vueuse/core";
import type { ConfirmationDialogOptions } from "./types";

// const bus = ref<UseEventBusReturn<ConfirmationDialogOptions, any>>({} as any);
export default function useConfirmDialog() {
  const bus = useEventBus<ConfirmationDialogOptions>("confirmationServiceBus");
  // const unsubscribe = bus.value.on((options: ConfirmationDialogOptions) => {});
  const require = (options: ConfirmationDialogOptions) => {
    options = {
      ...options,
      confirmLabel: options.confirmLabel ?? "Да",
      rejectLabel: options.rejectLabel ?? "Нет",
    };
    bus.emit(options);
  };

  // onUnmounted(unsubscribe);

  return { require };
}
