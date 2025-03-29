<script setup>
import { RouterLink, RouterView } from 'vue-router'
import '../../assets/client/css/bootstrap.min.css'
import '../../assets/client/css/animate.css'
import '../../assets/client/css/owl.carousel.min.css'
import '../../assets/client/css/themify-icons.css'
import '../../assets/client/css/flaticon.css'
import '../../assets/client/css/magnific-popup.css'
import '../../assets/client/css/slick.css'
import '../../assets/client/css/gijgo.min.css'
import '../../assets/client/css/nice-select.css'
import '../../assets/client/css/all.css'
import '../../assets/client/css/style.css'
import '../../assets/client/js/jquery-1.12.1.min.js'
import '@popperjs/core'
import '../../assets/client/js/bootstrap.min.js'
import '../../assets/client/js/jquery.magnific-popup.js'
import 'swiper/css'
import '../../assets/client/js/masonry.pkgd.js'
import '../../assets/client/js/owl.carousel.min.js'
import '../../assets/client/js/slick.min.js'
import '../../assets/client/js/gijgo.min.js'
import '../../assets/client/js/jquery.nice-select.min.js'
import '../../assets/client/js/custom.js'
import Header from '../../../src/components/Header.vue'
import Footer from '../../../src/components/Footer.vue'
</script>

<template>
  <div>
    <!--::header part start::-->
    <Header />
    <!-- Header part end-->

    <RouterView />

    <!-- footer part start-->
    <Footer />
    <!-- footer part end-->
  </div>
</template>

<style scoped></style>
