import type { MyDocument } from "../contents/MyDocument";
import type { Employee } from "../employees/Employee";

interface ReceivingResultMessage {
  created: Date;
  executor: Employee;
  result: string & "accept";
  comment?: string;
  documents?: MyDocument[];
}

export type { ReceivingResultMessage };
