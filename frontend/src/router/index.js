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
      component: () => import('@/views/main/Landing.vue')
    },
    {
      path: '/nova-reclamacao',
      name: 'register-complaint',
      component: () => import('@/views/main/RegisterComplaint.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/home',
      name: 'home',
      redirect: '/'
    }
  ]
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next({ name: 'auth-login' }) 
  } else {
    next()
  }
})

export default router
