/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./Areas/**/*.cshtml",
    "./wwwroot/js/**/*.js",
    "./wwwroot/js/*.js",
    "./*.cshtml"
  ],
  theme: {
    extend: {
      colors: {
        canvasia: {
          ink: "#1B2A4A",
          forest: "#3A7CA5",
          "forest-deep": "#2C5E7A",
          gold: "#4A90B8",
          sand: "#FDF6E3",
          mist: "#F5E6C8",
          cream: "#FFFBF0"
        }
      },
      fontFamily: {
        heading: ["Playfair Display", "Georgia", "serif"],
        body: ["Source Sans 3", "sans-serif"]
      },
      boxShadow: {
        soft: "0 18px 45px rgba(27, 42, 74, 0.10)",
        panel: "0 24px 60px rgba(58, 124, 165, 0.08)"
      }
    }
  },
  plugins: []
};
