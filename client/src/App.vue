<script setup lang="ts">
import DynamicDialog from "primevue/dynamicdialog";
import OConfimDialogService from "./features/confirmationService/OConfimDialogService.vue";
</script>
<template>
  <div>
    <router-view />
    <DynamicDialog />
    <OConfimDialogService />
    <Toast />
    <ScrollTop />
  </div>
</template>
