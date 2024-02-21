<script lang="ts" setup>
import Button from "primevue/button";
import getUserGroups from "@/server/api/bpm/users/getUserGroups";
import type { Employee } from "@/types/employees/Employee";
import { onMounted, ref } from "vue";

interface UserGroupMember {
  userName: string;
  userCode: string;
}

interface UserGroup {
  id: number;
  name: string;
  users: UserGroupMember[];
}

const emit = defineEmits(["updateExecutors"]);

const userGroups = ref<UserGroup[]>();
const execs = ref<Employee[]>([] as Employee[]);

async function fetchData() {
  userGroups.value = await getUserGroups();
}

function getUsersFromGroup(users: UserGroupMember[]) {
  execs.value = [] as Employee[];
  users.forEach((x) => {
    execs.value.push({ login: x.userCode, name: x.userName });
  });

  emit("updateExecutors", execs.value);
  console.log("EXecs", execs.value);
}

onMounted(async () => {
  await fetchData();
});
</script>

<template>
  <div v-if="userGroups?.length > 0">
    <h4 class="tw-font-bold">Мои группы</h4>
    <div class="cover tw-relative tw-w-full tw-flex tw-items-center">
      <ul class="group-list tw-flex tw-flex-nowrap tw-overflow-x-auto tw-pt-4">
        <li v-for="i in userGroups" class="tw-min-w-[100px] tw-w-auto tw-m-1">
          <button
            @click="getUsersFromGroup(i.users)"
            class="tw-w-full tw-rounded tw-bg-gray-200 tw-px-3 tw-py-1 tw-text-center hover:tw-bg-gray-300"
          >
            <span class="tw-w-full">{{ i.name }}</span>
          </button>
        </li>
      </ul>
    </div>
  </div>
</template>

<style>
.group-list {
  scroll-behavior: smooth;
}

/* .group-list::-webkit-scrollbar {
  -webkit-appearance: none;
} */
/* 
.left,
.right {
  position: absolute;
  top: 50%;
  transform: translateY(-50%);
}

.right {
  right: 0;
}

.left {
  left: 0;
} */
</style>
