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
                  <div class="xp-error-box text-center">
                    <h1 class="xp-error-title mb-3">
                      <span class="text-black">4</span><span class="text-black">0</span
                      ><span class="text-black">4</span>
                    </h1>
                    <h4 class="xp-error-subtitle text-black m-b-30">
                      <i class="mdi mdi-emoticon-sad text-danger font-32"></i>pps! Không tìm thấy
                      trang
                    </h4>
                    <p class="text-muted m-b-30">Trang bạn đang tìm kiếm không tồn tại.</p>
                    <RouterLink class="btn btn-primary btn-rounded mb-3" to="/">
                      <i class="icon-home"></i> Đưa tôi trở lại
                    </RouterLink>
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

<script>
import { RouterLink } from 'vue-router'
</script>

<style></style>
