<script setup>
import { ref, watch } from 'vue'
import Swal from 'sweetalert2'
const props = defineProps({
  ListProduct: Object,
})
const combo = ref({
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

//Thêm dòng điền chi tiết combo
function addDetailCombo() {
  combo.value.chitietcombos.push({
    maSp: '',
    soLuongSp: 1,
  })
}

// Validate SoLuong, PhanTramGiam, SoTienGiam
watch(
  combo,
  (newcombo) => {
    if (newcombo.soLuong < 1 && newcombo.soLuong != '') {
      newcombo.soLuong = 1
    }
    if (newcombo.soTienGiam < 0) {
      newcombo.soTienGiam = 0
    }
    if (newcombo.phanTramGiam < 0) {
      newcombo.phanTramGiam = 0
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
const blockNegativeNumbers = (event) => {
  if (event.key === '-') {
    event.preventDefault()
  }
}
// Xử lí hình ảnh
function handleFileChange(combo, event) {
  const file = event.target.files[0]
  combo.hinh = file
}
// Xóa chi tiết combo
function removeDetailCombo(index) {
  if (combo.value.chitietcombos.length > 1) {
    combo.value.chitietcombos.splice(index, 1);
  } 
}
// Xử lú gửi data về server
const addCombo = async () => {
  console.log(combo.value)
  try {
    let isValid = true;

    // Check trùng lặp chi tiết combo
    const hasDuplicates = combo.value.chitietcombos.some((detail, index, arr) =>
      arr.findIndex(d => d.maSp === detail.maSp) !== index
    );
    if (hasDuplicates) {
      isValid = false;
      Swal.fire('Không được để trùng lặp sản phẩm trong chi tiết combo!', '', 'error');
    }

    const form_input_combo = document.querySelectorAll('.data-createCombo .mb-3')
    form_input_combo.forEach((element) => {
      var inputValueCombo = element.querySelector('.form-control, .form-select')
      var messageErrorCombo = element.querySelector('.error-message')
      if (messageErrorCombo) {
        messageErrorCombo.textContent = ''
      }
      if (inputValueCombo.value.trim() == '') {
        const label = element.querySelector('.form-label')
        if (messageErrorCombo) {
          messageErrorCombo.textContent = `Không được để trống ${label.textContent}`
          isValid = false
        }
      }
      if (combo.value.hinh == null) {
        document.querySelector('.imageMessage').textContent = `Không được để trống hình ảnh`
        isValid = false
      }
    })
    combo.value.chitietcombos.forEach((p) => {
      if (p.soLuongSp == '') {
        isValid = false
        Swal.fire('Số lượng sản phẩm trong chi tiết combo không được để trống', '', 'error')
      }
      if (p.maSp == '') {
        isValid = false
        Swal.fire('Combo phải chứa tối thiểu một sản phẩm!', '', 'error')
      }
    })

    if (isValid == false) {
      return
    }
    //Tạo FormData
    const formData = new FormData()
    formData.append('tenCombo', combo.value.tenCombo)
    if (combo.value.hinh) {
      formData.append('hinh', combo.value.hinh)
    }
    formData.append('soLuong', combo.value.soLuong)
    formData.append('soTienGiam', combo.value.soTienGiam)
    formData.append('phanTramGiam', combo.value.phanTramGiam)
    formData.append('moTa', combo.value.moTa)
    formData.append('isDelete', combo.value.isDelete)
    combo.value.chitietcombos.forEach((detail, index) => {
      formData.append(`chitietcombos[${index}].maSp`, detail.maSp)
      formData.append(`chitietcombos[${index}].soLuongSp`, detail.soLuongSp)
    })

    //Gửi resquest
    const response = await fetch(GetApiUrl()+'/api/Combos', {
      method: 'POST',
      body: formData,
    })

    if (!response.ok) {
      throw new Error('Failed to add combo')
    }

    const result = await response.json()
    if (result.success) {
      Swal.fire('Đã thêm combo mới thành công', '', 'success')
      setTimeout(() => {
        window.location.reload()
      }, 2000)
    } else {
      throw new Error('Failed to add combo')
    }
  } catch (error) {
    console.error('Error:', error.message)
  }
}
</script>
<template>
  <!-- Modal -->
  <div
    class="modal fade"
    id="exampleModal"
    tabindex="-1"
    aria-labelledby="exampleModalLabel"
    aria-hidden="true"
  >
    <div class="modal-dialog modal-xl">
      <div class="modal-content">
        <div class="modal-header">
          <h5 class="modal-title" id="exampleModalLabel">Thêm combo</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body data-createCombo">
          <form>
            <!-- Tên combo -->
            <div class="mb-3">
              <label for="tenCombo" class="form-label">Tên combo</label>
              <input
                type="text"
                class="form-control"
                id="tenCombo"
                v-model="combo.tenCombo"
                placeholder="Nhập tên combo"
              />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Mô tả -->
            <div class="mb-3">
              <label for="moTa" class="form-label">Mô tả</label>
              <textarea
                class="form-control"
                id="moTa"
                rows="3"
                v-model="combo.moTa"
                placeholder="Nhập mô tả combo"
              ></textarea>
              <label style="color: red" class="error-message"></label>
            </div>
            <!-- Số lượng combo -->
            <div class="mb-3">
              <label for="SoLuong" class="form-label">Số lượng</label>
              <input
                type="number"
                class="form-control"
                id="SoLuong"
                v-model="combo.soLuong"
                min="1"
                @keydown="blockNegativeNumbers"
              />
              <label style="color: red" class="error-message soluongMessage"></label>
            </div>

            <!-- Chi tiết combo -->
            <div>
              <label class="form-label">Chi tiết combo</label>
              <div v-for="(detail, index) in combo.chitietcombos" :key="index" class="card mb-3">
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
                        >
                          {{ product.tenSanPham }}
                        </option>
                      </select>
                    </div>
                    <div class="col-md-6">
                      <label class="form-label">Số lượng</label>
                      <input
                        v-model="detail.soLuongSp"
                        type="number"
                        class="form-control"
                        min="1"
                        @keydown="blockNegativeNumbers"
                      />
                    </div>
                    <!-- Xóa chi tiết combo -->
                    <div style="margin-top: 10px;" class="col-md-2 d-flex align-items-end">
                      <button
                        type="button"
                        class="btn btn-danger btn-sm"
                        @click="removeDetailCombo(index)"
                      >
                        Xóa
                      </button>
                    </div>
                  </div>
                </div>
              </div>
              <!-- Nút thêm chi tiết combo (tĩnh, không có chức năng) -->
              <button type="button" @click="addDetailCombo()" class="btn btn-secondary">
                Thêm chi tiết combo
              </button>
            </div>
            <!-- Phần trăm giảm combo -->
            <div style="margin-top: 30px">
              <label for="phantramCombo" class="form-label">Phần trăm giảm</label>
              <input
                type="number"
                class="form-control"
                id="phantramCombo"
                v-model="combo.phanTramGiam"
                min="0"
                @input="resetSoTienGiam"
              />
              <label style="color: red" class="error-message"></label>
            </div>
            <!-- Sô tiền giảm giảm combo -->
            <div style="">
              <label for="sotienGiam" class="form-label">Số tiền giảm</label>
              <input
                type="number"
                class="form-control"
                id="sotienGiam"
                v-model="combo.soTienGiam"
                min="0"
                @input="resetPhanTramGiam"
              />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Hình ảnh -->
            <div class="mt-3">
              <label class="form-label">Hình ảnh</label>
              <input
                type="file"
                @change="handleFileChange(combo, $event)"
                class="form-control"
                accept="image/*"
              />
              <label style="color: red" class="error-message imageMessage"></label>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button type="button" @click="addCombo()" class="btn btn-primary">Xác nhận</button>
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
</style>