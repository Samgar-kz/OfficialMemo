import "./assets/css/main.scss";
import "./assets/css/reset.css";

import { addIcons, OhVueIcon } from "oh-vue-icons";
import {
  BiBackspace,
  BiArrowLeftRight,
  BiClipboard2Check,
  BiClipboard2Plus,
  BiFileEarmarkCheck,
  BiFileEarmarkCheckFill,
  BiFileEarmarkMinus,
  BiPlayCircle,
  BiXCircle,
  BiPencilSquare,
} from "oh-vue-icons/icons/bi";
import {
  IoSendSharp,
  IoReloadCircle,
  HiSearch,
  MdWarningamberRound,
} from "oh-vue-icons/icons";
import {
  BiCircleFill,
  BiClockHistory,
  BiPauseCircleFill,
  BiPlayCircleFill,
  FaFileSignature,
  FaRegularTimesCircle,
  MdAssignmentreturnOutlined,
  MdPausecircleoutlineSharp,
  OiCircleSlash,
  PrSearchPlus,
  RiNodeTree,
} from "./assets/icons/icons";

import ruLocale from "@/plugins/prime-vue/ru-RU";
import { FaRegularSave } from "oh-vue-icons/icons/fa";
import Button from "primevue/button";
import PrimeVue from "primevue/config";
import ConfirmationService from "primevue/confirmationservice";
import DialogService from "primevue/dialogservice";
import Tooltip from "primevue/tooltip";
import { createApp } from "vue";
import App from "./App.vue";
import router from "./router/index";
import ToastService from "primevue/toastservice";
import Toast from "primevue/toast";
import Editor from "primevue/editor";

addIcons(
  BiBackspace,
  BiPauseCircleFill,
  BiPlayCircle,
  MdAssignmentreturnOutlined,
  FaFileSignature,
  BiArrowLeftRight,
  BiXCircle,
  IoSendSharp,
  IoReloadCircle,
  OiCircleSlash,
  BiFileEarmarkCheckFill,
  PrSearchPlus,
  BiClipboard2Plus,
  BiClipboard2Check,
  BiFileEarmarkMinus,
  BiFileEarmarkCheck,
  BiPencilSquare,
  FaRegularTimesCircle,
  BiCircleFill,
  BiPlayCircleFill,
  BiClockHistory,
  RiNodeTree,
  MdPausecircleoutlineSharp,
  FaRegularSave,
  HiSearch,
  MdWarningamberRound
);
import("primevue/resources/themes/lara-light-blue/theme.css"); //theme css
import("./assets/css/themes/_lara-light.scss");
import("primevue/resources/primevue.min.css"); //core css
import("primeicons/primeicons.css"); //icons

import("@/plugins/configureVeeValidateRules");

import "vue-loading-overlay/dist/css/index.css";

const app = createApp(App)
  .use(router)
  .use(PrimeVue, {
    locale: ruLocale,
  })
  .use(DialogService)
  .use(ConfirmationService)
  .use(ToastService);

app.directive("tooltip", Tooltip);
// eslint-disable-next-line vue/multi-word-component-names, vue/no-reserved-component-names
app.component("Button", Button);
app.component("Toast", Toast);
app.component("Editor", Editor);

app.component("v-icon", OhVueIcon);
app.mount("#app");
