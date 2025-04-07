<template>
    <button class="cart-button" @click="openCartModal">
      <i class="fas fa-shopping-cart"></i>
      <span class="cart-count" v-if="cartItemCount > 0">{{ cartItemCount }}</span>
    </button>
  </template>
  
  <script setup>
  import { ref, computed, onMounted } from 'vue';
  import { jwtDecode } from 'jwt-decode';
  import axios from 'axios';
  import Cookies from 'js-cookie';
  
  const emit = defineEmits(['open-cart']);
  const cartItems = ref([]);
  const maKh = ref(null);
  const apiBaseUrl = 'https://localhost:7139/api/Cart';
  
  const checkLoginAndFetchUserInfo = async () => {
    const token = ref(Cookies.get('accessToken'));
    if (!token) return false;
    try {
      const decodedToken = jwtDecode(token);
      maKh.value = parseInt(decodedToken.sub);
      return true;
    } catch (error) {
      console.error('Lỗi khi giải mã token:', error);
      return false;
    }
  };
  
  const fetchCartItems = async () => {
    try {
      const token = ref(Cookies.get('accessToken'));
      if (!token || !maKh.value) return;
      const response = await axios.get(`${apiBaseUrl}/${maKh.value}`, {
        headers: {
          Authorization: `Bearer ${token}`,
        },
      });
      cartItems.value = response.data;
    } catch (error) {
      console.error('Lỗi khi lấy giỏ hàng:', error);
    }
  };
  
  const cartItemCount = computed(() => {
    return cartItems.value.reduce((total, item) => total + item.soLuong, 0);
  });
  
  const openCartModal = () => {
    emit('open-cart');
  };
  
  onMounted(async () => {
    const loggedIn = await checkLoginAndFetchUserInfo();
    if (loggedIn) await fetchCartItems();
  });
  </script>
  
  <style scoped>
  .cart-button {
    position: relative;
    background: none;
    border: none;
    font-size: 24px;
    color: #ff6f61;
    cursor: pointer;
    transition: color 0.3s ease;
    padding: 10px;
  }
  
  .cart-button:hover {
    color: #e65b50;
  }
  
  .cart-count {
    position: absolute;
    top: 0;
    right: 0;
    background-color: #e74c3c;
    color: #fff;
    font-size: 12px;
    font-weight: 600;
    border-radius: 50%;
    width: 20px;
    height: 20px;
    display: flex;
    align-items: center;
    justify-content: center;
  }
  </style>