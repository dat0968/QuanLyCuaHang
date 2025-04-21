<script setup>
import { useRouter, useRoute } from 'vue-router';
import { onMounted } from 'vue';
import Swal from 'sweetalert2';
import Cookies from 'js-cookie';

const router = useRouter();
const route = useRoute();

onMounted(async () => {
  const accessToken = route.query.access_token;
  const refreshToken = route.query.refresh_token;
  const error = route.query.error;

  if (accessToken && refreshToken) {
    Cookies.set('accessToken', accessToken, { expires: 3 / 24 });
    Cookies.set('refreshToken', refreshToken, { expires: 3 / 24 });
    console.log('Google login thành công, lưu token và chuyển hướng...');
    await Swal.fire({
      icon: 'success',
      title: 'Đăng nhập Google thành công!',
      text: 'Chào mừng bạn trở lại.',
      confirmButtonText: 'OK',
    });
    router.push('/'); // Chuyển hướng đến trang chính
  } else if (error) {
    let errorMessage = 'Không thể đăng nhập qua Google. Vui lòng thử lại!';
    if (error.includes('Tài khoản đang bị tạm khóa')) {
      errorMessage = 'Tài khoản của bạn đang bị tạm khóa. Vui lòng liên hệ hỗ trợ!';
    } else if (error.includes('Xác thực Google thất bại')) {
      errorMessage = 'Xác thực Google thất bại. Vui lòng kiểm tra lại thông tin đăng nhập!';
    } else {
      errorMessage = error; // Sử dụng thông báo từ backend nếu không khớp
    }

    await Swal.fire({
      icon: 'error',
      title: 'Đăng nhập thất bại!',
      text: errorMessage,
      confirmButtonText: 'OK',
    });
    router.push('/Login'); // Chuyển hướng về trang đăng nhập
  } else {
    console.error('Không nhận được token hoặc thông báo lỗi từ Google login');
    await Swal.fire({
      icon: 'error',
      title: 'Đăng nhập thất bại!',
      text: 'Có lỗi xảy ra khi xử lý đăng nhập Google. Vui lòng thử lại!',
      confirmButtonText: 'OK',
    });
    router.push('/Login');
  }
});
</script>

<template>
  <div>
    <h3>Đang xử lý đăng nhập Google...</h3>
  </div>
</template>