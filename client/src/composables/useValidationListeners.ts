import { computed, type Ref } from "vue";

export default function useValidationListeners(errorMessage: Ref<string>, handleChange: (e: unknown, shouldValidate?: boolean) => void) {
  const validationListeners = computed(() => {
    // If the field is valid or have not been validated yet
    // lazy
    if (!errorMessage.value) {
      return {
        blur: handleChange,
        change: handleChange,
        // disable `shouldValidate` to avoid validating on input
        input: (e) => handleChange(e, false),
      };
    }
    // Aggressive
    return {
      blur: handleChange,
      change: handleChange,
      input: handleChange, // only switched this
    };
  });
  return validationListeners;
}
