<template>
  <div class="cart-button">
    <button @click="goToCart" class="btn-cart">
      <i class="fas fa-shopping-cart"></i>
      <span v-if="cartItemCount > 0" class="cart-count">{{ cartItemCount }}</span>
    </button>
  </div>
</template>
  
<script setup>
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'

const router = useRouter()
const cartItems = ref([])

const cartItemCount = computed(() => {
  return cartItems.value.reduce((total, item) => total + item.soLuong, 0)
})

const goToCart = () => {
  router.push('/cart')
}

const FetchCart = async () => {
  try {
    const response = await fetch(`https://localhost:7139/api/Cart/120`, {
      method: 'GET',
      Headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('ERROR', response.status)
    }
    const result = await response.json()
    cartItems.value = result.cartItems
  } catch (error) {
    console.log(error)
  }
}

onMounted(() => {
  FetchCart()
  window.addEventListener('updateCart', FetchCart)
})

onUnmounted(() => {
  window.removeEventListener('updateCart', FetchCart)
})
</script>
  
<style scoped>
.cart-button {
  position: relative;
}

.btn-cart {
  background: none;
  border: none;
  color: #333;
  font-size: 1.2rem;
  cursor: pointer;
  padding: 8px;
  position: relative;
}

.cart-count {
  position: absolute;
  top: -5px;
  right: -5px;
  background: #e44d26;
  color: white;
  font-size: 0.8rem;
  padding: 2px 6px;
  border-radius: 50%;
  min-width: 18px;
  text-align: center;
}

.btn-cart:hover {
  color: #e44d26;
}
</style>