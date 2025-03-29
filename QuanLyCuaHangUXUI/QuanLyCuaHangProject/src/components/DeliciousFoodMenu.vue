<template>
  <!-- food_menu start -->
  <section class="food_menu">
    <div class="container position-relative">
      <!-- Thanh tìm kiếm và sắp xếp -->
      <div class="search-sort-bar">
        <input
          v-model="searchQuery"
          type="text"
          placeholder="Tìm kiếm theo tên hoặc mô tả..."
          class="search-input"
          @input="filterProducts"
        />
        <div class="sort-options">
          <select v-model="sortOption" @change="sortProducts">
            <option value="default">Sắp xếp mặc định</option>
            <option value="price-asc">Giá: Thấp đến Cao</option>
            <option value="price-desc">Giá: Cao đến Thấp</option>
          </select>
        </div>
      </div>

      <!-- Danh sách danh mục -->
      <div class="category-list">
        <div v-for="category in categories" :key="category.maDanhMuc" class="category-item animate__animated animate__fadeIn">
          <button
            :class="{ active: selectedCategory === category.maDanhMuc }"
            @click="selectCategory(category.maDanhMuc)"
          >
            <div class="category-image">
              <img
                :src="category.hinhDaiDien ? (category.maDanhMuc === -1 ? `${imageCBBaseUrl}/${category.hinhDaiDien}` : `${imageBaseUrl}/${category.hinhDaiDien}`) : '../assets/client/img/food_menu/chicken_default.png'"
                alt="category"
              />
            </div>
            <span>{{ category.tenDanhMuc }}</span>
          </button>
        </div>
      </div>

      <!-- Danh sách sản phẩm theo danh mục -->
      <div class="menu-content">
        <!-- Món ngon phải thử (Combos) -->
        <div v-if="selectedCategory === -1" class="row">
          <div v-if="filteredCombos.length === 0" class="col-12 text-center">
            <p class="text-muted">Không tìm thấy combo nào.</p>
          </div>
          <div v-else v-for="combo in filteredCombos" :key="combo.maCombo" class="col-sm-12 col-md-6 col-lg-4 mb-4 animate__animated animate__fadeInUp">
            <div class="menu_item" @click="goToDetail('combo', combo.maCombo)">
              <div class="menu_img">
                <img
                  :src="combo.hinh ? `${imageCBBaseUrl}/${combo.hinh}` : '../assets/client/img/food_menu/chicken_combo.png'"
                  alt="combo"
                  class="img-fluid"
                />
              </div>
              <div class="menu_text">
                <h3>{{ combo.tenCombo }}</h3>
                <h5>
                  {{ combo.discountedPrice ? `${combo.discountedPrice.toLocaleString('vi-VN')} đ` : 'Giá không khả dụng' }}
                  <span v-if="combo.soTienGiam" class="original-price">{{ combo.totalPrice.toLocaleString('vi-VN') }} đ</span>
                </h5>
                <button class="btn-order" @click="addToCart(combo, $event)">
                  <i class="bi bi-cart"></i> ĐẶT HÀNG
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Món bán chạy (Best Sellers) -->
        <div v-if="selectedCategory === -2" class="row">
          <div v-if="filteredBestSellers.length === 0" class="col-12 text-center">
            <p class="text-muted">Không tìm thấy món bán chạy nào.</p>
          </div>
          <div v-else v-for="bestSeller in filteredBestSellers" :key="bestSeller.maCtsp" class="col-sm-12 col-md-6 col-lg-4 mb-4 animate__animated animate__fadeInUp">
            <div class="menu_item" @click="goToDetail('best-seller', bestSeller.maCtsp)">
              <div class="menu_img">
                <img
                  :src="bestSeller.hinh ? `${imageBaseUrl}/${bestSeller.hinh}` : '../assets/client/img/food_menu/chicken_best_seller.png'"
                  alt="best-seller"
                  class="img-fluid"
                />
              </div>
              <div class="menu_text">
                <h3>{{ bestSeller.tenSanPham }}</h3>
                <h5>{{ bestSeller.donGia ? `${bestSeller.donGia.toLocaleString('vi-VN')} đ` : 'Giá không khả dụng' }}</h5>
                <button class="btn-order" @click="addToCart(bestSeller, $event)">
                  <i class="bi bi-cart"></i> ĐẶT HÀNG
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Sản phẩm theo danh mục -->
        <div v-else class="row">
          <div v-if="filteredProducts.length === 0" class="col-12 text-center">
            <p class="text-muted">Không tìm thấy sản phẩm nào.</p>
          </div>
          <div v-else v-for="product in filteredProducts" :key="product.maSp" class="col-sm-12 col-md-6 col-lg-4 mb-4 animate__animated animate__fadeInUp">
            <div class="menu_item" @click="goToDetail('product', product.maSp)">
              <div class="menu_img">
                <img
                  :src="product.chitietsanphams[0]?.hinhanhs[0]?.tenHinhAnh ? `${imageBaseUrl}/${product.chitietsanphams[0].hinhanhs[0].tenHinhAnh}` : '../assets/client/img/food_menu/chicken_default.png'"
                  alt="product"
                  class="img-fluid"
                />
              </div>
              <div class="menu_text">
                <h3>{{ product.tenSanPham }}</h3>
                <h5>{{ product.khoangGia || 'Giá không khả dụng' }}</h5>
                <button class="btn-order" @click="addToCart(product, $event)">
                  <i class="bi bi-cart"></i> ĐẶT HÀNG
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </section>
  <Cart ref="cartComponent" />
</template>

<script setup>
import { ref, onMounted, computed } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import 'animate.css';
import Cart from './Cart.vue';

const categories = ref([]);
const selectedCategory = ref(null);
const products = ref([]);
const combos = ref([]);
const bestSellers = ref([]);
const searchQuery = ref('');
const sortOption = ref('default');
const cartComponent = ref(null);

const router = useRouter();

const apiBaseUrl = 'https://localhost:7139/api/Home';
const imageBaseUrl = 'https://localhost:7139/HinhAnh/Food_Drink';
const imageCBBaseUrl = 'https://localhost:7139/HinhAnh/Combo';

const fetchData = async () => {
  try {
    const categoryResponse = await axios.get(`${apiBaseUrl}/categories`);
    categories.value = categoryResponse.data;
    selectedCategory.value = categories.value[0]?.maDanhMuc || -1;

    const comboResponse = await axios.get(`${apiBaseUrl}/combos`);
    combos.value = comboResponse.data.map(combo => ({
      ...combo,
      totalPrice: calculateTotalPrice(combo),
      discountedPrice: calculateDiscountedPrice(combo)
    }));

    const bestSellerResponse = await axios.get(`${apiBaseUrl}/best-sellers`);
    bestSellers.value = bestSellerResponse.data;

    fetchAllProducts(); // Gọi lần đầu để tải sản phẩm
  } catch (error) {
    console.error('Error fetching data with Axios:', error);
  }
};

const fetchAllProducts = async () => {
  try {
    const response = await axios.get(`${apiBaseUrl}/all-products`, {
      params: { search: searchQuery.value, sort: sortOption.value === 'default' ? null : sortOption.value.split('-')[1] }
    });
    products.value = response.data.data; // Lấy data từ response phân trang
  } catch (error) {
    console.error('Error fetching all products:', error);
  }
};

const fetchProductsByCategory = async (categoryId) => {
  if (categoryId === -1 || categoryId === -2 || categoryId === -3) return;
  try {
    const response = await axios.get(`${apiBaseUrl}/products-by-category/${categoryId}`);
    products.value = response.data;
  } catch (error) {
    console.error('Error fetching products by category:', error);
  }
};

// Tính tổng giá combo từ chi tiết
const calculateTotalPrice = (combo) => {
  if (!combo.chitietcombos || combo.chitietcombos.length === 0) return 0;
  // Giả định giá được lấy từ API sản phẩm nếu cần, hiện tại trả về 0 vì không có giá trong chitietcombos
  return 0; // Cần gọi API chi tiết sản phẩm để lấy giá nếu muốn chính xác
};

// Tính giá sau giảm
const calculateDiscountedPrice = (combo) => {
  const total = calculateTotalPrice(combo);
  if (combo.soTienGiam) return total - combo.soTienGiam;
  if (combo.phanTramGiam) return total * (1 - combo.phanTramGiam / 100);
  return total;
};

const filterItems = (items, keyName) => {
  let filtered = [...items];
  
  if (searchQuery.value) {
    filtered = filtered.filter(item =>
      (item[keyName]?.toLowerCase() || '').includes(searchQuery.value.toLowerCase()) ||
      (item.moTa?.toLowerCase() || '').includes(searchQuery.value.toLowerCase())
    );
  }

  return filtered;
};

const filteredProducts = computed(() => filterItems(products.value, 'tenSanPham'));
const filteredCombos = computed(() => filterItems(combos.value, 'tenCombo'));
const filteredBestSellers = computed(() => filterItems(bestSellers.value, 'tenSanPham'));

const selectCategory = (categoryId) => {
  selectedCategory.value = categoryId;
  if (categoryId === -3) {
    fetchAllProducts();
  } else if (categoryId !== -1 && categoryId !== -2) {
    fetchProductsByCategory(categoryId);
  }
};

const filterProducts = () => {
  if (selectedCategory.value === -3) {
    fetchAllProducts();
  }
};

const sortProducts = () => {
  if (selectedCategory.value === -3) {
    fetchAllProducts();
  }
  // Sắp xếp client-side cho combos và best-sellers nếu cần
};

const goToDetail = (type, id) => {
  router.push({ name: 'DetailProduct', params: { type, id } });
};

const addToCart = (item, event) => {
  event.stopPropagation();
  cartComponent.value.openCartModal();
  // Logic thêm vào giỏ hàng có thể được thêm ở đây
  console.log('Added to cart:', item);
};

onMounted(fetchData);
</script>

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
  transition: transform 0.3s ease, box-shadow 0.3s ease;
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
  transition: background-color 0.3s ease, color 0.3s ease;
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
  background-color: #fff;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
  max-width: 300px;
  margin: 0 auto;
}

.menu_item:hover {
  transform: translateY(-5px);
  box-shadow: 0 10px 20px rgba(0, 0, 0, 0.15);
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
  background-color: #ff8c00;
  color: #fff;
  border: none;
  padding: 10px 20px;
  font-size: 12px;
  font-weight: 600;
  text-transform: uppercase;
  border-radius: 25px;
  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.3s ease;
}

.btn-order:hover {
  background-color: #e67e22;
  transform: scale(1.05);
}

@media (max-width: 768px) {
  .category-image { width: 70px; height: 70px; }
  .category-image img { width: 50px; height: 50px; }
  .category-item button { font-size: 12px; padding: 10px; }
  .menu_img { height: 150px; }
  .menu_item { max-width: 250px; }
  .menu_text h3 { font-size: 14px; }
  .menu_text h5 { font-size: 12px; }
  .btn-order { padding: 8px 15px; font-size: 10px; }
}

@media (max-width: 576px) {
  .category-image { width: 60px; height: 60px; }
  .category-image img { width: 40px; height: 40px; }
  .category-item button { font-size: 10px; padding: 8px; }
  .menu_img { height: 120px; }
  .menu_item { max-width: 200px; }
  .menu_text h3 { font-size: 12px; }
  .menu_text h5 { font-size: 10px; }
  .btn-order { padding: 6px 12px; font-size: 8px; }
}
</style>