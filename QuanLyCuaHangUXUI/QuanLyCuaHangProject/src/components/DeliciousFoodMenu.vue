<script setup>
import { ref, onMounted, watch } from 'vue'
// import 'animate.css'

const selectedCategory = ref(-1)
const products = ref([])
const combos = ref([])
const bestSellers = ref([])
const sortOption = ref('default')

const selectCategory = (categorySelected) => {
  selectedCategory.value = categorySelected
}

// Danh sách danh mục
const categories = ref([])
async function fetchCategory() {
  const response = await fetch('https://localhost:7139/api/Categories', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  })
  if (!response.ok) {
    throw new Error('Error' + response.status)
  }
  const dataCategories = await response.json()
  categories.value = dataCategories
}

// Lấy danh sách sản phẩm
async function fetchProducts() {
  try {
    const response = await fetch('https://localhost:7139/api/Home/all-products', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('Error' + response.status)
    }
    const data = await response.json()
    products.value = data.data
  } catch (error) {
    console.error('Error fetching products:', error)
  }
}

// Lấy danh sách combo
async function fetchCombos() {
  try {
    const response = await fetch('https://localhost:7139/api/Home/combos', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('Error' + response.status)
    }
    const data = await response.json()
    combos.value = data
  } catch (error) {
    console.error('Error fetching combos:', error)
  }
}

// Lấy danh sách món bán chạy
async function fetchBestSellers() {
  try {
    const response = await fetch('https://localhost:7139/api/Home/best-sellers', {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('Error' + response.status)
    }
    const data = await response.json()
    bestSellers.value = data
  } catch (error) {
    console.error('Error fetching best sellers:', error)
  }
}

// Lấy sản phẩm theo danh mục
async function fetchProductsByCategory(categoryId) {
  try {
    const response = await fetch(
      `https://localhost:7139/api/Home/all-products?filterCategories=${categoryId}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      },
    )
    if (!response.ok) {
      throw new Error('Error' + response.status)
    }
    const data = await response.json()
    products.value = data.data
  } catch (error) {
    console.error('Error fetching products by category:', error)
  }
}

// Tính tổng giá combo
const calculateTotalPrice = (combo) => {
  if (!combo.chitietcombos || combo.chitietcombos.length === 0) return 0
  return combo.chitietcombos.reduce((total, item) => {
    const variant = item.chitietsanphams[0]
    if (!variant) return total
    return total + variant.donGia * item.soLuongSp
  }, 0)
}

// Tính giá sau giảm giá của combo
const calculateDiscountedPrice = (combo) => {
  const totalPrice = calculateTotalPrice(combo)
  if (combo.soTienGiam) {
    return totalPrice - combo.soTienGiam
  }
  if (combo.phanTramGiam) {
    return totalPrice - (totalPrice * combo.phanTramGiam) / 100
  }
  return totalPrice
}

// Thêm hàm sắp xếp sản phẩm
const sortProducts = () => {
  if (!products.value) return

  switch (sortOption.value) {
    case 'price-asc':
      products.value.sort((a, b) => {
        const priceA = a.chitietsanphams?.[0]?.donGia || 0
        const priceB = b.chitietsanphams?.[0]?.donGia || 0
        return priceA - priceB
      })
      break
    case 'price-desc':
      products.value.sort((a, b) => {
        const priceA = a.chitietsanphams?.[0]?.donGia || 0
        const priceB = b.chitietsanphams?.[0]?.donGia || 0
        return priceB - priceA
      })
      break
    default:
      // Không sắp xếp, giữ nguyên thứ tự
      break
  }
}

// Thêm hàm sắp xếp combo
const sortCombos = () => {
  if (!combos.value) return

  switch (sortOption.value) {
    case 'price-asc':
      combos.value.sort((a, b) => {
        const priceA = calculateDiscountedPrice(a)
        const priceB = calculateDiscountedPrice(b)
        return priceA - priceB
      })
      break
    case 'price-desc':
      combos.value.sort((a, b) => {
        const priceA = calculateDiscountedPrice(a)
        const priceB = calculateDiscountedPrice(b)
        return priceB - priceA
      })
      break
    default:
      // Không sắp xếp, giữ nguyên thứ tự
      break
  }
}

// Thêm hàm sắp xếp món bán chạy
const sortBestSellers = () => {
  if (!bestSellers.value) return

  switch (sortOption.value) {
    case 'price-asc':
      bestSellers.value.sort((a, b) => {
        const priceA = a.chitietsanphams?.[0]?.donGia || 0
        const priceB = b.chitietsanphams?.[0]?.donGia || 0
        return priceA - priceB
      })
      break
    case 'price-desc':
      bestSellers.value.sort((a, b) => {
        const priceA = a.chitietsanphams?.[0]?.donGia || 0
        const priceB = b.chitietsanphams?.[0]?.donGia || 0
        return priceB - priceA
      })
      break
    default:
      // Không sắp xếp, giữ nguyên thứ tự
      break
  }
}

// Watch sortOption để sắp xếp lại khi thay đổi
watch(sortOption, () => {
  switch (selectedCategory.value) {
    case -1:
      sortProducts()
      break
    case -2:
      sortBestSellers()
      break
    case -3:
      sortCombos()
      break
    default:
      sortProducts()
      break
  }
})

// Watch selectedCategory để reset sortOption khi chuyển danh mục
watch(selectedCategory, async (newValue) => {
  sortOption.value = 'default'
  if (newValue === -1) {
    await fetchProducts()
  } else if (newValue === -2) {
    await fetchBestSellers()
  } else if (newValue === -3) {
    await fetchCombos()
  } else {
    await fetchProductsByCategory(newValue)
  }
})

onMounted(() => {
  fetchCategory()
  fetchProducts()
})
</script>
<template>
  <div>
    <!-- food_menu start -->
    <section class="food_menu">
      <div class="container position-relative">
        <!-- Thanh tìm kiếm và sắp xếp -->
        <div class="search-sort-bar">
          <input type="text" placeholder="Tìm kiếm theo tên hoặc mô tả..." class="search-input" />
          <div class="sort-options">
            <select v-model="sortOption">
              <option value="default">Sắp xếp mặc định</option>
              <option value="price-asc">Giá: Thấp đến Cao</option>
              <option value="price-desc">Giá: Cao đến Thấp</option>
            </select>
          </div>
        </div>

        <!-- Danh sách danh mục -->
        <div class="category-list">
          <div class="category-item animate__animated animate__fadeIn">
            <button :class="{ active: selectedCategory === -3 }" @click="selectCategory(-3)">
              <div class="category-image">
                <img src="../assets/client/img/food_menu/chicken_combo.png" alt="combo" />
              </div>
              <span>Món ngon phải thử</span>
            </button>
          </div>
          <div class="category-item animate__animated animate__fadeIn">
            <button :class="{ active: selectedCategory === -1 }" @click="selectCategory(-1)">
              <div class="category-image">
                <img src="../assets/client/img/food_menu/chicken_default.png" alt="all-food" />
              </div>
              <span>Tất cả đồ ăn</span>
            </button>
          </div>
          <div class="category-item animate__animated animate__fadeIn">
            <button :class="{ active: selectedCategory === -2 }" @click="selectCategory(-2)">
              <div class="category-image">
                <img
                  src="../assets/client/img/food_menu/chicken_best_seller.png"
                  alt="best-seller"
                />
              </div>
              <span>Món bán chạy</span>
            </button>
          </div>
          <div
            v-for="category in categories"
            :key="category.maDanhMuc"
            class="category-item animate__animated animate__fadeIn"
          >
            <button
              :class="{ active: selectedCategory === category.maDanhMuc }"
              @click="selectCategory(category.maDanhMuc)"
            >
              <div class="category-image">
                <img src="../assets/client/img/food_menu/chicken_default.png" alt="category" />
              </div>
              <span>{{ category.tenDanhMuc }}</span>
            </button>
          </div>
        </div>

        <!-- Danh sách sản phẩm theo danh mục -->
        <div class="menu-content">
          <!-- Tất cả sản phẩm -->
          <div v-if="selectedCategory === -1" class="row">
            <div v-if="products.length === 0" class="col-12 text-center">
              <p class="text-muted">Không tìm thấy sản phẩm nào.</p>
            </div>
            <div
              v-else
              v-for="product in products"
              :key="product.maSp"
              class="col-sm-12 col-md-6 col-lg-4 mb-4 animate__animated animate__fadeInUp"
            >
              <div class="menu_item">
                <router-link
                  :to="{ name: 'DetailProduct', params: { id: product.maSp } }"
                  class="product-link"
                >
                  <div class="menu_img">
                    <img
                      :src="
                        product.chitietsanphams?.[0]?.hinhanhs?.[0]?.tenHinhAnh
                          ? `https://localhost:7139/HinhAnh/Food_Drink/${product.chitietsanphams[0].hinhanhs[0].tenHinhAnh}`
                          : '../assets/client/img/food_menu/chicken_default.png'
                      "
                      alt="product"
                      class="img-fluid"
                    />
                  </div>
                  <div class="menu_text">
                    <h3>{{ product.tenSanPham }}</h3>
                    <p class="product-description">{{ product.moTa }}</p>
                    <h5>{{ product.khoangGia }}</h5>
                  </div>
                </router-link>
                <router-link style="text-decoration: none;" :to="`/product/` + product.maSp" class="btn-order"><i class="fas fa-eye"></i> Xem chi tiết</router-link>
              </div>
            </div>
          </div>

          <!-- Món ngon phải thử -->
          <div v-else-if="selectedCategory === -3" class="row">
            <div v-if="combos.length === 0" class="col-12 text-center">
              <p class="text-muted">Không tìm thấy combo nào.</p>
            </div>
            <div
              v-else
              v-for="combo in combos"
              :key="combo.maCombo"
              class="col-sm-12 col-md-6 col-lg-4 mb-4 animate__animated animate__fadeInUp"
            >
              <div class="menu_item">
                <router-link
                  :to="{ name: 'DetailCombo', params: { id: combo.maCombo } }"
                  class="product-link"
                >
                  <div class="menu_img">
                    <img
                      :src="
                        combo.hinh
                          ? `https://localhost:7139/HinhAnh/Food_Drink/${combo.hinh}`
                          : '../assets/client/img/food_menu/combo_default.png'
                      "
                      alt="combo"
                      class="img-fluid"
                    />
                  </div>
                  <div class="menu_text">
                    <h3>{{ combo.tenCombo }}</h3>
                    <p class="product-description">{{ combo.moTa }}</p>
                    <h5>{{ combo.khoangGia }}</h5>
                  </div>
                </router-link>
                <router-link style="text-decoration: none;" :to="`/combo/` + combo.maCombo" class="btn-order"><i class="fas fa-eye"></i> Xem chi tiết</router-link>
              </div>
            </div>
          </div>

          <!-- Sản phẩm theo danh mục -->
          <div v-else class="row">
            <div v-if="products.length === 0" class="col-12 text-center">
              <p class="text-muted">Không tìm thấy sản phẩm nào.</p>
            </div>
            <div
              v-else
              v-for="product in products"
              :key="product.maSp"
              class="col-sm-12 col-md-6 col-lg-4 mb-4 animate__animated animate__fadeInUp"
            >
              <div class="menu_item">
                <router-link
                  :to="{ name: 'DetailProduct', params: { id: product.maSp } }"
                  class="product-link"
                >
                  <div class="menu_img">
                    <img
                      :src="
                        product.chitietsanphams?.[0]?.hinhanhs?.[0]?.tenHinhAnh
                          ? `https://localhost:7139/HinhAnh/Food_Drink/${product.chitietsanphams[0].hinhanhs[0].tenHinhAnh}`
                          : '../assets/client/img/food_menu/chicken_default.png'
                      "
                      alt="product"
                      class="img-fluid"
                    />
                  </div>
                  <div class="menu_text">
                    <h3>{{ product.tenSanPham }}</h3>
                    <p class="product-description">{{ product.moTa }}</p>
                    <h5>{{ product.khoangGia }}</h5>
                  </div>
                </router-link>
                <router-link style="text-decoration: none;" :to="`/product/` + product.maSp" class="btn-order"><i class="fas fa-eye"></i> Xem chi tiết</router-link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  </div>
</template>

<style scoped>
.food_menu {
  padding: 40px 0;
  background: linear-gradient(135deg, #ffecd2 0%, #fcb69f 100%);
  position: relative;
  overflow: hidden;
}

.search-sort-bar {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 30px;
  flex-wrap: wrap;
  gap: 15px;
}

.search-input {
  flex: 1;
  padding: 10px 15px;
  border: 1px solid #ddd;
  border-radius: 25px;
  font-size: 14px;
  outline: none;
  transition: border-color 0.3s ease;
}

.search-input:focus {
  border-color: #ff8c00;
}

.sort-options {
  display: flex;
  gap: 10px;
}

.sort-options select {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 25px;
  font-size: 14px;
  outline: none;
  cursor: pointer;
  transition: border-color 0.3s ease;
}

.sort-options select:hover,
.sort-options select:focus {
  border-color: #ff8c00;
}

.category-list {
  display: flex;
  justify-content: center;
  flex-wrap: wrap;
  gap: 20px;
  margin-bottom: 40px;
}

.category-item {
  flex: 0 0 auto;
  transition:
    transform 0.3s ease,
    box-shadow 0.3s ease;
}

.category-item:hover {
  transform: scale(1.1);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.2);
}

.category-item button {
  display: flex;
  flex-direction: column;
  align-items: center;
  background: #fff;
  border: none;
  padding: 15px;
  border-radius: 10px;
  font-size: 14px;
  font-weight: 600;
  color: #333;
  text-transform: uppercase;
  transition:
    background-color 0.3s ease,
    color 0.3s ease;
}

.category-item button:hover {
  background-color: #fff3e0;
  color: #ff8c00;
}

.category-item button.active {
  background-color: #fff3e0;
  color: #ff8c00;
}

.category-image {
  margin-bottom: 10px;
  width: 80px;
  height: 80px;
  background: #fff;
  border-radius: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.category-image img {
  width: 60px;
  height: 60px;
  object-fit: contain;
  border-radius: 50%;
}

.menu_item {
  position: relative;
  overflow: hidden;
  border-radius: 15px;
  background: #fff;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
  transition: all 0.3s ease;
}

.menu_item:hover {
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.menu_img {
  width: 100%;
  height: 180px;
  display: flex;
  justify-content: center;
  align-items: center;
  background: #fff;
}

.menu_img img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  transition: transform 0.3s ease;
}

.menu_item:hover .menu_img img {
  transform: scale(1.1);
}

.menu_text {
  padding: 15px;
  text-align: center;
}

.menu_text h3 {
  font-size: 16px;
  font-weight: 700;
  margin-bottom: 8px;
  color: #333;
  text-transform: uppercase;
}

.menu_text h5 {
  font-size: 14px;
  color: #ff8c00;
  margin-bottom: 10px;
  font-weight: 600;
}

.menu_text .original-price {
  text-decoration: line-through;
  color: #999;
  font-size: 12px;
  margin-left: 5px;
}

.btn-order {
  position: absolute;
  bottom: 15px;
  left: 50%;
  transform: translateX(-50%);
  z-index: 1;
  background: #ff8c00;
  color: #fff;
  border: none;
  padding: 8px 20px;
  border-radius: 20px;
  cursor: pointer;
  transition: all 0.3s ease;
  opacity: 0;
}

.menu_item:hover .btn-order {
  opacity: 1;
}

.btn-order:hover {
  background: #e67e22;
  transform: translateX(-50%) translateY(-2px);
}

.combo-description {
  font-size: 12px;
  color: #666;
  margin-bottom: 8px;
  font-style: italic;
}

.product-description {
  font-size: 0.9rem;
  color: #666;
  margin-bottom: 10px;
  height: 40px;
  overflow: hidden;
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
}

.product-link {
  text-decoration: none;
  color: inherit;
  display: block;
  transition: transform 0.3s ease;
}

.product-link:hover {
  transform: translateY(-5px);
}

@media (max-width: 768px) {
  .category-image {
    width: 70px;
    height: 70px;
  }
  .category-image img {
    width: 50px;
    height: 50px;
  }
  .category-item button {
    font-size: 12px;
    padding: 10px;
  }
  .menu_img {
    height: 150px;
  }
  .menu_item {
    max-width: 250px;
  }
  .menu_text h3 {
    font-size: 14px;
  }
  .menu_text h5 {
    font-size: 12px;
  }
  .btn-order {
    padding: 8px 15px;
    font-size: 10px;
  }
}

@media (max-width: 576px) {
  .category-image {
    width: 60px;
    height: 60px;
  }
  .category-image img {
    width: 40px;
    height: 40px;
  }
  .category-item button {
    font-size: 10px;
    padding: 8px;
  }
  .menu_img {
    height: 120px;
  }
  .menu_item {
    max-width: 200px;
  }
  .menu_text h3 {
    font-size: 12px;
  }
  .menu_text h5 {
    font-size: 10px;
  }
  .btn-order {
    padding: 6px 12px;
    font-size: 8px;
  }
}
</style>
