<template>
  <div>
    <div class="row">
      <div class="col-md-8">
        <card>
          <template #header>
            <h4 class="card-title">Registrar Ocorrência</h4>
          </template>

          <form @submit.prevent="submit">
            <base-input
              label="Título"
              v-model="form.title"
              required
            />

            <base-input
              label="Descrição"
              type="textarea"
              v-model="form.description"
              required
            />

            <div class="row">
              <div class="col-md-6">
                <base-input label="Rua / Avenida" v-model="form.street" />
              </div>
              <div class="col-md-6">
                <base-input label="Bairro" v-model="form.neighborhood" />
              </div>
            </div>

            <div class="row">
              <div class="col-md-6">
                <label>Categoria</label>
                <select v-model="form.category" class="form-control">
                  <option value="0">Saúde</option>
                  <option value="1">Infraestrutura</option>
                  <option value="2">Trânsito</option>
                  <option value="3">Iluminação Pública</option>
                  <option value="4">Limpeza Urbana</option>
                  <option value="5">Segurança</option>
                  <option value="6">Educação</option>
                  <option value="7">Meio Ambiente</option>
                  <option value="8">Outros</option>
                </select>
              </div>

              <div class="col-md-6">
                <label>Mídia (Imagem ou Vídeo)</label>
                <input type="file" class="form-control" @change="onFile" />
              </div>
            </div>

            <base-button type="primary" class="mt-3">
              Enviar Ocorrência
            </base-button>
          </form>
        </card>
      </div>

      <div class="col-md-4">
        <card>
          <h5>Localização</h5>
          <p><strong>Latitude:</strong> {{ latitude }}</p>
          <p><strong>Longitude:</strong> {{ longitude }}</p>
        </card>
      </div>
    </div>
  </div>
</template>

<script>
import { createComplaint } from "@/services/complaint.service";

export default {
  data() {
    return {
      form: {
        title: "",
        description: "",
        street: "",
        neighborhood: "",
        category: ""
      },
      latitude: null,
      longitude: null,
      media: null
    };
  },
  mounted() {
    navigator.geolocation.getCurrentPosition(pos => {
      this.latitude = pos.coords.latitude;
      this.longitude = pos.coords.longitude;
    });
  },
  methods: {
    onFile(e) {
      this.media = e.target.files[0];
    },
    async submit() {
      const fd = new FormData();
      Object.entries(this.form).forEach(([k, v]) => fd.append(k, v));
      fd.append("latitude", this.latitude);
      fd.append("longitude", this.longitude);
      fd.append("media", this.media);

      await createComplaint(fd);
      this.$router.push("/feed");
    }
  }
};
