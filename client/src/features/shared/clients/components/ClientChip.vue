<script setup lang="ts">
import type { Client } from "../types/Client";

defineProps<{
  model?: Client;
}>();
</script>

<template>
  <div>
    <p class="tw-font-bold">
      {{ model.name }} <span v-if="model.identityNumber">{{ model.identityNumber }} </span>
    </p>
    <p class="tw-text-sm" v-if="model.email || model.phone">{{ [model.email, model.phone].join(", ") }}</p>
    <p class="tw-text-sm" v-if="model.address">{{ model.address }}</p>
    <template v-if="model.clientType == 'entity'">
      <p class="tw-text-sm" v-if="model.member != null && model.hasEsedo">Есть подключение к ЕСЭДО</p>
      <p class="tw-text-sm" v-else>Нет подключения к ЕСЭДО</p>
    </template>
  </div>
</template>
