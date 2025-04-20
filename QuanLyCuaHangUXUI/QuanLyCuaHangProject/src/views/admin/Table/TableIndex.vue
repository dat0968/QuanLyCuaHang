<template>
  <div class="kiot-container">
    <h2 class="mb-4 text-center">Quản lý bàn</h2>

    <!-- Tìm kiếm và lọc -->
    <div class="row g-3 mb-3 justify-content-center">
      <div class="col-md-4">
        <input v-model="searchQuery" type="text" class="form-control shadow-sm" placeholder="🔍 Tìm theo id bàn..." />
      </div>
      <div class="col-md-4">
        <select v-model="statusFilter" class="form-select shadow-sm">
          <option value="">📋 Tất cả trạng thái</option>
          <option v-for="status in statusOptions" :key="status" :value="status">{{ status }}</option>
        </select>
      </div>
    </div>
    <button class="btn btn-primary btn-sm" data-bs-toggle="modal" data-bs-target="#addTableModal">Thêm bàn mới</button>
    <br>
    <br>

    <!-- Modal thêm bàn -->
    <div class="modal fade" id="addTableModal" tabindex="-1" aria-labelledby="addTableModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="addTableModalLabel">Thêm bàn mới</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body">
            <select v-model="newTableStatus" class="form-select shadow-sm">
              <option v-for="status in statusOptions" :key="status" :value="status">{{ status }}</option>
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
    <div class="modal fade" id="orderAtCounterModal" tabindex="-1" aria-labelledby="orderAtCounterModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-xl modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="orderAtCounterModalLabel">Đặt hàng - Bàn {{ selectedTable?.id }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close" @click="resetOrder"></button>
          </div>
          <div class="modal-body">
            <div class="menu-container">
              <!-- Bộ lọc và sắp xếp -->
              <div class="menu-controls row mb-3">
                <div class="col-md-4">
                  <input v-model="menuSearch" type="text" class="form-control" placeholder="🔍 Tìm sản phẩm..." />
                </div>
                <div class="col-md-4">
                  <select v-model="categoryFilter" class="form-select">
                    <option value="">Tất cả danh mục</option>
                    <option v-for="category in categories" :key="category.maDanhMuc" :value="category.maDanhMuc">
                      {{ category.tenDanhMuc }}
                    </option>
                  </select>
                </div>
                <div class="col-md-4">
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
                <h3>Sản phẩm</h3>
                <div class="menu-grid">
                  <div v-for="product in filteredProducts" :key="product.maSp" class="menu-card">
                    <div class="menu-card-image">
                      <img
                        :src="product.chitietsanphams[0]?.hinhanhs?.[0]?.tenHinhAnh
                          ? getApiUrl+`/HinhAnh/Food_Drink/${product.chitietsanphams[0].hinhanhs[0].tenHinhAnh}`
                          : 'https://via.placeholder.com/150'"
                        :alt="product.tenSanPham"
                        class="img-fluid"
                      />
                    </div>
                    <div class="menu-card-content">
                      <h5>{{ product.tenSanPham }}</h5>
                      <p>{{ product.khoangGia }}</p>
                      <div class="variant-selection">
                        <select v-model="selectedVariants[product.maSp]" class="form-select mb-2" @change="updateSelectedVariant(product.maSp, $event.target.value)">
                          <option v-for="variant in product.chitietsanphams" :key="variant.maCtsp" :value="variant.maCtsp">
                            {{ variant.kichThuoc || 'N/A' }} - {{ variant.huongVi || 'N/A' }} ({{ variant.donGia }} VNĐ)
                          </option>
                        </select>
                      </div>
                      <div class="quantity-controls">
                        <button @click="decreaseQuantity(product.maSp)" :disabled="quantities[product.maSp] <= 1">-</button>
                        <input
                          type="number"
                          v-model="quantities[product.maSp]"
                          @input="handleQuantityChange(product.maSp, $event)"
                          :min="1"
                          :max="getMaxQuantity(product.maSp)"
                          class="quantity-input"
                          :class="{ error: quantityErrors[product.maSp] }"
                        />
                        <button @click="increaseQuantity(product.maSp)" :disabled="quantities[product.maSp] >= getMaxQuantity(product.maSp)">+</button>
                      </div>
                      <p v-if="quantityErrors[product.maSp]" class="error-message">{{ quantityErrors[product.maSp] }}</p>
                      <button class="btn btn-success btn-sm mt-2" @click="addToOrder('product', product)">Thêm</button>
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
                        :src="combo.hinh
                          ? getApiUrl+`/HinhAnh/Food_Drink/${combo.hinh}`
                          : 'https://via.placeholder.com/150'"
                        :alt="combo.tenCombo"
                        class="img-fluid"
                      />
                    </div>
                    <div class="menu-card-content">
                      <h5>{{ combo.tenCombo }}</h5>
                      <p>{{ combo.moTa }}</p>
                      <div class="combo-details">
                        <div v-for="item in combo.chitietcombos" :key="item.maSp" class="combo-item">
                          <span>{{ item.tenSp }} (x{{ item.soLuongSp }})</span>
                          <select v-model="selectedVariants[item.maSp]" class="form-select mb-2" @change="updateSelectedVariant(item.maSp, $event.target.value)">
                            <option v-for="variant in item.chitietsanphams" :key="variant.maCtsp" :value="variant.maCtsp">
                              {{ variant.kichThuoc || 'N/A' }} - {{ variant.huongVi || 'N/A' }} ({{ variant.donGia }} VNĐ)
                            </option>
                          </select>
                        </div>
                      </div>
                      <div class="quantity-controls">
                        <button @click="decreaseQuantity(combo.maCombo)" :disabled="quantities[combo.maCombo] <= 1">-</button>
                        <input
                          type="number"
                          v-model="quantities[combo.maCombo]"
                          @input="handleQuantityChange(combo.maCombo, $event)"
                          :min="1"
                          :max="combo.soLuong"
                          class="quantity-input"
                          :class="{ error: quantityErrors[combo.maCombo] }"
                        />
                        <button @click="increaseQuantity(combo.maCombo)" :disabled="quantities[combo.maCombo] >= combo.soLuong">+</button>
                      </div>
                      <p v-if="quantityErrors[combo.maCombo]" class="error-message">{{ quantityErrors[combo.maCombo] }}</p>
                      <button class="btn btn-success btn-sm mt-2" @click="addToOrder('combo', combo)">Thêm</button>
                    </div>
                  </div>
                </div>
              </div>

              <!-- Đơn hàng -->
              <div class="order-summary mt-4">
                <h3>Đơn hàng</h3>
                <table class="table table-bordered">
                  <thead>
                    <tr>
                      <th>Tên</th>
                      <th>Số lượng</th>
                      <th>Đơn giá</th>
                      <th>Tổng</th>
                      <th>Thao tác</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr v-for="(item, index) in orderItems" :key="index">
                      <td>{{ item.name }}</td>
                      <td>{{ item.quantity }}</td>
                      <td>{{ item.price }} VNĐ</td>
                      <td>{{ item.quantity * item.price }} VNĐ</td>
                      <td><button class="btn btn-danger btn-sm" @click="removeFromOrder(index)">Xóa</button></td>
                    </tr>
                    <tr v-if="!orderItems.length">
                      <td colspan="5" class="text-center">Chưa có sản phẩm</td>
                    </tr>
                  </tbody>
                </table>
                <p class="text-end"><strong>Tổng tiền: {{ totalAmount }} VNĐ</strong></p>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button class="btn btn-success" @click="checkout" :disabled="!orderItems.length">Thanh toán</button>
            <button class="btn btn-secondary" data-bs-dismiss="modal" @click="resetOrder">Hủy</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal hóa đơn -->
    <div class="modal fade" id="billModal" tabindex="-1" aria-labelledby="billModalLabel" aria-hidden="true">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title" id="billModalLabel">Hóa đơn - Mã HD: {{ bill?.maHd }}</h5>
            <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
          </div>
          <div class="modal-body text-center">
            <p><strong>Nhân viên:</strong> {{ bill?.maNv }}</p>
            <p v-if="bill?.maKh"><strong>Khách hàng:</strong> {{ bill?.maKh }}</p>
            <p><strong>Tổng tiền:</strong> {{ bill?.tongtien }} VNĐ</p>
            <canvas id="qrcode" ref="qrCodeCanvas"></canvas> <!-- Sử dụng canvas để render QR -->
          </div>
          <div class="modal-footer">
            <button class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
          </div>
        </div>
      </div>
    </div>

    <!-- Hiển thị bàn dạng card -->
    <div v-if="isLoading" class="text-center py-4">Đang tải dữ liệu...</div>
    <div class="table-grid" v-else>
      <div v-for="table in tables" :key="table.id" :class="getTableClass(table.tinhTrang)" class="table-card">
        <div class="card-body">
          <h5 class="card-title">Bàn {{ table.id }} (id: {{ table.id }}) </h5>
          <p class="card-text">{{ table.tinhTrang }}</p>
          <div class="card-actions">
            <button class="btn btn-success btn-sm" @click="openOrderModal(table)">Đặt hàng</button>
            <button class="btn btn-danger btn-sm" @click="deleteTable(table.id)">Xóa</button>
          </div>
        </div>
      </div>
      <div v-if="!tables.length" class="col-12 text-center py-4">Không có dữ liệu</div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, watch, computed, nextTick } from 'vue';
import Swal from 'sweetalert2';
import QRCode from 'qrcode';
import { GetApiUrl } from '@constants/api'
import {jwtDecode} from 'jwt-decode';
import Cookies from 'js-cookie'
// const immutableStatuses = ["Đang sử dụng", "Đang sửa chữa"];
const tables = ref([]);
const searchQuery = ref('');
const statusFilter = ref('');
const isLoading = ref(false);
const newTableStatus = ref('Trống');
const statusOptions = ["Trống", "Đang sử dụng", "Đã đặt trước", "Đang sửa chữa"];
const selectedTable = ref(null);
const staffId = ref(null);
const customerId = ref(null);
const products = ref([]);
const combos = ref([]);
const categories = ref([]);
const selectedVariants = ref({});
const quantities = ref({});
const quantityErrors = ref({});
const orderItems = ref([]);
const bill = ref(null);
const token = ref('');
const menuSearch = ref('');
const categoryFilter = ref('');
const sortOption = ref('');
const qrCodeCanvas = ref(null); // Ref để tham chiếu canvas
const maNv = ref(null);
const totalAmount = computed(() => {
  return orderItems.value.reduce((sum, item) => sum + item.quantity * item.price, 0);
});
let getApiUrl = GetApiUrl()
// Hàm lấy token và giải mã để lấy MaNv
const getTokenAndDecode = () => {
  token.value = localStorage.getItem('accessToken') || Cookies.get('accessToken') || '';
  if (token.value) {
    try {
      const decoded = jwtDecode(token.value); // Giải mã token
      maNv.value = decoded['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'] || decoded.sub; // Lấy MaNv từ claim
    } catch (error) {
      console.error('Lỗi giải mã token:', error);
      Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể giải mã token.' });
    }
  }
};
// Lọc và sắp xếp sản phẩm
const filteredProducts = computed(() => {
  let filtered = [...products.value];

  if (menuSearch.value) {
    filtered = filtered.filter(p => p.tenSanPham.toLowerCase().includes(menuSearch.value.toLowerCase()));
  }

  if (categoryFilter.value) {
    filtered = filtered.filter(p => p.maDanhMuc === parseInt(categoryFilter.value));
  }

  switch (sortOption.value) {
    case 'priceAsc':
      filtered.sort((a, b) => a.chitietsanphams[0].donGia - b.chitietsanphams[0].donGia);
      break;
    case 'priceDesc':
      filtered.sort((a, b) => b.chitietsanphams[0].donGia - a.chitietsanphams[0].donGia);
      break;
    case 'nameAsc':
      filtered.sort((a, b) => a.tenSanPham.localeCompare(b.tenSanPham));
      break;
    case 'nameDesc':
      filtered.sort((a, b) => b.tenSanPham.localeCompare(a.tenSanPham));
      break;
  }

  return filtered;
});

// Lọc combo
const filteredCombos = computed(() => {
  let filtered = [...combos.value];

  if (menuSearch.value) {
    filtered = filtered.filter(c => c.tenCombo.toLowerCase().includes(menuSearch.value.toLowerCase()));
  }

  return filtered;
});

const getToken = () => {
  token.value = localStorage.getItem('accessToken') || '';
};

const fetchTables = async () => {
  isLoading.value = true;
  try {
    const url = statusFilter.value
      ? getApiUrl+`/api/Table/filter?tinhTrang=${encodeURIComponent(statusFilter.value)}`
      : getApiUrl+`/api/Table`;
    const response = await fetch(url, {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    if (!response.ok) throw new Error(`Không thể tải dữ liệu: ${await response.text()}`);
    const data = await response.json();
    tables.value = Array.isArray(data) ? data : data.items || [];
    if (searchQuery.value) {
      const query = parseInt(searchQuery.value, 10);
      if (!isNaN(query)) tables.value = tables.value.filter(table => table.id === query);
    }
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message });
  } finally {
    isLoading.value = false;
  }
};

const fetchCategories = async () => {
  try {
    const response = await fetch(getApiUrl+'/api/Home/categories', {
      headers: { Authorization: `Bearer ${token.value}` }
    });
    if (!response.ok) throw new Error(await response.text());
    categories.value = await response.json();
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tải danh mục: ' + error.message });
  }
};


const updateStatus = async (table, newStatus) => {
  const previousStatus = table.tinhTrang;
  try {
    const response = await fetch(getApiUrl+`/api/Table/${table.id}`, {
      method: "PUT",
      headers: { 
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token.value}`
      },
      body: JSON.stringify({ id: table.id, tinhTrang: newStatus }),
    });
    if (!response.ok) throw new Error(await response.text() || "Cập nhật thất bại");
    table.tinhTrang = (await response.json()).tinhTrang;
    Swal.fire({ icon: 'success', title: 'Thành công!', text: 'Cập nhật trạng thái thành công', timer: 1500 });
  } catch (error) {
    table.tinhTrang = previousStatus;
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message });
  }
};


const addTable = async () => {
  try {
    const response = await fetch(getApiUrl+`/api/Table`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token.value}`
      },
      body: JSON.stringify({ tinhTrang: newTableStatus.value }),
    });
    if (!response.ok) throw new Error(await response.text());
    tables.value.push(await response.json());
    newTableStatus.value = 'Trống';
    Swal.fire({ icon: 'success', title: 'Thành công!', text: 'Thêm bàn thành công', timer: 1500 });
    const modalElement = document.getElementById('addTableModal');
    const modal = window.bootstrap.Modal.getInstance(modalElement);
    if (modal) modal.hide();
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message });
  }
};

const deleteTable = async (tableId) => {
  const confirm = await Swal.fire({
    title: 'Bạn có chắc chắn?',
    text: "Bạn sẽ không thể khôi phục bàn này!",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Xóa',
    cancelButtonText: 'Hủy',
  });
  if (!confirm.isConfirmed) return;
  try {
    const response = await fetch(getApiUrl+`/api/Table/${tableId}`, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token.value}`
      },
    });
    if (!response.ok) throw new Error(await response.text());
    tables.value = tables.value.filter(table => table.id !== tableId);
    Swal.fire({ icon: 'success', title: 'Thành công!', text: 'Xóa bàn thành công', timer: 1500 });
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message });
  }
};

const openOrderModal = async (table) => {
  selectedTable.value = table;
  try {
    const response = await fetch(getApiUrl+`/api/Table/${table.id}/menu`, {
      headers: { "Authorization": `Bearer ${token.value}` }
    });
    if (!response.ok) throw new Error(await response.text());
    const data = await response.json();
    products.value = data.products;
    combos.value = data.combos;
    // Khởi tạo số lượng và biến thể mặc định
    products.value.forEach(p => {
      quantities.value[p.maSp] = 1;
      selectedVariants.value[p.maSp] = p.chitietsanphams[0]?.maCtsp;
    });
    combos.value.forEach(c => {
      quantities.value[c.maCombo] = 1;
      c.chitietcombos.forEach(item => {
        selectedVariants.value[item.maSp] = item.chitietsanphams[0]?.maCtsp;
      });
    });
    await fetchCategories(); // Lấy danh mục khi mở modal
    const modalElement = document.getElementById('orderAtCounterModal');
    const modal = new window.bootstrap.Modal(modalElement);
    modal.show();
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message });
  }
};

const getMaxQuantity = (id) => {
  const product = products.value.find(p => p.maSp === id);
  if (product) {
    const variant = product.chitietsanphams.find(v => v.maCtsp === selectedVariants.value[id]);
    return variant?.soLuongTon || 0;
  }
  const combo = combos.value.find(c => c.maCombo === id);
  return combo?.soLuong || 0;
};

const validateQuantity = (id, value) => {
  if (!value || isNaN(value) || value < 1) {
    quantityErrors.value[id] = 'Số lượng phải là số lớn hơn 0';
    return false;
  }
  const max = getMaxQuantity(id);
  if (value > max) {
    quantityErrors.value[id] = `Số lượng tối đa là ${max}`;
    return false;
  }
  quantityErrors.value[id] = '';
  return true;
};

const handleQuantityChange = (id, event) => {
  const value = parseInt(event.target.value) || 1;
  if (validateQuantity(id, value)) {
    quantities.value[id] = value;
  } else {
    quantities.value[id] = Math.min(Math.max(value, 1), getMaxQuantity(id));
  }
};

const increaseQuantity = (id) => {
  if (quantities.value[id] < getMaxQuantity(id)) {
    quantities.value[id]++;
    quantityErrors.value[id] = '';
  }
};

const decreaseQuantity = (id) => {
  if (quantities.value[id] > 1) {
    quantities.value[id]--;
    quantityErrors.value[id] = '';
  }
};

const updateSelectedVariant = (id, variantId) => {
  selectedVariants.value[id] = variantId;
};

const addToOrder = (type, item) => {
  if (type === 'product') {
    const variant = item.chitietsanphams.find(v => v.maCtsp === selectedVariants.value[item.maSp]);
    if (!variant) {
      Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Vui lòng chọn biến thể sản phẩm.' });
      return;
    }
    if (validateQuantity(item.maSp, quantities.value[item.maSp])) {
      orderItems.value.push({
        name: `${item.tenSanPham} (${variant.kichThuoc || 'N/A'} - ${variant.huongVi || 'N/A'})`,
        quantity: quantities.value[item.maSp],
        price: variant.donGia,
        maCtsp: variant.maCtsp,
      });
      quantities.value[item.maSp] = 1; // Reset số lượng sau khi thêm
    }
  } else if (type === 'combo') {
    if (validateQuantity(item.maCombo, quantities.value[item.maCombo])) {
      const chiTietCombo = item.chitietcombos.map(ct => {
        const variant = ct.chitietsanphams.find(v => v.maCtsp === selectedVariants.value[ct.maSp]);
        if (!variant) {
          Swal.fire({ icon: 'error', title: 'Lỗi!', text: `Vui lòng chọn biến thể cho ${ct.tenSp} trong combo.` });
          return null;
        }
        return `${ct.tenSp} (${variant.kichThuoc || 'N/A'} - ${variant.huongVi || 'N/A'}) x${ct.soLuongSp}`;
      }).filter(Boolean).join(', ');
      if (!chiTietCombo) return; // Nếu có lỗi biến thể, không thêm
      orderItems.value.push({
        name: `${item.tenCombo} (${chiTietCombo})`,
        quantity: quantities.value[item.maCombo],
        price: item.soTienGiam || 0,
        maCombo: item.maCombo,
      });
      quantities.value[item.maCombo] = 1; // Reset số lượng sau khi thêm
    }
  }
};

const removeFromOrder = (index) => {
  orderItems.value.splice(index, 1);
};

const checkout = async () => {
  try {
    // Kiểm tra MaNv từ token
    if (!maNv.value || isNaN(maNv.value)) {
      throw new Error("Không tìm thấy mã nhân viên trong token.");
    }

    const orderData = {
      maKh: customerId.value && !isNaN(customerId.value) ? parseInt(customerId.value) : null,
      maNv: parseInt(maNv.value), // Sử dụng MaNv từ token
      tinhTrang: 'Đã thanh toán',
      ngayTao: new Date().toISOString(),
      hinhThucTt: 'Tại quầy',
      tienGoc: totalAmount.value,
      phiVanChuyen: 0,
      cthoadons: orderItems.value.map(item => {
        if (!item.maCtsp || isNaN(item.maCtsp)) {
          throw new Error(`Mã chi tiết sản phẩm không hợp lệ cho ${item.name}`);
        }
        return {
          maCtsp: parseInt(item.maCtsp),
          soLuong: item.quantity,
          donGia: item.price
        };
      }),
    };

    const response = await fetch(getApiUrl + '/api/CounterBill', {
      method: 'POST',
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token.value}`
      },
      body: JSON.stringify(orderData),
    });

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Lỗi từ server: ${response.status} - ${errorText}`);
    }

    bill.value = await response.json();

    await fetch(getApiUrl + `/api/Table/${selectedTable.value.id}`, {
      method: 'PUT',
      headers: {
        "Content-Type": "application/json",
        "Authorization": `Bearer ${token.value}`
      },
      body: JSON.stringify({ id: selectedTable.value.id, tinhTrang: 'Đang sử dụng' }),
    });

    // Tạo mã QR
    const qrData = `https://localhost:7139/api/Bill/GetBillDetails/details/${bill.value.maHd}`;
    await nextTick();
    if (qrCodeCanvas.value) {
      QRCode.toCanvas(qrCodeCanvas.value, qrData, { width: 200 }, (error) => {
        if (error) {
          console.error('Error generating QR code:', error);
          Swal.fire({ icon: 'error', title: 'Lỗi!', text: 'Không thể tạo mã QR.' });
        }
      });
    } else {
      console.error('QR code canvas element not found.');
    }

    const orderModal = document.getElementById('orderAtCounterModal');
    const billModal = document.getElementById('billModal');
    const orderModalInstance = window.bootstrap.Modal.getInstance(orderModal);
    if (orderModalInstance) orderModalInstance.hide();
    const billModalInstance = new window.bootstrap.Modal(billModal);
    billModalInstance.show();
  } catch (error) {
    Swal.fire({ icon: 'error', title: 'Lỗi!', text: error.message });
    console.error('Checkout error:', error);
  }
};

const resetOrder = () => {
  staffId.value = null;
  customerId.value = null;
  orderItems.value = [];
  selectedVariants.value = {};
  quantities.value = {};
  quantityErrors.value = {};
  bill.value = null;
  menuSearch.value = '';
  categoryFilter.value = '';
  sortOption.value = '';
};

const getTableClass = (status) => {
  switch (status) {
    case 'Trống': return 'table-card-available';
    case 'Đang sử dụng': return 'table-card-in-use';
    case 'Đã đặt trước': return 'table-card-reserved';
    case 'Đang sửa chữa': return 'table-card-maintenance';
    default: return 'table-card-available';
  }
};

watch([searchQuery, statusFilter], fetchTables, { debounce: 300 });
onMounted(() => {
  getToken();
  fetchTables();
  getTokenAndDecode();
});
</script>

<style scoped>
.kiot-container {
  max-width: 1200px;
  margin: 20px auto;
  padding: 20px;
  font-family: 'Arial', sans-serif;
}

h2 {
  text-align: center;
  color: #333;
  margin-bottom: 30px;
  font-weight: bold;
}

.form-control, .form-select {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 5px;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.form-control:focus, .form-select:focus {
  border-color: #28a745;
  box-shadow: 0 0 5px rgba(40, 167, 69, 0.3);
  outline: none;
}

.table-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(150px, 1fr));
  gap: 20px;
}

.table-card {
  border-radius: 8px;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  text-align: center;
  padding: 15px;
  transition: transform 0.2s;
  cursor: pointer;
}

.table-card:hover { transform: translateY(-5px); }
.table-card-available { background-color: #e6ffe6; border: 2px solid #28a745; }
.table-card-in-use { background-color: #ffe6e6; border: 2px solid #dc3545; }
.table-card-reserved { background-color: #fff3e6; border: 2px solid #fd7e14; }
.table-card-maintenance { background-color: #f2f2f2; border: 2px solid #6c757d; }

.card-body { padding: 10px; }
.card-title { font-size: 18px; font-weight: bold; margin-bottom: 10px; }
.card-text { font-size: 14px; margin-bottom: 15px; }
.card-actions { display: flex; justify-content: center; gap: 10px; }

.btn {
  padding: 6px 12px;
  border-radius: 5px;
  font-size: 12px;
  transition: background 0.3s;
}

.btn-primary { background: #007bff; color: white; }
.btn-primary:hover { background: #0056b3; }
.btn-success { background: #28a745; color: white; }
.btn-success:hover { background: #218838; }
.btn-danger { background: #dc3545; color: white; }
.btn-danger:hover { background: #c82333; }
.btn-secondary { background: #6c757d; color: white; }
.btn-secondary:hover { background: #5a6268; }

/* Modal Menu Styles */
.modal-xl { max-width: 90%; }
.menu-container { padding: 20px; background: #fff; border-radius: 15px; }
.menu-controls { display: flex; gap: 15px; }
.menu-section { margin-bottom: 40px; }
.menu-section h3 { font-size: 24px; color: #333; margin-bottom: 20px; }
.menu-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
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
.menu-card:hover { transform: translateY(-5px); }
.menu-card-image {
  width: 150px;
  height: 150px;
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
.menu-card-content h5 { font-size: 18px; font-weight: bold; margin-bottom: 10px; }
.menu-card-content p { font-size: 14px; color: #666; margin-bottom: 15px; }
.variant-selection { margin-bottom: 15px; }
.combo-details { margin-bottom: 15px; }
.combo-item { margin-bottom: 10px; }
.quantity-controls {
  display: flex;
  align-items: center;
  gap: 10px;
  margin-top: 10px;
}
.quantity-controls button {
  width: 30px;
  height: 30px;
  border-radius: 50%;
  border: 1px solid #ddd;
  background: #fff;
  font-size: 16px;
  cursor: pointer;
}
.quantity-controls button:hover:not(:disabled) { background: #ff8c00; color: #fff; border-color: #ff8c00; }
.quantity-controls button:disabled { opacity: 0.5; cursor: not-allowed; }
.quantity-input {
  width: 60px;
  text-align: center;
  border: 1px solid #ddd;
  border-radius: 4px;
  padding: 4px;
}
.quantity-input.error { border-color: #dc3545; }
.error-message { color: #dc3545; font-size: 12px; margin-top: 5px; }
.order-summary { margin-top: 40px; }
.order-summary h3 { font-size: 24px; color: #333; margin-bottom: 20px; }
#qrcode { margin: 20px auto; }
</style>
