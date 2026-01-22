<template>
  <div class="landing-page">
    <b-navbar toggleable="lg" type="dark" variant="dark" class="px-4 py-3 custom-navbar">
      <b-navbar-brand href="#" class="font-weight-bold">FalaBará</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>
        <b-nav-item-dropdown
          right
          no-caret
          class="notification-dropdown mr-3"
          @show="markNotificationsAsRead"
        >
          <template #button-content>
              <div class="position-relative">
                <bell-icon size="20" class="text-white" />
                <span v-if="unreadCount > 0" class="notification-badge">{{ unreadCount }}</span>
              </div>
          </template>

          <b-dropdown-header class="d-flex justify-content-between align-items-center px-3 py-2 border-bottom">
              <span class="font-weight-bold text-dark me-3">Notificações</span>
              <small class="text-muted cursor-pointer" @click="fetchNotifications">Atualizar</small>
          </b-dropdown-header>

          <div class="notification-list-container">
              <div v-if="notifications.length === 0" class="text-center p-3 text-muted">
                <small>Nenhuma notificação.</small>
              </div>

              <b-dropdown-item
                v-for="n in notifications"
                :key="n.id"
                class="border-bottom"
                style="min-width: 300px;"
              >
                <div class="d-flex flex-column py-2">
                    <div class="d-flex justify-content-between">
                      <strong :class="{ 'text-primary': !n.isRead, 'text-dark': n.isRead }">Sistema</strong>
                      <small class="text-muted">{{ formatDate(n.createdAt) }}</small>
                    </div>
                    <span class="text-secondary small mt-1 text-wrap" style="line-height: 1.4;">
                      {{ n.message }}
                    </span>
                </div>
              </b-dropdown-item>
          </div>
        </b-nav-item-dropdown>
        <b-navbar-nav class="w-100 d-flex align-items-center justify-content-between">
          
          <div class="d-none d-lg-block" style="flex: 1;"></div>

          <div v-if="isLoggedIn" class="navbar-center-text">
            <span class="text-white font-weight-medium">Olá, {{ userName }}</span>
          </div>
          
          
          <div v-else style="flex: 1;"></div>

          <div class="d-flex align-items-center justify-content-end" style="flex: 1;">
            <div v-if="isLoggedIn" class="d-flex align-items-center">
              <b-nav-item-dropdown
                right
                no-caret
                class="notification-dropdown mr-3"
                @show="markNotificationsAsRead"
              >
                </b-nav-item-dropdown>
              <b-button variant="outline-danger" size="b" @click="logout">Sair</b-button>
            </div>

            <div v-else>
              <b-button variant="outline-light me-3" :to="{ name: 'auth-login' }" class="mr-2">Entrar</b-button>
              <b-button variant="outline-light" :to="{ name: 'auth-register' }">Cadastrar</b-button>
            </div>
          </div>

        </b-navbar-nav>
      </b-collapse>
    </b-navbar>

    <div class="hero bg-sabara text-white text-center py-5">
      <b-container class="content-overlay">
        <h1 class="display-4 font-weight-bold">Transforme Sabará com sua voz</h1>
        <p class="lead mb-4">Relate problemas, acompanhe soluções e ajude a prefeitura a priorizar.</p>

        <b-button size="lg" variant="light" class="text-sabara font-weight-bold shadow p-3" @click="goToNewComplaint">
          <plus-circle-icon /> REGISTRAR RECLAMAÇÃO
        </b-button>
      </b-container>
    </div>

    <b-card class="w-75 mx-auto">
      <b-container class="py-5">
        <div>
          <h3 class="font-weight-bold mb-4 border-left-sabara p-2">Últimas Ocorrências</h3>
          <complaint-feed />
        </div>
        <hr class="my-5">
        <div>
          <h3 class="font-weight-bold border-left-sabara p-2">Transparência em Tempo Real</h3>
          <sabara-dashboard />
        </div>
      </b-container>
    </b-card>
  </div>
</template>

<script>
import axios from '@/libs/axios'
import SabaraDashboard from './SabaraDashboard.vue'
import ComplaintFeed from './ComplaintsFeed.vue'
import { PlusCircleIcon, BellIcon } from 'vue-feather-icons'

export default {
  name: 'LandingPage',
  components: { SabaraDashboard, ComplaintFeed, PlusCircleIcon, BellIcon },
  data () {
    return {
      isLoggedIn: false,
      userName: 'Cidadão',
      notifications: []
    }
  },
  computed: {
    unreadCount () {
      return this.notifications.filter(n => !n.isRead).length
    }
  },
  created () {
    const token = localStorage.getItem('token')
    const userData = localStorage.getItem('userData')

    this.isLoggedIn = !!token

    if (userData) {
      try {
        this.userName = JSON.parse(userData).nome || JSON.parse(userData).name || 'Cidadão'
      } catch (e) {}
    }

    if (this.isLoggedIn) {
      this.fetchNotifications()
    }
  },
  methods: {
    async fetchNotifications () {
      try {
        const token = localStorage.getItem('token')
        const config = { headers: { Authorization: `Bearer ${token}` } }
        const { data } = await axios.get('/api/notifications', config)
        this.notifications = data
      } catch (error) {
        console.error('Erro ao buscar notificações', error)
      }
    },

    async markNotificationsAsRead () {
      if (this.unreadCount === 0) return

      try {
        const token = localStorage.getItem('token')
        const config = { headers: { Authorization: `Bearer ${token}` } }

        await axios.post('/api/notifications/read-all', {}, config)

        // CORREÇÃO DO ESLINT (Adicionadas as chaves {})
        this.notifications.forEach(n => {
          n.isRead = true
        })
      } catch (error) {
        console.error('Erro ao marcar como lidas', error)
      }
    },

    goToNewComplaint () {
      if (this.isLoggedIn) {
        this.$router.push({ name: 'register-complaint' })
      } else {
        this.$swal({
          title: 'Identifique-se',
          text: 'Para registrar uma reclamação oficial, você precisa entrar ou se cadastrar.',
          icon: 'warning',
          showCancelButton: true,
          confirmButtonText: 'Entrar',
          cancelButtonText: 'Navegar pelo site'
        }).then((result) => {
          if (result.isConfirmed) this.$router.push({ name: 'auth-login' })
        })
      }
    },
    logout () {
      localStorage.removeItem('token')
      localStorage.removeItem('userData')
      this.isLoggedIn = false
      this.$toast.info('Até logo!')
      this.$router.push({ name: 'auth-login' })
    },
    formatDate (date) {
      if (!date) return ''
      const d = new Date(date)
      return `${d.getDate().toString().padStart(2, '0')}/${(d.getMonth() + 1).toString().padStart(2, '0')} às ${d.getHours()}:${d.getMinutes()}`
    }
  }
}
</script>

<style scoped>
.custom-navbar {
  min-height: 80px; 
}

@media (min-width: 992px) {
  .navbar-center-text {
    position: absolute;
    right: 3%;
    transform: translateX(-50%);
    white-space: nowrap;
    font-size: 1.1rem; 
  }
}

.notification-badge {
  position: absolute;
  right: 30px;
  background-color: #dc3545;
  color: white;
  border-radius: 50%;
  padding: 2px 6px;
  font-size: 10px;
  font-weight: bold;
}

.notification-list-container {
  max-height: 400px;
  overflow-y: auto;
}
.hero {
  
  background-image: linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), 
                    url("../../../public/Bairro-Pompeu.jpg");
  
  background-size: cover;      /* Faz a imagem cobrir todo o espaço */
  background-position: center; /* Centraliza a imagem */
  background-repeat: no-repeat;
  min-height: 540px;           /* Altura mínima para visualização */
  display: flex;
  align-items: center;         /* Centraliza o conteúdo verticalmente */
  justify-content: center;     /* Centraliza o conteúdo horizontalmente */
  position: relative;
}

.content-overlay {
  z-index: 2;                  
}
.img_inicio {
  height: 540px;
}
.bg-sabara { background-color: #8B0000; }
.text-sabara { color: white; 
background-color: #8B0000;
border: #8B0000;}
.border-left-sabara { border-left: 5px solid #8B0000; }
.notification-badge {
  position: absolute;
  top: -5px;
  right: -5px;
  background-color: #ea5455;
  color: white;
  font-size: 0.65rem;
  font-weight: bold;
  padding: 2px 5px;
  min-width: 18px;
  text-align: center;
  border-radius: 50%;
  border: 1px solid #343a40;
}
.notification-list-container {
  max-height: 300px;
  overflow-y: auto;
}
.cursor-pointer { 
  color: white !important;
  cursor: pointer; 
  border: solid, #8B0000;
  background-color: #8B0000;
  padding: 7px;
  border-radius: 5px;
}
.cursor-pointer:hover {
  background-color: white;
  color: black !important;
}
</style>
