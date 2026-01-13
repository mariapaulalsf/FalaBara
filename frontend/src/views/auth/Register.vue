<template>
  <div class="auth-wrapper auth-v1 px-2">
    <div class="auth-inner py-2">
      <b-card class="mb-0 sabara-card">

        <div class="text-center mb-2">
            <h2 class="brand-text text-sabara font-weight-bold">Cadastro</h2>
            <p class="card-text text-muted">Junte-se à comunidade de Sabará</p>
        </div>

        <validation-observer ref="registerForm">
          <b-form class="auth-register-form mt-2" @submit.prevent="handleRegister">

            <b-form-group label="Nome Completo" label-for="register-username">
              <validation-provider #default="{ errors }" name="Nome" rules="required|min:3">
                <b-form-input id="register-username" v-model="userData.nome" placeholder="Maria da Silva" />
                <small class="text-danger">{{ errors[0] }}</small>
              </validation-provider>
            </b-form-group>

            <b-form-group label="CPF" label-for="register-cpf">
              <validation-provider #default="{ errors }" name="CPF" rules="required">
                <b-form-input id="register-cpf" v-model="userData.cpf" placeholder="000.000.000-00" />
                <small class="text-danger">{{ errors[0] }}</small>
              </validation-provider>
            </b-form-group>

            <b-form-group label="E-mail" label-for="register-email">
              <validation-provider #default="{ errors }" name="Email" rules="required|email">
                <b-form-input id="register-email" v-model="userData.email" placeholder="maria@exemplo.com" />
                <small class="text-danger">{{ errors[0] }}</small>
              </validation-provider>
            </b-form-group>

            <b-form-group label="Senha" label-for="register-password">
              <validation-provider #default="{ errors }" name="Senha" rules="required|min:6">
                <b-form-input id="register-password" v-model="userData.senha" type="password" placeholder="············" />
                <small class="text-danger">{{ errors[0] }}</small>
              </validation-provider>
            </b-form-group>

            <b-button variant="sabara" type="submit" block :disabled="loading">
              <span v-if="loading" class="spinner-border spinner-border-sm"/>
              <span v-else>Cadastrar</span>
            </b-button>
          </b-form>
        </validation-observer>

        <b-card-text class="text-center mt-2">
          <span>Já possui conta? </span>
          <router-link :to="{ name: 'auth-login' }">
            <span class="text-sabara-secondary font-weight-bold">Faça Login</span>
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
  name: 'AuthRegister', // Corrigido o nome do componente
  components: { ValidationProvider, ValidationObserver },
  setup () {
    if (!store.hasModule('auth')) store.registerModule('auth', require('@/store/modules/auth').default)

    const toast = useToast()
    const loading = ref(false)
    const userData = ref({
      nome: '',
      email: '',
      cpf: '',
      senha: '',
      userType: 0,
      department: ''
    })

    return { userData, loading, toast }
  },
  methods: {
    handleRegister () {
      this.$refs.registerForm.validate().then(success => {
        if (success) {
          this.loading = true
          store.dispatch('auth/register', this.userData)
            .then(() => {
              this.toast.success('Cadastro realizado! Faça login.')
              router.push({ name: 'auth-login' })
            })
            .catch(error => {
              this.toast.error(error.response?.data?.message || 'Erro no cadastro.')
            })
            .finally(() => {
              this.loading = false // Corrigido o arrow function
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
  background: rgba(0, 0, 0, 0.6);
}
.auth-inner {
  position: relative;
  z-index: 1;
  width: 100%;
  max-width: 500px;
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
