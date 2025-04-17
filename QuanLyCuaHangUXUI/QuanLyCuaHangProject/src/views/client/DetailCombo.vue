<script setup>
import { ref, onMounted, computed } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import 'animate.css'
import 'bootstrap-icons/font/bootstrap-icons.css'
import Swal from 'sweetalert2'
import Cookies from 'js-cookie'
import { ReadToken, ValidateToken } from '../../Authentication_Authorization/auth.js'
import { GetApiUrl } from '@constants/api'
const route = useRoute()
const combo = ref(null)
const selectedVariant = ref(null)
const quantity = ref(1)
const quantityError = ref('')
const selectedVariants = ref({}) // Lưu biến thể được chọn cho mỗi sản phẩm

let accesstoken = Cookies.get('accessToken')
const refreshtoken = Cookies.get('refreshToken')
const router = useRouter()
// Tính toán giá gốc của combo
const originalPrice = computed(() => {
  if (!combo.value?.chitietcombos) return 0
  return combo.value.chitietcombos.reduce((total, item) => {
    // Lấy biến thể đã chọn hoặc biến thể đầu tiên nếu chưa chọn
    const selectedVariant = selectedVariants.value[item.maSp] || item.chitietsanphams[0]
    return total + selectedVariant.donGia * item.soLuongSp
  }, 0)
})

// Tính toán giá sau khi giảm
const discountedPrice = computed(() => {
  if (!combo.value) return 0
  if (combo.value.phanTramGiam) {
    return originalPrice.value * (1 - combo.value.phanTramGiam / 100)
  }
  if (combo.value.soTienGiam) {
    return originalPrice.value - combo.value.soTienGiam
  }
  return originalPrice.value
})

// Tính toán số lượng tồn tối thiểu của combo
const maxQuantity = computed(() => {
  if (!combo.value?.chitietcombos) return 0
  return Math.min(
    ...combo.value.chitietcombos.map((item) => {
      const selectedVariant = selectedVariants.value[item.maSp] || item.chitietsanphams[0]
      return Math.floor(selectedVariant.soLuongTon / item.soLuongSp)
    })
  )
})

// Validate số lượng
const validateQuantity = (value) => {
  if (!value) {
    quantityError.value = 'Vui lòng nhập số lượng'
    return false
  }

  if (isNaN(value) || value < 1) {
    quantityError.value = 'Số lượng phải là số lớn hơn 0'
    return false
  }

  if (value > combo.value.soLuong) {
    quantityError.value = `Số lượng tối đa là ${combo.value.soLuong}`
    return false
  }

  quantityError.value = ''
  return true
}

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
    const response = await fetch(GetApiUrl()+`/api/Home/Combos/${route.params.id}`, {
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
  if (quantity.value < combo.value.soLuong) {
    quantity.value++
    quantityError.value = ''
  }
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
    quantityError.value = ''
  }
}

const handleQuantityChange = (event) => {
  // Chỉ cho phép nhập số
  const value = event.target.value.replace(/[^0-9]/g, '')
  if (!value) {
    quantity.value = 1
    return
  }

  const numValue = parseInt(value)
  if (validateQuantity(numValue)) {
    // Nếu giá trị hợp lệ, cập nhật số lượng
    quantity.value = numValue
  } else {
    // Nếu giá trị không hợp lệ, reset về giá trị hợp lệ gần nhất
    if (numValue > combo.value.soLuong) {
      quantity.value = combo.value.soLuong
    } else if (numValue < 1) {
      quantity.value = 1
    }
  }
}

// Xử lý chọn biến thể
const selectVariant = (maSp, variant) => {
  selectedVariants.value[maSp] = variant
  // Kiểm tra xem đã chọn đủ biến thể cho tất cả sản phẩm chưa
  const allSelected =
    Object.keys(selectedVariants.value).length === combo.value.chitietcombos.length
  console.log('All variants selected:', allSelected)
}

// Xử lý thêm vào giỏ hàng
const addToCart = async () => {
  if (!validateQuantity(quantity.value)) {
    return
  }

  // Kiểm tra xem đã chọn đủ biến thể cho tất cả sản phẩm chưa
  const allSelected =
    Object.keys(selectedVariants.value).length === combo.value.chitietcombos.length
  if (!allSelected) {
    Swal.fire({
      icon: 'warning',
      title: 'Thông báo',
      text: 'Vui lòng chọn đầy đủ biến thể cho tất cả sản phẩm trong combo',
      confirmButtonColor: '#ff8c00',
    })
    return
  }

  try {
    let IdUser = ''
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if(validateToken == true){
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if(readtoken){
        IdUser = readtoken.IdUser
      }
    }else{
      router.push('/Login')
      return;
    }

    // Tạo danh sách chi tiết combo với biến thể đã chọn
    const chiTietCombo = combo.value.chitietcombos.map((item) => ({
      maSp: item.maSp,
      maCtsp: selectedVariants.value[item.maSp].maCtsp,
      soLuong: item.soLuongSp * quantity.value,
      donGia: selectedVariants.value[item.maSp].donGia,
    }))

    const cartItem = {
      maKh: IdUser, 
      maCombo: combo.value.maCombo,
      soLuong: quantity.value,
      donGia: originalPrice.value,
      cartDetailRequestCombos: chiTietCombo,
    }

    const response = await fetch(GetApiUrl()+'/api/Cart', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accesstoken}`,
      },
      body: JSON.stringify(cartItem),
    })
    if(response.status === 401){
      Swal.fire({
        icon: 'error',
        title: 'Phiên của bạn đã hết hoặc bạn chưa đăng nhập, vui lòng đăng nhập lại!',
        timer: 2000,
        showConfirmButton: false,
      })
      router.push('/Login')
      return;
    }
    var result = await response.json()
    if (result.success) {
      Swal.fire('Đã thêm combo vào giỏ hàng', '', 'success')
      // Emit event để cập nhật giỏ hàng
      window.dispatchEvent(new CustomEvent('updateCart'))
    } else {
      Swal.fire(`${result.message}`, '', 'error')
    }
  } catch (error) {
    console.error('Error adding to cart:', error)
    Swal.fire({
      icon: 'error',
      title: 'Lỗi',
      text: error.message || 'Có lỗi xảy ra khi thêm vào giỏ hàng',
      confirmButtonColor: '#ff8c00',
    })
  }
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
                  ? GetApiUrl()+`/HinhAnh/Food_Drink/${combo.hinh}`
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
                                ? GetApiUrl()+`/HinhAnh/Food_Drink/${variant.anhDaiDien}`
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
            <div class="price-quantity">
              <div class="price">
                <div class="original-price" v-if="combo?.phanTramGiam || combo?.soTienGiam">
                  {{ originalPrice.toLocaleString('vi-VN') }} đ
                </div>
                <span class="current-price">{{ discountedPrice.toLocaleString('vi-VN') }} đ</span>
                <div class="discount-badge" v-if="combo?.phanTramGiam">
                  -{{ combo.phanTramGiam }}%
                </div>
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
                <input
                  type="number"
                  v-model="quantity"
                  @input="handleQuantityChange"
                  :min="1"
                  :max="combo?.soLuong"
                  class="quantity-input"
                  :class="{ error: quantityError }"
                />
                <button
                  class="btn btn-outline-primary"
                  @click="increaseQuantity"
                  :disabled="quantity >= combo?.soLuong"
                >
                  <i class="bi bi-plus"></i>
                </button>
              </div>
              <div v-if="quantityError" class="error-message">{{ quantityError }}</div>
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

.price-quantity {
  margin: 20px 0;
}

.price {
  display: flex;
  align-items: center;
  gap: 10px;
}

.original-price {
  font-size: 18px;
  color: #999;
  text-decoration: line-through;
}

.current-price {
  font-size: 24px;
  font-weight: 700;
  color: #ff8c00;
}

.discount-badge {
  background: #ff4444;
  color: white;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 14px;
  font-weight: 600;
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

.quantity-input {
  width: 60px;
  text-align: center;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 4px;
  font-size: 1rem;
}

.quantity-input.error {
  border-color: #dc3545;
}

.error-message {
  color: #dc3545;
  font-size: 0.875rem;
  margin-top: 4px;
}

.quantity-input::-webkit-inner-spin-button,
.quantity-input::-webkit-outer-spin-button {
  -webkit-appearance: none;
  margin: 0;
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
