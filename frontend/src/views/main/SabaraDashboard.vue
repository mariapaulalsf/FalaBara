<template>
  <div class="sabara-dashboard">
    <div class="d-flex justify-content-between align-items-center mb-4">
      <h4 class="text-dark font-weight-bold d-flex align-items-center mb-0">
        <bar-chart-2-icon size="24" class="mr-2 text-sabara" />
        Painel da Cidade
      </h4>
      <b-button size="sm" variant="outline-secondary" class="shadow-sm btn-semi-rounded" @click="fetchAllData">
        <refresh-cw-icon size="16" class="mr-1" :class="{ 'spin-icon': loading }" />
        Atualizar
      </b-button>
    </div>

    <div v-if="loading" class="text-center py-5">
      <b-spinner variant="danger" />
      <p class="text-muted mt-2 small font-weight-bold">Sincronizando dados...</p>
    </div>

    <div v-else>
      <b-row class="mb-4">
        <b-col cols="12" md="4" class="mb-3 mb-md-0">
          <div class="kpi-card kpi-total h-100 shadow-sm">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <p class="text-muted mb-1 font-weight-bold small text-uppercase">Total de Ocorrências</p>
                <h2 class="font-weight-bolder mb-0 text-dark">{{ metrics.totalComplaints }}</h2>
              </div>
              <div class="icon-bg bg-light-danger">
                <activity-icon size="20" class="text-sabara" />
              </div>
            </div>
          </div>
        </b-col>

        <b-col cols="12" md="4" class="mb-3 mb-md-0">
          <div class="kpi-card kpi-analysis h-100 shadow-sm">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <p class="text-muted mb-1 font-weight-bold small text-uppercase">Em Análise</p>
                <h2 class="font-weight-bolder mb-0 text-warning">{{ metrics.totalInAnalysis }}</h2>
              </div>
              <div class="icon-bg bg-light-warning">
                <clock-icon size="20" class="text-warning" />
              </div>
            </div>
          </div>
        </b-col>

        <b-col cols="12" md="4">
          <div class="kpi-card kpi-resolved h-100 shadow-sm">
            <div class="d-flex justify-content-between align-items-start">
              <div>
                <p class="text-muted mb-1 font-weight-bold small text-uppercase">Resolvido</p>
                <h2 class="font-weight-bolder mb-0 text-success">{{ metrics.totalResolved }}</h2>
              </div>
              <div class="icon-bg bg-light-success">
                <check-circle-icon size="20" class="text-success" />
              </div>
            </div>
          </div>
        </b-col>
      </b-row>

      <b-row class="mb-4">
        <b-col cols="12">
          <b-card no-body class="border-0 shadow-sm rounded-lg overflow-hidden">
            <div class="p-3 border-bottom bg-white d-flex justify-content-between align-items-center flex-wrap">
              <div>
                <h5 class="mb-0 font-weight-bold">Geolocalização</h5>
                <small class="text-muted">Distribuição das ocorrências na cidade</small>
              </div>

              <div class="map-controls mt-2 mt-md-0">
                <b-button-group size="sm">
                  <b-button
                    :variant="mapMode === 'heat' ? 'danger' : 'outline-secondary'"
                    @click="setMapMode('heat')"
                  >
                    Mapa de Calor
                  </b-button>
                  <b-button
                    :variant="mapMode === 'marker' ? 'danger' : 'outline-secondary'"
                    @click="setMapMode('marker')"
                  >
                    Pontos
                  </b-button>
                </b-button-group>
              </div>
            </div>

            <div id="heatmap-container" style="height: 450px; width: 100%;"></div>
          </b-card>
        </b-col>
      </b-row>

      <b-row v-if="userType == 2">
        <b-col cols="12" lg="4" class="mb-4">
          <b-card class="h-100 shadow-sm border-0 rounded-lg">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h6 class="font-weight-bold mb-0">Status Geral</h6>
              <b-button size="sm" variant="light" class="text-muted" @click="downloadChart('donutChart')" v-b-tooltip.hover title="Baixar PNG">
                <download-icon size="16" />
              </b-button>
            </div>

            <ApexChart
              ref="donutChart"
              type="donut"
              height="300"
              :options="chartStatusOptions"
              :series="chartStatusSeries"
            />
          </b-card>
        </b-col>

        <b-col cols="12" lg="8" class="mb-4">
          <b-card class="h-100 shadow-sm border-0 rounded-lg">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <div>
                <h6 class="font-weight-bold mb-0">Evolução Diária</h6>
                <small class="text-muted">Novas reclamações nos últimos 7 dias</small>
              </div>
              <b-button size="sm" variant="light" class="text-muted" @click="downloadChart('evolutionChart')" v-b-tooltip.hover title="Baixar PNG">
                <download-icon size="16" />
              </b-button>
            </div>

            <ApexChart
              ref="evolutionChart"
              type="area"
              height="300"
              :options="chartEvolutionOptions"
              :series="chartEvolutionSeries"
            />
          </b-card>
        </b-col>
      </b-row>

      <b-row v-if="userType == 2">
         <b-col cols="12">
          <b-card class="shadow-sm border-0 rounded-lg mb-4">
            <div class="d-flex justify-content-between align-items-center mb-3">
              <h6 class="font-weight-bold mb-0">Ocorrências por Categoria</h6>
              <b-button size="sm" variant="light" class="text-muted" @click="downloadChart('categoryChart')" v-b-tooltip.hover title="Baixar PNG">
                <download-icon size="16" />
              </b-button>
            </div>

            <ApexChart
              ref="categoryChart"
              type="bar"
              height="350"
              :options="chartCategoryOptions"
              :series="chartCategorySeries"
            />
          </b-card>
        </b-col>
      </b-row>
    </div>
  </div>
</template>

<script>
import axios from '@/libs/axios'
import { BRow, BCol, BCard, BButton, BButtonGroup, BSpinner } from 'bootstrap-vue'
import ApexChart from 'vue-apexcharts'
import { BarChart2Icon, RefreshCwIcon, ActivityIcon, CheckCircleIcon, ClockIcon, DownloadIcon } from 'vue-feather-icons'

import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import 'leaflet.heat'

const STATUS_MAP = { 0: 'Aberto', 1: 'Em Análise', 2: 'Em Andamento', 3: 'Resolvido', 4: 'Cancelado' }
const CATEGORY_MAP = { 0: 'Saúde', 1: 'Infraestrutura', 2: 'Trânsito', 3: 'Iluminação', 4: 'Limpeza', 5: 'Segurança', 6: 'Educação', 7: 'Meio Ambiente', 8: 'Outros' }

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
    BButtonGroup,
    BSpinner,
    ApexChart,
    BarChart2Icon,
    RefreshCwIcon,
    ActivityIcon,
    CheckCircleIcon,
    ClockIcon,
    DownloadIcon
  },
  data () {
    return {
      userName: 'Cidadão',
      userEmail: '',
      userCpf: '',
      userType: '',
      notifications: [],
      profileLoading: false,
      profileForm: {
        name: '',
        foneNumber: ''
      },
      loading: false,
      map: null,
      mapLayerGroup: null,
      mapMode: 'heat',
      tempPoints: [],

      dailyEvolutionData: { categories: [], data: [] },

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
    chartStatusSeries () {
      return this.metrics.complaintsByStatus.map(i => i.value)
    },
    chartStatusOptions () {
      return {
        chart: { id: 'status-donut', toolbar: { show: true } },
        labels: this.metrics.complaintsByStatus.map(i => STATUS_MAP[i.label]),
        colors: ['#ea5455', '#ff9f43', '#00cfe8', '#28c76f', '#82868b'],
        dataLabels: { enabled: false },
        legend: { position: 'bottom' },
        plotOptions: { pie: { donut: { size: '65%' } } }
      }
    },

    chartCategorySeries () {
      return [{ name: 'Ocorrências', data: this.metrics.complaintsByCategory.map(i => i.value) }]
    },
    chartCategoryOptions () {
      return {
        chart: { id: 'category-bar', toolbar: { show: true } }, // Toolbar habilitada para export funcionar
        plotOptions: { bar: { horizontal: true, borderRadius: 4, barHeight: '50%' } },
        xaxis: { categories: this.metrics.complaintsByCategory.map(i => CATEGORY_MAP[i.label]) },
        colors: ['#8B0000'],
        grid: { borderColor: '#f1f1f1' }
      }
    },

    chartEvolutionSeries () {
      return [{ name: 'Novas Reclamações', data: this.dailyEvolutionData.data }]
    },
    chartEvolutionOptions () {
      return {
        chart: { id: 'evolution-area', type: 'area', toolbar: { show: true }, zoom: { enabled: false } },
        dataLabels: { enabled: false },
        stroke: { curve: 'smooth', width: 2 },
        xaxis: {
          categories: this.dailyEvolutionData.categories,
          axisBorder: { show: false },
          axisTicks: { show: false }
        },
        yaxis: { show: false },
        grid: { show: false, padding: { left: 0, right: 0 } },
        colors: ['#8B0000'],
        fill: {
          type: 'gradient',
          gradient: { shadeIntensity: 1, opacityFrom: 0.7, opacityTo: 0.2, stops: [0, 90, 100] }
        }
      }
    }
  },
  mounted () {
    this.fetchAllData()
  },
  beforeDestroy () {
    if (this.map) this.map.remove()
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
    async fetchAllData () {
      this.loading = true
      try {
        const resMetrics = await axios.get('/dashboard')
        this.metrics = resMetrics.data

        const resPoints = await axios.get('/complaints/search?perPage=500')
        this.tempPoints = resPoints.data.data

        this.calculateDailyEvolution(this.tempPoints)
      } catch (e) {
        console.error('Erro dashboard:', e)
      } finally {
        this.loading = false
        this.$nextTick(() => this.updateMap())
      }
    },

    calculateDailyEvolution (complaints) {
      const days = 7
      const dateCount = {}
      const labels = []
      const data = []

      for (let i = days - 1; i >= 0; i--) {
        const d = new Date()
        d.setDate(d.getDate() - i)
        const key = d.toISOString().split('T')[0]
        const label = `${d.getDate()}/${d.getMonth() + 1}`
        dateCount[key] = 0
        labels.push(label)
      }

      complaints.forEach(c => {
        if (c.createdAt) {
          const key = c.createdAt.split('T')[0]
          if (Object.prototype.hasOwnProperty.call(dateCount, key)) {
            dateCount[key]++
          }
        }
      })

      for (let i = days - 1; i >= 0; i--) {
        const d = new Date()
        d.setDate(d.getDate() - i)
        const key = d.toISOString().split('T')[0]
        data.push(dateCount[key] || 0)
      }

      this.dailyEvolutionData = { categories: labels, data: data }
    },

    setMapMode (mode) {
      this.mapMode = mode
      this.updateMap()
    },

    updateMap () {
      if (!this.map) {
        this.map = L.map('heatmap-container').setView([-19.8917, -43.807], 13)
        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
          attribution: '&copy; OpenStreetMap'
        }).addTo(this.map)

        this.mapLayerGroup = L.layerGroup().addTo(this.map)
      } else {
        this.mapLayerGroup.clearLayers()
      }

      const validPoints = this.tempPoints.filter(c => c.latitude && c.longitude)

      if (this.mapMode === 'heat') {
        const heatPoints = validPoints.map(c => [c.latitude, c.longitude, 0.6])
        if (heatPoints.length) {
          L.heatLayer(heatPoints, { radius: 25, blur: 15, maxZoom: 17 }).addTo(this.mapLayerGroup)
        }
      } else {
        validPoints.forEach(c => {
          const color = '#8B0000'
          L.circleMarker([c.latitude, c.longitude], {
            radius: 8,
            fillColor: color,
            color: '#fff',
            weight: 2,
            opacity: 1,
            fillOpacity: 0.8
          })
            .bindPopup(`<strong>${c.title}</strong><br/>${c.neighborhood || 'Bairro não inf.'}`)
            .addTo(this.mapLayerGroup)
        })
      }
    },

    downloadChart (refName) {
      const chartInstance = this.$refs[refName]
      if (chartInstance && chartInstance.chart) {
        chartInstance.chart.exports.exportToPng()
      }
    }
  }
}
</script>

<style scoped>
.sabara-dashboard {
}

.kpi-card {
  background: #fff;
  padding: 1.5rem;
  border-radius: 16px;
  border: 1px solid rgba(0,0,0,0.05);
  transition: transform 0.2s;
}
.kpi-card:hover {
  transform: translateY(-3px);
  box-shadow: 0 10px 20px rgba(0,0,0,0.05) !important;
}

.icon-bg {
  width: 40px;
  height: 40px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
}
.bg-light-danger { background-color: #fff0f0; }
.bg-light-warning { background-color: #fff8ec; }
.bg-light-success { background-color: #eafbe7; }

.btn-semi-rounded { border-radius: 8px; }

.spin-icon {
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}

.map-controls button {
  font-weight: 500;
  font-size: 0.85rem;
}

.text-sabara { color: #8b0000; }
</style>
