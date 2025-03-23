<template>
    <br>
    <div class="staff-page" style="width: 1600px;">
      <h1>Quản Lý Nhân Viên</h1>
  
      <!-- Form tìm kiếm và lọc -->
      <div class="filter-section">
        <input
          v-model="searchQuery"
          placeholder="Tìm kiếm theo tên..."
          @input="fetchStaffList"
          class="search-input"
        />
        <select v-model="gioiTinhFilter" @change="fetchStaffList">
          <option value="">Tất cả giới tính</option>
          <option value="nam">Nam</option>
          <option value="nu">Nữ</option>
        </select>
        <select v-model="tinhTrangFilter" @change="fetchStaffList">
          <option value="">Tất cả tình trạng</option>
          <option value="dang lam">Đang làm</option>
          <option value="nghi viec">Nghỉ việc</option>
        </select>
      </div>
  
      <!-- Nút mở form thêm nhân viên -->
      <button @click="openAddForm" class="add-btn">Thêm Nhân Viên</button>
  
      <!-- Bảng danh sách nhân viên -->
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
            <td>{{ staff.hoTen }}</td>
            <td>{{ staff.gioiTinh }}</td>
            <td>{{ staff.ngaySinh ? staff.ngaySinh.toString() : '' }}</td>
            <td>{{ staff.diaChi }}</td>
            <td>{{ staff.cccd }}</td>
            <td>{{ staff.sdt }}</td>
            <td>{{ staff.email }}</td>
            <td>{{ staff.ngayVaoLam.toString() }}</td>
            <td>{{ staff.tinhTrang }}</td>
            <td>{{ staff.maChucVu }}</td>
            <td>
              <img
                v-if="getImagePath(staff.maNv)"
                :src="getImagePath(staff.maNv)"
                alt="Hình ảnh nhân viên"
                class="staff-image"
                @error="handleImageError"
              />
              <span v-else class="no-image">Không có ảnh</span>
            </td>
            <td class="action-buttons">
              <button @click="openDetailModal(staff)" class="detail-btn">Chi Tiết</button>
              <button @click="openEditForm(staff)" class="edit-btn">Sửa</button>
              <button @click="toggleDeleteStaff(staff.maNv)" class="delete-btn">Ẩn</button>
            </td>
          </tr>
        </tbody>
      </table>
  
      <!-- Phân trang -->
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
  
      <!-- Modal xem chi tiết nhân viên -->
      <div v-if="showDetailModal" class="modal">
        <div class="modal-content">
          <h2>Chi Tiết Nhân Viên</h2>
          <div class="detail-content">
            <div class="detail-image">
              <img
                v-if="getImagePath(selectedStaff.maNv)"
                :src="getImagePath(selectedStaff.maNv)"
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
              <p><strong>Ngày Sinh:</strong> {{ selectedStaff.ngaySinh ? selectedStaff.ngaySinh.toString() : '' }}</p>
              <p><strong>Địa Chỉ:</strong> {{ selectedStaff.diaChi || 'Không có' }}</p>
              <p><strong>CCCD:</strong> {{ selectedStaff.cccd || 'Không có' }}</p>
              <p><strong>SĐT:</strong> {{ selectedStaff.sdt }}</p>
              <p><strong>Email:</strong> {{ selectedStaff.email }}</p>
              <p><strong>Ngày Vào Làm:</strong> {{ selectedStaff.ngayVaoLam.toString() }}</p>
              <p><strong>Tình Trạng:</strong> {{ selectedStaff.tinhTrang }}</p>
              <p><strong>Chức Vụ:</strong> {{ selectedStaff.maChucVu || 'Không có' }}</p>
              <p><strong>Tên Tài Khoản:</strong> {{ selectedStaff.tenTaiKhoan || 'Không có' }}</p>
            </div>
          </div>
          <div class="form-actions">
            <button type="button" @click="closeDetailModal">Đóng</button>
          </div>
        </div>
      </div>
  
      <!-- Form thêm/sửa nhân viên -->
      <div v-if="showForm" class="modal">
        <div class="modal-content">
          <h2>{{ isEditMode ? 'Sửa Nhân Viên' : 'Thêm Nhân Viên' }}</h2>
          <form @submit.prevent="submitForm">
            <div class="form-group">
              <label>Họ Tên:</label>
              <input v-model="formData.hoTen" required />
            </div>
            <div class="form-group">
              <label>Giới Tính:</label>
              <select v-model="formData.gioiTinh" required>
                <option value="nam">Nam</option>
                <option value="nu">Nữ</option>
              </select>
            </div>
            <div class="form-group">
              <label>Ngày Sinh:</label>
              <input
                type="date"
                v-model="formData.ngaySinh"
                @change="convertNgaySinh"
              />
            </div>
            <div class="form-group">
              <label>Địa Chỉ:</label>
              <input v-model="formData.diaChi" />
            </div>
            <div class="form-group">
              <label>CCCD:</label>
              <input v-model="formData.cccd" />
            </div>
            <div class="form-group">
              <label>SĐT:</label>
              <input v-model="formData.sdt" required />
            </div>
            <div class="form-group">
              <label>Email:</label>
              <input type="email" v-model="formData.email" required />
            </div>
            <div class="form-group">
              <label>Ngày Vào Làm:</label>
              <input
                type="date"
                v-model="formData.ngayVaoLam"
                @change="convertNgayVaoLam"
                required
              />
            </div>
            <div class="form-group">
              <label>Tình Trạng:</label>
              <select v-model="formData.tinhTrang">
                <option value="dang lam">Đang làm</option>
                <option value="nghi viec">Nghỉ việc</option>
              </select>
            </div>
            <div class="form-group">
              <label>Mã Chức Vụ:</label>
              <input type="number" v-model="formData.maChucVu" />
            </div>
            <div class="form-group">
              <label>Tên Tài Khoản:</label>
              <input v-model="formData.tenTaiKhoan" />
            </div>
            <div class="form-group">
              <label>Mật Khẩu:</label>
              <input type="password" v-model="formData.matKhau" required />
            </div>
            <div class="form-group">
              <label>Hình Ảnh:</label>
              <input type="file" @change="onFileChange" accept="image/*" />
              <img
                v-if="formData.hinhAnhDuongDan"
                :src="formData.hinhAnhDuongDan"
                alt="Hình ảnh nhân viên"
                class="preview-image"
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
  
  export default {
    name: 'Staff',
    data() {
      return {
        staffList: [],
        searchQuery: '',
        gioiTinhFilter: '',
        tinhTrangFilter: '',
        showForm: false,
        isEditMode: false,
        showDetailModal: false,
        selectedStaff: null,
        currentPage: 1,
        pageSize: 5,
        formData: {
          maNv: null,
          hoTen: '',
          gioiTinh: 'nam',
          ngaySinh: '',
          diaChi: '',
          cccd: '',
          sdt: '',
          email: '',
          ngayVaoLam: '',
          tenTaiKhoan: '',
          matKhau: '',
          tinhTrang: 'dang lam',
          maChucVu: null,
          hinhAnh: null,
          hinhAnhDuongDan: '',
        },
      };
    },
    computed: {
      paginatedStaffList() {
        const start = (this.currentPage - 1) * this.pageSize;
        const end = start + this.pageSize;
        return this.staffList.slice(start, end);
      },
      totalPages() {
        return Math.ceil(this.staffList.length / this.pageSize);
      },
    },
    mounted() {
      this.fetchStaffList();
    },
    methods: {
      getImagePath(maNv) {
        try {
          let imagePath = `/HinhAnh/AnhNhanVien/NhanVien${maNv}.png`;
          if (maNv >= 19) {
            imagePath = `/HinhAnh/AnhNhanVien/NhanVien${maNv}.jpg`;
          }
          return imagePath;
        } catch (error) {
          console.error(`Không thể tạo đường dẫn ảnh cho nhân viên ${maNv}:`, error);
          return null;
        }
      },
      handleImageError(event) {
        event.target.style.display = 'none';
        event.target.nextElementSibling.style.display = 'block';
      },
      openDetailModal(staff) {
        this.selectedStaff = { ...staff };
        this.showDetailModal = true;
      },
      closeDetailModal() {
        this.showDetailModal = false;
        this.selectedStaff = null;
      },
      prevPage() {
        if (this.currentPage > 1) {
          this.currentPage--;
        }
      },
      nextPage() {
        if (this.currentPage < this.totalPages) {
          this.currentPage++;
        }
      },
      updatePageSize() {
        this.currentPage = 1;
      },
      async fetchStaffList() {
        try {
          const params = {
            search: this.searchQuery,
            gioiTinh: this.gioiTinhFilter,
            tinhTrang: this.tinhTrangFilter,
          };
          const response = await axios.get('https://localhost:7139/api/Staff', {
            params,
          });
          console.log('Dữ liệu từ API:', response.data);
          this.staffList = response.data;
          this.currentPage = 1;
        } catch (error) {
          console.error('Lỗi khi lấy danh sách nhân viên:', error);
          alert('Không thể lấy danh sách nhân viên. Vui lòng thử lại.');
        }
      },
      openAddForm() {
        this.isEditMode = false;
        this.resetForm();
        this.showForm = true;
      },
      openEditForm(staff) {
        this.isEditMode = true;
        this.formData = { ...staff };
        this.formData.ngaySinh = staff.ngaySinh
          ? new Date(staff.ngaySinh).toISOString().split('T')[0]
          : '';
        this.formData.ngayVaoLam = staff.ngayVaoLam
          ? new Date(staff.ngayVaoLam).toISOString().split('T')[0]
          : '';
        this.showForm = true;
      },
      closeForm() {
        this.showForm = false;
        this.resetForm();
      },
      resetForm() {
        this.formData = {
          maNv: null,
          hoTen: '',
          gioiTinh: 'nam',
          ngaySinh: '',
          diaChi: '',
          cccd: '',
          sdt: '',
          email: '',
          ngayVaoLam: '',
          tenTaiKhoan: '',
          matKhau: '',
          tinhTrang: 'dang lam',
          maChucVu: null,
          hinhAnh: null,
          hinhAnhDuongDan: '',
        };
      },
      onFileChange(event) {
        const file = event.target.files[0];
        if (file) {
          this.formData.hinhAnh = file;
          this.formData.hinhAnhDuongDan = URL.createObjectURL(file);
        }
      },
      convertNgaySinh() {
        if (this.formData.ngaySinh) {
          this.formData.ngaySinh = new Date(this.formData.ngaySinh)
            .toISOString()
            .split('T')[0];
        }
      },
      convertNgayVaoLam() {
        if (this.formData.ngayVaoLam) {
          this.formData.ngayVaoLam = new Date(this.formData.ngayVaoLam)
            .toISOString()
            .split('T')[0];
        }
      },
      async submitForm() {
        try {
          const formData = new FormData();
          formData.append('hoTen', this.formData.hoTen);
          formData.append('gioiTinh', this.formData.gioiTinh);
          if (this.formData.ngaySinh)
            formData.append('ngaySinh', this.formData.ngaySinh);
          formData.append('diaChi', this.formData.diaChi || '');
          formData.append('cccd', this.formData.cccd || '');
          formData.append('sdt', this.formData.sdt);
          formData.append('email', this.formData.email);
          formData.append('ngayVaoLam', this.formData.ngayVaoLam);
          formData.append('tenTaiKhoan', this.formData.tenTaiKhoan || '');
          formData.append('matKhau', this.formData.matKhau);
          formData.append('tinhTrang', this.formData.tinhTrang);
          if (this.formData.maChucVu)
            formData.append('maChucVu', this.formData.maChucVu);
          if (this.formData.hinhAnh)
            formData.append('hinhAnh', this.formData.hinhAnh);
  
          if (this.isEditMode) {
            const response = await axios.put(
              `https://localhost:7296/api/Staff/${this.formData.maNv}`,
              formData,
              {
                headers: { 'Content-Type': 'multipart/form-data' },
              }
            );
            alert('Cập nhật nhân viên thành công!');
          } else {
            const response = await axios.post(
              'https://localhost:7296/api/Staff',
              formData,
              {
                headers: { 'Content-Type': 'multipart/form-data' },
              }
            );
            alert('Thêm nhân viên thành công!');
          }
          this.closeForm();
          this.fetchStaffList();
        } catch (error) {
          console.error('Lỗi khi lưu nhân viên:', error);
          alert(
            'Không thể lưu nhân viên. Lỗi: ' +
              (error.response?.data || error.message)
          );
        }
      },
      async toggleDeleteStaff(id) {
        if (confirm('Bạn có chắc chắn muốn ẩn nhân viên này?')) {
          try {
            await axios.patch(
              `https://localhost:7296/api/Staff/${id}/toggle-delete`
            );
            alert('Ẩn nhân viên thành công!');
            this.fetchStaffList();
          } catch (error) {
            console.error('Lỗi khi ẩn nhân viên:', error);
            alert('Không thể ẩn nhân viên. Vui lòng thử lại.');
          }
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
  
  /* Filter Section */
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
  
  /* Buttons */
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
  
  /* Table */
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
  
  /* Pagination */
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
  
  /* Modal */
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
  
  /* Detail Modal */
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
  
  /* Form */
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
  
  /* Form Actions */
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
  
  /* Responsive */
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
  