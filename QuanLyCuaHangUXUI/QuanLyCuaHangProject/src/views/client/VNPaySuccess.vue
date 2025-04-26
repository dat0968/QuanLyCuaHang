<template>
  <div class="success-container">
    <div class="success-content">
      <div class="success-icon">
        <svg
          xmlns="http://www.w3.org/2000/svg"
          width="80"
          height="80"
          viewBox="0 0 24 24"
          fill="none"
          stroke="#4CAF50"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
        >
          <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"></path>
          <polyline points="22 4 12 14.01 9 11.01"></polyline>
        </svg>
      </div>
      <h1 class="success-title">Thanh toán thành công</h1>
      <p class="success-message">Cảm ơn bạn đã mua hàng tại cửa hàng của chúng tôi</p>

      <div class="order-details">
        <div class="detail-item">
          <span class="detail-label">Mã đơn hàng</span>
          <span class="detail-value">{{ orderCode }}</span>
        </div>
        <div class="detail-item">
          <span class="detail-label">Số tiền</span>
          <span class="detail-value">{{ amount.toLocaleString('vi-VN') }}đ</span>
        </div>
        <div class="detail-item">
          <span class="detail-label">Thời gian</span>
          <span class="detail-value">{{ formatDateTime(paymentTime) }}</span>
        </div>
      </div>

      <div class="action-buttons">
        <button @click="goToHome" class="btn-home">
          <i class="bi bi-house-door"></i>
          Về trang chủ
        </button>
       
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { useRouter, useRoute } from 'vue-router'
import Swal from 'sweetalert2'

const router = useRouter()
const route = useRoute()

const orderCode = ref('')
const amount = ref(0)
const paymentTime = ref(new Date().toISOString()) // Set default to current time

const formatDateTime = (dateString) => {
  if (!dateString) return ''
  const date = new Date(dateString)
  return date.toLocaleString('vi-VN', {
    day: '2-digit',
    month: '2-digit',
    year: 'numeric',
    hour: '2-digit',
    minute: '2-digit',
  })
}

onMounted(() => {
  orderCode.value = route.params.OderId || ''
  amount.value = parseInt(route.params.Total / 100 || 0) 
  paymentTime.value = route.query.vnp_PayDate || paymentTime.value // Use query or default to current time
})

const goToHome = () => {
  router.push('/')
}

const viewOrder = () => {
  router.push('/order-history')
}
</script>

<style scoped>
.success-container {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  padding: 20px;
}

.success-content {
  background: white;
  padding: 40px;
  border-radius: 20px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.1);
  text-align: center;
  max-width: 500px;
  width: 100%;
}

.success-icon {
  margin-bottom: 20px;
  animation: scaleIn 0.5s ease-out;
}

.success-title {
  color: #2c3e50;
  font-size: 28px;
  font-weight: 600;
  margin-bottom: 15px;
}

.success-message {
  color: #7f8c8d;
  font-size: 16px;
  margin-bottom: 30px;
}

.order-details {
  background: #f8f9fa;
  border-radius: 15px;
  padding: 20px;
  margin-bottom: 30px;
}

.detail-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #e9ecef;
}

.detail-item:last-child {
  border-bottom: none;
}

.detail-label {
  color: #6c757d;
  font-size: 14px;
}

.detail-value {
  color: #2c3e50;
  font-weight: 500;
}

.action-buttons {
  display: flex;
  gap: 15px;
  justify-content: center;
}

.btn-home,
.btn-order {
  padding: 12px 25px;
  border-radius: 10px;
  font-weight: 500;
  display: flex;
  align-items: center;
  gap: 8px;
  transition: all 0.3s ease;
  cursor: pointer;
}

.btn-home {
  background: #4caf50;
  color: white;
  border: none;
}

.btn-home:hover {
  background: #45a049;
  transform: translateY(-2px);
}

.btn-order {
  background: white;
  color: #4caf50;
  border: 2px solid #4caf50;
}

.btn-order:hover {
  background: #4caf50;
  color: white;
  transform: translateY(-2px);
}

@keyframes scaleIn {
  from {
    transform: scale(0);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}

@media (max-width: 576px) {
  .success-content {
    padding: 30px 20px;
  }

  .action-buttons {
    flex-direction: column;
  }

  .btn-home,
  .btn-order {
    width: 100%;
    justify-content: center;
  }
}
</style> 