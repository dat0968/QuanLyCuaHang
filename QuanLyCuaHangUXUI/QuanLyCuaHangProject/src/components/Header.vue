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
              <a href="#" class="btn_1 d-none d-sm-block">đặt bàn</a>
            </div>
          </nav>
        </div>
      </div>
    </div>
  </header>
</template>

<script>
import { RouterLink } from 'vue-router'
</script>

<style></style>
