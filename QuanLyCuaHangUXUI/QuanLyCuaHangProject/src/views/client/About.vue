<script setup>
import OurExperienceChefs from '../../components/OurExperienceChefs.vue'
</script>
<template>
  <div>
    <!-- about part start-->
    <section class="about_part about_bg">
      <!-- breadcrumb start-->
      <section style="margin-top: 60px; width: 100%" class="breadcrumb breadcrumb_bg">
        <div class="container">
          <div class="row">
            <div class="col-lg-12">
              <div class="breadcrumb_iner text-center">
                <div class="breadcrumb_iner_item">
                  <h2>Về chúng tôi</h2>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
      <!-- breadcrumb start-->

      <div class="container-fluid">
        <div class="row align-items-center">
          <div class="col-sm-4 col-lg-5 offset-lg-1">
            <div class="about_img">
              <img src="../../assets/client/img/about.png" alt="" />
            </div>
          </div>
          <div class="col-sm-8 col-lg-4">
            <div class="about_text">
              <h5>Tiểu sử của chúng tôi</h5>
              <h2>Nơi thức ăn ngon như Root Beer.</h2>
              <h4>Làm hài lòng mọi người khao khát những thú vui đơn giản</h4>
              <p>
                Có thể qua là.Là dấu hiệu hai.Tinh thần.Đã mang lại cho biết sự khô khan của riêng
                mình ít tốt nhất thứ sáu, anh ta, tập hợp bạn may mắn mang anh ta vị trí tốt nhất
                của chúng ta trong hố tháng sâu
              </p>
              <a href="#" class="btn_3"
                >Xem thêm<img src="../assets/client/img/icon/left_2.svg" alt=""
              /></a>
            </div>
          </div>
        </div>
      </div>

      <!--::chefs_part start::-->
      <OurExperienceChefs />
      <!--::chefs_part end::-->
    </section>
    <!-- about part end-->
  </div>
</template>

<style></style>
