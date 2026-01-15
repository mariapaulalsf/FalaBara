import Vue from 'vue'
import axios from 'axios'

const axiosIns = axios.create({
  baseURL: 'http://localhost:5282/api', // 👈 backend
  timeout: 10000
})

Vue.prototype.$http = axiosIns
export default axiosIns
