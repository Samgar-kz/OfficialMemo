<script setup lang="ts">
import getTask from "@/server/api/bpm/task/getTask";
import { onMounted, ref } from "vue";
import CardBlock from "@/components/CardBlock.vue";
import type { Task } from "@/types/task/Task";
import Card from "primevue/card";
import Divider from "primevue/divider";
import usePreviousTask from "@/composables/tasks/usePreviousTask";

const props = defineProps<{
  taskGuid: string;
  title: string;
  updateTask?: boolean;
}>();
const task = ref<Task>({} as Task);
const emit = defineEmits(["updateTask"]);
async function fetchData() {
  task.value = await getTask(props.taskGuid);
  emit("updateTask", task.value);
}

onMounted(async () => await fetchData());

const { previousTask } = usePreviousTask(task);

function replaceWithBr(str: string) {
  return str?.replace(/\\n/g, "<br />");
}
</script>

<template>
  <card-block>
    <template v-slot:header>
      <div class="tw-px-4">
        {{ title }}
      </div>
    </template>
    <card class="tw-rounded-none">
      <template #header>
        <div class="tw-flex tw-justify-between tw-px-4 tw-py-2">
          <span class="tw-text-base">{{ task.responsibleName }}</span>
          <slot name="replaceExecutor">
            <task-update-dialog
              v-if="updateTask"
              :task-guid="taskGuid"
              title="Поменять исполнителя"
              class="tw-bg-secondary"
              update-object="Executor"
              @update-task="fetchData"
            />
          </slot>
          <span
            class="tw-flex tw-items-center tw-justify-center tw-rounded-md tw-px-2 tw-text-center tw-text-xs tw-text-white"
            :class="{
              'bg-success':
                task.replyDecision == 'accept' ||
                task.replyDecision == 'approve',
              'bg-error':
                task.replyDecision == 'reject' ||
                task.replyDecision == 'rework',
            }"
            >{{ task.replyDecisionName }}</span
          >
        </div>
      </template>
      <template #content>
        <div class="tw-ml-4 tw-grid tw-grid-cols-[150px_1fr]">
          <Divider
            class="tw-col-span-2 tw-mb-1 divider-text tw-text-left"
            type="solid"
            ><b>Резолюция</b></Divider
          >
          <!-- <p class="tw-col-span-2 tw--ml-2 tw-mb-2">Резолюция:</p> -->

          <span class="tw-text-sm tw-font-semibold">Инициатор:</span>
          <span>{{ task.initiatorName }}</span>

          <span class="tw-text-sm tw-font-semibold">Дата резолюции:</span>
          <span>{{ new Date(task.taskDate).toLocaleString("kk-KZ") }}</span>

          <!-- <template v-if="task?.dueToDate"> -->
          <span class="tw-text-sm tw-font-semibold" v-if="task?.dueToDate"
            >Срок исполнения:</span
          >

          <div class="tw-flex">
            <span v-if="task?.dueToDate">{{
              new Date(task.dueToDate).toLocaleString("kk-KZ", {
                year: "numeric",
                month: "numeric",
                day: "numeric",
              })
            }}</span>
            <task-update-dialog
              v-if="updateTask"
              :default-due-to-date="new Date(task?.dueToDate)"
              :task-guid="taskGuid"
              update-object="DueToDate"
              @update-task="fetchData"
              title="Изменить срок"
              class="tw-bg-secondary"
            />
          </div>
          <!-- </template> -->

          <span class="tw-text-sm tw-font-semibold">Текст резолюции:</span>
          <span v-html="replaceWithBr(task.taskComment)"></span>

          <documents-list class="tw-col-span-2" :items="task.taskDocuments" />

          <template v-if="task?.replyDate">
            <Divider
              class="tw-col-span-2 tw-mt-5 tw-mb-1 divider-text tw-text-left"
              type="solid"
              ><b>Ответ</b></Divider
            >
            <!-- <v-divider class="tw-col-span-2 tw-my-4 tw--ml-4" /> -->
            <!-- <p class="tw-col-span-2 tw--ml-2 tw-mb-2">Ответ:</p> -->

            <span class="tw-text-sm tw-font-semibold">Исполнитель:</span>
            <span>{{ task.repliedByName }}</span>

            <span class="tw-text-sm tw-font-semibold">Дата исполнения:</span>
            <span>{{ new Date(task.replyDate).toLocaleString("kk-KZ") }}</span>

            <template v-if="task?.replyComment">
              <span class="tw-text-sm tw-font-semibold">Коментарий:</span>
              <span v-html="replaceWithBr(task.replyComment)"></span>
            </template>

            <documents-list
              class="tw-col-span-2"
              :items="task.replyDocuments"
            />
          </template>

          <template v-if="previousTask">
            <!-- <divider class="tw-col-span-2 tw-my-4 tw--ml-4" /> -->
            <Divider
              class="tw-col-span-2 tw-mt-5 tw-mb-1 divider-text tw-text-left"
              type="solid"
              ><b>Предыдущий ответ</b></Divider
            >

            <!-- <p class="tw-col-span-2 tw--ml-2 tw-mb-2">Предыдущий ответ:</p> -->

            <span class="tw-text-sm tw-font-semibold">Исполнитель:</span>
            <span>{{ previousTask.repliedByName }}</span>

            <span class="tw-text-sm tw-font-semibold">Дата исполнения:</span>
            <span>{{
              new Date(previousTask.replyDate).toLocaleString("kk-KZ")
            }}</span>

            <template v-if="previousTask.replyComment">
              <span class="tw-text-sm tw-font-semibold">Коментарий:</span>
              <span v-html="replaceWithBr(previousTask.replyComment)"></span>
            </template>

            <documents-list
              class="tw-col-span-2"
              :items="previousTask.replyDocuments"
            />

            <Divider
              class="tw-col-span-2 tw-mt-5 tw-mb-1 divider-text tw-text-left"
              type="solid"
              ><b>Ответ инициатора</b></Divider
            >
            <!-- <p class="tw-col-span-2 tw--ml-2 tw-mb-2">Ответ инициатора:</p> -->

            <span class="tw-text-sm tw-font-semibold">Исполнитель:</span>
            <span>{{ previousTask.approvedByName }}</span>

            <span class="tw-text-sm tw-font-semibold">Дата исполнения:</span>
            <span>{{
              new Date(previousTask.approvalDate).toLocaleString("kk-KZ")
            }}</span>

            <template v-if="previousTask.approvalComment">
              <span class="tw-text-sm tw-font-semibold">Коментарий:</span>
              <span v-html="replaceWithBr(previousTask.approvalComment)"></span>
            </template>

            <documents-list
              class="tw-col-span-2"
              :items="previousTask.approvalDocuments"
            />
          </template>
        </div>
      </template>

      <template #footer>
        <slot name="actions" />
      </template>
    </card>
  </card-block>
</template>
