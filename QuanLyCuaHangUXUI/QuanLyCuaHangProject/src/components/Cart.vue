<template>
    <div class="cart-modal" v-if="isCartOpen">
      <div class="cart-overlay" @click="closeCartModal"></div>
      <div class="cart-content">
        <div class="cart-header">
          <h3 style="font-family:'Times New Roman', Times, serif;">Giỏ hàng của bạn</h3>
          <button class="close-btn" @click="closeCartModal">×</button>
        </div>
        <div v-if="!isLoggedIn" class="not-logged-in">
          <p>Vui lòng đăng nhập để xem giỏ hàng!</p>
          <button class="btn btn-primary" @click="redirectToLogin">Đăng nhập</button>
        </div>
        <div v-else-if="isLoading" class="loading">
          <p>Đang tải giỏ hàng...</p>
        </div>
        <div v-else-if="errorMessage" class="error-message">
          <p>{{ errorMessage }}</p>
        </div>
        <div v-else-if="cartItems.length > 0" class="cart-body">
          <div class="cart-item" v-for="item in cartItems" :key="item.maCombo ? `combo-${item.maKh}-${item.maCombo}` : `product-${item.maKh}-${item.maCtsp}`">
            <div class="cart-item-image">
                <img
  :src="item.hinhAnhUrls && item.hinhAnhUrls.length > 0 && item.hinhAnhUrls[0] ? `${imageBaseUrl}/${item.hinhAnhUrls[0]}` : '../assets/client/img/food_menu/chicken_default.png'"
  alt="cart-item"
  class="item-image"
/>
            </div>
            <div class="cart-item-details">
              <h5 class="item-name">{{ item.tenSanPham }}</h5>
              <p class="item-price">{{ item.donGia.toLocaleString('vi-VN') }} đ</p>
              <div class="quantity-control">
  <button @click="updateQuantity(item.maKh, item.maCtsp, item.maCombo, item.soLuong - 1)" :disabled="item.soLuong <= 1">-</button>
  <span>{{ item.soLuong }}</span>
  <button @click="updateQuantity(item.maKh, item.maCtsp, item.maCombo, item.soLuong + 1)">+</button>
</div>
            </div>
            <button class="remove-btn" @click="removeItem(item.maKh, item.maCtsp, item.maCombo)">Xóa</button>
          </div>
          <div class="cart-footer">
            <p class="total-price">Tổng tiền: {{ totalPrice.toLocaleString('vi-VN') }} đ</p>
            <button class="btn btn-checkout" @click="proceedToCheckout">Thanh toán</button>
          </div>
        </div>
        <div v-else class="empty-cart">
          <p>Giỏ hàng của bạn đang trống!</p>
          <button class="btn btn-primary" @click="closeCartModal">Tiếp tục mua sắm</button>
        </div>
      </div>
    </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import axios from 'axios';
import Swal from 'sweetalert2';
import { jwtDecode } from 'jwt-decode';

const isCartOpen = ref(false);
const isLoggedIn = ref(false);
const maKh = ref(null);
const cartItems = ref([]);
const isLoading = ref(false);
const errorMessage = ref('');
const router = useRouter();

const apiBaseUrl = 'https://localhost:7139/api/Cart';
const imageBaseUrl = 'https://localhost:7139/HinhAnh/Food_Drink';

const checkLoginAndFetchUserInfo = async () => {
    const token = localStorage.getItem('accessToken');
    if (!token) {
        isLoggedIn.value = false;
        return false;
    }
    try {
        const decodedToken = jwtDecode(token);
        maKh.value = parseInt(decodedToken.sub);
        isLoggedIn.value = true;
        return true;
    } catch (error) {
        console.error('Lỗi khi giải mã token:', error);
        isLoggedIn.value = false;
        return false;
    }
};

const fetchCartItems = async () => {
    try {
        isLoading.value = true;
        errorMessage.value = '';
        const token = localStorage.getItem('accessToken');
        if (!token || !maKh.value) throw new Error('Không thể lấy giỏ hàng. Vui lòng đăng nhập lại.');
        const response = await axios.get(`${apiBaseUrl}/${maKh.value}`, {
            headers: {
                Authorization: `Bearer ${token}`,
            },
        });
        cartItems.value = response.data;
    } catch (error) {
        console.error('Lỗi khi lấy giỏ hàng:', error);
        if (error.response?.status === 401) {
            errorMessage.value = 'Phiên đăng nhập hết hạn. Vui lòng đăng nhập lại.';
            redirectToLogin();
        } else {
            errorMessage.value = error.response?.data?.message || 'Không thể tải giỏ hàng. Vui lòng thử lại.';
        }
    } finally {
        isLoading.value = false;
    }
};

const openCartModal = async () => {
    const loggedIn = await checkLoginAndFetchUserInfo();
    if (loggedIn) await fetchCartItems();
    isCartOpen.value = true;
};

const closeCartModal = () => {
    isCartOpen.value = false;
    errorMessage.value = '';
};

const redirectToLogin = () => {
    closeCartModal();
    router.push('/login');
};

const updateQuantity = async (maKh, maCtsp, maCombo, newQuantity) => {
    if (newQuantity < 1) return;
    try {
        // Tải lại giỏ hàng để đảm bảo dữ liệu đồng bộ
        await fetchCartItems();

        const token = localStorage.getItem('accessToken');
        const item = cartItems.value.find(item => (maCtsp && item.maCtsp === maCtsp) || (maCombo && item.maCombo === maCombo));
        if (!item) throw new Error('Sản phẩm hoặc combo không tồn tại trong giỏ hàng.');

        const payload = {
            maKh,
            maCtsp,
            maCombo,
            tenCombo: item.tenCombo || null,
            soLuong: newQuantity,
            donGia: item.donGia,
            tenSanPham: item.tenSanPham || null,
            hoTenKhachHang: item.hoTenKhachHang || "",
            soLuongTon: item.soLuongTon || 0,
            hinhAnhUrls: item.hinhAnhUrls || [],
            kichThuoc: item.kichThuoc || "NO",
            huongVi: item.huongVi || "NO"
        };
        await axios.put(`${apiBaseUrl}/update`, payload, {
            headers: {
                Authorization: `Bearer ${token}`,
                'Content-Type': 'application/json',
            },
        });
        await fetchCartItems();
    } catch (error) {
        console.error('Lỗi khi cập nhật số lượng:', error);
        Swal.fire({
            icon: 'error',
            title: 'Lỗi',
            text: error.response?.data?.message || error.message || 'Không thể cập nhật số lượng. Vui lòng thử lại.',
            confirmButtonText: 'OK',
        });
    }
};

const removeItem = async (maKh, maCtsp, maCombo) => {
    const result = await Swal.fire({
        title: 'Xác nhận xóa',
        text: "Bạn có chắc chắn muốn xóa sản phẩm này khỏi giỏ hàng không?",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#ff6f61',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Có, xóa!',
        cancelButtonText: 'Hủy'
    });

    if (result.isConfirmed) {
        try {
            const token = localStorage.getItem('accessToken');
            if (maCombo) {
                await axios.delete(`${apiBaseUrl}/remove-combo/${maKh}/${maCombo}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
            } else if (maCtsp) {
                await axios.delete(`${apiBaseUrl}/remove/${maKh}/${maCtsp}`, {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                });
            }
            await fetchCartItems();
            Swal.fire({
                icon: 'success',
                title: 'Thành công',
                text: 'Đã xóa khỏi giỏ hàng!',
                confirmButtonText: 'OK',
            });
        } catch (error) {
            console.error('Lỗi khi xóa:', error);
            Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: error.response?.data?.message || 'Không thể xóa. Vui lòng thử lại.',
                confirmButtonText: 'OK',
            });
        }
    }
};

const totalPrice = computed(() => {
    return cartItems.value.reduce((total, item) => total + item.donGia * item.soLuong, 0);
});

const proceedToCheckout = () => {
    closeCartModal();
    router.push('/checkout');
};

onMounted(async () => {
    await checkLoginAndFetchUserInfo();
});

defineExpose({
    openCartModal
});
</script>

<style scoped>
.quantity-control {
    display: flex;
    align-items: center;
    justify-content: center; /* Căn giữa các phần tử */
    gap: 15px; /* Tăng khoảng cách giữa các nút */
}

.quantity-control button {
    background-color: #ff6f61;
    color: #fff;
    border: none;
    padding: 8px 12px; /* Tăng kích thước nút */
    font-size: 18px; /* Tăng kích thước chữ */
    font-weight: 600;
    cursor: pointer;
    border-radius: 50%;
    transition: background-color 0.3s ease;
}

.quantity-control button:hover {
    background-color: #e65b50;
}

.quantity-control span {
    font-size: 18px; /* Tăng kích thước chữ */
    font-weight: 600;
    color: #333;
    text-align: center; /* Căn giữa chữ trong span */
    min-width: 40px; /* Đặt chiều rộng tối thiểu để giữ cho nó không bị co lại */
}
.cart-modal {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    z-index: 1050;
    display: flex;
    align-items: center;
    justify-content: center;
}

.cart-overlay {
    position: absolute;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: rgba(0, 0, 0, 0.7); /* Tăng độ tối của overlay */
    backdrop-filter: blur(8px); /* Thêm hiệu ứng mờ cho overlay */
}

.cart-content {
    position: relative;
    background: #fff;
    border-radius: 15px;
    width: 90%; /* Mở rộng modal */
    max-width: 800px; /* Tăng chiều rộng tối đa */
    max-height: 80vh;
    overflow-y: auto;
    box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3); /* Tăng độ bóng */
    padding: 30px; /* Tăng padding */
    transition: transform 0.3s ease; /* Thêm hiệu ứng chuyển động */
}

.cart-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    border-bottom: 2px solid #e0e0e0; /* Tăng độ dày của đường viền */
    padding-bottom: 15px;
    margin-bottom: 20px;
}

.cart-header h3 {
    font-size: 28px; /* Tăng kích thước tiêu đề */
    font-weight: 700;
    color: #2c3e50;
}

.close-btn {
    background: none;
    border: none;
    font-size: 30px;
    color: #666;
    cursor: pointer;
    transition: color 0.3s ease;
}

.close-btn:hover {
    color: #ff4d4f;
}

.not-logged-in {
    text-align: center;
    padding: 20px;
}

.not-logged-in p {
    font-size: 18px;
    color: #ff4d4f;
    margin-bottom: 20px;
}

.loading,
.error-message {
    text-align: center;
    padding: 20px;
}

.loading p,
.error-message p {
    font-size: 18px;
    color: #2c3e50;
}

.error-message p {
    color: #ff4d4f;
}

.cart-body {
    max-height: 50vh;
    overflow-y: auto;
    padding-right: 10px;
}

.cart-item {
    display: flex;
    align-items: center;
    gap: 20px; /* Tăng khoảng cách giữa các phần tử */
    padding: 15px 0;
    border-bottom: 1px solid #e0e0e0;
}

.cart-item-image {
    width: 100px; /* Tăng kích thước hình ảnh */
    height: 100px;
    border-radius: 10px;
    overflow: hidden;
    box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.item-image {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.cart-item-details {
    flex: 1;
}

.item-name {
    font-size: 18px; /* Tăng kích thước tên sản phẩm */
    font-weight: 600;
    color: #2c3e50;
    margin-bottom: 5px;
}

.item-price {
    font-size: 16px; /* Tăng kích thước giá sản phẩm */
    color: #e74c3c;
    font-weight: 700;
    margin-bottom: 10px;
}

.quantity-control {
    display: flex;
    align-items: center;
    gap: 15px; /* Tăng khoảng cách giữa các nút */
}

.quantity-control button {
    background-color: #ff6f61;
    color: #fff;
    border: none;
    padding: 8px 12px; /* Tăng kích thước nút */
    font-size: 18px; /* Tăng kích thước chữ */
    font-weight: 600;
    cursor: pointer;
    border-radius: 50%;
    transition: background-color 0.3s ease;
}

.quantity-control button:hover {
    background-color: #e65b50;
}

.quantity-control span {
    font-size: 18px; /* Tăng kích thước chữ */
    font-weight: 600;
    color: #333;
}

.remove-btn {
    background: none;
    border: none;
    color: #ff4d4f;
    font-size: 16px;
    font-weight: 600;
    cursor: pointer;
    transition: color 0.3s ease;
}

.remove-btn:hover {
    color: #e63946;
}

.cart-footer {
    border-top: 2px solid #e0e0e0; /* Tăng độ dày của đường viền */
    padding-top: 20px;
    margin-top: 20px;
    text-align: right;
}

.total-price {
    font-size: 22px; /* Tăng kích thước tổng tiền */
    font-weight: 700;
    color: #2c3e50;
    margin-bottom: 15px;
}

.btn-checkout {
    background-color: #ff6f61;
    color: #fff;
    border: none;
    padding: 12px 30px; /* Tăng kích thước nút thanh toán */
    font-size: 18px; /* Tăng kích thước chữ */
    font-weight: 600;
    text-transform: uppercase;
    border-radius: 30px;
    cursor: pointer;
    transition: background-color 0.3s ease, transform 0.2s ease;
}

.btn-checkout:hover {
    background-color: #e65b50;
    transform: translateY(-2px);
}

.empty-cart {
    text-align: center;
    padding: 20px;
}

.empty-cart p {
    font-size: 18px;
    color: #666;
    margin-bottom: 20px;
}

.btn-primary {
    background-color: #ff6f61;
    color: #fff;
    border: none;
    padding: 10px 20px;
    font-size: 16px;
    font-weight: 600;
    border-radius: 30px;
    cursor: pointer;
    transition: background-color 0.3s ease;
}

.btn-primary:hover {
    background-color: #e65b50;
}

@media (max-width: 576px) {
    .cart-content {
        width: 90%;
        max-height: 90vh;
    }
    .cart-header h3 {
        font-size: 20px;
    }
    .cart-item-image {
        width: 60px;
        height: 60px;
    }
    .item-name {
        font-size: 14px;
    }
    .item-price {
        font-size: 14px;
    }
    .total-price {
        font-size: 18px;
    }
    .btn-checkout {
        padding: 8px 20px;
        font-size: 14px;
    }
}
</style>
