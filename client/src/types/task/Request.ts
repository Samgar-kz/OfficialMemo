import type { MyDocument } from "@/types/contents/MyDocument";

interface Request {
  dueToDate?: Date;
  taskName?: string;
  taskDate: Date;
  taskComment?: string;
  taskDocuments: MyDocument[];
}

export type { Request };
