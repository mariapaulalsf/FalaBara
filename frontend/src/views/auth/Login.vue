<template>
  <div class="auth-wrapper d-flex align-items-center justify-content-center py-5 bg-light" style="min-height: 100vh">
    <b-card class="shadow p-4 border-0" style="max-width: 400px; width: 100%">
      <div class="text-center mb-4">
        <h3 class="font-weight-bold text-sabara">Bem-vindo</h3>
        <p class="text-muted">Faça login para continuar</p>
      </div>

      <b-form @submit.prevent="handleLogin">
        <b-form-group label="E-mail">
          <b-form-input type="email" v-model="form.email" required placeholder="seu@email.com" />
        </b-form-group>

        <b-form-group label="Senha">
          <b-form-input type="password" v-model="form.senha" required placeholder="Sua senha" />
        </b-form-group>

        <b-button type="submit" variant="primary" block :disabled="loading" class="mt-4 bg-sabara border-0">
          <b-spinner small v-if="loading" />
          {{ loading ? 'Entrando...' : 'Entrar' }}
        </b-button>
      </b-form>

      <div class="text-center mt-3">
        <small>Não tem conta? <router-link :to="{name: 'auth-register'}">Cadastre-se</router-link></small>
      </div>
    </b-card>
  </div>
</template>

<script>
import axios from '@/libs/axios'

export default {
  name: 'AuthLogin',
  data () {
    return {
      loading: false,
      form: { email: '', senha: '' }
    }
  },
  methods: {
    async handleLogin () {
      this.loading = true
      try {
        // Ajuste a URL conforme seu backend (ex: /auth/login)
        const { data } = await axios.post('/auth/login', this.form)

        // Salva o Token
        localStorage.setItem('token', data.token)

        // Redireciona para a Home ou Dashboard
        this.$toast.success(`Bem-vindo, ${data.user?.name || 'Cidadão'}!`)
        this.$router.push({ name: 'landing' }) // Ou 'home'
      } catch (error) {
        const msg = error.response?.data?.message || 'Email ou senha inválidos.'
        this.$toast.error(msg)
      } finally {
        this.loading = false
      }
    }
  }
}
</script>

<style scoped>
.bg-sabara { background-color: #8B0000; }
.text-sabara { color: #8B0000; }
</style>
