import type { MyDocument } from "../contents/MyDocument";
import type { Employee } from "../employees/Employee";

interface ApprovalResult {
  created: Date;
  approver: Employee;
  executor: Employee;
  result: string & ("approve" | "rework" | "reject" | "accept");
  comment?: string;
  documents?: MyDocument[];
}

export type { ApprovalResult };
