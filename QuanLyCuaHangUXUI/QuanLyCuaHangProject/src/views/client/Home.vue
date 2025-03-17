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
                <h5>Expensive but the best</h5>
                <h1>Deliciousness jumping into the mouth</h1>
                <p>
                  Together creeping heaven upon third dominion be upon won't darkness rule land behold it created good
                  saw after she'd Our set living. Signs midst dominion creepeth morning
                </p>
                <div class="banner_btn">
                  <div class="banner_btn_iner">
                    <a href="#" class="btn_2"
                      >Reservation <img src="../src/assets/client/img/icon/left_1.svg" alt=""
                    /></a>
                  </div>
                  <a href="https://www.youtube.com/watch?v=pBFQdxA-apI" class="popup-youtube video_popup">
                    <span><img src="../src/assets/client/img/icon/play.svg" alt="" /></span> Watch our story</a
                  >
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
              <p>Popular Dishes</p>
              <h2>Our Exclusive Items</h2>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/food_item_1.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>Indian Burger</h3>
                <p>Was brean shed moveth day yielding tree yielding day were female and</p>
                <a href="#" class="btn_3">Read More <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/food_item_2.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>Cremy Noodles</h3>
                <p>Was brean shed moveth day yielding tree yielding day were female and</p>
                <a href="#" class="btn_3">Read More <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/food_item_3.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>Honey Meat</h3>
                <p>Was brean shed moveth day yielding tree yielding day were female and</p>
                <a href="#" class="btn_3">Read More <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4 d-none d-sm-block d-lg-none">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="../../assets/client/img/food_item/food_item_1.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>Cremy Noodles</h3>
                <p>Was brean shed moveth day yielding tree yielding day were female and</p>
                <a href="#" class="btn_3">Read More <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
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
              <img src="../../assets/client/img/about.png" alt="" />
            </div>
          </div>
          <div class="col-sm-8 col-lg-4">
            <div class="about_text">
              <h5>Our History</h5>
              <h2>Where The Food’s As Good As The Root Beer.</h2>
              <h4>Satisfying people hunger for simple pleasures</h4>
              <p>
                May over was. Be signs two. Spirit. Brought said dry own firmament lesser best sixth deep abundantly
                bearing, him, gathering you blessed bearing he our position best ticket in month hole deep
              </p>
              <a href="#" class="btn_3">Read More <img src="../assets/client/img/icon/left_2.svg" alt="" /></a>
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
    <OurExperienceChefs/>
    <!--::chefs_part end::-->

    <!--::regervation_part start::-->
    <OrderFood/>
    <!--::regervation_part end::-->
    
    <!--::exclusive_item_part start::-->
    <LatestFromBlog/>
    <!--::exclusive_item_part end::-->
  </div>
</template>



<style>

</style>