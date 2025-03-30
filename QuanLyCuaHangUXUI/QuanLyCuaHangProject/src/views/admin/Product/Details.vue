<script setup>
import { ref, computed } from 'vue'
import { Swiper, SwiperSlide } from 'swiper/vue'
import { Navigation, Pagination } from 'swiper/modules'
const modules = [Navigation, Pagination]
import 'swiper/css'
import 'swiper/css/navigation'
import 'swiper/css/pagination'
const ListProduct = ref([])
const Props = defineProps({
  Product: Object,
})
ListProduct.value = Props.Product?.chitietsanphams
</script>
<template>
  <div
    class="modal fade"
    :id="`productDetailModal_${Props.Product?.maSp || 'default'}`"
    tabindex="-1"
    :aria-labelledby="`productDetailModalLabel_${Props.Product?.maSp || 'default'}`"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-xl text-start">
      <div class="modal-content">
        <div class="modal-header bg-primary text-white">
          <h5
            :id="`productDetailModalLabel_${Props.Product?.maSp || 'default'}`"
            class="modal-title"
          >
            Chi tiết sản phẩm
          </h5>
          <button
            type="button"
            class="btn-close btn-close-white"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body p-4">
          <form @submit.prevent>
            <!-- Tên sản phẩm -->
            <div class="mb-3">
              <label for="tenSanPham" class="form-label">Tên sản phẩm</label>
              <input
                type="text"
                class="form-control"
                id="tenSanPham"
                placeholder="Nhập tên sản phẩm"
                :value="Props.Product.tenSanPham"
                readonly
              />
            </div>

            <!-- Danh mục -->
            <div class="mb-3">
              <label for="maDanhMuc" class="form-label">Danh mục</label>
              <input
                type="text"
                class="form-control"
                id="tenSanPham"
                placeholder="Nhập tên sản phẩm"
                :value="Props.Product.tenDanhMuc"
                readonly
              />
            </div>
            <!-- Tổng số lượng -->
            <div class="mb-3">
              <label for="maDanhMuc" class="form-label">Số lượng</label>
              <input
                type="text"
                class="form-control"
                id="tongSoLuong"
                :value="Props.Product.tongSoLuong"
                readonly
              />
            </div>

            <!-- Mô tả -->
            <div class="mb-3">
              <label for="moTa" class="form-label">Mô tả</label>
              <textarea
                readonly
                class="form-control"
                id="moTa"
                rows="3"
                placeholder="Nhập mô tả sản phẩm"
                :value="Props.Product.moTa"
              ></textarea>
            </div>

            <!-- Biến thể sản phẩm -->
            <div class="mb-3">
              <label class="form-label">Biến thể sản phẩm</label>
              <div v-for="(variant, index) in ListProduct" :key="index" class="card mb-3">
                <div class="card-body">
                  <div class="row">
                    <!-- Slider hiển thị ảnh biến thể -->
                    <div class="col-md-3">
                      <label class="form-label">Ảnh</label>
                      <swiper
                        :slides-per-view="1"
                        :space-between="10"
                        :modules="modules"
                        navigation
                        pagination
                        class="variant-slider"
                      >
                        <swiper-slide v-for="(image, index) in variant.hinhanhs" :key="index">
                          <img
                            :src="`https://localhost:7139/HinhAnh/Food_Drink/${
                              image.tenHinhAnh
                            }`"
                            alt="Ảnh biến thể"
                            class="img-fluid rounded"
                            style="max-width: 100px; height: auto"
                          />
                          
                        </swiper-slide>
                      </swiper>
                    </div>
                    
                    <div class="col-md-2">
                      <label class="form-label">Kích thước</label>
                      <input type="text" class="form-control" :value="variant.kichThuoc" readonly />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Hương vị</label>
                      <input type="text" class="form-control" :value="variant.huongVi" readonly />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Số lượng tồn</label>
                      <input
                        type="number"
                        class="form-control"
                        :value="variant.soLuongTon"
                        readonly
                      />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Đơn giá</label>
                      <input type="number" class="form-control" :value="variant.donGia" readonly />
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </form>
        </div>
        <div class="modal-footer bg-light">
          <button type="button" class="btn btn-outline-primary" data-bs-dismiss="modal">
            Đóng
          </button>
        </div>
      </div>
    </div>
  </div>
</template>
  

  
<style scoped>
.form-label {
  vertical-align: left;
}
.card {
  border: 1px solid #ddd;
}
.modal-xl {
  max-width: 90%;
}
.variant-slider .swiper-slide {
  display: flex;
  justify-content: center; /* Căn giữa ngang */
  align-items: center; /* Căn giữa dọc */
  height: 100%; /* Đảm bảo Swiper giữ đúng chiều cao */
}
.variant-slider img {
  max-width: 100px;
  height: auto;
  object-fit: contain; /* Giữ tỉ lệ ảnh */
}

</style>
