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
          forest: "#C0392B",
          "forest-deep": "#922B21",
          gold: "#E74C3C",
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
        panel: "0 24px 60px rgba(192, 57, 43, 0.08)"
      }
    }
  },
  plugins: []
};
