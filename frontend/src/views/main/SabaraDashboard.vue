<template>
  <div class="sabara-dashboard p-3">
    <b-card class="p-2 border-0 shadow-sm">

      <!-- Header -->
      <div class="d-flex justify-content-between align-items-center mb-4 border-bottom pb-3">
        <h4 class="text-dark font-weight-bold d-flex align-items-center mb-0">
          <bar-chart-2-icon size="24" class="mr-2 text-sabara" />
          Painel da Cidade
        </h4>

        <b-button size="sm" class="bt_atualizar shadow-sm" @click="fetchAllData">
          <refresh-cw-icon size="16" class="mr-1" :class="{ 'spin-icon': loading }" />
          Atualizar Dados
        </b-button>
      </div>

      <!-- Loading -->
      <div v-if="loading" class="text-center py-5">
        <b-spinner variant="danger" />
        <p class="text-muted mt-2 small font-weight-bold">
          Sincronizando dados...
        </p>
      </div>

      <!-- Conteúdo -->
      <div v-else>

        <!-- KPIs -->
        <b-row class="mb-4">
          <b-col cols="12" md="4" class="mb-3 mb-md-0">
            <div class="kpi-card kpi-total h-100">
              <p class="text-muted mb-1 font-weight-bold small">TOTAL</p>
              <h2 class="font-weight-bolder mb-0">
                {{ metrics.totalComplaints }}
              </h2>
            </div>
          </b-col>

          <b-col cols="12" md="4" class="mb-3 mb-md-0">
            <div class="kpi-card kpi-analysis h-100">
              <p class="text-muted mb-1 font-weight-bold small">EM ANÁLISE</p>
              <h2 class="font-weight-bolder text-warning mb-0">
                {{ metrics.totalInAnalysis }}
              </h2>
            </div>
          </b-col>

          <b-col cols="12" md="4">
            <div class="kpi-card kpi-resolved h-100">
              <p class="text-muted mb-1 font-weight-bold small">RESOLVIDAS</p>
              <h2 class="font-weight-bolder text-success mb-0">
                {{ metrics.totalResolved }}
              </h2>
            </div>
          </b-col>
        </b-row>

        <!-- Mapa -->
        <b-row class="mb-4">
          <b-col cols="12">
            <b-card no-body class="border shadow-sm">
              <div class="p-3 border-bottom bg-light">
                <h5 class="mb-0 font-weight-bold">Mapa de Calor</h5>
                <small class="text-muted">
                  Concentração de reclamações em aberto
                </small>
              </div>
              <div id="heatmap-container" style="height: 400px;"></div>
            </b-card>
          </b-col>
        </b-row>

        <!-- GRÁFICOS -->
        <b-row class="mt-4">
          <b-col cols="12" lg="6" class="mb-4">
            <b-card class="h-100 shadow-sm border-0">
              <h5 class="font-weight-bold mb-2">
                Status das Solicitações
              </h5>
              <ApexChart
                type="donut"
                height="300"
                :options="chartStatusOptions"
                :series="chartStatusSeries"
              />
            </b-card>
          </b-col>

          <b-col cols="12" lg="6" class="mb-4">
            <b-card class="h-100 shadow-sm border-0">
              <h5 class="font-weight-bold mb-2">
                Principais Problemas
              </h5>
              <ApexChart
                type="bar"
                height="300"
                :options="chartCategoryOptions"
                :series="chartCategorySeries"
              />
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
import ApexChart from 'vue-apexcharts'
import { BarChart2Icon, RefreshCwIcon } from 'vue-feather-icons'

import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import 'leaflet.heat'

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
    ApexChart,
    BarChart2Icon,
    RefreshCwIcon
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
  computed: {
    chartStatusSeries () {
      return this.metrics.complaintsByStatus.map(i => i.value)
    },
    chartStatusOptions () {
      return {
        labels: this.metrics.complaintsByStatus.map(
          i => STATUS_MAP[i.label]
        ),
        legend: { position: 'bottom' }
      }
    },
    chartCategorySeries () {
      return [
        { data: this.metrics.complaintsByCategory.map(i => i.value) }
      ]
    },
    chartCategoryOptions () {
      return {
        plotOptions: {
          bar: { horizontal: true, borderRadius: 6 }
        },
        xaxis: {
          categories: this.metrics.complaintsByCategory.map(
            i => CATEGORY_MAP[i.label]
          )
        },
        colors: ['#8B0000']
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
      } catch (e) {
        console.error('Erro dashboard:', e)
      } finally {
        this.loading = false
        this.$nextTick(() => this.initHeatmap(this.tempPoints))
      }
    },

    initHeatmap (complaints) {
      if (this.map) this.map.remove()

      this.map = L.map('heatmap-container').setView([-19.8917, -43.807], 13)

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(this.map)

      const points = complaints
        .filter(c => c.latitude && c.longitude)
        .map(c => [c.latitude, c.longitude, 0.6])

      if (points.length) {
        L.heatLayer(points, { radius: 25, blur: 15 }).addTo(this.map)
      }
    }
  }
}
</script>

<style scoped>
.bt_atualizar {
  background: #8b0000;
  color: #fff;
  font-weight: 600;
}
.kpi-card {
  background: #fff;
  padding: 1.5rem;
  border-radius: 12px;
  border: 1px solid #eee;
}
.kpi-total { border-bottom: 4px solid #8b0000; }
.kpi-analysis { border-bottom: 4px solid #ff9f43; }
.kpi-resolved { border-bottom: 4px solid #28c76f; }

.text-sabara { color: #8b0000; }

.spin-icon {
  animation: spin 1s linear infinite;
}
@keyframes spin {
  to { transform: rotate(360deg); }
}
</style>
