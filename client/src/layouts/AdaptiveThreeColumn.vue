<script setup lang="ts">
import { v4 as uuidv4 } from "uuid";
import { breakpointsTailwind, useBreakpoints } from "@vueuse/core";

const breakpoints = useBreakpoints(breakpointsTailwind);
const isGreaterThan2Xl = breakpoints.greater("2xl");

const id = uuidv4();
const leftAsideId = `leftAside_${id}`;
const rightAsideId = `rightAside_${id}`;
</script>

<template>
  <div
    class="tw-grid tw-grid-cols-[min-content] tw-justify-center tw-gap-4 tw-px-4 tw-py-4 xl:tw-grid-cols-[1fr_auto] 2xl:tw-grid-cols-[4fr_auto_5fr]"
  >
    <div class="tw-flex tw-flex-col tw-gap-4" :id="leftAsideId">
      <slot name="left-aside" />
    </div>
    <div class="tw-flex tw-flex-col tw-gap-4"><slot /></div>
    <div class="tw-flex tw-flex-col tw-gap-4" :id="rightAsideId"></div>
  </div>
  <Teleport :to="`#${isGreaterThan2Xl ? rightAsideId : leftAsideId}`"
    ><slot name="right-aside"
  /></Teleport>
</template>
