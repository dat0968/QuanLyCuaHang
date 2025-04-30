<script setup>
import '../../assets/admin/images/favicon.ico'
import LogoutAdmin from '../../components/LogoutAdmin.vue'
import '../../assets/admin/plugins/chartist-js/chartist.min.css'
import '../../assets/admin/plugins/datepicker/datepicker.min.css'
import '../../assets/admin/css/bootstrap.min.css'
import '../../assets/admin/css/icons.css'
import '../../assets/admin/css/style.css'
import '../../assets/admin/js/jquery.min.js'
import '@popperjs/core'
import '../../assets/admin/js/bootstrap.min.js'
//import '../../assets/admin/js/modernizr.min.js'
import '../../assets/admin/js/detect.js'
import '../../assets/admin/js/jquery.slimscroll.js'
import '../../assets/admin/js/sidebar-menu.js'
//import '../../assets/admin/plugins/chartist-js/chartist.min.js'
//import '../../assets/admin/plugins/chartist-js/chartist-plugin-tooltip.min.js'
import '../../assets/admin/js/init/to-do-list-init.js'
import '../../assets/admin/plugins/datepicker/datepicker.min.js'
import '../../assets/admin/plugins/datepicker/i18n/datepicker.en.js'
//import '../../assets/admin/js/init/dashborad.js'
import '../../assets/admin/js/main.js'
import '../../assets/admin/images/favicon.ico'
import '../../assets/admin/plugins/chartist-js/chartist.min.css'
import '../../assets/admin/plugins/datepicker/datepicker.min.css'
import '../../assets/admin/css/bootstrap.min.css'
import '../../assets/admin/css/icons.css'
import '../../assets/admin/css/style.css'
import '../../assets/admin/js/jquery.min.js'
import '@popperjs/core'
import '../../assets/admin/js/bootstrap.min.js'
//import '../../assets/admin/js/modernizr.min.js'
import '../../assets/admin/js/detect.js'
import '../../assets/admin/js/jquery.slimscroll.js'
import '../../assets/admin/js/sidebar-menu.js'
//import '../../assets/admin/plugins/chartist-js/chartist.min.js'
//import '../../assets/admin/plugins/chartist-js/chartist-plugin-tooltip.min.js'
import '../../assets/admin/js/init/to-do-list-init.js'
import '../../assets/admin/plugins/datepicker/datepicker.min.js'
import '../../assets/admin/plugins/datepicker/i18n/datepicker.en.js'
//import '../../assets/admin/js/init/dashborad.js'
import '../../assets/admin/js/main.js'
import Cookies from 'js-cookie'
import {ref, onMounted} from 'vue'
import { ReadToken, ValidateToken } from '../../Authentication_Authorization/auth.js'
const role = ref('')
onMounted(async () => {
  let accesstoken = Cookies.get('accessToken')
  let refreshtoken = Cookies.get('refreshToken')

  const validatetoken = await ValidateToken(accesstoken, refreshtoken)
  if (validatetoken) {
    accesstoken = Cookies.get('accessToken')
    const readtoken = ReadToken(accesstoken)
    if (readtoken) {
      role.value = readtoken.Role
    }
  }
})
</script>
<template>
  <div class="xp-vertical">
    <!-- Start XP Container -->
    <div id="xp-container">
      <!-- Start XP Leftbar -->
      <div class="xp-leftbar">
        <!-- Start XP Sidebar -->
        <div class="xp-sidebar">
          <!-- Start XP Logobar -->
          <div class="xp-logobar text-center">
            <a href="index.html" class="xp-logo"
              ><img
                style="height: 100px"
                src="../../assets/admin/images/Red and Yellow Illustrative Fried Chicken Logo.png"
                class="img-fluid"
                alt="logo"
            /></a>
          </div>
          <!-- End XP Logobar -->

          <!-- Start XP Navigationbar -->
          <div class="xp-navigationbar">
            <ul class="xp-vertical-menu">
              <li v-if="role.toLowerCase() == 'admin'">
                <RouterLink to="/Admin">
                  <i class="icon-speedometer"></i><span>THỐNG KÊ</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Product">
                  <i class="icon-tag"></i><span>SẢN PHẨM</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Combo">
                  <i class="icon-trophy"></i><span>COMBO</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Bill">
                  <i class="icon-paypal"></i><span>ĐƠN HÀNG</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Coupon">
                  <i class="icon-present"></i><span>COUPON</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/admin/customer">
                  <i class="icon-user"></i><span>KHÁCH HÀNG</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Table">
                  <i class="icon-note"></i><span>BÀN</span>
                </RouterLink>
              </li>
              <li v-if="role.toLowerCase() == 'admin'">
                <router-link to="/admin/staff">
                  <i class="icon-people"></i><span>NHÂN VIÊN</span>
                </router-link>
              </li>          
            </ul>
          </div>
          <!-- End XP Navigationbar -->
        </div>
        <!-- End XP Sidebar -->
      </div>
      <!-- End XP Leftbar -->

      <!-- Start XP Rightbar -->
      <div class="xp-rightbar">
        <!-- Start XP Topbar -->
        <div class="xp-topbar">
          <!-- Start XP Row -->
          <div class="row">
            <!-- Start XP Col -->
            <div class="col-2 col-md-1 col-lg-1 order-2 order-md-1 align-self-center">
              <div class="xp-menubar">
                <a class="xp-menu-hamburger" href="javascript:void();">
                  <i class="icon-menu font-20 text-white"></i>
                </a>
              </div>
            </div>
            <!-- End XP Col -->

            <!-- Start XP Col -->
            <div class="col-10 col-md-11 col-lg-11 order-1 order-md-2">
              <div class="xp-profilebar text-right">
                <ul class="list-inline mb-0">
                  <li class="list-inline-item mr-0">
                    <div class="dropdown xp-userprofile">
                      <a
                        class="dropdown-toggle"
                        href="#"
                        role="button"
                        id="xp-userprofile"
                        data-toggle="dropdown"
                        aria-haspopup="true"
                        aria-expanded="false"
                        ><img
                          src="../../assets/admin/images/360_F_229758328_7x8jwCwjtBMmC6rgFzLFhZoEpLobB6L8.jpg"
                          alt="user-profile"
                          class="rounded-circle img-fluid" /><span class="xp-user-live"></span
                      ></a>

                      <div
                        class="dropdown-menu dropdown-menu-right"
                        aria-labelledby="xp-userprofile"
                      >
                        <!-- <a class="dropdown-item py-3 text-white text-center font-16" href="#"
                          >Chào mừng, John Doe</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-user text-primary mr-2"></i> Hồ sơ</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-wallet text-success mr-2"></i> Thanh toán</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-settings text-warning mr-2"></i> Cài đặt</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-lock text-info mr-2"></i> Khóa màn hình</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-power text-danger mr-2"></i> Đăng xuất</a
                        > -->
                        <!-- Đăng xuất -->
                        <LogoutAdmin />
                      </div>
                    </div>
                  </li>
                </ul>
              </div>
            </div>
            <!-- End XP Col -->
          </div>
          <!-- End XP Row -->
        </div>
        <!-- End XP Topbar -->

        <RouterView />
      </div>
      <!-- End XP Rightbar -->
    </div>
    <!-- End XP Container -->
  </div>
</template>

<style></style>
