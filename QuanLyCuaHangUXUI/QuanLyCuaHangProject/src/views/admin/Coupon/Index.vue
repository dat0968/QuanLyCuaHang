<script setup>
import { ref, onMounted } from 'vue';
import Swal from 'sweetalert2';

const coupons = ref([]);
const showModal = ref(false);
const isEdit = ref(false);
const couponForm = ref({
  maCode: '',
  soTienGiam: null,
  phanTramGiam: null,
  ngayBatDau: '',
  ngayKetThuc: '',
  trangThai: true, // Mặc định là true (Hoạt động)
  donHangToiThieu: null,
  soLuong: null,
  soLuongDaDung: 0
});

const baseUrl = 'https://localhost:7139/api/Coupon';

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

// Format date for display in table
const formatDate = (dateString) => {
  if (!dateString) return '';
  const date = new Date(dateString);
  return isNaN(date.getTime()) ? '' : date.toLocaleDateString('vi-VN');
};

// Format date for input fields
const formatDateForInput = (dateString) => {
  if (!dateString) return '';
  return new Date(dateString).toISOString().split('T')[0];
};

// Show add modal
const showAddModal = () => {
  isEdit.value = false;
  couponForm.value = {
    maCode: '',
    soTienGiam: null,
    phanTramGiam: null,
    ngayBatDau: '',
    ngayKetThuc: '',
    trangThai: true, // Mặc định là Hoạt động khi thêm mới
    donHangToiThieu: null,
    soLuong: null,
    soLuongDaDung: 0
  };
  showModal.value = true;
};

// Show edit modal
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

// Hide modal
const hideModal = () => {
  showModal.value = false;
};

// Validate discount fields: Chỉ cho phép nhập một trong hai
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

// Create coupon
const createCoupon = async () => {
  if (!validateDiscount()) return;

  try {
    const response = await fetch(`${baseUrl}/Create`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        soTienGiam: couponForm.value.soTienGiam || 0, // Gửi 0 nếu không nhập
        phanTramGiam: couponForm.value.phanTramGiam || 0, // Gửi 0 nếu không nhập
        ngayBatDau: couponForm.value.ngayBatDau ? new Date(couponForm.value.ngayBatDau).toISOString() : null,
        ngayKetThuc: couponForm.value.ngayKetThuc ? new Date(couponForm.value.ngayKetThuc).toISOString() : null,
        trangThai: couponForm.value.trangThai,
        donHangToiThieu: couponForm.value.donHangToiThieu || 0,
        soLuong: couponForm.value.soLuong,
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

// Update coupon
const updateCoupon = async () => {
  if (!validateDiscount()) return;

  try {
    const response = await fetch(`${baseUrl}/Update`, {
      method: 'PUT',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        maCode: couponForm.value.maCode,
        soTienGiam: couponForm.value.soTienGiam || 0,
        phanTramGiam: couponForm.value.phanTramGiam || 0,
        ngayBatDau: couponForm.value.ngayBatDau ? new Date(couponForm.value.ngayBatDau).toISOString() : null,
        ngayKetThuc: couponForm.value.ngayKetThuc ? new Date(couponForm.value.ngayKetThuc).toISOString() : null,
        trangThai: couponForm.value.trangThai,
        donHangToiThieu: couponForm.value.donHangToiThieu || 0,
        soLuong: couponForm.value.soLuong,
        soLuongDaDung: couponForm.value.soLuongDaDung || 0
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

// Delete coupon
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

onMounted(() => {
  fetchCoupons();
});
</script>

<template>
  <br>
  <br>
  <div class="container mt-4">
    <h1>Quản lý mã Coupon</h1>
    
    <!-- Add Button -->
    <button class="btn btn-primary mb-3" @click="showAddModal">Thêm mới Coupon</button>

    <!-- Coupons Table -->
    <table class="table table-striped">
      <thead>
        <tr>
          <th>Mã Coupon</th>
          <th>Số tiền giảm</th>
          <th>Phần trăm giảm</th>
          <th>Ngày bắt đầu</th>
          <th>Ngày kết thúc</th>
          <th>Đơn tối thiểu</th>
          <th>Số lượng</th>
          <th>Trạng thái</th>
          <th>Hành động</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="coupon in coupons" :key="coupon.maCode">
          <td>{{ coupon.maCode }}</td>
          <td>{{ coupon.soTienGiam }}</td>
          <td>{{ coupon.phanTramGiam }}</td>
          <td>{{ formatDate(coupon.ngayBatDau) }}</td>
          <td>{{ formatDate(coupon.ngayKetThuc) }}</td>
          <td>{{ coupon.donHangToiThieu }}</td>
          <td>{{ coupon.soLuong }}</td>
          <td>{{ coupon.trangThai ? 'Hoạt động' : 'Đã hủy' }}</td>
          <td>
            <button class="btn btn-warning btn-sm me-2" @click="showEditModal(coupon)">Sửa</button>
            <button class="btn btn-danger btn-sm" @click="deleteCoupon(coupon.maCode)">Hủy</button>
          </td>
        </tr>
      </tbody>
    </table>

    <!-- Add/Edit Modal -->
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
</template>

<style scoped>
.modal {
  background: rgba(0, 0, 0, 0.5);
}
.table {
  margin-top: 20px;
}
</style>