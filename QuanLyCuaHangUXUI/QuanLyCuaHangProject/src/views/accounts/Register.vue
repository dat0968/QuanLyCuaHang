
<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';

const hoTen = ref('');
const tenTaiKhoan = ref('');
const email = ref('');
const matKhau = ref('');
const termsAccepted = ref(false);
const errorMessage = ref('');
const router = useRouter();

const handleRegister = async () => {
  errorMessage.value = '';
  
  if (!termsAccepted.value) {
    errorMessage.value = 'Vui lòng đồng ý với các điều khoản và điều kiện!';
    return;
  }

  try {
    const payload = {
      HoTen: hoTen.value.trim(),
      TenTaiKhoan: tenTaiKhoan.value.trim(),
      Email: email.value.trim(),
      MatKhau: matKhau.value,
    };

    const response = await fetch('https://localhost:7139/api/Account/Register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Lỗi HTTP ${response.status}: ${errorText || 'Không có chi tiết'}`);
    }

    const data = await response.json();

    if (data.success) {
      await Swal.fire({
        icon: 'success',
        title: 'Đăng ký thành công!',
        text: 'Vui lòng đăng nhập để tiếp tục.',
        confirmButtonText: 'OK',
      });
      router.push('/Login');
    } else {
      errorMessage.value = data.message || 'Đăng ký thất bại';
    }
  } catch (error) {
    console.error('Lỗi trong handleRegister:', {
      message: error.message,
      name: error.name,
      stack: error.stack,
    });
    errorMessage.value = error.message || 'Có lỗi xảy ra, vui lòng thử lại!';
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
                    <form @submit.prevent="handleRegister">
                      <div class="text-center mb-3">
                        <h4 class="text-black">Tạo tài khoản</h4>
                        <p class="text-muted">
                          Bạn đã có tài khoản ? 
                          <router-link to="/Login" class="text-primary">Đăng nhập</router-link> 
                          Here
                        </p>
                      </div>
                      <div v-if="errorMessage" class="alert alert-danger text-center">
                        {{ errorMessage }}
                      </div>
                      
                      <div class="login-or">
                        <h6 class="text-muted">Hoặc</h6>
                      </div>
                      <div class="form-group">
                        <input
                          v-model="hoTen"
                          type="text"
                          class="form-control"
                          id="hoTen"
                          placeholder="Họ và tên"
                          required
                        />
                      </div>
                      <div class="form-group">
                        <input
                          v-model="tenTaiKhoan"
                          type="text"
                          class="form-control"
                          id="username"
                          placeholder="Tên tài khoản"
                          required
                        />
                      </div>
                      <div class="form-group">
                        <input
                          v-model="email"
                          type="email"
                          class="form-control"
                          id="email"
                          placeholder="Email"
                          required
                        />
                      </div>
                      <div class="form-group">
                        <input
                          v-model="matKhau"
                          type="password"
                          class="form-control"
                          id="password"
                          placeholder="Mật khẩu"
                          required
                        />
                      </div>
                      <div class="form-group">
                        <div class="custom-control custom-checkbox">
                          <input
                            v-model="termsAccepted"
                            type="checkbox"
                            class="custom-control-input"
                            id="terms"
                            required
                          />
                          <label class="custom-control-label" for="terms">
                            Tôi đồng ý với các điều khoản & điều kiện
                          </label>
                        </div>
                      </div>
                      <button type="submit" class="btn btn-primary btn-rounded btn-lg btn-block">
                        Tạo tài khoản
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
.alert-danger {
  margin-bottom: 15px;
}
.text-primary {
  color: #007bff;
  text-decoration: none;
}
.text-primary:hover {
  text-decoration: underline;
}
</style>