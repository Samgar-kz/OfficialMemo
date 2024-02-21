import type { MyDocument } from "../contents/MyDocument";
import type { Employee } from "../employees/Employee";
import type { ApprovalResult } from "./ApprovalResult";
import type { ConfidenceType } from "./ConfidenceType";
import type { Language } from "./Language";
import type { ReceivingResult } from "./ReceivingResult";
import type { SignData } from "./SignData";

interface CoreData {
  messageGuid?: string;
  messageDate?: Date;
  executor?: Employee;
  initiator: string;
  language: Language;
  confidenceType: ConfidenceType;
  indexNomenclature?: string;
  subject: string;
  amountPage?: number;
  dueToDate: Date;
  approvers?: Employee[];
  approvalMode?: "parallel" | "serial";
  signer: Employee;
  signData?: SignData;
  recipients: Employee[];
  attachments?: MyDocument[];
  responsible?: Employee;
  details?: string;
  verticalText?: string;
  registerRegNum?: string;
  registerDate?: Date;
  regNum?: string;
  approvalResults?: ApprovalResult[];
  // receivingResults?: ReceivingResult[];
  signType?: string;
}
export { type CoreData };
