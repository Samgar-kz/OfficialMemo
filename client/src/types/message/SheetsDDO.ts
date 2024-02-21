import type { Document } from "@/types/contents/MyDocument";

interface OfficialMemo {
  documentId: string;
  document: Document;
  regNum: string;
  sheetName: string;
  period: string;
  fromDate: Date;
  toDate: Date;
  writeDate: Date;
}

export type { OfficialMemo };
