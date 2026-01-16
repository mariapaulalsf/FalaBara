<template>
  <div>
    <div class="row mb-3">
      <div class="col-md-4">
        <base-input placeholder="Buscar..." v-model="filters.search" />
      </div>

      <div class="col-md-3">
        <select class="form-control" v-model="filters.category">
          <option value="">Categoria</option>
          <option value="3">Iluminação</option>
          <option value="1">Infraestrutura</option>
        </select>
      </div>

      <div class="col-md-3">
        <select class="form-control" v-model="filters.order">
          <option value="recent">Mais Recentes</option>
          <option value="votes">Mais Votadas</option>
        </select>
      </div>

      <div class="col-md-2">
        <base-button type="info" @click="load">Filtrar</base-button>
      </div>
    </div>

    <div class="row">
      <div class="col-md-6" v-for="c in complaints" :key="c.id">
        <card>
          <h4>{{ c.title }}</h4>
          <p>{{ c.description }}</p>
          <p><strong>Bairro:</strong> {{ c.neighborhood }}</p>
          <p><strong>Status:</strong> {{ c.statusLabel }}</p>

          <base-button size="sm" @click="vote(c.id)">
            ❤️ {{ c.votes }}
          </base-button>
        </card>
      </div>
    </div>
  </div>
</template>

<script>
import { getComplaints, vote } from "@/services/complaint.service";

export default {
  data() {
    return {
      complaints: [],
      filters: {
        search: "",
        category: "",
        order: "recent",
        page: 1
      }
    };
  },
  mounted() {
    this.load();
  },
  methods: {
    async load() {
      const { data } = await getComplaints(this.filters);
      this.complaints = data.items;
    },
    async vote(id) {
      await vote(id);
      this.load();
    }
  }
};
</script>
