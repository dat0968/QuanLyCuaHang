<script setup>
import { ref, onMounted, computed } from 'vue';
import Swal from 'sweetalert2';
import { ReadToken, ValidateToken } from '../../../Authentication_Authorization/auth.js'
import Cookies from 'js-cookie'
import { GetApiUrl } from '@constants/api'
let getApiUrl = GetApiUrl()
const coupons = ref([]);
const showModal = ref(false);
const isEdit = ref(false);
const couponForm = ref({
  maCode: '', // Sửa từ masabCode thành maCode để đồng bộ
  soTienGiam: null,
  phanTramGiam: null,
  ngayBatDau: '',
  ngayKetThuc: '',
  trangThai: true,
  donHangToiThieu: null,
  soLuong: null,
  soLuongDaDung: 0
});


// Thêm các biến cho bộ lọc
const searchQuery = ref('');
const sortField = ref('maCode');
const sortOrder = ref('asc');
const filterStatus = ref('all');
const currentPage = ref(1);
const itemsPerPage = ref(10);


const baseUrl = getApiUrl+'/api/Coupon';

// Fetch all coupons
const fetchCoupons = async () => {
  try {
    const response = await fetch(`${baseUrl}/GetAll`);
    const data = await response.json();
    if (data.success) {
      coupons.value = data.data.map(coupon => ({
        ...coupon,
        ngayBatDau: coupon.ngayBatDau ? new Date(coupon.ngayBatDau).toISOString().split('T')[0] : '',
        ngayKetThuc: coupon.ngayKetThuc ? new Date(coupon.ngayKetThuc).toISOString().split('T')[0] : ''
      }));
    }
  } catch (error) {
    Swal.fire('Lỗi', 'Không thể tải danh sách coupon', 'error');
    console.error('Fetch error:', error);
  }
};

// Computed properties
const filteredCoupons = computed(() => {
  let result = [...coupons.value];
  if (searchQuery.value) {
    const query = searchQuery.value.toLowerCase();
    result = result.filter(coupon => 
      coupon.maCode.toLowerCase().includes(query) ||
      String(coupon.soTienGiam).includes(query) ||
      String(coupon.phanTramGiam).includes(query)
    );
  }
  if (filterStatus.value !== 'all') {
    result = result.filter(coupon => 
      coupon.trangThai === (filterStatus.value === 'active')
    );
  }
  result.sort((a, b) => {
    const fieldA = a[sortField.value];
    const fieldB = b[sortField.value];
    if (sortOrder.value === 'asc') {
      return fieldA > fieldB ? 1 : -1;
    }
    return fieldA < fieldB ? 1 : -1;
  });
  return result;
});

const paginatedCoupons = computed(() => {
  const start = (currentPage.value - 1) * itemsPerPage.value;
  const end = start + itemsPerPage.value;
  return filteredCoupons.value.slice(start, end);
});

const totalPages = computed(() => {
  return Math.ceil(filteredCoupons.value.length / itemsPerPage.value);
});

// Format date
const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return isNaN(date.getTime()) ? '' : date.toLocaleDateString('vi-VN');
};

const formatDateForInput = (dateString) => {
  if (!dateString) return '';
  return new Date(dateString).toISOString().split('T')[0];
};

// Modal handlers
const showAddModal = () => {
  isEdit.value = false;
  couponForm.value = {
    maCode: '',
    soTienGiam: null,
    phanTramGiam: null,
    ngayBatDau: '',
    ngayKetThuc: '',
    trangThai: true,
    donHangToiThieu: null,
    soLuong: null,
    soLuongDaDung: 0
  };
  showModal.value = true;
};

const showEditModal = (coupon) => {
  isEdit.value = true;
  couponForm.value = {
    maCode: coupon.maCode,
    soTienGiam: coupon.soTienGiam,
    phanTramGiam: coupon.phanTramGiam,
    ngayBatDau: coupon.ngayBatDau ? formatDateForInput(coupon.ngayBatDau) : '',
    ngayKetThuc: coupon.ngayKetThuc ? formatDateForInput(coupon.ngayKetThuc) : '',
    trangThai: coupon.trangThai,
    donHangToiThieu: coupon.donHangToiThieu,
    soLuong: coupon.soLuong,
    soLuongDaDung: coupon.soLuongDaDung || 0
  };
  showModal.value = true;
};

const hideModal = () => {
  showModal.value = false;
};

// Validate discount
const validateDiscount = () => {
  const hasSoTienGiam = couponForm.value.soTienGiam !== null && couponForm.value.soTienGiam > 0;
  const hasPhanTramGiam = couponForm.value.phanTramGiam !== null && couponForm.value.phanTramGiam > 0;

  if (hasSoTienGiam && hasPhanTramGiam) {
    Swal.fire('Lỗi', 'Chỉ được nhập một trong hai: Số tiền giảm hoặc Phần trăm giảm', 'error');
    return false;
  }
  if (!hasSoTienGiam && !hasPhanTramGiam) {
    Swal.fire('Lỗi', 'Phải nhập ít nhất một trong hai: Số tiền giảm hoặc Phần trăm giảm', 'error');
    return false;
  }
  return true;
};

// CRUD operations
const createCoupon = async () => {
  if (!validateDiscount()) return;

  try {
    const response = await fetch(`${baseUrl}/Create`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ...couponForm.value,
        soTienGiam: couponForm.value.soTienGiam || 0,
        phanTramGiam: couponForm.value.phanTramGiam || 0,
        ngayBatDau: couponForm.value.ngayBatDau ? new Date(couponForm.value.ngayBatDau).toISOString() : null,
        ngayKetThuc: couponForm.value.ngayKetThuc ? new Date(couponForm.value.ngayKetThuc).toISOString() : null,
        donHangToiThieu: couponForm.value.donHangToiThieu || 0,
        soLuongDaDung: 0
      })
    });
    const data = await response.json();
    if (data.success) {
      Swal.fire('Thành công', data.message, 'success');
      hideModal();
      fetchCoupons();
    } else {
      Swal.fire('Lỗi', data.message, 'error');
    }
  } catch (error) {
    Swal.fire('Lỗi', 'Không thể thêm coupon', 'error');
    console.error('Create error:', error);
  }
};

const updateCoupon = async () => {
  if (!validateDiscount()) return;

  try {
    const response = await fetch(`${baseUrl}/Update`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        ...couponForm.value,
        soTienGiam: couponForm.value.soTienGiam || 0,
        phanTramGiam: couponForm.value.phanTramGiam || 0,
        ngayBatDau: couponForm.value.ngayBatDau ? new Date(couponForm.value.ngayBatDau).toISOString() : null,
        ngayKetThuc: couponForm.value.ngayKetThuc ? new Date(couponForm.value.ngayKetThuc).toISOString() : null,
        donHangToiThieu: couponForm.value.donHangToiThieu || 0
      })
    });
    const data = await response.json();
    if (data.success) {
      Swal.fire('Thành công', data.message, 'success');
      hideModal();
      fetchCoupons();
    } else {
      Swal.fire('Lỗi', data.message, 'error');
    }
  } catch (error) {
    Swal.fire('Lỗi', 'Không thể cập nhật coupon', 'error');
    console.error('Update error:', error);
  }
};

const deleteCoupon = async (id) => {
  const result = await Swal.fire({
    title: 'Bạn có chắc?',
    text: 'Bạn muốn hủy coupon này?',
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Có',
    cancelButtonText: 'Không'
  });

  if (result.isConfirmed) {
    try {
      const response = await fetch(`${baseUrl}/Cancel?id=${id}`, {
        method: 'PUT'
      });
      const data = await response.json();
      if (data.success) {
        Swal.fire('Thành công', data.message, 'success');
        fetchCoupons();
      } else {
        Swal.fire('Lỗi', data.message, 'error');
      }
    } catch (error) {
      Swal.fire('Lỗi', 'Không thể hủy coupon', 'error');
      console.error('Delete error:', error);
    }
  }
};

// Sort and pagination handlers
const changeSort = (field) => {
  if (sortField.value === field) {
    sortOrder.value = sortOrder.value === 'asc' ? 'desc' : 'asc';
  } else {
    sortField.value = field;
    sortOrder.value = 'asc';
  }
};

const goToPage = (page) => {
  if (page >= 1 && page <= totalPages.value) {
    currentPage.value = page;
  }
};

onMounted(() => {
  fetchCoupons();
});
</script>

<template>
  <div>
    <br>
  <br>
  <div class="container mt-4">
    <h1>Quản lý mã Coupon</h1>
   
    <!-- Bộ lọc -->
    <div class="row mb-3">
      
      <div class="col-md-4">
        <label for="search" class="form-label">Tìm kiếm</label>
        <input 
          v-model="searchQuery" 
          type="text" 
          class="form-control" 
          placeholder="Tìm kiếm mã, số tiền, phần trăm..." 
        >
      </div>
      
      <div class="col-md-3">
        <label for="statusFilter" class="form-label">Trạng thái</label>
        <select v-model="filterStatus" class="form-control">
          <option value="all">Tất cả trạng thái</option>
          <option value="active">Hoạt động</option>
          <option value="inactive">Đã hủy</option>
        </select>
      </div>
      <div class="col-md-1">
        <label for="itemsPerPage" class="form-label">Số trang</label>
        <select v-model="itemsPerPage" class="form-control">
          <option value="5">5</option>
          <option value="10">10</option>
          <option value="20">20</option>
          <option value="50">50</option>
        </select>
      </div>
      <div class="col-md-3">
      <label class="form-label invisible">Ẩn label</label>
      <button class="btn btn-primary w-100" @click="showAddModal">Thêm mới</button>
    </div>
    </div>

    <!-- Bảng coupons -->
    <table class="table table-striped">
      <thead>
        <tr>
          <th @click="changeSort('maCode')" class="sortable">
            Mã Coupon
            <span v-if="sortField === 'maCode'">{{ sortOrder === 'asc' ? '↑' : '↓' }}</span>
          </th>
          <th @click="changeSort('soTienGiam')" class="sortable">
            Số tiền giảm
            <span v-if="sortField === 'soTienGiam'">{{ sortOrder === 'asc' ? '↑' : '↓' }}</span>
          </th>
          <th @click="changeSort('phanTramGiam')" class="sortable">
            Phần trăm giảm
            <span v-if="sortField === 'phanTramGiam'">{{ sortOrder === 'asc' ? '↑' : '↓' }}</span>
          </th>
          <th>Ngày bắt đầu</th>
          <th>Ngày kết thúc</th>
          <th>Đơn tối thiểu</th>
          <th>Số lượng</th>
          <th>Trạng thái</th>
          <th>Hành động</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="coupon in paginatedCoupons" :key="coupon.maCode">
          <td>{{ coupon.maCode }}</td>
          <td>{{ coupon.soTienGiam }}</td>
          <td>{{ coupon.phanTramGiam?? 0 }} %</td>
          <td>{{ formatDate(coupon.ngayBatDau) }}</td>
          <td>{{ formatDate(coupon.ngayKetThuc) }}</td>
          <td>{{ coupon.donHangToiThieu }}</td>
          <td>{{ coupon.soLuong }}</td>
          <td>{{ coupon.trangThai ? 'Hoạt động' : 'Đã hủy' }}</td>
          <td>
            <button 
              v-if="coupon.trangThai" 
              class="btn btn-warning btn-sm me-2" 
              @click="showEditModal(coupon)"
            >
              Sửa
            </button>
            <button 
              v-if="coupon.trangThai" 
              class="btn btn-danger btn-sm" 
              @click="deleteCoupon(coupon.maCode)"
            >
              Hủy
            </button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Phân trang -->
    <div class="d-flex justify-content-between align-items-center">
      <div>
        Hiển thị {{ (currentPage - 1) * itemsPerPage + 1 }} - 
        {{ Math.min(currentPage * itemsPerPage, filteredCoupons.length) }} 
        của {{ filteredCoupons.length }} kết quả
      </div>
      <div>
        <button 
          class="btn btn-secondary me-2" 
          :disabled="currentPage === 1" 
          @click="goToPage(currentPage - 1)"
        >
          Trước
        </button>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>
        <button 
          class="btn btn-secondary ms-2" 
          :disabled="currentPage === totalPages" 
          @click="goToPage(currentPage + 1)"
        >
          Sau
        </button>
      </div>
    </div>

    <!-- Modal -->
    <div class="modal" :class="{ 'd-block': showModal }" tabindex="-1">
      <div class="modal-dialog modal-lg">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">{{ isEdit ? 'Sửa Coupon' : 'Thêm Coupon' }}</h5>
            <button type="button" class="btn-close" @click="hideModal"></button>
          </div>
          <div class="modal-body">
            <form @submit.prevent="isEdit ? updateCoupon() : createCoupon()">
              <div class="row">
                <div v-if="isEdit" class="col-md-6 mb-3">
                  <label class="form-label">Mã Coupon</label>
                  <input v-model="couponForm.maCode" class="form-control" disabled>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Số tiền giảm</label>
                  <input 
                    v-model="couponForm.soTienGiam" 
                    type="number" 
                    class="form-control" 
                    min="0"
                    :disabled="couponForm.phanTramGiam > 0" 
                  >
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Phần trăm giảm</label>
                  <input 
                    v-model="couponForm.phanTramGiam" 
                    type="number" 
                    class="form-control" 
                    min="0" 
                    max="100"
                    :disabled="couponForm.soTienGiam > 0" 
                  >
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Giá trị đơn hàng tối thiểu</label>
                  <input v-model="couponForm.donHangToiThieu" type="number" class="form-control" min="0">
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ngày bắt đầu</label>
                  <input v-model="couponForm.ngayBatDau" type="date" class="form-control" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Ngày kết thúc</label>
                  <input v-model="couponForm.ngayKetThuc" type="date" class="form-control" required>
                </div>
                <div class="col-md-6 mb-3">
                  <label class="form-label">Số lượng</label>
                  <input v-model="couponForm.soLuong" type="number" class="form-control" min="1" required>
                </div>
                <div v-if="isEdit" class="col-md-6 mb-3">
                  <label class="form-label">Trạng thái</label>
                  <select v-model="couponForm.trangThai" class="form-control">
                    <option :value="true">Hoạt động</option>
                    <option :value="false">Đã hủy</option>
                  </select>
                </div>
              </div>
              <button type="submit" class="btn btn-primary">Lưu</button>
            </form>
          </div>
        </div>
      </div>
    </div>
  </div>
  </div>
</template>

<style scoped>
.modal {
  background: rgba(0, 0, 0, 0.5);
}
.table {
  margin-top: 20px;
}
.sortable {
  cursor: pointer;
  user-select: none;
}
.sortable:hover {
  background-color: #f5f5f5;
}
</style>