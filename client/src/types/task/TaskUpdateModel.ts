interface TaskUpdateModel {
  taskGuid: string;
  dueToDate?: Date;
  executor?: string;
  updateObject?: updateObject;
}
type updateObject = "DueToDate" | "Executor";

export type { TaskUpdateModel, updateObject };
