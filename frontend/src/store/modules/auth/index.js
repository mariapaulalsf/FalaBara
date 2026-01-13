import axios from '@/libs/axios'

export default {
  namespaced: true,
  state: {
    // Tenta recuperar do localStorage ao iniciar
    token: localStorage.getItem('accessToken') || '',
    user: localStorage.getItem('userData') ? JSON.parse(localStorage.userData) : {}
  },
  getters: {
    isAuthenticated: state => !!state.token,
    currentUser: state => state.user,
    userRole: state => state.user.role || 'Population'
  },
  mutations: {
    SET_TOKEN (state, token) {
      state.token = token
      localStorage.setItem('accessToken', token)
    },
    SET_USER (state, user) {
      state.user = user
      localStorage.setItem('userData', JSON.stringify(user))
    },
    LOGOUT (state) {
      state.token = ''
      state.user = {}
      localStorage.removeItem('accessToken')
      localStorage.removeItem('userData')
    }
  },
  actions: {
    login ({ commit }, credentials) {
      return new Promise((resolve, reject) => {
        // Rota do nosso Backend .NET
        axios.post('/auth/login', credentials)
          .then(response => {
            const { token, email, nome, role, id } = response.data

            commit('SET_TOKEN', token)
            commit('SET_USER', { id, nome, email, role })

            // Configura o header para as próximas chamadas
            axios.defaults.headers.common.Authorization = `Bearer ${token}`

            resolve(response)
          })
          .catch(error => reject(error))
      })
    },
    register ({ commit }, userData) {
      return new Promise((resolve, reject) => {
        axios.post('/auth/register', userData)
          .then(response => resolve(response))
          .catch(error => reject(error))
      })
    },
    logout ({ commit }) {
      commit('LOGOUT')
      // Remove o header
      delete axios.defaults.headers.common.Authorization
    }
  }
}
