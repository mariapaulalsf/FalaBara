<template>
  <div class="complaint-form-container">
    <b-card title="Nova Reclamação" class="shadow-sm border-0">
      <b-form @submit.prevent="submitComplaint">

        <b-row>
          <b-col cols="12">
            <b-form-group label="Título do Problema" label-for="title">
              <b-form-input id="title" v-model="form.title" required placeholder="Ex: Buraco na via" />
            </b-form-group>
          </b-col>
          <b-col cols="12">
            <b-form-group label="Descrição Detalhada" label-for="description">
              <b-form-textarea id="description" v-model="form.description" required rows="3"
                placeholder="Descreva o problema com detalhes..." />
            </b-form-group>
          </b-col>
        </b-row>

        <div class="d-flex align-items-center mb-4">
          <b-button size="sm" variant="outline-primary" @click="getLocation" :disabled="gpsLoading">
            <feather-icon icon="MapPinIcon" class="mr-1" :class="{ 'spin-icon': gpsLoading }" />
            {{ gpsLoading ? 'Buscando GPS...' : 'Pegar Minha Localização Atual' }}
          </b-button>
          
          <small v-if="form.latitude" class="text-success ml-3 font-weight-bold">
            <feather-icon icon="CheckIcon" size="14" />
            GPS Capturado! ({{ form.latitude.toFixed(4) }}, {{ form.longitude.toFixed(4) }})
          </small>
          <small v-else class="text-danger ml-3">
            * GPS Obrigatório
          </small>
        </div>

        <b-form-group label="Categoria" label-for="category">
          <b-form-select id="category" v-model="form.category" :options="categoryOptions" required />
        </b-form-group>

        <b-form-group label="Evidência (Foto ou Vídeo)" label-for="file-upload">
          <b-form-file
            id="file-upload"
            v-model="form.imageFile"
            :state="Boolean(form.imageFile)"
            placeholder="Escolha um arquivo ou solte aqui..."
            drop-placeholder="Solte o arquivo aqui..."
            accept="image/*, video/*"
          ></b-form-file>
          <small class="text-muted">Formatos aceitos: JPG, PNG, MP4. Máx: 1 arquivo.</small>
        </b-form-group>

        <div class="text-right mt-4">
          <b-button type="submit" variant="primary" :disabled="loading || !form.latitude">
            <b-spinner small v-if="loading" class="mr-1"></b-spinner>
            {{ loading ? 'Enviando...' : 'Registrar Reclamação' }}
          </b-button>
        </div>

      </b-form>
    </b-card>
  </div>
</template>

<script>
import axios from '@/libs/axios' // Ajuste conforme seu arquivo de axios
import { BCard, BForm, BFormGroup, BFormInput, BFormTextarea, BRow, BCol, BButton, BFormSelect, BFormFile, BSpinner } from 'bootstrap-vue'
import { MapPinIcon, CheckIcon } from 'vue-feather-icons'

export default {
  name: 'ComplaintForm',
  components: {
    BCard, BForm, BFormGroup, BFormInput, BFormTextarea, BRow, BCol, BButton, BFormSelect, BFormFile, BSpinner,
    MapPinIcon, CheckIcon
  },
  data() {
    return {
      loading: false,
      gpsLoading: false,
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
      // Opções baseadas no seu Enum C# (ComplaintCategory)
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
    // 1. Captura Lat/Long do Navegador
    getLocation() {
      if (!("geolocation" in navigator)) {
        this.$toast?.error("Seu navegador não suporta geolocalização.");
        return;
      }

      this.gpsLoading = true;
      navigator.geolocation.getCurrentPosition(
        (position) => {
          this.form.latitude = position.coords.latitude;
          this.form.longitude = position.coords.longitude;
          this.gpsLoading = false;
          this.$toast?.success("Localização capturada!");
        },
        (error) => {
          console.error(error);
          this.$toast?.error("Erro ao obter GPS. Permita o acesso ou tente novamente.");
          this.gpsLoading = false;
        }
      );
    },

    // 2. Envia para o Backend (FormData)
    async submitComplaint() {
      if (!this.form.latitude || !this.form.longitude) {
        this.$toast?.warning("Por favor, capture a localização (GPS) antes de enviar.");
        return;
      }

      this.loading = true;

      try {
        // IMPORTANTE: Criar FormData para enviar arquivos
        const formData = new FormData();
        
        formData.append('Title', this.form.title);
        formData.append('Description', this.form.description);
        formData.append('Location', this.form.location);
        formData.append('Neighborhood', this.form.neighborhood);
        
        // Números precisam ser convertidos corretamente, mas append aceita string/blob
        formData.append('Latitude', this.form.latitude.toString().replace('.', ',')); // Ajuste regional as vezes necessario, mas o .toString() padrao costuma funcionar. O ideal é mandar com ponto.
        formData.append('Longitude', this.form.longitude.toString());
        formData.append('Category', this.form.category);

        // Anexa o arquivo se existir
        if (this.form.imageFile) {
          formData.append('Image', this.form.imageFile);
        }

        // Envia para a API
        // O Axios detecta FormData e configura o Content-Type: multipart/form-data automaticamente
        await axios.post('/complaints', formData);

        this.$toast?.success("Reclamação registrada com sucesso!");
        
        // Limpa o formulário
        this.resetForm();

        // Opcional: Redirecionar
        // this.$router.push({ name: 'home' });

      } catch (error) {
        console.error("Erro no envio:", error);
        const msg = error.response?.data?.mensagem || "Erro ao conectar com o servidor.";
        this.$toast?.error(msg);
      } finally {
        this.loading = false;
      }
    },

    resetForm() {
      this.form = {
        title: '',
        description: '',
        location: '',
        neighborhood: '',
        latitude: null,
        longitude: null,
        category: 0,
        imageFile: null
      };
    }
  }
}
</script>

<style scoped>
.spin-icon {
  animation: spin 1s linear infinite;
}
@keyframes spin { 100% { transform: rotate(360deg); } }
</style>