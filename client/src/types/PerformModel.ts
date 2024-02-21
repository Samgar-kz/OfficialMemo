import type { MyDocument } from "@/types/contents/MyDocument";

interface PerformModel {
  guid: string;
  replyDecision?: string;
  replyDecisionName?: string;
  replyComment?: string;
  replyDocuments: MyDocument[];
  meta?: string;
  employeeCode: string;
  executorCode?: string;
  isRoot?: boolean;
}

export type { PerformModel };
