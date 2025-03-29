<script setup>
import DeliciousFoodMenu from '../../components/DeliciousFoodMenu.vue'
import OurExperienceChefs from '../../components/OurExperienceChefs.vue'
import OrderFood from '../../components/OrderFood.vue'
import LatestFromBlog from '../../components/LatestFromBlog.vue'
</script>
<template>
  <div>
    <!-- banner part start-->
    <section class="banner_part">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-lg-6">
            <div class="banner_text">
              <div class="banner_text_iner">
                <h1>Ngon đậm đà, cắn một miếng là mê ngay</h1>
              
                <p>
                  Cả nhà quây quần, cảm giác như lên thiên đường với từng miếng gà giòn rụm, nhìn thôi đã thấy ngon, nhất là khi chấm sốt. Đĩa gà trên bàn làm mọi người xuýt xoa. Sáng sớm nghe mùi gà rán là chỉ muốn ăn luôn.
                </p>
                <div class="banner_btn">
                  <div class="banner_btn_iner " style="margin-right: 20px;">
                    <a href="/menu" class="btn_2"
                      >Thực đơn <img src="../src/assets/client/img/icon/left_1.svg" alt=""
                    /></a>
                  </div>
                
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!-- banner part start-->

    <!--::exclusive_item_part start::-->
    <section class="exclusive_item_part blog_item_section">
      <div class="container">
        <div class="row">
          <div class="col-xl-5">
            <div class="section_tittle">
              <p>Dark Dark</p>
              <h2>Ăn Gì Hôm Nay</h2>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/image1.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>GÀ GIÒN VUI VẺ</h3>
                <p>Trải nghiệm hượng vị gà thơm ngon , giòn rụm</p>
                <a href="#" class="btn_3">Đặt ngay <img src="../assets/client/img/icon/image.png" alt="" /></a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/image4.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>  GÀ GIÒN HƯƠNG VỊ CAY</h3>
                <p>2 Gà Sốt Cay + 1 Khoai tây chiên vừa + 1 Nước ngọt</p>
                <a href="#" class="btn_3">Đặt ngay <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/image3.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>COMBO GÀ GIÒN</h3>
                <p>Sự kết hợp giữa mì và gà giòn </p>
                <a href="http://localhost:5173/detail/combo/1" class="btn_3">Đặt ngay <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4 d-none d-sm-block d-lg-none">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/food_item_1.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3></h3>
                <p>Was brean shed moveth day yielding tree yielding day were female and</p>
                <a href="#" class="btn_3">Đặt ngay <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!--::exclusive_item_part end::-->

    <!-- about part start-->
    <section class="about_part">
      <div class="container-fluid">
        <div class="row align-items-center">
          <div class="col-sm-4 col-lg-5 offset-lg-1">
            <div class="about_img">
              <img src="../../assets/client/img/food_item/image5.png" alt="" />
            </div>
          </div>
          <div class="col-sm-8 col-lg-4">
            <div class="about_text">
              <h5></h5>
              <h2>GÀ GIÒN RỘP RỘP</h2>
              <h4>Đáp ứng cơn thèm những niềm vui đơn giản</h4>
<p> Miếng gà giòn tan, thơm lừng làm ai cũng mê. Mang đến cảm giác vui vẻ, cả nhà cùng thưởng thức, chẳng gì sánh bằng khi cắn vào lớp vỏ ngon tuyệt, chấm thêm sốt nữa thì đúng là đỉnh cao mỗi bữa ăn.
</p>
              <a href="http://localhost:5173/detail/product/1001" class="btn_3">Đặt ngay <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!-- about part end-->

    <!-- food_menu start-->
    <DeliciousFoodMenu/>
    <!-- food_menu part end-->

    <!--::chefs_part start::-->
    <!-- <OurExperienceChefs/> -->
    <!--::chefs_part end::-->

    <!--::regervation_part start::-->
    <!-- <OrderFood/> -->
    <!--::regervation_part end::-->
    
    <!--::exclusive_item_part start::-->
    <!-- <LatestFromBlog/> -->
    <!--::exclusive_item_part end::-->
  </div>
</template>



<style>

</style>