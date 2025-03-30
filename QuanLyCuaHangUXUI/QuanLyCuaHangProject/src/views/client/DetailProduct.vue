<script setup>
import { ref, onMounted } from 'vue'
import { useRoute } from 'vue-router'
import 'animate.css'

const route = useRoute()
const product = ref(null)
const selectedVariant = ref(null)
const quantity = ref(1)

// Thêm hàm scrollToTop
const scrollToTop = () => {
  window.scrollTo({
    top: 0,
    behavior: 'smooth',
  })
}

// Lấy chi tiết sản phẩm
async function fetchProductDetail() {
  try {
    const response = await fetch(
      `https://localhost:7139/api/Home/products/${route.params.id}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    )
    if (!response.ok) {
      throw new Error('Error' + response.status)
    }
    const data = await response.json()
    product.value = data
    // Mặc định chọn variant đầu tiên
    if (product.value.chitietsanphams && product.value.chitietsanphams.length > 0) {
      selectedVariant.value = product.value.chitietsanphams[0]
    }
  } catch (error) {
    console.error('Error fetching product detail:', error)
  }
}

// Xử lý khi chọn variant
const selectVariant = (variant) => {
  selectedVariant.value = variant
}

// Xử lý tăng/giảm số lượng
const increaseQuantity = () => {
  if (selectedVariant.value && quantity.value < selectedVariant.value.soLuongTon) {
    quantity.value++
  }
}

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--
  }
}

// Xử lý thêm vào giỏ hàng
const addToCart = () => {
  if (!selectedVariant.value) return

  const cartItem = {
    maCtsp: selectedVariant.value.maCtsp,
    tenSanPham: selectedVariant.value.tenSanPham,
    donGia: selectedVariant.value.donGia,
    soLuong: quantity.value,
    hinhAnh: selectedVariant.value.hinhanhs?.[0]?.tenHinhAnh,
    kichThuoc: selectedVariant.value.kichThuoc,
    huongVi: selectedVariant.value.huongVi,
  }

  // TODO: Thêm logic xử lý giỏ hàng ở đây
  console.log('Add to cart:', cartItem)
}

onMounted(() => {
  scrollToTop()
  fetchProductDetail()
})
</script>

<template>
  <div class="product-detail">
    <div class="container">
      <div v-if="product" class="row">
        <!-- Phần ảnh sản phẩm -->
        <div class="col-md-6">
          <div class="product-images">
            <div class="main-image">
              <img
                :src="
                  selectedVariant?.hinhanhs?.[0]?.tenHinhAnh
                    ? `https://localhost:7139/HinhAnh/Food_Drink/${selectedVariant.hinhanhs[0].tenHinhAnh}`
                    : '../assets/client/img/food_menu/chicken_default.png'
                "
                :alt="product.tenSanPham"
                class="img-fluid"
              />
            </div>
            <div class="thumbnail-images">
              <div
                v-for="variant in product.chitietsanphams"
                :key="variant.maCtsp"
                class="thumbnail"
                :class="{ active: selectedVariant?.maCtsp === variant.maCtsp }"
                @click="selectVariant(variant)"
              >
                <img
                  :src="
                    variant.hinhanhs?.[0]?.tenHinhAnh
                      ? `https://localhost:7139/HinhAnh/Food_Drink/${variant.hinhanhs[0].tenHinhAnh}`
                      : '../assets/client/img/food_menu/chicken_default.png'
                  "
                  :alt="variant.tenSanPham"
                />
              </div>
            </div>
          </div>
        </div>

        <!-- Phần thông tin sản phẩm -->
        <div class="col-md-6">
          <div class="product-info">
            <h1 class="product-title">{{ product.tenSanPham }}</h1>
            <p class="product-description">{{ product.moTa }}</p>

            <!-- Phần chọn variant -->
            <div class="variant-selection">
              <h3>Chọn phiên bản</h3>
              <div class="variant-buttons">
                <button
                  v-for="variant in product.chitietsanphams"
                  :key="variant.maCtsp"
                  :class="{ active: selectedVariant?.maCtsp === variant.maCtsp }"
                  @click="selectVariant(variant)"
                >
                  {{ variant.huongVi || variant.kichThuoc || 'Mặc định' }}
                </button>
              </div>
            </div>

            <!-- Phần giá và số lượng -->
            <div class="price-quantity">
              <div class="price">
                <span class="current-price"
                  >{{ selectedVariant?.donGia?.toLocaleString('vi-VN') }} đ</span
                >
              </div>
              <div class="quantity">
                <button @click="decreaseQuantity" :disabled="quantity <= 1">-</button>
                <span>{{ quantity }}</span>
                <button
                  @click="increaseQuantity"
                  :disabled="!selectedVariant || quantity >= selectedVariant.soLuongTon"
                >
                  +
                </button>
              </div>
            </div>

            <!-- Phần thêm vào giỏ hàng -->
            <div class="add-to-cart">
              <button
                class="btn-add-to-cart"
                @click="addToCart"
                :disabled="!selectedVariant || selectedVariant.soLuongTon === 0"
              >
                <i class="bi bi-cart"></i> Thêm vào giỏ hàng
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.product-detail {
  padding: 200px 0 40px 0;
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  min-height: 100vh;
}

.product-images {
  background: #fff;
  border-radius: 15px;
  padding: 20px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.main-image {
  width: 100%;
  height: 400px;
  margin-bottom: 20px;
  border-radius: 10px;
  overflow: hidden;
}

.main-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.thumbnail-images {
  display: flex;
  gap: 10px;
  overflow-x: auto;
  padding: 10px 0;
}

.thumbnail {
  width: 80px;
  height: 80px;
  border-radius: 8px;
  overflow: hidden;
  cursor: pointer;
  border: 2px solid transparent;
  transition: all 0.3s ease;
}

.thumbnail.active {
  border-color: #ff8c00;
}

.thumbnail img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.product-info {
  background: #fff;
  border-radius: 15px;
  padding: 30px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
}

.product-title {
  font-size: 24px;
  font-weight: 700;
  margin-bottom: 15px;
  color: #333;
}

.product-description {
  font-size: 16px;
  color: #666;
  margin-bottom: 25px;
  line-height: 1.6;
}

.variant-selection {
  margin-bottom: 25px;
}

.variant-selection h3 {
  font-size: 18px;
  margin-bottom: 15px;
  color: #333;
}

.variant-buttons {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.variant-buttons button {
  padding: 8px 20px;
  border: 1px solid #ddd;
  border-radius: 20px;
  background: #fff;
  cursor: pointer;
  transition: all 0.3s ease;
}

.variant-buttons button.active {
  background: #ff8c00;
  color: #fff;
  border-color: #ff8c00;
}

.price-quantity {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 25px;
  padding: 15px 0;
  border-top: 1px solid #eee;
  border-bottom: 1px solid #eee;
}

.current-price {
  font-size: 24px;
  font-weight: 700;
  color: #ff8c00;
}

.quantity {
  display: flex;
  align-items: center;
  gap: 15px;
}

.quantity button {
  width: 35px;
  height: 35px;
  border-radius: 50%;
  border: 1px solid #ddd;
  background: #fff;
  font-size: 18px;
  cursor: pointer;
  transition: all 0.3s ease;
}

.quantity button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity button:not(:disabled):hover {
  background: #ff8c00;
  color: #fff;
  border-color: #ff8c00;
}

.quantity span {
  font-size: 18px;
  font-weight: 600;
  min-width: 30px;
  text-align: center;
}

.btn-add-to-cart {
  width: 100%;
  padding: 15px;
  background: #ff8c00;
  color: #fff;
  border: none;
  border-radius: 25px;
  font-size: 16px;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 10px;
}

.btn-add-to-cart:hover:not(:disabled) {
  background: #e67e22;
  transform: translateY(-2px);
}

.btn-add-to-cart:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

@media (max-width: 768px) {
  .main-image {
    height: 300px;
  }

  .thumbnail {
    width: 60px;
    height: 60px;
  }

  .product-info {
    padding: 20px;
  }

  .product-title {
    font-size: 20px;
  }

  .product-description {
    font-size: 14px;
  }
}

@media (max-width: 576px) {
  .main-image {
    height: 250px;
  }

  .thumbnail {
    width: 50px;
    height: 50px;
  }

  .variant-buttons button {
    padding: 6px 15px;
    font-size: 14px;
  }

  .current-price {
    font-size: 20px;
  }

  .quantity button {
    width: 30px;
    height: 30px;
    font-size: 16px;
  }

  .quantity span {
    font-size: 16px;
  }

  .btn-add-to-cart {
    padding: 12px;
    font-size: 14px;
  }
}
</style> 