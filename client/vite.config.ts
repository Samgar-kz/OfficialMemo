import { defineConfig, loadEnv } from "vite";

import vue from "@vitejs/plugin-vue";
import path from "path";
import AutoImport from "unplugin-auto-import/vite";
import { PrimeVueResolver } from "unplugin-vue-components/resolvers";
import Components from "unplugin-vue-components/vite";
import Icons from "unplugin-icons/vite";
import IconsResolver from "unplugin-icons/resolver";

export default defineConfig(({ mode }) => {
  // https://vitejs.dev/config/
  const env = loadEnv(mode, process.cwd(), "");
  return {
    plugins: [
      vue(),
      Icons({
        autoInstall: true,
      }),
      AutoImport({
        include: [
          /\.[tj]sx?$/, // .ts, .tsx, .js, .jsx
          /\.vue$/,
          /\.vue\?vue/, // .vue
        ],
        imports: ["vue", "vue-router"],
        eslintrc: {
          enabled: false, // Default `false`
          filepath: "./.eslintrc-auto-import.json", // Default `./.eslintrc-auto-import.json`
          globalsPropValue: true, // Default `true`, (true | false | 'readonly' | 'readable' | 'writable' | 'writeable')
        },
      }),
      Components({
        dts: true,
        resolvers: [
          PrimeVueResolver(),
          IconsResolver({
            prefix: "i",
          }),
        ],
        dirs: [
          "./src/components",
          "./src/features/**",
          "./src/features/shared/components",
          "./src/layouts/**",
          "./src/printPages/default",
        ],
      }),
    ],
    resolve: {
      alias: {
        "@": path.resolve(__dirname, "./src"),
      },
    },
    base: env.BASE_URL,
    server: {
      host: "localhost",
    },
    optimizeDeps: {
      exclude: ["oh-vue-icons/icons"],
    },
    css: {
      preprocessorOptions: {
        scss: {
          additionalData: `@import "@/assets/fonts/stylesheet.scss";`,
        },
      },
    },
  };
});
