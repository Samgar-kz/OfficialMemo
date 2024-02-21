import type { MyDocument } from "../contents/MyDocument";
import type { Employee } from "../employees/Employee";

interface SignData {
  signedTime: Date;
  signer: Employee;
  signType?: signType;
  signature?: string;
  signDocument?: MyDocument;
}

type signType = "Digital" | "HandWritten";

export type { SignData };
