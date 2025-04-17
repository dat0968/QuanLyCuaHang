<template>
  <div class="staff-page" style="width: 1600px;">
    <h1>Quản Lý Nhân Viên</h1>
    <div class="filter-section">
      <input
        v-model="searchTerm"
        placeholder="Tìm kiếm theo họ tên, SĐT, email..."
        @input="fetchStaff"
        class="search-input"
      />
      <select v-model="gioiTinh" @change="fetchStaff">
        <option value="">Tất cả giới tính</option>
        <option value="Nam">Nam</option>
        <option value="Nữ">Nữ</option>
      </select>
      <select v-model="tinhTrang" @change="fetchStaff">
        <option value="">Tất cả tình trạng</option>
        <option value="Đang hoạt động">Đang hoạt động</option>
        <option value="Đã tạm khóa">Đã tạm khóa</option>
      </select>
    </div>
    <button @click="openAddModal" class="add-btn">Thêm Nhân Viên</button>
    <table class="staff-table">
      <thead>
        <tr>
          <th>Mã NV</th>
          <th>Họ Tên</th>
          <th>Giới Tính</th>
          <th>Ngày Sinh</th>
          <th>Địa Chỉ</th>
          <th>CCCD</th>
          <th>SĐT</th>
          <th>Email</th>
          <th>Ngày Vào Làm</th>
          <th>Tình Trạng</th>
          <th>Chức Vụ</th>
          <th>Hình Ảnh</th>
          <th>Hành Động</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="staff in paginatedStaffList" :key="staff.maNv">
          <td>{{ staff.maNv }}</td>
          <td>
            <a href="#" @click.prevent="openDetailModal(staff)">{{ staff.hoTen }}</a>
          </td>
          <td>{{ staff.gioiTinh }}</td>
          <td>{{ staff.ngaySinh || 'Chưa có' }}</td>
          <td>{{ staff.diaChi }}</td>
          <td>{{ staff.cccd }}</td>
          <td>{{ staff.sdt }}</td>
          <td>{{ staff.email }}</td>
          <td>{{ staff.ngayVaoLam }}</td>
          <td>{{ staff.tinhTrang }}</td>
          <td>{{ staff.maChucVu || 'Chưa có' }}</td>
          <!-- <td>
            <img
              v-if="staff.hinhAnhDuongDan"
              :src="getImageUrl(staff.S)"
              alt="Hình ảnh nhân viên"
              class="staff-image"
              @error="handleImageError"
            />
            <span v-else class="no-image">Không có ảnh</span>
          </td> -->
          <td class="action-buttons">
            <button @click="openDetailModal(staff)" class="detail-btn">Chi Tiết</button>
            <button @click="openEditModal(staff)" class="edit-btn">Sửa</button>
            <button @click="toggleDelete(staff.maNv)" class="delete-btn">Ẩn</button>
          </td>
        </tr>
      </tbody>
    </table>
    <div class="pagination">
      <div class="pagination-controls">
        <button @click="prevPage" :disabled="currentPage === 1">Trước</button>
        <span>Trang {{ currentPage }} / {{ totalPages }}</span>
        <button @click="nextPage" :disabled="currentPage === totalPages">Sau</button>
      </div>
      <div class="pagination-size">
        <label>Số bản ghi mỗi trang:</label>
        <select v-model="pageSize" @change="updatePageSize">
          <option :value="5">5</option>
          <option :value="10">10</option>
          <option :value="20">20</option>
        </select>
      </div>
    </div>
    <div v-if="showDetailModal" class="modal">
      <div class="modal-content">
        <h2>Chi Tiết Nhân Viên</h2>
        <div class="detail-content">
          <div class="detail-image">
            <img
              v-if="selectedStaff.hinhAnhDuongDan"
              :src="getImageUrl(selectedStaff.hinhAnhDuongDan)"
              alt="Hình ảnh nhân viên"
              class="preview-image"
              @error="handleImageError"
            />
            <span v-else class="no-image">Không có ảnh</span>
          </div>
          <div class="detail-info">
            <p><strong>Mã NV:</strong> {{ selectedStaff.maNv }}</p>
            <p><strong>Họ Tên:</strong> {{ selectedStaff.hoTen }}</p>
            <p><strong>Giới Tính:</strong> {{ selectedStaff.gioiTinh }}</p>
            <p><strong>Ngày Sinh:</strong> {{ selectedStaff.ngaySinh || 'Chưa có' }}</p>
            <p><strong>Địa Chỉ:</strong> {{ selectedStaff.diaChi || 'Không có' }}</p>
            <p><strong>CCCD:</strong> {{ selectedStaff.cccd || 'Không có' }}</p>
            <p><strong>SĐT:</strong> {{ selectedStaff.sdt }}</p>
            <p><strong>Email:</strong> {{ selectedStaff.email }}</p>
            <p><strong>Ngày Vào Làm:</strong> {{ selectedStaff.ngayVaoLam }}</p>
            <p><strong>Tình Trạng:</strong> {{ selectedStaff.tinhTrang }}</p>
            <p><strong>Chức Vụ:</strong> {{ selectedStaff.maChucVu || 'Không có' }}</p>
            <p><strong>Tên Tài Khoản:</strong> {{ selectedStaff.tenTaiKhoan || 'Không có' }}</p>
          </div>
        </div>
        <div class="form-actions">
          <button type="button" @click="closeDetailModal">Đóng</button>
          <button type="button" @click="openEditModal(selectedStaff)" class="edit-btn">Sửa</button>
        </div>
      </div>
    </div>
    <div v-if="showForm" class="modal">
      <div class="modal-content">
        <h2>{{ isEditMode ? 'Sửa Nhân Viên' : 'Thêm Nhân Viên' }}</h2>
        <form @submit.prevent="isEditMode ? updateStaff() : addStaff()">
          <div class="form-group">
            <label>Họ Tên:</label>
            <input v-model="currentStaff.hoTen" required />
          </div>
          <div class="form-group">
            <label>Giới Tính:</label>
            <select v-model="currentStaff.gioiTinh" required>
              <option value="Nam">Nam</option>
              <option value="Nữ">Nữ</option>
            </select>
          </div>
          <div class="form-group">
            <label>Ngày Sinh:</label>
            <input type="date" v-model="currentStaff.ngaySinh" />
          </div>
          <div class="form-group">
            <label>Địa Chỉ:</label>
            <input v-model="currentStaff.diaChi" required />
          </div>
          <div class="form-group">
            <label>CCCD:</label>
            <input v-model="currentStaff.cccd" required />
          </div>
          <div class="form-group">
            <label>SĐT:</label>
            <input v-model="currentStaff.sdt" required />
          </div>
          <div class="form-group">
            <label>Email:</label>
            <input type="email" v-model="currentStaff.email" required />
          </div>
          <div class="form-group">
            <label>Ngày Vào Làm:</label>
            <input type="date" v-model="currentStaff.ngayVaoLam" required />
          </div>
          <div class="form-group" v-if="isEditMode">
            <label>Tình Trạng:</label>
            <select v-model="currentStaff.tinhTrang" required>
              <option value="Đang hoạt động">Đang hoạt động</option>
              <option value="Đã tạm khóa">Đã tạm khóa</option>
            </select>
          </div>
          <div class="form-group">
            <label>Mã Chức Vụ:</label>
            <input type="number" v-model="currentStaff.maChucVu" />
          </div>
          <div class="form-group">
            <label>Tên Tài Khoản:</label>
            <input v-model="currentStaff.tenTaiKhoan" :readonly="isEditMode" required />
          </div>
          <div class="form-group">
            <label>Mật Khẩu:</label>
            <input type="password" v-model="currentStaff.matKhau" />
          </div>
          <div class="form-group">
            <label>Hình Ảnh:</label>
            <input type="file" @change="handleFileUpload" accept="image/*" />
            <img
              v-if="currentStaff.hinhAnhDuongDan"
              :src="isEditMode && !currentStaff.hinhAnh ? getImageUrl(currentStaff.hinhAnhDuongDan) : currentStaff.hinhAnhDuongDan"
              alt="Hình ảnh nhân viên"
              class="preview-image"
              @error="handleImageError"
            />
          </div>
          <div class="form-actions">
            <button type="submit">{{ isEditMode ? 'Cập Nhật' : 'Thêm' }}</button>
            <button type="button" @click="closeForm">Hủy</button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios';
import Swal from 'sweetalert2';
import { GetApiUrl } from '@constants/api'
export default {
  data() {
    return {
      staffList: [],
      searchTerm: '',
      gioiTinh: '',
      tinhTrang: '',
      currentPage: 1,
      pageSize: 5,
      totalRecords: 0,
      totalPages: 0,
      isEditMode: false,
      showForm: false,
      showDetailModal: false,
      currentStaff: {
        maNv: 0,
        hoTen: '',
        gioiTinh: '',
        ngaySinh: '',
        diaChi: '',
        cccd: '',
        sdt: '',
        email: '',
        ngayVaoLam: '',
        tenTaiKhoan: '',
        matKhau: '',
        tinhTrang: 'Đang hoạt động',
        maChucVu: null,
        hinhAnh: null,
        hinhAnhDuongDan: '',
        isDelete: false,
      },
      selectedStaff: {},
      apiBaseUrl: GetApiUrl()+'/api', // URL API backend
      imageBaseUrl: GetApiUrl(), // URL để truy cập hình ảnh
      defaultImagePath: '/uploads/default-image.jpg', // Đường dẫn mặc định nếu không có ảnh
    };
  },
  computed: {
    paginatedStaffList() {
      const start = (this.currentPage - 1) * this.pageSize;
      const end = start + this.pageSize;
      return this.staffList.slice(start, end);
    },
    totalPages() {
      return Math.ceil(this.totalRecords / this.pageSize);
    },
  },
  mounted() {
    this.fetchStaff();
  },
  methods: {
    async fetchStaff() {
      try {
        const response = await axios.get(`${this.apiBaseUrl}/staff`, {
          params: {
            pageNumber: this.currentPage,
            pageSize: this.pageSize,
            search: this.searchTerm,
            gioiTinh: this.gioiTinh,
            tinhTrang: this.tinhTrang,
          },
        });
        this.staffList = response.data;
        this.totalRecords = response.data.length;
        this.totalPages = Math.ceil(this.totalRecords / this.pageSize);
      } catch (error) {
        console.error('Lỗi khi lấy danh sách nhân viên:', error);
        let errorMessage = 'Đã có lỗi xảy ra khi lấy danh sách nhân viên.';
        if (error.code === 'ERR_NETWORK') {
          errorMessage = 'Không thể kết nối đến backend. Vui lòng kiểm tra xem backend có đang chạy trên https://localhost:7139 không.';
        } else if (error.response) {
          errorMessage = error.response.data.message || errorMessage;
        }
        Swal.fire('Lỗi!', errorMessage, 'error');
      }
    },
    prevPage() {
      if (this.currentPage > 1) {
        this.currentPage--;
        this.fetchStaff();
      }
    },
    nextPage() {
      if (this.currentPage < this.totalPages) {
        this.currentPage++;
        this.fetchStaff();
      }
    },
    updatePageSize() {
      this.currentPage = 1;
      this.fetchStaff();
    },
    openAddModal() {
      this.isEditMode = false;
      this.showForm = true;
      this.currentStaff = {
        maNv: 0,
        hoTen: '',
        gioiTinh: '',
        ngaySinh: '',
        diaChi: '',
        cccd: '',
        sdt: '',
        email: '',
        ngayVaoLam: '',
        tenTaiKhoan: '',
        matKhau: '',
        tinhTrang: 'Đang hoạt động',
        maChucVu: null,
        hinhAnh: null,
        hinhAnhDuongDan: '',
        isDelete: false,
      };
    },
    openEditModal(staff) {
      this.isEditMode = true;
      this.showForm = true;
      this.currentStaff = { ...staff, hinhAnh: null };
      this.showDetailModal = false;
    },
    openDetailModal(staff) {
      this.selectedStaff = { ...staff };
      this.showDetailModal = true;
    },
    closeDetailModal() {
      this.showDetailModal = false;
      this.selectedStaff = {};
    },
    closeForm() {
      this.showForm = false;
      this.currentStaff = {
        maNv: 0,
        hoTen: '',
        gioiTinh: '',
        ngaySinh: '',
        diaChi: '',
        cccd: '',
        sdt: '',
        email: '',
        ngayVaoLam: '',
        tenTaiKhoan: '',
        matKhau: '',
        tinhTrang: 'Đang hoạt động',
        maChucVu: null,
        hinhAnh: null,
        hinhAnhDuongDan: '',
        isDelete: false,
      };
    },
    getImageUrl(imagePath) {
      // Tạo URL đầy đủ cho hình ảnh
      return imagePath ? `${this.imageBaseUrl}${imagePath}` : this.defaultImagePath;
    },
    handleImageError(event) {
      console.error('Không thể tải hình ảnh:', event.target.src); // Ghi log lỗi
      event.target.style.display = 'none';
      const span = document.createElement('span');
      span.className = 'no-image';
      span.textContent = 'Không có ảnh';
      event.target.parentNode.appendChild(span);
    },
    handleFileUpload(event) {
      const file = event.target.files[0];
      if (file) {
        this.currentStaff.hinhAnh = file;
        this.currentStaff.hinhAnhDuongDan = URL.createObjectURL(file); // Tạo URL tạm để hiển thị preview
      }
    },
    async addStaff() {
      try {
        if (!/^\d{12}$/.test(this.currentStaff.cccd)) {
          Swal.fire('Lỗi!', 'CCCD phải là 12 chữ số.', 'error');
          return;
        }
        if (!/^0\d{9,10}$/.test(this.currentStaff.sdt)) {
          Swal.fire('Lỗi!', 'Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.', 'error');
          return;
        }
        if (!/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(this.currentStaff.email)) {
          Swal.fire('Lỗi!', 'Email không hợp lệ.', 'error');
          return;
        }

        const formData = new FormData();
        formData.append('hoTen', this.currentStaff.hoTen.trim());
        formData.append('gioiTinh', this.currentStaff.gioiTinh.trim());
        if (this.currentStaff.ngaySinh) {
          formData.append('ngaySinh', this.currentStaff.ngaySinh);
        }
        formData.append('diaChi', this.currentStaff.diaChi.trim());
        formData.append('cccd', this.currentStaff.cccd.trim());
        formData.append('sdt', this.currentStaff.sdt.trim());
        formData.append('email', this.currentStaff.email.trim());
        formData.append('ngayVaoLam', this.currentStaff.ngayVaoLam);
        formData.append('tenTaiKhoan', this.currentStaff.tenTaiKhoan.trim());
        formData.append('matKhau', this.currentStaff.matKhau.trim());
        formData.append('tinhTrang', 'Đang hoạt động');
        if (this.currentStaff.maChucVu) {
          formData.append('maChucVu', this.currentStaff.maChucVu);
        }
        if (this.currentStaff.hinhAnh) {
          formData.append('hinhAnh', this.currentStaff.hinhAnh);
        }

        const response = await axios.post(`${this.apiBaseUrl}/staff`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' },
        });
        Swal.fire('Thành công!', 'Thêm nhân viên thành công.', 'success');
        this.closeForm();
        this.fetchStaff();
      } catch (error) {
        console.error('Lỗi khi thêm nhân viên:', error);
        let errorMessage = 'Đã có lỗi xảy ra khi thêm nhân viên.';
        if (error.response && error.response.data) {
          if (typeof error.response.data === 'string') {
            errorMessage = error.response.data;
          } else if (error.response.data.errors) {
            const errors = error.response.data.errors;
            errorMessage = Object.keys(errors)
              .map(key => `${key}: ${errors[key].join(', ')}`)
              .join('\n');
          } else if (error.response.data.message) {
            errorMessage = error.response.data.message;
          }
        } else if (error.code === 'ERR_NETWORK') {
          errorMessage = 'Không thể kết nối đến backend. Vui lòng kiểm tra xem backend có đang chạy trên https://localhost:7139 không.';
        }
        Swal.fire('Lỗi!', errorMessage, 'error');
      }
    },
    async updateStaff() {
      try {
        if (!/^\d{12}$/.test(this.currentStaff.cccd)) {
          Swal.fire('Lỗi!', 'CCCD phải là 12 chữ số.', 'error');
          return;
        }
        if (!/^0\d{9,10}$/.test(this.currentStaff.sdt)) {
          Swal.fire('Lỗi!', 'Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.', 'error');
          return;
        }
        if (!/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(this.currentStaff.email)) {
          Swal.fire('Lỗi!', 'Email không hợp lệ.', 'error');
          return;
        }
        if (!['Đang hoạt động', 'Đã tạm khóa'].includes(this.currentStaff.tinhTrang)) {
          Swal.fire('Lỗi!', "Tình trạng chỉ được phép là 'Đang hoạt động' hoặc 'Đã tạm khóa'.", 'error');
          return;
        }

        const formData = new FormData();
        formData.append('maNv', this.currentStaff.maNv);
        formData.append('hoTen', this.currentStaff.hoTen.trim());
        formData.append('gioiTinh', this.currentStaff.gioiTinh.trim());
        if (this.currentStaff.ngaySinh) {
          formData.append('ngaySinh', this.currentStaff.ngaySinh);
        }
        formData.append('diaChi', this.currentStaff.diaChi.trim());
        formData.append('cccd', this.currentStaff.cccd.trim());
        formData.append('sdt', this.currentStaff.sdt.trim());
        formData.append('email', this.currentStaff.email.trim());
        formData.append('ngayVaoLam', this.currentStaff.ngayVaoLam);
        formData.append('tenTaiKhoan', this.currentStaff.tenTaiKhoan.trim());
        formData.append('tinhTrang', this.currentStaff.tinhTrang.trim());
        if (this.currentStaff.matKhau) {
          formData.append('matKhau', this.currentStaff.matKhau.trim());
        }
        if (this.currentStaff.maChucVu) {
          formData.append('maChucVu', this.currentStaff.maChucVu);
        }
        if (this.currentStaff.hinhAnh) {
          formData.append('hinhAnh', this.currentStaff.hinhAnh);
        }

        const response = await axios.put(`${this.apiBaseUrl}/staff/${this.currentStaff.maNv}`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' },
        });
        Swal.fire('Thành công!', 'Cập nhật nhân viên thành công.', 'success');
        this.closeForm();
        this.fetchStaff();
      } catch (error) {
        console.error('Lỗi khi cập nhật nhân viên:', error);
        let errorMessage = 'Đã có lỗi xảy ra khi cập nhật nhân viên.';
        if (error.response && error.response.data) {
          if (typeof error.response.data === 'string') {
            errorMessage = error.response.data;
          } else if (error.response.data.errors) {
            const errors = error.response.data.errors;
            errorMessage = Object.keys(errors)
              .map(key => `${key}: ${errors[key].join(', ')}`)
              .join('\n');
          } else if (error.response.data.message) {
            errorMessage = error.response.data.message;
          }
        } else if (error.code === 'ERR_NETWORK') {
          errorMessage = 'Không thể kết nối đến backend. Vui lòng kiểm tra xem backend có đang chạy trên https://localhost:7139 không.';
        }
        Swal.fire('Lỗi!', errorMessage, 'error');
      }
    },
    async toggleDelete(id) {
      if (!confirm('Bạn có chắc chắn muốn ẩn nhân viên này?')) return;
      try {
        const response = await axios.delete(`${this.apiBaseUrl}/staff/${id}`);
        Swal.fire('Thành công!', 'Ẩn nhân viên thành công.', 'success');
        this.fetchStaff();
      } catch (error) {
        console.error('Lỗi khi ẩn nhân viên:', error);
        let errorMessage = 'Đã có lỗi xảy ra khi ẩn nhân viên.';
        if (error.response && error.response.data) {
          if (typeof error.response.data === 'string') {
            errorMessage = error.response.data;
          } else if (error.response.data.message) {
            errorMessage = error.response.data.message;
          } else if (error.response.data.errors) {
            const errors = error.response.data.errors;
            errorMessage = Object.keys(errors)
              .map(key => `${key}: ${errors[key].join(', ')}`)
              .join('\n');
          }
        } else if (error.code === 'ERR_NETWORK') {
          errorMessage = 'Không thể kết nối đến backend. Vui lòng kiểm tra xem backend có đang chạy trên https://localhost:7139 không.';
        }
        Swal.fire('Lỗi!', errorMessage, 'error');
      }
    },
  },
};
</script>

<style scoped>
.staff-page {
  padding: 20px;
  max-width: 1600px;
  margin: 0 auto;
  font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
}

h1 {
  text-align: left;
  color: #333;
  margin-bottom: 20px;
  font-size: 1.5rem;
  font-weight: 600;
}

.filter-section {
  display: flex;
  gap: 10px;
  margin-bottom: 15px;
  flex-wrap: wrap;
}

.search-input,
select {
  padding: 8px;
  border: 1px solid #ccc;
  border-radius: 4px;
  font-size: 14px;
  transition: border-color 0.3s ease;
}

.search-input:focus,
select:focus {
  border-color: #007bff;
  outline: none;
}

.add-btn,
.edit-btn,
.delete-btn,
.detail-btn,
.form-actions button {
  padding: 6px 12px;
  border: none;
  border-radius: 4px;
  cursor: pointer;
  font-size: 14px;
  transition: background-color 0.3s ease;
}

.add-btn {
  background-color: #28a745;
  color: white;
  margin-bottom: 15px;
  display: inline-block;
}

.add-btn:hover {
  background-color: #218838;
}

.detail-btn {
  background-color: #17a2b8;
  color: white;
  margin-right: 5px;
}

.detail-btn:hover {
  background-color: #138496;
}

.edit-btn {
  background-color: #007bff;
  color: white;
  margin-right: 5px;
}

.edit-btn:hover {
  background-color: #0056b3;
}

.delete-btn {
  background-color: #dc3545;
  color: white;
}

.delete-btn:hover {
  background-color: #c82333;
}

.staff-table {
  width: 100%;
  border-collapse: collapse;
  background-color: #fff;
  border: 1px solid #dee2e6;
}

.staff-table th,
.staff-table td {
  padding: 10px;
  text-align: left;
  border: 1px solid #dee2e6;
}

.staff-table th {
  background-color: #f8f9fa;
  color: #333;
  font-weight: 600;
  font-size: 14px;
}

.staff-table td {
  font-size: 14px;
  color: #333;
}

.staff-table tr:nth-child(even) {
  background-color: #f9f9f9;
}

.staff-table tr:hover {
  background-color: #f1f1f1;
}

.staff-image {
  width: 40px;
  height: 40px;
  object-fit: cover;
  border-radius: 4px;
}

.no-image {
  color: #dc3545;
  font-size: 12px;
}

.action-buttons {
  display: flex;
  gap: 5px;
  white-space: nowrap;
}

.pagination {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 15px;
  padding: 10px 0;
}

.pagination-controls button {
  padding: 6px 12px;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  background-color: #fff;
  color: #333;
  cursor: pointer;
  transition: background-color 0.3s ease;
}

.pagination-controls button:disabled {
  background-color: #e9ecef;
  cursor: not-allowed;
}

.pagination-controls button:hover:not(:disabled) {
  background-color: #e9ecef;
}

.pagination-controls span {
  font-size: 14px;
  color: #333;
}

.pagination-size {
  display: flex;
  align-items: center;
  gap: 10px;
}

.pagination-size label {
  font-size: 14px;
  color: #333;
}

.pagination-size select {
  padding: 6px;
  border-radius: 4px;
  border: 1px solid #dee2e6;
}

.modal {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 1000;
}

.modal-content {
  background: white;
  padding: 20px;
  border-radius: 8px;
  width: 500px;
  max-height: 80vh;
  overflow-y: auto;
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
}

.detail-content {
  display: flex;
  gap: 15px;
  margin-bottom: 15px;
}

.detail-image {
  flex: 1;
  text-align: center;
}

.detail-info {
  flex: 2;
}

.detail-info p {
  margin: 6px 0;
  font-size: 14px;
  color: #333;
}

.detail-info p strong {
  color: #333;
}

.form-group {
  margin-bottom: 15px;
}

.form-group label {
  display: block;
  margin-bottom: 6px;
  font-weight: 500;
  color: #333;
}

.form-group input,
.form-group select {
  width: 100%;
  padding: 8px;
  border: 1px solid #dee2e6;
  border-radius: 4px;
  font-size: 14px;
}

.form-group input:focus,
.form-group select:focus {
  border-color: #007bff;
  outline: none;
}

.preview-image {
  width: 100px;
  height: 100px;
  margin-top: 8px;
  object-fit: cover;
  border-radius: 4px;
  border: 1px solid #dee2e6;
}

.form-actions {
  display: flex;
  gap: 10px;
  justify-content: flex-end;
  margin-top: 20px;
}

.form-actions button[type='submit'] {
  background-color: #28a745;
  color: white;
}

.form-actions button[type='submit']:hover {
  background-color: #218838;
}

.form-actions button[type='button'] {
  background-color: #6c757d;
  color: white;
}

.form-actions button[type='button']:hover {
  background-color: #5a6268;
}

@media (max-width: 768px) {
  .filter-section {
    flex-direction: column;
  }

  .staff-table {
    font-size: 12px;
  }

  .modal-content {
    width: 90%;
  }

  .pagination {
    flex-direction: column;
    gap: 10px;
  }

  .detail-content {
    flex-direction: column;
    align-items: center;
  }
}
</style>