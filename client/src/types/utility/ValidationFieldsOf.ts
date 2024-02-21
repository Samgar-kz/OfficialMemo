import type { MaybeRef } from "@vueuse/shared";
import { unref } from "vue";
import type NestedKeyOf from "./NestedKeyOf";

type ValidationFieldsOf<T extends object> = {
  [Property in NestedKeyOf<T> ]?: string | (object & { required?: MaybeRef<boolean> | undefined });
};

export default ValidationFieldsOf;

function isRequired(rules: string | (object & { required?: MaybeRef<boolean> | undefined })) {
  if (!rules) return false;
  if (typeof rules === "string") return rules.includes("required");
  if (rules instanceof Object) return unref(rules["required"]) ?? false;
  return false;
}

export { isRequired };
