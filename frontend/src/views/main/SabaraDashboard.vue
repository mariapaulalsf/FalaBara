<template>
  <div class="sabara-dashboard p-3">
    <b-card class="p-2 border-0 shadow-sm">

      <div class="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3">
        <h4 class="text-dark font-weight-bold d-flex align-items-center mb-0">
          <bar-chart-2-icon size="24" class="mr-2 text-sabara" />
          Painel da Cidade
        </h4>
        <b-button size="sm" class="bt_atualizar shadow-sm" @click="fetchAllData">
          <refresh-cw-icon size="16" :class="{ 'spin-icon': loading }" class="mr-1"/>
          Atualizar Dados
        </b-button>
      </div>

      <div v-if="loading" class="text-center py-5">
        <b-spinner variant="danger" label="Carregando..."></b-spinner>
        <p class="text-muted mt-2 small font-weight-bold">Sincronizando dados...</p>
      </div>

      <div v-else>

        <b-row class="mb-4">
          <b-col cols="12" md="4" class="mb-3 mb-md-0">
            <div class="kpi-card kpi-total h-100">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                   <p class="text-muted mb-1 font-weight-bold text-uppercase small">Total de Ocorrências</p>
                   <h2 class="font-weight-bolder text-dark mb-0">{{ metrics.totalComplaints }}</h2>
                </div>
                <div class="icon-circle bg-light-primary text-primary">
                   <file-text-icon size="24" />
                </div>
              </div>
            </div>
          </b-col>

          <b-col cols="12" md="4" class="mb-3 mb-md-0">
            <div class="kpi-card kpi-analysis h-100">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                   <p class="text-muted mb-1 font-weight-bold text-uppercase small">Em Análise</p>
                   <h2 class="font-weight-bolder text-warning mb-0">{{ metrics.totalInAnalysis }}</h2>
                </div>
                <div class="icon-circle bg-light-warning text-warning">
                   <clock-icon size="24" />
                </div>
              </div>
            </div>
          </b-col>

          <b-col cols="12" md="4">
            <div class="kpi-card kpi-resolved h-100">
              <div class="d-flex justify-content-between align-items-start">
                <div>
                   <p class="text-muted mb-1 font-weight-bold text-uppercase small">Resolvidas</p>
                   <h2 class="font-weight-bolder text-success mb-0">{{ metrics.totalResolved }}</h2>
                </div>
                <div class="icon-circle bg-light-success text-success">
                   <check-circle-icon size="24" />
                </div>
              </div>
            </div>
          </b-col>
        </b-row>

        <b-row>
          <b-col cols="12">
            <b-card no-body class="shadow-none border rounded overflow-hidden">
               <div class="p-3 bg-light border-bottom">
                  <h5 class="mb-0 font-weight-bold text-dark">Mapa de Calor</h5>
                  <small class="text-muted">Concentração geográfica das reclamações em aberto.</small>
               </div>
               <div id="heatmap-container" style="height: 400px; width: 100%;"></div>
            </b-card>
          </b-col>
        </b-row>

      </div>
    </b-card>
  </div>
</template>

<script>
import axios from '@/libs/axios'
import { BRow, BCol, BCard, BButton, BSpinner } from 'bootstrap-vue'
import { BarChart2Icon, RefreshCwIcon, FileTextIcon, ClockIcon, CheckCircleIcon } from 'vue-feather-icons'

import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import 'leaflet.heat'

delete L.Icon.Default.prototype._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
  iconUrl: require('leaflet/dist/images/marker-icon.png'),
  shadowUrl: require('leaflet/dist/images/marker-shadow.png')
})

export default {
  name: 'SabaraDashboard',
  components: {
    BRow,
    BCol,
    BCard,
    BButton,
    BSpinner,
    BarChart2Icon,
    RefreshCwIcon,
    FileTextIcon,
    ClockIcon,
    CheckCircleIcon
  },
  data () {
    return {
      loading: false,
      map: null,
      tempPoints: [],
      metrics: {
        totalComplaints: 0,
        totalResolved: 0,
        totalInAnalysis: 0,
        complaintsByCategory: [],
        complaintsByStatus: []
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
        const resMetrics = await axios.get('/dashboard')
        this.metrics = resMetrics.data
        const resPoints = await axios.get('/complaints/search?perPage=500')
        this.tempPoints = resPoints.data.data
      } catch (error) {
        console.error('Erro dashboard:', error)
      } finally {
        this.loading = false
        this.$nextTick(() => { this.initHeatmap(this.tempPoints) })
      }
    },

    initHeatmap (complaints) {
      const container = document.getElementById('heatmap-container')
      if (!container) return

      if (this.map) {
        this.map.remove()
        this.map = null
      }
      this.map = L.map('heatmap-container').setView([-19.8917, -43.807], 13)

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; OpenStreetMap'
      }).addTo(this.map)

      const heatPoints = complaints
        .filter((c) => c.latitude && c.longitude)
        .map((c) => [c.latitude, c.longitude, 0.6])

      if (heatPoints.length > 0) {
        L.heatLayer(heatPoints, { radius: 25, blur: 15, maxZoom: 17 }).addTo(this.map)
      }
    }
  }
}
</script>

<style scoped>
.bt_atualizar {
  background-color: #8B0000;
  border-color: #8B0000;
  color: white;
  font-weight: 600;
  padding: 0.5rem 1rem;
  transition: all 0.3s;
}
.bt_atualizar:hover {
  background-color: #660000;
  transform: translateY(-2px);
}

.kpi-card {
  background: white;
  padding: 1.5rem;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.05);
  border: 1px solid #eee;
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  position: relative;
  overflow: hidden;
}
.kpi-card:hover {
  transform: translateY(-5px);
  box-shadow: 0 8px 25px rgba(0,0,0,0.1);
}
.kpi-total { border-bottom: 4px solid #8B0000; }
.kpi-analysis { border-bottom: 4px solid #FF9F43; }
.kpi-resolved { border-bottom: 4px solid #28C76F; }
.icon-circle {
  width: 48px;
  height: 48px;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}
.bg-light-primary { background-color: rgba(15, 82, 186, 0.1); }
.bg-light-warning { background-color: rgba(255, 159, 67, 0.1); }
.bg-light-success { background-color: rgba(40, 199, 111, 0.1); }

/* Cores de Texto */
.text-primary { color: #8B0000 !important; }
.text-warning { color: #FF9F43 !important; }
.text-success { color: #28C76F !important; }
.text-sabara { color: #8B0000; }

.spin-icon { animation: spin 1s linear infinite; }
@keyframes spin { 100% { transform: rotate(360deg); } }
</style>
