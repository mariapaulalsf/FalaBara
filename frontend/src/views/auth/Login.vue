<template>
  <div class="Tela auth-wrapper d-flex align-items-center justify-content-center py-5" style="min-height: 100vh">
    <b-card class="shadow p-4 border-0 card_login" style="max-width: 400px; width: 100%">
      <div class="header_card text-center mb-4">
        <h3 class="font-weight-bold text-sabara text-white">Bem-vindo</h3>
        <p class="text-white">Faça login para continuar</p>
      </div>

      <b-form @submit.prevent="handleLogin">
        <b-form-group label="E-mail" class="text-white m-2">
          <b-form-input type="email" v-model="form.email" required placeholder="seu@email.com" />
        </b-form-group>

        <b-form-group label="Senha" class="text-white m-2">
          <b-form-input type="password" v-model="form.senha" required placeholder="Sua senha" />
        </b-form-group>

        <div class="d-flex justify-content-center">
          <b-button type="submit" variant="primary" block :disabled="loading" class="mt-4 bg-sabara border-0">
            <b-spinner small v-if="loading" />
            {{ loading ? 'Entrando...' : 'Entrar' }}
          </b-button>
        </div>
      </b-form>

      <div class="text-center mt-3 text-white">
        <small>Não tem conta? <router-link id="faca_login" :to="{name: 'auth-register'}">Cadastre-se</router-link></small>
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
        const { data } = await axios.post('/auth/login', this.form)

        // 1. Salva o Token
        localStorage.setItem('token', data.token)

        // 2. CORREÇÃO: Salva os dados do usuário para a LandingPage usar
        // O backend retorna: { token, id, nome, email, tipo }
        const userData = {
          id: data.id,
          nome: data.nome,
          email: data.email,
          role: data.tipo
        }
        localStorage.setItem('userData', JSON.stringify(userData))

        // Redireciona
        this.$toast.success(`Bem-vindo, ${data.nome || 'Cidadão'}!`)
        this.$router.push({ name: 'landing' })
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
.Tela{
  background-color: rgb(27, 25, 25);
}
.header_card{
  position: relative;
  border-radius: 7px 7px 0 0;
  padding-top: 30px;
  right: 40px;
  bottom: 42px;
  background-color: #8B0000;
  width: 400px !important;
}
.card_login{
  background-color: #474345;
}
#faca_login {
  text-decoration: none;
  color: white;
}
#faca_login:hover{
  text-decoration: underline;
  color: #8B0000;
}
.bg-sabara { background-color: #8B0000; }
.text-sabara { color: #8B0000; }
</style>
