<template>
  <div class="landing-page">
    <b-navbar toggleable="lg" type="dark" variant="dark" class="px-4">
      <b-navbar-brand href="#" class="font-weight-bold">FalaBará</b-navbar-brand>

      <b-navbar-nav class="ml-auto">
        <div v-if="!isLoggedIn">
          <b-button variant="outline-light" :to="{ name: 'auth-login' }" class="mr-2">Entrar</b-button>
          <b-button variant="warning" :to="{ name: 'auth-register' }">Cadastrar</b-button>
        </div>
        <div v-else class="d-flex align-items-center">
          <span class="text-white mr-3">Olá, Cidadão</span>
          <b-button variant="outline-danger" size="sm" @click="logout">Sair</b-button>
        </div>
      </b-navbar-nav>
    </b-navbar>

    <div class="hero bg-sabara text-white text-center py-5">
      <b-container>
        <h1 class="display-4 font-weight-bold">Transforme Sabará com sua voz</h1>
        <p class="lead mb-4">Relate problemas, acompanhe soluções e ajude a prefeitura a priorizar.</p>

        <b-button size="lg" variant="light" class="text-sabara font-weight-bold shadow" @click="goToNewComplaint">
          <plus-circle-icon /> REGISTRAR RECLAMAÇÃO
        </b-button>
      </b-container>
    </div>

    <b-container class="py-5">
      <div class="mb-5">
        <h3 class="font-weight-bold mb-4 border-left-sabara pl-3">Transparência em Tempo Real</h3>
        <sabara-dashboard />
      </div>

      <hr class="my-5">

      <div>
        <h3 class="font-weight-bold mb-4 border-left-sabara pl-3">Últimas Ocorrências</h3>
        <complaint-feed />
      </div>
    </b-container>
  </div>
</template>

<script>
import SabaraDashboard from './SabaraDashboard.vue'
import ComplaintFeed from './ComplaintsFeed.vue'
import { PlusCircleIcon } from 'vue-feather-icons'

export default {
  name: 'LandingPage', 
  components: { SabaraDashboard, ComplaintFeed, PlusCircleIcon },
  data () {
    return {
      isLoggedIn: false
    }
  },
  created () {
    this.isLoggedIn = !!localStorage.getItem('token')
  },
  methods: {
    goToNewComplaint () {
      if (this.isLoggedIn) {
        this.$router.push({ name: 'register-complaint' })
      } else {
        this.$swal({
          title: 'Identifique-se',
          text: 'Para registrar uma reclamação oficial, você precisa entrar ou se cadastrar.',
          icon: 'warning',
          showCancelButton: true,
          confirmButtonText: 'Entrar / Cadastrar',
          cancelButtonText: 'Só olhar'
        }).then((result) => {
          if (result.isConfirmed) this.$router.push({ name: 'auth-login' })
        })
      }
    },
    logout () {
      localStorage.removeItem('token')
      localStorage.removeItem('userData')
      this.isLoggedIn = false
      this.$toast.info('Você saiu do sistema.')
      this.$router.go()
    }
  }
}
</script>

<style scoped>
.bg-sabara { background-color: #8B0000; }
.text-sabara { color: #8B0000; }
.border-left-sabara { border-left: 5px solid #8B0000; }
</style>
