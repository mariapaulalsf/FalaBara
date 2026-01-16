<template>
  <div class="complaint-feed">
    <b-row class="mb-4">
      <b-col md="8">
        <b-form-input v-model="search" placeholder="Buscar por título, bairro ou descrição..." @debounce="fetchComplaints" />
      </b-col>
      <b-col md="4" class="text-right">
        <b-button variant="sabara" @click="fetchComplaints">
          <!-- <refresh-cw-icon size="16" /> Atualizar -->
        </b-button>
      </b-col>
    </b-row>

    <div v-if="loading" class="text-center my-5">
      <b-spinner variant="primary"></b-spinner>
    </div>

    <div v-else-if="complaints.length === 0" class="text-center my-5 text-muted">
      Nenhuma reclamação encontrada.
    </div>

    <b-row v-else>
      <b-col md="6" lg="4" v-for="c in complaints" :key="c.id" class="mb-4">
        <b-card no-body class="h-100 shadow-sm border-0 complaint-card">
          <b-card-img-lazy v-if="c.imageUrl" :src="c.imageUrl" top style="height: 200px; object-fit: cover;"></b-card-img-lazy>
          <div v-else class="bg-light d-flex align-items-center justify-content-center" style="height: 200px;">
            <!-- <image-icon size="40" class="text-muted" /> -->
          </div>

          <b-card-body class="d-flex flex-column">
            <div class="d-flex justify-content-between mb-2">
              <b-badge variant="info">{{ c.categoryName }}</b-badge>
              <small class="text-muted">{{ formatDate(c.createdAt) }}</small>
            </div>

            <h5 class="card-title font-weight-bold">{{ c.title }}</h5>
            <h6 class="card-subtitle mb-2 text-muted">
              <!-- <map-pin-icon size="14" />  -->
              {{ c.neighborhood }}
            </h6>

            <p class="card-text text-truncate-3 flex-grow-1">
              {{ c.description }}
            </p>

            <div class="mt-3 d-flex justify-content-between align-items-center">
              <b-badge :variant="getStatusVariant(c.statusName)">{{ c.statusName }}</b-badge>

              <b-button size="sm" variant="outline-danger" @click="vote(c.id)" :disabled="voting === c.id">
                <!-- <heart-icon size="14" /> {{ c.likesCount || 0 }} Apoiar -->
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
// Importamos os ícones
// import { MapPinIcon, ImageIcon, HeartIcon, RefreshCwIcon } from 'vue-feather-icons'

export default {
  // Registramos os ícones
  // components: { MapPinIcon, ImageIcon, HeartIcon, RefreshCwIcon },
  data () {
    return {
      complaints: [],
      loading: false,
      search: '',
      voting: null
    }
  },
  mounted () {
    this.fetchComplaints()
  },
  methods: {
    async fetchComplaints () {
      this.loading = true
      try {
        const { data } = await axios.get('/complaints/search', {
          params: { search: this.search, perPage: 12 }
        })
        this.complaints = data.data
      } catch (error) {
        console.error(error)
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
      if (status === 'Resolvido') return 'success'
      if (status === 'Aberto') return 'danger'
      return 'warning'
    },
    formatDate (date) {
      return new Date(date).toLocaleDateString('pt-BR')
    }
  }
}
</script>

<style scoped>
.text-truncate-3 {
  display: -webkit-box;
  -webkit-line-clamp: 3;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
.complaint-card:hover {
  transform: translateY(-5px);
  transition: transform 0.3s;
}
</style>
