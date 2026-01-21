<template>
  <div class="complaint-form-container p-4">
    <b-card class="shadow border-0 rounded-lg">

      <h3 class="font-weight-bold text-sabara mb-3 p-3">Nova Reclamação</h3>

      <b-form @submit.prevent="submitComplaint" class="p-3">

        <b-card>
          <b-row>
            <b-col cols="12">
              <b-form-group label="Título do Problema" label-for="title" class="font-weight-bold">
                <b-form-input id="title" v-model="form.title" required placeholder="Ex: Buraco na rua principal" class="input-sabara" />
              </b-form-group>
            </b-col>
            <b-col cols="12">
              <b-form-group label="Descrição Detalhada" label-for="description" class="font-weight-bold">
                <b-form-textarea id="description" v-model="form.description" required rows="4"
                  placeholder="Descreva o problema, pontos de referência, etc..." class="input-sabara" />
              </b-form-group>
            </b-col>
          </b-row>
        </b-card>

        <hr class="my-4">

        <b-card>
          <h5 class="text-muted mb-3">Localização da Ocorrência</h5>

          <b-row class="mb-3">
             <b-col md="6">
               <b-form-group label="Endereço (Preenchimento Automático)">
                 <b-form-input v-model="form.location" placeholder="Rua detectada..." />
               </b-form-group>
             </b-col>
             <b-col md="6">
               <b-form-group label="Bairro (Preenchimento Automático)">
                 <b-form-input v-model="form.neighborhood" placeholder="Bairro detectado..." required />
               </b-form-group>
             </b-col>
          </b-row>

          <div class="d-flex flex-wrap align-items-center mb-4 gap-2">
            <b-button variant="outline-danger" class="mr-2 btn-gps" @click="getLocation" :disabled="gpsLoading">
              {{ gpsLoading ? 'Buscando...' : '📍 Usar meu GPS' }}
            </b-button>
            <b-button variant="outline-secondary" class="btn-map" @click="openMapModal">
              🗺️ Selecionar no Mapa
            </b-button>
            <div v-if="form.latitude" class="ml-3 p-2 bg-light rounded border-success text-success font-weight-bold">
              Local Confirmado! <small>({{ form.latitude.toFixed(5) }}, {{ form.longitude.toFixed(5) }})</small>
            </div>
          </div>
        </b-card>

        <hr class="my-4">

        <b-card>
          <b-form-group label="Categoria" label-for="category" class="font-weight-bold">
            <b-form-select id="category" v-model="form.category" :options="categoryOptions" required class="input-sabara" />
          </b-form-group>
          <b-form-group label="Evidência (Foto ou Vídeo)" label-for="file-upload" class="font-weight-bold">
            <b-form-file
              id="file-upload"
              v-model="form.imageFile"
              :state="Boolean(form.imageFile)"
              placeholder="Clique para escolher..."
              drop-placeholder="Solte aqui..."
              accept="image/*, video/*"
            ></b-form-file>
          </b-form-group>
        </b-card>

        <div class="text-right mt-5">
            <b-button type="submit" variant="primary" size="lg" class="btn-sabara px-5" :disabled="loading || !form.latitude">
              <b-spinner small v-if="loading" class="mr-1"></b-spinner>
              {{ loading ? 'Enviando...' : 'Registrar Reclamação' }}
            </b-button>
        </div>

      </b-form>
    </b-card>

    <b-modal id="map-modal" title="Selecione o local exato" size="lg" @shown="initMap" ok-title="Confirmar Local" ok-variant="success" cancel-title="Cancelar">
      <p class="text-muted text-center mb-2">Clique no mapa para posicionar o marcador.</p>
      <div id="leaflet-map-container" style="height: 400px; width: 100%;"></div>
    </b-modal>

  </div>
</template>

<script>
import axios from '@/libs/axios'
import { BCard, BForm, BFormGroup, BFormInput, BFormTextarea, BRow, BCol, BButton, BFormSelect, BFormFile, BSpinner, BModal } from 'bootstrap-vue'
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
    BCard, BForm, BFormGroup, BFormInput, BFormTextarea, BRow, BCol, BButton, BFormSelect, BFormFile, BSpinner, BModal
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
        category: 0,
        imageFile: null
      },
      categoryOptions: [
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
    getLocation () {
      if (!('geolocation' in navigator)) {
        this.$toast?.error('Navegador sem suporte a GPS.')
        return
      }
      this.gpsLoading = true
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.form.latitude = position.coords.latitude
          this.form.longitude = position.coords.longitude
          this.getAddressFromCoords(this.form.latitude, this.form.longitude)
          this.gpsLoading = false
          this.$toast?.success('Localização capturada!')
        },
        (error) => {
          console.error(error)
          this.$toast?.error('Erro no GPS. Tente o mapa.')
          this.gpsLoading = false
        }
      )
    },

    // --- NOVA FUNÇÃO: Preenche Bairro e Rua Automaticamente ---
    async getAddressFromCoords (lat, lon) {
      try {
        const res = await axios.get(`https://nominatim.openstreetmap.org/reverse?format=json&lat=${lat}&lon=${lon}`)
        if (res.data && res.data.address) {
          const addr = res.data.address
          // Tenta pegar o bairro de vários campos possíveis
          this.form.neighborhood = addr.suburb || addr.neighbourhood || addr.residential || addr.village || ''
          this.form.location = addr.road || addr.pedestrian || ''
        }
      } catch (e) {
        console.log('Erro ao buscar endereço', e)
      }
    },

    openMapModal () {
      this.$bvModal.show('map-modal')
    },

    initMap () {
      if (this.map) this.map.remove()
      const lat = this.form.latitude || -19.8917
      const lng = this.form.longitude || -43.8070
      this.map = L.map('leaflet-map-container').setView([lat, lng], 15)
      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', { attribution: '&copy; OpenStreetMap' }).addTo(this.map)

      if (this.form.latitude) this.placeMarker(this.form.latitude, this.form.longitude)

      this.map.on('click', (e) => {
        this.placeMarker(e.latlng.lat, e.latlng.lng)
        // Busca endereço ao clicar no mapa também
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
      if (!this.form.latitude) {
        this.$toast?.warning('Localização obrigatória!')
        return
      }
      this.loading = true
      try {
        const formData = new FormData()
        formData.append('Title', this.form.title)
        formData.append('Description', this.form.description)
        formData.append('Location', this.form.location)
        formData.append('Neighborhood', this.form.neighborhood) // Agora vai preenchido!
        formData.append('Latitude', this.form.latitude.toString().replace('.', ','))
        formData.append('Longitude', this.form.longitude.toString().replace('.', ','))
        formData.append('Category', this.form.category)
        if (this.form.imageFile) formData.append('Image', this.form.imageFile)

        const token = localStorage.getItem('token')
        const config = { headers: { Authorization: `Bearer ${token}` } }

        await axios.post('/complaints', formData, config)
        this.$swal({ title: 'Sucesso!', text: 'Reclamação registrada!', icon: 'success' })
        this.resetForm()
        this.$router.push({ name: 'landing' }) // Redireciona para ver a reclamação
      } catch (error) {
        const msg = error.response?.data?.mensagem || 'Erro ao enviar.'
        this.$toast?.error(msg)
      } finally {
        this.loading = false
      }
    },

    resetForm () {
      this.form = { title: '', description: '', location: '', neighborhood: '', latitude: null, longitude: null, category: 0, imageFile: null }
      this.marker = null
    }
  }
}
</script>

<style scoped>
.text-sabara { color: #8B0000; }
.input-sabara:focus { border-color: #8B0000; box-shadow: 0 0 0 0.2rem rgba(139, 0, 0, 0.25); }
.btn-sabara { background-color: #8B0000; border-color: #8B0000; }
.btn-sabara:hover { background-color: #660000; border-color: #660000; }
</style>
