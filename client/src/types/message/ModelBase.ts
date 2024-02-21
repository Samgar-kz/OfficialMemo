import type { Register } from "@/types/message/Register";
import type { Executor } from "@/types/message/Executor";
import type { OfficialMemo } from "@/types/message/OfficialMemo";
import type { User } from "@/types/message/User";
import type { Sys } from "@/types/message/Sys";
import type { Approvers } from "@/types/message/Approvers";

interface ModelBase {
  register: Register;
  approvers: Approvers;
  signer: Executor;
  OfficialMemo: OfficialMemo;
  user: User;
  sys: Sys;
}

export type { ModelBase };
