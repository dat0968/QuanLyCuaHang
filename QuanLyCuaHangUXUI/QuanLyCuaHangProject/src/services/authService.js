import { jwtDecode } from 'jwt-decode'

const authService = {
  isAccess() {
    return !!localStorage.getItem('accessToken')
  },

  getRole() {
    const token = localStorage.getItem('accessToken')
    if (token) {
      return jwtDecode(token).role
    }
    return null
  },

  isUserHaveRole(rolesRequest, isCustomerHasPower = false) {
    const roleUser = this.getRole()
    if (roleUser === 'Customer' && !isCustomerHasPower) {
      return false
    }
    return rolesRequest.includes(roleUser)
  },
}

export default authService
