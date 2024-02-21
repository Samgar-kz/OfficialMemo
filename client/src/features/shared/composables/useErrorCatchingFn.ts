import type { ToastServiceMethods } from "primevue/toastservice";
import { useToast } from "primevue/usetoast";
import { ref } from "vue";

export function useErrorCatchingFn<
  TFun extends (...args: any[]) => any | Promise<any>
>(
  fn: TFun,
  {
    errorMessage = "Error",
    showToast = true,
  }: { errorMessage?: string; showToast?: boolean } = {}
) {
  const toast = showToast ? useToast() : undefined;
  const isError = ref(false);

  const execute = ((...args: Parameters<TFun>) => {
    try {
      isError.value = false;
      const result = fn(...args);
      return result instanceof Promise
        ? result.catch((error: Error) => handleCatch(error))
        : result;
    } catch (error) {
      return handleCatch(error);
    }

    function handleCatch(error: Error) {
      isError.value = true;
      console.error("error", error);
      toast?.add({ severity: "error", detail: errorMessage, life: 3000 });
      //   throw error; // Re-throwing the error to preserve the behavior outside of the execute function
    }
  }) as TFun;

  return { execute, isError };
}
