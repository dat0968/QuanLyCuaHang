<template>
  <div class="cart-modal" v-if="isOpen">
    <div class="cart-overlay" @click="closeCartModal"></div>
    <div class="cart-content">
      <div class="cart-header">
        <h3>Giỏ hàng</h3>
        <button class="close-btn" @click="closeCartModal">×</button>
      </div>

      <div class="cart-items">
        <div v-for="item in cartItems" :key="item.maSp" class="cart-item">
          <div class="item-image">
            <img :src="item.hinhAnh" :alt="item.tenSanPham" />
          </div>
          <div class="item-details">
            <h4>{{ item.tenSanPham }}</h4>
            <p class="price">{{ item.gia.toLocaleString('vi-VN') }}đ</p>
            <div class="quantity-controls">
              <button class="btn-quantity">-</button>
              <span>{{ item.soLuong }}</span>
              <button class="btn-quantity">+</button>
            </div>
          </div>
          <div class="item-total">{{ (item.gia * item.soLuong).toLocaleString('vi-VN') }}đ</div>
          <button class="btn-remove">
            <i class="fas fa-trash"></i>
          </button>
        </div>
      </div>

      <div class="cart-summary">
        <div class="summary-row">
          <span>Tạm tính:</span>
          <span>{{ total.toLocaleString('vi-VN') }}đ</span>
        </div>
        <div class="summary-row">
          <span>Phí vận chuyển:</span>
          <span>30.000đ</span>
        </div>
        <div class="summary-row total">
          <span>Tổng cộng:</span>
          <span>{{ (total + 30000).toLocaleString('vi-VN') }}đ</span>
        </div>
        <button class="btn-checkout">Tiến hành thanh toán</button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const isOpen = ref(false)

const openCartModal = () => {
  isOpen.value = true
}

const closeCartModal = () => {
  isOpen.value = false
}

defineExpose({
  openCartModal,
  closeCartModal,
})

// Dữ liệu mẫu cho giao diện tĩnh
const cartItems = [
  {
    maSp: 1,
    tenSanPham: 'Sản phẩm 1',
    gia: 150000,
    soLuong: 2,
    hinhAnh: 'https://via.placeholder.com/100',
  },
  {
    maSp: 2,
    tenSanPham: 'Sản phẩm 2',
    gia: 200000,
    soLuong: 1,
    hinhAnh: 'https://via.placeholder.com/100',
  },
]

const total = cartItems.reduce((sum, item) => sum + item.gia * item.soLuong, 0)
</script>

<style scoped>
.cart-modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  z-index: 1000;
  display: flex;
  align-items: center;
  justify-content: center;
}

.cart-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(4px);
}

.cart-content {
  position: relative;
  background: white;
  border-radius: 8px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  padding: 20px;
  max-width: 800px;
  width: 90%;
  max-height: 90vh;
  overflow-y: auto;
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    transform: translateY(-20px);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

.cart-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.close-btn {
  background: none;
  border: none;
  font-size: 24px;
  color: #666;
  cursor: pointer;
  padding: 0 10px;
}

.close-btn:hover {
  color: #333;
}

.cart-count {
  color: #666;
  font-size: 0.9em;
}

.cart-items {
  margin-bottom: 20px;
}

.cart-item {
  display: flex;
  align-items: center;
  padding: 15px;
  border-bottom: 1px solid #eee;
  gap: 15px;
}

.item-image img {
  width: 80px;
  height: 80px;
  object-fit: cover;
  border-radius: 4px;
}

.item-details {
  flex: 1;
}

.item-details h4 {
  margin: 0 0 5px 0;
  font-size: 1.1em;
}

.price {
  color: #e44d26;
  font-weight: bold;
  margin: 5px 0;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 10px;
}

.btn-quantity {
  background: #f8f9fa;
  border: 1px solid #ddd;
  border-radius: 4px;
  width: 30px;
  height: 30px;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
}

.btn-quantity:hover {
  background: #e9ecef;
}

.item-total {
  font-weight: bold;
  color: #e44d26;
}

.btn-remove {
  background: none;
  border: none;
  color: #dc3545;
  cursor: pointer;
  padding: 5px;
}

.btn-remove:hover {
  color: #c82333;
}

.cart-summary {
  background: #f8f9fa;
  padding: 20px;
  border-radius: 8px;
}

.summary-row {
  display: flex;
  justify-content: space-between;
  margin-bottom: 10px;
}

.summary-row.total {
  font-weight: bold;
  font-size: 1.2em;
  border-top: 1px solid #ddd;
  padding-top: 10px;
  margin-top: 10px;
}

.btn-checkout {
  width: 100%;
  padding: 12px;
  background: #28a745;
  color: white;
  border: none;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  margin-top: 15px;
}

.btn-checkout:hover {
  background: #218838;
}

@media (max-width: 576px) {
  .cart-content {
    width: 95%;
    padding: 15px;
  }

  .cart-item {
    flex-direction: column;
    text-align: center;
    gap: 10px;
  }

  .item-details {
    width: 100%;
  }

  .quantity-controls {
    justify-content: center;
  }
}
</style>