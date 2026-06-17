import api from './api'

export function createComplaint (formData) {
  return api.post('/complaints', formData, {
    headers: { 'Content-Type': 'multipart/form-data' }
  })
}

export function getComplaints (params) {
  return api.get('/complaints', { params })
}

export function vote (id) {
  return api.post(`/complaints/${id}/vote`)
}
