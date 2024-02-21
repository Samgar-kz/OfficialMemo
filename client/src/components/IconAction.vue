<template>
  <slot class="class">
    <span
      class="hover-list"
      @click="emit('click')"
      @mouseover="iconColor = icon.hoverColor"
      @mouseout="iconColor = icon.color"
    >
      <v-icon
        v-if="icon.name"
        :name="icon.name"
        :fill="iconColor"
        :inverse="icon.inverse"
        :animation="icon.animation"
      />
      {{ label }}
      <slot name="secondIcon"></slot>
    </span>
    <slot name="secondaryAction"
  /></slot>
</template>

<script setup lang="ts">
import { ref, onMounted } from "vue";
import { getActionIcon } from "@/server/api/handbooks/getActionIcons";
import type { Icon } from "@/server/api/handbooks/getActionIcons";
const props = withDefaults(
  defineProps<{
    label: string;
    class: string;
  }>(),
  {}
);
const emit = defineEmits(["click"]);
const iconColor = ref<string>();

const icon = ref<Icon>({} as Icon);
onMounted(() => {
  icon.value = getActionIcon(props.label);
  iconColor.value = icon.value.color;
});
</script>
