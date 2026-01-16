import Vue from 'vue'
import { BootstrapVue, IconsPlugin } from 'bootstrap-vue'
import VueCompositionAPI from '@vue/composition-api'
import VueSweetalert2 from 'vue-sweetalert2'
import Toast from 'vue-toastification'
import VueApexCharts from 'vue-apexcharts'

import 'bootstrap/dist/css/bootstrap.css'
import 'bootstrap-vue/dist/bootstrap-vue.css'
import 'sweetalert2/dist/sweetalert2.min.css'
import 'vue-toastification/dist/index.css'
import { BarChart2Icon, RefreshCwIcon } from 'vue-feather-icons'

import App from './App.vue'
import router from './router'
import store from './store'
import './libs/axios'
import './libs/validations'

Vue.component('BarChart2Icon', BarChart2Icon)
Vue.component('RefreshCwIcon', RefreshCwIcon)

Vue.use(VueCompositionAPI)
Vue.use(BootstrapVue)
Vue.use(IconsPlugin)
Vue.use(VueSweetalert2)
Vue.use(Toast)
Vue.use(VueApexCharts)
Vue.component('ApexChart', VueApexCharts)

Vue.config.productionTip = false

new Vue({
  router,
  store,
  render: h => h(App)
}).$mount('#app')
