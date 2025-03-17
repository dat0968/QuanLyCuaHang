<template>
  <header class="main_menu home_menu">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-lg-12">
            <nav class="navbar navbar-expand-lg navbar-light">
              <a class="navbar-brand" href="index.html"> <img src="../assets/client/img/logo.png" alt="logo" /> </a>
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

              <div class="collapse navbar-collapse main-menu-item justify-content-end" id="navbarSupportedContent">
                <ul class="navbar-nav">
                  <li class="nav-item">
                    <RouterLink class="nav-link" to="/" >Home</RouterLink>
                  </li>
                  <li class="nav-item">
                    <RouterLink class="nav-link" to="/about" >About</RouterLink>
                  </li>
                  <li class="nav-item">
                    <RouterLink class="nav-link" to="/menu">Menu</RouterLink>
                  </li>
                  <li class="nav-item">
                    <RouterLink class="nav-link" to="/chefs">Chefs</RouterLink>
                  </li>                
                  <li class="nav-item">
                    <a class="nav-link" href="">Contact</a>
                  </li>
                </ul>
              </div>
              <div class="menu_btn">
                <a href="#" class="btn_1 d-none d-sm-block">book a tabel</a>
              </div>
            </nav>
          </div>
        </div>
      </div>
    </header>
</template>

<script>
import { RouterLink } from 'vue-router';


</script>

<style>

</style>