import Vue from 'vue'
import axios from 'axios'

const axiosIns = axios.create({
  baseURL: '/api',
  timeout: 10000
})

Vue.prototype.$http = axiosIns

export default axiosIns
