import { ref } from "vue";
import { reactive } from "vue";

export default function useAsyncKeyedState<TState>(
  fn: (key: string) => Promise<TState>
) {
  const state = reactive({});
  const isLoading = ref(false);
  const isError = ref(false);
  const error = ref(null);

  const fetch = async (key: string) => {
    isLoading.value = true;
    isError.value = false;
    try {
      if (Object.prototype.hasOwnProperty.call(state, key)) return;
      const value = await fn(key);
      state[key] = value;
    } catch (e) {
      error.value = e;
      isError.value = true;
      throw e;
    } finally {
      isLoading.value = false;
    }
  };

  return {
    state,
    isLoading,
    isError,
    error,
    fetch,
  };
}
