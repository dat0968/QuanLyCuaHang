<template>
  <div>
    <br />
    <br />
    <br />
    <div class="container mt-5">
      <h2 class="mb-4" style="color: black; font-size: 50px">Thông Tin Cá Nhân</h2>
      <div v-if="loading" class="text-center">
        <p>Đang tải...</p>
      </div>
      <div v-else-if="error" class="alert alert-danger">
        {{ error }}
      </div>
      <div v-else-if="profile" class="card custom-card">
        <div class="card-body">
          <div class="row">
            <div class="col-md-6 text-center">
              <img
                v-if="profile.hinhDaiDien"
                :src="getApiUrl + profile.hinhDaiDien"
                alt="Hình đại diện"
                class="avatar-img mb-3"
              />
              <img
                v-else
                src="https://via.placeholder.com/200x200?text=No+Image"
                alt="Hình đại diện mặc định"
                class="avatar-img mb-3"
              />
            </div>

            <div class="col-md-6">
              <p><strong>Mã khách hàng:</strong> {{ profile.maKh }}</p>
              <p><strong>Họ tên:</strong> {{ profile.hoTen }}</p>
              <p><strong>Giới tính:</strong> {{ profile.gioiTinh }}</p>
              <p><strong>Ngày sinh:</strong> {{ formatDate(profile.ngaySinh) }}</p>
              <p><strong>Địa chỉ:</strong> {{ profile.diaChi }}</p>
              <p><strong>CCCD:</strong> {{ profile.cccd }}</p>
              <p><strong>Số điện thoại:</strong> {{ profile.sdt }}</p>
              <p><strong>Email:</strong> {{ profile.email || 'Chưa có' }}</p>
              <p><strong>Tên tài khoản:</strong> {{ profile.tenTaiKhoan }}</p>
              <p>
                <strong>Trạng thái:</strong>
                <span
                  :class="{
                    'badge bg-success': profile.tinhTrang === 'Đang hoạt động',
                    'badge bg-warning': profile.tinhTrang === 'Đã tạm khóa',
                  }"
                >
                  {{ profile.tinhTrang }}
                </span>
              </p>
              <button class="btn btn-primary custom-btn" @click="showEditModal">
                Sửa thông tin
              </button>
            </div>
          </div>
        </div>
      </div>
      <div v-else class="alert alert-warning">Không tìm thấy thông tin khách hàng.</div>
      <div
        class="modal fade"
        id="editModal"
        tabindex="-1"
        aria-labelledby="editModalLabel"
        aria-hidden="true"
      >
        <div class="modal-dialog modal-lg">
          <div class="modal-content custom-modal-content">
            <div class="modal-header">
              <h5 class="modal-title" id="editModalLabel" style="font-size: 50px">
                Sửa Thông Tin Cá Nhân
              </h5>
              <button
                type="button"
                class="btn-close"
                data-bs-dismiss="modal"
                aria-label="Close"
              ></button>
            </div>
            <div class="modal-body">
              <form @submit.prevent="updateProfile">
                <div class="row">
                  <div class="col-md-6 text-center">
                    <div class="avatar-container">
                      <img
                        v-if="editProfile.hinhDaiDien && !editProfile.anh"
                        :src="getApiUrl + editProfile.hinhDaiDien"
                        alt="Hình đại diện"
                        class="avatar-img"
                      />
                      <img
                        v-else-if="editProfile.anh"
                        :src="URL.createObjectURL(editProfile.anh)"
                        alt="Hình đại diện mới"
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
                        @change="onFileChange"
                        accept="image/*"
                      />
                    </div>
                  </div>
                  <div class="col-md-6">
                    <div class="mb-3">
                      <label for="hoTen" class="form-label">Họ tên</label>
                      <input
                        v-model="editProfile.hoTen"
                        type="text"
                        class="form-control custom-input"
                        id="hoTen"
                        required
                      />
                    </div>
                    <div class="mb-3">
                      <label for="gioiTinh" class="form-label">Giới tính</label>
                      <select
                        v-model="editProfile.gioiTinh"
                        class="form-select custom-input"
                        id="gioiTinh"
                        required
                      >
                        <option value="Nam">Nam</option>
                        <option value="Nữ">Nữ</option>
                        <option value="Khác">Khác</option>
                      </select>
                    </div>
                    <div class="mb-3">
                      <label for="ngaySinh" class="form-label">Ngày sinh</label>
                      <input
                        v-model="editProfile.ngaySinh"
                        type="date"
                        class="form-control custom-input"
                        id="ngaySinh"
                      />
                    </div>
                    <div class="mb-3">
                      <label for="diaChi" class="form-label">Địa chỉ</label>
                      <input
                        v-model="editProfile.diaChi"
                        type="text"
                        class="form-control custom-input"
                        id="diaChi"
                        required
                      />
                    </div>
                    <div class="mb-3">
                      <label for="cccd" class="form-label">CCCD</label>
                      <input
                        v-model="editProfile.cccd"
                        type="text"
                        class="form-control custom-input"
                        id="cccd"
                        required
                      />
                    </div>
                    <div class="mb-3">
                      <label for="sdt" class="form-label">Số điện thoại</label>
                      <input
                        v-model="editProfile.sdt"
                        type="text"
                        class="form-control custom-input"
                        id="sdt"
                        required
                      />
                    </div>
                    <div class="mb-3">
                      <label for="email" class="form-label">Email</label>
                      <input
                        v-model="editProfile.email"
                        type="email"
                        class="form-control custom-input"
                        id="email"
                      />
                    </div>
                  </div>
                </div>
                <div class="text-end">
                  <button type="submit" class="btn btn-success custom-submit-btn">Cập nhật</button>
                </div>
              </form>
            </div>
          </div>
        </div>
      </div>
    </div>
    <br />
    <br />
  </div>
</template>

<script>
import { ref, onMounted } from 'vue'
import axios from 'axios'
import { Modal } from 'bootstrap'
import Swal from 'sweetalert2'
import { GetApiUrl } from '@constants/api'
export default {
  name: 'ProfilePage',
  setup() {
    const profile = ref(null)
    const editProfile = ref({})
    const loading = ref(true)
    const error = ref(null)
    let editModal = null
    let getApiUrl = GetApiUrl()
    const fetchProfile = async () => {
      try {
        const response = await axios.get(getApiUrl + '/api/Profile/GetProfile')
        console.log('API Response:', response.data)

        if (response.data.success) {
          profile.value = response.data.data
          editProfile.value = { ...response.data.data, anh: null }
        } else {
          error.value = response.data.message || 'Không tìm thấy thông tin khách hàng'
        }
      } catch (err) {
        console.error('Lỗi khi lấy thông tin hồ sơ:', err)
        error.value =
          'Có lỗi xảy ra khi tải thông tin hồ sơ: ' + (err.response?.data?.message || err.message)
      } finally {
        loading.value = false
      }
    }

    const showEditModal = () => {
      editModal = new Modal(document.getElementById('editModal'))
      editModal.show()
    }

    const onFileChange = (event) => {
      const file = event.target.files[0]
      if (file) {
        editProfile.value.anh = file
      }
    }

    const updateProfile = async () => {
      if (!editProfile.value.hoTen) {
        Swal.fire('Lỗi!', 'Họ và tên không được để trống.', 'error')
        return
      }
      if (!editProfile.value.gioiTinh) {
        Swal.fire('Lỗi!', 'Giới tính không được để trống.', 'error')
        return
      }
      if (!editProfile.value.diaChi) {
        Swal.fire('Lỗi!', 'Địa chỉ không được để trống.', 'error')
        return
      }
      if (!editProfile.value.cccd) {
        Swal.fire('Lỗi!', 'CCCD không được để trống.', 'error')
        return
      }
      if (!/^\d{12}$/.test(editProfile.value.cccd)) {
        Swal.fire('Lỗi!', 'CCCD phải là 12 chữ số.', 'error')
        return
      }
      if (!editProfile.value.sdt) {
        Swal.fire('Lỗi!', 'Số điện thoại không được để trống.', 'error')
        return
      }
      if (!/^0\d{9,10}$/.test(editProfile.value.sdt)) {
        Swal.fire('Lỗi!', 'Số điện thoại phải bắt đầu bằng 0 và có 10-11 chữ số.', 'error')
        return
      }
      if (
        editProfile.value.email &&
        !/^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/.test(editProfile.value.email)
      ) {
        Swal.fire('Lỗi!', 'Email không hợp lệ.', 'error')
        return
      }

      const formData = new FormData()
      formData.append('MaKh', editProfile.value.maKh || 104)
      formData.append('HoTen', editProfile.value.hoTen.trim())
      formData.append('GioiTinh', editProfile.value.gioiTinh.trim())
      if (editProfile.value.ngaySinh) {
        formData.append('NgaySinh', editProfile.value.ngaySinh)
      }
      formData.append('DiaChi', editProfile.value.diaChi.trim())
      formData.append('Cccd', editProfile.value.cccd.trim())
      formData.append('Sdt', editProfile.value.sdt.trim())
      if (editProfile.value.email) {
        formData.append('Email', editProfile.value.email.trim())
      }
      formData.append('TenTaiKhoan', editProfile.value.tenTaiKhoan.trim())
      formData.append('TinhTrang', editProfile.value.tinhTrang || 'Đang hoạt động')
      if (editProfile.value.anh) {
        formData.append('Anh', editProfile.value.anh)
      }

      try {
        const response = await axios.put(getApiUrl + '/api/Profile/UpdateProfile', formData, {
          headers: { 'Content-Type': 'multipart/form-data' },
        })
        if (response.data.success) {
          Swal.fire('Thành công!', 'Cập nhật thông tin thành công!', 'success')
          editModal.hide()
          await fetchProfile()
        } else {
          Swal.fire('Lỗi!', response.data.message, 'error')
        }
      } catch (error) {
        console.error('Lỗi khi cập nhật thông tin:', error)
        let errorMessage = 'Có lỗi xảy ra khi cập nhật thông tin.'
        if (error.response && error.response.data) {
          errorMessage = error.response.data.message || errorMessage
        }
        Swal.fire('Lỗi!', errorMessage, 'error')
      }
    }

    const formatDate = (date) => {
      if (!date) return 'Chưa cập nhật'
      return new Date(date).toLocaleDateString('vi-VN')
    }

    onMounted(() => {
      fetchProfile()
    })

    return {
      profile,
      editProfile,
      loading,
      error,
      showEditModal,
      onFileChange,
      updateProfile,
      formatDate,
    }
  },
}
</script>

<style scoped>
.container {
  max-width: 800px;
}
h2.text-primary {
  font-size: 28px;
  font-weight: 700;
  color: #007bff;
  text-transform: uppercase;
  letter-spacing: 1px;
}
.custom-card {
  border-radius: 10px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.1);
  background-color: #fff;
}

.card-body {
  padding: 30px;
}

.avatar-img {
  width: 300px;
  height: 400px;
  object-fit: cover;
  border: 3px solid aqua;
  box-shadow: 0 2px 10px rgba(0, 0, 0, 0.1);
  transition: transform 0.3s ease;
}

.avatar-img:hover {
  transform: scale(1.05);
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

/* Nút hành động */
.custom-btn {
  border-radius: 5px;
  padding: 10px 20px;
  font-weight: 600;
  transition: transform 0.1s ease, background-color 0.3s ease;
}

.custom-btn:hover {
  transform: translateY(-2px);
  background-color: #0056b3;
}

/* Modal chỉnh sửa */
.custom-modal-content {
  border-radius: 10px;
  box-shadow: 0 5px 15px rgba(0, 0, 0, 0.3);
}

.custom-modal-header {
  background: linear-gradient(90deg, #007bff, #00c4ff);
  color: white;
  border-top-left-radius: 10px;
  border-top-right-radius: 10px;
  padding: 20px;
}

.modal-body {
  padding: 30px;
  background-color: #f8f9fa;
}

.form-label {
  font-weight: 600;
  color: #343a40;
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