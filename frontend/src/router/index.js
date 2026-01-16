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
    },
    {
      path: "/admin",
      component: AdminDashboard,
      meta: { requiresAuth: true, role: "Admin" }
    }

  ]
})

router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");
  if (to.meta.requiresAuth && !token) next("/login");
  else next();
});


export default router
