import type { MyDocument } from "@/types/contents/MyDocument";

interface ProcessMessage {
  processGuid: string;
  messageGuid?: string;
  requestGuid?: string;
  parentGuid?: string;
  taskGuid?: string;
  previousTaskGuid?: string;

  messageType?: string;
  stepName?: string;
  regNum?: string;
  childCount: number;
  messageDate: Date;
  messageComment?: string;
  messageStatus?: string;
  messageStatusName?: string;
  messageDocuments?: MyDocument[];
  initiatorCode: string;
  initiatorName: string;

  executorCode: string;
  executorName: string;

  replyDate?: Date;
  responseRecieved: boolean;
  userCode?: string;
  userName?: string;
  replyDecision?: string;
  replyDecisionName?: string;
  replyComment?: string;
  replyDocuments?: MyDocument[];

  approvalRequired?: boolean;
  approved?: boolean;
  approverCode?: string;
  approverName?: string;
  approvalDate?: Date;
  approvalDecision?: string;
  approvalDecisionName?: string;
  approvalComment?: string;
  approvalDocuments?: MyDocument[];

  dueToDate?: Date;

  children?: Array<ProcessMessage>;
  // data?: string;
}

export type { ProcessMessage };
