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
                  <h3 class="text-center mt-0 m-b-15">
                    <a href="index.html" class="xp-web-logo"
                      ><img src="../../assets/admin/images/logo.svg" height="40" alt="logo"
                    /></a>
                  </h3>
                  <div class="p-3">
                    <form action="#">
                      <div class="text-center mb-3">
                        <h4 class="text-black">Quên mật khẩu</h4>
                        <p class="text-muted">Nhớ mật khẩu <a href="page-login.html">Đăng nhập</a> Tại đây</p>
                      </div>
                      <p class="text-muted text-center m-b-30">
                        Chúng tôi sẽ gửi cho bạn hướng dẫn qua email để giúp bạn đặt lại mật khẩu.
                      </p>
                      <div class="form-group">
                        <input type="email" class="form-control" id="email" placeholder="Email" required />
                      </div>
                      <button type="submit" class="btn btn-primary btn-rounded btn-lg btn-block">Gửi Email</button>
                    </form>
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
export default {

}
</script>

<style>

</style>