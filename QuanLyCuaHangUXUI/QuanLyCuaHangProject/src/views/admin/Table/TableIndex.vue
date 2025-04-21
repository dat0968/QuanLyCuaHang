<template>
  <div class="kiot-container">
    <h2 class="mb-4 text-center">Quản lý bàn</h2>

    <!-- Tìm kiếm và lọc -->
    <div class="row g-3 mb-3 justify-content-center">
      <div class="col-md-4">
        <input
          v-model="searchQuery"
          type="text"
          class="form-control shadow-sm"
          placeholder="🔍 Tìm theo id bàn..."
        />
      </div>
      <div class="col-md-4">
        <select v-model="statusFilter" class="form-select shadow-sm">
          <option value="">📋 Tất cả trạng thái</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>
    </div>
    <button class="btn btn-primary btn-sm" data-bs-toggle="modal" data-bs-target="#addTableModal">
      Thêm bàn mới
    </button>
    <br />
    <br />

    <!-- Modal thêm bàn -->
    <div
      class="modal fade"
      id="addTableModal"
      tabindex="-1"
      aria-labelledby="addTableModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="addTableModalLabel">Thêm bàn mới</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <select v-model="newTableStatus" class="form-select shadow-sm">
              <option v-for="status in statusOptions" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </div>
          <div class="modal-footer">
            <button class="btn btn-success" @click="addTable" data-bs-dismiss="modal">Lưu</button>
            <button class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal đặt hàng -->
    <div
      class="modal fade"
      id="orderAtCounterModal"
      tabindex="-1"
      aria-labelledby="orderAtCounterModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="orderAtCounterModalLabel">
              Đặt món - Bàn {{ selectedTable?.id }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
              @click="resetOrder"
            ></button>
          </div>
          <div class="modal-body">
            <div class="menu-order-container">
              <!-- Menu (bên trái) -->
              <div class="menu-left">
                <!-- Bộ lọc và sắp xếp -->
                <div class="menu-controls row mb-3">
                  <div class="col-md-6">
                    <input
                      v-model="menuSearch"
                      type="text"
                      class="form-control"
                      placeholder="🔍 Tìm sản phẩm..."
                    />
                  </div>
                  <div class="col-md-3">
                    <select v-model="categoryFilter" class="form-select">
                      <option value="">Tất cả danh mục</option>
                      <option
                        v-for="category in categories"
                        :key="category.maDanhMuc"
                        :value="category.maDanhMuc"
                      >
                        {{ category.tenDanhMuc }}
                      </option>
                    </select>
                  </div>
                  <div class="col-md-3">
                    <select v-model="sortOption" class="form-select">
                      <option value="">Sắp xếp mặc định</option>
                      <option value="priceAsc">Giá tăng dần</option>
                      <option value="priceDesc">Giá giảm dần</option>
                      <option value="nameAsc">Tên A-Z</option>
                      <option value="nameDesc">Tên Z-A</option>
                    </select>
                  </div>
                </div>

                <!-- Sản phẩm -->
                <div class="menu-section">
                  <h3 v-if="filteredProducts != ''">Sản phẩm</h3>
                  <div class="menu-grid">
                    <div v-for="product in filteredProducts" :key="product.maSp" class="menu-card">
                      <div class="menu-card-image">
                        <img
                          :src="
                            product.chitietsanphams[0]?.hinhanhs?.[0]?.tenHinhAnh
                              ? getApiUrl +
                                `/HinhAnh/Food_Drink/${product.chitietsanphams[0].hinhanhs[0].tenHinhAnh}`
                              : 'https://via.placeholder.com/150'
                          "
                          :alt="product.tenSanPham"
                          class="img-fluid"
                        />
                      </div>
                      <div class="menu-card-content">
                        <h5>{{ product.tenSanPham }}</h5>
                        <p>{{ product.khoangGia }}</p>
                        <div class="variant-selection">
                          <select
                            v-model="selectedVariants[product.maSp]"
                            class="form-select mb-2"
                            @change="updateSelectedVariant(product.maSp, $event.target.value)"
                          >
                            <option
                              v-for="variant in product.chitietsanphams"
                              :key="variant.maCtsp"
                              :value="variant.maCtsp"
                            >
                              {{
                                [
                                  variant.kichThuoc && variant.kichThuoc !== ''
                                    ? variant.kichThuoc
                                    : null,
                                  variant.huongVi && variant.huongVi !== ''
                                    ? variant.huongVi
                                    : null,
                                ]
                                  .filter(Boolean)
                                  .join(' - ') || 'Mặc định'
                              }}
                              ({{ variant.donGia }} VNĐ)
                            </option>
                          </select>
                        </div>
                        <div class="quantity-controls">
                          <button
                            @click="decreaseQuantity(product.maSp)"
                            :disabled="quantities[product.maSp] <= 1"
                          >
                            -
                          </button>
                          <input
                            type="number"
                            v-model="quantities[product.maSp]"
                            @input="handleQuantityChange(product.maSp, $event)"
                            :min="1"
                            :max="getMaxQuantity(product.maSp)"
                            class="quantity-input"
                            :class="{ error: quantityErrors[product.maSp] }"
                          />
                          <button
                            @click="increaseQuantity(product.maSp)"
                            :disabled="quantities[product.maSp] >= getMaxQuantity(product.maSp)"
                          >
                            +
                          </button>
                        </div>
                        <p v-if="quantityErrors[product.maSp]" class="error-message">
                          {{ quantityErrors[product.maSp] }}
                        </p>
                        <button
                          class="btn btn-success btn-sm mt-2"
                          @click="addToOrder('product', product)"
                        >
                          Thêm
                        </button>
                      </div>
                    </div>
                  </div>
                </div>

                <!-- Combo -->
                <div class="menu-section">
                  <h3>Combo</h3>
                  <div class="menu-grid">
                    <div v-for="combo in filteredCombos" :key="combo.maCombo" class="menu-card">
                      <div class="menu-card-image">
                        <img
                          :src="
                            combo.hinh
                              ? getApiUrl + `/HinhAnh/Food_Drink/${combo.hinh}`
                              : 'https://via.placeholder.com/150'
                          "
                          :alt="combo.tenCombo"
                          class="img-fluid"
                        />
                      </div>
                      <div class="menu-card-content">
                        <h5>{{ combo.tenCombo }}</h5>
                        <p>{{ combo.moTa }}</p>
                        <p>
                          <strong>Giá: {{ calculateComboPrice(combo) }} VNĐ</strong>
                        </p>
                        <div class="combo-details">
                          <div
                            v-for="item in combo.chitietcombos"
                            :key="item.maSp"
                            class="combo-item"
                          >
                            <span>{{ item.tenSp }} (x{{ item.soLuongSp }})</span>
                            <select
                              v-model="selectedVariants[item.maSp]"
                              class="form-select mb-2"
                              @change="updateSelectedVariant(item.maSp, $event.target.value)"
                            >
                              <option
                                v-for="variant in item.chitietsanphams"
                                :key="variant.maCtsp"
                                :value="variant.maCtsp"
                              >
                                {{
                                  [
                                    variant.kichThuoc && variant.kichThuoc !== ''
                                      ? variant.kichThuoc
                                      : null,
                                    variant.huongVi && variant.huongVi !== ''
                                      ? variant.huongVi
                                      : null,
                                  ]
                                    .filter(Boolean)
                                    .join(' - ') || 'Mặc định'
                                }}
                                ({{ variant.donGia }} VNĐ)
                              </option>
                            </select>
                          </div>
                        </div>
                        <div class="quantity-controls">
                          <button
                            @click="decreaseQuantity(combo.maCombo)"
                            :disabled="quantities[combo.maCombo] <= 1"
                          >
                            -
                          </button>
                          <input
                            type="number"
                            v-model="quantities[combo.maCombo]"
                            @input="handleQuantityChange(combo.maCombo, $event)"
                            :min="1"
                            :max="combo.soLuong"
                            class="quantity-input"
                            :class="{ error: quantityErrors[combo.maCombo] }"
                          />
                          <button
                            @click="increaseQuantity(combo.maCombo)"
                            :disabled="quantities[combo.maCombo] >= combo.soLuong"
                          >
                            +
                          </button>
                        </div>
                        <p v-if="quantityErrors[combo.maCombo]" class="error-message">
                          {{ quantityErrors[combo.maCombo] }}
                        </p>
                        <button
                          class="btn btn-success btn-sm mt-2"
                          @click="addToOrder('combo', combo)"
                        >
                          Thêm
                        </button>
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Đơn hàng (bên phải) -->
              <!-- Đơn hàng (bên phải) -->
              <div class="order-right">
                <div class="order-summary">
                  <h3>Đơn hàng</h3>
                  <div class="order-items">
                    <div v-for="(item, index) in orderItems" :key="index" class="order-item">
                      <span class="item-name">{{ item.name }}</span>
                      <div class="item-controls">
                        <span class="item-quantity">{{ item.quantity }}</span>
                        <span class="item-price"
                          >{{ item.quantity * item.price }} VNĐ <br />
                          <span
                            v-if="item.maCombo"
                            class="item-price text-danger text-decoration-line-through"
                            >{{ item.original_price }} VNĐ</span
                          >
                        </span>

                        <button class="btn btn-danger btn-sm" @click="removeFromOrder(index)">
                          X
                        </button>
                      </div>
                    </div>
                    <div v-if="!orderItems.length" class="text-center py-3">Chưa có sản phẩm</div>
                  </div>
                  <!-- Ô nhập mã coupon -->
                  <!-- <div class="coupon-section">
                  <div class="coupon-input-group">
                    <input
                      v-model="couponCode"
                      type="text"
                      class="form-control"
                      placeholder="Nhập mã coupon..."
                    />
                    <button
                      class="btn btn-primary"
                      @click="applyCoupon"
                      :disabled="!couponCode || isApplyingCoupon"
                    >
                      {{ isApplyingCoupon ? 'Đang áp dụng...' : 'Áp dụng' }}
                    </button>
                  </div>
                  <p v-if="couponError" class="coupon-error">{{ couponError }}</p>
                  <p v-if="discountAmount > 0" class="coupon-discount">
                    Giảm giá: -{{ discountAmount }} VNĐ
                  </p>
                </div> -->
                  <div class="order-total">
                    <strong>Tổng tiền: {{ finalAmount }} VNĐ</strong>
                  </div>
                  <div class="order-actions">
                    <button
                      class="btn btn-success w-100"
                      @click="checkout"
                      :disabled="!orderItems.length"
                    >
                      Thanh toán
                    </button>
                    <button
                      class="btn btn-secondary w-100 mt-2"
                      data-bs-dismiss="modal"
                      @click="resetOrder"
                    >
                      Hủy
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal hóa đơn -->
    <!-- Modal hóa đơn -->
    <div
      class="modal fade"
      id="billModal"
      tabindex="-1"
      aria-labelledby="billModalLabel"
      aria-hidden="true"
    >
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content bill-modal-content">
          <div class="modal-header bill-modal-header">
            <h5 class="modal-title" id="billModalLabel">Hóa đơn – Mã HD: {{ bill?.maHd }}</h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body bill-modal-body">
            <div class="bill-info">
              <div class="bill-info-row">
                <span class="bill-label"><strong>Mã hóa đơn:</strong></span>
                <span class="bill-value">{{ bill?.maHd || 'N/A' }}</span>
              </div>
              <div class="bill-info-row">
                <span class="bill-label"><strong>Nhân viên:</strong></span>
                <span class="bill-value">{{ bill?.hoTenNv || 'N/A' }}</span>
              </div>
              <div class="bill-info-row">
                <span class="bill-label"><strong>Ngày tạo:</strong></span>
                <span class="bill-value">{{
                  bill?.ngayTao ? new Date(bill.ngayTao).toLocaleString('vi-VN') : 'N/A'
                }}</span>
              </div>
              <div class="bill-info-row">
                <span class="bill-label"><strong>Tổng tiền:</strong></span>
                <span class="bill-value">{{ totalAmount || 0 }} VNĐ</span>
              </div>
            </div>
            <div class="bill-qr-code">
              <canvas id="qrcode" ref="qrCodeCanvas" class="qr-code-canvas"></canvas>
              <p class="qr-code-text">Quét để xem chi tiết hóa đơn</p>
            </div>
          </div>
          <div class="modal-footer bill-modal-footer">
            <button class="btn btn-success bill-modal-close" data-bs-dismiss="modal">Đóng</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Hiển thị bàn dạng card -->
    <div v-if="isLoading" class="text-center py-4">Đang tải dữ liệu...</div>
    <div class="table-grid" v-else>
      <div
        v-for="table in tables"
        :key="table.id"
        :class="getTableClass(table.tinhTrang)"
        class="table-card"
      >
        <div class="card-body">
          <h5 class="card-title">Bàn {{ table.id }} (id: {{ table.id }})</h5>
          <p class="card-text">{{ table.tinhTrang }}</p>
          <div class="card-actions">
            <!-- Nút Đặt hàng: chỉ hiển thị khi trạng thái không phải "Đang sử dụng" -->
            <button
              v-if="table.tinhTrang !== 'Đang sử dụng' && table.tinhTrang !== 'Đang sửa chữa'"
              class="btn btn-success btn-sm"
              @click="openOrderModal(table)"
            >
              Đặt hàng
            </button>
            <!-- Nút Trả bàn: chỉ hiển thị khi trạng thái là "Đang sử dụng" -->
            <button
              v-if="table.tinhTrang === 'Đang sử dụng'"
              class="btn btn-warning btn-sm"
              @click="updateStatus(table, 'Trống')"
            >
              Trả bàn
            </button>
            <!-- Nút Xóa: luôn hiển thị -->
            <button v-if="table.tinhTrang !== 'Đang sử dụng' " class="btn btn-danger btn-sm" @click="deleteTable(table.id)">Xóa</button>
          </div>
        </div>
      </div>
      <div v-if="!tables.length" class="col-12 text-center py-4">Không có dữ liệu</div>
    </div>
    <!-- Template để in hóa đơn -->
    <!-- Template để in hóa đơn -->
    <div id="print-bill" style="display: none">
      <div class="print-bill-content">
        <h2 class="print-bill-header">CỬA HÀNG DARK BEE</h2>
        <p class="print-bill-info">300, 6 đường Hà Huy Tập, BMT, Đắk Lắk</p>
        <p class="print-bill-info">0262 888 4375 - datntpk00396@gmail.com</p>

        <h3 class="print-bill-subheader">HÓA ĐƠN BÁN HÀNG</h3>
        <p class="print-bill-detail"><strong>Mã Hóa Đơn:</strong> {{ bill?.maHd || 'N/A' }}</p>
        <p class="print-bill-detail">
          <strong>Ngày Tạo:</strong>
          {{ bill?.ngayTao ? new Date(bill.ngayTao).toLocaleString('vi-VN') : 'N/A' }}
        </p>

       

        <h4 class="print-bill-section">CHI TIẾT HÓA ĐƠN</h4>
        <table class="print-bill-table">
          <thead>
            <tr>
              <th>STT</th>
              <th>Tên Sản Phẩm</th>
              <th>Số Lượng</th>
              <th>Đơn Giá</th>
              <th>Thành Tiền</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(item, index) in orderItems" :key="index">
              <td>{{ index + 1 }}</td>
              <td>{{ item.name }}</td>
              <td>{{ item.quantity }}</td>
              <td>{{ item.price.toLocaleString('vi-VN') }} ₫</td>
              <td>{{ (item.quantity * item.price).toLocaleString('vi-VN') }} ₫</td>
            </tr>
          </tbody>
        </table>

        <h4 class="print-bill-section">TỔNG CỘNG</h4>
        <p class="print-bill-detail">
          <strong>Hình Thức Thanh Toán:</strong> {{ bill?.hinhThucTt || 'Tại quầy' }}
        </p>
        <p class="print-bill-detail">
          <strong>Tổng Tiền Hàng:</strong> {{ totalAmount.toLocaleString('vi-VN') }} ₫
        </p>
        <p class="print-bill-detail">
          <strong>Phí Vận Chuyển:</strong> {{ bill?.phiVanChuyen || 0 }} ₫
        </p>
        <p class="print-bill-detail">
          <strong>Giảm Giá:</strong> {{ discountAmount.toLocaleString('vi-VN') }} ₫
        </p>
        <p class="print-bill-total">
          <strong>Tổng Tiền Thanh Toán:</strong> {{ finalAmount.toLocaleString('vi-VN') }} ₫
        </p>
        <p class="print-bill-note">(Bằng chữ: {{ numberToWords(finalAmount) }})</p>

        <div class="print-bill-qr">
          <img ref="printQrCodeImg" class="qr-code-img" />
          <p class="print-bill-qr-label">Quét để xem chi tiết hóa đơn</p>
        </div>

        <div class="print-bill-qr">
          <img ref="printHomeQrCodeImg" class="qr-code-img" />
          <p class="print-bill-qr-label">Quét để truy cập trang chủ</p>
        </div>

        <p class="print-bill-footer">Quý khách xem chi tiết hóa đơn</p>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed, nextTick } from 'vue'
import Swal from 'sweetalert2'
import QRCode from 'qrcode'
import { GetApiUrl } from '@constants/api'
import { jwtDecode } from 'jwt-decode'
import Cookies from 'js-cookie'
import { ReadToken, ValidateToken } from '../../../Authentication_Authorization/auth.js'
const immutableStatuses = ['Đang sử dụng', 'Đang sửa chữa']
const tables = ref([])
const searchQuery = ref('')
const statusFilter = ref('')
const isLoading = ref(false)
const newTableStatus = ref('Trống')
const statusOptions = ['Trống', 'Đang sử dụng', 'Đang sửa chữa']
const selectedTable = ref(null)
const products = ref([])
const combos = ref([])
const categories = ref([])
const selectedVariants = ref({})
const quantities = ref({})
const quantityErrors = ref({})
const orderItems = ref([])
const bill = ref(null)
const token = ref('')
const menuSearch = ref('')
const categoryFilter = ref('')
const sortOption = ref('')
const qrCodeCanvas = ref(null)
const maNv = ref(null)
const couponCode = ref('') // Mã coupon người dùng nhập
const discountAmount = ref(0) // Số tiền giảm giá
const couponError = ref('') // Thông báo lỗi khi áp dụng mã coupon
const isApplyingCoupon = ref(false) // Trạng thái đang áp dụng mã coupon
let getApiUrl = GetApiUrl()
const printQrCodeCanvas = ref(null)
const printHomeQrCodeCanvas = ref(null)
const printQrCodeImg = ref(null)
const printHomeQrCodeImg = ref(null)
const generateQRCode = (canvas, data, size = 100) => {
  return new Promise((resolve, reject) => {
    QRCode.toCanvas(canvas, data, { width: size }, (error) => {
      if (error) {
        reject(error)
      } else {
        resolve(canvas.toDataURL('image/png'))
      }
    })
  })
}
// Tổng tiền trước khi giảm giá
const totalAmount = computed(() => {
  return orderItems.value.reduce((sum, item) => sum + item.quantity * item.price, 0)
})

// Tổng tiền sau khi giảm giá
const finalAmount = computed(() => {
  return Math.max(0, totalAmount.value - discountAmount.value)
})

const calculateComboPrice = (combo) => {
  let price = 0
  if (combo.soTienGiam !== null && combo.soTienGiam > 0) {
    price = combo.soTienGiam
  } else {
    price = combo.chitietcombos.reduce((total, ct) => {
      const variant = ct.chitietsanphams.find((v) => v.maCtsp === selectedVariants.value[ct.maSp])
      return total + (variant ? variant.donGia * ct.soLuongSp : 0)
    }, 0)
    if (combo.phanTramGiam !== null && combo.phanTramGiam > 0) {
      price = price * (1 - combo.phanTramGiam / 100)
    }
  }
  return Math.round(price)
}

const getTokenAndDecode = async () => {
  let accesstoken = Cookies.get('accessToken')
  let refreshtoken = Cookies.get('refreshToken')
  const validateToken = await ValidateToken(accesstoken, refreshtoken)
  if(validateToken == true){
    token.value = Cookies.get('accessToken') || ''
    const readtoken = ReadToken(accesstoken)
    if(readtoken){
      maNv.value = readtoken.IdUser
    }
    else {
      router.push('/Login')
      return
    }
  }
}

const filteredProducts = computed(() => {
  let filtered = [...products.value]
  if (menuSearch.value) {
    filtered = filtered.filter((p) =>
      p.tenSanPham.toLowerCase().includes(menuSearch.value.toLowerCase())
    )
  }
  if (categoryFilter.value) {
    filtered = filtered.filter((p) => p.maDanhMuc === parseInt(categoryFilter.value))
  }
  switch (sortOption.value) {
    case 'priceAsc':
      filtered.sort((a, b) => a.chitietsanphams[0].donGia - b.chitietsanphams[0].donGia)
      break
    case 'priceDesc':
      filtered.sort((a, b) => b.chitietsanphams[0].donGia - a.chitietsanphams[0].donGia)
      break
    case 'nameAsc':
      filtered.sort((a, b) => a.tenSanPham.localeCompare(b.tenSanPham))
      break
    case 'nameDesc':
      filtered.sort((a, b) => b.tenSanPham.localeCompare(b.tenSanPham))
      break
  }
  return filtered
})

const filteredCombos = computed(() => {
  let filtered = [...combos.value]
  if (menuSearch.value) {
    filtered = filtered.filter((c) =>
      c.tenCombo.toLowerCase().includes(menuSearch.value.toLowerCase())
    )
  }
  return filtered
})

const fetchTables = async () => {
  isLoading.value = true
  try {
    const url = statusFilter.value
      ? getApiUrl + `/api/Table/filter?tinhTrang=${encodeURIComponent(statusFilter.value)}`
      : getApiUrl + `/api/Table`
    const response = await fetch(url, {
      headers: { Authorization: `Bearer ${token.value}` },
    })
    if (!response.ok) throw new Error(`Không thể tải dữ liệu: ${await response.text()}`)
    const data = await response.json()
    tables.value = Array.isArray(data) ? data : data.items || []
    if (searchQuery.value) {
      const query = parseInt(searchQuery.value, 10)
      if (!isNaN(query)) tables.value = tables.value.filter((table) => table.id === query)
    }
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message })
  } finally {
    isLoading.value = false
  }
}

const fetchCategories = async () => {
  try {
    const response = await fetch(getApiUrl + '/api/Home/categories', {
      headers: { Authorization: `Bearer ${token.value}` },
    })
    if (!response.ok) throw new Error(await response.text())
    categories.value = await response.json()
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tải danh mục: ' + error.message })
  }
}

const updateStatus = async (table, newStatus) => {
  const previousStatus = table.tinhTrang
  try {
    const response = await fetch(getApiUrl + `/api/Table/${table.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token.value}`,
      },
      body: JSON.stringify({ id: table.id, tinhTrang: newStatus }),
    })
    if (!response.ok) throw new Error((await response.text()) || 'Cập nhật thất bại')
    table.tinhTrang = (await response.json()).tinhTrang
    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: 'Cập nhật trạng thái thành công',
      timer: 1500,
    })
  } catch (error) {
    table.tinhTrang = previousStatus
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message })
  }
}

const addTable = async () => {
  try {
    const response = await fetch(getApiUrl + `/api/Table`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token.value}`,
      },
      body: JSON.stringify({ tinhTrang: newTableStatus.value }),
    })
    if (!response.ok) throw new Error(await response.text())
    tables.value.push(await response.json())
    newTableStatus.value = 'Trống'
    Swal.fire({ icon: 'success', title: 'Thành công!', text: 'Thêm bàn thành công', timer: 1500 })
    const modalElement = document.getElementById('addTableModal')
    const modal = window.bootstrap.Modal.getInstance(modalElement)
    if (modal) modal.hide()
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message })
  }
}

const deleteTable = async (tableId) => {
  const confirm = await Swal.fire({
    title: 'Bạn có chắc chắn?',
    text: 'Bạn sẽ không thể khôi phục bàn này!',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Xóa',
    cancelButtonText: 'Hủy',
  })
  if (!confirm.isConfirmed) return
  try {
    const response = await fetch(getApiUrl + `/api/Table/${tableId}`, {
      method: 'DELETE',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token.value}`,
      },
    })
    if (!response.ok) throw new Error(await response.text())
    tables.value = tables.value.filter((table) => table.id !== tableId)
    Swal.fire({ icon: 'success', title: 'Thành công!', text: 'Xóa bàn thành công', timer: 1500 })
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message })
  }
}

const openOrderModal = async (table) => {
  selectedTable.value = table
  try {
    const response = await fetch(getApiUrl + `/api/Table/${table.id}/menu`, {
      headers: { Authorization: `Bearer ${token.value}` },
    })
    if (!response.ok) throw new Error(await response.text())
    const data = await response.json()
    products.value = data.products || []
    combos.value = data.combos || []

    // Khởi tạo số lượng và biến thể mặc định
    selectedVariants.value = {}
    quantities.value = {}

    // Khởi tạo cho sản phẩm
    products.value.forEach((p) => {
      quantities.value[p.maSp] = 1
      if (p.chitietsanphams?.length > 0) {
        selectedVariants.value[p.maSp] = p.chitietsanphams[0].maCtsp
      }
    })

    // Khởi tạo cho combo
    combos.value.forEach((c) => {
      quantities.value[c.maCombo] = 1
      if (c.chitietcombos?.length > 0) {
        c.chitietcombos.forEach((item) => {
          if (item.chitietsanphams?.length > 0) {
            selectedVariants.value[item.maSp] = item.chitietsanphams[0].maCtsp
          } else {
            console.warn(
              `Sản phẩm ${item.tenSp} (maSp: ${item.maSp}) trong combo ${c.tenCombo} không có biến thể.`
            )
          }
        })
      }
    })

    await fetchCategories()
    const modalElement = document.getElementById('orderAtCounterModal')
    const modal = new window.bootstrap.Modal(modalElement)
    modal.show()
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message })
  }
}

const getMaxQuantity = (id) => {
  const product = products.value.find((p) => p.maSp === id)
  if (product) {
    const variant = product.chitietsanphams.find((v) => v.maCtsp === selectedVariants.value[id])
    return variant?.soLuongTon || 0
  }
  const combo = combos.value.find((c) => c.maCombo === id)
  return combo?.soLuong || 0
}

const validateQuantity = (id, value) => {
  if (!value || isNaN(value) || value < 1) {
    quantityErrors.value[id] = 'Số lượng phải là số lớn hơn 0'
    return false
  }
  const max = getMaxQuantity(id)
  if (value > max) {
    quantityErrors.value[id] = `Số lượng tối đa là ${max}`
    return false
  }
  quantityErrors.value[id] = ''
  return true
}

const handleQuantityChange = (id, event) => {
  const value = parseInt(event.target.value) || 1
  if (validateQuantity(id, value)) {
    quantities.value[id] = value
  } else {
    quantities.value[id] = Math.min(Math.max(value, 1), getMaxQuantity(id))
  }
}

const increaseQuantity = (id) => {
  if (quantities.value[id] < getMaxQuantity(id)) {
    quantities.value[id]++
    quantityErrors.value[id] = ''
  }
}

const decreaseQuantity = (id) => {
  if (quantities.value[id] > 1) {
    quantities.value[id]--
    quantityErrors.value[id] = ''
  }
}

const updateSelectedVariant = (id, variantId) => {
  selectedVariants.value[id] = parseInt(variantId)
}

const addToOrder = (type, item) => {
  if (type === 'product') {
    const variant = item.chitietsanphams.find((v) => v.maCtsp === selectedVariants.value[item.maSp])
    if (!variant) {
      Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Vui lòng chọn biến thể sản phẩm.' })
      return
    }
    if (validateQuantity(item.maSp, quantities.value[item.maSp])) {
      const variantName =
        [
          variant.kichThuoc && variant.kichThuoc !== '' ? variant.kichThuoc : null,
          variant.huongVi && variant.huongVi !== '' ? variant.huongVi : null,
        ]
          .filter(Boolean)
          .join(' - ') || ''
      // Kiểm tra donGia
      const price = variant.donGia > 0 ? variant.donGia : null
      if (!price) {
        Swal.fire({
          icon: 'error',
          title: 'Lỗi!',
          text: `Sản phẩm ${item.tenSanPham} (Kích thước: ${variant.kichThuoc}) không có giá hợp lệ. Vui lòng kiểm tra dữ liệu.`,
        })
        return
      }
      let sameItem = false
      orderItems.value.forEach((e) => {
        if (e.maCtsp == variant.maCtsp) {
          e.quantity += quantities.value[item.maSp]
          sameItem = true
        }
      })
      if (sameItem == false) {
        orderItems.value.push({
          name: `${item.tenSanPham}${variantName ? ` (${variantName})` : ''}`,
          quantity: quantities.value[item.maSp],
          price: price,
          maCtsp: variant.maCtsp,
        })
      }

      quantities.value[item.maSp] = 1
    }
  } else if (type === 'combo') {
    if (validateQuantity(item.maCombo, quantities.value[item.maCombo])) {
      const invalidItems = item.chitietcombos.filter(
        (ct) => !ct.chitietsanphams.find((v) => v.maCtsp === selectedVariants.value[ct.maSp])
      )
      if (invalidItems.length > 0) {
        Swal.fire({
          icon: 'error',
          title: 'Lỗi!',
          text: `Vui lòng chọn biến thể cho: ${invalidItems.map((ct) => ct.tenSp).join(', ')}.`,
        })
        return
      }

      const chiTietCombo = item.chitietcombos
        .map((ct) => {
          const variant = ct.chitietsanphams.find(
            (v) => v.maCtsp === selectedVariants.value[ct.maSp]
          )
          const variantName =
            [
              variant.kichThuoc && variant.kichThuoc !== '' ? variant.kichThuoc : null,
              variant.huongVi && variant.huongVi !== '' ? variant.huongVi : null,
            ]
              .filter(Boolean)
              .join(' - ') || ''
          return `${ct.tenSp}${variantName ? ` (${variantName})` : ''} x${ct.soLuongSp}`
        })
        .join(', ')

      let comboPrice = 0
      let originalprice = 0
      comboPrice = item.chitietcombos.reduce((total, ct) => {
        const variant = ct.chitietsanphams.find((v) => v.maCtsp === selectedVariants.value[ct.maSp])
        return total + (variant ? (variant.donGia > 0 ? variant.donGia : 0) * ct.soLuongSp : 0)
      }, 0)
      originalprice = comboPrice
      if (item.phanTramGiam !== null && item.phanTramGiam > 0) {
        comboPrice = comboPrice * (1 - item.phanTramGiam / 100)
      }

      if (item.soTienGiam !== null && item.soTienGiam > 0) {
        comboPrice -= item.soTienGiam
      }
      if (comboPrice <= 0) {
        Swal.fire({
          icon: 'error',
          title: 'Lỗi!',
          text: `Combo ${item.tenCombo} không có giá hợp lệ. Vui lòng kiểm tra dữ liệu.`,
        })
        return
      }
      let sameValue = false
      orderItems.value.forEach((combo) => {
        let name = combo.name.substring(combo.name.indexOf('(') + 1, combo.name.lastIndexOf(')'))
        if (combo.maCombo == item.maCombo && name == chiTietCombo) {
          combo.quantity += quantities.value[item.maCombo]
          sameValue = true
        }
      })
      if (sameValue == false) {
        orderItems.value.push({
          name: `${item.tenCombo} (${chiTietCombo})`,
          quantity: quantities.value[item.maCombo],
          price: Math.round(comboPrice),
          maCombo: item.maCombo,
          original_price: originalprice,
        })
      }
      quantities.value[item.maCombo] = 1
    }
  }
}

const removeFromOrder = (index) => {
  orderItems.value.splice(index, 1)
  // Cập nhật lại giảm giá nếu tổng tiền thay đổi
  if (discountAmount.value > 0) {
    applyCoupon()
  }
}

// Hàm áp dụng mã coupon
const applyCoupon = async () => {
  if (!couponCode.value) {
    couponError.value = 'Vui lòng nhập mã coupon'
    return
  }

  isApplyingCoupon.value = true
  couponError.value = ''
  discountAmount.value = 0

  try {
    const response = await fetch(`${getApiUrl}/api/Coupon/GetAll`, {
      headers: {
        Authorization: `Bearer ${token.value}`,
      },
    })

    if (!response.ok) {
      throw new Error('Không thể kiểm tra mã coupon. Vui lòng thử lại sau.')
    }

    const result = await response.json()
    const coupons = result.data || []
    const coupon = coupons.find((c) => c.maCode === couponCode.value.trim())

    if (!coupon) {
      throw new Error('Mã coupon không tồn tại. Vui lòng kiểm tra lại.')
    }

    // Kiểm tra điều kiện áp dụng mã coupon
    const now = new Date()
    if (!coupon.trangThai) {
      throw new Error('Mã coupon đã bị hủy')
    }
    if (new Date(coupon.ngayKetThuc) < now) {
      throw new Error('Mã coupon đã hết hạn')
    }
    if (new Date(coupon.ngayBatDau) > now) {
      throw new Error('Mã coupon chưa có hiệu lực')
    }
    if (coupon.soLuongDaDung >= coupon.soLuong) {
      throw new Error('Mã coupon đã được sử dụng hết')
    }
    // if (totalAmount.value < coupon.donHangToiThieu) {
    //   throw new Error(
    //     `Đơn hàng phải đạt tối thiểu ${coupon.donHangToiThieu} VNĐ để áp dụng mã này`
    //   );
    // }

    // Tính toán giảm giá
    let discount = 0
    if (coupon.soTienGiam && coupon.soTienGiam > 0) {
      discount = coupon.soTienGiam
    } else if (coupon.phanTramGiam && coupon.phanTramGiam > 0) {
      discount = (totalAmount.value * coupon.phanTramGiam) / 100
    }

    discountAmount.value = Math.round(discount)
    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: `Áp dụng mã coupon thành công. Giảm giá: ${discountAmount.value} VNĐ`,
      timer: 1500,
    })
  } catch (error) {
    couponError.value = error.message
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error.message,
    })
  } finally {
    isApplyingCoupon.value = false
  }
}

// Hàm xóa mã coupon
const removeCoupon = () => {
  couponCode.value = ''
  discountAmount.value = 0
  couponError.value = ''
  Swal.fire({
    icon: 'success',
    title: 'Thành công!',
    text: 'Đã xóa mã coupon.',
    timer: 1500,
  })
}

const checkout = async () => {
  try {
    if (!maNv.value || isNaN(maNv.value)) {
      throw new Error('Không tìm thấy mã nhân viên trong token.')
    }
    const detailComboOrderRequests = []
    const cthoadons = orderItems.value.flatMap((item) => {
      if (item.maCtsp) {
        if (item.price <= 0) {
          throw new Error(`Sản phẩm ${item.name} không có giá hợp lệ.`)
        }
        return [
          {
            maCtsp: parseInt(item.maCtsp),
            soLuong: item.quantity,
            donGia: item.price,
          },
        ]
      } else if (item.maCombo) {
        const combo = combos.value.find((c) => c.maCombo === item.maCombo)
        if (!combo) {
          throw new Error(`Combo ${item.name} không tồn tại.`)
        }
        combo.chitietcombos.map((ct) => {
          const variant = ct.chitietsanphams.find(
            (v) => v.maCtsp === selectedVariants.value[ct.maSp]
          )
          if (!variant) {
            throw new Error(`Biến thể không hợp lệ cho ${ct.tenSp} trong combo.`)
          }
          detailComboOrderRequests.push({
            maCombo: item.maCombo,
            maCTSp: variant.maCtsp,
            soLuong: ct.soLuongSp * item.quantity,
            donGia: variant.donGia,
          })
        })
        return {
          soLuong: item.quantity,
          donGia: item.price,
          maCombo: item.maCombo,
        }
      }
      return []
    })

    const orderData = {
      maNv: parseInt(maNv.value),
      tinhTrang: 'Đã thanh toán',
      ngayTao: new Date().toISOString(),
      diaChiNhanHang: 'Tại quầy',
      hinhThucTt: 'Tại quầy',
      hoTen: 'Khách tại quầy',
      sdt: 'N/A',
      tienGoc: totalAmount.value,
      phiVanChuyen: 0,
      //banId: selectedTable.value.id,
      //giamGiaCoupon: discountAmount.value,
      maCoupon: couponCode.value || null,
      detailCombo_OrderResquests: detailComboOrderRequests,
      cthoadons: cthoadons,
    }
    // console.log(orderData)
    // return
    const response = await fetch(getApiUrl + '/api/CounterBill', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token.value}`,
      },
      body: JSON.stringify(orderData),
    })

    if (!response.ok) {
      const errorText = await response.text()
      throw new Error(`Lỗi từ server: ${errorText}`)
    }
    var result = await response.json()
    bill.value = result.data;
    await fetch(getApiUrl + `/api/Table/${selectedTable.value.id}`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${token.value}`,
      },
      body: JSON.stringify({ id: selectedTable.value.id, tinhTrang: 'Đang sử dụng' }),
    })

    const qrData = getApiUrl + `/api/CounterBill/GetBillDetails/details/${bill.value.maHd}`
    // const homeQrData = 'https://jollibeefood.site'; // URL trang chủ
    const homeQrData = '/' // URL trang chủ
    await nextTick()

    // Tạo mã QR cho modal hóa đơn
    if (qrCodeCanvas.value) {
      await generateQRCode(qrCodeCanvas.value, qrData, 200)
    } else {
      console.error('QR code canvas element for modal not found.')
    }

    // Tạo mã QR cho chi tiết hóa đơn (template in)
    let qrDetailsUrl = ''
    let qrHomeUrl = ''
    const tempCanvasDetails = document.createElement('canvas')
    const tempCanvasHome = document.createElement('canvas')

    try {
      qrDetailsUrl = await generateQRCode(tempCanvasDetails, qrData, 100)
      if (printQrCodeImg.value) {
        printQrCodeImg.value.src = qrDetailsUrl
      }
    } catch (error) {
      console.error('Error generating QR code for print (details):', error)
      Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tạo mã QR chi tiết để in.' })
    }

    // Tạo mã QR cho trang chủ (template in)
    try {
      qrHomeUrl = await generateQRCode(tempCanvasHome, homeQrData, 100)
      if (printHomeQrCodeImg.value) {
        printHomeQrCodeImg.value.src = qrHomeUrl
      }
    } catch (error) {
      console.error('Error generating QR code for print (home):', error)
      Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tạo mã QR trang chủ để in.' })
    }

    // In hóa đơn
    await nextTick()
    const printWindow = window.open('', '_blank')
    if (!printWindow) {
      Swal.fire({
        icon: 'warning',
        title: 'Cảnh báo!',
        text: 'Trình duyệt đã chặn cửa sổ in. Vui lòng cho phép cửa sổ bật lên.',
      })
      return
    }
    const printContent = document.getElementById('print-bill').innerHTML
    printWindow.document.write(`
      <html>
        <head>
          <title>In Hóa Đơn</title>
          <style>
            body { margin: 0; padding: 0; font-family: Arial, sans-serif; }
            ${document.querySelector('style').innerText}
          </style>
        </head>
        <body onload="window.print(); window.close();">
          <div id="print-bill">${printContent}</div>
        </body>
      </html>
    `)
    printWindow.document.close()

    // Hiển thị modal hóa đơn
    const orderModal = document.getElementById('orderAtCounterModal')
    const billModal = document.getElementById('billModal')
    const orderModalInstance = window.bootstrap.Modal.getInstance(orderModal)
    if (orderModalInstance) orderModalInstance.hide()
    const billModalInstance = new window.bootstrap.Modal(billModal)
    billModalInstance.show()
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message })
    console.error('Checkout error:', error)
  }
  fetchTables()
}
const numberToWords = (number) => {
  const units = ['đồng', 'nghìn', 'triệu', 'tỷ']
  const digits = ['không', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín']
  const teens = [
    'mười',
    'mười một',
    'mười hai',
    'mười ba',
    'mười bốn',
    'mười lăm',
    'mười sáu',
    'mười bảy',
    'mười tám',
    'mười chín',
  ]
  const tens = [
    '',
    '',
    'hai mươi',
    'ba mươi',
    'bốn mươi',
    'năm mươi',
    'sáu mươi',
    'bảy mươi',
    'tám mươi',
    'chín mươi',
  ]

  if (number === 0) return 'không đồng'

  let result = ''
  let unitIndex = 0

  while (number > 0) {
    let chunk = number % 1000
    let chunkStr = ''

    if (chunk > 0) {
      let hundreds = Math.floor(chunk / 100)
      let tensUnits = chunk % 100
      let tensDigit = Math.floor(tensUnits / 10)
      let unitsDigit = tensUnits % 10

      if (hundreds > 0) {
        chunkStr += digits[hundreds] + ' trăm'
      }

      if (tensUnits > 0) {
        if (chunkStr) chunkStr += ' '
        if (tensUnits < 10) {
          if (tensUnits === 5 && hundreds > 0) {
            chunkStr += 'lăm'
          } else {
            chunkStr += digits[unitsDigit]
          }
        } else if (tensUnits < 20) {
          if (tensUnits === 15) {
            chunkStr += 'mười lăm'
          } else {
            chunkStr += teens[tensUnits - 10]
          }
        } else {
          chunkStr += tens[tensDigit]
          if (unitsDigit > 0) {
            chunkStr += ' '
            if (unitsDigit === 1) {
              chunkStr += 'mốt'
            } else if (unitsDigit === 5) {
              chunkStr += 'lăm'
            } else {
              chunkStr += digits[unitsDigit]
            }
          }
        }
      }

      if (chunkStr) {
        if (result) result = chunkStr + ' ' + units[unitIndex] + ' ' + result
        else result = chunkStr + ' ' + units[unitIndex]
      }
    }

    number = Math.floor(number / 1000)
    unitIndex++
  }

  return result.trim()
}
const resetOrder = () => {
  orderItems.value = []
  selectedVariants.value = {}
  quantities.value = {}
  quantityErrors.value = {}
  bill.value = null
  menuSearch.value = ''
  categoryFilter.value = ''
  sortOption.value = ''
  couponCode.value = '' // Reset mã coupon
  discountAmount.value = 0 // Reset giảm giá
  couponError.value = '' // Reset lỗi
}

const getTableClass = (status) => {
  switch (status) {
    case 'Trống':
      return 'table-card-available'
    case 'Đang sử dụng':
      return 'table-card-in-use'
    case 'Đã đặt trước':
      return 'table-card-reserved'
    case 'Đang sửa chữa':
      return 'table-card-maintenance'
    default:
      return 'table-card-available'
  }
}

// Theo dõi thay đổi để cập nhật danh sách bàn
watch([searchQuery, statusFilter], fetchTables, { debounce: 300 })

// Khởi tạo khi component được mount
onMounted(() => {
  getTokenAndDecode()
  fetchTables()
})
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css2?family=Roboto:wght@400;500;700&display=swap');
.kiot-container {
  max-width: 1400px;
  margin: 20px auto;
  padding: 20px;
  font-family: 'Arial', sans-serif;
  margin-top: 100px;
}

h2 {
  text-align: center;
  color: #333;
  margin-bottom: 30px;
  font-weight: bold;
}

.form-control,
.form-select {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.form-control:focus,
.form-select:focus {
  border-color: #28a745;
  box-shadow: 0 0 5px rgba(40, 167, 69, 0.3);
  outline: none;
}

/* Table Grid */
.table-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
  gap: 25px;
  padding: 20px;
}

.table-card {
  border-radius: 10px;
  box-shadow: 0 6px 12px rgba(0, 0, 0, 0.1);
  text-align: center;
  padding: 20px;
  transition: transform 0.2s;
  cursor: pointer;
}

.table-card:hover {
  transform: translateY(-5px);
}

.table-card-available {
  background-color: #e6ffe6;
  border: 3px solid #28a745;
}

.table-card-in-use {
  background-color: #ffe6e6;
  border: 3px solid #dc3545;
}

.table-card-reserved {
  background-color: #fff3e6;
  border: 3px solid #fd7e14;
}

.table-card-maintenance {
  background-color: #f2f2f2;
  border: 3px solid #6c757d;
}

.card-body {
  padding: 15px;
}

.card-title {
  font-size: 22px;
  font-weight: bold;
  margin-bottom: 15px;
}

.card-text {
  font-size: 16px;
  margin-bottom: 20px;
}

.card-actions {
  display: flex;
  justify-content: center;
  gap: 15px;
}

.btn {
  padding: 8px 16px;
  border-radius: 5px;
  font-size: 14px;
  transition: background 0.3s;
}

.btn-primary {
  background: #007bff;
  color: white;
}

.btn-primary:hover {
  background: #0056b3;
}

.btn-success {
  background: #28a745;
  color: white;
}

.btn-success:hover {
  background: #218838;
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.btn-danger:hover {
  background: #c82333;
}

.btn-warning {
  background: #ffc107;
  color: #333;
}

.btn-warning:hover {
  background: #e0a800;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}

/* Modal Menu Styles */
.modal-xl {
  max-width: 95%;
}

.menu-order-container {
  display: flex;
  gap: 20px;
  padding: 20px;
}

.menu-left {
  flex: 2;
  background: #fff;
  border-radius: 15px;
  padding: 20px;
  overflow-y: auto;
  max-height: 70vh;
}

.order-right {
  flex: 1;
  background: #f8f9fa;
  border-radius: 15px;
  padding: 20px;
  display: flex;
  flex-direction: column;
  max-height: 70vh;
}

.menu-controls {
  display: flex;
  gap: 10px;
}

.menu-section {
  margin-bottom: 40px;
}

.menu-section h3 {
  font-size: 26px;
  color: #333;
  margin-bottom: 20px;
  border-bottom: 2px solid #ddd;
  padding-bottom: 10px;
}

.menu-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(250px, 1fr));
  gap: 20px;
}

.menu-card {
  display: flex;
  border: 1px solid #ddd;
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  transition: transform 0.2s;
}

.menu-card:hover {
  transform: translateY(-5px);
}

.menu-card-image {
  width: 120px;
  height: 120px;
  flex-shrink: 0;
}

.menu-card-image img {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.menu-card-content {
  padding: 15px;
  flex-grow: 1;
}

.menu-card-content h5 {
  font-size: 16px;
  font-weight: bold;
  margin-bottom: 8px;
}

.menu-card-content p {
  font-size: 14px;
  color: #666;
  margin-bottom: 10px;
}

.variant-selection {
  margin-bottom: 10px;
}

.combo-details {
  margin-bottom: 10px;
}

.combo-item {
  margin-bottom: 8px;
}

.quantity-controls {
  display: flex;
  align-items: center;
  gap: 8px;
  margin-top: 8px;
}

.quantity-controls button {
  width: 28px;
  height: 28px;
  border-radius: 50%;
  border: 1px solid #ddd;
  background: #fff;
  font-size: 14px;
  cursor: pointer;
}

.quantity-controls button:hover:not(:disabled) {
  background: #ff8c00;
  color: #fff;
  border-color: #ff8c00;
}

.quantity-controls button:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.quantity-input {
  width: 50px;
  text-align: center;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 4px;
}
.quantity-input.error {
  border-color: #dc3545;
}
.error-message {
  color: #dc3545;
  font-size: 12px;
  margin-top: 5px;
}
.order-summary {
  margin-top: 40px;
  flex: 1;
  display: flex;
  flex-direction: column;
}
.order-summary h3 {
  font-size: 24px;
  color: #333;
  margin-bottom: 20px;
  border-bottom: 2px solid #ddd;
  padding-bottom: 10px;
}
#qrcode {
  margin: 20px auto;
}
.order-items {
  flex: 1;
  overflow-y: auto;
  margin-bottom: 20px;
}

.order-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 10px;
  border-bottom: 1px solid #ddd;
}
.item-name {
  font-size: 14px;
  flex: 1;
}

.item-controls {
  display: flex;
  align-items: center;
  gap: 10px;
}

.item-quantity {
  font-size: 14px;
  min-width: 30px;
  text-align: center;
}

.item-price {
  font-size: 14px;
  min-width: 80px;
  text-align: right;
}

.order-total {
  font-size: 18px;
  font-weight: bold;
  text-align: right;
  padding: 10px 0;
  border-top: 2px solid #ddd;
}

.order-actions {
  display: flex;
  flex-direction: column;
  gap: 10px;
}

#qrcode {
  margin: 20px auto;
}
/* Bill Modal Styles */
.bill-modal-content {
  background: #fff;
  border-radius: 12px;
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.15);
  font-family: 'Roboto', sans-serif;
}

.bill-modal-header {
  background: linear-gradient(135deg, #007bff, #00c4ff);
  color: white;
  border-top-left-radius: 12px;
  border-top-right-radius: 12px;
  padding: 20px;
}

.bill-modal-header h5 {
  font-size: 24px;
  font-weight: 700;
  margin: 0;
}

.bill-modal-body {
  padding: 30px;
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 20px;
}

.bill-info {
  width: 100%;
  background: #f8f9fa;
  border-radius: 8px;
  padding: 15px;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);
}

.bill-info-row {
  display: grid;
  grid-template-columns: 1fr 1fr; /* Chia đều thành 2 cột */
  align-items: center;
  padding: 10px 0;
  border-bottom: 1px solid #e9ecef;
}

.bill-info-row:last-child {
  border-bottom: none;
}

.bill-label {
  font-size: 16px;
  color: #007bff;
  font-weight: 600;
  text-align: left;
  padding-left: 10px;
}

.bill-value {
  font-size: 16px;
  color: #333;
  text-align: right;
  padding-right: 10px;
}

.bill-qr-code {
  text-align: center;
}

.qr-code-canvas {
  border: 2px solid #e9ecef;
  border-radius: 8px;
  padding: 10px;
  background: #fff;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
  width: 200px;
  height: 200px;
}

.qr-code-text {
  font-size: 14px;
  color: #666;
  margin-top: 10px;
  font-style: italic;
}

.bill-modal-footer {
  border-top: none;
  padding: 15px 30px;
  justify-content: center;
}

.bill-modal-close {
  background: #28a745;
  color: white;
  padding: 10px 25px;
  border-radius: 5px;
  font-size: 16px;
  font-weight: 500;
  transition: background 0.3s ease;
}

.bill-modal-close:hover {
  background: #218838;
}
/* Print Bill Styles */
.print-bill-content {
  width: 80mm; /* Chiều rộng chuẩn cho máy in hóa đơn */
  padding: 10px;
  font-family: 'Arial', sans-serif;
  font-size: 12px;
  line-height: 1.4;
  color: #000;
}
.print-bill-qr-label {
  font-size: 10px;
  font-style: italic;
  text-align: center;
  margin-top: 5px;
  margin-bottom: 10px;
}
.print-bill-header {
  font-size: 16px;
  font-weight: bold;
  text-align: center;
  margin-bottom: 5px;
}

.print-bill-info {
  font-size: 10px;
  text-align: center;
  margin: 0;
}

.print-bill-subheader {
  font-size: 14px;
  font-weight: bold;
  text-align: center;
  margin: 10px 0;
}

.print-bill-section {
  font-size: 12px;
  font-weight: bold;
  text-align: center;
  margin: 10px 0 5px;
}

.print-bill-detail {
  font-size: 10px;
  margin: 3px 0;
}

.print-bill-table {
  width: 100%;
  border-collapse: collapse;
  margin: 5px 0;
}

.print-bill-table th,
.print-bill-table td {
  border: 1px solid #000;
  padding: 3px;
  font-size: 10px;
  text-align: center;
}

.print-bill-table th {
  font-weight: bold;
  background-color: #f0f0f0;
}

.print-bill-total {
  font-size: 12px;
  font-weight: bold;
  text-align: right;
  margin: 5px 0;
  color: #ff0000;
}

.print-bill-note {
  font-size: 10px;
  font-style: italic;
  text-align: center;
  margin: 5px 0;
}

.print-bill-qr {
  text-align: center;
  margin-top: 10px;
}

.print-bill-qr canvas {
  width: 100px;
  height: 100px;
}

/* Ẩn nội dung khác khi in */
@media print {
  body * {
    visibility: hidden;
  }
  #print-bill,
  #print-bill * {
    visibility: visible;
  }
  #print-bill {
    position: absolute;
    left: 0;
    top: 0;
    width: 80mm;
    display: block !important;
  }
}
.qr-code-img {
  width: 100px;
  height: 100px;
  display: block;
  margin: 0 auto;
}
</style>
