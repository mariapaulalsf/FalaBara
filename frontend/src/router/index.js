import Vue from 'vue'
import VueRouter from 'vue-router'
import authRoutes from './routes/auth' 

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    ...authRoutes,
    {
      path: '/',
      name: 'landing',
      component: () => import('@/views/main/SabaraDashboard.vue')
    },
    {
      path: '/home', 
      name: 'home',
      component: () => import('@/views/main/Home.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

export default router
