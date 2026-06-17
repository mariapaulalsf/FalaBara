<template>
  <div class="complaint-form-container p-4">
    <b-card class="shadow border-0 rounded-lg">
      <div class="d-flex justify-content-between align-items-center px-3 pt-3">
        <div>
          <h3 class="font-weight-bold text-sabara mb-1">
            Nova Reclamação
          </h3>
          <p class="text-muted mb-0">
            Preencha os dados abaixo para registrar uma nova ocorrência.
          </p>
        </div>

        <b-button
          variant="outline-secondary"
          size="sm"
          @click="goBack"
        >
          ← Voltar
        </b-button>
      </div>

      <b-form @submit.prevent="submitComplaint" class="p-3">
        <b-card class="section-card mb-4">
          <b-row>
            <b-col cols="12">
              <b-form-group label="Título do Problema" label-for="title" class="font-weight-bold">
                <b-form-input
                  id="title"
                  v-model="form.title"
                  required
                  placeholder="Ex: Buraco na rua principal"
                  class="input-sabara"
                />
              </b-form-group>
            </b-col>

            <b-col cols="12">
              <b-form-group label="Descrição Detalhada" label-for="description" class="font-weight-bold">
                <b-form-textarea
                  id="description"
                  v-model="form.description"
                  required
                  rows="4"
                  placeholder="Descreva o problema, pontos de referência, etc..."
                  class="input-sabara"
                />
              </b-form-group>
            </b-col>
          </b-row>
        </b-card>

        <!-- LOCALIZAÇÃO -->
        <b-card class="section-card mb-4">
          <h5 class="text-muted mb-3">Localização da Ocorrência</h5>

          <b-row class="mb-3">
            <b-col md="6">
              <b-form-group label="Endereço (Automático)">
                <b-form-input
                  v-model="form.location"
                  placeholder="Rua detectada..."
                  readonly
                />
              </b-form-group>
            </b-col>

            <b-col md="6">
              <b-form-group label="Bairro (Automático)">
                <b-form-input
                  v-model="form.neighborhood"
                  placeholder="Bairro detectado..."
                  readonly
                  required
                />
              </b-form-group>
            </b-col>
          </b-row>

          <div class="d-flex flex-wrap align-items-center gap-2">
            <b-button
              variant="outline-danger"
              class="btn-gps"
              @click="getLocation"
              :disabled="gpsLoading"
            >
              {{ gpsLoading ? 'Buscando...' : '📍 Usar meu GPS' }}
            </b-button>

            <b-button
              variant="outline-secondary"
              class="btn-map"
              @click="openMapModal"
            >
              🗺️ Selecionar no Mapa
            </b-button>

            <div
              v-if="form.latitude"
              class="ml-3 px-3 py-2 bg-light rounded border-success text-success font-weight-bold"
            >
              Local confirmado
              <small>({{ form.latitude.toFixed(5) }}, {{ form.longitude.toFixed(5) }})</small>
            </div>
          </div>
        </b-card>

        <!-- CATEGORIA + EVIDÊNCIA -->
        <b-card class="section-card mb-2">
          <b-form-group
            label="Categoria da Reclamação"
            label-for="category"
            class="font-weight-bold mb-3"
          >
            <b-form-select
              id="category"
              v-model="form.category"
              :options="categoryOptions"
              required
              class="select-sabara"
            />
          </b-form-group>

          <b-form-group
            label="Evidência (Foto ou Vídeo)"
            label-for="file-upload"
            class="font-weight-bold"
          >
            <b-form-file
              id="file-upload"
              v-model="form.imageFile"
              accept="image/*, video/*"
              class="file-sabara"
              browse-text="Selecionar arquivo"
              placeholder=" "
              drop-placeholder="Solte o arquivo aqui"
            />
          </b-form-group>
        </b-card>

        <!-- BOTÕES FINAIS -->
        <div class="d-flex justify-content-between align-items-center mt-5">
          <b-button
            variant="outline-secondary"
            @click="goBack"
          >
            ←
          </b-button>

          <b-button
            type="submit"
            size="lg"
            class="btn-sabara px-5"
            :disabled="loading || !form.latitude"
          >
            <b-spinner small v-if="loading" class="mr-1"></b-spinner>
            {{ loading ? 'Enviando...' : 'Registrar Reclamação' }}
          </b-button>
        </div>

      </b-form>
    </b-card>

    <!-- MODAL MAPA -->
    <b-modal
      id="map-modal"
      title="Selecione o local exato"
      size="lg"
      @shown="initMap"
      ok-title="Confirmar Local"
      ok-variant="success"
      cancel-title="Cancelar"
    >
      <p class="text-muted text-center mb-2">
        Clique no mapa para posicionar o marcador.
      </p>
      <div id="leaflet-map-container" style="height: 400px; width: 100%;"></div>
    </b-modal>
  </div>
</template>

<script>
import axios from '@/libs/axios'
import {
  BCard, BForm, BFormGroup, BFormInput, BFormTextarea,
  BRow, BCol, BButton, BFormSelect, BFormFile,
  BSpinner, BModal
} from 'bootstrap-vue'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'

delete L.Icon.Default.prototype._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: require('leaflet/dist/images/marker-icon-2x.png'),
  iconUrl: require('leaflet/dist/images/marker-icon.png'),
  shadowUrl: require('leaflet/dist/images/marker-shadow.png')
})

export default {
  name: 'ComplaintForm',
  components: {
    BCard,
    BForm,
    BFormGroup,
    BFormInput,
    BFormTextarea,
    BRow,
    BCol,
    BButton,
    BFormSelect,
    BFormFile,
    BSpinner,
    BModal
  },
  data () {
    return {
      loading: false,
      gpsLoading: false,
      map: null,
      marker: null,
      form: {
        title: '',
        description: '',
        location: '',
        neighborhood: '',
        latitude: null,
        longitude: null,
        category: null,
        imageFile: null
      },
      categoryOptions: [
        { value: null, text: 'Selecione uma categoria', disabled: true },
        { value: 0, text: 'Saúde' },
        { value: 1, text: 'Infraestrutura / Obras' },
        { value: 2, text: 'Trânsito' },
        { value: 3, text: 'Iluminação Pública' },
        { value: 4, text: 'Limpeza Urbana' },
        { value: 5, text: 'Segurança' },
        { value: 6, text: 'Educação' },
        { value: 7, text: 'Meio Ambiente' },
        { value: 8, text: 'Outros' }
      ]
    }
  },
  methods: {
    goBack () {
      this.$router.push({ name: 'landing' })
    },

    getLocation () {
      if (!('geolocation' in navigator)) return
      this.gpsLoading = true

      navigator.geolocation.getCurrentPosition(
        pos => {
          this.form.latitude = pos.coords.latitude
          this.form.longitude = pos.coords.longitude
          this.getAddressFromCoords(this.form.latitude, this.form.longitude)
          this.gpsLoading = false
        },
        () => { this.gpsLoading = false }
      )
    },

    async getAddressFromCoords (lat, lon) {
      try {
        const res = await axios.get(
          `https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}`
        )
        const addr = res.data.address || {}
        this.form.neighborhood = addr.suburb || addr.neighbourhood || ''
        this.form.location = addr.road || ''
      } catch {}
    },

    openMapModal () {
      this.$bvModal.show('map-modal')
    },

    initMap () {
      if (this.map) this.map.remove()
      const lat = this.form.latitude || -19.8917
      const lng = this.form.longitude || -43.8070

      this.map = L.map('leaflet-map-container').setView([lat, lng], 15)
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png').addTo(this.map)

      this.map.on('click', e => {
        this.placeMarker(e.latlng.lat, e.latlng.lng)
        this.getAddressFromCoords(e.latlng.lat, e.latlng.lng)
      })
    },

    placeMarker (lat, lng) {
      if (this.marker) this.map.removeLayer(this.marker)
      this.marker = L.marker([lat, lng]).addTo(this.map)
      this.form.latitude = lat
      this.form.longitude = lng
    },

    async submitComplaint () {
      if (!this.form.latitude || this.form.category === null) return

      this.loading = true

      try {
        const fd = new FormData()
        fd.append('Title', this.form.title)
        fd.append('Description', this.form.description)
        fd.append('Location', this.form.location)
        fd.append('Neighborhood', this.form.neighborhood)
        fd.append('Latitude', this.form.latitude.toString().replace('.', ','))
        fd.append('Longitude', this.form.longitude.toString().replace('.', ','))
        fd.append('Category', this.form.category)

        if (this.form.imageFile) {
          fd.append('Image', this.form.imageFile)
        }

        const token = localStorage.getItem('token')

        await axios.post('/complaints', fd, {
          headers: { Authorization: `Bearer ${token}` }
        })

        this.$swal({
          title: 'Sucesso!',
          text: 'Reclamação registrada com sucesso.',
          icon: 'success'
        })

        this.resetForm()
        this.$router.push({ name: 'landing' })
      } catch (e) {
        this.$toast?.error('Erro ao registrar reclamação')
      } finally {
        this.loading = false
      }
    },

    resetForm () {
      this.form = {
        title: '',
        description: '',
        location: '',
        neighborhood: '',
        latitude: null,
        longitude: null,
        category: null,
        imageFile: null
      }
    }
  }
}
</script>

<style scoped>
.text-sabara { color: #8B0000; }

.section-card {
  border-radius: 16px;
  border: none;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.05);
}

.input-sabara:focus,
.select-sabara:focus {
  border-color: #8B0000;
  box-shadow: 0 0 0 0.2rem rgba(139, 0, 0, 0.25);
}

.select-sabara {
  height: 48px;
  border-radius: 10px;
}

.file-sabara {
  border: 2px dashed #ced4da;
  border-radius: 12px;
  padding: 20px;
  background-color: #fafafa;
}

.file-sabara:hover {
  border-color: #8B0000;
  background-color: #fff5f5;
}

.btn-sabara {
  background-color: #8B0000;
  border-color: #8B0000;
  border-radius: 30px;
  font-weight: 600;
}

.btn-sabara:hover {
  background-color: #660000;
}
</style>
