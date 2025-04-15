import { jwtDecode } from 'jwt-decode'
import toastr from 'toastr'
import router from '@/router/index'
import Cookies from 'js-cookie' // Import js-cookie

const authService = {
  isAccess() {
    return !!Cookies.get('accessToken')
  },

  getRole() {
    const token = Cookies.get('accessToken')
    if (!token) return null

    try {
      return jwtDecode(token).role
    } catch (error) {
      console.error('Invalid JWT token:', error)
      localStorage.removeItem('accessToken')
      return null
    }
  },

  /**
   * Chỉ kiểm tra xem role của người dùng có nằm trong danh sách roles được phép hay không.
   * Không thực hiện chuyển hướng hoặc hiển thị lỗi.
   *
   * @param {string[]} allowedRoles Mảng các roles được phép.
   * @returns {boolean} `true` nếu người dùng có một trong các role được phép, ngược lại `false`.
   */
  hasAnyRole(allowedRoles, navigateToError = false) {
    const roleUser = this.getRole()
    if (!roleUser) {
      if (navigateToError) {
        router.push({ path: '/Error/401' })
        toastr.error('Bạn không có quyền truy cập trang này.')
      }
      return false // Không có role, không có quyền truy cập
    }
    return allowedRoles.includes(roleUser)
  },

  isUserHaveRole(
    rolesRequest,
    isCustomerHasPower = false,
    navigateToLogin = false,
    navigateToError = false,
  ) {
    const hasAccess = this.isAccess()
    const roleUser = this.getRole()

    if (!hasAccess || !roleUser) {
      if (navigateToLogin) {
        router.push({
          path: '/login',
          state: { from: router.currentRoute.fullPath },
        })
        toastr.info('Bạn chưa đăng nhập.')
        return false
      }

      if (navigateToError) {
        router.push({ path: '/Error/401' })
        toastr.error('Bạn không có quyền truy cập trang này.')
        return false
      }

      return false
    }

    if (roleUser === 'Customer' && !isCustomerHasPower) {
      toastr.warning('Bạn không có quyền thực hiện hành động này.')
      return false
    }

    const hasRequiredRole = rolesRequest.includes(roleUser)

    if (!hasRequiredRole) {
      if (navigateToError) {
        router.push({ path: '/Error/403' })
        toastr.error('Bạn không có quyền truy cập trang này.')
        return false
      }
      toastr.warning('Bạn không có quyền truy cập.')
      return false
    }

    return true
  },
}

export default authService
