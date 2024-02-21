import type NestedKeyOf from "./NestedKeyOf";

type FieldsOf<T extends object> = { [Property in NestedKeyOf<T>]?: string | object };
export default FieldsOf;
