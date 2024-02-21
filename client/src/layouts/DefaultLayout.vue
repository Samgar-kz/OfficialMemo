<template>
  <div
    class="default_layout tw-min-h-screen tw-grid tw-grid-cols-[1fr_auto_1fr] tw-gap-4 tw-pt-[30px] tw-pb-[40px] tw-px-4">
    <div>
      <slot name="left-side" />
    </div>
    <div class="default_layout__center">
      <slot />
    </div>
    <div>
      <slot name="right-side" />
    </div>
  </div>
</template>
