<script setup>
import { ref, onMounted, watch, computed } from 'vue'
import Swal from 'sweetalert2'
import { jwtDecode } from 'jwt-decode'
import Cookies from 'js-cookie'
import { ReadToken, ValidateToken } from '../../../Authentication_Authorization/auth.js'
import { GetApiUrl } from '@constants/api'
let getApiUrl = GetApiUrl()
const immutableStatuses = ['Đã hủy', 'Hoàn trả/Hoàn tiền']

const orders = ref([])
const totalItems = ref(0)
const searchQuery = ref('')
const paymentFilter = ref('')
const statusFilter = ref('')
const currentPage = ref(1)
const pageSize = 10
const selectedOrder = ref(null)
const isLoading = ref(false)
const statusOptions = [
  'Đang xử lý VNPAY',
  'Chờ xác nhận',
  'Đã xác nhận',
  'Đã giao cho đơn vị vận chuyển',
  'Đã nhận',
  'Đã thanh toán',
  'Đã hủy',
  'Hoàn trả/Hoàn tiền',
]
const chitietcombohoadonDTOs = ref([])
const paymentOptions = ['COD', 'VNPAY']
let accesstoken = Cookies.get('accessToken')
const refreshtoken = Cookies.get('refreshToken')
let validateToken = true
const maNV = ref(-1)
const fetchOrders = async () => {
  isLoading.value = true
  validateToken = await ValidateToken(accesstoken, refreshtoken)
  if (validateToken == true) {
    accesstoken = Cookies.get('accessToken')
  }
  var readtoken = ReadToken(accesstoken)
  if (readtoken) {
    maNV.value = readtoken.IdUser
  } else {
    router.push('/Login')
    return
  }
  try {
    const params = new URLSearchParams({
      page: currentPage.value,
      pageSize,
      ...(searchQuery.value && { maHd: searchQuery.value }),
      ...(paymentFilter.value && { hinhThucTt: paymentFilter.value }),
      ...(statusFilter.value && { tinhTrang: statusFilter.value }),
    })

    const response = await fetch(
      getApiUrl+`/api/Bill/GetFiltered/search?${params.toString()}`
    )
    if (!response.ok) throw new Error('Không thể tải dữ liệu')
    const data = await response.json()

    orders.value = data.orders || []
    totalItems.value = data.totalItems || 0
  } catch (error) {
    console.error('Lỗi khi lấy danh sách hóa đơn:', error)
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Đã xảy ra lỗi khi tải dữ liệu!',
    })
  } finally {
    isLoading.value = false
  }
}
const showCancelReasonModal = ref(false)
const cancelReason = ref('')
const selectStatusCancel = ref('')
const cancelModal = () => {
  showCancelReasonModal.value = false
}
const confirmCancel = async () => {
  if (cancelReason.value == '') {
    Swal.fire({
      icon: 'error',
      title: 'Lý do hủy hoặc hoàn trả/hoàn tiền không được để trống!',
      timer: 2000,
      showConfirmButton: false,
    })
    return
  }
  await updateStatus(selectedOrder.value, selectStatusCancel.value, cancelReason.value)
}
const updateStatus = async (order, newStatus, reasonCancel = '') => {
  console.log('Đã rõ')
  const previousStatus = order.tinhTrang
  validateToken = await ValidateToken(accesstoken, refreshtoken)
  if (validateToken == true) {
    accesstoken = Cookies.get('accessToken')
  }
  var readtoken = ReadToken(accesstoken)
  if (readtoken) {
    maNV.value = readtoken.IdUser
  } else {
    router.push('/Login')
    return
  }
  try {
    if (maNV.value != order.maNv && order.maNv != undefined) {
      Swal.fire({
        icon: 'error',
        title: 'Đơn hàng đã có nhân viên tiếp nhận, không thể cập nhật!',
        timer: 2000,
        showConfirmButton: false,
      })
      return
    }
    if (
      (newStatus.toLowerCase() == 'đã hủy' || newStatus.toLowerCase() == 'hoàn trả/hoàn tiền') &&
      reasonCancel == ''
    ) {
      selectStatusCancel.value = newStatus
      selectedOrder.value = order
      showCancelReasonModal.value = true
      return
    }
    const response = await fetch(
      getApiUrl+`/api/Bill/UpdateStatus/update-status/${order.maHd}`,
      {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ tinhTrang: newStatus, maNv: maNV.value, lyDoHuy: reasonCancel }),
      }
    )
    if (!response.ok) {
      let errorMessage = 'Cập nhật thất bại'
      try {
        const errorData = await response.text()
        errorMessage = errorData
      } catch {
        errorMessage = response.statusText || 'Không thể kết nối tới server'
      }
      throw new Error(errorMessage)
    }

    const result = await response.json()
    order.tinhTrang = newStatus
    order.maNv = maNV.value

    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: result.message,
      showConfirmButton: false,
      timer: 1500,
    })
    showCancelReasonModal.value = false
  } catch (error) {
    console.error('Lỗi khi cập nhật trạng thái:', error)
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error,
      confirmButtonText: 'OK',
    })
    order.tinhTrang = previousStatus
    fetchOrders()
  }
}
// const viewDetails = (order) => {
//   selectedOrder.value = order;
// };
const viewDetails = async (order) => {
  try {
    const response = await fetch(
      getApiUrl+`/api/Bill/GetBillDetails/details/${order.maHd}`
    )
    if (!response.ok) {
      var responsetext = await response.text()
      console.log(order.maHd)
      throw new Error(`Lỗi: ${responsetext}`)
    }
    const data = await response.json()
    console.log('Dữ liệu từ API:', data) // 🔍 Kiểm tra dữ liệu trả về
    selectedOrder.value = data
    chitietcombohoadonDTOs.value = selectedOrder.value.chitietcombohoadonDTOs
  } catch (error) {
    console.error('Lỗi khi lấy chi tiết đơn hàng:', error)
  }
}

const closeDetails = () => {
  selectedOrder.value = null
}

const nextPage = () => {
  if (currentPage.value < Math.ceil(totalItems.value / pageSize)) currentPage.value++
}

const prevPage = () => {
  if (currentPage.value > 1) currentPage.value--
}

const changePage = (page) => {
  currentPage.value = page
  fetchOrders()
}

watch([searchQuery, paymentFilter, statusFilter, currentPage], fetchOrders, { debounce: 300 })
onMounted(() => {
  fetchOrders()
})

const filteredStatusOptions = computed(() => {
  return (tinhTrang) => {
    if (tinhTrang?.toLowerCase() === 'chờ xác nhận') {
      return statusOptions.filter((status) => !['đang xử lý vnpay'].includes(status.toLowerCase()))
    }
    if (tinhTrang?.toLowerCase() === 'đã xác nhận') {
      return statusOptions.filter(
        (status) =>
          !['chờ xác nhận', 'đang xử lý vnpay', 'hoàn trả/hoàn tiền'].includes(status.toLowerCase())
      )
    }
    if (tinhTrang?.toLowerCase() === 'đã giao cho đơn vị vận chuyển') {
      return statusOptions.filter(
        (status) =>
          !['chờ xác nhận', 'đã xác nhận', 'đang xử lý vnpay', 'hoàn trả/hoàn tiền'].includes(
            status.toLowerCase()
          )
      )
    }
    if (tinhTrang?.toLowerCase() === 'đã nhận') {
      return statusOptions.filter(
        (status) =>
          ![
            'chờ xác nhận',
            'đã xác nhận',
            'đã giao cho đơn vị vận chuyển',
            'đang xử lý vnpay',
            'hoàn trả/hoàn tiền',
          ].includes(status.toLowerCase())
      )
    }
    if (tinhTrang?.toLowerCase() === 'đã thanh toán') {
      return statusOptions.filter(
        (status) =>
          ![
            'chờ xác nhận',
            'đã xác nhận',
            'đã giao cho đơn vị vận chuyển',
            'đã nhận',
            'đang xử lý vnpay',
            'đã hủy',
          ].includes(status.toLowerCase())
      )
    }
    if (tinhTrang?.toLowerCase() === 'đã hủy') {
      return ['Đã hủy']
    }
    if (tinhTrang?.toLowerCase() === 'hoàn trả/hoàn tiền') {
      return ['Hoàn trả/Hoàn tiền']
    }
    return statusOptions
  }
})
</script>

<template>
  <div class="container mt-4">
    <div v-if="showCancelReasonModal" class="modal-overlay">
      <div class="modal-content">
        <h5>Nhập lý do hủy/hoàn trả đơn hàng</h5>
        <textarea
          v-model="cancelReason"
          class="form-control"
          rows="3"
          placeholder="Lý do hủy..."
        ></textarea>
        <div class="text-end mt-3">
          <button class="btn btn-secondary me-2" @click="cancelModal">Hủy</button>
          <button class="btn btn-danger" @click="confirmCancel">Xác nhận</button>
        </div>
      </div>
    </div>

    <!-- Thanh tìm kiếm và lọc -->
    <div style="margin-top: 25px" class="row g-3 mb-3 align-items-center">
      <div class="col-md-3">
        <input
          v-model="searchQuery"
          type="text"
          class="form-control shadow-sm border-primary bg-white"
          placeholder="🔍 Nhập mã đơn hàng..."
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
            <th>Khách hàng</th>
            <th>Hình thức thanh toán</th>
            <th>Trạng thái</th>
            <th>Tổng tiền</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="order in orders" :key="order.maHd">
            <td class="text-center">{{ order.maHd }}</td>
            <td v-if="order.maKh" class="text-center">{{ order.hoTenNguoiDat }} (id: {{ order.maKh }})</td>
            <td v-else class="text-center">Khách tại quầy</td>
            <td class="text-center">{{ order.hinhThucTt }}</td>
            <td class="text-center">
              <select
                class="form-select status-select"
                :value="order.tinhTrang"
                @change="updateStatus(order, $event.target.value)"
                :disabled="immutableStatuses.includes(order.tinhTrang)"
              >
                <option
                  v-for="status in filteredStatusOptions(order.tinhTrang)"
                  :key="status"
                  :value="status"
                >
                  {{ status }}
                </option>
              </select>
            </td>

            <td class="text-center">{{ order.tongtien.toLocaleString('vi-VN') }} VNĐ</td>
            <td class="text-center">
              <button class="btn btn-info btn-sm" @click="viewDetails(order)">ℹ️ Chi tiết</button>
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
          <li
            class="page-item"
            :class="{ disabled: currentPage >= Math.ceil(totalItems / pageSize) }"
          >
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
                <label>Tên người nhận</label>
                <div class="value">{{ selectedOrder.hoTenNguoiNhan }}</div>
              </div>
              <div class="modal-item">
                <label>Tên người đặt</label>
                <div class="value">
                  {{ selectedOrder.hoTenNguoiDat }} (id: {{ selectedOrder.maKh }})
                </div>
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
                <div class="value">
                  {{ selectedOrder.phiVanChuyen?.toLocaleString('vi-VN') || '0' }} VNĐ
                </div>
              </div>
              <div class="modal-item">
                <label>Tiền gốc</label>
                <div class="value">
                  {{ selectedOrder.tienGoc?.toLocaleString('vi-VN') || '0' }} VNĐ
                </div>
              </div>
              <div class="modal-item">
                <label>Giảm giá coupon</label>
                <div style="color: red" class="value">
                  - {{ selectedOrder.giamGiaCoupon?.toLocaleString('vi-VN') || '0' }} VNĐ
                </div>
              </div>
            </div>

            <!-- Cột 3 -->
            <div class="modal-column">
              <div class="modal-item">
                <label>Tên nhân viên</label>
                <div class="value">{{ selectedOrder.hoTenNv }} (ID: {{ selectedOrder.maNv }})</div>
              </div>
              <div class="modal-item">
                <label>Ngày tạo</label>
                <div class="value">
                  {{ new Date(selectedOrder.ngayTao).toLocaleString('vi-VN') }}
                </div>
              </div>
              <div class="modal-item">
                <label>Ngày bắt đầu giao</label>
                <div class="value">
                  {{ new Date(selectedOrder.batDauGiao).toLocaleString('vi-VN') }}
                </div>
              </div>
              <div class="modal-item">
                <label>Ngày nhận</label>
                <div class="value">
                  {{ new Date(selectedOrder.ngayNhan).toLocaleString('vi-VN') }}
                </div>
              </div>
              <div class="modal-item">
                <label>Ngày thanh toán</label>
                <div class="value">
                  {{
                    selectedOrder.ngayThanhToan
                      ? new Date(selectedOrder.ngayThanhToan).toLocaleString('vi-VN')
                      : 'Chưa thanh toán'
                  }}
                </div>
              </div>
              <div class="modal-item">
                <label>Mô tả</label>
                <div class="value">{{ selectedOrder.moTa || 'Không có' }}</div>
              </div>
              <div class="modal-item">
                <label>Lý do hủy/hoàn trả</label>
                <div class="value">{{ selectedOrder.lyDoHuy || 'Không có' }}</div>
              </div>
              <div class="modal-item">
                <label>Tổng tiền</label>
                <div class="value">
                  {{ selectedOrder.tongtien?.toLocaleString('vi-VN') || '0' }} VNĐ
                </div>
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
                  <th>Tiền gốc</th>
                  <th>Giảm giá</th>
                  <th>Tổng tiền</th>
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(item, index) in selectedOrder.chiTietHoaDon.filter(
                    (p) => p.maCombo == null
                  )"
                  :key="index"
                >
                  <td>{{ index + 1 }}</td>
                  <td>
                    <img
                      v-if="item.hinhAnh"
                      :src="getApiUrl+'/HinhAnh/Food_Drink/' + item.hinhAnh"
                      alt="Ảnh sản phẩm"
                      width="60"
                    />
                    <span v-else>Không có ảnh</span>
                  </td>
                  <td>{{ item.tenSanPham || 'Không có tên' }}</td>
                  <td>{{ item.kichThuoc || 'Không có' }}</td>
                  <td>{{ item.huongVi || 'Không có' }}</td>
                  <td>{{ item.donGia?.toLocaleString('vi-VN') }} VNĐ</td>
                  <td>{{ item.soLuong }}</td>
                  <td>{{ item.tienGoc }}</td>
                  <td>
                    <span style="color: red"
                      >- {{ item.giamGia != null && item.giamGia > 0 ? item.giamGia : 0 }} VNĐ</span
                    >
                  </td>
                  <td>{{ item.tienGoc - item.giamGia }}</td>
                </tr>
              </tbody>
            </table>
          </div>

          <!-- Danh sách combo -->
          <div class="combo-list">
            <h4>Combo trong đơn hàng</h4>
            <table class="combo-table">
              <thead>
                <tr>
                  <th>STT</th>
                  <th>Tên combo</th>
                  <th>Số lượng</th>
                  <th>Đơn giá</th>
                  <th>Giá gốc</th>
                  <th>Giảm giá</th>
                  <th>Tổng giá</th>
                  <th>Chi tiết sản phẩm</th>
                </tr>
              </thead>
              <tbody>
                <!-- Combo 1 -->
                <tr
                  v-for="(item, index) in selectedOrder.chiTietHoaDon.filter(
                    (p) => p.maCombo != null
                  )"
                  :key="index"
                >
                  <td>{{ index + 1 }}</td>
                  <td>{{ item.tenCombo }}</td>
                  <td>{{ item.soLuong }}</td>
                  <td>{{ item.donGia }} VNĐ</td>
                  <td>{{ item.tienGoc }} VNĐ</td>
                  <td style="color: red">- {{ item.giamGia }} VNĐ</td>
                  <td>{{ item.tienGoc - item.giamGia }} VNĐ</td>
                  <td>
                    <ul class="combo-products">
                      <li v-for="(detail, index) in chitietcombohoadonDTOs" :key="index">
                        <div><strong>Tên SP:</strong> {{ detail.tenSpCombo }}</div>
                        <div v-if="detail.kichThuoc || detail.huongVi">
                          <strong>Biến thể:</strong> <br />
                          <span v-if="detail.kichThuoc">Kích thước: {{ detail.kichThuoc }}</span>
                          <span v-if="detail.kichThuoc && detail.huongVi"> | </span>
                          <span v-if="detail.huongVi">Hương vị: {{ detail.huongVi }}</span>
                        </div>
                        <div><strong>Số lượng:</strong> {{ detail.soLuong }}</div>
                        <div>
                          <strong>Đơn giá:</strong> {{ detail.donGia.toLocaleString() }} VNĐ
                        </div>
                      </li>
                    </ul>
                  </td>
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
.combo-list {
  margin-top: 25px;
}

.combo-list h4 {
  color: black;
  font-size: 1.5rem;
  font-weight: 500;
  margin-bottom: 15px;
  padding-bottom: 5px;
}

.combo-table {
  width: 100%;
  border-collapse: collapse;
  margin-top: 10px;
  background: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
  border-radius: 8px;
  overflow: hidden;
}

.combo-table th,
.combo-table td {
  border: 1px solid #e0e4e8;
  padding: 12px;
  text-align: center;
  font-size: 14px;
}

.combo-table th {
  background: #f0f0f0;
  color: #2c3e50;
  font-weight: bold;
  text-transform: uppercase;
  letter-spacing: 0.5px;
}

.combo-table tr:nth-child(even) {
  background: #fafafa;
}

.combo-products {
  list-style: none;
  padding: 0;
  margin: 0;
  text-align: left;
}

.combo-products li {
  padding: 6px 0;
  font-size: 13px;
  color: #34495e;
  border-bottom: 1px dashed #dfe6e9;
}

.combo-products li:last-child {
  border-bottom: none;
}
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
  overflow: auto;
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

.product-table th,
.product-table td {
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