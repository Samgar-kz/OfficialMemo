<script setup lang="ts">
import type { DefaultTemplateAdditionalData } from ".";
import type { CoreData } from "@/types/process/CoreData";
import type { Model } from "@/types/process/Model";

const props = withDefaults(
  defineProps<{
    model: Model<DefaultTemplateAdditionalData>;
  }>(),
  {
    model: () => ({
      data: {} as CoreData,
    }),
  }
);
const logoUrl = import.meta.env.VITE_BASE_URL + "/app_icon.svg";
</script>

<template>
  <div class="tw-min-h-[271mm]">
    <div
      class="times-new-roman tw-gird tw-m-[10mm_15mm_20mm_15mm] tw-grid-cols-2 tw-text-black"
      id="main_content"
    >
      <div class="times-new-roman tw-text-black tw-pb-4">
        <div class="content-top-header">
          <p class="tw-leading-7 w-40percent"></p>
          <div class="tw-mx-auto tw-block">
            <img src="https://bpm2d/img/app_icon.svg" style="height: 80px" />
          </div>

          <p class="w-40percent tw-text-right" style="text-align: right">
            <b>"{{ model?.data?.confidenceType.displayTextKz }}"</b>
          </p>
        </div>
        <p
          class="tw-text-[1.2rem] tw-leading-7"
          style="text-align: center; margin-top: 2rem"
        >
          <b> ҚЫЗМЕТТІК ЖАЗБА</b>
        </p>
        <div class="content-header">
          <table id="content-table">
            <tr>
              <td><b>Кімге:</b></td>
              <td class="padding-left-5" style="text-align: justify">
                <span
                  v-for="(recipient, index) in model?.data?.recipients"
                  :key="index"
                >
                  {{ recipient.name
                  }}<span v-if="index + 1 != model?.data?.recipients?.length"
                    >,</span
                  >&nbsp;
                </span>
              </td>
            </tr>
            <tr>
              <td><b>Кімнен:</b></td>
              <td class="padding-left-5" style="text-align: justify">
                {{ model?.data?.signer?.name }}
              </td>
            </tr>
            <tr>
              <td><b>Тақырып:</b></td>
              <td class="padding-left-5" style="text-align: justify">
                {{ model?.data?.subject }}
              </td>
            </tr>
          </table>
          <!-- <div><b>Кому:</b></div>
          <div class="underline">
            <span
              v-for="(recipient, index) in model?.data?.recipients"
              :key="index"
            >
              {{ recipient.name
              }}<span v-if="index + 1 != model?.data?.recipients.length">,</span
              >&nbsp;
            </span>
          </div>
          <div><b>От:</b></div>
          <div class="underline">
            {{ model?.data?.signer.name }}
          </div>
          <div><b>Тема:</b></div>
          <div class="underline">
            {{ model?.data?.subject }}
          </div> -->
        </div>
      </div>
      <div
        class="ql-editor"
        style="padding: 0; text-align: justify"
        v-html="model.data?.details"
      ></div>
      <!-- <div
        class="vertical-text tw-absolute tw-right-6 tw-top-[300px] tw-ml-4 tw-text-[0.7rem]"
      >
        {{
          model.data?.signData?.signType !== "HandWritten"
            ? model.data?.verticalText
            : ""
        }}
      </div> -->
      <p>&emsp;</p>
      <div class="tw-col-span-full tw-mt-8 content-position">
        <p class="tw-max-w-[250px]">
          <b>{{ model?.data?.signer?.positionKz }}</b>
        </p>

        <p class="tw-text-right">
          <b>{{ model?.data?.signer?.name }}</b>
        </p>
      </div>
      <p>&emsp;</p>
      <div v-if="model?.data?.attachments" class="tw-pt-8">
        <p class="tw-pb-1"><b>Тіркелген файлдар:</b></p>
        <span
          v-for="(attachment, index) in model?.data?.attachments"
          :key="index"
        >
          {{ attachment.name
          }}<span v-if="index + 1 != model?.data?.attachments.length">,</span
          >&nbsp;
        </span>
      </div>
      <p>&emsp;</p>
      <p class="tw-mb--1 tw-pt-8">
        <i
          ><small>Орынд.: {{ model?.data?.executor?.name }}</small></i
        >
      </p>
      <i
        ><small>Тел.: {{ model?.data?.executor?.localPhone }}</small></i
      >
      <br />
      <i
        ><small>{{ model?.data?.executor?.email }}</small></i
      >
      <!-- <p>&emsp;</p> -->
      <!-- <div class="tw-mt-4 tw-text-sm">
        <p v-if="model?.data?.signData">
          &#10004;
          {{
            model.data.signData?.signType === "HandWritten" ? "ЭЦҚ-сыз қ" : "Қ"
          }}ол қойылды
          <span class="tw-italic">
            {{ model.data?.signData?.signer?.name }}
          </span>
          {{
            new Date(model.data?.signData?.signedTime).toLocaleString("kk-KZ")
          }}
        </p>
        <p
          v-for="(approvalResult, index) in model?.data?.approvalResults"
          :key="index"
          v-show="approvalResult.result === 'approve'"
        >
          &#10004; Келісілді
          <span class="tw-italic"> {{ approvalResult?.approver?.name }} </span>
          {{ new Date(approvalResult?.created).toLocaleString("kk-KZ") }}
        </p>
      </div> -->
    </div>
  </div>
</template>
