<script setup>
import { ref, onMounted, watch } from 'vue';
import Swal from 'sweetalert2';
import {jwtDecode} from "jwt-decode";
const immutableStatuses = ["Đã hủy", "Hoàn trả/Hoàn tiền"];

const orders = ref([]);
const totalItems = ref(0);
const searchQuery = ref('');
const paymentFilter = ref('');
const statusFilter = ref('');
const currentPage = ref(1);
const pageSize = 10;
const selectedOrder = ref(null);
const isLoading = ref(false);
const userInfo = ref(null);
const statusOptions = ["Chờ xác nhận", "Đã xác nhận", "Đã giao cho đơn vị vận chuyển", "Đã Nhận", "Đã thanh toán", "Đã hủy", "Hoàn trả/Hoàn tiền"];
const paymentOptions = ["COD", "VNPAY"];
const token = localStorage.getItem("accessToken");
try {
    if (token == null){
      throw new Error("Error")
    };
    userInfo.value = jwtDecode(token) // Giải mã Base64
    console.log(userInfo.value);
  } catch (error) {
    console.error("Lỗi khi giải mã token:", error.message);
  }
const fetchOrders = async () => {
  
  isLoading.value = true;
  try {
    const params = new URLSearchParams({
      page: currentPage.value,
      pageSize,
      ...(searchQuery.value && { hoTen: searchQuery.value }),
      ...(paymentFilter.value && { hinhThucTt: paymentFilter.value }),
      ...(statusFilter.value && { tinhTrang: statusFilter.value }),
    });

    const response = await fetch(`https://localhost:7139/api/Bill/GetFiltered/search?${params.toString()}`);
    if (!response.ok) throw new Error("Không thể tải dữ liệu");
    const data = await response.json();

    orders.value = data.orders || [];
    totalItems.value = data.totalItems || 0;
  } catch (error) {
    console.error('Lỗi khi lấy danh sách hóa đơn:', error);
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Đã xảy ra lỗi khi tải dữ liệu!',
    });
  } finally {
    isLoading.value = false;
  }
};
const updateStatus = async (order, newStatus) => {
  const previousStatus = order.tinhTrang;
  try {
    // Kiểm tra xem userInfo.value có tồn tại và có sub không
    if (!userInfo.value || !userInfo.value.sub) {
      throw new Error("Không tìm thấy thông tin nhân viên, vui lòng đăng nhập lại");
    }
    const maNv = parseInt(userInfo.value.sub); // Chuyển sub thành số nguyên (100)
    const response = await fetch(
      `https://localhost:7139/api/Bill/UpdateStatus/update-status/${order.maHd}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json"},
        body: JSON.stringify({ tinhTrang: newStatus, maNv: maNv }),
      }
    );
    if (!response.ok) {
      let errorMessage = "Cập nhật thất bại";
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorMessage;
      } catch {
        errorMessage = response.statusText || "Không thể kết nối tới server";
      }
      throw new Error(errorMessage);
    }

    const result = await response.json();
    order.tinhTrang = newStatus;
    order.maNv = maNv;
    
    // order.maNv = 
    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: result.message,
      showConfirmButton: false,
      timer: 1500,
    });
  } catch (error) {
    console.error("Lỗi khi cập nhật trạng thái:", error);
    order.tinhTrang = previousStatus;
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error.message,
      confirmButtonText: 'OK',
    });
  }
};
// const viewDetails = (order) => {
//   selectedOrder.value = order;
// };
const viewDetails = async (order) => {
  try {
    const response = await fetch(`https://localhost:7139/api/Bill/GetBillDetails/details/${order.maHd}`);
    if (!response.ok) {
      throw new Error(`Lỗi: ${response.status}`);
    }
    const data = await response.json();
    console.log("Dữ liệu từ API:", data); // 🔍 Kiểm tra dữ liệu trả về
    selectedOrder.value = data;
  } catch (error) {
    console.error("Lỗi khi lấy chi tiết đơn hàng:", error);
  }
};

const closeDetails = () => {
  selectedOrder.value = null;
};

const nextPage = () => {
  if (currentPage.value < Math.ceil(totalItems.value / pageSize)) currentPage.value++;
};

const prevPage = () => {
  if (currentPage.value > 1) currentPage.value--;
};

const changePage = (page) => {
  currentPage.value = page;
  fetchOrders();
};

watch([searchQuery, paymentFilter, statusFilter, currentPage], fetchOrders, { debounce: 300 });
onMounted(() => {
  fetchOrders();
  
});
</script>

<template>
  <div class="container mt-4">
    <h2 class="mb-4 text-center">Quản lý đơn hàng</h2>

    <!-- Thanh tìm kiếm và lọc -->
    <div class="row g-3 mb-3 align-items-center">
      <div class="col-md-3">
        <input
          v-model="searchQuery"
          type="text"
          class="form-control shadow-sm border-primary bg-white"
          placeholder="🔍 Nhập tên khách hàng..."
        />
      </div>
      <div class="col-md-3">
        <select v-model="paymentFilter" class="form-select shadow-sm bg-white">
          <option value="">📂 Lọc theo hình thức thanh toán</option>
          <option v-for="option in paymentOptions" :key="option" :value="option">
            {{ option }}
          </option>
        </select>
      </div>
      <div class="col-md-3">
        <select v-model="statusFilter" class="form-select shadow-sm bg-white">
          <option value="">📋 Lọc theo trạng thái</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>
    </div>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-4">Đang tải dữ liệu...</div>

    <!-- Bảng dữ liệu -->
    <div class="table-responsive" v-else>
      <table class="table table-hover table-bordered">
        <thead class="table-dark text-center">
          <tr>
            <th>Mã hóa đơn</th>
            <th>Tên khách hàng</th>
            <th>Hình thức thanh toán</th>
            <th>Trạng thái</th>
            <th>Tổng tiền</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.maHd">
            <td class="text-center">{{ order.maHd }}</td>
            <td class="text-center">{{ order.hoTen }}</td>
            <td class="text-center">{{ order.hinhThucTt }}</td>
            <td class="text-center">
            <select
              class="form-select status-select" :value="order.tinhTrang" @change="updateStatus(order, $event.target.value)" :disabled="immutableStatuses.includes(order.tinhTrang)">
                <option v-for="status in statusOptions" :key="status" :value="status">
                  {{ status }}
                </option>
              </select>
            </td>

            <td class="text-center">{{ order.tongtien.toLocaleString('vi-VN') }} VNĐ</td>
            <td class="text-center">
              <button class="btn btn-info btn-sm" @click="viewDetails(order)">
                ℹ️ Chi tiết
              </button>
            </td>
          </tr>
          <tr v-if="!orders.length">
            <td colspan="6" class="text-center py-4">Không có dữ liệu</td>
          </tr>
        </tbody>
      </table>
    </div>

    <!-- Phân trang -->
    <div class="d-flex justify-content-center mt-4">
      <nav>
        <ul class="pagination">
          <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <a class="page-link" @click="prevPage" href="#">«</a>
          </li>
          <li
            v-for="page in Math.ceil(totalItems / pageSize)"
            :key="page"
            :class="{ active: page === currentPage }"
            class="page-item"
          >
            <a class="page-link" @click="changePage(page)">{{ page }}</a>
          </li>
          <li class="page-item" :class="{ disabled: currentPage >= Math.ceil(totalItems / pageSize) }">
            <a class="page-link" @click="nextPage" href="#">»</a>
          </li>
        </ul>
      </nav>
    </div>

    <!-- Modal chi tiết -->
<!-- Modal chi tiết -->
<!-- Modal chi tiết -->
<transition name="modal">
  <div v-if="selectedOrder" class="modal-overlay" @click.self="closeDetails">
    <div class="modal-content modal-wide"> 
      <!-- Nút X đóng form -->
      <button class="close-btn" @click="closeDetails">×</button>
      
      <h3>Chi tiết hóa đơn có mã hóa đơn: {{ selectedOrder.maHd }}</h3>

      <!-- Grid 3 cột chứa thông tin đơn hàng -->
      <div class="modal-grid">
        <!-- Cột 1 -->
        <div class="modal-column">
          <div class="modal-item">
            <label>Tên khách hàng</label>
            <div class="value">{{ selectedOrder.hoTen }}</div>
          </div>
          <div class="modal-item">
            <label>Số điện thoại</label>
            <div class="value">{{ selectedOrder.sdt }}</div>
          </div>
          <div class="modal-item">
            <label>Địa chỉ nhận hàng</label>
            <div class="value">{{ selectedOrder.diaChiNhanHang }}</div>
          </div>
          <div class="modal-item">
            <label>Hình thức thanh toán</label>
            <div class="value">{{ selectedOrder.hinhThucTt }}</div>
          </div>
          <div class="modal-item">
            <label>Tình trạng</label>
            <div class="value">{{ selectedOrder.tinhTrang }}</div>
          </div>
          <div class="modal-item">
            <label>Phí vận chuyển</label>
            <div class="value">{{ selectedOrder.phiVanChuyen?.toLocaleString('vi-VN') || "0" }} VNĐ</div>
          </div>
          <div class="modal-item">
            <label>Tiền gốc</label>
            <div class="value">{{ selectedOrder.tienGoc?.toLocaleString('vi-VN') || "0" }} VNĐ</div>
          </div>
        </div>

        <!-- Cột 3 -->
        <div class="modal-column">
          <div class="modal-item">
            <label>Tên nhân viên</label>
            <div class="value">{{ selectedOrder.hoTenNv }}</div>
          </div>
          <div class="modal-item">
            <label>Ngày tạo</label>
            <div class="value">{{ new Date(selectedOrder.ngayTao).toLocaleString('vi-VN') }}</div>
          </div>
          <div class="modal-item">
            <label>Ngày bắt đầu giao</label>
            <div class="value">{{ new Date(selectedOrder.batDauGiao).toLocaleString('vi-VN') }}</div>
          </div>
          <div class="modal-item">
            <label>Ngày nhận</label>
            <div class="value">{{ new Date(selectedOrder.ngayNhan).toLocaleString('vi-VN') }}</div>
          </div>
          <div class="modal-item">
            <label>Ngày thanh toán</label>
            <div class="value">
              {{ selectedOrder.ngayThanhToan ? new Date(selectedOrder.ngayThanhToan).toLocaleString('vi-VN') : "Chưa thanh toán" }}
            </div>
          </div>
          <div class="modal-item">
            <label>Mô tả</label>
            <div class="value">{{ selectedOrder.moTa || "Không có" }}</div>
          </div>
          <div class="modal-item">
            <label>Tổng tiền</label>
            <div class="value">{{ selectedOrder.tongtien?.toLocaleString('vi-VN') || "0" }} VNĐ</div>
          </div>
        </div>
      </div>


      <!-- Danh sách sản phẩm bên dưới -->
      <div class="product-list">
        <h4>Sản phẩm trong đơn hàng</h4>
        <table class="product-table">
          <thead>
            <tr>
              <th>STT</th>
              <th>Hình ảnh</th>
              <th>Tên sản phẩm</th>
              <th>Kích thước</th>
              <th>Hương vị</th>
              <th>Đơn giá</th>
              <th>Số lượng</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in selectedOrder.chiTietHoaDon" :key="index">
              <td>{{ index + 1 }}</td>
              <td>
                <img v-if="item.hinhAnh" :src="'https://localhost:7139/HinhAnh/Food_Drink/' + item.hinhAnh" alt="Ảnh sản phẩm" width="60">
                <span v-else>Không có ảnh</span>
              </td>
              <td>{{ item.tenSanPham || "Không có tên" }}</td>
              <td>{{ item.kichThuoc || "Không có" }}</td>
              <td>{{ item.huongVi || "Không có" }}</td>
              <td>{{ item.donGia?.toLocaleString('vi-VN') }} VNĐ</td>
              <td>{{ item.soLuong }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>
  </div>
</transition>


  </div>
</template>

<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  overflow: auto; /* Cho phép cuộn toàn màn hình */
  padding: 20px;
}

.modal-content {
  background: white;
  padding: 20px;
  border-radius: 8px;
  max-width: 1100px;
  width: 90%;
  max-height: 90vh; /* Giới hạn chiều cao */
  overflow-y: auto; /* Cuộn nội dung bên trong modal */
  position: relative;
}

.modal-wide {
  max-width: 1200px;
}

.close-btn {
  position: absolute;
  top: 10px;
  right: 15px;
  background: none;
  border: none;
  font-size: 24px;
  cursor: pointer;
}

.close-btn:hover {
  color: red;
}

.modal-grid {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  gap: 16px; 
}

.modal-column {
  flex: 1;
}

.modal-item {
  margin-bottom: 10px;
}

.modal-item label {
  font-weight: bold;
}

.value {
  padding: 5px;
  background: #f9f9f9;
  border-radius: 5px;
}

.product-list {
  margin-top: 20px;
}

.product-table {
  width: 100%;
  border-collapse: collapse;
}

.product-table th, .product-table td {
  border: 1px solid #ddd;
  padding: 10px;
  text-align: center;
}

.product-table th {
  background: #f0f0f0;
}

.container {
  max-width: 1200px;
  margin: 20px auto;
  padding: 20px;
  font-family: 'Segoe UI', sans-serif;
}

h2 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 30px;
  font-size: 28px;
  font-weight: 600;
}

.row {
  display: flex;
  flex-wrap: wrap;
  gap: 15px;
}

.form-control,
.form-select,
.status-select {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 8px;
  font-size: 14px;
  transition: border-color 0.3s, box-shadow 0.3s;
}

.form-control:focus,
.form-select:focus,
.status-select:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 5px rgba(52, 152, 219, 0.3);
}

.bg-white {
  background: white;
}

.shadow-sm {
  box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
}

.border-primary {
  border-color: #3498db !important;
}

.table-responsive {
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.table {
  width: 100%;
  border-collapse: collapse;
}

.table th,
.table td {
  padding: 15px;
}

.table th {
  background: #343a40;
  color: white;
  font-weight: 600;
}

.table tr:nth-child(even) {
  background: #f9f9f9;
}

.table tr:hover {
  background: #eef2f7;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.3s;
}

.btn-info {
  background: #17a2b8;
  color: white;
}

.btn-info:hover {
  background: #138496;
}

.btn-close {
  background: #e74c3c;
  color: white;
  width: 100%;
  padding: 12px;
  margin-top: 25px;
  border-radius: 8px;
  font-size: 16px;
  font-weight: 500;
}

.btn-close:hover {
  background: #c0392b;
}

.pagination {
  margin-top: 20px;
}

.page-item.disabled .page-link {
  background: #bdc3c7;
  cursor: not-allowed;
}

.page-link {
  color: #3498db;
  padding: 8px 16px;
  border-radius: 5px;
  cursor: pointer;
}

.page-item.active .page-link {
  background: #3498db;
  color: white;
}

.page-link:hover:not(.disabled) {
  background: #2980b9;
  color: white;
}

.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.7);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: #fff;
  padding: 30px;
  border-radius: 12px;
  max-width: 800px;
  width: 90%;
  box-shadow: 0 15px 40px rgba(0, 0, 0, 0.25);
}

.modal-content h3 {
  color: #2c3e50;
  font-size: 24px;
  font-weight: 600;
  margin-bottom: 25px;
  text-align: center;
  border-bottom: 2px solid #3498db;
  padding-bottom: 10px;
}

.modal-grid {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 20px;
}

.modal-column {
  display: flex;
  flex-direction: column;
  gap: 15px;
}

.modal-item {
  display: flex;
  flex-direction: column;
}

.modal-item label {
  font-weight: 1000px;
  color: #2c3e50;
  font-size: 14px;
  margin-bottom: 5px;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.modal-item .value {
  background: #f5f6fa;
  padding: 10px;
  border-radius: 6px;
  color: #34495e;
  font-size: 15px;
  word-wrap: break-word;
  border: 1px solid #dfe6e9;
}

.modal-enter-active,
.modal-leave-active {
  transition: opacity 0.3s;
}

.modal-enter-from,
.modal-leave-to {
  opacity: 0;
}

.modal-content {
  transition: transform 0.3s;
}

.modal-enter-from .modal-content,
.modal-leave-to .modal-content {
  transform: scale(0.95);
}

@media (max-width: 600px) {
  .modal-grid {
    grid-template-columns: 1fr;
  }
  .modal-content {
    padding: 20px;
    max-width: 90%;
  }
}
</style>