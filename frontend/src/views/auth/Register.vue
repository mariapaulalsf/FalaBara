<template>
  <div class="auth-wrapper d-flex align-items-center justify-content-center py-5 bg-light" style="min-height: 100vh">
    <b-card class="shadow p-3 border-0" style="max-width: 400px; width: 100%">
      <div class="text-center mb-4">
        <h3 class="font-weight-bold text-sabara">Crie sua Conta</h3>
        <p class="text-muted">Junte-se ao FalaBará</p>
      </div>

      <b-form @submit.prevent="handleRegister">

        <b-form-group label="Nome Completo">
          <b-form-input v-model="form.name" required placeholder="João da Silva" />
        </b-form-group>

        <b-form-group label="CPF">
          <b-form-input v-model="form.cpf" required placeholder="000.000.000-00" v-mask="'###.###.###-##'" />
        </b-form-group>

        <b-form-group label="E-mail">
          <b-form-input type="email" v-model="form.email" required placeholder="seu@email.com" />
        </b-form-group>

        <b-form-group label="Senha">
          <b-form-input type="password" v-model="form.password" required placeholder="Mínimo 6 caracteres" />
        </b-form-group>

        <b-button type="submit" variant="primary" block :disabled="loading" class="mt-4" style="background-color: #8B0000; border: none;">
          <b-spinner small v-if="loading" />
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
import { mask } from 'vue-the-mask' // npm install vue-the-mask se não tiver

export default {
  name: 'AuthRegister', // <--- MUDANÇA AQUI
  directives: { mask },
  data () {
    return {
      loading: false,
      form: {
        name: '',
        cpf: '',
        email: '',
        password: ''
      }
    }
  },
  methods: {
    async handleRegister () {
      this.loading = true
      try {
        // Limpa pontuação do CPF antes de enviar
        const payload = {
          ...this.form,
          cpf: this.form.cpf.replace(/\D/g, '')
        }

        await axios.post('/auth/register', payload)

        this.$swal({
          title: 'Sucesso!',
          text: 'Conta criada. Faça login para continuar.',
          icon: 'success'
        }).then(() => {
          this.$router.push({ name: 'auth-login' })
        })
      } catch (error) {
        const msg = error.response?.data?.message || 'Erro ao criar conta.'
        this.$toast.error(msg)
      } finally {
        this.loading = false
      }
    }
  }
}
</script>
