<template>
  <div class="complaint-feed">
    <div class="twitter-tabs mb-3 d-flex align-items-center justify-content-between border-bottom">
      <div class="d-flex">
        <div class="tab-item" :class="{ active: activeTab === 'recent' }" @click="setActiveTab('recent')">Mais recentes</div>
        <div class="tab-item" :class="{ active: activeTab === 'votes' }" @click="setActiveTab('votes')">Em Destaque</div>
        <div class="tab-item" :class="{ active: activeTab === 'mine' }" @click="setActiveTab('mine')">Minhas</div>
        <div class="tab-item" :class="{ active: activeTab === 'resolved' }" @click="setActiveTab('resolved')">Resolvidas</div>
      </div>
    </div>

    <b-card no-body class="mb-4 shadow-sm border-0">
      <div class="p-3">
        <b-row class="align-items-center">
          <b-col cols="12" md="8" class="mb-2 mb-md-0">
            <div class="search-wrapper">
              <search-icon size="18" class="search-icon-input" />
              <b-form-input
                v-model="search"
                placeholder="Buscar por título ou descrição..."
                class="input-clean"
                @input="onSearchInput"
              />
            </div>
          </b-col>

          <b-col cols="12" md="4" class="d-flex justify-content-md-end justify-content-between align-items-center">
            <div class="d-flex align-items-center">
              <b-button
                variant="flat-primary"
                class="btn-icon-filter me-2"
                :class="{
                  'filter-applied': hasActiveFilters,
                  'filter-open': isFilterActive
                }"
                @click="isFilterActive = !isFilterActive"
                v-b-tooltip.hover title="Filtros Avançados"
              >
                <filter-icon size="20" />
              </b-button>

              <b-button class="btn-refresh-square shadow-sm" @click="fetchComplaints(true)">
                <refresh-cw-icon size="16" :class="{ 'spin-icon': loading }" />
                <span class="ml-2 font-weight-bold d-none d-lg-inline">Atualizar</span>
              </b-button>
            </div>
          </b-col>
        </b-row>
      </div>

      <b-collapse v-model="isFilterActive" id="filters-collapse">
        <div class="p-3 bg-light-gray border-top">
          <b-row>
            <b-col md="4" class="mb-3">
              <label class="label-clean">Bairro</label>
              <v-select
                v-model="filterNeighborhood"
                :options="sabaraNeighborhoods"
                placeholder="Selecione o Bairro..."
                class="bg-white"
              />
            </b-col>
            <b-col md="4" class="mb-3">
              <label class="label-clean">Categoria</label>
              <v-select v-model="filterCategory" :options="categoryOptions" label="text" :reduce="op => op.value" placeholder="Todas" class="bg-white" />
            </b-col>
            <b-col md="4" class="mb-3">
              <label class="label-clean">Status</label>
              <v-select v-model="filterStatus" :options="statusOptions" label="text" :reduce="op => op.value" placeholder="Todos" class="bg-white" />
            </b-col>
          </b-row>
          <div class="text-right d-flex justify-content-end align-items-center">
             <b-button variant="light" class="text-muted m-3 p-2" size="sm" @click="clearFilters">Limpar</b-button>
             <b-button variant="danger" size="sm" class="px-4 p-2" @click="fetchComplaints(true)">Aplicar Filtros</b-button>
          </div>
        </div>
      </b-collapse>
    </b-card>

    <div v-if="loading" class="text-center my-5 py-5"><b-spinner variant="danger"></b-spinner></div>

    <div v-else-if="complaints.length === 0" class="text-center my-5 py-5 bg-white shadow-sm rounded">
      <inbox-icon size="48" class="text-muted mb-3" />
      <h5 class="text-muted">Nenhuma ocorrência encontrada.</h5>
    </div>

    <b-row v-else>
      <b-col md="6" lg="4" v-for="c in complaints" :key="c.id" class="mb-4">
        <b-card no-body class="h-100 shadow-sm border-0 complaint-card">

          <div class="card-img-wrapper">
             <template v-if="c.imageUrl">
                <div v-if="isVideo(c.imageUrl)" class="video-container">
                  <video
                    controls
                    playsinline
                    preload="metadata"
                    class="complaint-video"
                  >
                    <source :src="resolveImageUrl(c.imageUrl)" type="video/mp4">
                    Seu navegador não suporta vídeos.
                  </video>
                </div>
                <b-card-img-lazy
                  v-else
                  :src="resolveImageUrl(c.imageUrl)"
                  top
                  class="complaint-img"
                ></b-card-img-lazy>
             </template>

             <div v-else class="no-img-placeholder placeholder-gradient">
                <h3 class="placeholder-text mb-2">FalaBará</h3>
                <image-icon size="24" class="text-white-50" />
             </div>

             <span class="category-badge shadow-sm">{{ c.categoryName }}</span>

             <b-button
               v-if="isOwner(c.authorId)"
               variant="danger"
               size="sm"
               class="btn-delete-post shadow"
               @click="deleteComplaint(c.id)"
               v-b-tooltip.hover title="Excluir Post"
             >
               <trash-2-icon size="14" />
             </b-button>
          </div>

          <b-card-body class="d-flex flex-column pt-4">
            <div class="d-flex justify-content-between align-items-center mb-2">
              <small class="text-muted"><calendar-icon size="12"/> {{ formatDate(c.createdAt) }}</small>
              <b-badge :variant="getStatusVariant(c.statusName)">{{ c.statusName }}</b-badge>
            </div>

            <h5 class="card-title font-weight-bold text-dark mb-1">{{ c.title }}</h5>
            <h6 class="card-subtitle mb-3 text-muted small">
              <map-pin-icon size="12" class="text-danger" />
              {{ c.resolvedAddress || c.neighborhood || 'Local não informado' }}
            </h6>
            <p class="card-text text-truncate-3 flex-grow-1 text-secondary">{{ c.description }}</p>

            <hr class="mt-3 mb-3 border-light">

            <div class="d-flex justify-content-between align-items-center">
              <small class="text-muted">Por: <strong>{{ c.authorName || 'Anônimo' }}</strong></small>
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
import { MapPinIcon, ImageIcon, HeartIcon, RefreshCwIcon, SearchIcon, CalendarIcon, InboxIcon, FilterIcon, Trash2Icon } from 'vue-feather-icons'
import vSelect from 'vue-select'
import 'vue-select/dist/vue-select.css'

export default {
  components: { MapPinIcon, ImageIcon, HeartIcon, RefreshCwIcon, SearchIcon, CalendarIcon, InboxIcon, FilterIcon, Trash2Icon, vSelect },
  data () {
    return {
      complaints: [],
      loading: false,
      search: '',
      voting: null,
      apiBaseUrl: 'http://localhost:5282',
      searchTimeout: null,
      activeTab: 'recent',
      isFilterActive: false,
      filterCategory: null,
      filterStatus: null,
      filterNeighborhood: '',
      currentUserId: null,

      sabaraNeighborhoods: [
        'Centro', 'Nações Unidas', 'Siderúrgica', 'Alvorada', 'General Carneiro',
        'Ana Lúcia', 'Santa Inês', 'Rosário', 'Fátima', 'Cabral',
        'Vila Real', 'Nova Vista', 'Santo Antônio', 'Roça Grande',
        'Itacolomi', 'Morada da Serra', 'Vila Rica', 'Pompéu', 'Ravenna'
      ].sort(),

      categoryOptions: [{ value: 0, text: 'Saúde' }, { value: 1, text: 'Infraestrutura' }, { value: 2, text: 'Trânsito' }, { value: 3, text: 'Iluminação' }, { value: 4, text: 'Limpeza' }, { value: 5, text: 'Segurança' }, { value: 6, text: 'Educação' }, { value: 7, text: 'Meio Ambiente' }, { value: 8, text: 'Outros' }],
      statusOptions: [{ value: 0, text: 'Aberto' }, { value: 1, text: 'Em Análise' }, { value: 2, text: 'Em Andamento' }, { value: 3, text: 'Resolvido' }, { value: 4, text: 'Cancelado' }]
    }
  },
  computed: {
    hasActiveFilters () {
      return (this.filterNeighborhood !== '' && this.filterNeighborhood !== null) || this.filterCategory !== null || this.filterStatus !== null
    }
  },
  mounted () { this.checkUser(); this.fetchComplaints() },
  methods: {
    checkUser () {
      const userData = localStorage.getItem('userData')
      if (userData) {
        try {
          this.currentUserId = String(JSON.parse(userData).id)
        } catch (e) {}
      }
    },
    isOwner (userId) {
      return this.currentUserId && String(userId) === this.currentUserId
    },
    resolveImageUrl (path) { if (!path) return null; return path.startsWith('http') ? path : `${this.apiBaseUrl}${path}` },
    isVideo (url) {
      if (!url) return false
      return ['mp4', 'mov', 'webm', 'ogg', 'qt'].includes(url.split('.').pop().toLowerCase())
    },
    setActiveTab (tab) { this.activeTab = tab; this.fetchComplaints(true) },
    onSearchInput () { if (this.searchTimeout) clearTimeout(this.searchTimeout); this.searchTimeout = setTimeout(() => { this.fetchComplaints(true) }, 500) },

    async fetchComplaints (showLoader = true) {
      if (showLoader) this.loading = true
      try {
        let orderBy = 'recent'; let onlyMine = false; let statusFilter = this.filterStatus
        if (this.activeTab === 'votes') orderBy = 'votes'
        if (this.activeTab === 'mine') onlyMine = true
        if (this.activeTab === 'resolved') statusFilter = 3

        const params = {
          search: this.search,
          neighborhood: this.filterNeighborhood,
          perPage: 12,
          category: this.filterCategory,
          status: statusFilter,
          orderBy: orderBy,
          onlyMine: onlyMine
        }

        Object.keys(params).forEach(key => (params[key] === null || params[key] === '') && delete params[key])

        const token = localStorage.getItem('token')
        const config = { params: params, headers: {} }
        if (token) config.headers.Authorization = `Bearer ${token}`

        const { data } = await axios.get('/complaints/search', config)
        this.complaints = data.data

        this.complaints.forEach(c => this.resolveAddress(c))
      } catch (error) {
        console.error(error)
        if (showLoader) this.$toast?.error('Erro ao carregar.')
      } finally {
        this.loading = false
      }
    },

    async resolveAddress (complaint) {
      if (!complaint.latitude || !complaint.longitude) return
      try {
        const res = await axios.get(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${complaint.latitude}&lon=${complaint.longitude}`)
        if (res.data && res.data.display_name) {
          this.$set(complaint, 'resolvedAddress', res.data.display_name.split(',')[0])
        }
      } catch (e) { }
    },

    async vote (id) {
      const token = localStorage.getItem('token')
      if (!token) return this.$swal({ title: 'Entre para votar', icon: 'info' })
      this.voting = id
      try { await axios.post(`/votes/${id}`, {}, { headers: { Authorization: `Bearer ${token}` } }); this.$toast.success('Voto ok!'); await this.fetchComplaints(false) } catch (e) { this.$toast.error('Erro ao votar') } finally { this.voting = null }
    },
    async deleteComplaint (id) {
      this.$swal({ title: 'Excluir?', icon: 'warning', showCancelButton: true, confirmButtonText: 'Sim', cancelButtonText: 'Não' }).then(async (result) => {
        if (result.isConfirmed) {
          try {
            const token = localStorage.getItem('token')
            await axios.delete(`/complaints/${id}`, { headers: { Authorization: `Bearer ${token}` } })
            this.$toast.success('Excluído com sucesso.')
            this.fetchComplaints(true)
          } catch (e) { this.$toast.error('Erro ao excluir.') }
        }
      })
    },
    clearFilters () { this.filterCategory = null; this.filterStatus = null; this.filterNeighborhood = ''; this.fetchComplaints(true) },
    getStatusVariant (status) { return status === 'Resolvido' ? 'success' : 'danger' },
    formatDate (date) { return date ? new Date(date).toLocaleDateString('pt-BR') : '' }
  }
}
</script>

<style scoped>
.twitter-tabs { overflow-x: auto; white-space: nowrap; }
.tab-item { padding: 1rem 1.5rem; font-weight: 600; color: #555; cursor: pointer; position: relative; }
.tab-item:hover { color: #8B0000; }
.tab-item.active { color: #8B0000; }
.tab-item.active::after { content: ''; position: absolute; bottom: 0; left: 0; width: 100%; height: 4px; background-color: #8B0000; }
.search-wrapper { position: relative; width: 100%; }
.search-icon-input { position: absolute; left: 12px; top: 50%; transform: translateY(-50%); color: #999; }
.input-clean { border-radius: 20px; background-color: #f0f2f5; border: 1px solid transparent; padding-left: 40px; height: 45px; }
.btn-icon-filter { background-color: #f8f9fa; border: 1px solid #e9ecef; color: #5e5e5e; border-radius: 8px; width: 42px; height: 42px; display: flex; align-items: center; justify-content: center; transition: all 0.2s; }
.btn-icon-filter.filter-open { background-color: #e2e6ea; color: #8B0000; border-color: #dae0e5; }
.btn-icon-filter.filter-applied { background-color: #8B0000; color: white; border-color: #8B0000; }
.btn-icon-filter.filter-applied:hover { background-color: #660000; }
.btn-refresh-square { background: linear-gradient(135deg, #8B0000 0%, #a01010 100%); border: none; color: white; border-radius: 8px; padding: 0.6rem 1.2rem; height: 42px; display: flex; align-items: center; }
.complaint-card { border-radius: 12px; overflow: hidden; transition: transform 0.2s; }
.card-img-wrapper { height: 250px; overflow: hidden; position: relative; background-color: #eee; }
.complaint-img { width: 100%; height: 100%; object-fit: cover; }
.video-container { width: 100%; height: 100%; background-color: black; display: flex; align-items: center; justify-content: center; }
.complaint-video { width: 100%; height: 100%; object-fit: contain; }
.category-badge { position: absolute; bottom: 10px; right: 10px; background: white; color: #8B0000; padding: 4px 12px; border-radius: 20px; font-weight: bold; font-size: 0.75rem; }
.btn-delete-post { position: absolute; top: 10px; right: 10px; opacity: 0.9; z-index: 10; }
.bg-light-gray { background-color: #fbfbfb; }

.placeholder-gradient {
  background: linear-gradient(135deg, #8B0000 0%, #bd2130 100%);
  height: 100%;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  color: white;
}
.placeholder-text {
  font-family: 'Georgia', serif;
  font-style: italic;
  opacity: 0.9;
}
.text-white-50 {
  color: rgba(255, 255, 255, 0.5);
}
</style>
