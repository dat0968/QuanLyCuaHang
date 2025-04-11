<template>
  <div class="cart-page">
    <div class="container py-5">
      <div class="cart-content">
        <div class="cart-header">
          <h3>Giỏ hàng của bạn</h3>
        </div>

        <div v-if="cartItems.length === 0" class="empty-cart">
          <i class="fas fa-shopping-cart"></i>
          <p>Giỏ hàng trống</p>
          <button class="btn-continue-shopping" @click="$router.push('/')">Tiếp tục mua sắm</button>
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
                  :disabled="item.soLuong <= 1"
                  @click="UpdateCart(item, -1)"
                >
                  -
                </button>
                <input
                  type="number"
                  v-model="item.soLuong"
                  :min="1"
                  :max="item.soLuongTon"
                  class="quantity-input"
                  :class="{ error: quantityError[item.maCtsp] }"
                  @change="handleQuantityChange(item, $event)"
                />
                <button
                  class="btn-quantity"
                  :disabled="item.soLuong >= item.soLuongTon"
                  @click="UpdateCart(item, 1)"
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
            <button @click="RemoveCart(item.id)" class="btn-remove">
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
                  :disabled="combo.soLuong <= 1"
                  @click="UpdateCart(combo, -1)"
                >
                  -
                </button>
                <input
                  type="number"
                  v-model="combo.soLuong"
                  :min="1"
                  :max="combo.soLuongTon"
                  class="quantity-input"
                  :class="{ error: quantityError[combo.maCtsp] }"
                  @change="handleQuantityChange(combo, $event)"
                />
                <button
                  class="btn-quantity"
                  :disabled="combo.soLuong >= combo.soLuongTon"
                  @click="UpdateCart(combo, 1)"
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
            <button class="btn-remove" @click="RemoveCart(combo.id)">
              <i class="fas fa-trash"></i>
            </button>
          </div>
        </div>

        <div v-if="cartItems.length > 0" class="cart-summary">
          <div class="summary-row">
            <span>Tổng cộng:</span>
            <span>{{ subtotal.toLocaleString('vi-VN') }}đ</span>
          </div>

          <button class="btn-checkout" @click="proceedToCheckout">Tiến hành thanh toán</button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ReadToken, ValidateToken } from '../../Authentication_Authorization/auth.js'
import { ref, computed, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Swal from 'sweetalert2'
import Cookies from 'js-cookie'
const router = useRouter()
let accesstoken = Cookies.get('accessToken')
let refreshtoken = Cookies.get('refreshToken')
let IdUser = ''
const cartItems = ref([])
const quantityError = ref({})
const oldQuantities = ref({})
const FetchCart = async () => {
  try {
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if (validateToken == true) {
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if (readtoken) {
        IdUser = readtoken.IdUser
      } else {
        router.push('/Login')
        return
      }
    }
    const response = await fetch(`https://localhost:7139/api/Cart/${IdUser}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accesstoken}`,
      },
    })
    if (response.status == 401) {
      Swal.fire({
        icon: 'error',
        title: 'Phiên của bạn đã hết hoặc bạn chưa đăng nhập, vui lòng đăng nhập lại!',
        timer: 2000,
        showConfirmButton: false,
      })
      router.push('/Login')
      return
    }
    if (!response.ok) {
      throw new Error('ERROR ' + response.status)
    }
    const result = await response.json()
    cartItems.value = result.cartItems
    result.cartItems.forEach((item) => {
      oldQuantities.value[item.maCtsp] = item.soLuong
    })
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
// Xóa giỏ hàng
const RemoveCart = async (maGioHang) => {
  try {
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if (validateToken == true) {
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if (readtoken) {
        IdUser = readtoken.IdUser
      }
    } else {
      router.push('/Login')
      return
    }
    Swal.fire({
      title: 'Bạn có muốn xóa sản phẩm/combo này ra khỏi giỏ hàng ?',
      showCancelButton: true,
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Hủy',
    }).then(async (result) => {
      if (result.isConfirmed) {
        console.log('Dữ liệu hợp lệ, chuẩn bị gửi request')
        const response = await fetch(`https://localhost:7139/api/Cart/${maGioHang}/${IdUser}`, {
          method: 'DELETE',
          headers: {
            'Content-type': 'application/json',
            'Authorization': `Bearer ${accesstoken}`,
          },
        })
        if (response.status == 401) {
          router.push('/Error/401')
          return;
        }
        if (!response.ok) {
          throw new Error('Lỗi ' + response.status)
        }
        const result = await response.json()
        if (result.success) {
          FetchCart()
          Swal.fire(result.message, '', 'success')
        } else {
          Swal.fire(result.message, '', 'error')
        }
      }
    })
  } catch (error) {
    console.log('Lỗi ' + error)
  }
}
// Update giỏ hàng
const UpdateCart = async (data, quantity) => {
  try {
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if (validateToken == true) {
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if(readtoken){
        IdUser = readtoken.IdUser
      }
    } else {
      router.push('/Login')
      return
    }
    const content = {
      maKh: IdUser,
      maCtsp: data.maCtsp,
      maCombo: data.maCombo ?? null,
      soLuong: quantity,
      donGia: data.donGia,
    }
    const response = await fetch(`https://localhost:7139/api/Cart/${data.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${accesstoken}`,
      },
      body: JSON.stringify(content),
    })
    if(response.status == 401){
      router.push('/Error/401')
      return;
    }
    const result = await response.json()
    if (result.success) {
      // Cập nhật giá trị cũ sau khi thay đổi thành công
      FetchCart()
    } else {
      Swal.fire(result.message, '', 'error')
      // Cập nhật giá trị cũ sau khi thay đổi thành công
      data.soLuong = oldQuantities.value[data.maCtsp]
    }
  } catch (error) {
    console.log(error)
  }
}
const handleQuantityChange = (item, event) => {
  if (item.soLuong === '' || item.soLuong < 1) {
    item.soLuong = oldQuantities.value[item.maCtsp] || item.soLuong
    Swal.fire('Số lượng giỏ hàng phải tối thiểu bằng 1', '', 'error')
    return
  }
  const newQuantity = parseInt(event.target.value)
  const oldQuantity = oldQuantities.value[item.maCtsp] || item.soLuong
  const difference = newQuantity - oldQuantity
  // Gọi UpdateCart với chênh lệch
  if (difference !== 0) {
    UpdateCart(item, difference)
  }
}
const subtotal = computed(() => {
  return cartItems.value.reduce((sum, item) => {
    return sum + item.donGia * item.soLuong
  }, 0)
})

// Lấy URL hình ảnh
const getImageUrl = (imageName) => {
  return `https://localhost:7139/HinhAnh/Food_Drink/${imageName}`
}

const proceedToCheckout = () => {
  router.push('/checkout')
}

onMounted(() => {
  FetchCart()
})
</script>

<style scoped>
.cart-page {
  padding: 100px 0;
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  min-height: 100vh;
}

.cart-content {
  background: white;
  border-radius: 12px;
  box-shadow: 0 4px 20px rgba(0, 0, 0, 0.15);
  padding: 24px;
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
  padding-bottom: 16px;
  border-bottom: 2px solid #f0f0f0;
}

.cart-header h3 {
  font-size: 1.5rem;
  color: #333;
  margin: 0;
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
  width: 100px;
  height: 100px;
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

@media (max-width: 768px) {
  .cart-content {
    padding: 16px;
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