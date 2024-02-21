/** @type {import('tailwindcss').Config} */
module.exports = {
  mode: "jit",
  content: ["./index.html", "./src/**/*.{vue,js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        primary: "#008b8a",
        secondary: "#f05e22",
        "primary-100": "#e6f3f3",
        "primary-150": "#ecf7f8",
        "primary-200": "#b3dcdc",
        error: "#b00020",
      },
    },
  },
  variants: {
    extend: {},
  },
  plugins: [
    function ({ addVariant }) {
      addVariant("child", "& > *");
      addVariant("child-hover", "& > *:hover");
    },
  ],
  prefix: "tw-",
  important: "body",
};
