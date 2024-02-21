
<template>
  <div class="tw-min-h-screen tw-grid tw-grid-rows-[64px_1fr_auto]">
    <div class="header">
      <slot name="header">
        <div class="tw-h-[64px] tw-bg-primary tw-flex tw-flex-row tw-items-center">
          <img src="@/assets/img/logo_inverted.svg" class="tw-h-[36px] tw-ml-4" />
        </div>
      </slot>
    </div>
    <div class="content">
      <slot></slot>
    </div>
    <slot name="rejectbutton"></slot>
    <div class="footer">
      <slot name="footer"></slot>
    </div>
  </div>
</template>