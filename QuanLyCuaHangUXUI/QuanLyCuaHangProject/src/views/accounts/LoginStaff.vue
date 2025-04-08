<script setup>
import { ref } from 'vue';
import { RouterLink, useRouter } from 'vue-router';
import Swal from 'sweetalert2';
const emailOrUsername = ref('');
const password = ref('');
const errorMessage = ref('');
const router = useRouter();

const handleLogin = async () => {
  errorMessage.value = '';
  try {
    const payload = {
      Email_TenTaiKhoan: emailOrUsername.value.trim(),
      MatKhau: password.value,
    };
    const response = await fetch('https://localhost:7139/api/Account/LoginStaff', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(payload),
    });

    if (!response.ok) {
      const errorText = await response.text();
      console.log('Phản hồi lỗi từ server:', errorText);
      throw new Error(`Lỗi HTTP ${response.status}: ${errorText || 'Không có chi tiết'}`);
    }
    const data = await response.json();

    if (data.success) {
      Cookies.set('accessToken', data.data.accessToken, { expires: 2 / 24 }); // set 2 giờ nha 
      Cookies.set('refreshToken', data.data.refreshToken, { expires: 2 / 24 });
      await Swal.fire({
        icon: 'success',
        title: 'Đăng nhập thành công!',
        text: 'Chào mừng bạn trở lại.',
        confirmButtonText: 'OK',
      });
      // ? Kiểm tra có link web không có đăng nhập trước đó để dùng thì điều hướng lại
      if (router.currentRoute.query && router.currentRoute.query.redirect) {
        router.push(router.currentRoute.query.redirect)
      } else {
        router.push('/')
      }
    } else {
      errorMessage.value = data.Message || 'Đăng nhập thất bại';
    }
  } catch (error) {
    console.error('Lỗi trong handleLogin:', {
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
    <div id="xp-container" class="xp-container">
      <div class="container">
        <div class="row vh-100 align-items-center">
          <div class="col-lg-12">
            <div class="xp-auth-box">
              <div class="card">
                <div class="card-body">
                  <h3 class="text-center mt-0 m-b-15">
                    <a href="index.html" class="xp-web-logo">
                      <img src="../../assets/admin/images/logo.svg" height="40" alt="logo" />
                    </a>
                  </h3>
                  <div class="p-3">
                    <form @submit.prevent="handleLogin">
                      <div class="text-center mb-3">
                        <h4 class="text-black">Đăng nhập</h4>
                      </div>
                      <div v-if="errorMessage" class="alert alert-danger text-center">
                        {{ errorMessage }}
                      </div>
                      <div class="form-group">
                        <input
                          v-model="emailOrUsername"
                          type="text"
                          class="form-control"
                          id="username"
                          placeholder="Tên tài khoản hoặc Email"
                          required
                        />
                      </div>
                      <div class="form-group">
                        <input
                          v-model="password"
                          type="password"
                          class="form-control"
                          id="password"
                          placeholder="Mật khẩu"
                          required
                        />
                      </div>
                      <div class="form-row">
                        <div class="form-group col-6">
                          <div class="custom-control custom-checkbox">
                            <input type="checkbox" class="custom-control-input" id="rememberme" />
                            <label class="custom-control-label" for="rememberme">Nhớ tài khoản</label>
                          </div>
                        </div>
                        <div class="form-group col-6 text-right">
                          <label class="forgot-psw">
                            <router-link to="/ForgotPasswordStaff">Quên mật khẩu</router-link>
                          </label>
                        </div>
                      </div>
                      <button type="submit" class="btn btn-primary btn-rounded btn-lg btn-block">
                        Đăng nhập
                      </button>
                    </form>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.alert-danger {
  margin-bottom: 15px;
}
</style>