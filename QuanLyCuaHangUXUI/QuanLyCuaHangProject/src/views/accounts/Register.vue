<script setup>
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import Swal from 'sweetalert2';
import { GetApiUrl } from '@constants/api';

const hoTen = ref('');
const tenTaiKhoan = ref('');
const email = ref('');
const matKhau = ref('');
const termsAccepted = ref(false);
const errorMessage = ref('');
const usernameValid = ref(true); // Trạng thái hợp lệ của tên tài khoản
const emailValid = ref(true); // Trạng thái hợp lệ của email
const router = useRouter();
const getApiUrl = GetApiUrl();

// Hàm kiểm tra định dạng mật khẩu
const isValidPassword = (password) => {
  const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{6,}$/;
  return passwordRegex.test(password);
};
const isValidUsernameFormat = (username) => {
  const usernameRegex =  /^[a-zA-Z0-9_]+$/; // Chỉ cho phép chữ cái và số
  // Nếu muốn cho phép dấu gạch dưới, dùng: /^[a-zA-Z0-9_]+$/
  return usernameRegex.test(username);
};
// Hàm kiểm tra tên tài khoản
const checkUsername = async () => {
  if (!isValidUsernameFormat(tenTaiKhoan.value.trim())) {
    await Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Tên tài khoản không được chứa ký tự đặc biệt!',
      confirmButtonText: 'OK',
    });
    usernameValid.value = false;
    return;
  }
  try {
    const response = await fetch(`${getApiUrl}/api/Account/checkUsername?username=${tenTaiKhoan.value.trim()}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Lỗi HTTP ${response.status}`);
    }

    const data = await response.json();
    if (!data.success) {
      await Swal.fire({
        icon: 'error',
        title: 'Lỗi!',
        text: data.message || 'Tên tài khoản này đã tồn tại',
        confirmButtonText: 'OK',
      });
      usernameValid.value = false;
    } else {
      usernameValid.value = true;
    }
  } catch (error) {
    console.error('Lỗi khi kiểm tra tên tài khoản:', error);
    await Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Có lỗi xảy ra khi kiểm tra tên tài khoản!',
      confirmButtonText: 'OK',
    });
    usernameValid.value = false;
  }
};

// Hàm kiểm tra email
const checkEmail = async () => {
  
  try {
    const response = await fetch(`${getApiUrl}/api/Account/checkEmail?email=${email.value.trim()}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    });

    if (!response.ok) {
      throw new Error(`Lỗi HTTP ${response.status}`);
    }

    const data = await response.json();
    if (!data.success) {
      await Swal.fire({
        icon: 'error',
        title: 'Lỗi!',
        text: data.message || 'Email này đã tồn tại',
        confirmButtonText: 'OK',
      });
      emailValid.value = false;
    } else {
      emailValid.value = true;
    }
  } catch (error) {
    console.error('Lỗi khi kiểm tra email:', error);
    await Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Có lỗi xảy ra khi kiểm tra email!',
      confirmButtonText: 'OK',
    });
    emailValid.value = false;
  }
};

const handleRegister = async () => {
  errorMessage.value = '';

  // Kiểm tra các trường bắt buộc
  if (!hoTen.value.trim()) {
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Vui lòng nhập họ và tên!',
      confirmButtonText: 'OK',
    });
    return;
  }
  if (!tenTaiKhoan.value.trim()) {
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Vui lòng nhập tên tài khoản!',
      confirmButtonText: 'OK',
    });
    return;
  }
  if (!email.value.trim()) {
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Vui lòng nhập email!',
      confirmButtonText: 'OK',
    });
    return;
  }
  
  if (!matKhau.value) {
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Vui lòng nhập mật khẩu!',
      confirmButtonText: 'OK',
    });
    return;
  }

  // Kiểm tra điều khoản
  if (!termsAccepted.value) {
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Vui lòng đồng ý với các điều khoản và điều kiện!',
      confirmButtonText: 'OK',
    });
    return;
  }

  // Kiểm tra định dạng mật khẩu
  if (!isValidPassword(matKhau.value)) {
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt!',
      confirmButtonText: 'OK',
    });
    return;
  }

  // Kiểm tra trùng lặp tên tài khoản và email
  await checkUsername();
  await checkEmail();

  // Nếu có lỗi trùng lặp, dừng xử lý
  if (!usernameValid.value || !emailValid.value) {
    return;
  }

  try {
    const payload = {
      HoTen: hoTen.value.trim(),
      TenTaiKhoan: tenTaiKhoan.value.trim(),
      Email: email.value.trim(),
      MatKhau: matKhau.value,
    };

    const response = await fetch(getApiUrl + '/api/Account/Register', {
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
      await Swal.fire({
        icon: 'error',
        title: 'Đăng ký thất bại!',
        text: data.message || 'Đăng ký thất bại',
        confirmButtonText: 'OK',
      });
    }
  } catch (error) {
    console.error('Lỗi trong handleRegister:', {
      message: error.message,
      name: error.name,
      stack: error.stack,
    });
    await Swal.fire({
      icon: 'error',
      title: 'Đăng ký thất bại!',
      text: 'Có lỗi xảy ra, vui lòng thử lại!',
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
                    <form @submit.prevent="handleRegister">
                      <div class="text-center mb-3">
                        <h4 class="text-black">Tạo tài khoản</h4>
                        <p class="text-muted">
                          Bạn đã có tài khoản ? 
                          <router-link to="/Login" class="text-primary">Đăng nhập</router-link> 
                          Here
                        </p>
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
                        <small class="form-text text-muted">
                          Mật khẩu phải có ít nhất 6 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt (!@#$%^&*).
                        </small>
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
<!-- thêm 2 cái này vào tên tài khoản và email nếu muốn kiểm tra ngay khi thoát thẻ input (nhìn hơi đau mắt) -->
<!-- @blur="checkUsername" -->
<!-- @blur="checkEmail" -->
<style scoped>
.text-primary {
  color: #007bff;
  text-decoration: none;
}
.text-primary:hover {
  text-decoration: underline;
}
.form-text {
  font-size: 0.875rem;
}
</style>