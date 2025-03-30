<template>
  <div class="container py-5 mt-5 pt-5" style="margin-top: 8rem !important">
    <h2 class="mb-4">Thanh Toán</h2>

    <div class="row">
      <!-- Thông tin đơn hàng -->
      <div class="col-md-8">
        <div class="card mb-4">
          <div class="card-header">
            <h5 class="mb-0">Thông tin đơn hàng</h5>
          </div>
          <div class="card-body">
            <div class="table-responsive">
              <table class="table">
                <thead>
                  <tr>
                    <th>Sản phẩm</th>
                    <th>Giá</th>
                    <th>Số lượng</th>
                    <th>Tổng</th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="item in cartItems" :key="item.maSp">
                    <td>
                      <div class="d-flex align-items-center">
                        <img
                          :src="item.hinhAnh"
                          :alt="item.tenSanPham"
                          class="me-2"
                          style="width: 50px; height: 50px; object-fit: cover"
                        />
                        <span>{{ item.tenSanPham }}</span>
                      </div>
                    </td>
                    <td>{{ item.gia.toLocaleString('vi-VN') }}đ</td>
                    <td>{{ item.soLuong }}</td>
                    <td>{{ (item.gia * item.soLuong).toLocaleString('vi-VN') }}đ</td>
                  </tr>
                </tbody>
              </table>
            </div>

            <!-- Mã giảm giá -->
            <div class="mt-3">
              <div class="input-group">
                <input
                  type="text"
                  class="form-control"
                  v-model="couponCode"
                  placeholder="Nhập mã giảm giá"
                />
                <button class="btn btn-outline-primary">Áp dụng</button>
              </div>
            </div>

            <!-- Phương thức vận chuyển -->
            <div class="mt-4">
              <h6>Phương thức vận chuyển</h6>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="radio"
                  v-model="shippingMethod"
                  value="standard"
                  id="standard"
                />
                <label class="form-check-label" for="standard">
                  Giao hàng tiêu chuẩn (30.000đ)
                </label>
              </div>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="radio"
                  v-model="shippingMethod"
                  value="express"
                  id="express"
                />
                <label class="form-check-label" for="express"> Giao hàng nhanh (50.000đ) </label>
              </div>
            </div>
          </div>
        </div>

        <!-- Thông tin người nhận -->
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Thông tin người nhận</h5>
          </div>
          <div class="card-body">
            <div class="mb-3">
              <label class="form-label">Họ và tên</label>
              <input type="text" class="form-control" :value="userInfo.hoTen" />
            </div>
            <div class="mb-3">
              <label class="form-label">Email</label>
              <input type="email" class="form-control" :value="userInfo.email" />
            </div>
            <div class="mb-3">
              <label class="form-label">Số điện thoại</label>
              <input type="tel" class="form-control" :value="userInfo.soDienThoai" />
            </div>
            <div class="mb-3">
              <label class="form-label">Địa chỉ</label>
              <textarea class="form-control" rows="3" v-model="userInfo.diaChi"></textarea>
            </div>
          </div>
        </div>
      </div>

      <!-- Tổng quan đơn hàng -->
      <div class="col-md-4">
        <div class="card">
          <div class="card-header">
            <h5 class="mb-0">Tổng quan đơn hàng</h5>
          </div>
          <div class="card-body">
            <div class="d-flex justify-content-between mb-2">
              <span>Tạm tính:</span>
              <span
                >{{
                  cartItems
                    .reduce((total, item) => total + item.gia * item.soLuong, 0)
                    .toLocaleString('vi-VN')
                }}đ</span
              >
            </div>
            <div class="d-flex justify-content-between mb-2">
              <span>Phí vận chuyển:</span>
              <span>{{ shippingFee.toLocaleString('vi-VN') }}đ</span>
            </div>
            <div class="d-flex justify-content-between mb-2">
              <span>Giảm giá:</span>
              <span>0đ</span>
            </div>
            <hr />
            <div class="d-flex justify-content-between mb-3">
              <strong>Tổng cộng:</strong>
              <strong
                >{{
                  (
                    cartItems.reduce((total, item) => total + item.gia * item.soLuong, 0) +
                    shippingFee
                  ).toLocaleString('vi-VN')
                }}đ</strong
              >
            </div>
            <button class="btn btn-primary w-100">Đặt hàng</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'
// Dữ liệu mẫu cho giao diện tĩnh
const cartItems = [
  {
    maSp: 1,
    tenSanPham: 'Sản phẩm 1',
    gia: 150000,
    soLuong: 2,
    hinhAnh: 'https://via.placeholder.com/100',
  },
  {
    maSp: 2,
    tenSanPham: 'Sản phẩm 2',
    gia: 200000,
    soLuong: 1,
    hinhAnh: 'https://via.placeholder.com/100',
  },
]

const userInfo = {
  hoTen: 'Nguyễn Văn A',
  email: 'example@email.com',
  soDienThoai: '0123456789',
  diaChi: '123 Đường ABC, Quận 1, TP.HCM',
}

const couponCode = ref('')
const shippingMethod = ref('standard')
const shippingFee = 30000
</script>

<style scoped>
.card {
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.card-header {
  background-color: #f8f9fa;
  border-bottom: 1px solid #dee2e6;
}

.form-control:focus {
  border-color: #80bdff;
  box-shadow: 0 0 0 0.2rem rgba(0, 123, 255, 0.25);
}

.btn-primary {
  background-color: #007bff;
  border-color: #007bff;
}

.btn-primary:hover {
  background-color: #0056b3;
  border-color: #0056b3;
}
</style>