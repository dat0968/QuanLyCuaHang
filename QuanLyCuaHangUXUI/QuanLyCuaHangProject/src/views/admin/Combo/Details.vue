<script setup>
import { ref, onMounted } from 'vue'
import Swal from 'sweetalert2'
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
</script>
<template>
  <div
    class="modal fade"
    :id="`comboDetailModal_${props.Combo.maCombo}`"
    tabindex="-1"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
  >
    <div class="modal-dialog modal-xl text-start">
      <div class="modal-content">
        <div class="modal-header bg-primary text-white">
          <h5 class="modal-title"> Thông tin combo</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>

        <div class="modal-body p-4 data-editCombo">
          <form @submit.prevent>
            <!-- Tên combo -->
            <div class="mb-3">
              <label class="form-label">Tên combo</label>
              <input readonly type="text" class="form-control" v-model="comboEdit.tenCombo" />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Mô tả -->
            <div class="mb-3">
              <label for="moTa" class="form-label">Mô tả</label>
              <textarea
                readonly
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
              <input readonly v-model="comboEdit.soLuong" type="number" class="form-control" min="1" />
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
                      <input
                        readonly
                        type="text"
                        class="form-control"
                        v-model="detail.tenSp"
                        min="1"
                      />
                    </div>
                    <div class="col-md-6">
                      <label class="form-label">Số lượng</label>
                      <input
                        readonly
                        type="number"
                        class="form-control"
                        v-model="detail.soLuongSp"
                        min="1"
                      />
                    </div>
                  </div>
                 
                </div>
              </div>
            </div>

            <!-- Phần trăm giảm -->
            <div class="mb-3">
              <label class="form-label">Phần trăm giảm</label>
              <input readonly type="number" class="form-control" v-model="comboEdit.phanTramGiam" min="0" />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Số tiền giảm -->
            <div class="mb-3">
              <label class="form-label">Số tiền giảm</label>
              <input readonly type="number" class="form-control" v-model="comboEdit.soTienGiam" min="0" />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Hình ảnh -->
            <div>
              <label class="form-label">Hình ảnh</label>
              <img
                :src="GetApiUrl()+`/HinhAnh/Food_Drink/${comboEdit.hinh}`"
                alt="Ảnh combo"
                class="img-fluid mt-2"
                style="max-width: 100px; height: auto"
              />
              <label style="color: red" class="error-message imageMessage"></label>
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