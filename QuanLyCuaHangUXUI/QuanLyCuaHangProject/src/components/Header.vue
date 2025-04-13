<script setup>
import { ref } from 'vue';
import CartButton from './CartButton.vue';
import LoginButton from './LoginButton.vue';
import '@fortawesome/fontawesome-free/css/all.min.css'
import OrderClientButton from './ui/OrderClientButton.vue'
const cartComponent = ref(null)
</script>
<template>
  <header class="main_menu home_menu">
    <div class="container">
      <div class="row align-items-center">
        <div class="col-lg-12">
          <nav class="navbar navbar-expand-lg navbar-light">
            <RouterLink class="navbar-brand" to="/">
              <img src="../assets/client/img/logo.png" alt="logo" />
            </RouterLink>
            <button
              class="navbar-toggler"
              type="button"
              data-toggle="collapse"
              data-target="#navbarSupportedContent"
              aria-controls="navbarSupportedContent"
              aria-expanded="false"
              aria-label="Toggle navigation"
            >
              <span class="navbar-toggler-icon"></span>
            </button>

            <div
              class="collapse navbar-collapse main-menu-item justify-content-end"
              id="navbarSupportedContent"
            >
              <ul class="navbar-nav">
                <li class="nav-item">
                  <RouterLink class="nav-link" to="/"> Trang chủ </RouterLink>
                </li>
                <li class="nav-item">
                    <RouterLink class="nav-link" to="/profile">
                    <span>Hồ sơ cá nhân</span>
                  </RouterLink>
                </li>
                <li class="nav-item">
                  <RouterLink class="nav-link" to="/about"> Về chúng tôi</RouterLink>
                </li>
                <li class="nav-item">
                  <RouterLink class="nav-link" to="/menu"> Thực đơn </RouterLink>
                </li>
                <li class="nav-item">
                  <RouterLink class="nav-link" to="/chefs">Đầu bếp</RouterLink>
                </li>
                <li class="nav-item">
                  <a class="nav-link" href="">Liên hệ</a>
                </li>
              </ul>
            </div>
            <div class="menu_btn">
              <!-- Nút giỏ hàng -->
              <CartButton />
              
              <!-- Cart modal -->
              <cart ref="cartComponent" />
              <!-- Nút đơn hàng -->
              <OrderClientButton />
              <!-- Nút đăng nhập -->
              <LoginButton />
            </div>
          </nav>
        </div>
      </div>
    </div>
  </header>
</template>


<style scoped>
.menu_btn {
  display: flex;
  align-items: center;
  gap: 10px;
}
</style>
