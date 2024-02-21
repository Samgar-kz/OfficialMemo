import type { AttachmentKind } from "./types";

export default function getDocumentKind(name: string): AttachmentKind {
  name = name.toLowerCase();
  if (name.endsWith("pdf")) return "pdf";
  if (name.endsWith("docx")) return "docx";
  if (name.endsWith("png")) return "img";
  if (name.endsWith("jpg")) return "img";

  return "unknown";
}
