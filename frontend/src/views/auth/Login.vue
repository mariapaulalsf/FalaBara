<template>
  <div class="auth-wrapper auth-v1 px-2">
    <div class="auth-inner py-2">

      <b-card class="mb-0 sabara-card">

        <div class="text-center mb-2">
            <b-img src="https://upload.wikimedia.org/wikipedia/commons/thumb/1/14/Bras%C3%A3o_de_Sabar%C3%A1.png/180px-Bras%C3%A3o_de_Sabar%C3%A1.png"
                   height="80" alt="Brasão Sabará" class="mb-1"/>
            <h2 class="brand-text text-sabara font-weight-bold">FalaBará</h2>
            <p class="card-text text-muted">Sistema de Ouvidoria Municipal</p>
        </div>

        <validation-observer ref="loginForm">
          <b-form class="auth-login-form mt-2" @submit.prevent="handleLogin">

            <b-form-group label="E-mail" label-for="login-email">
              <validation-provider #default="{ errors }" name="Email" rules="required|email">
                <b-form-input
                  id="login-email"
                  v-model="userEmail"
                  placeholder="cidadao@sabara.mg.gov.br"
                />
                <small class="text-danger">{{ errors[0] }}</small>
              </validation-provider>
            </b-form-group>

            <b-form-group>
              <div class="d-flex justify-content-between">
                <label for="login-password">Senha</label>
              </div>
              <validation-provider #default="{ errors }" name="Senha" rules="required">
                <b-form-input
                  id="login-password"
                  v-model="password"
                  type="password"
                  placeholder="············"
                />
                <small class="text-danger">{{ errors[0] }}</small>
              </validation-provider>
            </b-form-group>

            <b-button variant="sabara" type="submit" block :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm"/>
              <span v-else>Acessar Portal</span>
            </b-button>
          </b-form>
        </validation-observer>

        <b-card-text class="text-center mt-2">
          <span>Novo em Sabará? </span>
          <router-link :to="{ name: 'auth-register' }">
            <span class="text-sabara-secondary font-weight-bold">Crie sua conta</span>
          </router-link>
        </b-card-text>

      </b-card>
    </div>
  </div>
</template>

<script>
import { ValidationProvider, ValidationObserver } from 'vee-validate'
import { ref } from '@vue/composition-api'
import store from '@/store'
import router from '@/router'
import { useToast } from 'vue-toastification/composition'

export default {
  name: 'AuthLogin', // Corrigido o nome
  components: { ValidationProvider, ValidationObserver },
  setup () {
    if (!store.hasModule('auth')) store.registerModule('auth', require('@/store/modules/auth').default)

    const toast = useToast()
    const userEmail = ref('')
    const password = ref('')
    const loading = ref(false)

    return {
      userEmail, password, loading, toast
    }
  },
  methods: {
    handleLogin () {
      this.$refs.loginForm.validate().then(success => {
        if (success) {
          this.loading = true
          store.dispatch('auth/login', {
            email: this.userEmail,
            senha: this.password
          })
            .then(response => {
              this.toast.success(`Bem-vindo, ${response.data.nome}!`, { timeout: 3000 })
              router.push({ name: 'home' })
            })
            .catch(error => {
              const msg = error.response?.data?.message || 'Erro ao conectar ao servidor.'
              this.toast.error(msg)
            })
            .finally(() => {
              this.loading = false // Corrigido
            })
        }
      })
    }
  }
}
</script>

<style scoped>
.auth-wrapper {
  display: flex;
  justify-content: center;
  align-items: center;
  min-height: 100vh;
  background: url('https://sabara.mg.gov.br/wp-content/uploads/2021/04/capa-sabara.jpg') no-repeat center center;
  background-size: cover;
  position: relative;
}
.auth-wrapper::before {
  content: "";
  position: absolute;
  top: 0; left: 0; width: 100%; height: 100%;
  background: rgba(0, 0, 0, 0.5);
}
.auth-inner {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 400px;
}
.sabara-card {
  border-top: 5px solid #8B0000;
  box-shadow: 0 4px 24px 0 rgba(0,0,0,0.1);
}
.text-sabara { color: #8B0000; }
.text-sabara-secondary { color: #D4AF37; }
.btn-sabara {
  background-color: #8B0000;
  border-color: #8B0000;
  color: white;
}
.btn-sabara:hover {
  background-color: #6d0202;
  border-color: #6d0202;
}
</style>
