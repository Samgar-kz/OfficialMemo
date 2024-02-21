import type { Language } from "@/types/process/Language";

const langs: Language[] = ["Қазақша", "Русский"];

export default async function getLanguages() {
  return Promise.resolve(langs);
}
