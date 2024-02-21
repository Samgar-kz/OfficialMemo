import deleteFile from "@/server/api/contents/delete";
import uploadFile from "@/server/api/contents/upload";
import type { MyDocument } from "@/types/contents/MyDocument";
import { useFileDialog } from "@vueuse/core";
import { watch, type Ref, ref } from "vue";

const { files, open, reset } = useFileDialog();
var documents: Ref<MyDocument[]>;

const errorMessage = ref<string>();

watch(files, async () => {
  if (!files.value) return;

  const filesArray = Array.from(files.value);

  const fileAlreadyExistsNames = filesArray
    .filter((file) =>
      documents.value?.some(
        (val) => val.name === file.name && val.size === file.size
      )
    )
    .map((file) => file.name);

  if (fileAlreadyExistsNames.length > 0) {
    errorMessage.value =
      (fileAlreadyExistsNames.length > 1 ? "Файлы" : "Файл") +
      ` уже существует: ${fileAlreadyExistsNames}`;
    return;
  }

  const data = await uploadFile(files.value);

  if (documents.value) documents.value = documents.value.concat(data);
  else documents.value = data;
  reset();
});

export default function useContents(list: Ref<MyDocument[]>) {
  documents = list;
  const removeFile = async (file: MyDocument) => {
    if (!file.url) return;

    for (let i = 0; i < list.value.length; i++) {
      if (list.value[i].url === file.url) {
        list.value.splice(i, 1);
        break;
      }
    }
    try {
      await deleteFile(file.url);
    } catch {
      errorMessage.value = "Не удалось удалить файл на сервере!";
    }
  };

  return { removeFile, open, errorMessage };
}
