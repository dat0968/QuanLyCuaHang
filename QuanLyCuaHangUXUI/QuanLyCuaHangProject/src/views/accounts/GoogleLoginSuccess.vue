<script setup>
import { useRouter, useRoute } from 'vue-router';
import { onMounted } from 'vue';
import Swal from 'sweetalert2';

const router = useRouter();
const route = useRoute();

onMounted(async () => {
  const accessToken = route.query.access_token; 
  const refreshToken = route.query.refresh_token;

  if (accessToken && refreshToken) {
    localStorage.setItem('accessToken', accessToken);
    localStorage.setItem('refreshToken', refreshToken);
    console.log('Google login thành công, lưu token và chuyển hướng...');
    await Swal.fire({
      icon: 'success',
      title: 'Đăng nhập Google thành công!',
      text: 'Chào mừng bạn trở lại.',
      confirmButtonText: 'OK',
    });
    router.push('/'); // Chuyển hướng đến trang chính
  } else {
    console.error('Không nhận được token từ Google login');
    await Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Không thể đăng nhập qua Google. Vui lòng thử lại.',
      confirmButtonText: 'OK',
    });
    router.push('/Login'); // Quay lại trang đăng nhập nếu lỗi
  }
});
</script>

<template>
  <div>
    <h3>Đang xử lý đăng nhập Google...</h3>
  </div>
</template>