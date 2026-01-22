<template>
  <div class="landing-page">
    <b-navbar toggleable="lg" type="dark" variant="dark" class="px-4 py-3 custom-navbar">
      <b-navbar-brand href="#" class="font-weight-bold">FalaBará</b-navbar-brand>

      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>

      <b-collapse id="nav-collapse" is-nav>

        <b-navbar-nav class="ml-auto d-flex align-items-center justify-content-end w-100">

          <div v-if="isLoggedIn" class="d-flex align-items-center">

            <b-nav-item-dropdown no-caret class="notification-dropdown mr-3" @show="markNotificationsAsRead">
              <template #button-content>
                <div class="position-relative d-flex align-items-center">
                  <bell-icon size="20" class="text-white" />
                  <span v-if="unreadCount > 0" class="notification-badge">{{ unreadCount }}</span>
                </div>
              </template>

              <b-dropdown-header class="d-flex justify-content-between align-items-center px-3 py-2 border-bottom">
                <span class="font-weight-bold text-dark">Notificações</span>
                <small class="text-muted cursor-pointer" @click="fetchNotifications">Atualizar</small>
              </b-dropdown-header>

              <div class="notification-list-container">
                <div v-if="notifications.length === 0" class="text-center p-3 text-muted">
                  <small>Nenhuma notificação.</small>
                </div>

                <b-dropdown-item v-for="n in notifications" :key="n.id" class="border-bottom" style="min-width: 300px;">
                  <div class="d-flex flex-column py-2">
                    <div class="d-flex justify-content-between">
                      <strong :class="{ 'text-primary': !n.isRead, 'text-dark': n.isRead }">Sistema</strong>
                      <small class="text-muted">{{ formatDate(n.createdAt) }}</small>
                    </div>
                    <span class="text-secondary small mt-1 text-wrap" style="line-height: 1.4;">{{ n.message }}</span>
                  </div>
                </b-dropdown-item>
              </div>
            </b-nav-item-dropdown>

            <b-nav-item-dropdown no-caret class="user-dropdown">
              <template #button-content>
                <div class="d-flex align-items-center text-white">
                  <span class="font-weight-bold mr-2">{{ userName }}</span>

                  <div class="user-avatar">
                    <user-icon size="18" />
                  </div>
                </div>
              </template>

              <b-dropdown-item @click="openProfileModal">
                <user-icon size="16" class="mr-2" /> Meu Perfil
              </b-dropdown-item>
              <b-dropdown-divider></b-dropdown-divider>
              <b-dropdown-item @click="logout" variant="danger">
                <log-out-icon size="16" class="mr-2" /> Sair
              </b-dropdown-item>
            </b-nav-item-dropdown>

          </div>

          <div v-else class="d-flex align-items-center ml-auto auth-buttons">
            <b-button variant="outline-light" :to="{ name: 'auth-login' }" class="px-4">
              Entrar
            </b-button>

            <b-button variant="outline-light" :to="{ name: 'auth-register' }" class="px-4">
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

    <b-card class="w-75 mx-auto mt-3 border-0 shadow-sm">
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

    <b-modal id="profile-modal" title="Meu Perfil" hide-footer centered>
      <div class="text-center mb-4">
        <div
          class="avatar-large bg-light-sabara text-sabara mx-auto mb-2 d-flex align-items-center justify-content-center rounded-circle"
          style="width: 80px; height: 80px;">
          <user-icon size="40" />
        </div>
        <h5 class="font-weight-bold mb-0">{{ userName }}</h5>
        <span class="badge badge-secondary mt-1">{{ userType }}</span>
      </div>
      <b-form @submit.prevent="updateProfile">
        <b-form-group label="Nome Completo" label-for="profile-name">
          <b-form-input id="profile-name" v-model="profileForm.name" required></b-form-input>
        </b-form-group>
        <b-form-group class="mt-2" label="Telefone (WhatsApp)" label-for="profile-phone"
          description="Opcional. Usado para contato rápido.">
          <b-form-input id="profile-phone" v-model="profileForm.foneNumber"
            placeholder="(31) 99999-9999"></b-form-input>
        </b-form-group>
        <b-form-group class="mt-1" label="E-mail" label-for="profile-email">
          <b-form-input id="profile-email" :value="userEmail" disabled class="bg-light"></b-form-input>
          <small class="text-muted">O e-mail não pode ser alterado.</small>
        </b-form-group>
        <b-form-group class="mt-1" label="CPF" label-for="profile-cpf">
          <b-form-input id="profile-cpf" :value="userCpf" disabled class="bg-light"></b-form-input>
          <small class="text-muted">O CPF não pode ser alterado.</small>
        </b-form-group>
        <div class="d-flex justify-content-end align-items-center ml-auto auth-buttons mt-4">
          <b-button variant="secondary" class="mr-2" @click="$bvModal.hide('profile-modal')">
            Cancelar
          </b-button>

          <b-button type="submit" variant="success" :disabled="profileLoading">
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
import { PlusCircleIcon, BellIcon, UserIcon, LogOutIcon } from 'vue-feather-icons'
import { BModal } from 'bootstrap-vue'

export default {
  name: 'LandingPage',
  components: { SabaraDashboard, ComplaintFeed, PlusCircleIcon, BellIcon, UserIcon, LogOutIcon, BModal },
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
            console.warn('Dados de usuário incompletos. Forçando logout.')
            this.logout()
            return
          }

          this.userName = userData.nome || userData.name || 'Cidadão'
          this.userEmail = userData.email || ''
          this.userCpf = userData.cpf || ''
          this.userType = userData.role || ''

          this.profileForm.name = this.userName
        } catch (e) {
          console.error('Erro ao ler dados do usuário', e)
          this.logout()
        }
      }
    },

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

        this.notifications.forEach(n => {
          n.isRead = true
        })
      } catch (error) {
        console.error('Erro ao marcar como lidas', error)
      }
    },

    async openProfileModal () {
      try {
        const userDataStr = localStorage.getItem('userData')
        if (!userDataStr) {
          this.$toast.error('Sessão expirada. Faça login novamente.')
          this.logout()
          return
        }

        const userData = JSON.parse(userDataStr)
        const userId = userData.id || userData.Id
        if (!userId) {
          this.$toast.warning('Seus dados de sessão estão incompletos. Por favor, entre novamente.')
          this.logout()
          return
        }

        const token = localStorage.getItem('token')

        const { data } = await axios.get(`/users/${userId}`, {
          headers: { Authorization: `Bearer ${token}` }
        })

        this.profileForm.name = data.name
        this.profileForm.foneNumber = data.foneNumber || ''
        this.userCpf = data.cpf
        this.userEmail = data.email

        this.$bvModal.show('profile-modal')
      } catch (error) {
        console.error(error)
        if (error.response && error.response.status === 404) {
          this.$toast.error('Usuário não encontrado no sistema.')
        } else {
          this.$toast.error('Erro ao carregar perfil.')
        }
      }
    },

    async updateProfile () {
      this.profileLoading = true
      try {
        const token = localStorage.getItem('token')
        const payload = {
          Nome: this.profileForm.name,
          FoneNumber: this.profileForm.foneNumber
        }

        await axios.put('/users/update', payload, {
          headers: { Authorization: `Bearer ${token}` }
        })

        this.$toast.success('Perfil atualizado com sucesso!')

        this.userName = this.profileForm.name

        const userData = JSON.parse(localStorage.getItem('userData'))
        userData.nome = this.profileForm.name
        userData.name = this.profileForm.name
        localStorage.setItem('userData', JSON.stringify(userData))

        this.$bvModal.hide('profile-modal')
      } catch (error) {
        this.$toast.error('Erro ao atualizar perfil.')
      } finally {
        this.profileLoading = false
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

.user-avatar {
  width: 35px;
  height: 35px;
  background-color: rgba(255, 255, 255, 0.2);
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  border: 1px solid rgba(255, 255, 255, 0.3);
}

.notification-dropdown .dropdown-menu,
.user-dropdown .dropdown-menu {
  right: 0;
  left: auto;
}

.bg-light-sabara {
  background-color: #fce8e8;
}

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

.bg-sabara {
  background-color: #8B0000;
}

.text-sabara {
  color: white;
  background-color: #8B0000;
  border: #8B0000;
}

.border-left-sabara {
  border-left: 5px solid #8B0000;
}

.auth-buttons {
  margin-left: auto;
  display: flex;
  gap: 12px;
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
