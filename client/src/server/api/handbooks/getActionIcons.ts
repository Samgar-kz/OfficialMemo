export interface Icon {
  name: string;
  actionName: string;
  color: string;
  hoverColor: string;
  animation?: string;
  inverse: boolean;
}

export const icons: Icon[] = [
  {
    name: "io-send-sharp",
    actionName: "Отправить | Далее",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "bi-play-circle",
    actionName: "Исполнить",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "fa-file-signature",
    actionName: "Подписать",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "bi-clipboard2-check",
    actionName: "Принять",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "bi-file-earmark-minus",
    actionName: "Отклонить",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "md-assignmentreturn-outlined",
    actionName: "На доработку",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "bi-file-earmark-check-fill",
    actionName: "Зарегистрировать",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "fa-file-signature ",
    actionName: "Подписать",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "bi-clipboard2-plus",
    actionName: "Создать резолюцию | Создать резолюцию в дополнение к текущему",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: true,
  },
  {
    name: "bi-file-earmark-check",
    actionName: "Согласовать",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "bi-arrow-left-right",
    actionName: "Определить другого исполнителя",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: true,
  },
  {
    name: "pr-search-plus",
    actionName: "Создать анализ",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "oi-circle-clash",
    actionName: "Отменить резолюцию ",
    color: "red",
    hoverColor: "red",
    inverse: false,
  },
  {
    name: "bi-backspace",
    actionName: "Отозвать | Назад",
    color: "var(--primary-color)",
    hoverColor: "var(--primary-color)",
    inverse: false,
  },
  {
    name: "fa-regular-save",
    actionName: "Сохранить",
    color: "var(--primary-color)",
    hoverColor: "var(--primary-color)",
    inverse: false,
  },
  {
    name: "bi-pencil-square",
    actionName: "Изменить",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
  {
    name: "hi-search",
    actionName: "Лупа",
    color: "var(--primary-color)",
    hoverColor: "#004645",
    inverse: false,
  },
];
export function getActionIcon<Icon>(label) {
  return icons.find((element) => element.actionName.includes(label));
}
