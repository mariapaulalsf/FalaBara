<template>
  <div class="p-4">
    <b-card title="Painel do Cidadão">
      <b-card-text>
        Bem-vindo ao FalaBará! Você está logado.
      </b-card-text>
      <b-button variant="danger" @click="logout">Sair</b-button>
    </b-card>
  </div>
</template>

<script>
import store from '@/store'
import router from '@/router'

export default {
  name: 'HomePage',
  methods: {
    logout () {
      store.dispatch('auth/logout')
      router.push({ name: 'auth-login' })
    }
  }
}
</script>
