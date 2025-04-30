<template>
  <div v-if="accessToken != ''" class="suggested-products">
    <h2>Gợi ý cho bạn</h2>
    <div class="product-list">
      <a :href='`http://localhost:5174/product/${item.maSp}`' class="product-card" v-for="item in listProductRecommend" :key="item.maSp">
        <img
          width="150"
          height="150"
          style="object-fit: cover; border-radius: 8px"
          :src="`${getApiUrl}/HinhAnh/Food_Drink/${item.chitietsanphams?.[0]?.hinhanhs?.[0]?.tenHinhAnh}`"
          :alt="`${getApiUrl}/HinhAnh/Food_Drink/${item.chitietsanphams?.[0]?.hinhanhs?.[0]?.tenHinhAnh}`"
        />
        <h3>{{ item.tenSanPham }}</h3>
        <p class="price">{{ formatPriceRange(item.khoangGia) }}</p>
        <button>Mua ngay</button>
      </a>
    </div>
  </div>
</template>
  
  <script setup>
import Cookies from 'js-cookie'
import { ReadToken, ValidateToken } from '../../Authentication_Authorization/auth.js'
import { GetApiUrl } from '@constants/api'
import { onMounted, ref } from 'vue'
const props = defineProps({
  token: String,
})

const formatPriceRange = (range) => {
  // Tách chuỗi thành các từ/cụm bằng khoảng trắng
  return range
    .split('-')
    .map((part) => {
      const number = parseFloat(part.trim())
      if (isNaN(number)) return part.trim() // Nếu không phải số, giữ nguyên
      return Math.round(number).toLocaleString('vi-VN') + ' VNĐ' // Làm tròn và định dạng
    })
    .join(' - ')
}

const accessToken = ref('')
let idUser = ''
const getApiUrl = ref('')
getApiUrl.value = GetApiUrl()
const listProductRecommend = ref([])
const listDetailProductRecmmend = ref([])
accessToken.value = props.token
let refreshToken = Cookies.get('refreshToken')
onMounted(async () => {
  let validatetoken = await ValidateToken(accessToken.value, refreshToken)
  if (validatetoken) {
    accessToken.value = Cookies.get('accessToken')
    let readtoken = ReadToken(accessToken.value)
    if (readtoken) {
      idUser = readtoken.IdUser
      const response = await fetch(`${getApiUrl.value}/api/Home/recommend/${idUser}`, {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      })
      const result = await response.json()
      listProductRecommend.value = result
      listDetailProductRecmmend.value = result.chitietsanphams
    }
  } else {
    accessToken.value = ''
  }
})
</script>
  
  <style scoped>
.suggested-products {
  padding: 20px;
  background-color: #f9f9f9;
  text-align: center;
}
.suggested-products h2 {
  font-size: 24px;
  margin-bottom: 16px;
}
.product-list {
  display: flex;
  flex-wrap: wrap;
  gap: 16px;
  justify-content: center; /* Căn giữa các sản phẩm */
}
.product-card {
  background-color: white;
  border: 1px solid #ddd;
  border-radius: 8px;
  padding: 12px;
  width: 200px;
  text-align: center;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  text-decoration: none;
  color: inherit;
}
.product-card img {
  width: 100%;
  border-radius: 6px;
}
.product-card h3 {
  font-size: 16px;
  margin: 10px 0 5px;
}
.product-card .price {
  color: #e91e63;
  font-weight: bold;
  margin-bottom: 10px;
}
.product-card button {
  background-color: #2196f3;
  color: white;
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
}
.product-card button:hover {
  background-color: #1976d2;
}
.product-card .price {
  color: #e91e63;
  font-weight: bold;
  margin-bottom: 10px;
  font-size: 14px; /* Giảm kích thước chữ ở đây */
}
</style>
  