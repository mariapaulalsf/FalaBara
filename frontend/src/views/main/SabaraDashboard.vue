<template>
  <div class="sabara-dashboard p-3">

    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="text-sabara font-weight-bold d-flex align-items-center">
        <bar-chart-2-icon size="24" class="mr-1" />
        Painel da Cidade
      </h4>
      <b-button size="sm" variant="outline-secondary" @click="fetchAllData">
       <refresh-cw-icon size="16" :class="{ 'spin-icon': loading }" /> Atualizar
      </b-button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <b-spinner variant="sabara" label="Carregando..."></b-spinner>
      <p class="text-muted mt-2">Atualizando dados...</p>
    </div>

    <div v-else>
      <b-row class="mb-4">
        <b-col cols="12" md="4">
          <div class="kpi-card shadow-sm border-left-sabara">
            <h6 class="text-muted">Total</h6>
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

      <b-row class="mb-4">
        <b-col cols="12">
          <b-card title="Mapa de Calor de Problemas" class="shadow-sm border-0">
            <p class="text-muted small">Regiões com maior concentração de reclamações em aberto.</p>
            <div id="heatmap-container" style="height: 400px; width: 100%; border-radius: 8px;"></div>
          </b-card>
        </b-col>
      </b-row>

      <b-row>
        <b-col cols="12" lg="6" class="mb-4">
          <b-card title="Status das Solicitações" class="h-100 shadow-sm border-0">
            <ApexChart type="donut" height="300" :options="chartStatusOptions" :series="chartStatusSeries" />
          </b-card>
        </b-col>

        <b-col cols="12" lg="6" class="mb-4">
          <b-card title="Principais Problemas" class="h-100 shadow-sm border-0">
            <ApexChart type="bar" height="300" :options="chartCategoryOptions" :series="chartCategorySeries" />
          </b-card>
        </b-col>
      </b-row>
    </div>

  </div>
</template>

<script>
import axios from '@/libs/axios'
import { BRow, BCol, BCard, BButton, BSpinner } from 'bootstrap-vue'
// CORREÇÃO 1: Importamos os ícones para usar como componentes
import { BarChart2Icon, RefreshCwIcon } from 'vue-feather-icons'

// MAPAS (Leaflet + Heatmap)
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import 'leaflet.heat'

// Correção dos pinos do Leaflet
delete L.Icon.Default.prototype._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
  iconUrl: require('leaflet/dist/images/marker-icon.png'),
  shadowUrl: require('leaflet/dist/images/marker-shadow.png')
})

const STATUS_MAP = {
  0: 'Aberto', 1: 'Em Análise', 2: 'Em Andamento', 3: 'Resolvido', 4: 'Cancelado'
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
  // Registramos os componentes aqui
  components: {
    BRow,
    BCol,
    BCard,
    BButton,
    BSpinner,
    BarChart2Icon,
    RefreshCwIcon
  },
  data () {
    return {
      loading: false,
      map: null,
      tempPoints: [], // Guarda os pontos temporariamente
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
    chartStatusSeries () { return this.metrics.complaintsByStatus.map(i => i.value) },
    chartStatusOptions () {
      return {
        labels: this.metrics.complaintsByStatus.map(i => STATUS_MAP[i.label] || `Status ${i.label}`),
        colors: ['#EA5455', '#FF9F43', '#00CFE8', '#28C76F', '#D4AF37'],
        legend: { position: 'bottom' }
      }
    },
    chartCategorySeries () { return [{ name: 'Qtd', data: this.metrics.complaintsByCategory.map(i => i.value) }] },
    chartCategoryOptions () {
      return {
        xaxis: { categories: this.metrics.complaintsByCategory.map(i => CATEGORY_MAP[i.label] || `Cat. ${i.label}`) },
        colors: ['#8B0000'],
        plotOptions: { bar: { borderRadius: 4, horizontal: true } }
      }
    }
  },
  mounted () {
    this.fetchAllData()
  },
  methods: {
    async fetchAllData () {
      this.loading = true
      try {
        // 1. Busca Métricas (Gráficos)
        const resMetrics = await axios.get('/dashboard')
        this.metrics = resMetrics.data

        // 2. Busca Lista de Reclamações (Para o Mapa de Calor)
        const resPoints = await axios.get('/complaints/search?perPage=500')
        this.tempPoints = resPoints.data.data
      } catch (error) {
        console.error('Erro ao carregar dashboard:', error)
      } finally {
        // CORREÇÃO 2: Primeiro paramos o loading para o v-else renderizar a div
        this.loading = false

        // Esperamos o Vue atualizar o DOM (NextTick) para só então desenhar o mapa
        this.$nextTick(() => {
          this.initHeatmap(this.tempPoints)
        })
      }
    },

    initHeatmap (complaints) {
      const container = document.getElementById('heatmap-container')
      if (!container) return

      if (this.map) {
        this.map.remove()
        this.map = null
      }
      this.map = L.map('heatmap-container').setView([-19.8917, -43.8070], 13)

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap'
      }).addTo(this.map)
      const heatPoints = complaints
        .filter(c => c.latitude && c.longitude)
        .map(c => [c.latitude, c.longitude, 0.6])

      if (heatPoints.length > 0) {
        L.heatLayer(heatPoints, { radius: 25, blur: 15, maxZoom: 17 }).addTo(this.map)
      }
    }
  }
}
</script>

<style scoped>
.text-sabara { color: #8B0000; }
.bg-sabara { background-color: #8B0000; }
.kpi-card { background: white; padding: 1.5rem; border-radius: 8px; margin-bottom: 1rem; }
.border-left-sabara { border-left: 5px solid #8B0000; }
.border-left-warning { border-left: 5px solid #FF9F43; }
.border-left-success { border-left: 5px solid #28C76F; }
.spin-icon { animation: spin 1s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }
#heatmap-container { z-index: 1; }
</style>
