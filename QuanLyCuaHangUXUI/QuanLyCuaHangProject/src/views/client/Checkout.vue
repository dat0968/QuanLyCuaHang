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
                  <!-- Sản phẩm đơn lẻ -->
                  <tr v-for="item in ProductList" :key="item.id">
                    <td>
                      <div class="d-flex align-items-center">
                        <img
                          :src="getImageUrl(item.hinhAnh)"
                          :alt="item.tenSanPham"
                          class="me-2"
                          style="width: 50px; height: 50px; object-fit: cover"
                        />
                        <div>
                          <span>{{ item.tenSanPham }}</span>
                          <div class="item-variants">
                            <span v-if="item.kichThuoc" class="variant text-muted small">{{
                              item.kichThuoc
                            }}</span>
                            <span v-if="item.huongVi" class="variant text-muted small">{{
                              item.huongVi
                            }}</span>
                          </div>
                        </div>
                      </div>
                    </td>
                    <td>{{ item.donGia }}đ</td>
                    <td>{{ item.soLuong }}</td>
                    <td>{{ item.donGia * item.soLuong }}đ</td>
                  </tr>

                  <!-- Combo -->
                  <tr v-for="combo in ComboList" :key="combo.id">
                    <td>
                      <div class="d-flex align-items-center">
                        <img
                          :src="getImageUrl(combo.hinhAnh)"
                          alt="Combo Đặc Biệt"
                          class="me-2"
                          style="width: 50px; height: 50px; object-fit: cover"
                        />
                        <div>
                          <span>{{ combo.tenCombo }}</span>
                          <div class="text-muted small mt-1">
                            <div class="combo-items">
                              <div
                                v-for="detail in combo.cartDetailCombos"
                                :key="detail.id"
                                class="combo-item-detail"
                              >
                                <span>{{ detail.soLuong }}x {{ detail.tenSanPham }}</span>
                                <span v-if="detail.kichThuoc">({{ detail.kichThuoc }})</span>
                                <span v-if="detail.huongVi">({{ detail.huongVi }})</span>
                              </div>
                            </div>
                          </div>
                        </div>
                      </div>
                    </td>
                    <td>{{ combo.donGia }}đ</td>
                    <td>{{ combo.soLuong }}</td>
                    <td>{{ combo.donGia * combo.soLuong }}đ</td>
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
                <label class="form-check-label" for="standard"> COD </label>
              </div>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="radio"
                  v-model="shippingMethod"
                  value="express"
                  id="express"
                />
                <label class="form-check-label" for="express"> VNPAY </label>
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
              <label class="form-label">Số điện thoại</label>
              <input type="tel" class="form-control" :value="userInfo.soDienThoai" />
            </div>
            <div class="mb-3">
              <label class="form-label">Tỉnh/Thành phố</label>
              <select
                class="form-control"
                v-model="userInfo.provinceId"
               
              >
                <option
                  v-for="province in provinces"
                  :key="province.ProvinceID"
                  :value="province.ProvinceID"
                >
                  {{ province.ProvinceName }}
                </option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Quận/Huyện</label>
              <select
                class="form-control"
                v-model="userInfo.districtId"
          
              >
                <option
                  v-for="district in districts"
                  :key="district.DistrictID"
                  :value="district.DistrictID"
                  
                >
                  {{ district.DistrictName }}
                </option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Phường/Xã</label>
              <select @change="CalculateFee()" class="form-control" v-model="userInfo.wardCode">
                <option value="" disabled>Chọn phường/xã</option>
                <option v-for="ward in wards" :key="ward.WardCode" :value="ward.WardCode">
                  {{ ward.WardName }}
                </option>
              </select>
            </div>
            <div class="mb-3">
              <label class="form-label">Địa chỉ chi tiết</label>
              <textarea class="form-control" rows="3" v-model="userInfo.diaChi"></textarea>
            </div>
            <div class="mb-3">
              <label class="form-label">Mô tả</label>
              <textarea class="form-control" rows="3" v-model="userInfo.moTa"></textarea>
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
              <span>Tổng tiền hàng:</span>
              <span
                >{{
                  SumCart.toLocaleString('vi-VN')
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
                    SumCart +
                    shippingFee
                  ).toLocaleString('vi-VN')
                }}đ</strong
              >
            </div>
            <button class="btn btn-primary w-100" @click="proceedToCheckout">Đặt hàng</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Swal from 'sweetalert2'

const router = useRouter()
const cartItems = ref([])
const provinces = ref([])
const districts = ref([])
const wards = ref([])
const token = 'eb507c61-0fad-11f0-9aa0-bece206412cb'
const shippingFee = ref(0)
const SumCart = ref(0)
const totalQuantity = ref(0)
// Lấy URL hình ảnh
const getImageUrl = (imageName) => {
  return `https://localhost:7139/HinhAnh/Food_Drink/${imageName}`
}
const FetchCart = async () => {
  try {
    const response = await fetch(`https://localhost:7139/api/Cart/120`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('ERROR', response.status)
    }
    const result = await response.json()
    cartItems.value = result.cartItems;
    SumCart.value = result.total;
    totalQuantity.value = result.totalQuantity;
  } catch (error) {
    console.log(error)
  }
}
const userInfo = ref({
  hoTen: 'Nguyễn Văn A',
  email: 'example@email.com',
  soDienThoai: '0123456789',
  diaChi: '123 Đường ABC, Quận 1, TP.HCM',
  moTa: '',
  provinceId: "",
  districtId: "",
  wardCode: "",
})

const fetchAddress = async () => {
  const responseProvince = await fetch(
    `https://online-gateway.ghn.vn/shiip/public-api/master-data/province`,
    {
      method: 'GET',
      headers: {
        'Content-type': 'application/json',
        'Token': `${token}`,
      },
    }
  )
  const resultProvince = await responseProvince.json()
  if (resultProvince.code === 200) {
    provinces.value = resultProvince.data.filter((p) => p.ProvinceName === 'Đắk Lắk')
    if (provinces.value.length > 0) {
      userInfo.value.provinceId = provinces.value[0].ProvinceID // Cập nhật khi có dữ liệu
    }
  }
  const responseDistrict = await fetch(
    `https://online-gateway.ghn.vn/shiip/public-api/master-data/district`,
    {
      method: 'POST',
      headers: {
        'Content-type': 'application/json',
        'Token': `${token}`,
      },
      body: JSON.stringify({
        "province_id": userInfo.value.provinceId
      })
    }
  )
  const resultDistrict = await responseDistrict.json()
  if (resultDistrict.code === 200) {
    districts.value = resultDistrict.data.filter((p) => p.DistrictName === 'Thành phố Buôn Ma Thuột')
    if (districts.value.length > 0) {
      userInfo.value.districtId = districts.value[0].DistrictID // Cập nhật khi có dữ liệu
    }
  }
  
  const responseWard = await fetch(
    `https://online-gateway.ghn.vn/shiip/public-api/master-data/ward`,
    {
      method: 'POST',
      headers: {
        'Content-type': 'application/json',
        'Token': `${token}`,
      },
      body: JSON.stringify({
        "district_id": userInfo.value.districtId
      })
    }
  )
  const allowedWard = ['Phường Thống Nhất', 'Phường Thành Công', 'Phường Thắng Lợi', 'Phường Tân Lợi', 'Phường Tân Lập', 'Phường Tân An']
  const resultWard = await responseWard.json()
  if (resultWard.code === 200) {
    wards.value = resultWard.data.filter((p) => allowedWard.includes(p.WardName))
  }
}
const CalculateFee = async () => {
  const content = {
      "from_district_id": userInfo.value.districtId,
      "from_ward_code": "400103", // 400103 là WardCode của phường Tân An - BMT, đây là địa điểm của cửa hàng
      "service_id": 53320,
      "service_type_id":null,
      "to_district_id": userInfo.value.districtId,
      "to_ward_code": userInfo.value.wardCode,
      "weight":200,
      "insurance_value":10000,
      "cod_failed_amount":2000,
  }
  const fetchAPIFee = await fetch(`https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee`, {
    method: 'POST',
    headers: {
        'Content-type': 'application/json',
        'Token': `${token}`,
        'ShopId': '5715364'
    },

    /* Cửa hàng chỉ giao cho khách trong một phạm vi địa điểm nhất định, ở đây thì tỉnh/thành phố và quận/huyện sẽ được
    fix cứng chỉ để một giá trị duy nhất, chỉ có phường xã là có giá trị dựa trên lựa chọn của khách hàng*/
    body: JSON.stringify(content)
  })
  const result = await fetchAPIFee.json();
  console.log(content)
  if(result.code === 200){
    shippingFee.value = result.data.total;
  }
}
onMounted(() => {
  FetchCart()
  fetchAddress()
})

// Tách sản phẩm và combo ra riêng
const ProductList = computed(() => {
  return cartItems.value.filter((item) => item.maCombo === null)
})
console.log(ProductList)
const ComboList = computed(() => {
  return cartItems.value.filter((item) => item.maCombo !== null)
})


const couponCode = ref('')
const shippingMethod = ref('standard')


const proceedToCheckout = async () => {
  try {
    const orderData = {
      maKh: 120, // TODO: Lấy từ thông tin người dùng đăng nhập
      diaChiNhanHang: userInfo.value.diaChi,
      ngayThanhToan: new Date(),
      hinhThucTt: 'COD',
      moTa: '',
      hoTen: userInfo.value.hoTen,
      sdt: userInfo.value.soDienThoai,
      phiVanChuyen: shippingFee.value,
      tienGoc: cartItems.value.reduce((total, item) => total + item.gia * item.soLuong, 0),
    }

    const response = await fetch('https://localhost:7139/api/Checkout', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(orderData),
    })

    if (!response.ok) {
      throw new Error('Lỗi khi đặt hàng')
    }

    const result = await response.json()
    if (result.success) {
      Swal.fire({
        icon: 'success',
        title: 'Đặt hàng thành công!',
        text: 'Cảm ơn bạn đã mua hàng tại cửa hàng chúng tôi.',
      }).then(() => {
        window.dispatchEvent(new CustomEvent('update-cart'))
        router.push('/')
      })
    }
  } catch (error) {
    console.error('Lỗi khi đặt hàng:', error)
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Đã xảy ra lỗi khi đặt hàng. Vui lòng thử lại.',
    })
  }
}
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