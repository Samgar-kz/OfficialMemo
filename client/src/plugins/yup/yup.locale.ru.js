import printValue from "yup/lib/util/printValue";

export let mixed = {
    default: "Содержит ошибку",
    required: "Обязательное поле",
    defined: "${path} должен быть определен",
    oneOf: "Поле должено содержать одно из следующих значении: ${values}",
    notOneOf:
        "Поле не должено содержать ни одно из следующих значении: ${values}",
    notType: ({ path, type, value, originalValue }) => {
        let isCast = originalValue != null && originalValue !== value;
        let msg =
            `${path} должен быть \`${type}\` типом, ` +
            `но финальное значение: \`${printValue(value, true)}\`` +
            (isCast
                ? ` (приведено из значения \`${printValue(
                      originalValue,
                      true
                  )}\`).`
                : ".");

        if (value === null) {
            msg += `\n Если "null" является пустым значением, убедитесь что схема помечена как \`.nullable()\``;
        }

        return msg;
    },
    notNull: "${path} не может быть null",
};

export let string = {
    min: ({ min }) => {
        if (min === 1) return `Должно быть не менее ${min} символа`;
        return `Должен содержать не менее ${min} символов`;
    },
    max: ({ max }) => {
        if (max === 1) return `Должно быть не более ${max} символа`;
        return `Должно быть не более ${max} символов`;
    },
    length: "Длина должна быть ${length}",
    matches:
        '${path} должен совпадать со следующим регулярном выражением: "${regex}"',
    email: "Неккоректный Email",
    url: "значение должно быть валидной ссылкой",
    uuid: "значение должно быть валидными UUID",
    trim: "поле не должно содержать в начале или в конце пробелы",
    lowercase: "значение должно быть в нижним регистре",
    uppercase: "значение должно быть в верхнем регистре",
};

export let number = {
    min: "значение должно быть больше или равно ${min}",
    max: "значение должно быть меньше или равно ${max}",
    lessThan: "значение должно быть меньше чем ${less}",
    moreThan: "значение должно быть больше ${more}",
    notEqual: "значение не должно быть равно ${notEqual}",
    positive: "значение должно быть положительном числом",
    negative: "значение должно быть негативном числом",
    integer: "значение должно быть целым числом",
};

export let date = {
    min: "Дата не может быть меньше начальной",
    max: "Дата не может быть больше конечной",
};

export let boolean = {
    isValue: "должно иметь значение: ${value}",
};

export let object = {
    noUnknown:
        "${path} field cannot have keys not specified in the object shape",
};

export let array = {
    min: ({ min }) => {
        if (min === 1) return `Должно быть не менее ${min} элемента`;
        return `Должно быть не менее ${min} элементов`;
    },
    max: ({ max }) => {
        if (max === 1) return `Должно быть не более ${max} элемента`;
        return `Должно быть не более ${max} элементов`;
    },
    length: ({ length }) => {
        if (length === 1) return `Должно быть ровно ${length} элемент`;
        return `Должно быть ровно ${length} элементов`;
    },
};

export default Object.assign(Object.create(null), {
    mixed,
    string,
    number,
    date,
    object,
    array,
    boolean,
});
