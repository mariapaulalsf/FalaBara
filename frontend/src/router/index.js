import Vue from 'vue'
import VueRouter from 'vue-router'
import authRoutes from './routes/auth' // <--- Importar as rotas que criamos

Vue.use(VueRouter)

const router = new VueRouter({
  mode: 'history',
  base: process.env.BASE_URL,
  routes: [
    ...authRoutes,
    {
      path: '/',
      name: 'landing', // Nome da rota
      component: () => import('@/views/main/Landing.vue')
      // meta: { requiresAuth: false } // Página pública
    },
    {
      path: '/home', // A Home real (Dashboard) será aqui depois do login
      name: 'home',
      component: () => import('@/views/main/Home.vue'),
      meta: { requiresAuth: true }
    }
  ]
})

export default router
