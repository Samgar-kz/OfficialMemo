export type AttachmentKind = "pdf" | "docx" | "img" | "template" | "unknown";

export type Attachment = {
  name: string;
  url?: string;
  kind?: AttachmentKind;
};
