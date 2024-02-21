import type { Employee } from "@/types/employees/Employee";
import type { MyDocument } from "./contents/MyDocument";

type ReplyDecision = "accept" | "approve" | "rework" | "redirect" | "reject";

interface Reply {
  requestGuid: string;

  repliedBy: Employee;
  replyDate: Date;

  replyDecision: ReplyDecision;
  replyDecisionName: string;
  replyComment: string;
  replyDocuments: MyDocument[];
}

interface Redirect extends Reply {
  redirectTo: Employee;
}

export type { Reply, Redirect, ReplyDecision };
