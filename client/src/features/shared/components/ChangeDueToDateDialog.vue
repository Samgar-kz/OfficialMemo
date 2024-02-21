<script setup lang="ts">
import { useForm } from "vee-validate";
import { inject, onMounted, ref } from "vue";

const dialogRef = inject<{
  value: { data: { messageId: string; dueToDate: Date }; close: () => void };
}>("dialogRef");
const messageId = ref("");
const dueToDate = ref<Date>();

const emit = defineEmits(["changeDueToDate", "info"]);

onMounted(async () => {
  const data = dialogRef.value.data;
  messageId.value = data.messageId;
  dueToDate.value = new Date(data.dueToDate);
});

const validationSchema = {
  dueToDate: "required|notBeforeTodayInclusive",
};
const { validate } = useForm({
  validationSchema: validationSchema,
  keepValuesOnUnmount: true,
});

async function submit() {
  const { valid } = await validate();
  if (!valid) return;
  emit("changeDueToDate", {
    messageId: messageId.value,
    dueToDate: dueToDate.value,
  });
}
</script>

<template>
  <v-form
    :schema="validationSchema"
    class="tw-flex tw-w-[400px] tw-flex-col tw-gap-2"
  >
    <v-datetime-picker
      name="dueToDate"
      label="Срок исполнения"
      v-model="dueToDate"
      :min-date="new Date()"
      class="tw-col-span-4"
    />
    <Button @click="submit" class="tw-self-end">Применить</Button>
  </v-form>
</template>
