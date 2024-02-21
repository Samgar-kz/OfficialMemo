import { configure, defineRule } from "vee-validate";
import {
  required,
  email,
  min,
  length,
  numeric,
  digits,
} from "@vee-validate/rules";
import { localize, setLocale } from "@vee-validate/i18n";
import ru from "@vee-validate/i18n/dist/locale/ru.json";
import type { Employee } from "@/types/message/Employee";

defineRule("required", required);
defineRule("email", email);
defineRule("length", length);
defineRule("numeric", numeric);
defineRule("digits", digits);
defineRule("min", min);

defineRule("phone", (value: string) => {
  const regex =
    /^$|^(\+?\d)[ ]?(\(?\d{3}\)?)[ ]?(\d{3})[ -]?(\d{2}[ -]?\d{2})$/m;
  if (!value || regex.test(value)) return true;
  return "Неккоректный номер телефона";
});

const notBeforeTodayErrorMessage =
  "Значение поля не должно быть прошедшей датой";
defineRule("notBeforeToday", (value: Date) => {
  if (value > new Date()) return true;
  return notBeforeTodayErrorMessage;
});

defineRule("notBeforeTodayInclusive", (value: Date) => {
  if (value >= new Date()) return true;
  return notBeforeTodayErrorMessage;
});

defineRule(
  "required_if",
  (value: any, [condition]: [boolean]) => !condition || value
);

configure({
  generateMessage: localize({
    ru,
  }),
});

setLocale("ru");
