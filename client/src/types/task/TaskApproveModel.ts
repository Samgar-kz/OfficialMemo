import type { MyDocument } from "@/types/contents/MyDocument";

interface TaskApproveModel {
  taskGuid: string;
  approvalComment?: string;
  approvalDocuments: MyDocument[];
  appendReplyToParent: boolean;
  performAfterApprove?: boolean;
}

export type { TaskApproveModel };
