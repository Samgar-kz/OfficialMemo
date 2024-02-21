import { defineAsyncComponent } from "vue";
import type { Language } from "@/types/process/Language";

export interface MessageTemplate {
  name: string;
  value: string;
  printPage: (lang: Language) => any;
}

function getLangShort(lang: Language) {
  if (lang === "Русский") return "ru";
  if (lang === "Қазақша") return "kz";
}

const templates: MessageTemplate = {
  name: "Шаблон по умолчанию",
  value: "default",
  printPage: (lang: Language) =>
    defineAsyncComponent(
      () => import(`@/printPages/default/default_${getLangShort(lang)}.vue`)
    ),
};

export default templates;
