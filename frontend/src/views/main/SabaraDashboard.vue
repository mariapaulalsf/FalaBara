<template>
  <div class="sabara-dashboard p-3">
    
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="text-sabara font-weight-bold">
        <feather-icon icon="BarChart2Icon" size="24" class="mr-1" />
        Painel de Controle
      </h4>
      <b-button size="sm" variant="outline-secondary" @click="fetchData">
       <feather-icon icon="RefreshCwIcon" :class="{ 'spin-icon': loading }" /> Atualizar
      </b-button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <b-spinner variant="sabara" label="Carregando..."></b-spinner>
      <p class="text-muted mt-2">Buscando dados da prefeitura...</p>
    </div>

    <div v-else>
      <b-row class="mb-4">
        <b-col cols="12" md="4">
          <div class="kpi-card shadow-sm border-left-sabara">
            <h6 class="text-muted">Total de Reclamações</h6>
            <h2 class="font-weight-bolder text-sabara mb-0">{{ metrics.totalComplaints }}</h2>
          </div>
        </b-col>
        <b-col cols="12" md="4">
          <div class="kpi-card shadow-sm border-left-warning">
            <h6 class="text-muted">Em Análise</h6>
            <h2 class="font-weight-bolder text-warning mb-0">{{ metrics.totalInAnalysis }}</h2>
          </div>
        </b-col>
        <b-col cols="12" md="4">
          <div class="kpi-card shadow-sm border-left-success">
            <h6 class="text-muted">Resolvidas</h6>
            <h2 class="font-weight-bolder text-success mb-0">{{ metrics.totalResolved }}</h2>
          </div>
        </b-col>
      </b-row>

      <b-row>
        <b-col cols="12" lg="6" class="mb-4">
          <b-card title="Status das Solicitações" class="h-100 shadow-sm border-0">
            <apexchart type="donut" height="300" :options="chartStatusOptions" :series="chartStatusSeries" />
          </b-card>
        </b-col>

        <b-col cols="12" lg="6" class="mb-4">
          <b-card title="Principais Problemas" class="h-100 shadow-sm border-0">
            <apexchart type="bar" height="300" :options="chartCategoryOptions" :series="chartCategorySeries" />
          </b-card>
        </b-col>
      </b-row>
    </div>

  </div>
</template>

<script>
import axios from '@/libs/axios' 
import { BRow, BCol, BCard, BButton, BSpinner } from 'bootstrap-vue'
import { BarChart2Icon, RefreshCwIcon } from 'vue-feather-icons'

const STATUS_MAP = {
  0: 'Aberto',
  1: 'Em Análise',
  2: 'Em Andamento',
  3: 'Resolvido',
  4: 'Cancelado'
}

const CATEGORY_MAP = {
  0: 'Saúde',
  1: 'Infraestrutura', 
  2: 'Trânsito',
  3: 'Iluminação',
  4: 'Limpeza',
  5: 'Segurança',
  6: 'Educação',
  7: 'Meio Ambiente',
  8: 'Outros'
}

export default {
  name: 'SabaraDashboard',
  components: { BRow, BCol, BCard, BButton, BSpinner,BarChart2Icon, RefreshCwIcon},
  data() {
    return {
      loading: false,
      metrics: {
        totalComplaints: 0,
        totalResolved: 0,
        totalInAnalysis: 0,
        complaintsByCategory: [],
        complaintsByStatus: []
      }
    }
  },
  computed: {
    chartStatusSeries() {
      return this.metrics.complaintsByStatus.map(i => i.value)
    },
    chartStatusOptions() {
      return {
        labels: this.metrics.complaintsByStatus.map(i => STATUS_MAP[i.label] || `Status ${i.label}`),
        colors: ['#EA5455','#28C76F','#D4AF37', '#FF9F43', '#00CFE8',  ], 
        legend: { position: 'bottom' },
        noData: { text: 'Sem dados para exibir' }
      }
    },

    chartCategorySeries() {
      return [{ name: 'Qtd', data: this.metrics.complaintsByCategory.map(i => i.value) }]
    },
    chartCategoryOptions() {
      return {
        xaxis: { 
          categories: this.metrics.complaintsByCategory.map(i => CATEGORY_MAP[i.label] || `Cat. ${i.label}`) 
        },
        colors: ['#8B0000'],
        plotOptions: { bar: { borderRadius: 4, horizontal: true } }
      }
    }
  },
  mounted() {
    this.fetchData()
  },
  methods: {
    async fetchData() {
      this.loading = true
      try {
        const response = await axios.get('/dashboard')
        this.metrics = response.data
      } catch (error) {
        console.error('Erro ao carregar dashboard:', error)
        if (this.$toast) this.$toast.error('Não foi possível atualizar os dados.')
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

<style scoped>
.text-sabara { color: #8B0000; }
.bg-sabara { background-color: #8B0000; }

.kpi-card {
  background: white;
  padding: 1.5rem;
  border-radius: 8px;
  margin-bottom: 1rem;
  transition: transform 0.2s;
}
.kpi-card:hover { transform: translateY(-3px); }
.border-left-sabara { border-left: 5px solid #8B0000; }
.border-left-warning { border-left: 5px solid #FF9F43; }
.border-left-success { border-left: 5px solid #28C76F; }

.spin-icon { animation: spin 1s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }
</style>