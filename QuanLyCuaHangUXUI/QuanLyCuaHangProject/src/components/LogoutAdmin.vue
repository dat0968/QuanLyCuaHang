<template>
  <a class="dropdown-item py-3 text-white text-center font-16" href="#"
    >Chào mừng, {{ userName }}</a
  >
  <!-- dán link sang trang cập nhật vào dòng hồ sơ -->
  <a class="dropdown-item" href="#"><i class="icon-user text-primary mr-2"></i> Hồ sơ</a>

  <QrScanAndShiftManagerModal />
  <a v-if="isLoggedIn" class="dropdown-item" @click.prevent="handleLogout">
    <i class="icon-power text-danger mr-2"></i> Đăng xuất
  </a>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue'
import Cookies from 'js-cookie'
import Swal from 'sweetalert2'
import { ReadToken } from '../Authentication_Authorization/auth.js'

import QrScanAndShiftManagerModal from '@/components/shift/QrScanAndShiftManagerModal.vue'

components: {
  QrScanAndShiftManagerModal
}
// Kiểm tra trạng thái đăng nhập
const isLoggedIn = computed(() => {
  return !!Cookies.get('accessToken')
})

// Lấy tên người dùng từ accessToken
const userName = ref('')
onMounted(() => {
  const accessToken = Cookies.get('accessToken')
  if (accessToken) {
    const tokenData = ReadToken(accessToken)
    if (tokenData && tokenData.Name) {
      userName.value = tokenData.Name
    }
  }
})
// Hàm xử lý đăng xuất
const handleLogout = async () => {
  try {
    const refreshToken = Cookies.get('refreshToken')
    if (!refreshToken) {
      await Swal.fire({
        icon: 'warning',
        title: 'Cảnh báo',
        text: 'Không tìm thấy refresh token trong cookies',
        confirmButtonText: 'OK',
      })
      return
    }

    // Gọi API Logout
    const response = await fetch('https://localhost:7139/api/Account/Logout', {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${Cookies.get('accessToken')}`,
      },
      body: JSON.stringify(refreshToken),
    })

    // Kiểm tra Content-Type
    const contentType = response.headers.get('content-type')
    let data = null
    let errorMessage = 'Lỗi không xác định'

    // Xử lý phản hồi
    if (contentType && contentType.includes('application/problem+json')) {
      data = await response.json()
      console.log('Problem details:', data)
      if (data.errors && data.errors.RefreshToken) {
        errorMessage = data.errors.RefreshToken.join(', ')
      } else {
        errorMessage = data.detail || data.title || `Lỗi HTTP ${response.status}`
      }
    } else if (contentType && contentType.includes('application/json')) {
      data = await response.json()
      console.log('Response data:', data)
      errorMessage = data.Message || `Lỗi HTTP ${response.status}`
    } else {
      errorMessage = `Lỗi HTTP ${response.status}: Phản hồi không phải JSON`
    }

    if (response.ok && data?.success) {
      // Xóa thông tin đăng nhập
      Cookies.remove('accessToken')
      Cookies.remove('refreshToken')
      await Swal.fire({
        icon: 'success',
        title: 'Thành công',
        text: 'Đăng xuất thành công',
        confirmButtonText: 'OK',
      })
      window.location.href = '/'
    } else {
      await Swal.fire({
        icon: 'error',
        title: 'Lỗi',
        text: `Đăng xuất thất bại: ${errorMessage}`,
        confirmButtonText: 'OK',
      })
    }
  } catch (error) {
    console.error('Lỗi khi đăng xuất:', error)
    await Swal.fire({
      icon: 'error',
      title: 'Lỗi',
      text: `Đã xảy ra lỗi khi đăng xuất: ${error.message || 'Vui lòng kiểm tra console'}`,
      confirmButtonText: 'OK',
    })
  }
}
</script>

<style scoped>
.logout-container {
  display: inline-block;
}

.logout-link {
  color: #000; /* Màu đen giống các liên kết trong menu */
  font-size: 14px;
  font-weight: 500;
  text-decoration: none;
  padding: 8px 16px;
  transition: color 0.3s ease;
}

.logout-link:hover {
  color: #ff5733; /* Màu đỏ cam khi hover, giống các liên kết trong menu */
}
</style>
