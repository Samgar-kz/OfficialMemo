<script setup lang="ts">
import CardBlock from "@/components/CardBlock.vue";
import Card from "primevue/card";

const props = withDefaults(
  defineProps<{
    title?: string;
  }>(),
  { title: "Доступные действия" }
);
</script>

<template>
  <card-block class="card_block">
    <template v-slot:header>
      <div
        class="actions_block__header tw-px-4 md:tw-text-sm lg:tw-text-sm xl:tw-text-base"
      >
        {{ title }}
      </div>
    </template>
    <card class="actions_block__body tw-rounded-none">
      <template v-slot:content>
        <slot />
      </template>
    </card>
  </card-block>
</template>

<style>
.card_block__header:has(> div.actions_block__header) {
  background-color: var(--secondary-color) !important;
  padding: 5px;
}

div.actions_block__body > div.p-card-body {
  padding: 0px !important;
}
</style>
