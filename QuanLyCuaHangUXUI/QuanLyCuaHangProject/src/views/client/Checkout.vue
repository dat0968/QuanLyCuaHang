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
                <button @click="fetchCoupon" class="btn btn-outline-primary">Áp dụng</button>
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
                  value="COD"
                  id="standard"
                />
                <label class="form-check-label" for="standard"> COD </label>
              </div>
              <div class="form-check">
                <input
                  class="form-check-input"
                  type="radio"
                  v-model="shippingMethod"
                  value="VNPAY"
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
              <input type="text" class="form-control" v-model="userInfo.hoTen" />
              <span class="text-danger" v-if="errors.hoTen">{{ errors.hoTen }}</span>
            </div>
            <div class="mb-3">
              <label class="form-label">Số điện thoại</label>
              <input type="tel" class="form-control" v-model="userInfo.soDienThoai" />
              <span class="text-danger" v-if="errors.soDienThoai">{{ errors.soDienThoai }}</span>
            </div>
            <div class="mb-3">
              <label class="form-label">Tỉnh/Thành phố</label>
              <select class="form-control" v-model="userInfo.provinceId" disabled>
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
              <select class="form-control" v-model="userInfo.districtId" disabled>
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
              <span class="text-danger" v-if="errors.wardCode">{{ errors.wardCode }}</span>
            </div>
            <div class="mb-3">
              <label class="form-label">Địa chỉ chi tiết</label>
              <textarea class="form-control" rows="3" v-model="userInfo.diaChi"></textarea>
              <span class="text-danger" v-if="errors.diaChi">{{ errors.diaChi }}</span>
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
              <span>{{ SumCart.toLocaleString('vi-VN') }}đ</span>
            </div>
            <div class="d-flex justify-content-between mb-2">
              <span>Phí vận chuyển:</span>
              <span>{{ shippingFee.toLocaleString('vi-VN') }}đ</span>
            </div>
            <div class="d-flex justify-content-between mb-2">
              <span>Giảm giá:</span>
              <span>{{ discount.toLocaleString('vi-VN') }}đ</span>
            </div>
            <hr />
            <div class="d-flex justify-content-between mb-3">
              <strong>Tổng cộng:</strong>
              <strong>{{ (SumCart + shippingFee - discount).toLocaleString('vi-VN') }}đ</strong>
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
import { ReadToken, ValidateToken } from '../../Authentication_Authorization/auth.js'
import Cookies from 'js-cookie'
import { GetApiUrl } from '@constants/api'
const router = useRouter()
const cartItems = ref([])
const provinces = ref([])
const districts = ref([])
const wards = ref([])
const token = 'eb507c61-0fad-11f0-9aa0-bece206412cb'
const shippingFee = ref(0)
const SumCart = ref(0)
const totalQuantity = ref(0)
const shippingMethod = ref('COD')
const discount = ref(0)
const couponCode = ref('')
let accesstoken = Cookies.get('accessToken')
let refreshtoken = Cookies.get('refreshToken')
let IdUser = ''
let getApiUrl = GetApiUrl()
// Validation errors
const errors = ref({
  hoTen: '',
  soDienThoai: '',
  wardCode: '',
  diaChi: '',
})

// Validate form
const validateForm = () => {
  let isValid = true
  errors.value = {
    hoTen: '',
    soDienThoai: '',
    wardCode: '',
    diaChi: '',
  }

  if (!userInfo.value) {
    return false
  }

  // Validate họ tên
  if (!userInfo.value.hoTen || !userInfo.value.hoTen.trim()) {
    errors.value.hoTen = 'Vui lòng nhập họ và tên'
    isValid = false
  }

  // Validate số điện thoại
  if (!userInfo.value.soDienThoai || !userInfo.value.soDienThoai.trim()) {
    errors.value.soDienThoai = 'Vui lòng nhập số điện thoại'
    isValid = false
  } else if (!/^[0-9]{10}$/.test(userInfo.value.soDienThoai)) {
    errors.value.soDienThoai = 'Số điện thoại phải có 10 chữ số'
    isValid = false
  }

  // Validate phường/xã
  if (!userInfo.value.wardCode) {
    errors.value.wardCode = 'Vui lòng chọn phường/xã'
    isValid = false
  }

  // Validate địa chỉ
  if (!userInfo.value.diaChi || !userInfo.value.diaChi.trim()) {
    errors.value.diaChi = 'Vui lòng nhập địa chỉ chi tiết'
    isValid = false
  }

  return isValid
}

const userInfo = ref({
  hoTen: '',
  soDienThoai: '',
  diaChi: '',
  moTa: '',
  provinceId: '',
  districtId: '',
  wardCode: '',
})

const getImageUrl = (imageName) => {
  return getApiUrl+`/HinhAnh/Food_Drink/${imageName}`
}

//Lấy thông tin cá nhân dựa trên tài khoản đăng nhập
const fetchCustomer = async () => {
  const validateToken = await ValidateToken(accesstoken, refreshtoken)
  if (validateToken == true) {
    accesstoken = Cookies.get('accessToken')
    const readtoken = ReadToken(accesstoken)
    if (readtoken) {
      IdUser = readtoken.IdUser
    } else {
      router.push('/Login')
      return
    }
  }
  const responseCustomer = await fetch(getApiUrl+`/api/Customer/${IdUser}`, {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
      Authorization: `Bearer ${accesstoken}`,
    },
  })
  if (responseCustomer.status == 401) {
    Swal.fire({
      icon: 'error',
      title: 'Phiên của bạn đã hết hoặc bạn chưa đăng nhập, vui lòng đăng nhập lại!',
      timer: 2000,
      showConfirmButton: false,
    })
    router.push('/Login')
    return
  }
  if (!responseCustomer.ok) {
    throw new Error('ERROR', responseCustomer.status)
  }
  const result = await responseCustomer.json()
  if (result.success) {
    ;(userInfo.value.hoTen = result.data.hoTen), (userInfo.value.soDienThoai = result.data.sdt)
  } else {
    throw new Error('Lỗi ' + responseCustomer.message)
  }
}

const FetchCart = async () => {
  try {
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if (validateToken == true) {
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if (readtoken) {
        IdUser = readtoken.IdUser
      } else {
        router.push('/Login')
        return
      }
    }
    const response = await fetch(getApiUrl+`/api/Cart/${IdUser}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        Authorization: `Bearer ${accesstoken}`,
      },
    })
    if (response.status == 401) {
      Swal.fire({
        icon: 'error',
        title: 'Phiên của bạn đã hết hoặc bạn chưa đăng nhập, vui lòng đăng nhập lại!',
        timer: 2000,
        showConfirmButton: false,
      })
      router.push('/Login')
      return
    }
    if (!response.ok) {
      throw new Error('ERROR', response.status)
    }
    const result = await response.json()
    cartItems.value = result.cartItems
    SumCart.value = result.total
    totalQuantity.value = result.totalQuantity
    if (totalQuantity.value == 0) {
      Swal.fire({
        icon: 'error',
        title: 'Giỏ hàng của bạn đang trống, hãy thêm sản phẩm vào giỏ hàng trước khi thanh toán',
        timer: 2000,
        showConfirmButton: false,
      })
      router.push('/')
    }
  } catch (error) {
    console.log(error)
  }
}

const fetchAddress = async () => {
  const responseProvince = await fetch(
    `https://online-gateway.ghn.vn/shiip/public-api/master-data/province`,
    {
      method: 'GET',
      headers: {
        'Content-type': 'application/json',
        Token: `${token}`,
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
        Token: `${token}`,
      },
      body: JSON.stringify({
        province_id: userInfo.value.provinceId,
      }),
    }
  )
  const resultDistrict = await responseDistrict.json()
  if (resultDistrict.code === 200) {
    districts.value = resultDistrict.data.filter(
      (p) => p.DistrictName === 'Thành phố Buôn Ma Thuột'
    )
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
        Token: `${token}`,
      },
      body: JSON.stringify({
        district_id: userInfo.value.districtId,
      }),
    }
  )
  const allowedWard = [
    'Phường Thống Nhất',
    'Phường Thành Công',
    'Phường Thắng Lợi',
    'Phường Tân Lợi',
    'Phường Tân Lập',
    'Phường Tân An',
  ]
  const resultWard = await responseWard.json()
  if (resultWard.code === 200) {
    wards.value = resultWard.data.filter((p) => allowedWard.includes(p.WardName))
  }
}

const CalculateFee = async () => {
  const content = {
    from_district_id: userInfo.value.districtId,
    from_ward_code: '400103', // 400103 là WardCode của phường Tân An - BMT, đây là địa điểm của cửa hàng
    service_id: 53320,
    service_type_id: null,
    to_district_id: userInfo.value.districtId,
    to_ward_code: userInfo.value.wardCode,
    weight: 200,
    insurance_value: 10000,
    cod_failed_amount: 2000,
  }
  const fetchAPIFee = await fetch(
    `https://online-gateway.ghn.vn/shiip/public-api/v2/shipping-order/fee`,
    {
      method: 'POST',
      headers: {
        'Content-type': 'application/json',
        Token: `${token}`,
        ShopId: '5715364',
      },

      /* Cửa hàng chỉ giao cho khách trong một phạm vi địa điểm nhất định, ở đây thì tỉnh/thành phố và quận/huyện sẽ được
    fix cứng chỉ để một giá trị duy nhất, chỉ có phường xã là có giá trị dựa trên lựa chọn của khách hàng*/
      body: JSON.stringify(content),
    }
  )
  const result = await fetchAPIFee.json()
  if (result.code === 200) {
    shippingFee.value = result.data.total
  }
}

onMounted(() => {
  FetchCart()
  fetchAddress()
  fetchCustomer()
})

// Tách sản phẩm và combo ra riêng
const ProductList = computed(() => {
  return cartItems.value.filter((item) => item.maCombo === null)
})
const ComboList = computed(() => {
  return cartItems.value.filter((item) => item.maCombo !== null)
})

const fetchCoupon = async () => {
  try {
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if (validateToken == true) {
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if (readtoken) {
        IdUser = readtoken.IdUser
      } else {
        router.push('/Login')
        return
      }
    }
    if(couponCode.value == ''){
      discount.value = 0
      couponCode.value = ''
      return;
    }
    const response = await fetch(
      getApiUrl+`/api/Checkout/GetDiscountCoupon?maUser=${IdUser}&&couponcode=${couponCode.value}&&originalPrice=${SumCart.value}`,
      {
        headers: {
          'Content-type': 'application/json',
          Authorization: `Bearer ${accesstoken}`,
        },
      }
    )
    if (response.status == 401) {
      Swal.fire({
        icon: 'error',
        title: 'Phiên của bạn đã hết hoặc bạn chưa đăng nhập, vui lòng đăng nhập lại!',
        timer: 2000,
        showConfirmButton: false,
      })
      router.push('/Login')
      return
    }
    if (!response.ok) {
      const errorMessage = response.message
      throw new Error(errorMessage)
    }
    const result = await response.json()
    if (result.success) {
      discount.value = result.discount
      Swal.fire(result.message, '', 'success')
    } else {
      Swal.fire(result.message, '', 'error')
      discount.value = 0
      couponCode.value = ''
    }
  } catch (error) {
    console.error('Lỗi khi áp dụng mã coupon:', error)
    couponCode.value = ''
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: 'Đã xảy ra lỗi. Vui lòng thử lại.',
    })
  }
}

const proceedToCheckout = async () => {
  try {
    if (!validateForm()) {
      return
    }

    const detailComboOrderRequests = []
    const comboItems = cartItems.value.filter((p) => p.maCombo !== null)
    comboItems.forEach((p) => {
      p.cartDetailCombos.forEach((detailcombo) => {
        detailComboOrderRequests.push({
          maCombo: p.maCombo,
          maCTSp: detailcombo.maCTSp,
          soLuong: detailcombo.soLuong,
          donGia: detailcombo.donGia,
        })
      })
    })
    const cthoadons = []
    cartItems.value.forEach((detail) => {
      cthoadons.push({
        maCtsp: detail.maCtsp,
        soLuong: detail.soLuong,
        donGia: detail.donGia,
        giamGia: detail.giamGia ?? 0,
        maCombo: detail.maCombo,
      })
    })
    const validateToken = await ValidateToken(accesstoken, refreshtoken)
    if (validateToken == true) {
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if (readtoken) {
        IdUser = readtoken.IdUser
      } else {
        router.push('/Login')
        return
      }
    }
    const provinceName = provinces.value.filter(p => p.ProvinceID == userInfo.value.provinceId)[0].ProvinceName
    const districtName = districts.value.filter(p => p.DistrictID == userInfo.value.districtId)[0].DistrictName
    const wardName = wards.value.filter(p => p.WardCode == userInfo.value.wardCode)[0].WardName
    console.log(provinceName + "-" + districtName + "-" + wardName)
    Swal.fire({
      title: 'Bạn có chắc chắn muốn đặt hàng với các sản phẩm này không ?',
      showCancelButton: true,
      confirmButtonText: 'Xác nhận đặt hàng',
      cancelButtonText: 'Hủy',
    }).then(async (result) => {
      if (result.isConfirmed) {
        console.log('Dữ liệu hợp lệ, chuẩn bị gửi request')
        const orderData = {
          maKh: IdUser,
          diaChiNhanHang: userInfo.value.diaChi + " " + wardName + " " + districtName + " " + provinceName,
          hinhThucTt: shippingMethod.value,
          moTa: userInfo.value.moTa,
          hoTen: userInfo.value.hoTen,
          sdt: userInfo.value.soDienThoai,
          phiVanChuyen: shippingFee.value,
          tienGoc: SumCart.value,
          maCoupon: couponCode.value == '' ? null : couponCode.value,
          detailCombo_OrderResquests: detailComboOrderRequests,
          cthoadons: cthoadons,
        }
        if (orderData.hinhThucTt.toLowerCase() == 'vnpay') {

          //const CreatePaymentUrl = await fetch(`https://localhost:7139/api/VNPAY`, {

          const total = orderData.tienGoc + orderData.phiVanChuyen - orderData.giamGiaCoupon
          const CreatePaymentUrl = await fetch(getApiUrl+`/api/VNPAY`, {

            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify(orderData),
          })
          const responseVNPAY = await CreatePaymentUrl.text()
          window.location.href = responseVNPAY
        } else {
          const response = await fetch( getApiUrl+`/api/Checkout`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
              Authorization: `Bearer ${accesstoken}`,
            },
            body: JSON.stringify(orderData),
          })
          if (response.status == 401) {
            Swal.fire({
              icon: 'error',
              title: 'Phiên của bạn đã hết hoặc bạn chưa đăng nhập, vui lòng đăng nhập lại!',
              timer: 2000,
              showConfirmButton: false,
            })
            router.push('/Login')
            return
          }
          if (!response.ok) {
            const errorMessage = await response.text()
            throw new Error(errorMessage)
          }
          const result = await response.json()
          if (result.success) {
            Swal.fire({
              icon: 'success',
              title: result.message,
              timer: 2000,
              showConfirmButton: false,
            })
            window.dispatchEvent(new CustomEvent('updateCart'))
            setTimeout(function () {
              router.push('/')
            }, 2000)
          } else {
            Swal.fire(result.message, '', 'error')
          }
        }
      }
    })
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