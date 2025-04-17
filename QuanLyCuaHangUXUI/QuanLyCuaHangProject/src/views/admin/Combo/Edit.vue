<script setup>
import { ref, watch, onMounted } from 'vue'
import Swal from 'sweetalert2'
import { GetApiUrl } from '@constants/api'
const props = defineProps({
  ListProduct: Object,
  Combo: Object,
})
const initialCombo = ref(null)
const comboEdit = ref({
  tenCombo: '',
  hinh: null,
  soTienGiam: 0,
  phanTramGiam: 0,
  soLuong: 1,
  moTa: '',
  isDelete: false,
  chitietcombos: [
    {
      maSp: '',
      soLuongSp: 1,
    },
  ],
})

onMounted(() => {
  initialCombo.value = {
    tenCombo: props.Combo.tenCombo,
    hinh: props.Combo.hinh,
    soTienGiam: props.Combo.soTienGiam,
    phanTramGiam: props.Combo.phanTramGiam,
    soLuong: props.Combo.soLuong,
    moTa: props.Combo.moTa,
    isDelete: false,
    chitietcombos: props.Combo.chitietcombos.map((detail) => ({
      ...detail,
    })),
  }
  comboEdit.value = {
    ...initialCombo.value,
    chitietcombos: initialCombo.value.chitietcombos.map((detail) => ({ ...detail })),
  }

  console.log(comboEdit.value)
})
const blockNegativeNumbers = (event) => {
  if (event.key === '-') {
    event.preventDefault()
  }
}
// Validate SoLuong, PhanTramGiam, SoTienGiam
watch(
  comboEdit,
  (newcomboEdit) => {
    if (newcomboEdit.soLuong < 1 && newcomboEdit.soLuong != '') {
      newcomboEdit.soLuong = 1
    }
    if (newcomboEdit.soTienGiam < 0) {
      newcomboEdit.soTienGiam = 0
    }
    if (newcomboEdit.phanTramGiam < 0) {
      newcomboEdit.phanTramGiam = 0
    }
  },
  { deep: true }
)
// Reset soTienGiam khi nhập vào phanTramGiam
function resetSoTienGiam() {
  combo.value.soTienGiam = 0
}

// Reset phanTramGiam khi nhập vào soTienGiam
function resetPhanTramGiam() {
  combo.value.phanTramGiam = 0
}
// Xử lí hình ảnh
function handleFileChange(comboEdit, event) {
  const file = event.target.files[0]
  comboEdit.hinh = file
}
// Xóa chi tiết combo
function removeDetailCombo(index) {
  if (comboEdit.value.chitietcombos.length > 1) {
    comboEdit.value.chitietcombos.splice(index, 1)
  }
}
//Thêm chi tiết combo
function addDetailCombo() {
  comboEdit.value.chitietcombos.push({
    maSp: '',
    soLuongSp: 1,
  })
}

//Xử lí việc hủy thay đổi
const cancelEdit = () => {
  const comboChanged = JSON.stringify(comboEdit.value) != JSON.stringify(initialCombo.value)
  if (comboChanged) {
    Swal.fire({
      title: 'Bạn có muốn lưu các thay đổi này không ?',
      showDenyButton: true,
      showCancelButton: true,
      confirmButtonText: 'Có',
      denyButtonText: `Tiếp tục chỉnh sửa`,
      cancelButtonText: 'Hủy',
    }).then((result) => {
      /* Read more about isConfirmed, isDenied below */
      if (result.isConfirmed) {
        try {
          UpdateCombo()
        } catch (error) {
          console.log(error)
        }
      } else if (result.isDenied) {
        Swal.clickCancel()
      } else {
        comboEdit.value = {
          ...initialCombo.value,
          chitietcombos: initialCombo.value.chitietcombos.map((detail) => ({ ...detail })),
        }
        var instanceModal = document.getElementById(`comboEditModal_${props.Combo.maCombo}`)
        const closeButton = instanceModal.querySelector('[data-bs-dismiss="modal"]')
        if (closeButton) {
          closeButton.click()
        }
      }
    })
  } else {
    var instanceModal = document.getElementById(`comboEditModal_${props.Combo.maCombo}`)
    const closeButton = instanceModal.querySelector('[data-bs-dismiss="modal"]')
    if (closeButton) {
      closeButton.click()
    }
  }
}

//Cập nhật lên server
async function UpdateCombo() {
  //Tạo FormData
  try {
    let isValid = true
    const hasDuplicates = comboEdit.value.chitietcombos.some(
      (item, index, arr) => arr.findIndex((obj) => obj.maSp === item.maSp) !== index
    )

    if (hasDuplicates) {
      Swal.fire('Vui lòng không để hai sản phẩm trùng lặp trong combo', '', 'error')
      isValid = false
    }
    if (!isValid) {
      return
    }
    const formData = new FormData()
    formData.append('tenCombo', comboEdit.value.tenCombo)
    if (comboEdit.value.hinh) {
      formData.append('hinh', comboEdit.value.hinh)
    }
    formData.append('soLuong', comboEdit.value.soLuong)
    formData.append('soTienGiam', comboEdit.value.soTienGiam)
    formData.append('phanTramGiam', comboEdit.value.phanTramGiam)
    formData.append('moTa', comboEdit.value.moTa)
    formData.append('isDelete', false)
    comboEdit.value.chitietcombos.forEach((detail, index) => {
      formData.append(`chitietcombos[${index}].maSp`, detail.maSp)
      formData.append(`chitietcombos[${index}].soLuongSp`, detail.soLuongSp)
    })
    //Gửi resquest
    const response = await fetch(GetApiUrl()+`/api/Combos/${props.Combo.maCombo}`, {
      method: 'PUT',
      body: formData,
    })
    if (!response.ok) {
      throw new Error('Failed to add combo' + response.status)
    }
    const result = await response.json()
    if (result.success) {
      Swal.fire('Đã cập nhật thông tin combo sản phẩm', '', 'success')
      setTimeout(() => {
        window.location.reload()
      }, 2000)
    } else {
      throw new Error('Failed to add combo')
    }
  } catch (error) {
    console.error('Error:', error)
  }
}
</script>
<template>
  <div
    class="modal fade"
    :id="`comboEditModal_${props.Combo.maCombo}`"
    tabindex="-1"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
  >
    <button class="btn-close" data-bs-dismiss="modal"></button>
    <div class="modal-dialog modal-xl text-start">
      <div class="modal-content">
        <div class="modal-header bg-primary text-white">
          <h5 class="modal-title">Sửa thông tin combo</h5>
          <button @click="cancelEdit()" type="button" style="background: none; border: 0px">
            X
          </button>
        </div>

        <div class="modal-body p-4 data-editCombo">
          <form @submit.prevent>
            <!-- Tên combo -->
            <div class="mb-3">
              <label class="form-label">Tên combo</label>
              <input type="text" class="form-control" v-model="comboEdit.tenCombo" />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Mô tả -->
            <div class="mb-3">
              <label for="moTa" class="form-label">Mô tả</label>
              <textarea
                v-model="comboEdit.moTa"
                class="form-control"
                id="moTa"
                rows="3"
                placeholder="Nhập mô tả combo"
              ></textarea>
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Số lượng combo -->
            <div class="mb-3">
              <label class="form-label">Số lượng</label>
              <input
                @keydown="blockNegativeNumbers"
                v-model="comboEdit.soLuong"
                type="number"
                class="form-control"
                min="1"
              />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Chi tiết combo -->
            <div class="mb-3">
              <label class="form-label">Chi tiết combo</label>
              <div
                class="card mb-3"
                v-for="(detail, index) in comboEdit.chitietcombos"
                :key="index"
              >
                <div class="card-body">
                  <div class="row">
                    <div class="col-md-6">
                      <label class="form-label">Sản phẩm</label>
                      <select v-model="detail.maSp" class="form-select">
                        <option disabled value="">Chọn sản phẩm</option>
                        <option
                          v-for="product in props.ListProduct"
                          :key="product.maSp"
                          :value="product.maSp"
                          :selected="product.maSp === detail.maSp"
                        >
                          {{ product.tenSanPham }}
                        </option>
                      </select>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label">Số lượng</label>
                      <input
                        type="number"
                        class="form-control"
                        v-model="detail.soLuongSp"
                        min="1"
                        @keydown="blockNegativeNumbers"
                      />
                    </div>
                  </div>
                  <button
                    @click="removeDetailCombo(index)"
                    type="button"
                    class="btn btn-danger btn-sm mt-2"
                  >
                    Xóa chi tiết
                  </button>
                </div>
              </div>
              <!-- Nút thêm chi tiết combo -->
              <button @click="addDetailCombo()" type="button" class="btn btn-secondary">
                Thêm chi tiết combo
              </button>
            </div>

            <!-- Phần trăm giảm -->
            <div class="mb-3">
              <label class="form-label">Phần trăm giảm</label>
              <input
                type="number"
                class="form-control"
                v-model="comboEdit.phanTramGiam"
                min="0"
                @input="resetSoTienGiam"
              />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Số tiền giảm -->
            <div class="mb-3">
              <label class="form-label">Số tiền giảm</label>
              <input
                type="number"
                class="form-control"
                v-model="comboEdit.soTienGiam"
                min="0"
                @input="resetPhanTramGiam"
              />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Hình ảnh -->
            <div>
              <label class="form-label">Hình ảnh</label>
              <input
                @change="handleFileChange(comboEdit, $event)"
                type="file"
                class="form-control"
                accept="image/*"
              />
              <img
                :src="GetApiUrl()+`/HinhAnh/Food_Drink/${comboEdit.hinh}`"
                alt="Ảnh combo"
                class="img-fluid mt-2"
                style="max-width: 100px; height: auto"
              />
              <label style="color: red" class="error-message imageMessage"></label>
            </div>

            <!-- Nút lưu -->
            <div class="text-end">
              <button type="button" @click="UpdateCombo()" class="btn btn-primary">
                Lưu thay đổi
              </button>
            </div>
          </form>
        </div>
      </div>
    </div>
  </div>
</template>
  
  <style scoped>
.card {
  border: 1px solid #ddd;
}
.modal-xl {
  max-width: 90%;
}
.btn-danger {
  font-size: 12px;
  padding: 2px 6px;
}
</style>