<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
// import 'animate.css'
// import 'bootstrap-icons/font/bootstrap-icons.css'

const route = useRoute()
const combo = ref(null)
const quantity = ref(1)
const selectedVariants = ref({}) // Lưu biến thể được chọn cho mỗi sản phẩm

// Thêm hàm scrollToTop
const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: 'smooth',
  })
}

// Lấy chi tiết combo
async function fetchComboDetail() {
  try {
    const response = await fetch(`https://localhost:7139/api/Home/Combos/${route.params.id}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('Error' + response.status)
    }
    const data = await response.json()
    combo.value = data
    // Khởi tạo biến thể mặc định cho mỗi sản phẩm
    if (data.chitietcombos) {
      data.chitietcombos.forEach((item) => {
        if (item.chitietsanphams && item.chitietsanphams.length > 0) {
          selectedVariants.value[item.maSp] = item.chitietsanphams[0]
        }
      })
    }
  } catch (error) {
    console.error('Error fetching combo detail:', error)
  }
}

// Xử lý tăng/giảm số lượng
const increaseQuantity = () => {
  // Tăng số lượng không giới hạn
  quantity.value++
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

// Xử lý chọn biến thể
const selectVariant = (maSp, variant) => {
  selectedVariants.value[maSp] = variant
  // Kiểm tra xem đã chọn đủ biến thể cho tất cả sản phẩm chưa
  const allSelected = selectedVariants.value[maSp]
  console.log('All variants selected:', allSelected)
}

// Xử lý thêm vào giỏ hàng
const addToCart = () => {
  if (!combo.value) return

  // TODO: Thêm logic xử lý giỏ hàng ở đây
  console.log('Add to cart:', null)
}

onMounted(() => {
  scrollToTop()
  fetchComboDetail()
})
</script>

<template>
  <div class="combo-detail">
    <div class="container">
      <div v-if="combo" class="row">
        <!-- Hình ảnh combo -->
        <div class="col-md-6 mb-4 animate__animated animate__fadeInLeft">
          <div class="combo-image">
            <img
              :src="
                combo.hinh
                  ? `https://localhost:7139/HinhAnh/Food_Drink/${combo.hinh}`
                  : '../assets/client/img/food_menu/combo_default.png'
              "
              :alt="combo.tenCombo"
              class="img-fluid"
            />
          </div>
        </div>

        <!-- Thông tin combo -->
        <div class="col-md-6 animate__animated animate__fadeInRight">
          <div class="combo-info">
            <h1 class="combo-title">{{ combo.tenCombo }}</h1>
            <p class="combo-description">{{ combo.moTa }}</p>

            <!-- Danh sách sản phẩm trong combo -->
            <div class="combo-products">
              <h3>Sản phẩm trong combo:</h3>
              <div v-for="item in combo.chitietcombos" :key="item.maSp" class="combo-product-item">
                <div class="product-header">
                  <i class="bi bi-check-circle-fill text-success me-2"></i>
                  <span>{{ item.tenSp }} (x{{ item.soLuongSp }})</span>
                </div>

                <!-- Chọn biến thể -->
                <div
                  v-if="item.chitietsanphams && item.chitietsanphams.length > 0"
                  class="variant-selector"
                >
                  <div class="variant-options">
                    <div
                      v-for="variant in item.chitietsanphams"
                      :key="variant.maCtsp"
                      class="variant-option"
                    >
                      <input
                        type="radio"
                        :id="'variant-' + variant.maCtsp"
                        :name="'variant-' + item.maSp"
                        :value="variant"
                        @change="selectVariant(item.maSp, variant)"
                        :checked="variant === item.chitietsanphams[0]"
                      />
                      <label :for="'variant-' + variant.maCtsp">
                        <div class="variant-info">
                          <div class="variant-name">
                            {{ variant.tenSanPham }}
                            <span v-if="variant.kichThuoc" class="variant-size"
                              >({{ variant.kichThuoc }})</span
                            >
                            <span v-if="variant.huongVi" class="variant-flavor"
                              >({{ variant.huongVi }})</span
                            >
                          </div>
                          <div class="variant-details">
                            <span class="variant-stock">Còn: {{ variant.soLuongTon }}</span>
                            <span class="variant-price"
                              >{{ variant.donGia.toLocaleString('vi-VN') }}đ</span
                            >
                          </div>
                        </div>
                        <div class="variant-image">
                          <img
                            :src="
                              variant.anhDaiDien
                                ? `https://localhost:7139/HinhAnh/Food_Drink/${variant.anhDaiDien}`
                                : '../assets/client/img/food_menu/chicken_default.png'
                            "
                            :alt="variant.tenSanPham"
                            class="img-fluid"
                          />
                        </div>
                      </label>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            <!-- Giá và giảm giá -->
            <div class="combo-pricing">
              <div v-if="combo.phanTramGiam" class="discount-badge">
                Giảm {{ combo.phanTramGiam }}%
              </div>
              <div v-if="combo.soTienGiam" class="discount-badge">
                Giảm {{ combo.soTienGiam.toLocaleString('vi-VN') }}đ
              </div>
            </div>

            <!-- Chọn số lượng -->
            <div class="quantity-selector">
              <h4>Số lượng:</h4>
              <div class="quantity-controls">
                <button
                  class="btn btn-outline-primary"
                  @click="decreaseQuantity"
                  :disabled="quantity <= 1"
                >
                  <i class="bi bi-dash"></i>
                </button>
                <span class="quantity-display">{{ quantity }}</span>
                <button class="btn btn-outline-primary" @click="increaseQuantity">
                  <i class="bi bi-plus"></i>
                </button>
              </div>
            </div>

            <!-- Nút thêm vào giỏ hàng -->
            <button class="btn btn-primary btn-lg w-100 mt-4" @click="addToCart">
              <i class="bi bi-cart-plus me-2"></i>Thêm vào giỏ hàng
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.combo-detail {
  padding: 200px 0 40px 0;
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  min-height: 100vh;
}

.combo-image {
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.combo-image img {
  width: 100%;
  height: auto;
  object-fit: cover;
}

.combo-info {
  background: white;
  padding: 30px;
  border-radius: 15px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.combo-title {
  color: #333;
  font-size: 2rem;
  margin-bottom: 20px;
  font-weight: bold;
}

.combo-description {
  color: #666;
  font-size: 1.1rem;
  margin-bottom: 30px;
  line-height: 1.6;
}

.combo-products {
  margin: 30px 0;
}

.combo-product-item {
  margin: 20px 0;
  padding: 15px;
  border: 1px solid #eee;
  border-radius: 10px;
}

.product-header {
  font-size: 1.1rem;
  color: #444;
  margin-bottom: 10px;
}

.variant-selector {
  margin-top: 10px;
}

.variant-options {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.variant-option {
  display: flex;
  align-items: center;
  padding: 15px;
  border: 1px solid #ddd;
  border-radius: 10px;
  cursor: pointer;
  transition: all 0.3s ease;
  margin-bottom: 10px;
}

.variant-option:hover {
  background: #f8f9fa;
  border-color: #ff4757;
}

.variant-option input[type='radio'] {
  margin-right: 15px;
}

.variant-option label {
  display: flex;
  justify-content: space-between;
  align-items: center;
  width: 100%;
  cursor: pointer;
}

.variant-info {
  flex: 1;
}

.variant-name {
  font-weight: 500;
  margin-bottom: 5px;
}

.variant-size,
.variant-flavor {
  color: #666;
  font-size: 0.9em;
  margin-left: 5px;
}

.variant-details {
  display: flex;
  gap: 15px;
  font-size: 0.9em;
}

.variant-stock {
  color: #28a745;
}

.variant-price {
  color: #ff4757;
  font-weight: bold;
}

.variant-image {
  width: 80px;
  height: 80px;
  margin-left: 15px;
  border-radius: 8px;
  overflow: hidden;
}

.variant-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.combo-pricing {
  margin: 20px 0;
}

.discount-badge {
  display: inline-block;
  background: #ff4757;
  color: white;
  padding: 5px 15px;
  border-radius: 20px;
  font-weight: bold;
  margin-right: 10px;
}

.quantity-selector {
  margin: 30px 0;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 15px;
  margin-top: 10px;
}

.quantity-display {
  font-size: 1.2rem;
  font-weight: bold;
  min-width: 40px;
  text-align: center;
}

.btn-primary {
  background: #ff4757;
  border: none;
  padding: 12px 30px;
  font-size: 1.1rem;
  transition: all 0.3s ease;
}

.btn-primary:hover {
  background: #ff6b81;
  transform: translateY(-2px);
}

.btn-outline-primary {
  color: #ff4757;
  border-color: #ff4757;
}

.btn-outline-primary:hover {
  background: #ff4757;
  color: white;
}

.btn-outline-primary:disabled {
  color: #ccc;
  border-color: #ccc;
  cursor: not-allowed;
}
</style>
