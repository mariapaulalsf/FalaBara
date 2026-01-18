<template>
  <div class="complaint-feed">
    <b-row class="mb-4">
      <b-col md="8">
        <b-input-group class="search-shadow">
          <b-form-input
            v-model="search"
            placeholder="Pesquisar por bairro, título..."
            class="input-sabara border-right-0"
            @keyup.enter="fetchComplaints"
          />
          <b-input-group-append>
            <b-button class="btn-search border-left-0" @click="fetchComplaints">
              <search-icon size="18" />
            </b-button>
          </b-input-group-append>
        </b-input-group>
      </b-col>

      <b-col md="4" class="text-right mt-3 mt-md-0">
        <b-button class="btn-refresh shadow-sm" @click="fetchComplaints">
          <refresh-cw-icon size="16" :class="{ 'spin-icon': loading }" />
          <span class="ml-2 font-weight-bold">Atualizar</span>
        </b-button>
      </b-col>
    </b-row>

    <div v-if="loading" class="text-center my-5 py-5">
      <b-spinner variant="danger" type="grow" label="Carregando..."></b-spinner>
    </div>

    <div v-else-if="complaints.length === 0" class="text-center my-5 py-5 bg-light rounded">
      <inbox-icon size="48" class="text-muted mb-3" />
      <h5 class="text-muted">Nenhuma ocorrência encontrada.</h5>
    </div>

    <b-row v-else>
      <b-col md="6" lg="4" v-for="c in complaints" :key="c.id" class="mb-4">
        <b-card no-body class="h-100 shadow-sm border-0 complaint-card">

          <div class="card-img-wrapper">
            <b-card-img-lazy
              v-if="c.imageUrl"
              :src="resolveImageUrl(c.imageUrl)"
              top
              class="complaint-img"
              alt="Evidência"
            ></b-card-img-lazy>

            <div v-else class="no-img-placeholder">
              <image-icon size="40" class="text-muted-light" />
              <small class="text-muted d-block mt-2">Sem imagem</small>
            </div>

            <span class="category-badge shadow-sm">{{ c.categoryName }}</span>
          </div>

          <b-card-body class="d-flex flex-column pt-4">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <small class="text-muted d-flex align-items-center">
                <calendar-icon size="12" class="mr-1"/> {{ formatDate(c.createdAt) }}
              </small>
              <b-badge :variant="getStatusVariant(c.statusName)" class="px-2 py-1">
                {{ c.statusName }}
              </b-badge>
            </div>

            <h5 class="card-title font-weight-bold text-dark mb-1">{{ c.title }}</h5>

            <h6 class="card-subtitle mb-3 text-muted small d-flex align-items-center">
              <map-pin-icon size="12" class="mr-1 text-danger" /> {{ c.neighborhood || 'Local não informado' }}
            </h6>

            <p class="card-text text-truncate-3 flex-grow-1 text-secondary">
              {{ c.description }}
            </p>

            <hr class="mt-3 mb-3 border-light">

            <div class="d-flex justify-content-between align-items-center">
              <div class="author-info">
                <small class="text-muted">Por: <strong>{{ c.authorName || 'Anônimo' }}</strong></small>
              </div>

              <b-button
                size="sm"
                :variant="voting === c.id ? 'danger' : 'outline-danger'"
                class="btn-vote rounded-pill px-3"
                @click="vote(c.id)"
                :disabled="voting === c.id"
              >
                <heart-icon size="14" :class="{ 'fill-current': voting === c.id }" />
                <span class="ml-1">{{ c.likesCount || 0 }}</span>
              </b-button>
            </div>
          </b-card-body>
        </b-card>
      </b-col>
    </b-row>
  </div>
</template>

<script>
import axios from '@/libs/axios'
import { MapPinIcon, ImageIcon, HeartIcon, RefreshCwIcon, SearchIcon, CalendarIcon, InboxIcon } from 'vue-feather-icons'

export default {
  components: { MapPinIcon, ImageIcon, HeartIcon, RefreshCwIcon, SearchIcon, CalendarIcon, InboxIcon },
  data () {
    return {
      complaints: [],
      loading: false,
      search: '',
      voting: null,
      // URL base do seu backend (ajuste se mudar a porta)
      apiBaseUrl: 'http://localhost:5282'
    }
  },
  mounted () {
    this.fetchComplaints()
  },
  methods: {
    // --- NOVO MÉTODO PARA ARRUMAR AS IMAGENS ---
    resolveImageUrl (path) {
      if (!path) return null
      // Se já for um link completo (começa com http), retorna ele mesmo
      if (path.startsWith('http')) return path
      // Se for relativo (/uploads/...), junta com a URL do backend
      return `${this.apiBaseUrl}${path}`
    },

    async fetchComplaints () {
      this.loading = true
      try {
        const { data } = await axios.get('/complaints/search', {
          params: { search: this.search, perPage: 12 }
        })
        this.complaints = data.data
      } catch (error) {
        console.error(error)
        this.$toast?.error('Erro ao carregar o feed.')
      } finally {
        this.loading = false
      }
    },
    async vote (id) {
      const token = localStorage.getItem('token')
      if (!token) {
        this.$swal({
          title: 'Login Necessário',
          text: 'Você precisa entrar para apoiar uma reclamação.',
          icon: 'info',
          showCancelButton: true,
          confirmButtonText: 'Ir para Login',
          cancelButtonText: 'Cancelar'
        }).then((result) => {
          if (result.isConfirmed) this.$router.push({ name: 'auth-login' })
        })
        return
      }

      this.voting = id
      try {
        await axios.post(`/votes/${id}`)
        this.$toast.success('Voto registrado!')
        this.fetchComplaints()
      } catch (error) {
        this.$toast.error('Erro ao votar.')
      } finally {
        this.voting = null
      }
    },
    getStatusVariant (status) {
      switch (status) {
        case 'Resolvido': return 'success'
        case 'Em Andamento': return 'warning'
        case 'Cancelado': return 'secondary'
        default: return 'danger'
      }
    },
    formatDate (date) {
      if (!date) return ''
      return new Date(date).toLocaleDateString('pt-BR')
    }
  }
}
</script>

<style scoped>
/* Mesmos estilos anteriores para manter o visual bonito */
:root { --sabara-red: #8B0000; }
.search-shadow { box-shadow: 0 4px 12px rgba(0,0,0,0.05); border-radius: 8px; overflow: hidden; }
.input-sabara { border: 1px solid #eee; padding: 1.5rem 1rem; }
.input-sabara:focus { border-color: #8B0000; box-shadow: none; }
.btn-search { background-color: white; border: 1px solid #eee; color: #8B0000; }
.btn-search:hover { background-color: #8B0000; color: white; }
.btn-refresh { background: linear-gradient(135deg, #8B0000 0%, #a01010 100%); border: none; color: white; border-radius: 50px; padding: 0.7rem 1.5rem; }
.complaint-card { transition: transform 0.3s; border-radius: 12px; overflow: hidden; }
.complaint-card:hover { transform: translateY(-5px); box-shadow: 0 10px 25px rgba(0,0,0,0.1) !important; }
.card-img-wrapper { position: relative; height: 200px; overflow: hidden; background-color: #f0f0f0; }
.complaint-img { height: 100%; width: 100%; object-fit: cover; transition: transform 0.5s; }
.complaint-card:hover .complaint-img { transform: scale(1.05); }
.no-img-placeholder { height: 100%; display: flex; flex-direction: column; align-items: center; justify-content: center; }
.category-badge { position: absolute; bottom: 10px; right: 10px; background: white; color: #8B0000; padding: 4px 12px; border-radius: 20px; font-weight: bold; font-size: 0.75rem; }
.text-truncate-3 { display: -webkit-box; -webkit-line-clamp: 3; -webkit-box-orient: vertical; overflow: hidden; }
.spin-icon { animation: spin 1s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }
</style>
