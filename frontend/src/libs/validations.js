import { extend } from 'vee-validate'
import {
  required,
  email,
  min,
  confirmed,
  digits
} from 'vee-validate/dist/rules'

extend('required', {
  ...required,
  message: 'Este campo é obrigatório'
})

extend('email', {
  ...email,
  message: 'Digite um email válido'
})

extend('min', {
  ...min,
  message: 'Mínimo de {length} caracteres'
})

extend('confirmed', {
  ...confirmed,
  message: 'As senhas não conferem'
})

extend('digits', {
  ...digits,
  message: 'O campo deve ter {length} dígitos'
})

export { required, email, min, confirmed, digits }
