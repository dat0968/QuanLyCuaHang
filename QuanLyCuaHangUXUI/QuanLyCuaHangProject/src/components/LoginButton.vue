<template>
    <div class="login-container">
      <!-- Hiển thị dropdown khi chưa đăng nhập -->
      <select v-if="!isLoggedIn" class="login-dropdown" @change="handleSelection($event)">
        <!-- Tùy chọn mặc định -->
        <option value="" disabled selected hidden>Đăng nhập</option>
        <!-- Tùy chọn khi chưa đăng nhập -->
        <option value="customer">Khách hàng</option>
        <option value="staff">Nhân viên</option>
      </select>
  
      <!-- Hiển thị nút đăng xuất khi đã đăng nhập -->
      <button v-else class="logout-btn" @click="handleLogout">Đăng xuất</button>
    </div>
  </template>
  
  <script>
  import { computed } from 'vue';
  import Cookies from 'js-cookie';
  import Swal from 'sweetalert2';
  
  export default {
    name: 'LoginButton',
    setup() {
      const isLoggedIn = computed(() => {
        return !!Cookies.get('accessToken');
      });
  
      const handleSelection = async (event) => {
        const value = event.target.value;
        if (!value) return; // Không làm gì nếu không chọn giá trị
  
        if (value === 'logout') {
          await handleLogout();
        } else if (value === 'customer') {
          window.location.href = '/login'; // Chuyển hướng đến trang đăng nhập khách hàng
        } else if (value === 'staff') {
          window.location.href = '/loginStaff'; // Chuyển hướng đến trang đăng nhập nhân viên
        }
  
        // Reset dropdown về giá trị mặc định
        event.target.value = '';
      };
  
      const handleLogout = async () => {
        try {
          const refreshToken = Cookies.get('refreshToken');
          if (!refreshToken) {
            await Swal.fire({
              icon: 'warning',
              title: 'Cảnh báo',
              text: 'Không tìm thấy refresh token trong cookies',
              confirmButtonText: 'OK'
            });
            return;
          }
  
          // Gọi API Logout
          const response = await fetch('https://localhost:7139/api/Account/Logout', {
            method: 'DELETE',
            headers: {
              'Content-Type': 'application/json',
              'Authorization': `Bearer ${Cookies.get('accessToken')}`
            },
            body: JSON.stringify(refreshToken)
          });
  
          // Kiểm tra Content-Type
          const contentType = response.headers.get('content-type');
          let data = null;
          let errorMessage = 'Lỗi không xác định';
  
          // Xử lý phản hồi
          if (contentType && contentType.includes('application/problem+json')) {
            data = await response.json();
            console.log('Problem details:', data);
            if (data.errors && data.errors.RefreshToken) {
              errorMessage = data.errors.RefreshToken.join(', ');
            } else {
              errorMessage = data.detail || data.title || `Lỗi HTTP ${response.status}`;
            }
          } else if (contentType && contentType.includes('application/json')) {
            data = await response.json();
            console.log('Response data:', data);
            errorMessage = data.Message || `Lỗi HTTP ${response.status}`;
          } else {
            errorMessage = `Lỗi HTTP ${response.status}: Phản hồi không phải JSON`;
          }
  
          if (response.ok && data?.success) {
            // Xóa thông tin đăng nhập
            Cookies.remove('accessToken');
            Cookies.remove('refreshToken');
            await Swal.fire({
              icon: 'success',
              title: 'Thành công',
              text: 'Đăng xuất thành công',
              confirmButtonText: 'OK'
            });
            window.location.href = '/';
          } else {
            await Swal.fire({
              icon: 'error',
              title: 'Lỗi',
              text: `Đăng xuất thất bại: ${errorMessage}`,
              confirmButtonText: 'OK'
            });
          }
        } catch (error) {
          console.error('Lỗi khi đăng xuất:', error);
          await Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: `Đã xảy ra lỗi khi đăng xuất: ${error.message || 'Vui lòng kiểm tra console'}`,
            confirmButtonText: 'OK'
          });
        }
      };
  
      return {
        isLoggedIn,
        handleSelection,
        handleLogout
      };
    }
  };
  </script>
  
  <style scoped>
  .login-container {
    display: inline-block;
  }
  
  .login-dropdown {
    padding: 8px 16px;
    background-color: #ef785e; /* Màu đỏ cam của thương hiệu DINGO */
    color: white;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-weight: 500;
    font-size: 14px; /* Kích thước chữ nhỏ hơn một chút để phù hợp với menu */
    transition: background-color 0.3s ease;
    appearance: none; /* Xóa giao diện mặc định của select */
    -webkit-appearance: none;
    -moz-appearance: none;
    position: relative;
    background-image: url('data:image/svg+xml;utf8,<svg fill="white" height="24" viewBox="0 0 24 24" width="24" xmlns="http://www.w3.org/2000/svg"><path d="M7 10l5 5 5-5z"/></svg>'); /* Mũi tên trắng */
    background-repeat: no-repeat;
    background-position: right 8px center;
    padding-right: 32px; /* Tạo không gian cho mũi tên */
  }
  
  .login-dropdown:hover,
  .login-dropdown:focus {
    background-color: #E04C2D; /* Màu đỏ cam đậm hơn khi hover/focus */
    outline: none;
  }
  
  /* Tùy chỉnh giao diện các tùy chọn trong dropdown */
  .login-dropdown option {
    background-color: white;
    color: #333;
    font-weight: 400;
  }
  
  /* Đảm bảo dropdown không bị ảnh hưởng bởi các style toàn cục */
  .login-dropdown::-ms-expand {
    display: none; /* Ẩn mũi tên mặc định trên IE */
  }
  .logout-btn {
  padding: 8px 16px;
  background-color: #ef785e; /* Màu xanh dương đậm, đồng bộ với dropdown */
  color: white;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-weight: 500;
  font-size: 14px;
  transition: background-color 0.3s ease;
  }

  .logout-btn:hover {
    background-color: #E04C2D;
  }   
  </style>