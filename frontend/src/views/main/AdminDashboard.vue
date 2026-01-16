<template>
  <div>
    <div class="row">
      <div class="col-md-4" v-for="kpi in kpis" :key="kpi.label">
        <card>
          <h3>{{ kpi.value }}</h3>
          <p>{{ kpi.label }}</p>
        </card>
      </div>
    </div>

    <div class="row">
      <div class="col-md-6">
        <card>
          <h4>Status das Reclamações</h4>
          <pie-chart :chart-data="statusChart" />
        </card>
      </div>

      <div class="col-md-6">
        <card>
          <h4>Reclamações por Categoria</h4>
          <bar-chart :chart-data="categoryChart" />
        </card>
      </div>
    </div>
  </div>
</template>

<script>
import { getDashboard } from '@/services/dashboard.service'

export default {
  async mounted () {
    const { data } = await getDashboard()
    this.kpis = data.kpis
    this.charts = data.charts
  }
}
</script>
