<script setup>
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
import QrScanAndShiftManagerModal from '@/components/caKip/QrScanAndShiftManagerModal.vue'

components: {
  QrScanAndShiftManagerModal
}
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
              ><img src="../../assets/admin/images/logo.svg" class="img-fluid" alt="logo"
            /></a>
          </div>
          <!-- End XP Logobar -->

          <!-- Start XP Navigationbar -->
          <div class="xp-navigationbar">
            <ul class="xp-vertical-menu">
              <li class="xp-vertical-header">Chức năng quản lý</li>
              <li>
                <RouterLink to="/Admin"> <i class="icon-speedometer"></i><span>Bảng điều khiển</span> </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Product">
                  <i class="fas fa-box"></i><span>SẢN PHẨM</span>
                </RouterLink>
              </li>
              <li>
                <RouterLink to="/Admin/Combo">
                  <i class="fas fa-boxes"></i><span>COMBO</span>
                </RouterLink>
                <RouterLink to="/Admin/Bill">
                  <i class="fas fa-box"></i><span>QUẢN LÝ ĐƠN HÀNG</span>
                </RouterLink>
              </li>
              <li>
                <a href="javaScript:void();">
                  <i class="icon-layers"></i><span>Danh sách mục</span
                  ><i class="icon-arrow-right pull-right"></i>
                </a>
                <ul class="xp-vertical-submenu">
                  <li><a href="d">Mục 1</a></li>
                  <li><a href="d">Mục 2</a></li>
                  <li><a href="d">Muc 3</a></li>
                </ul>
              </li>
              <li>
                <a href="javaScript:void();">
                  <i class="icon-social-dropbox"></i><span>Danh sách mục</span
                  ><i class="icon-arrow-right pull-right"></i>
                </a>
                <ul class="xp-vertical-submenu">
                  <li><a href="d">Mục 1</a></li>
                  <li><a href="d">Mục 2</a></li>
                  <li><a href="d">Mục 3</a></li>
                </ul>
              </li>
              <!-- <li>
                            <a href="javaScript:void();">
                                <i class="icon-drop"></i><span>Danh sách mục</span><i class="icon-arrow-right pull-right"></i>
                            </a>
                            <ul class="xp-vertical-submenu">
                                <li><a href="">Mục 1</a></li>                                
                                <li><a href="">Mục 2</a></li>
                                <li><a href="">Mục 3</a></li>
                            </ul>
                        </li> -->
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
                  <li class="list-inline-item">
                    <div class="dropdown xp-message">
                      <a
                        class="dropdown-toggle text-white"
                        href="#"
                        role="button"
                        id="xp-message"
                        data-toggle="dropdown"
                        aria-haspopup="true"
                        aria-expanded="false"
                      >
                        <i class="icon-speech font-18 v-a-m"></i>
                        <span class="badge badge-pill bg-success-gradient xp-badge-up">8</span>
                      </a>
                      <div class="dropdown-menu dropdown-menu-right" aria-labelledby="xp-message">
                        <ul class="list-unstyled">
                          <li class="media">
                            <div class="media-body">
                              <h5 class="mt-0 mb-0 py-3 text-white text-center font-16">
                                8 Tin nhắn mới
                              </h5>
                            </div>
                          </li>
                          <li class="media xp-msg">
                            <img
                              class="mr-3 align-self-center rounded-circle"
                              src="../../assets/admin/images/topbar/user-message-1.jpg"
                              alt="Generic placeholder image"
                            />
                            <div class="media-body">
                              <a href="#">
                                <h5 class="mt-0 mb-1 font-14">
                                  Ariel Blue<span class="font-12 f-w-4 float-right"
                                    >3 phút trước</span
                                  >
                                </h5>
                                <p class="mb-0 font-13">
                                  Cảm ơn bạn đã tham dự...<span
                                    class="badge badge-pill badge-success float-right"
                                    >2</span
                                  >
                                </p>
                              </a>
                            </div>
                          </li>
                          <li class="media xp-msg">
                            <img
                              class="mr-3 align-self-center rounded-circle"
                              src="../../assets/admin/images/topbar/user-message-2.jpg"
                              alt="Generic placeholder image"
                            />
                            <div class="media-body">
                              <a href="#">
                                <h5 class="mt-0 mb-1 font-14">
                                  Jammy Moon<span class="font-12 f-w-4 float-right"
                                    >5 phút trước</span
                                  >
                                </h5>
                                <p class="mb-0 font-13">
                                  Này không phải lo lắng!Tin tôi đi...<span
                                    class="badge badge-pill badge-success float-right"
                                    >3</span
                                  >
                                </p>
                              </a>
                            </div>
                          </li>
                          <li class="media xp-msg">
                            <img
                              class="mr-3 align-self-center rounded-circle"
                              src="../../assets/admin/images/topbar/user-message-3.jpg"
                              alt="Generic placeholder image"
                            />
                            <div class="media-body">
                              <a href="#">
                                <h5 class="mt-0 mb-1 font-14">
                                  Lisa Ross<span class="font-12 f-w-4 float-right">5:25 PM</span>
                                </h5>
                                <p class="mb-0 font-13">
                                  Biện pháp khắc phục cho đau bụng?Tôi không...<span
                                    class="badge badge-pill badge-success float-right"
                                    >5</span
                                  >
                                </p>
                              </a>
                            </div>
                          </li>
                          <li class="media">
                            <div class="media-body">
                              <h5 class="mt-0 mb-0 py-3 text-black text-center font-14">
                                <a href="#" class="text-primary">Xem thêm</a>
                              </h5>
                            </div>
                          </li>
                        </ul>
                      </div>
                    </div>
                  </li>
                  <li class="list-inline-item">
                    <div class="dropdown xp-notification">
                      <a
                        class="dropdown-toggle text-white"
                        href="#"
                        role="button"
                        id="xp-notification"
                        data-toggle="dropdown"
                        aria-haspopup="true"
                        aria-expanded="false"
                      >
                        <i class="icon-bell font-18 v-a-m"></i>
                        <span class="badge badge-pill bg-danger-gradient xp-badge-up">3</span>
                      </a>

                      <div
                        class="dropdown-menu dropdown-menu-right"
                        aria-labelledby="xp-notification"
                      >
                        <ul class="list-unstyled">
                          <li class="media">
                            <div class="media-body">
                              <h5 class="mt-0 mb-0 py-3 text-white text-center font-16">
                                3 Thông báo mới
                              </h5>
                            </div>
                          </li>
                          <li class="media xp-noti">
                            <div class="mr-3 xp-noti-icon primary-rgba">
                              <i class="icon-user-follow text-primary"></i>
                            </div>
                            <div class="media-body">
                              <a href="#">
                                <h5 class="mt-0 mb-1 font-14">Người dùng mới đã đăng ký</h5>
                                <p class="mb-0 font-12 f-w-4">2 phút trước</p>
                              </a>
                            </div>
                          </li>
                          <li class="media xp-noti">
                            <div class="mr-3 xp-noti-icon success-rgba">
                              <i class="icon-basket-loaded text-success"></i>
                            </div>
                            <div class="media-body">
                              <a href="#">
                                <h5 class="mt-0 mb-1 font-14">New order placed</h5>
                                <p class="mb-0 font-12 f-w-4">8:45 PM</p>
                              </a>
                            </div>
                          </li>
                          <li class="media xp-noti">
                            <div class="mr-3 xp-noti-icon danger-rgba">
                              <i class="icon-like text-danger"></i>
                            </div>
                            <div class="media-body">
                              <a href="#">
                                <h5 class="mt-0 mb-1 font-14">John thích ảnh của bạn.</h5>
                                <p class="mb-0 font-12 f-w-4">Hôm qua</p>
                              </a>
                            </div>
                          </li>
                          <li class="media">
                            <div class="media-body">
                              <h5 class="mt-0 mb-0 py-3 text-black text-center font-14">
                                <a href="#" class="text-primary">Xem thêm</a>
                              </h5>
                            </div>
                          </li>
                        </ul>
                      </div>
                    </div>
                  </li>
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
                          src="../../assets/admin/images/topbar/user.jpg"
                          alt="user-profile"
                          class="rounded-circle img-fluid" /><span class="xp-user-live"></span
                      ></a>

                      <div
                        class="dropdown-menu dropdown-menu-right"
                        aria-labelledby="xp-userprofile"
                      >
                        <a class="dropdown-item py-3 text-white text-center font-16" href="#"
                          >Chào mừng, John Doe</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-user text-primary mr-2"></i> Hồ sơ</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-wallet text-success mr-2"></i> Thanh toán</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-user text-primary mr-2"></i>
                          <QrScanAndShiftManagerModal />
                        </a>
                        <a class="dropdown-item" href="#"
                          ><i class="icon-settings text-warning mr-2"></i> Cài đặt</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-lock text-info mr-2"></i> Khóa màn hình</a
                        >
                        <a class="dropdown-item" href="#"
                          ><i class="icon-power text-danger mr-2"></i> Đăng xuất</a
                        >
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