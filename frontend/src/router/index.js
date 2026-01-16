import Vue from 'vue'
import VueRouter from 'vue-router'
import authRoutes from './routes/auth' // Suponho que aqui tenham as rotas /login e /register

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    ...authRoutes, // Garanta que aqui dentro tem: { path: '/register', name: 'auth-register', ... }
    {
      path: '/',
      name: 'landing',
      // Aponta para a nova Landing Page que vamos criar abaixo
      component: () => import('@/views/main/Landing.vue')
    },
    {
      path: '/nova-reclamacao',
      name: 'register-complaint',
      // O formulário de registro de reclamação que você já tem ou vamos ajustar
      component: () => import('@/views/main/RegisterComplaint.vue'),
      meta: { requiresAuth: true }
    },
    {
      path: '/home',
      name: 'home',
      redirect: '/' // Podemos redirecionar home para landing se quiser unificar
    }
  ]
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem('token')
  if (to.meta.requiresAuth && !token) {
    next({ name: 'auth-login' }) // Redireciona para login se tentar acessar rota protegida
  } else {
    next()
  }
})

export default router
