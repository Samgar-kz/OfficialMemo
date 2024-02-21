import type { MyDocument } from "../contents/MyDocument";
import type { Employee } from "../employees/Employee";
import type { ReceivingResultMessage } from "./ReceivingResultMessage";

interface ReceivingResult {
  created: Date;
  receiver: Employee;
  executor: Employee;
  result: string & "accept";
  comment?: string;
  documents?: MyDocument[];
  children?: ReceivingResult[];
  childCount?: number;
}

export type { ReceivingResult };
