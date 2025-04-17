<script setup>
import { RouterView } from 'vue-router'
import '../../assets/client/css/bootstrap.min.css'
import '../../assets/client/css/animate.css'
import '../../assets/client/css/owl.carousel.min.css'
import '../../assets/client/css/themify-icons.css'
import '../../assets/client/css/flaticon.css'
import '../../assets/client/css/magnific-popup.css'
import '../../assets/client/css/slick.css'
import '../../assets/client/css/gijgo.min.css'
import '../../assets/client/css/nice-select.css'
import '../../assets/client/css/all.css'
import '../../assets/client/css/style.css'
import '../../assets/client/js/jquery-1.12.1.min.js'
import '@popperjs/core'
import '../../assets/client/js/bootstrap.min.js'
import '../../assets/client/js/jquery.magnific-popup.js'
import 'swiper/css'
import '../../assets/client/js/masonry.pkgd.js'
import '../../assets/client/js/owl.carousel.min.js'
import '../../assets/client/js/slick.min.js'
import '../../assets/client/js/gijgo.min.js'
import '../../assets/client/js/jquery.nice-select.min.js'
import '../../assets/client/js/custom.js'
import Header from '../../../src/components/Header.vue'
import Footer from '../../../src/components/Footer.vue'
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useToast } from 'vue-toastification'
import { useRouter } from 'vue-router'
import { GetApiUrl } from '@constants/api'
const router = useRouter()
const toast = useToast()
const showCartModal = ref(false)
const cartItems = ref([])
const quantityError = ref({})
const shippingFee = ref(30000)
let getApiUrl = GetApiUrl()
// Mở/đóng modal
const openCartModal = () => {
  showCartModal.value = true
  document.body.style.overflow = 'hidden'
}

const closeCartModal = () => {
  showCartModal.value = false
  document.body.style.overflow = 'auto'
}

// Lấy dữ liệu giỏ hàng
const FetchCart = async () => {
  try {
    const response = await fetch(getApiUrl+`/api/Cart/120`, {
      method: 'GET',
      Headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('ERROR', response.status)
    }
    const result = await response.json()
    cartItems.value = result
  } catch (error) {
    console.log(error)
  }
}

// Tách sản phẩm đơn lẻ và combo
const singleProducts = computed(() => {
  return cartItems.value.filter((item) => item.maCombo === null)
})

const comboProducts = computed(() => {
  return cartItems.value.filter((item) => item.maCombo !== null)
})

const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => {
    return sum + item.donGia * item.soLuong
  }, 0)
})

const total = computed(() => {
  return subtotal.value + shippingFee.value
})

// Lấy URL hình ảnh
const getImageUrl = (imageName) => {
  return `/src/assets/images/${imageName}`
}

// Validate số lượng
const validateQuantity = (maCtsp, value) => {
  if (!value) {
    quantityError.value[maCtsp] = 'Vui lòng nhập số lượng'
    return false
  }

  if (isNaN(value) || value < 1) {
    quantityError.value[maCtsp] = 'Số lượng phải là số lớn hơn 0'
    return false
  }

  const product = cartItems.value.find((item) => item.maCtsp === maCtsp)
  if (product && value > product.soLuongTon) {
    quantityError.value[maCtsp] = `Số lượng tối đa là ${product.soLuongTon}`
    return false
  }

  quantityError.value[maCtsp] = ''
  return true
}

// Xử lý tăng/giảm số lượng
const increaseQuantity = async (item) => {
  if (item.soLuong < item.soLuongTon) {
    item.soLuong++
    quantityError.value[item.maCtsp] = ''
    await updateCartItem(item)
  }
}

const decreaseQuantity = async (item) => {
  if (item.soLuong > 1) {
    item.soLuong--
    quantityError.value[item.maCtsp] = ''
    await updateCartItem(item)
  }
}

const handleQuantityChange = async (event, item) => {
  const value = parseInt(event.target.value)
  if (validateQuantity(item.maCtsp, value)) {
    item.soLuong = value
    await updateCartItem(item)
  } else {
    if (value > item.soLuongTon) {
      item.soLuong = item.soLuongTon
    } else if (value < 1) {
      item.soLuong = 1
    }
    await updateCartItem(item)
  }
}

const removeItem = async (item) => {
  try {
    const response = await fetch(getApiUrl+`/api/Cart/delete/${item.maCtsp}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) {
      throw new Error('Có lỗi xảy ra khi xóa sản phẩm')
    }

    await FetchCart()
    toast.success('Đã xóa sản phẩm khỏi giỏ hàng')
  } catch (error) {
    console.error('Lỗi khi xóa sản phẩm:', error)
    toast.error(error.message)
  }
}

const updateCartItem = async (item) => {
  try {
    const response = await fetch(getApiUrl+`/api/Cart/update/${item.maCtsp}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        maCtsp: item.maCtsp,
        soLuong: item.soLuong,
      }),
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(errorText || 'Có lỗi xảy ra khi cập nhật giỏ hàng')
    }

    toast.success('Đã cập nhật số lượng')
    await FetchCart()
  } catch (error) {
    console.error('Lỗi khi cập nhật giỏ hàng:', error)
    toast.error(error.message || 'Có lỗi xảy ra khi cập nhật giỏ hàng')
  }
}

const proceedToCheckout = () => {
  closeCartModal()
  router.push('/checkout')
}

onMounted(() => {
  FetchCart()
  window.addEventListener('openCartModal', openCartModal)
  window.addEventListener('updateCart', FetchCart)
})

onUnmounted(() => {
  window.removeEventListener('openCartModal', openCartModal)
  window.removeEventListener('updateCart', FetchCart)
})
</script>

<template>
  <div>
    <!--::header part start::-->
    <Header />
    <!-- Header part end-->

    <RouterView />
    <!-- Modal giỏ hàng -->
    <div v-if="showCartModal" class="modal-overlay" @click="closeCartModal">
      <div class="modal-content" @click.stop>
        <div class="modal-header">
          <h3>Giỏ hàng của bạn</h3>
          <button class="btn-close" @click="closeCartModal">&times;</button>
        </div>

        <div v-if="cartItems.length === 0" class="empty-cart">
          <i class="fas fa-shopping-cart"></i>
          <p>Giỏ hàng trống</p>
          <button class="btn-continue-shopping" @click="closeCartModal">Tiếp tục mua sắm</button>
        </div>

        <div v-else class="cart-items">
          <!-- Sản phẩm đơn lẻ -->
          <div v-for="item in singleProducts" :key="item.id" class="cart-item">
            <div class="item-image">
              <img :src="getImageUrl(item.hinhAnh)" :alt="item.tenSanPham" />
            </div>
            <div class="item-details">
              <h4>{{ item.tenSanPham }}</h4>
              <div class="item-variants">
                <span v-if="item.kichThuoc" class="variant">{{ item.kichThuoc }}</span>
                <span v-if="item.huongVi" class="variant">{{ item.huongVi }}</span>
              </div>
              <p class="price">{{ item.donGia.toLocaleString('vi-VN') }}đ</p>
              <div class="quantity-controls">
                <button
                  class="btn-quantity"
                  @click="decreaseQuantity(item)"
                  :disabled="item.soLuong <= 1"
                >
                  -
                </button>
                <input
                  type="number"
                  v-model="item.soLuong"
                  @input="handleQuantityChange($event, item)"
                  :min="1"
                  :max="item.soLuongTon"
                  class="quantity-input"
                  :class="{ error: quantityError[item.maCtsp] }"
                />
                <button
                  class="btn-quantity"
                  @click="increaseQuantity(item)"
                  :disabled="item.soLuong >= item.soLuongTon"
                >
                  +
                </button>
              </div>
              <div v-if="quantityError[item.maCtsp]" class="error-message">
                {{ quantityError[item.maCtsp] }}
              </div>
            </div>
            <div class="item-total">
              {{ (item.donGia * item.soLuong).toLocaleString('vi-VN') }}đ
            </div>
            <button class="btn-remove" @click="removeItem(item)">
              <i class="fas fa-trash"></i>
            </button>
          </div>

          <!-- Combo -->
          <div v-for="combo in comboProducts" :key="combo.id" class="cart-item combo-item">
            <div class="item-image">
              <img :src="getImageUrl(combo.hinhAnh)" :alt="combo.tenCombo" />
            </div>
            <div class="item-details">
              <h4>{{ combo.tenCombo }}</h4>
              <p class="price">{{ combo.donGia.toLocaleString('vi-VN') }}đ</p>
              <div class="quantity-controls">
                <button
                  class="btn-quantity"
                  @click="decreaseQuantity(combo)"
                  :disabled="combo.soLuong <= 1"
                >
                  -
                </button>
                <input
                  type="number"
                  v-model="combo.soLuong"
                  @input="handleQuantityChange($event, combo)"
                  :min="1"
                  :max="combo.soLuongTon"
                  class="quantity-input"
                  :class="{ error: quantityError[combo.maCtsp] }"
                />
                <button
                  class="btn-quantity"
                  @click="increaseQuantity(combo)"
                  :disabled="combo.soLuong >= combo.soLuongTon"
                >
                  +
                </button>
              </div>
              <div v-if="quantityError[combo.maCtsp]" class="error-message">
                {{ quantityError[combo.maCtsp] }}
              </div>
              <div class="combo-details" v-if="combo.cartDetailCombos.length > 0">
                <div class="combo-items">
                  <div
                    v-for="detail in combo.cartDetailCombos"
                    :key="detail.id"
                    class="combo-item-detail"
                  >
                    <span>{{ detail.soLuong }}x {{ detail.tenSanPham }}</span>
                    <span v-if="detail.kichThuoc">({{ detail.kichThuoc }})</span>
                    <span v-if="detail.huongVi">({{ detail.huongVi }})</span>
                  </div>
                </div>
              </div>
            </div>
            <div class="item-total">
              {{ (combo.donGia * combo.soLuong).toLocaleString('vi-VN') }}đ
            </div>
            <button class="btn-remove" @click="removeItem(combo)">
              <i class="fas fa-trash"></i>
            </button>
          </div>
        </div>

        <div v-if="cartItems.length > 0" class="cart-summary">
          <div class="summary-row">
            <span>Tạm tính:</span>
            <span>{{ subtotal.toLocaleString('vi-VN') }}đ</span>
          </div>
          <div class="summary-row">
            <span>Phí vận chuyển:</span>
            <span>{{ shippingFee.toLocaleString('vi-VN') }}đ</span>
          </div>
          <div class="summary-row total">
            <span>Tổng cộng:</span>
            <span>{{ total.toLocaleString('vi-VN') }}đ</span>
          </div>
          <button class="btn-checkout" @click="proceedToCheckout">Tiến hành thanh toán</button>
        </div>
      </div>
    </div>

    <!-- footer part start-->
    <Footer />
    <!-- footer part end-->
  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: flex-end;
  z-index: 1000;
}

.modal-content {
  background: white;
  width: 100%;
  max-width: 500px;
  height: 100vh;
  overflow-y: auto;
  padding: 24px;
  position: relative;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    transform: translateX(100%);
  }
  to {
    transform: translateX(0);
  }
}

.modal-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 2px solid #f0f0f0;
}

.modal-header h3 {
  font-size: 1.5rem;
  color: #333;
  margin: 0;
}

.btn-close {
  background: none;
  border: none;
  font-size: 24px;
  color: #666;
  cursor: pointer;
  padding: 4px;
}

.btn-close:hover {
  color: #333;
}

.empty-cart {
  text-align: center;
  padding: 40px 20px;
}

.empty-cart i {
  font-size: 48px;
  color: #ccc;
  margin-bottom: 16px;
}

.empty-cart p {
  color: #666;
  font-size: 1.1rem;
  margin-bottom: 24px;
}

.btn-continue-shopping {
  background: #4caf50;
  color: white;
  border: none;
  padding: 12px 24px;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  transition: background-color 0.2s;
}

.btn-continue-shopping:hover {
  background: #45a049;
}

.cart-items {
  margin-bottom: 24px;
}

.cart-item {
  display: flex;
  align-items: flex-start;
  padding: 16px;
  border-bottom: 1px solid #eee;
  gap: 16px;
  transition: background-color 0.2s;
  position: relative;
}

.cart-item:hover {
  background-color: #f9f9f9;
}

.item-image {
  position: relative;
  flex-shrink: 0;
}

.item-image img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.item-details {
  flex: 1;
}

.item-details h4 {
  margin: 0 0 8px 0;
  font-size: 1.1rem;
  color: #333;
}

.item-variants {
  display: flex;
  gap: 8px;
  margin-bottom: 8px;
}

.variant {
  background: #f0f0f0;
  padding: 4px 8px;
  border-radius: 4px;
  font-size: 0.9rem;
  color: #666;
}

.price {
  color: #e44d26;
  font-weight: bold;
  font-size: 1.1rem;
  margin: 8px 0;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 12px;
  margin-top: 8px;
}

.btn-quantity {
  background: #f8f9fa;
  border: 1px solid #ddd;
  border-radius: 6px;
  width: 32px;
  height: 32px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  transition: all 0.2s;
}

.btn-quantity:hover:not(:disabled) {
  background: #e9ecef;
}

.btn-quantity:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-input {
  width: 50px;
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

.combo-details {
  margin-top: 12px;
  padding-top: 12px;
  border-top: 1px dashed #ddd;
}

.combo-items {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.combo-item-detail {
  font-size: 0.9rem;
  color: #666;
}

.item-total {
  font-weight: bold;
  color: #e44d26;
  font-size: 1.1rem;
  min-width: 120px;
  text-align: right;
}

.btn-remove {
  background: none;
  border: none;
  color: #dc3545;
  cursor: pointer;
  padding: 8px;
  transition: color 0.2s;
}

.btn-remove:hover {
  color: #c82333;
}

.cart-summary {
  background: #f8f9fa;
  padding: 24px;
  border-radius: 12px;
  margin-top: 24px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 12px;
  color: #666;
}

.summary-row.total {
  font-weight: bold;
  font-size: 1.2rem;
  color: #333;
  margin-top: 16px;
  padding-top: 16px;
  border-top: 2px solid #eee;
}

.btn-checkout {
  width: 100%;
  background: #e44d26;
  color: white;
  border: none;
  padding: 16px;
  border-radius: 8px;
  font-size: 1.1rem;
  font-weight: bold;
  cursor: pointer;
  transition: background-color 0.2s;
  margin-top: 16px;
}

.btn-checkout:hover {
  background: #d43d1f;
}

@media (max-width: 768px) {
  .modal-content {
    max-width: 100%;
  }

  .cart-item {
    flex-direction: column;
    text-align: center;
    padding: 12px;
  }

  .item-image img {
    width: 120px;
    height: 120px;
  }

  .quantity-controls {
    justify-content: center;
  }

  .btn-remove {
    position: absolute;
    top: 12px;
    right: 12px;
  }

  .item-total {
    text-align: center;
    margin-top: 12px;
  }
}
</style>
