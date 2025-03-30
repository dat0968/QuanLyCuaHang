<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';

const email = ref('');
const message = ref('');
const success = ref(false);
const router = useRouter();

const handleForgotPassword = async () => {
  message.value = '';
  success.value = false;

  try {
    const response = await fetch(`https://localhost:7139/api/Account/ForgotPasswordCustomer?email=${encodeURIComponent(email.value.trim())}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Lỗi HTTP ${response.status}: ${errorText || 'Không có chi tiết'}`);
    }

    const data = await response.json();

    if (data.success) {
      success.value = true;
      message.value = data.message || 'Mật khẩu tạm thời đã được gửi về email. Vui lòng kiểm tra email!';
      await Swal.fire({
        icon: 'success',
        title: 'Thành công!',
        text: message.value,
        confirmButtonText: 'OK',
      });
      router.push('/Login'); // Chuyển hướng về trang đăng nhập
    } else {
      success.value = false;
      message.value = data.message || 'Có lỗi xảy ra, vui lòng thử lại!';
      await Swal.fire({
        icon: 'error',
        title: 'Lỗi!',
        text: message.value,
        confirmButtonText: 'OK',
      });
    }
  } catch (error) {
    console.error('Lỗi trong handleForgotPassword:', {
      message: error.message,
      name: error.name,
      stack: error.stack,
    });
    success.value = false;
    message.value = error.message || 'Có lỗi xảy ra, vui lòng thử lại!';
    await Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: message.value,
      confirmButtonText: 'OK',
    });
  }
};
</script>
<template>
  <div>
    <div class="xp-authenticate-bg"></div>
    <!-- Start XP Container -->
    <div id="xp-container" class="xp-container">
      <!-- Start Container -->
      <div class="container">
        <!-- Start XP Row -->
        <div class="row vh-100 align-items-center">
          <!-- Start XP Col -->
          <div class="col-lg-12">
            <!-- Start XP Auth Box -->
            <div class="xp-auth-box">
              <div class="card">
                <div class="card-body">
                  <h3 class="text-center mt-0 m-b-15">
                    <a href="index.html" class="xp-web-logo">
                      <img src="../../assets/admin/images/logo.svg" height="40" alt="logo" />
                    </a>
                  </h3>
                  <div class="p-3">
                    <form @submit.prevent="handleForgotPassword">
                      <div class="text-center mb-3">
                        <h4 class="text-black">Quên mật khẩu</h4>
                        <p class="text-muted">
                          Bạn đã nhớ lại mật khẩu ?
                          <router-link to="/Login" class="text-primary">Đăng nhập</router-link> 
                        </p>
                      </div>
                      <p class="text-muted text-center m-b-30">
                        Chúng tôi sẽ gửi mật khẩu mới tạm thời cho bạn
                      </p>
                      <div v-if="message" :class="['alert', success ? 'alert-success' : 'alert-danger', 'text-center']">
                        {{ message }}
                      </div>
                      <div class="form-group">
                        <input
                          v-model="email"
                          type="email"
                          class="form-control"
                          id="email"
                          placeholder="Nhập email đăng kí tài khoản"
                          required
                        />
                      </div>
                      <button type="submit" class="btn btn-primary btn-rounded btn-lg btn-block">
                        Gửi mật khẩu mới
                      </button>
                    </form>
                  </div>
                </div>
              </div>
            </div>
            <!-- End XP Auth Box -->
          </div>
          <!-- End XP Col -->
        </div>
        <!-- End XP Row -->
      </div>
      <!-- End Container -->
    </div>
    <!-- End XP Container -->
  </div>
</template>



<style scoped>
.alert-success {
  margin-bottom: 15px;
  color: #155724;
  background-color: #d4edda;
  border-color: #c3e6cb;
}
.alert-danger {
  margin-bottom: 15px;
  color: #721c24;
  background-color: #f8d7da;
  border-color: #f5c6cb;
}
.text-primary {
  color: #007bff;
  text-decoration: none;
}
.text-primary:hover {
  text-decoration: underline;
}
</style>