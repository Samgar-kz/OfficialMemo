import type { MyDocument } from "@/types/contents/MyDocument";

interface Task {
  taskGuid: string;
  parentGuid?: string;
  previousTaskGuid?: string;
  processGuid: string;
  requestGuid: string;
  regNum?: string;
  approvalRequired?: boolean;
  taskType?: string;
  dueToDate?: Date;
  initiator: string;
  initiatorName: string;
  responsible: string;
  responsibleName: string;
  repliedBy?: string;
  repliedByName?: string;
  approver?: string;
  approverName: string;
  approvedBy?: string;
  approvedByName?: string;
  taskName?: string;
  taskStatus?: string;
  taskDate: Date;
  taskComment?: string;
  taskDocuments: MyDocument[];
  replyDate?: Date;
  replyDecision?: string;
  replyDecisionName?: string;
  replyComment?: string;
  replyDocuments?: MyDocument[];
  approvalDate?: Date;
  approvalDecision?: string;
  approvalDecisionName?: string;
  approvalComment?: string;
  approvalDocuments?: MyDocument[];
  data?: string;
  meta?: string;
}

export type { Task };
