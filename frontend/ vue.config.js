module.exports = {
  devServer: {
    proxy: {
      '^/api': {
        target: 'http://localhost:5282', 
        changeOrigin: true,
        secure: false,
      },
    },
  },
}