/** @type {import('tailwindcss').Config} */
const plugin = require("tailwindcss/plugin")
const colors = require("tailwindcss/colors")

module.exports = {
  content: [
    "./components/**/*.{html,js,razor,cshtml}",
    "./wwwroot/**/*.{html,js}",
  ],
  darkMode: "class",
  theme: {
    extend: {
      keyframes: {
        "jump-more": {
          "0%, 100%": {
            transform: "scale(100%) rotate(0deg)",
          },
          "10%": {
            transform: "scale(120%) rotate(180deg)",
          },
          "50%": {
            transform: "scale(200%) rotate(360deg)",
          },
        },
      },
      animation: {
        "jump-more": "jump-more 3s infinite",
      },
    },
    colors: {
      ...colors,

      primary: "#3b5998",
      secondary: "#4285F4",
    },
  },
  plugins: [
    plugin(function ({ addVariant }) {
      addVariant("ios", `:is(.ios &)`)
      addVariant("android", `:is(.android &)`)
      addVariant("windows", `:is(.windows &)`)
    }),
    plugin(function ({ addComponents, theme }) {}),
  ],

    safelist: [
      "text-xl",
    "bg-gray-900/50",
    "list-decimal",
    "list-disc",
    "overscroll-contain",
    "z-[55]",
    "z-[60]",
    "animate-fade",

    "text-sm",
    "text-white",
    "text-black",
    "text-gray-400",
    "!text-primary",
    "text-red-400",
    "text-orange-400",
    "text-yellow-400",
    "text-lime-400",
    "text-green-400",
    "text-teal-400",
    "text-sky-400",
    "text-blue-400",
    "text-indigo-400",
    "text-purple-400",
    "text-pink-400",
    "text-rose-400",

    "bg-white",
    "bg-black",
    "bg-gray-400",
    "bg-red-400",
    "bg-orange-400",
    "bg-yellow-400",
    "bg-lime-400",
    "bg-green-400",
    "bg-teal-400",
    "bg-sky-400",
    "bg-blue-400",
    "bg-indigo-400",
    "bg-purple-400",
    "bg-pink-400",
    "bg-rose-400",

    "border-white",
    "border-black",
    "border-gray-400",
    "border-red-400",
    "border-orange-400",
    "border-yellow-400",
    "border-lime-400",
    "border-green-400",
    "border-teal-400",
    "border-sky-400",
    "border-blue-400",
    "border-indigo-400",
    "border-purple-400",
    "border-pink-400",
    "border-rose-400",

    "w-[24px]",
    "w-[28px]",
    "w-[32px]",
    "w-[36px]",
    "w-[40px]",
    "w-[44px]",
    "w-[48px]",
    "h-[24px]",
    "h-[28px]",
    "h-[32px]",
    "h-[36px]",
    "h-[40px]",
    "h-[44px]",
    "h-[48px]",

    "overflow-hidden",
    "max-w-3xl",

    "me-4",
    "right-2",
  ],
}
