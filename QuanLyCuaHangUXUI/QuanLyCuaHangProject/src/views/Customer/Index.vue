<template>
  <div class="main-content">
    <div class="container mt-5">
      <h2 class="mb-4 text-primary mt-5">Quản Lý Khách Hàng</h2>

      <!-- Thanh tìm kiếm, lọc và sắp xếp -->
      <div class="row mb-4 align-items-center">
        <div class="col-md-4">
          <div class="input-group">
            <span class="input-group-text bg-white border-end-0">
              <i class="bi bi-search"></i>
            </span>
            <input
              type="text"
              class="form-control custom-input border-start-0"
              v-model="searchTerm"
              placeholder="Tìm kiếm theo họ tên, SĐT, email..."
              @input="fetchCustomers"
            />
          </div>
        </div>
        <div class="col-md-3">
          <select class="form-select custom-input" v-model="sortBy" @change="fetchCustomers">
            <option value="">Sắp xếp mặc định</option>
            <option value="gioitinh">Theo giới tính</option>
            <option value="tinhtrang">Theo tình trạng</option>
            <option value="a-z">Theo họ tên (A-Z)</option>
          </select>
        </div>
        <div class="col-md-5 text-end">
          <button class="btn btn-primary custom-btn me-2" @click="openAddModal">
            <i class="bi bi-person-plus me-1"></i> Thêm Khách Hàng
          </button>
          <button class="btn btn-success custom-btn me-2" @click="exportToExcel">
            <i class="bi bi-file-earmark-excel me-1"></i> Export Excel
          </button>
          <input
            type="file"
            ref="fileInput"
            style="display: none"
            accept=".xlsx"
            @change="importFromExcel"
          />
          <button class="btn btn-info custom-btn" @click="$refs.fileInput.click()">
            <i class="bi bi-file-earmark-arrow-up me-1"></i> Import Excel
          </button>
        </div>
      </div>

      <!-- Bảng danh sách khách hàng -->
      <div class="table-container">
        <table class="table table-hover custom-table">
          <thead>
            <tr>
              <th>Hình Đại Diện</th>
              <th>Họ Tên</th>
              <th>Giới Tính</th>
              <th>Số Điện Thoại</th>
              <th>Email</th>
              <th>Tình Trạng</th>
              <th>Hành Động</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="customer in customers" :key="customer.maKh">
              <td>
                <img
                  v-if="customer.hinhDaiDien"
                  :src="getApiUrl + customer.hinhDaiDien"
                  alt="Hình đại diện"
                  class="avatar-table-img"
                  @click="openDetailModal(customer)"
                  style="height: 100px;width: 100px;"
                />
                <img
                  v-else
                  src="https://via.placeholder.com/50x50?text=No+Image"
                  alt="Hình Ảnh"
                  class="avatar-table-img"
                  @click="openDetailModal(customer)"
                  style="height: 100px;width: 100px;"
                />
              </td>
              <td>
                <a href="#" @click.prevent="openDetailModal(customer)" class="text-primary fw-semibold">{{ customer.hoTen }}</a>
              </td>
              <td>{{ customer.gioiTinh }}</td>
              <td>{{ customer.sdt }}</td>
              <td>{{ customer.email }}</td>
              <td>
                <span :class="{
                  'badge bg-success': customer.tinhTrang === 'Đang hoạt động',
                  'badge bg-warning': customer.tinhTrang === 'Đã tạm khóa'
                }">
                  {{ customer.tinhTrang }}
                </span>
              </td>
              <td>
                <button class="btn btn-warning btn-sm custom-action-btn me-2" @click="openEditModal(customer)">
                  <i class="bi bi-pencil"></i> Sửa
                </button>
                <button class="btn btn-danger btn-sm custom-action-btn" @click="hideCustomer(customer.maKh)">
                  <i class="bi bi-eye-slash"></i> Ẩn
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- Phân trang -->
      <nav class="mt-4">
        <ul class="pagination justify-content-center custom-pagination">
          <li class="page-item" :class="{ disabled: currentPage === 1 }">
            <a class="page-link" href="#" @click.prevent="changePage(currentPage - 1)">Sau</a>
          </li>
          <li
            class="page-item"
            v-for="page in totalPages"
            :key="page"
            :class="{ active: currentPage === page }"
          >
            <a class="page-link" href="#" @click.prevent="changePage(page)">{{ page }}</a>
          </li>
          <li class="page-item" :class="{ disabled: currentPage === totalPages }">
            <a class="page-link" href="#" @click.prevent="changePage(currentPage + 1)">Tiếp</a>
          </li>
        </ul>
      </nav>

      <!-- Modal thêm/sửa khách hàng -->
      <div class="modal fade" id="customerModal" tabindex="-1" aria-labelledby="customerModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl">
          <div class="modal-content custom-modal-content">
            <div class="modal-header custom-modal-header">
              <h5 class="modal-title" id="customerModalLabel">{{ isEditMode ? 'Sửa Khách Hàng' : 'Thêm Khách Hàng' }}</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="isEditMode ? updateCustomer() : addCustomer()">
                <div class="row">
                  <!-- Cột hình đại diện -->
                  <div class="col-md-3 text-center">
                    <div class="avatar-container">
                      <img
                        v-if="currentCustomer.hinhDaiDien"
                        :src="isEditMode && !currentCustomer.anh ? getApiUrl+currentCustomer.hinhDaiDien : currentCustomer.hinhDaiDien"
                        alt="Hình đại diện"
                        class="avatar-img"
                      />
                      <img
                        v-else
                        src="https://via.placeholder.com/200x200?text=No+Image"
                        alt="Hình đại diện mặc định"
                        class="avatar-img"
                      />
                    </div>
                    <div class="mb-3">
                      <label for="anh" class="form-label">Hình Đại Diện</label>
                      <input
                        type="file"
                        class="form-control custom-input"
                        id="anh"
                        @change="handleFileUpload"
                        accept="image/*"
                        :required="!isEditMode || !currentCustomer.hinhDaiDien"
                      />
                    </div>
                  </div>
                  <!-- Cột thông tin nhập liệu -->
                  <div class="col-md-9">
                    <div class="row">
                      <div class="col-md-6">
                        <div class="mb-3">
                          <label for="hoTen" class="form-label">Họ Tên</label>
                          <input type="text" class="form-control custom-input" id="hoTen" v-model="currentCustomer.hoTen" required />
                        </div>
                        <div class="mb-3">
                          <label for="gioiTinh" class="form-label">Giới Tính</label>
                          <select class="form-select custom-input" id="gioiTinh" v-model="currentCustomer.gioiTinh" required>
                            <option value="Nam">Nam</option>
                            <option value="Nữ">Nữ</option>
                            <option value="Khác">Khác</option>
                          </select>
                        </div>
                        <div class="mb-3">
                          <label for="ngaySinh" class="form-label">Ngày Sinh</label>
                          <input type="date" class="form-control custom-input" id="ngaySinh" v-model="currentCustomer.ngaySinh" />
                        </div>
                        <div class="mb-3">
                          <label for="diaChi" class="form-label">Địa Chỉ</label>
                          <input type="text" class="form-control custom-input" id="diaChi" v-model="currentCustomer.diaChi" required />
                        </div>
                        <div class="mb-3">
                          <label for="cccd" class="form-label">CCCD</label>
                          <input type="text" class="form-control custom-input" id="cccd" v-model="currentCustomer.cccd" required />
                        </div>
                      </div>
                      <div class="col-md-6">
                        <div class="mb-3">
                          <label for="sdt" class="form-label">Số Điện Thoại</label>
                          <input type="text" class="form-control custom-input" id="sdt" v-model="currentCustomer.sdt" required />
                        </div>
                        <div class="mb-3">
                          <label for="email" class="form-label">Email</label>
                          <input type="email" class="form-control custom-input" id="email" v-model="currentCustomer.email" />
                        </div>
                        <div class="mb-3">
                          <label for="tenTaiKhoan" class="form-label">Tên Tài Khoản</label>
                          <input
                            type="text"
                            class="form-control custom-input"
                            id="tenTaiKhoan"
                            v-model="currentCustomer.tenTaiKhoan"
                            :readonly="isEditMode"
                            required
                          />
                        </div>
                        <div class="mb-3" v-if="!isEditMode">
                          <label for="matKhau" class="form-label">Mật Khẩu</label>
                          <input
                            type="password"
                            class="form-control custom-input"
                            id="matKhau"
                            v-model="currentCustomer.matKhau"
                            required
                          />
                        </div>
                        <div class="mb-3">
                          <label for="tinhTrang" class="form-label">Tình Trạng</label>
                          <select class="form-select custom-input" id="tinhTrang" v-model="currentCustomer.tinhTrang" required>
                            <option value="Đang hoạt động">Đang hoạt động</option>
                            <option value="Đã tạm khóa">Đã tạm khóa</option>
                          </select>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="text-end mt-3">
                  <button type="submit" class="btn btn-primary custom-submit-btn">{{ isEditMode ? 'Cập Nhật' : 'Thêm' }}</button>
                </div>
              </form>
            </div>
            <div class="modal-footer custom-modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
            </div>
          </div>
        </div>
      </div>

      <!-- Modal chi tiết khách hàng -->
      <div class="modal fade" id="detailModal" tabindex="-1" aria-labelledby="detailModalLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl">
          <div class="modal-content custom-modal-content">
            <div class="modal-header custom-modal-header">
              <h5 class="modal-title" id="detailModalLabel">Chi Tiết Khách Hàng</h5>
              <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body">
              <div class="row">
                <!-- Cột hình đại diện -->
                <div class="col-md-3 text-center">
                  <div class="avatar-container">
                    <img
                      v-if="selectedCustomer.hinhDaiDien"
                      :src="getApiUrl+ selectedCustomer.hinhDaiDien"
                      alt="Hình đại diện"
                      class="avatar-img"
                    />
                    <img
                      v-else
                      src="https://via.placeholder.com/200x200?text=No+Image"
                      alt="Hình đại diện mặc định"
                      class="avatar-img"
                    />
                  </div>
                  <h5 class="mt-3">{{ selectedCustomer.hoTen }}</h5>
                  <p class="text-muted">{{ selectedCustomer.tinhTrang }}</p>
                </div>
                <!-- Cột thông tin chi tiết -->
                <div class="col-md-9">
                  <div class="row">
                    <div class="col-md-6">
                      <div class="info-item">
                        <strong>Giới Tính:</strong>
                        <span>{{ selectedCustomer.gioiTinh }}</span>
                      </div>
                      <div class="info-item">
                        <strong>Ngày Sinh:</strong>
                        <span>{{ selectedCustomer.ngaySinh || 'Chưa có' }}</span>
                      </div>
                      <div class="info-item">
                        <strong>Địa Chỉ:</strong>
                        <span>{{ selectedCustomer.diaChi }}</span>
                      </div>
                      <div class="info-item">
                        <strong>CCCD:</strong>
                        <span>{{ selectedCustomer.cccd }}</span>
                      </div>
                    </div>
                    <div class="col-md-6">
                      <div class="info-item">
                        <strong>Số Điện Thoại:</strong>
                        <span>{{ selectedCustomer.sdt }}</span>
                      </div>
                      <div class="info-item">
                        <strong>Email:</strong>
                        <span>{{ selectedCustomer.email || 'Chưa có' }}</span>
                      </div>
                      <div class="info-item">
                        <strong>Tên Tài Khoản:</strong>
                        <span>{{ selectedCustomer.tenTaiKhoan }}</span>
                      </div>
                      <div class="info-item">
                        <strong>Ngày Tạo:</strong>
                        <span>{{ selectedCustomer.ngayTao }}</span>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <div class="modal-footer custom-modal-footer">
              <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Đóng</button>
              <button type="button" class="btn btn-primary" @click="openEditModal(selectedCustomer)">Sửa</button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import axios from 'axios'
import { Modal } from 'bootstrap'
import Swal from 'sweetalert2'
import { GetApiUrl } from '@constants/api'
let getApiUrl = GetApiUrl()
export default {
  data() {
    return {
      customers: [],
      searchTerm: '',
      sortBy: '',
      currentPage: 1,
      pageSize: 10,
      totalRecords: 0,
      totalPages: 0,
      isEditMode: false,
      currentCustomer: {
        maKh: 0,
        hoTen: '',
        gioiTinh: '',
        ngaySinh: '',
        diaChi: '',
        cccd: '',
        sdt: '',
        email: '',
        tenTaiKhoan: '',
        matKhau: '',
        hinhDaiDien: '',
        tinhTrang: 'Đang hoạt động', // Giá trị mặc định là "Đang hoạt động"
        isDelete: false,
        anh: null
      },
      selectedCustomer: {},
      modal: null,
      detailModal: null
    }
  },
  mounted() {
    this.fetchCustomers()
    this.modal = new Modal(document.getElementById('customerModal'))
    this.detailModal = new Modal(document.getElementById('detailModal'))
  },
  methods: {
    async fetchCustomers() {
      try {
        const response = await axios.get(getApiUrl+'/api/customer', {
          params: {
            pageNumber: this.currentPage,
            pageSize: this.pageSize,
            searchTerm: this.searchTerm,
            sortBy: this.sortBy
          }
        })
        this.customers = response.data.data
        this.totalRecords = response.data.totalRecords
        this.totalPages = Math.ceil(this.totalRecords / this.pageSize)
      } catch (error) {
        console.error('Lỗi khi lấy danh sách khách hàng:', error)
        let errorMessage = 'Đã có lỗi xảy ra khi lấy danh sách khách hàng.'
        if (error.code === 'ERR_NETWORK') {
          errorMessage = 'Không thể kết nối đến backend. Vui lòng kiểm tra xem backend có đang chạy trên https://localhost:7139 không.'
        } else if (error.response) {
          errorMessage = error.response.data.message || errorMessage
        }
        Swal.fire('Lỗi!', errorMessage, 'error')
      }
    },
    changePage(page) {
      if (page < 1 || page > this.totalPages) return
      this.currentPage = page
      this.fetchCustomers()
    },
    openAddModal() {
      this.isEditMode = false
      this.currentCustomer = {
        maKh: 0,
        hoTen: '',
        gioiTinh: '',
        ngaySinh: '',
        diaChi: '',
        cccd: '',
        sdt: '',
        email: '',
        tenTaiKhoan: '',
        matKhau: '',
        hinhDaiDien: '',
        tinhTrang: 'Đang hoạt động', // Giá trị mặc định khi thêm mới
        isDelete: false,
        anh: null
      }
      this.modal.show()
    },
    openEditModal(customer) {
      this.isEditMode = true
      this.currentCustomer = { ...customer, anh: null }
      // Đảm bảo giá trị tinhTrang hợp lệ khi chỉnh sửa
      if (!['Đang hoạt động', 'Đã tạm khóa'].includes(this.currentCustomer.tinhTrang)) {
        this.currentCustomer.tinhTrang = 'Đang hoạt động' // Giá trị mặc định nếu không hợp lệ
      }
      this.detailModal.hide()
      this.modal.show()
    },
    openDetailModal(customer) {
      this.selectedCustomer = { ...customer }
      this.detailModal.show()
    },
    handleFileUpload(event) {
      const file = event.target.files[0]
      if (file) {
        this.currentCustomer.anh = file
        this.currentCustomer.hinhDaiDien = URL.createObjectURL(file)
      }
    },
    async addCustomer() {
      try {
        if (!this.currentCustomer.hoTen) {
          Swal.fire('Lỗi!', 'Họ và tên không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.gioiTinh) {
          Swal.fire('Lỗi!', 'Giới tính không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.diaChi) {
          Swal.fire('Lỗi!', 'Địa chỉ không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.cccd) {
          Swal.fire('Lỗi!', 'CCCD không được để trống.', 'error')
          return
        }
        if (!/^\d{12}$/.test(this.currentCustomer.cccd)) {
          Swal.fire('Lỗi!', 'CCCD phải là 12 chữ số.', 'error')
          return
        }
        if (!this.currentCustomer.sdt) {
          Swal.fire('Lỗi!', 'Số điện thoại không được để trống.', 'error')
          return
        }
        if (!/^0\d{9,10}$/.test(this.currentCustomer.sdt)) {
          Swal.fire('Lỗi!', 'Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.', 'error')
          return
        }
        if (this.currentCustomer.email && !/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(this.currentCustomer.email)) {
          Swal.fire('Lỗi!', 'Email không hợp lệ.', 'error')
          return
        }
        if (!this.currentCustomer.tenTaiKhoan) {
          Swal.fire('Lỗi!', 'Tên tài khoản không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.matKhau && !this.isEditMode) {
          Swal.fire('Lỗi!', 'Mật khẩu không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.tinhTrang) {
          Swal.fire('Lỗi!', 'Tình trạng không được để trống.', 'error')
          return
        }
        // Kiểm tra giá trị TinhTrang hợp lệ
        if (!['Đang hoạt động', 'Đã tạm khóa'].includes(this.currentCustomer.tinhTrang)) {
          Swal.fire('Lỗi!', "Tình trạng chỉ được phép là 'Đang hoạt động' hoặc 'Đã tạm khóa'.", 'error')
          return
        }
        if (!this.currentCustomer.anh && !this.currentCustomer.hinhDaiDien) {
          Swal.fire('Lỗi!', 'Hình đại diện không được để trống.', 'error')
          return
        }

        const formData = new FormData()
        formData.append('hoTen', this.currentCustomer.hoTen.trim())
        formData.append('gioiTinh', this.currentCustomer.gioiTinh.trim())
        if (this.currentCustomer.ngaySinh) {
          formData.append('ngaySinh', this.currentCustomer.ngaySinh)
        }
        formData.append('diaChi', this.currentCustomer.diaChi.trim())
        formData.append('cccd', this.currentCustomer.cccd.trim())
        formData.append('sdt', this.currentCustomer.sdt.trim())
        if (this.currentCustomer.email) {
          formData.append('email', this.currentCustomer.email.trim())
        }
        formData.append('tenTaiKhoan', this.currentCustomer.tenTaiKhoan.trim())
        if (!this.isEditMode) {
          formData.append('matKhau', this.currentCustomer.matKhau.trim())
        }
        formData.append('tinhTrang', this.currentCustomer.tinhTrang.trim())
        if (this.currentCustomer.anh) {
          formData.append('anh', this.currentCustomer.anh)
        }

        const response = await axios.post(getApiUrl+'/api/customer', formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
        Swal.fire('Thành công!', response.data.message, 'success')
        this.modal.hide()
        this.fetchCustomers()
      } catch (error) {
        console.error('Lỗi khi thêm khách hàng:', error)
        let errorMessage = 'Đã có lỗi xảy ra khi thêm khách hàng.'
        if (error.response && error.response.data && error.response.data.errors) {
          const errors = error.response.data.errors
          errorMessage = Object.keys(errors)
            .map(key => `${key}: ${errors[key].join(', ')}`)
            .join('\n')
        } else if (error.response && error.response.data) {
          errorMessage = error.response.data.message || errorMessage
        }
        Swal.fire('Lỗi!', errorMessage, 'error')
      }
    },
    async updateCustomer() {
      console.log(this.currentCustomer)
      try {
        if (!this.currentCustomer.hoTen) {
          Swal.fire('Lỗi!', 'Họ và tên không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.gioiTinh) {
          Swal.fire('Lỗi!', 'Giới tính không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.diaChi) {
          Swal.fire('Lỗi!', 'Địa chỉ không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.cccd) {
          Swal.fire('Lỗi!', 'CCCD không được để trống.', 'error')
          return
        }
        if (!/^\d{12}$/.test(this.currentCustomer.cccd)) {
          Swal.fire('Lỗi!', 'CCCD phải là 12 chữ số.', 'error')
          return
        }
        if (!this.currentCustomer.sdt) {
          Swal.fire('Lỗi!', 'Số điện thoại không được để trống.', 'error')
          return
        }
        if (!/^0\d{9,10}$/.test(this.currentCustomer.sdt)) {
          Swal.fire('Lỗi!', 'Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.', 'error')
          return
        }
        if (this.currentCustomer.email && !/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(this.currentCustomer.email)) {
          Swal.fire('Lỗi!', 'Email không hợp lệ.', 'error')
          return
        }
        if (!this.currentCustomer.tenTaiKhoan) {
          Swal.fire('Lỗi!', 'Tên tài khoản không được để trống.', 'error')
          return
        }
        if (!this.currentCustomer.tinhTrang) {
          Swal.fire('Lỗi!', 'Tình trạng không được để trống.', 'error')
          return
        }
        // Kiểm tra giá trị TinhTrang hợp lệ
        if (!['Đang hoạt động', 'Đã tạm khóa'].includes(this.currentCustomer.tinhTrang)) {
          Swal.fire('Lỗi!', "Tình trạng chỉ được phép là 'Đang hoạt động' hoặc 'Đã tạm khóa'.", 'error')
          return
        }

        const formData = new FormData()
        formData.append('maKh', this.currentCustomer.maKh)
        formData.append('hoTen', this.currentCustomer.hoTen.trim())
        formData.append('gioiTinh', this.currentCustomer.gioiTinh.trim())
        if (this.currentCustomer.ngaySinh) {
          formData.append('ngaySinh', this.currentCustomer.ngaySinh)
        }
        formData.append('diaChi', this.currentCustomer.diaChi.trim())
        formData.append('cccd', this.currentCustomer.cccd.trim())
        formData.append('sdt', this.currentCustomer.sdt.trim())
        if (this.currentCustomer.email) {
          formData.append('email', this.currentCustomer.email.trim())
        }
        formData.append('tenTaiKhoan', this.currentCustomer.tenTaiKhoan.trim())
        formData.append('tinhTrang', this.currentCustomer.tinhTrang.trim())
        if (this.currentCustomer.anh) {
          formData.append('anh', this.currentCustomer.anh)
        }

        const response = await axios.put(getApiUrl+`/api/customer/${this.currentCustomer.maKh}`, formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
        Swal.fire('Thành công!', response.data.message, 'success')
        this.modal.hide()
        this.fetchCustomers()
      } catch (error) {
        console.error('Lỗi khi cập nhật khách hàng:', error)
        let errorMessage = 'Đã có lỗi xảy ra khi cập nhật khách hàng.'
        if (error.response && error.response.data && error.response.data.errors) {
          const errors = error.response.data.errors
          errorMessage = Object.keys(errors)
            .map(key => `${key}: ${errors[key].join(', ')}`)
            .join('\n')
        } else if (error.response && error.response.data) {
          errorMessage = error.response.data.message || errorMessage
          if (error.response.data.error) {
            errorMessage += `\nChi tiết: ${error.response.data.error}`
          }
        }
        Swal.fire('Lỗi!', errorMessage, 'error')
      }
    },
    async hideCustomer(id) {
      if (!confirm('Bạn có chắc chắn muốn ẩn khách hàng này?')) return
      try {
        const response = await axios.put(getApiUrl+`/api/customer/Hide/${id}`)
        Swal.fire('Thành công!', response.data.message, 'success')
        this.fetchCustomers()
      } catch (error) {
        console.error('Lỗi khi ẩn khách hàng:', error)
        let errorMessage = 'Đã có lỗi xảy ra khi ẩn khách hàng.'
        if (error.response && error.response.data) {
          errorMessage = error.response.data.message || errorMessage
        }
        Swal.fire('Lỗi!', errorMessage, 'error')
      }
    },
    async exportToExcel() {
      try {
        const response = await axios.get(getApiUrl+'/api/customer/ExportExcel', {
          params: {
            searchTerm: this.searchTerm,
            sortBy: this.sortBy
          },
          responseType: 'blob'
        })
        const url = window.URL.createObjectURL(new Blob([response.data]))
        const link = document.createElement('a')
        link.href = url
        link.setAttribute('download', `KhachHang_${new Date().toISOString().slice(0, 10)}.xlsx`)
        document.body.appendChild(link)
        link.click()
        link.remove()
        Swal.fire('Thành công!', 'Export Excel thành công.', 'success')
      } catch (error) {
        console.error('Lỗi khi export Excel:', error)
        Swal.fire('Lỗi!', 'Đã có lỗi xảy ra khi export Excel.', 'error')
      }
    },
    async importFromExcel(event) {
      const file = event.target.files[0]
      if (!file) return

      try {
        const formData = new FormData()
        formData.append('file', file)
        const response = await axios.post(getApiUrl+'/api/customer/ImportExcel', formData, {
          headers: { 'Content-Type': 'multipart/form-data' }
        })
        Swal.fire(
          'Kết quả Import',
          `${response.data.message}\n` +
          `Thành công: ${response.data.successCount} bản ghi\n` +
          `Lỗi: ${response.data.errorCount} bản ghi\n` +
          (response.data.errors.length > 0 ? `Chi tiết lỗi:\n${response.data.errors.join('\n')}` : ''),
          'info'
        )
        this.fetchCustomers()
        this.$refs.fileInput.value = ''
      } catch (error) {
        console.error('Lỗi khi import Excel:', error)
        let errorMessage = 'Đã có lỗi xảy ra khi import Excel.'
        if (error.response && error.response.data) {
          errorMessage = error.response.data.message || errorMessage
        }
        Swal.fire('Lỗi!', errorMessage, 'error')
      }
    }
  }
}
</script>

<style scoped>
/* Đảm bảo nội dung chính không bị đè lên footer */
.main-content {
  min-height: calc(100vh - 300px);
  padding-bottom: 50px;
  padding-top: 40px;
}

/* Tiêu đề chính */
h2.text-primary {
  font-size: 28px;
  font-weight: 700;
  color: #007bff;
  text-transform: uppercase;
  letter-spacing: 1px;
}

/* Thanh tìm kiếm và sắp xếp */
.input-group-text {
  border: 1px solid #ced4da;
  border-radius: 5px 0 0 5px;
  background-color: #fff;
  color: #6c757d;
}

.custom-input {
  border: 1px solid #ced4da;
  border-radius: 5px;
  padding: 10px;
  transition: border-color 0.3s ease, box-shadow 0.3s ease;
}

.custom-input:focus {
  border-color: #007bff;
  box-shadow: 0 0 5px rgba(0, 123, 255, 0.3);
  outline: none;
}

.form-select.custom-input {
  border-radius: 5px;
}

/* Nút hành động */
.custom-btn {
  border-radius: 5px;
  padding: 10px 20px;
  font-weight: 600;
  transition: transform 0.1s ease, background-color 0.3s ease;
}

.custom-btn:hover {
  transform: translateY(-2px);
}

.btn-primary.custom-btn {
  background-color: #007bff;
  border: none;
}

.btn-primary.custom-btn:hover {
  background-color: #0056b3;
}

.btn-success.custom-btn {
  background-color: #28a745;
  border: none;
}

.btn-success.custom-btn:hover {
  background-color: #218838;
}

.btn-info.custom-btn {
  background-color: #17a2b8;
  border: none;
}

.btn-info.custom-btn:hover {
  background-color: #138496;
}

/* Bảng danh sách khách hàng */
.table-container {
  background-color: #fff;
  border-radius: 10px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
  overflow: hidden;
}

.custom-table {
  margin-bottom: 0;
}

.custom-table thead {
  background: linear-gradient(90deg, #007bff, #00c4ff);
  color: white;
}

.custom-table thead th {
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.5px;
  padding: 15px;
  border: none;
}

.custom-table tbody tr {
  transition: background-color 0.3s ease;
}

.custom-table tbody tr:hover {
  background-color: #f1f3f5;
}

.custom-table tbody td {
  vertical-align: middle;
  padding: 15px;
  border-color: #e9ecef;
}

.avatar-table-img {
  width: 100px;
  height: 100px;
  /* object-fit: cover; */
  border-radius: 50%;
  border: 2px solid #007bff;
  transition: transform 0.3s ease;
  cursor: pointer;
}

.avatar-table-img:hover {
  transform: scale(1.1);
}

.text-primary.fw-semibold {
  color: #007bff !important;
  font-weight: 600;
}

.text-primary.fw-semibold:hover {
  text-decoration: underline;
}

.badge.bg-success {
  background-color: #28a745 !important;
  padding: 5px 10px;
  border-radius: 12px;
  font-size: 12px;
}

.badge.bg-warning {
  background-color: #ffc107 !important;
  color: #212529 !important;
  padding: 5px 10px;
  border-radius: 12px;
  font-size: 12px;
}

.custom-action-btn {
  border-radius: 5px;
  padding: 5px 15px;
  font-size: 14px;
  font-weight: 600;
  transition: transform 0.1s ease, background-color 0.3s ease;
}

.custom-action-btn:hover {
  transform: translateY(-2px);
}

.btn-warning.custom-action-btn {
  background-color: #ffc107;
  border: none;
  color: #212529;
}

.btn-warning.custom-action-btn:hover {
  background-color: #e0a800;
}

.btn-danger.custom-action-btn {
  background-color: #dc3545;
  border: none;
}

.btn-danger.custom-action-btn:hover {
  background-color: #c82333;
}

/* Phân trang */
.custom-pagination .page-item .page-link {
  border-radius: 5px;
  margin: 0 5px;
  color: #007bff;
  border: 1px solid #dee2e6;
  transition: background-color 0.3s ease, color 0.3s ease;
}

.custom-pagination .page-item.active .page-link {
  background-color: #007bff;
  border-color: #007bff;
  color: white;
}

.custom-pagination .page-item .page-link:hover {
  background-color: #007bff;
  color: white;
  border-color: #007bff;
}

.custom-pagination .page-item.disabled .page-link {
  color: #6c757d;
  background-color: #f8f9fa;
  border-color: #dee2e6;
}

/* CSS chung cho cả hai modal */
.custom-modal-content {
  border-radius: 10px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
  border: none;
}

.custom-modal-header {
  background: linear-gradient(90deg, #007bff, #00c4ff);
  color: white;
  border-top-left-radius: 10px;
  border-top-right-radius: 10px;
  padding: 20px;
}

.custom-modal-header .modal-title {
  font-size: 24px;
  font-weight: 600;
}

.custom-modal-header .btn-close {
  filter: invert(1);
}

.modal-body {
  padding: 30px;
  background-color: #f8f9fa;
}

.avatar-container {
  position: relative;
  margin-bottom: 20px;
}

.avatar-img {
  width: 200px;
  height: 200px;
  object-fit: cover;
  border-radius: 50%;
  border: 5px solid #007bff;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.avatar-img:hover {
  transform: scale(1.05);
}

.custom-modal-footer {
  border-bottom-left-radius: 10px;
  border-bottom-right-radius: 10px;
  padding: 15px 30px;
  background-color: #f8f9fa;
}

.custom-modal-footer .btn-secondary {
  background-color: #6c757d;
  border: none;
  border-radius: 5px;
  padding: 10px 20px;
  transition: background-color 0.3s ease;
}

.custom-modal-footer .btn-secondary:hover {
  background-color: #5a6268;
}

.custom-modal-footer .btn-primary {
  background-color: #007bff;
  border: none;
  border-radius: 5px;
  padding: 10px 20px;
  transition: background-color 0.3s ease;
}

.custom-modal-footer .btn-primary:hover {
  background-color: #0056b3;
}

/* CSS cho modal chi tiết */
.info-item {
  display: flex;
  align-items: center;
  margin-bottom: 15px;
  padding: 10px;
  background-color: #fff;
  border-radius: 5px;
  box-shadow: 0 1px 5px rgba(0, 0, 0, 0.05);
  transition: background-color 0.3s ease;
}

.info-item:hover {
  background-color: #e9ecef;
}

.info-item strong {
  width: 150px;
  font-weight: 600;
  color: #343a40;
}

.info-item span {
  flex: 1;
  color: #495057;
}

.text-muted {
  color: #6c757d !important;
  font-size: 14px;
}

/* CSS cho modal thêm/sửa */
.form-label {
  font-weight: 600;
  color: #343a40;
  margin-bottom: 8px;
}

.custom-submit-btn {
  background-color: #28a745;
  border: none;
  border-radius: 5px;
  padding: 10px 30px;
  font-size: 16px;
  font-weight: 600;
  transition: background-color 0.3s ease;
}

.custom-submit-btn:hover {
  background-color: #218838;
}
</style>