import type { LabelValue } from "@/types/process/LabelValue";

const approvalTypes = [
  { title: "Параллельно", value: "parallel" } as LabelValue,
  { title: "Последовательно", value: "serial" } as LabelValue,
];

export default async function getApprovalTypes() {
  return Promise.resolve(approvalTypes);
}
