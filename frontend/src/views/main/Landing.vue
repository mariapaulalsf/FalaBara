<template>
  <div class="landing-page modern-ui">
    <b-navbar toggleable="lg" type="dark" variant="dark" class="px-4 py-3 custom-navbar glass-navbar">
      <b-navbar-brand href="#" class="font-weight-bold brand-logo">FalaBará</b-navbar-brand>
      <b-navbar-toggle target="nav-collapse"></b-navbar-toggle>
      <b-collapse id="nav-collapse" is-nav>
        <b-navbar-nav class="ml-auto d-flex align-items-center justify-content-end w-100">
          <div v-if="isLoggedIn" class="d-flex align-items-center gap-3">
            <b-nav-item-dropdown no-caret right menu-class="notification-menu shadow-lg border-0 rounded-lg"
              class="notification-dropdown mr-3" @show="markNotificationsAsRead">
              <template #button-content>
                <div class="icon-wrapper position-relative d-flex align-items-center justify-content-center">
                  <bell-icon size="20" class="text-white" />
                  <span v-if="unreadCount > 0" class="notification-badge">{{ unreadCount }}</span>
                </div>
              </template>
              <b-dropdown-header
                class="d-flex justify-content-between align-items-center px-3 py-3 border-bottom bg-white rounded-top">
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

            <b-nav-item-dropdown no-caret class="user-dropdown" menu-class="shadow-lg border-0 rounded-lg mt-2 me-4">
              <template #button-content>
                <div class="d-flex align-items-center text-white user-btn">
                  <span class="font-weight-bold mr-2 d-none d-md-inline">{{ userName }}</span>
                  <div class="user-avatar shadow-sm">
                    <user-icon size="18" />
                  </div>
                </div>
              </template>
              <b-dropdown-item @click="openProfileModal" class="py-2 "><user-icon size="16" class="mr-2 text-muted" />
                Meu
                Perfil</b-dropdown-item>
              <b-dropdown-divider></b-dropdown-divider>
              <b-dropdown-item @click="logout" class="py-2 text-danger"><log-out-icon size="16" class="mr-2" />
                Sair</b-dropdown-item>
            </b-nav-item-dropdown>
          </div>
          <div v-else class="d-flex align-items-center ml-auto auth-buttons gap-3">
            <b-button variant="outline-light" :to="{ name: 'auth-login' }"
              class="px-4 btn-semi-rounded font-weight-bold mr-3">Entrar</b-button>
            <b-button variant="light" :to="{ name: 'auth-register' }"
              class="px-4 btn-semi-rounded font-weight-bold text-sabara">Cadastrar</b-button>
          </div>
        </b-navbar-nav>
      </b-collapse>
    </b-navbar>

    <div class="hero bg-sabara text-white text-center py-5">
      <b-container class="content-overlay">
        <h1 class="display-4 font-weight-bold">Transforme Sabará com sua voz</h1>
        <p class="lead mb-4">Relate problemas, acompanhe soluções e ajude a prefeitura a priorizar.</p>
        <b-button size="lg" variant="danger" class="btn-cta-red font-weight-bold shadow p-3" @click="goToNewComplaint">
          <plus-circle-icon class="mr-2" /> REGISTRAR RECLAMAÇÃO
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

        <div v-if="userType == 2 || userType == 'CityHall' || userType == 'Prefeitura'" class="px-4 pb-4">
          <hr class="my-5 border-light">

          <div class="d-flex justify-content-between align-items-center mb-4 flex-wrap">
            <div class="section-header mb-2 mb-md-0">
              <h3 class="font-weight-bold text-dark">Gestão de Usuários</h3>
            </div>
            <b-button variant="success" class="font-weight-bold shadow-sm btn-semi-rounded"
              @click="openCreateUserModal">
              <plus-circle-icon size="16" class="mr-2" /> Novo Usuário
            </b-button>
          </div>

          <b-card no-body class="mb-4 shadow-sm border-0 bg-light rounded-lg">
            <div class="p-2">
              <b-row align-v="center" class="mx-0">
                <b-col md="8" class="pl-0 pr-md-2 mb-2 mb-md-0">
                  <div class="search-wrapper position-relative">
                    <search-icon size="18" class="search-icon-input" />
                    <b-form-input v-model="filters.search" placeholder="Pesquisar por nome, email ou CPF..."
                      class="input-clean bg-white border-0 shadow-sm" @input="handleSearchInput" />
                  </div>
                </b-col>

                <b-col md="4" class="pr-0 pl-md-2">
                  <b-form-select v-model="filters.type" :options="filterRoleOptions"
                    class="input-filter-select shadow-sm" @change="fetchUsers" />
                </b-col>
              </b-row>
            </div>
          </b-card>

          <b-card no-body class="border-0 shadow-sm rounded-lg overflow-hidden">
            <b-table responsive hover striped :items="users" :fields="tableColumns" primary-key="id" show-empty
              empty-text="Nenhum usuário encontrado." head-variant="white" class="align-middle mb-0 user-table"
              label-sort-asc="" label-sort-desc="" label-sort-clear="">
              <template #cell(type)="data">
                <span class="badge px-3 py-2" :class="getRoleBadgeClass(data.value)">
                  {{ getRoleName(data.value) }}
                </span>
              </template>

              <template #cell(foneNumber)="data">
                <span v-if="data.value" class="text-dark font-weight-500">{{ formatPhone(data.value) }}</span>
                <span v-else class="text-muted small">Não informado</span>
              </template>

              <template #cell(department)="data">
                <span v-if="data.value" class="text-dark font-weight-bold small">{{ data.value }}</span>
                <span v-else class="text-muted">-</span>
              </template>
            </b-table>

            <div v-if="tableLoading" class="text-center py-5">
              <b-spinner variant="danger" label="Carregando..."></b-spinner>
            </div>
          </b-card>
        </div>

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
        <div
          class="avatar-large bg-light-sabara text-sabara mx-auto mb-3 d-flex align-items-center justify-content-center rounded-circle shadow-sm"
          style="width: 90px; height: 90px;">
          <user-icon size="42" />
        </div>
        <h5 class="font-weight-bold mb-0 text-dark">{{ userName }}</h5>
      </div>
      <b-form @submit.prevent="updateProfile">
        <b-form-group label="Nome Completo" label-for="profile-name"
          class="font-weight-bold text-muted small text-uppercase">
          <b-form-input id="profile-name" v-model="profileForm.name" required class="input-modern"></b-form-input>
        </b-form-group>
        <b-form-group label="Telefone (WhatsApp)" label-for="profile-phone"
          description="Opcional. Usado para contato rápido."
          class="mt-3 font-weight-bold text-muted small text-uppercase">
          <b-form-input id="profile-phone" v-model="profileForm.foneNumber" placeholder="(31) 99999-9999"
            class="input-modern"></b-form-input>
        </b-form-group>
        <b-row>
          <b-col md="6"><b-form-group label="E-mail"
              class="mt-3 font-weight-bold text-muted small text-uppercase"><b-form-input id="profile-email"
                :value="userEmail" readonly class="input-disabled"></b-form-input></b-form-group></b-col>
          <b-col md="6"><b-form-group label="CPF"
              class="mt-3 font-weight-bold text-muted small text-uppercase"><b-form-input id="profile-cpf"
                :value="userCpf" readonly class="input-disabled"></b-form-input></b-form-group></b-col>
        </b-row>
        <div class="d-flex justify-content-end align-items-center mt-4 pt-3 border-top modal-footer-actions gap-2">
          <b-button variant="light" class="font-weight-bold text-muted btn-semi-rounded mr-2"
            @click="$bvModal.hide('profile-modal')">Cancelar</b-button>
          <b-button type="submit" variant="success" class="px-4 font-weight-bold shadow-sm btn-semi-rounded"
            :disabled="profileLoading"><b-spinner small v-if="profileLoading" class="mr-1"></b-spinner> Salvar
            Alterações</b-button>
        </div>
      </b-form>
    </b-modal>

    <b-modal id="create-user-modal" hide-footer centered content-class="shadow-lg border-0 rounded-lg">
      <template #modal-header="{ close }">
        <h5 class="font-weight-bold mb-0 text-dark">Cadastrar Novo Usuário</h5>
        <div class="close-btn-wrapper" @click="close" v-b-tooltip.hover title="Fechar">
          <x-icon size="20" class="text-muted" />
        </div>
      </template>
      <b-form @submit.prevent="createUser">
        <b-row><b-col md="12"><b-form-group label="Nome Completo"
              class="font-weight-bold text-muted small text-uppercase"><b-form-input v-model="createUserForm.name"
                required class="input-modern"></b-form-input></b-form-group></b-col></b-row>
        <b-row class="mt-3">
          <b-col md="6"><b-form-group label="CPF" class="font-weight-bold text-muted small text-uppercase"><b-form-input
                v-model="createUserForm.cpf" required class="input-modern"
                placeholder="Apenas números"></b-form-input></b-form-group></b-col>
          <b-col md="6"><b-form-group label="Telefone"
              class="font-weight-bold text-muted small text-uppercase"><b-form-input v-model="createUserForm.phone"
                class="input-modern" placeholder="Apenas números"></b-form-input></b-form-group></b-col>
        </b-row>
        <b-row class="mt-3"><b-col md="12"><b-form-group label="E-mail"
              class="font-weight-bold text-muted small text-uppercase"><b-form-input type="email"
                v-model="createUserForm.email" required
                class="input-modern"></b-form-input></b-form-group></b-col></b-row>
        <b-row class="mt-3">
          <b-col md="6"><b-form-group label="Senha"
              class="font-weight-bold text-muted small text-uppercase"><b-form-input type="password"
                v-model="createUserForm.password" required class="input-modern"></b-form-input></b-form-group></b-col>
          <b-col md="6"><b-form-group label="Tipo de Perfil"
              class="font-weight-bold text-muted small text-uppercase"><b-form-select v-model="createUserForm.role"
                :options="roleOptions" class="input-modern form-control"></b-form-select></b-form-group></b-col>
        </b-row>
        <b-row class="mt-3" v-if="createUserForm.role === 2"><b-col md="12"><b-form-group label="Departamento"
              class="font-weight-bold text-muted small text-uppercase text-sabara"><b-form-select
                v-model="createUserForm.department" :options="departmentOptions" class="input-modern form-control"
                required></b-form-select></b-form-group></b-col></b-row>
        <div class="d-flex justify-content-end align-items-center mt-4 pt-3 border-top modal-footer-actions gap-2">
          <b-button variant="light" class="font-weight-bold text-muted btn-semi-rounded mr-2"
            @click="$bvModal.hide('create-user-modal')">Cancelar</b-button>
          <b-button type="submit" variant="success" class="px-4 font-weight-bold shadow-sm btn-semi-rounded"
            :disabled="createLoading"><b-spinner small v-if="createLoading" class="mr-1"></b-spinner>
            Cadastrar</b-button>
        </div>
      </b-form>
    </b-modal>

  </div>
</template>

<script>
import axios from '@/libs/axios'
import SabaraDashboard from './SabaraDashboard.vue'
import ComplaintFeed from './ComplaintsFeed.vue'
import { PlusCircleIcon, BellIcon, UserIcon, LogOutIcon, XIcon, SearchIcon } from 'vue-feather-icons'
import { BModal } from 'bootstrap-vue'

export default {
  name: 'LandingPage',
  components: { SabaraDashboard, ComplaintFeed, PlusCircleIcon, BellIcon, UserIcon, LogOutIcon, XIcon, SearchIcon, BModal },
  data () {
    return {
      isLoggedIn: false,
      userName: 'Cidadão',
      userEmail: '',
      userCpf: '',
      userType: '',
      notifications: [],
      profileLoading: false,
      profileForm: { name: '', foneNumber: '' },

      // --- TABELA ---
      tableLoading: false,
      searchTimer: null,
      tableColumns: [
        { key: 'name', label: 'Nome', sortable: true },
        { key: 'email', label: 'E-mail', sortable: true },
        { key: 'type', label: 'Perfil', sortable: true, class: 'text-center' },
        { key: 'foneNumber', label: 'Telefone', sortable: false },
        { key: 'department', label: 'Departamento', sortable: false }
      ],
      users: [],
      filters: {
        search: '',
        type: null
      },
      filterRoleOptions: [
        { value: null, text: 'Todos os Perfis' },
        { value: 1, text: 'Cidadão' },
        { value: 2, text: 'Prefeitura' }
      ],
      createLoading: false,
      createUserForm: { name: '', cpf: '', email: '', phone: '', password: '', role: 1, department: null },
      roleOptions: [{ value: 1, text: 'Cidadão' }, { value: 2, text: 'Prefeitura (Admin)' }],
      departmentOptions: [
        { value: 'Saúde', text: 'Secretaria de Saúde' },
        { value: 'Obras', text: 'Secretaria de Obras' },
        { value: 'Trânsito', text: 'Dep. de Trânsito' },
        { value: 'Educação', text: 'Secretaria de Educação' },
        { value: 'Meio Ambiente', text: 'Meio Ambiente' },
        { value: 'Gabinete', text: 'Gabinete' }
      ]
    }
  },
  computed: {
    unreadCount () { return this.notifications.filter(n => !n.isRead).length }
  },
  created () {
    const token = localStorage.getItem('token')
    this.isLoggedIn = !!token
    if (this.isLoggedIn) {
      this.loadUserData()
      this.fetchNotifications()
    }
    if (this.userType === 2 || this.userType === 'CityHall' || this.userType === 'Prefeitura') {
      this.fetchUsers()
    }
  },
  methods: {
    loadUserData () {
      const userDataStr = localStorage.getItem('userData')
      if (userDataStr) {
        try {
          const userData = JSON.parse(userDataStr)
          if (!userData.id && !userData.Id) { this.logout(); return }
          this.userName = userData.nome || userData.name || 'Cidadão'
          this.userEmail = userData.email || ''
          this.userCpf = userData.cpf || ''
          this.userType = userData.role || userData.userType || userData.type
          this.profileForm.name = this.userName
        } catch (e) { this.logout() }
      }
    },

    handleSearchInput () {
      if (this.searchTimer) clearTimeout(this.searchTimer)
      this.searchTimer = setTimeout(() => {
        this.fetchUsers()
      }, 600)
    },

    async fetchUsers () {
      this.tableLoading = true
      try {
        const token = localStorage.getItem('token')
        const params = { perPage: 100 }
        if (this.filters.search) params.search = this.filters.search
        if (this.filters.type !== null) params.type = this.filters.type

        const { data } = await axios.get('/users/search', { params, headers: { Authorization: `Bearer ${token}` } })

        if (data && data.data) this.users = data.data
        else this.users = []
      } catch (error) {
        console.error('Erro ao buscar usuários:', error)
      } finally {
        this.tableLoading = false
      }
    },

    openCreateUserModal () {
      this.createUserForm = { name: '', cpf: '', email: '', phone: '', password: '', role: 1, department: null }
      this.$bvModal.show('create-user-modal')
    },

    async createUser () {
      this.createLoading = true
      try {
        const token = localStorage.getItem('token')
        const payload = {
          Nome: this.createUserForm.name,
          Email: this.createUserForm.email,
          Senha: this.createUserForm.password,
          Cpf: this.createUserForm.cpf,
          Type: this.createUserForm.role,
          Department: this.createUserForm.department,
          FoneNumber: this.createUserForm.phone
        }
        await axios.post('/auth/registrar', payload, { headers: { Authorization: `Bearer ${token}` } })
        this.$toast.success('Usuário criado com sucesso!')
        this.$bvModal.hide('create-user-modal')
        this.fetchUsers()
      } catch (error) {
        const msg = error.response?.data?.mensagem || 'Erro ao criar usuário.'
        this.$toast.error(msg)
      } finally {
        this.createLoading = false
      }
    },

    getRoleName (role) {
      if (role === 2 || role === 'CityHall' || role === 'Prefeitura') return 'Prefeitura'
      return 'Cidadão'
    },
    getRoleBadgeClass (role) {
      if (role === 2 || role === 'CityHall' || role === 'Prefeitura') return 'badge-light-primary'
      return 'badge-light-secondary'
    },
    formatPhone (phone) {
      if (!phone) return ''
      return phone.replace(/(\d{2})(\d{5})(\d{4})/, '($1) $2-$3')
    },

    async fetchNotifications () {
      try {
        const token = localStorage.getItem('token')
        const { data } = await axios.get('/notifications', { headers: { Authorization: `Bearer ${token}` } })
        this.notifications = data
      } catch (error) { console.error(error) }
    },
    async markNotificationsAsRead () {
      if (this.unreadCount === 0) return
      try {
        const token = localStorage.getItem('token')
        await axios.post('/notifications/read-all', {}, { headers: { Authorization: `Bearer ${token}` } })
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
      } catch (error) { this.$toast.error('Erro ao carregar perfil.') }
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
      if (this.isLoggedIn) this.$router.push({ name: 'register-complaint' })
      else {
        this.$swal({ title: 'Identifique-se', icon: 'warning', showCancelButton: true }).then((result) => {
          if (result.isConfirmed) this.$router.push({ name: 'auth-login' })
        })
      }
    },
    logout () {
      localStorage.removeItem('token')
      localStorage.removeItem('userData')
      this.isLoggedIn = false
      this.$toast.info('Até logo! Saindo...')
      setTimeout(() => { window.location.reload() }, 500)
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
  background-image: linear-gradient(70deg, #570303, #B11313);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.brand-logo {
  font-size: 1.5rem;
  letter-spacing: -0.5px;
}

.user-table th {
  border-top: none !important;
  font-size: 0.75rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  color: #b9b9c3;
  padding: 1.2rem 1rem;
  background-color: #ffffff;
}

.user-table td {
  padding: 1rem;
  vertical-align: middle;
  font-size: 0.9rem;
  color: #6e6b7b;
}

.user-table thead th:focus {
  outline: none;
}

.search-wrapper {
  position: relative;
  width: 100%;
}

.search-icon-input {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  color: #999;
}

.input-clean {
  border-radius: 20px;
  background-color: #ffffff;
  border: 1px solid #e5e7eb;
  padding-left: 40px;
  height: 45px;
  transition: all 0.2s ease;
}

.input-clean:focus {
  background-color: #ffffff;
  border-color: #8B0000;
  box-shadow: 0 0 0 3px rgba(139, 0, 0, 0.12);
}

.input-filter-select {
  height: 45px;
  border-radius: 8px;
  background-color: #ffffff;
  border: 1px solid #e5e7eb;
  padding: 0 14px;
  font-size: 0.9rem;
  font-weight: 500;
  color: #495057;
  transition: all 0.2s ease;
}

.input-filter-select:focus {
  border-color: #8B0000;
  box-shadow: 0 0 0 3px rgba(139, 0, 0, 0.12);
}

.input-filter-select.custom-select {
  background-image: none;
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

.input-disabled {
  background-color: #f5f5f5 !important;
  color: #6c757d !important;
  border-color: #eee !important;
  cursor: default !important;
  pointer-events: none;
}

.btn-cta-red {
  background-color: #8B0000 !important;
  border-color: #8B0000 !important;
  color: white !important;
}

.btn-cta-red:hover {
  background-color: #a01010 !important;
  border-color: #a01010 !important;
}

.btn-semi-rounded {
  border-radius: 8px !important;
}

.gap-3 {
  gap: 1rem;
}

.gap-2 {
  gap: 0.5rem;
}

.bg-light-sabara {
  background-color: #fff0f0;
}

.text-sabara {
  color: #8B0000;
}

.bg-sabara {
  background-color: #8B0000;
}

.badge-light-primary {
  background-color: #e3f2fd;
  color: #1565c0;
  font-weight: 600;
  border-radius: 6px;
}

.badge-light-secondary {
  background-color: #f3f3f3;
  color: #555;
  font-weight: 600;
  border-radius: 6px;
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
  border: 2px solid #343a40;
}

::v-deep .dropdown-item:active {
  background-color: #f8f9fa !important;
  color: #212529 !important;
}

::v-deep .dropdown-item:hover {
  background-color: #f1f1f1 !important;
  color: #212529 !important;
}

.hero {
  background-image: linear-gradient(rgba(0, 0, 0, 0.5), rgba(0, 0, 0, 0.5)), url("../../../public/Bairro-Pompeu.jpg");
  background-size: cover;
  background-position: center;
  min-height: 540px;
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
}

.content-overlay {
  z-index: 2;
}

.main-content-wrapper {
  margin-top: -30px;
  position: relative;
  z-index: 3;
  background-color: #272424;
  padding-bottom: 5rem;
}

.shadow-card {
  box-shadow: 0 15px 35px rgba(0, 0, 0, 0.08) !important;
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

.cursor-pointer {
  cursor: pointer;
}
</style>
