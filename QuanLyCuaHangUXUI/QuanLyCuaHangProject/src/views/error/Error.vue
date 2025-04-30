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
                      <span class="text-black">{{ statusMessage }}</span>
                    </h1>
                    <h4 class="xp-error-subtitle text-black m-b-30">
                      <i class="mdi mdi-emoticon-sad text-danger font-32"></i>{{ errorSubtitle }}
                    </h4>
                    <p class="text-muted m-b-30">{{ errorMessage }}</p>
                    <RouterLink :to="redirectLink" class="btn btn-primary"
                      >Quay lại trang</RouterLink
                    >
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
import { useRoute } from 'vue-router'

export default {
  name: 'ErrorComponent',
  components: { RouterLink },
  data() {
    return {
      status: '',
      statusMessage: '',
      errorSubtitle: '',
      errorMessage: '',
      redirectLink: '/',
    }
  },
  mounted() {
    const route = useRoute()
    this.status = route.params.status

    this.setErrorDetails()
  },
  methods: {
    setErrorDetails() {
      switch (this.status) {
        case '401':
          this.statusMessage = this.status
          this.errorSubtitle = 'Trang không tồn tại hoặc phiên đăng nhập của bạn đã hết!'
          this.errorMessage = 'Vui lòng quay lại trang bạn có thể truy cập.'
          this.redirectLink = '/'
          break
        case '404':
          this.statusMessage = this.status
          this.errorSubtitle = 'Đã xảy ra lỗi!'
          this.errorMessage = 'Trang bạn đang tìm kiếm không tồn tại.'
          this.redirectLink = '/'
          break
        case '500':
          this.statusMessage = this.status
          this.errorSubtitle = 'Oops! Đã có sự cố.'
          this.errorMessage = 'Đã xảy ra lỗi trên máy chủ.'
          this.redirectLink = '/login'
          break
        default:
          this.statusMessage = this.status
          this.errorSubtitle = 'Vui lòng thử lại sau.'
          this.errorMessage = 'Chúng tôi không thể xử lý yêu cầu của bạn.'
          this.redirectLink = '/'
          break
      }
    },
  },
}
</script>

<style scoped>
/* Thêm các kiểu CSS nếu cần */
</style>
