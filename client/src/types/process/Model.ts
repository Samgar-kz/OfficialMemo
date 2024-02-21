import type { CoreData } from "./CoreData";

interface Model<T> {
  data: CoreData;
  processGuid?: string;
  htmlDocument?: string;
  documentUrl?: string;
  originalDocumentUrl?: string;
}

export type { Model };
