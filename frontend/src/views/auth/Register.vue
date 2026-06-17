<template>
  <div class="Tela auth-wrapper d-flex align-items-center justify-content-center py-5" style="min-height: 100vh">
    <b-card class="shadow p-4 border-0 card_login" style="max-width: 400px; width: 100%">
      <div class="text-center header_card">
        <h3 class="font-weight-bold text-sabara">Criar Conta</h3>
        <p class="text-white">Junte-se ao FalaBará</p>
      </div>

      <b-form @submit.prevent="handleRegister" class="text-white">
        <b-form-group label="Nome" class="m-2">
          <b-form-input v-model="form.name" placeholder="Nome" required />
        </b-form-group>

        <b-form-group label="CPF" class="m-2">
          <b-form-input v-model="form.cpf" required placeholder="Apenas números" />
        </b-form-group>

        <b-form-group label="E-mail" class="m-2">
          <b-form-input type="email" v-model="form.email" required placeholder="SeuEmail@gmail.com"/>
        </b-form-group>

        <b-form-group label="Senha" class="m-2">
          <b-form-input type="password" v-model="form.password" required placeholder="Senha (8 Digitos)"/>
        </b-form-group>

        <div class="d-flex justify-content-center">
          <b-button
            type="submit"
            :disabled="loading"
            class="mt-3 bg-sabara border-0 d-flex align-items-center justify-content-center"
            style="min-width: 200px;"
          >
            {{ loading ? 'Criando...' : 'Cadastrar' }}
          </b-button>
        </div>
      </b-form>

      <div class="text-center mt-3 text-white">
        <small>Já tem conta? <router-link id="faca_login" :to="{name: 'auth-login'}">Faça Login</router-link></small>
      </div>
    </b-card>
  </div>
</template>

<script>
import axios from '@/libs/axios'

export default {
  name: 'AuthRegister',
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
  .Tela{
    background-color: rgb(27, 25, 25);
  }
.header_card{
  position: relative;
  border-radius: 7px 7px 0 0;
  padding-top: 30px;
  right: 40px;
  bottom: 40px;
  background-color: #8B0000;
  width: 400px !important;
}
#faca_login {
  text-decoration: none;
  color: white;
}
#faca_login:hover{
  text-decoration: underline;
  color: #8B0000;
}
.card_login{
  background-color: #474345;
}
.bg-sabara {
  background-color: #8B0000;
}
.text-sabara { color: white; }
</style>
