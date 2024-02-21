import type { Employee } from "@/types/employees/Employee";
import type { MyDocument } from "@/types/contents/MyDocument";

interface TaskCreateModel {
  requestGuid: string;
  parentGuid?: string;
  regNum?: string;
  processGuid?: string;
  previousTaskGuid?: string;
  executors: Employee[];
  executorCode: string;
  approvalRequired?: boolean;
  initiator?: string;
  approverSelf?: boolean;
  approver?: string;
  taskComment: string;
  documents: MyDocument[];
  data?: string;
  stepName?: string;
  dueToDate?: Date;
}

export type { TaskCreateModel };
