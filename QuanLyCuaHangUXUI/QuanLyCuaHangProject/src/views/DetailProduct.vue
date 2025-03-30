<template>
  <div>
    <!-- Kế thừa Header -->
    <Header />

    <!-- Nội dung chính -->
    <section class="product-detail" style="margin-top: 150px; margin-bottom: 50px;">
      <div class="container">
        <!-- Hiển thị trạng thái loading -->
        <div v-if="isLoading" class="loading">
          <p>Đang tải dữ liệu...</p>
        </div>

        <!-- Hiển thị thông báo lỗi -->
        <div v-else-if="errorMessage" class="error-message">
          <p>{{ errorMessage }}</p>
        </div>

        <!-- Hiển thị chi tiết sản phẩm khi dữ liệu đã tải -->
        <div v-else class="row">
          <!-- Hình ảnh món ăn -->
          <div class="col-md-6">
            <div class="product-image">
              <img :src="productImage" alt="product" class="img-fluid" />
            </div>

          </div>

          <!-- Thông tin món ăn -->
          <div class="col-md-6">
            <h3 class="product-title">{{ product.tenSanPham || product.tenCombo }}</h3>
            <p class="product-price">{{ productPrice }} đ</p>

            <!-- Tùy chọn nếu là combo -->
            <div v-if="type === 'combo'" class="options">
              <!-- Chọn gà 1 -->
              <div class="option-group">
                <label class="option-label">CHỌN GÀ 1</label>
                <div class="option-list">
                  <label class="option-item" v-for="chicken in chickenOptions" :key="chicken.value">
                    <div class="option-content">
                      <img :src="chicken.image" alt="chicken" class="option-image" />
                      <div class="option-details">
                        <span class="option-name">{{ chicken.name }}</span>
                        <span v-if="chicken.extraPrice" class="extra-price">+{{ chicken.extraPrice }} đ</span>
                      </div>
                    </div>
                    <input type="radio" v-model="selectedChicken1" :value="chicken.value" />
                  </label>
                </div>
              </div>

              <!-- Chọn gà 2 (hiển thị nếu combo có nhiều hơn 1 miếng gà) -->
              <div v-if="product.soLuongMiengGa > 1" class="option-group">
                <label class="option-label">CHỌN GÀ 2</label>
                <div class="option-list">
                  <label class="option-item" v-for="chicken in chickenOptions" :key="chicken.value">
                    <div class="option-content">
                      <img :src="chicken.image" alt="chicken" class="option-image" />
                      <div class="option-details">
                        <span class="option-name">{{ chicken.name }}</span>
                        <span v-if="chicken.extraPrice" class="extra-price">+{{ chicken.extraPrice }} đ</span>
                      </div>
                    </div>
                    <input type="radio" v-model="selectedChicken2" :value="chicken.value" />
                  </label>
                </div>
              </div>

              <!-- Chọn mì -->
              <div class="option-group">
                <label class="option-label">CHỌN MÌ</label>
                <div class="option-list">
                  <label class="option-item" v-for="pasta in pastaOptions" :key="pasta.value">
                    <div class="option-content">
                      <img :src="pasta.image" alt="pasta" class="option-image" />
                      <div class="option-details">
                        <span class="option-name">{{ pasta.name }}</span>
                        <span v-if="pasta.extraPrice" class="extra-price">+{{ pasta.extraPrice }} đ</span>
                      </div>
                    </div>
                    <input type="radio" v-model="selectedPasta" :value="pasta.value" />
                  </label>
                </div>
              </div>

              <!-- Khoai tặng -->
              <div class="option-group">
                <label class="option-label">KHOAI TẶNG</label>
                <div class="option-list">
                  <label class="option-item" v-for="fries in friesOptions" :key="fries.value">
                    <div class="option-content">
                      <img :src="fries.image" alt="fries" class="option-image" />
                      <div class="option-details">
                        <span class="option-name">{{ fries.name }}</span>
                        <span v-if="fries.extraPrice" class="extra-price">+{{ fries.extraPrice }} đ</span>
                      </div>
                    </div>
                    <input type="radio" v-model="selectedFries" :value="fries.value" />
                  </label>
                </div>
              </div>
            </div>

            <!-- Hiển thị biến thể nếu là sản phẩm -->
            <div v-if="type === 'product' && product.chitietsanphams?.length > 0" class="options">
<div class="option-group">
  <label class="option-label">Chọn đồ ăn</label>
  <div class="variant-selection">
    <select v-model="selectedVariant" class="variant-select">
      <option v-for="variant in product.chitietsanphams" :key="variant.maCtsp" :value="variant">
        {{ variant.kichThuoc !== 'NO' ? variant.kichThuoc : '' }}
        {{ variant.huongVi !== 'NO' ? variant.huongVi : '' }}
        ({{ variant.donGia.toLocaleString('vi-VN') }} đ)
      </option>
    </select>
    <!-- Hình ảnh nhỏ của biến thể được chọn -->
    <div v-if="selectedVariant" class="variant-image">
      <img
        :src="selectedVariant?.hinhanhs[0]?.tenHinhAnh
          ? `${imageBaseUrl}/${selectedVariant.hinhanhs[0].tenHinhAnh}`
          : '../assets/client/img/food_menu/chicken_default.png'"
        alt="variant-image"
        class="variant-image-preview"
      />
    </div>
  </div>
</div>
</div>

            <!-- Số lượng và đặt hàng -->
            <div class="order-section">
              <div class="quantity">
                <button @click="decreaseQuantity">-</button>
                <span>{{ quantity }}</span>
                <button @click="increaseQuantity">+</button>
              </div>
              <p class="total-price">{{ totalPrice.toLocaleString('vi-VN') }} đ</p>
              <button class="btn-order" @click="placeOrder" :disabled="isAddingToCart">
                <span v-if="isAddingToCart" class="spinner-border spinner-border-sm" role="status"></span>
                <span v-else>ĐẶT HÀNG</span>
              </button>
            </div>
          </div>
        </div>

        <!-- Sản phẩm liên quan -->
        <!-- <div v-if="!isLoading && !errorMessage" class="related-products mt-5">
          <h3 class="section-title">Sản phẩm liên quan</h3>
          <swiper
            :slides-per-view="3"
            :space-between="20"
            :navigation="true"
            :pagination="{ clickable: true }"
            :modules="[Navigation, Pagination]"
            class="related-products-slider"
          >
            <swiper-slide v-for="relatedProduct in relatedProducts" :key="relatedProduct.maSp">
              <div class="related-product-card">
                <router-link :to="`/detail/product/${relatedProduct.maSp}`">
                  <div class="related-product-image-wrapper">
                    <img
                      :src="relatedProduct.chitietsanphams[0]?.hinhanhs[0]?.tenHinhAnh
                        ? `${imageBaseUrl}/${relatedProduct.chitietsanphams[0].hinhanhs[0].tenHinhAnh}`
                        : '../assets/client/img/food_menu/chicken_default.png'"
                      alt="related-product"
                      class="related-product-image"
                    />
                  </div>
                  <h5 class="related-product-title">{{ relatedProduct.tenSanPham }}</h5>
                  <p class="related-product-price">{{ relatedProduct.khoangGia }}</p>
                </router-link>
              </div>
            </swiper-slide>
          </swiper>
        </div> -->
      </div>
    </section>

    <!-- Kế thừa Footer -->
    <Footer />

    <!-- Tích hợp Cart -->
    <Cart ref="cartComponent" />
  </div>
</template>

<script setup>
import { ref, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import axios from 'axios';
import Header from '../components/Header.vue';
import Footer from '../components/Footer.vue';
import Cart from '../components/Cart.vue';
import { Swiper, SwiperSlide } from 'swiper/vue';
import { Navigation, Pagination } from 'swiper/modules';
import Swal from 'sweetalert2';
import { jwtDecode } from 'jwt-decode';

const route = useRoute();
const router = useRouter();
const type = ref(route.params.type);
const id = ref(route.params.id);

const product = ref({});
const relatedProducts = ref([]);
const quantity = ref(1);
const selectedChicken1 = ref('original');
const selectedChicken2 = ref('original');
const selectedPasta = ref('small');
const selectedFries = ref('small');
const selectedVariant = ref(null);
const isLoading = ref(true);
const errorMessage = ref('');
const cartComponent = ref(null);
const isAddingToCart = ref(false);
const maKh = ref(null);

const apiBaseUrl = 'https://localhost:7139/api/Home';
const imageBaseUrl = 'https://localhost:7139/HinhAnh/Food_Drink';
const imageCBBaseUrl = 'https://localhost:7139/HinhAnh/Combo';

const chickenOptions = ref([
  { value: 'original', name: '1 miếng Gà Giòn Vui Vẻ', extraPrice: 0, image: '../assets/client/img/food_menu/chicken_original.png' },
  { value: 'spicy', name: 'Miếng gà sốt cay (up)', extraPrice: 2000, image: '../assets/client/img/food_menu/chicken_spicy.png' },
]);

const pastaOptions = ref([
  { value: 'small', name: 'Mì Ý Jolly vừa', extraPrice: 0, image: '../assets/client/img/food_menu/pasta_small.png' },
  { value: 'large', name: 'Mì Ý Jolly lớn (up)', extraPrice: 10000, image: '../assets/client/img/food_menu/pasta_large.png' },
]);

const friesOptions = ref([
  { value: 'small', name: 'Khoai tây chiên nhỏ', extraPrice: 0, image: '../assets/client/img/food_menu/fries_small.png' },
  { value: 'large', name: 'Khoai tây chiên lớn', extraPrice: 10000, image: '../assets/client/img/food_menu/fries_large.png' },
]);

const fetchUserInfo = async () => {
  const token = localStorage.getItem('accessToken');
  if (!token) return false;
  const decodedToken = jwtDecode(token);
  maKh.value = parseInt(decodedToken.sub);
  return true;
};

const fetchProductDetail = async () => {
  try {
    isLoading.value = true;
    errorMessage.value = '';
    let response;
    if (type.value === 'combo') {
      response = await axios.get(`${apiBaseUrl}/combos/${id.value}`);
    } else if (type.value === 'best-seller') {
      response = await axios.get(`${apiBaseUrl}/best-sellers/${id.value}`);
    } else if (type.value === 'product') {
      response = await axios.get(`${apiBaseUrl}/products/${id.value}`);
    } else {
      throw new Error('Loại sản phẩm không hợp lệ.');
    }
    if (!response.data) throw new Error('Không tìm thấy dữ liệu sản phẩm.');
    product.value = response.data;
    if (type.value === 'product' && product.value.chitietsanphams?.length > 0) {
      selectedVariant.value = product.value.chitietsanphams[0];
    }
  } catch (error) {
    console.error('Error fetching product detail:', error);
    errorMessage.value = error.response?.data?.message || 'Có lỗi xảy ra khi tải dữ liệu sản phẩm. Vui lòng thử lại sau.';
  } finally {
    isLoading.value = false;
  }
};

const fetchRelatedProducts = async () => {
  try {
    const response = await axios.get(`${apiBaseUrl}/related-products/${type.value}/${id.value}`);
    relatedProducts.value = response.data;
  } catch (error) {
    console.error('Error fetching related products:', error);
  }
};

const productPrice = ref(0);
const totalPrice = ref(0);

const calculatePrice = () => {
  let basePrice;
  if (type.value === 'combo') {
      basePrice = product.value.discountedPrice || product.value.totalPrice || 0;
      if (selectedChicken1.value === 'spicy') basePrice += 2000;
      if (product.value.soLuongMiengGa > 1 && selectedChicken2.value === 'spicy') basePrice += 2000;
      if (selectedPasta.value === 'large') basePrice += 10000;
      if (selectedFries.value === 'large') basePrice += 10000;
  } else if (type.value === 'best-seller') {
      basePrice = product.value.donGia || 0;
  } else {
      basePrice = selectedVariant.value?.donGia || 0;
  }
  productPrice.value = basePrice;
  totalPrice.value = basePrice * quantity.value;
};

const productImage = ref('');
const setProductImage = () => {
  if (type.value === 'combo') {
    productImage.value = product.value.hinh ? `${imageCBBaseUrl}/${product.value.hinh}` : '../assets/client/img/food_menu/chicken_combo.png';
  } else if (type.value === 'best-seller') {
    productImage.value = product.value.hinh ? `${imageBaseUrl}/${product.value.hinh}` : '../assets/client/img/food_menu/chicken_best_seller.png';
  } else {
    productImage.value = selectedVariant.value?.hinhanhs[0]?.tenHinhAnh
      ? `${imageBaseUrl}/${selectedVariant.value.hinhanhs[0].tenHinhAnh}`
      : '../assets/client/img/food_menu/chicken_default.png';
  }
};

const increaseQuantity = () => {
  quantity.value++;
  calculatePrice();
};

const decreaseQuantity = () => {
  if (quantity.value > 1) {
    quantity.value--;
    calculatePrice();
  }
};

const placeOrder = async () => {
  if (!product.value) {
      await Swal.fire({
          icon: 'error',
          title: 'Lỗi',
          text: 'Không thể tải thông tin sản phẩm. Vui lòng thử lại.',
          confirmButtonText: 'OK',
      });
      return;
  }

  try {
      isAddingToCart.value = true;
      const token = localStorage.getItem('accessToken');
      if (!token) {
          await Swal.fire({
              icon: 'warning',
              title: 'Chưa đăng nhập',
              text: 'Vui lòng đăng nhập để thêm vào giỏ hàng!',
              confirmButtonText: 'Đăng nhập',
              showCancelButton: true,
              cancelButtonText: 'Hủy',
          }).then((result) => {
              if (result.isConfirmed) router.push('/login');
          });
          return;
      }
      const isLoggedIn = await fetchUserInfo();
      if (!isLoggedIn || !maKh.value) {
          await Swal.fire({
              icon: 'error',
              title: 'Lỗi',
              text: 'Không thể lấy thông tin khách hàng. Vui lòng đăng nhập lại.',
              confirmButtonText: 'Đăng nhập',
              showCancelButton: true,
              cancelButtonText: 'Hủy',
          }).then((result) => {
              if (result.isConfirmed) router.push('/login');
          });
          return;
      }

      const cartItems = [];
      let tenSanPham;

      if (type.value === 'combo') {
          tenSanPham = product.value?.tenCombo || 'Combo Không Tên';
          // Lấy thông tin combo từ API
          const response = await axios.get(`https://localhost:7139/api/Home/combos/${id.value}`, {
              headers: {
                  Authorization: `Bearer ${token}`,
              },
          });

          cartItems.push({
              maKh: maKh.value,
              maCtsp: null, // Không lưu MaCtsp cho combo
              maCombo: response.data.maCombo || id.value,
              tenCombo: response.data.tenCombo || tenSanPham,
              soLuong: quantity.value,
              donGia: productPrice.value,
              tenSanPham: tenSanPham,
              hoTenKhachHang: "",
              soLuongTon: 0,
              hinhAnhUrls: [],
              kichThuoc: "NO",
              huongVi: "NO"
          });
      } else if (type.value === 'best-seller') {
          tenSanPham = product.value?.tenSanPham;
          if (!product.value || product.value.soLuongTon < quantity.value) {
              await Swal.fire({
                  icon: 'error',
                  title: 'Lỗi',
                  text: `Số lượng trong kho không đủ! Chỉ còn ${product.value?.soLuongTon || 0} sản phẩm.`,
                  confirmButtonText: 'OK',
              });
              return;
          }
          cartItems.push({
              maKh: maKh.value,
              maCtsp: product.value.maCtsp,
              maCombo: null,
              tenCombo: null,
              soLuong: quantity.value,
              donGia: product.value.donGia,
              tenSanPham: tenSanPham,
              hoTenKhachHang: "",
              soLuongTon: 0,
              hinhAnhUrls: [],
              kichThuoc: "",
              huongVi: ""
          });
      } else {
          if (!selectedVariant.value) {
              await Swal.fire({
                  icon: 'error',
                  title: 'Lỗi',
                  text: 'Vui lòng chọn một biến thể trước khi đặt hàng!',
                  confirmButtonText: 'OK',
              });
              return;
          }
          tenSanPham = product.value?.tenSanPham;
          if (selectedVariant.value.soLuongTon < quantity.value) {
              await Swal.fire({
                  icon: 'error',
                  title: 'Lỗi',
                  text: `Số lượng trong kho không đủ! Chỉ còn ${selectedVariant.value.soLuongTon} sản phẩm.`,
                  confirmButtonText: 'OK',
              });
              return;
          }
          cartItems.push({
              maKh: maKh.value,
              maCtsp: selectedVariant.value.maCtsp,
              maCombo: null,
              tenCombo: null,
              soLuong: quantity.value,
              donGia: selectedVariant.value.donGia,
              tenSanPham: tenSanPham,
              hoTenKhachHang: "",
              soLuongTon: 0,
              hinhAnhUrls: [],
              kichThuoc: selectedVariant.value.kichThuoc,
              huongVi: selectedVariant.value.huongVi
          });
      }

      if (cartItems.length === 0) {
          await Swal.fire({
              icon: 'error',
              title: 'Lỗi',
              text: 'Không thể thêm sản phẩm vào giỏ hàng. Thiếu thông tin chi tiết.',
              confirmButtonText: 'OK',
          });
          return;
      }

      // Gửi từng mục vào giỏ hàng
      for (const cartItem of cartItems) {
          console.log('Dữ liệu gửi đi:', cartItem);
          await axios.post('https://localhost:7139/api/Cart/add', cartItem, {
              headers: {
                  Authorization: `Bearer ${token}`,
                  'Content-Type': 'application/json',
              },
          });
      }

      await Swal.fire({
          icon: 'success',
          title: 'Thành công',
          text: 'Thêm vào giỏ hàng thành công!',
          confirmButtonText: 'Xem giỏ hàng',
          showCancelButton: true,
          cancelButtonText: 'Tiếp tục mua sắm',
      }).then((result) => {
          if (result.isConfirmed) {
              if (cartComponent.value) cartComponent.value.openCartModal();
          }
      });
  } catch (error) {
      console.error('Lỗi khi thêm vào giỏ hàng:', error);
      console.log('Chi tiết lỗi từ server:', error.response?.data);
      if (error.response?.status === 401) {
          await Swal.fire({
              icon: 'error',
              title: 'Phiên hết hạn',
              text: 'Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.',
              confirmButtonText: 'Đăng nhập',
              showCancelButton: true,
              cancelButtonText: 'Hủy',
          }).then((result) => {
              if (result.isConfirmed) router.push('/login');
          });
      } else {
          let errorMessage = 'Không thể thêm vào giỏ hàng. Vui lòng thử lại.';
          if (error.response?.data?.message) {
              errorMessage = error.response.data.message;
          }
          await Swal.fire({
              icon: 'error',
              title: 'Lỗi',
              text: errorMessage,
              confirmButtonText: 'OK',
          });
      }
  } finally {
      isAddingToCart.value = false;
  }
};

watch(selectedVariant, () => {
  calculatePrice();
  setProductImage();
});

watch([selectedChicken1, selectedChicken2, selectedPasta, selectedFries], () => {
  calculatePrice();
});

watch(
  () => route.params,
  (newParams) => {
    console.log('Route params changed:', newParams);
    type.value = newParams.type;
    id.value = newParams.id;
    product.value = {};
    relatedProducts.value = [];
    selectedVariant.value = null;
    quantity.value = 1;
    selectedChicken1.value = 'original';
    selectedChicken2.value = 'original';
    selectedPasta.value = 'small';
    selectedFries.value = 'small';
    fetchProductDetail().then(() => {
      if (!errorMessage.value) {
        setProductImage();
        calculatePrice();
        fetchRelatedProducts();
      }
    });
  },
  { deep: true, immediate: true }
);
</script>


<style scoped>
/* Import Swiper styles */
@import 'swiper/css';
@import 'swiper/css/navigation';
@import 'swiper/css/pagination';
.option-group {
  margin-bottom: 20px;
}

.option-label {
  display: block;
  font-size: 16px;
  font-weight: 600;
  color: #fff;
  background-color: #ffcc00; /* Màu vàng giống hình */
  padding: 12px 15px;
  text-transform: uppercase;
  border-radius: 8px;
  margin-bottom: 12px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.variant-selection {
  display: flex;
  align-items: center;
  gap: 15px;
}

.variant-select {
  flex: 1; /* Chiếm toàn bộ không gian còn lại */
  padding: 10px;
  border: 1px solid #e0e0e0;
  border-radius: 8px;
  font-size: 16px;
  color: #333;
  background-color: #fff;
  transition: border-color 0.3s ease;
}

.variant-select:focus {
  border-color: #ff6f61; /* Màu viền khi chọn */
  outline: none; /* Bỏ viền mặc định */
}

.variant-image {
  width: 60px; /* Chiều rộng hình ảnh */
  height: 60px; /* Chiều cao hình ảnh */
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.variant-image:hover {
  transform: scale(1.05); /* Phóng to nhẹ khi hover */
}

.variant-image-preview {
  width: 100%;
  height: 100%;
  object-fit: cover; /* Đảm bảo hình ảnh không bị méo */
  object-position: center; /* Căn giữa hình ảnh */
}
.product-detail {
  padding: 40px 0;
  background-color: #f9f9f9; /* Nền nhẹ để tạo độ tương phản */
}

/* Hình ảnh sản phẩm chính */
.product-image {
  position: relative;
  width: 100%;
  max-width: 500px; /* Giới hạn chiều rộng tối đa */
  margin: 0 auto; /* Căn giữa */
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1); /* Thêm bóng đổ */
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.product-image:hover {
  transform: scale(1.02); /* Phóng to nhẹ khi hover */
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.15); /* Tăng bóng đổ */
}

.product-image img {
  width: 100%;
  height: 400px; /* Chiều cao cố định */
  object-fit: cover; /* Đảm bảo hình ảnh không bị méo */
  object-position: center; /* Căn giữa hình ảnh */
  border-radius: 20px;
}

.deal-text {
  font-size: 14px;
  font-weight: 600;
  color: #ff4d4f;
  margin-top: 15px;
  text-transform: uppercase;
  text-align: center;
  letter-spacing: 1px;
}

/* Tiêu đề và giá sản phẩm */
.product-title {
  font-size: 28px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 15px;
  line-height: 1.3;
}

.product-price {
  font-size: 24px;
  color: #e74c3c;
  font-weight: 700;
  margin-bottom: 25px;
  letter-spacing: 1px;
}

/* Tùy chọn (combo, biến thể) */
.options {
  margin-bottom: 30px;
}

.option-group {
  margin-bottom: 20px;
}

.option-label {
  display: block;
  font-size: 16px;
  font-weight: 600;
  color: #fff;
  background-color: #ffcc00; /* Màu vàng giống hình */
  padding: 12px 15px;
  text-transform: uppercase;
  border-radius: 8px;
  margin-bottom: 12px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

/* Danh sách tùy chọn */
.option-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.option-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #fff;
  padding: 10px 15px;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
  transition: background-color 0.3s ease;
  cursor: pointer;
}

.option-item:hover {
  background-color: #f5f5f5;
}

.option-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.option-image {
  width: 40px;
  height: 40px;
  object-fit: cover;
  border-radius: 5px;
}

.option-details {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.option-name {
  font-size: 15px;
  color: #333;
}

.extra-price {
  color: #e74c3c;
  font-weight: 600;
  font-size: 14px;
}

.option-item input[type="radio"] {
  accent-color: #ff6f61;
}

/* Variant selection with image preview */
.variant-selection {
  display: flex;
  align-items: center;
  gap: 15px;
}

.variant-selection select {
  flex: 1; /* Chiếm toàn bộ không gian còn lại */
}

.variant-image {
  width: 60px;
  height: 60px;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.variant-image:hover {
  transform: scale(1.05); /* Phóng to nhẹ khi hover */
}

.variant-image-preview {
  width: 100%;
  height: 100%;
  object-fit: cover; /* Đảm bảo hình ảnh không bị méo */
  object-position: center; /* Căn giữa hình ảnh */
}

/* Phần đặt hàng */
.order-section {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-top: 30px;
}

.quantity {
  display: flex;
  align-items: center;
  gap: 12px;
  background-color: #fff;
  padding: 8px 12px;
  border-radius: 30px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.quantity button {
  background-color: #ff6f61;
  color: #fff;
  border: none;
  padding: 8px 14px;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  border-radius: 50%;
  transition: background-color 0.3s ease, transform 0.2s ease;
}

.quantity button:hover {
  background-color: #e65b50;
  transform: scale(1.1);
}

.quantity span {
  font-size: 18px;
  font-weight: 600;
  color: #333;
}

.total-price {
  font-size: 24px;
  font-weight: 700;
  color: #2c3e50;
}

.btn-order {
  background-color: #ff6f61;
  color: #fff;
  border: none;
  padding: 12px 40px;
  font-size: 16px;
  font-weight: 600;
  text-transform: uppercase;
  border-radius: 30px;
  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.2s ease;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}

.btn-order:hover {
  background-color: #e65b50;
  transform: translateY(-2px);
  box-shadow: 0 6px 15px rgba(0, 0, 0, 0.15);
}

.btn-order:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

/* Related Products */
.related-products {
  margin-top: 50px;
}

.section-title {
  font-size: 28px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 30px;
  text-align: center;
  position: relative;
}

.section-title::after {
  content: '';
  width: 60px;
  height: 3px;
  background-color: #ff6f61;
  position: absolute;
  bottom: -10px;
  left: 50%;
  transform: translateX(-50%);
}

.related-products-slider {
  padding-bottom: 50px;
}

.related-product-card {
  text-align: center;
  background-color: #fff;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.related-product-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.related-product-image-wrapper {
  position: relative;
  width: 100%;
  aspect-ratio: 4 / 3; /* Tỷ lệ khung hình 4:3 */
  overflow: hidden;
}

.related-product-image {
  width: 100%;
  height: 100%;
  object-fit: cover; /* Đảm bảo hình ảnh không bị méo */
  object-position: center; /* Căn giữa hình ảnh */
  transition: transform 0.3s ease;
}

.related-product-card:hover .related-product-image {
  transform: scale(1.05); /* Phóng to hình ảnh khi hover */
}

.related-product-title {
  font-size: 16px;
  font-weight: 600;
  color: #2c3e50;
  margin: 12px 0 8px;
  padding: 0 10px;
  line-height: 1.4;
}

.related-product-price {
  font-size: 15px;
  color: #e74c3c;
  font-weight: 700;
  margin-bottom: 12px;
}

/* Swiper Navigation and Pagination */
.swiper-button-next,
.swiper-button-prev {
  color: #ff6f61;
  background-color: #fff;
  border-radius: 50%;
  width: 40px;
  height: 40px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
  transition: background-color 0.3s ease;
}

.swiper-button-next:hover,
.swiper-button-prev:hover {
  background-color: #f5f5f5;
}

.swiper-pagination-bullet {
  background: #ff6f61;
  opacity: 0.5;
}

.swiper-pagination-bullet-active {
  opacity: 1;
}

/* Loading and Error */
.loading {
  text-align: center;
  padding: 30px;
}

.loading p {
  font-size: 20px;
  color: #2c3e50;
  font-weight: 500;
}

.error-message {
  text-align: center;
  padding: 30px;
}

.error-message p {
  font-size: 20px;
  color: #ff4d4f;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 768px) {
  .product-image {
    max-width: 100%;
  }

  .product-image img {
    height: 300px; /* Giảm chiều cao trên mobile */
  }

  .product-title {
    font-size: 22px;
  }

  .product-price {
    font-size: 20px;
  }

  .option-label {
    font-size: 14px;
    padding: 10px;
  }

  .option-name {
    font-size: 14px;
  }

  .extra-price {
    font-size: 13px;
  }

  .total-price {
    font-size: 20px;
  }

  .btn-order {
    padding: 10px 30px;
    font-size: 14px;
  }

  .section-title {
    font-size: 24px;
  }

  .related-products-slider {
    --swiper-slides-per-view: 1; /* Hiển thị 1 slide trên mobile */
  }

  .related-product-image-wrapper {
    aspect-ratio: 4 / 3;
  }

  .variant-selection {
    flex-direction: column;
    align-items: flex-start;
  }

  .variant-image {
    margin-top: 10px;
  }
}

@media (min-width: 769px) and (max-width: 1024px) {
  .product-image img {
    height: 350px;
  }

  .related-products-slider {
    --swiper-slides-per-view: 2; /* Hiển thị 2 slides trên tablet */
  }
}
</style>
<style scoped>
/* Import Swiper styles */
@import 'swiper/css';
@import 'swiper/css/navigation';
@import 'swiper/css/pagination';

.product-detail {
  padding: 40px 20px;
  background-color: #f9f9f9;
}

/* Hình ảnh sản phẩm chính */
.product-image {
  position: relative;
  width: 100%;
  max-width: 500px;
  margin: 0 auto;
  border-radius: 20px;
  overflow: hidden;
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.product-image:hover {
  transform: scale(1.05);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.15);
}

.product-image img {
  width: 100%;
  height: 400px;
  object-fit: cover;
  object-position: center;
  border-radius: 20px;
}

/* Tiêu đề và giá sản phẩm */
.product-title {
  font-size: 28px;
  font-weight: 700;
  color: #2c3e50;
  margin: 15px 0;
  line-height: 1.3;
}

.product-price {
  font-size: 24px;
  color: #e74c3c;
  font-weight: 700;
  margin-bottom: 25px;
}

/* Tùy chọn (combo, biến thể) */
.options {
  margin-bottom: 30px;
}

.option-group {
  margin-bottom: 20px;
}

.option-label {
  display: block;
  font-size: 16px;
  font-weight: 600;
  color: #fff;
  background-color: #ffcc00;
  padding: 12px 15px;
  text-transform: uppercase;
  border-radius: 8px;
  margin-bottom: 12px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

/* Danh sách tùy chọn */
.option-list {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

.option-item {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background-color: #fff;
  padding: 10px 15px;
  border-radius: 8px;
  border: 1px solid #e0e0e0;
  transition: background-color 0.3s ease, transform 0.3s ease;
  cursor: pointer;
}

.option-item:hover {
  background-color: #f5f5f5;
  transform: scale(1.02);
}

.option-content {
  display: flex;
  align-items: center;
  gap: 15px;
}

.option-image {
  width: 40px;
  height: 40px;
  object-fit: cover;
  border-radius: 5px;
}

.option-details {
  display: flex;
  flex-direction: column;
  gap: 5px;
}

.option-name {
  font-size: 15px;
  color: #333;
}

.extra-price {
  color: #e74c3c;
  font-weight: 600;
  font-size: 14px;
}

/* Phần đặt hàng */
.order-section {
  display: flex;
  align-items: center;
  gap: 20px;
  margin-top: 30px;
}

.quantity {
  display: flex;
  align-items: center;
  gap: 12px;
  background-color: #fff;
  padding: 8px 12px;
  border-radius: 30px;
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.quantity button {
  background-color: #ff6f61;
  color: #fff;
  border: none;
  padding: 8px 14px;
  font-size: 18px;
  font-weight: 600;
  cursor: pointer;
  border-radius: 50%;
  transition: background-color 0.3s ease, transform 0.2s ease;
}

.quantity button:hover {
  background-color: #e65b50;
  transform: scale(1.1);
}

.total-price {
  font-size: 24px;
  font-weight: 700;
  color: #2c3e50;
}

.btn-order {
  background-color: #ff6f61;
  color: #fff;
  border: none;
  padding: 12px 40px;
  font-size: 16px;
  font-weight: 600;
  text-transform: uppercase;
  border-radius: 30px;
  cursor: pointer;
  transition: background-color 0.3s ease, transform 0.2s ease;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
}

.btn-order:hover {
  background-color: #e65b50;
  transform: translateY(-2px);
}

.btn-order:disabled {
  background-color: #cccccc;
  cursor: not-allowed;
}

/* Related Products */
.related-products {
  margin-top: 50px;
}

.section-title {
  font-size: 28px;
  font-weight: 700;
  color: #2c3e50;
  margin-bottom: 30px;
  text-align: center;
  position: relative;
}

.section-title::after {
  content: '';
  width: 60px;
  height: 3px;
  background-color: #ff6f61;
  position: absolute;
  bottom: -10px;
  left: 50%;
  transform: translateX(-50%);
}

.related-products-slider {
  padding-bottom: 50px;
}

.related-product-card {
  text-align: center;
  background-color: #fff;
  border-radius: 15px;
  overflow: hidden;
  box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease, box-shadow 0.3s ease;
}

.related-product-card:hover {
  transform: translateY(-8px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
}

.related-product-image-wrapper {
  position: relative;
  width: 100%;
  aspect-ratio: 4 / 3;
  overflow: hidden;
}

.related-product-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
  object-position: center;
  transition: transform 0.3s ease;
}

.related-product-card:hover .related-product-image {
  transform: scale(1.05);
}

.related-product-title {
  font-size: 16px;
  font-weight: 600;
  color: #2c3e50;
  margin: 12px 0 8px;
  padding: 0 10px;
  line-height: 1.4;
}

.related-product-price {
  font-size: 15px;
  color: #e74c3c;
  font-weight: 700;
  margin-bottom: 12px;
}

/* Loading and Error */
.loading {
  text-align: center;
  padding: 30px;
}

.loading p {
  font-size: 20px;
  color: #2c3e50;
  font-weight: 500;
}

.error-message {
  text-align: center;
  padding: 30px;
}

.error-message p {
  font-size: 20px;
  color: #ff4d4f;
  font-weight: 500;
}

/* Responsive */
@media (max-width: 768px) {
  .product-image {
      max-width: 100%;
  }

  .product-image img {
      height: 300px;
  }

  .product-title {
      font-size: 22px;
  }

  .product-price {
      font-size: 20px;
  }

  .option-label {
      font-size: 14px;
      padding: 10px;
  }

  .option-name {
      font-size: 14px;
  }

  .extra-price {
      font-size: 13px;
  }

  .total-price {
      font-size: 20px;
  }

  .btn-order {
      padding: 10px 30px;
      font-size: 14px;
  }

  .section-title {
      font-size: 24px;
  }

  .related-products-slider {
      --swiper-slides-per-view: 1;
  }

  .related-product-image-wrapper {
      aspect-ratio: 4 / 3;
  }

  .variant-selection {
      flex-direction: column;
      align-items: flex-start;
  }

  .variant-image {
      margin-top: 10px;
  }
}

@media (min-width: 769px) and (max-width: 1024px) {
  .product-image img {
      height: 350px;
  }

  .related-products-slider {
      --swiper-slides-per-view: 2;
  }
}
</style>