<template>
  <div class="landing-page modern-ui">
    <b-navbar toggleable="lg" type="dark" variant="dark" class="px-4 py-3 custom-navbar glass-navbar">
      <b-navbar-brand href="#" class="font-weight-bold brand-logo">FalaBará</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav class="ml-auto d-flex align-items-center justify-content-end w-100">

          <div v-if="isLoggedIn" class="d-flex align-items-center">

            <b-nav-item-dropdown no-caret right menu-class="notification-menu shadow-lg border-0 rounded-lg" class="notification-dropdown mr-3" @show="markNotificationsAsRead">
              <template #button-content>
                <div class="icon-wrapper position-relative d-flex align-items-center justify-content-center">
                  <bell-icon size="20" class="text-white" />
                  <span v-if="unreadCount > 0" class="notification-badge">{{ unreadCount }}</span>
                </div>
              </template>

              <b-dropdown-header class="d-flex justify-content-between align-items-center px-3 py-3 border-bottom bg-white rounded-top">
                <span class="font-weight-bold text-dark h6 mb-0">Notificações</span>
              </b-dropdown-header>

              <div class="notification-list-container custom-scrollbar">
                <div v-if="notifications.length === 0" class="text-center p-4 text-muted">
                  <small>Nenhuma notificação recente.</small>
                </div>

                <b-dropdown-item v-for="n in notifications" :key="n.id" class="notification-item border-bottom">
                  <div class="d-flex flex-column py-2">
                    <div class="d-flex justify-content-between align-items-center mb-1">
                      <strong :class="{ 'text-primary': !n.isRead, 'text-dark': n.isRead }" class="notification-title">
                        <span v-if="!n.isRead" class="dot-unread"></span> Sistema
                      </strong>
                      <small class="text-muted" style="font-size: 0.70rem;">{{ formatDate(n.createdAt) }}</small>
                    </div>
                    <span class="text-secondary small mt-1 text-wrap notification-msg">{{ n.message }}</span>
                  </div>
                </b-dropdown-item>
              </div>
            </b-nav-item-dropdown>

            <b-nav-item-dropdown no-caret class="user-dropdown" menu-class="shadow-lg border-0 rounded-lg mt-2">
              <template #button-content>
                <div class="d-flex align-items-center text-white user-btn">
                  <span class="font-weight-bold mr-2 d-none d-md-inline">{{ userName }}</span>
                  <div class="user-avatar shadow-sm">
                    <user-icon size="18" />
                  </div>
                </div>
              </template>

              <b-dropdown-item @click="openProfileModal" class="py-2">
                <user-icon size="16" class="mr-2 text-muted" /> Meu Perfil
              </b-dropdown-item>
              <b-dropdown-divider></b-dropdown-divider>
              <b-dropdown-item @click="logout" class="py-2 text-danger">
                <log-out-icon size="16" class="mr-2" /> Sair
              </b-dropdown-item>
            </b-nav-item-dropdown>

          </div>

          <div v-else class="d-flex align-items-center ml-auto auth-buttons">
            <b-button variant="outline-light" :to="{ name: 'auth-login' }" class="px-4 btn-semi-rounded font-weight-bold mr-3">
              Entrar
            </b-button>
            <b-button variant="light" :to="{ name: 'auth-register' }" class="px-4 btn-semi-rounded font-weight-bold text-sabara">
              Cadastrar
            </b-button>
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

    <div class="main-content-wrapper py-5">
      <b-card class="mt-1 w-75 mx-auto border-0 shadow-card rounded-xl bg-white main-card">
        <b-container class="py-4">

          <div class="section-header mb-4">
            <h3 class="font-weight-bold text-dark">Últimas Ocorrências</h3>
            <div class="header-line"></div>
          </div>

          <complaint-feed />

          <hr class="my-5 border-light">

          <div class="section-header mb-4">
            <h3 class="font-weight-bold text-dark">Transparência em Tempo Real</h3>
            <div class="header-line"></div>
          </div>

          <sabara-dashboard />

        </b-container>
      </b-card>
    </div>

    <b-modal id="profile-modal" hide-footer centered content-class="shadow-lg border-0 rounded-lg">
      <template #modal-header="{ close }">
        <h5 class="font-weight-bold mb-0 text-dark">Meu Perfil</h5>
        <div class="close-btn-wrapper" @click="close" v-b-tooltip.hover title="Fechar">
          <x-icon size="20" class="text-muted" />
        </div>
      </template>

      <div class="text-center mb-4">
        <div class="avatar-large bg-light-sabara text-sabara mx-auto mb-3 d-flex align-items-center justify-content-center rounded-circle shadow-sm" style="width: 90px; height: 90px;">
          <user-icon size="42" />
        </div>
        <h5 class="font-weight-bold mb-0 text-dark">{{ userName }}</h5>
        <span v-if="userType != 1" class="badge badge-light-primary mt-2 px-3 py-1">{{ userType }}</span>
      </div>

      <b-form @submit.prevent="updateProfile">
        <b-form-group label="Nome Completo" label-for="profile-name" class="font-weight-bold text-muted small text-uppercase">
          <b-form-input id="profile-name" v-model="profileForm.name" required class="input-modern"></b-form-input>
        </b-form-group>

        <b-form-group label="Telefone (WhatsApp)" label-for="profile-phone" description="Opcional. Usado para contato rápido." class="mt-3 font-weight-bold text-muted small text-uppercase">
          <b-form-input id="profile-phone" v-model="profileForm.foneNumber" placeholder="(31) 99999-9999" class="input-modern"></b-form-input>
        </b-form-group>

        <b-row>
          <b-col md="6">
            <b-form-group label="E-mail" label-for="profile-email" class="mt-3 font-weight-bold text-muted small text-uppercase">
              <b-form-input id="profile-email" :value="userEmail" disabled class="bg-light border-0"></b-form-input>
            </b-form-group>
          </b-col>
          <b-col md="6">
            <b-form-group label="CPF" label-for="profile-cpf" class="mt-3 font-weight-bold text-muted small text-uppercase">
              <b-form-input id="profile-cpf" :value="userCpf" disabled class="bg-light border-0"></b-form-input>
            </b-form-group>
          </b-col>
        </b-row>

        <div class="d-flex justify-content-end align-items-center mt-4 pt-3 border-top modal-footer-actions">
          <b-button variant="light" class="font-weight-bold text-muted btn-semi-rounded" @click="$bvModal.hide('profile-modal')">
            Cancelar
          </b-button>
          <b-button type="submit" variant="success" class="px-4 font-weight-bold shadow-sm btn-semi-rounded" :disabled="profileLoading">
            <b-spinner small v-if="profileLoading" class="mr-1"></b-spinner>
            Salvar Alterações
          </b-button>
        </div>
      </b-form>
    </b-modal>

  </div>
</template>

<script>
import axios from '@/libs/axios'
import SabaraDashboard from './SabaraDashboard.vue'
import ComplaintFeed from './ComplaintsFeed.vue'
import { PlusCircleIcon, BellIcon, UserIcon, LogOutIcon, XIcon } from 'vue-feather-icons'
import { BModal } from 'bootstrap-vue'

export default {
  name: 'LandingPage',
  components: { SabaraDashboard, ComplaintFeed, PlusCircleIcon, BellIcon, UserIcon, LogOutIcon, XIcon, BModal },
  data () {
    return {
      isLoggedIn: false,
      userName: 'Cidadão',
      userEmail: '',
      userCpf: '',
      userType: '',
      notifications: [],
      profileLoading: false,
      profileForm: {
        name: '',
        foneNumber: ''
      }
    }
  },
  computed: {
    unreadCount () {
      return this.notifications.filter(n => !n.isRead).length
    }
  },
  created () {
    const token = localStorage.getItem('token')
    this.isLoggedIn = !!token
    if (this.isLoggedIn) {
      this.loadUserData()
      this.fetchNotifications()
    }
  },
  methods: {
    loadUserData () {
      const userDataStr = localStorage.getItem('userData')
      if (userDataStr) {
        try {
          const userData = JSON.parse(userDataStr)
          if (!userData.id && !userData.Id) {
            this.logout()
            return
          }
          this.userName = userData.nome || userData.name || 'Cidadão'
          this.userEmail = userData.email || ''
          this.userCpf = userData.cpf || ''
          this.userType = userData.role || ''
          this.profileForm.name = this.userName
        } catch (e) {
          this.logout()
        }
      }
    },
    async fetchNotifications () {
      try {
        const token = localStorage.getItem('token')
        const config = { headers: { Authorization: `Bearer ${token}` } }
        const { data } = await axios.get('/notifications', config)
        this.notifications = data
      } catch (error) {
        console.error(error)
      }
    },
    async markNotificationsAsRead () {
      if (this.unreadCount === 0) return
      try {
        const token = localStorage.getItem('token')
        const config = { headers: { Authorization: `Bearer ${token}` } }
        await axios.post('/notifications/read-all', {}, config)
        this.notifications.forEach(n => { n.isRead = true })
      } catch (error) { console.error(error) }
    },
    async openProfileModal () {
      try {
        const userDataStr = localStorage.getItem('userData')
        if (!userDataStr) { this.logout(); return }
        const userData = JSON.parse(userDataStr)
        const userId = userData.id || userData.Id
        const token = localStorage.getItem('token')
        const { data } = await axios.get(`/users/${userId}`, { headers: { Authorization: `Bearer ${token}` } })

        this.profileForm.name = data.name
        this.profileForm.foneNumber = data.foneNumber || ''
        this.userCpf = data.cpf
        this.userEmail = data.email
        this.$bvModal.show('profile-modal')
      } catch (error) {
        this.$toast.error('Erro ao carregar perfil.')
      }
    },
    async updateProfile () {
      this.profileLoading = true
      try {
        const token = localStorage.getItem('token')
        const payload = { Nome: this.profileForm.name, FoneNumber: this.profileForm.foneNumber }
        await axios.put('/users/update', payload, { headers: { Authorization: `Bearer ${token}` } })

        this.$toast.success('Perfil atualizado!')
        this.userName = this.profileForm.name
        const userData = JSON.parse(localStorage.getItem('userData'))
        userData.nome = this.profileForm.name
        localStorage.setItem('userData', JSON.stringify(userData))
        this.$bvModal.hide('profile-modal')
      } catch (error) { this.$toast.error('Erro ao atualizar.') } finally { this.profileLoading = false }
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
          cancelButtonText: 'Navegar'
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
@import url('https://fonts.googleapis.com/css2?family=Inter:wght@300;400;500;600;700&display=swap');

.modern-ui {
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
  background-color: #f8f9fa;
  min-height: 100vh;
}

.custom-navbar {
  min-height: 80px;
  background-color: #1a1a1a !important;
  box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}
.brand-logo {
  font-size: 1.5rem;
  letter-spacing: -0.5px;
}

.notification-menu {
  width: 380px;
  max-width: 90vw;
  padding: 0;
  overflow: hidden;
}

.notification-list-container {
  max-height: 300px;
  overflow-y: auto;
  scrollbar-width: thin;
  scrollbar-color: #ccc #f1f1f1;
}
.notification-list-container::-webkit-scrollbar {
  width: 6px;
}
.notification-list-container::-webkit-scrollbar-thumb {
  background-color: #ccc;
  border-radius: 4px;
}

.notification-item {
  cursor: pointer;
  transition: background-color 0.2s;
}
.notification-item:hover {
  background-color: #f8f9fa;
}
.dot-unread {
  height: 8px;
  width: 8px;
  background-color: #8B0000;
  border-radius: 50%;
  display: inline-block;
  margin-right: 6px;
}
.refresh-btn {
  padding: 6px;
  border-radius: 50%;
  transition: background-color 0.2s;
}
.refresh-btn:hover {
  background-color: #f0f0f0;
}
.hover-spin:hover {
  animation: spin 1s linear infinite;
}
@keyframes spin { 100% { transform: rotate(360deg); } }

.icon-wrapper {
  width: 40px;
  height: 40px;
  border-radius: 50%;
  background-color: rgba(255,255,255,0.1);
  transition: background 0.2s;
}
.icon-wrapper:hover {
  background-color: rgba(255,255,255,0.2);
}
.user-avatar {
  width: 38px;
  height: 38px;
  background-color: #2d2d2d;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 2px solid rgba(255, 255, 255, 0.1);
  transition: transform 0.2s;
}
.user-btn:hover .user-avatar {
  transform: scale(1.05);
  border-color: #8B0000;
}

::v-deep .dropdown-menu {
  border: none !important;
  box-shadow: 0 10px 30px rgba(0,0,0,0.15) !important;
  border-radius: 12px !important;
}

.btn-semi-rounded {
  border-radius: 8px !important;
}
.modal-footer-actions {
  gap: 12px;
}
.auth-buttons {
  margin-left: auto;
  display: flex;
  gap: 20px;
}
.hero {
  background-image: linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)),
                    url("../../../public/Bairro-Pompeu.jpg");
  background-size: cover;
  background-position: center;
  background-repeat: no-repeat;
  min-height: 540px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}
.content-overlay {
  z-index: 2;
}
.bg-sabara { background-color: #8B0000; }
.text-sabara { color: white;
background-color: #8B0000;
border: #8B0000;}
.btn-cta {
  border-radius: 0.25rem !important;
}

.main-content-wrapper {
  margin-top: -30px;
  position: relative;
  z-index: 3;
  background-color: #272424;
  padding-bottom: 5rem;
}
.shadow-card {
  box-shadow: 0 15px 35px rgba(0,0,0,0.08) !important;
}
.rounded-xl {
  border-radius: 16px !important;
}
.section-header {
  display: flex;
  align-items: center;
  gap: 15px;
}
.header-line {
  flex-grow: 1;
  height: 2px;
  background: linear-gradient(90deg, #8B0000 0%, transparent 100%);
  opacity: 0.2;
}

.bg-light-sabara { background-color: #fff0f0; }
.badge-light-primary {
  background-color: #e3f2fd;
  color: #1565c0;
  font-weight: 600;
  border-radius: 6px;
}

.input-modern {
  border: 2px solid #eee;
  border-radius: 8px;
  padding: 10px 15px;
  height: auto;
  transition: border-color 0.2s;
}
.input-modern:focus {
  border-color: #8B0000;
  box-shadow: none;
}

.notification-badge {
  position: absolute;
  top: -5px; right: -5px;
  background-color: #ea5455;
  color: white;
  font-size: 0.65rem;
  font-weight: bold;
  padding: 2px 5px;
  min-width: 18px;
  text-align: center;
  border-radius: 50%;
  border: 2px solid #343a40;
}
.close-btn-wrapper {
  width: 36px;
  height: 36px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s ease;
}

.close-btn-wrapper:hover {
  background-color: #f1f3f5;
  transform: rotate(90deg);
}

.close-btn-wrapper:hover svg {
  color: #dc3545 !important;
}

.cursor-pointer { cursor: pointer; }
</style>
