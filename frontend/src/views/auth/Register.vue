<template>
  <div class="auth-wrapper d-flex align-items-center justify-content-center py-5 bg-light" style="min-height: 100vh">
    <b-card class="shadow p-4 border-0" style="max-width: 400px; width: 100%">
      <div class="text-center mb-4">
        <h3 class="font-weight-bold text-sabara">Criar Conta</h3>
        <p class="text-muted">Junte-se ao FalaBará</p>
      </div>

      <b-form @submit.prevent="handleRegister">
        <b-form-group label="Nome">
          <b-form-input v-model="form.name" required />
        </b-form-group>

        <b-form-group label="CPF">
          <b-form-input v-model="form.cpf" required placeholder="Apenas números" />
        </b-form-group>

        <b-form-group label="E-mail">
          <b-form-input type="email" v-model="form.email" required />
        </b-form-group>

        <b-form-group label="Senha">
          <b-form-input type="password" v-model="form.password" required />
        </b-form-group>

        <b-button type="submit" block :disabled="loading" class="mt-4 bg-sabara border-0">
          {{ loading ? 'Criando...' : 'Cadastrar' }}
        </b-button>
      </b-form>

      <div class="text-center mt-3">
        <small>Já tem conta? <router-link :to="{name: 'auth-login'}">Faça Login</router-link></small>
      </div>
    </b-card>
  </div>
</template>

<script>
import axios from '@/libs/axios'

export default {
  name: 'AuthRegister', // Nome composto obrigatório
  data () {
    return {
      loading: false,
      form: { name: '', cpf: '', email: '', password: '' }
    }
  },
  methods: {
    async handleRegister () {
      this.loading = true
      try {
        const payload = {
          Nome: this.form.name,
          Email: this.form.email,
          Senha: this.form.password,
          Cpf: this.form.cpf.replace(/\D/g, ''),
          Type: 0
        }
        await axios.post('/auth/registrar', payload)

        this.$swal({
          title: 'Sucesso!',
          text: 'Conta criada com sucesso! Faça login para continuar.',
          icon: 'success'
        }).then(() => {
          this.$router.push({ name: 'auth-login' })
        })
      } catch (error) {
        console.error(error)
        const msg = error.response?.data?.mensagem || 'Erro ao criar conta. Verifique os dados.'
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
