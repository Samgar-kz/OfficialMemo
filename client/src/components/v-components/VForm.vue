<script setup lang="ts">
import { provide } from "vue";

const props = defineProps<{
  schema: Object;
}>();

provide("validationSchema", props.schema);
</script>

<template>
  <!-- <v-form> -->
  <div>
    <slot />
  </div>
  <!-- </v-form> -->
</template>
