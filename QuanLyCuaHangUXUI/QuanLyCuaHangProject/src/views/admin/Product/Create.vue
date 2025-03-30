<script setup>
import { ref, watch } from 'vue'
import Swal from 'sweetalert2'
const props = defineProps({
  categories: Object,
})
// Upload file lên server
const uploadImage = async (file) => {
  try {
    const formData = new FormData()
    formData.append('file', file)
    const response = await fetch('https://localhost:7139/api/UploadImage', {
      method: 'POST',
      body: formData,
    })
    if (!response.ok) {
      throw new Error(`Lỗi khi upload ảnh: ${response.status} ${response.statusText}`)
    }
  } catch (error) {
    console.error('Error uploading image:', error.message)
  }
}
const handleFileChange = async (variant, event) => {
  variant.images = Array.from(event.target.files)
  for (const file of variant.images) {
    await uploadImage(file)
  }
}
// Dữ liệu sản phẩm
const product = ref({
  maDanhMuc: '',
  tenSanPham: '',
  moTa: '',
  isDelete: false,
})

// Danh sách biến thể
const variants = ref([
  {
    kichThuoc: '',
    huongVi: '',
    soLuongTon: 1,
    donGia: 0,
    images: [''],
  },
])

watch(
  variants,
  (newVariants) => {
    newVariants.forEach((variant) => {
      if (variant.soLuongTon != '' && variant.soLuongTon < 1) {
        variant.soLuongTon = 1
      }
      if (variant.donGia != '' && variant.donGia < 0) {
        variant.donGia = 0
      }
    })
  },
  { deep: true }
)

// Danh sách danh mục (tĩnh)
// const categories = ref([])
// async function fetchCategory() {
//   const response = await fetch('https://localhost:7139/api/Categories', {
//     method: 'GET',
//     headers: {
//       'Content-Type': 'application/json',
//     },
//   })
//   if (!response.ok) {
//     throw new Error('Error' + response.status)
//   }
//   const dataCategories = await response.json()
//   categories.value = dataCategories
//   console.log(categories.value)
// }
// fetchCategory()
// Hàm thêm biến thể mới
const addVariant = () => {
  variants.value.push({
    kichThuoc: '',
    huongVi: '',
    soLuongTon: 1,
    donGia: 0,
    images: [''],
  })
}

// Hàm xóa biến thể
const removeVariant = (index) => {
  if (variants.value.length > 1) {
    variants.value.splice(index, 1)
  }
}

// Xử lí gửi thông tin đến server
const submitForm = async () => {
  try {
    let isValid = true

    const hasDuplicates = variants.value.some(
      (item, index, arr) =>
        arr.findIndex((obj) => obj.kichThuoc === item.kichThuoc && obj.huongVi === item.huongVi) !==
        index
    )

    if(hasDuplicates){
      Swal.fire('Vui lòng không để hai dòng biến thể trùng lặp', '', 'error')
      isValid = false;
    }

    const form_input_Product = document.querySelectorAll('.data-createProduct .mb-3')
    form_input_Product.forEach((element) => {
      var inputValueProduct = element.querySelector('.form-control, .form-select')
      var messageErrorProduct = element.querySelector('.error-message')
      if (messageErrorProduct) {
        messageErrorProduct.textContent = ''
      }
      if (inputValueProduct.value.trim() == '') {
        const label = element.querySelector('.form-label')
        if (messageErrorProduct) {
          messageErrorProduct.textContent = `Không được để trống ${label.textContent}`
          isValid = false
        }
      }
    })

    variants.value.forEach(e => {
      if(e.donGia <= 0){
        Swal.fire('Đơn giá biến thể sản phẩm phải lớn hơn 0 ', '', 'error')
        isValid = false
      }
      if(e.soLuongTon == ''){
        Swal.fire('Số lượng tồn biến thể sản phẩm không được để trống', '', 'error')
        isValid = false
      }
    })

    if (isValid == false) {
      return
    }

    Swal.fire({
      title: 'Bạn có muốn thêm sản phẩm này ?',
      showCancelButton: true,
      confirmButtonText: 'Xác nhận',
      cancelButtonText: 'Hủy',
    }).then(async (result) => {
      if (result.isConfirmed) {
        console.log('Dữ liệu hợp lệ, chuẩn bị gửi request')
        const content = {
          maDanhMuc: product.value.maDanhMuc,
          tenSanPham: product.value.tenSanPham,
          moTa: product.value.moTa,
          isDelete: false,
          detailProductCreateRequestDTOs: variants.value.map((variant) => ({
            kichThuoc: variant.kichThuoc,
            huongVi: variant.huongVi,
            soLuongTon: variant.soLuongTon,
            donGia: variant.donGia,
            imageProductRequestDTOs: variant.images.map((file) => ({
              tenHinhAnh: file.name,
            })),
          })),
        }

        const response = await fetch('https://localhost:7139/api/Products', {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
          },
          body: JSON.stringify(content),
        })
        if (!response.ok) {
          throw new Error('Failed to add product')
        }
        const result = await response.json()
        if (result.success == true) {
          Swal.fire('Đã thêm sản phẩm mới thành công', '', 'success')
          setTimeout(function () {
            window.location.reload()
          }, 2000)
        }
        throw new Error('Failed to add product')
      }
    })
  } catch (error) {
    console.error('Error:', error.message)
  }
}

const blockNegativeNumbers = (event) => {
  if (event.key === '-') {
    event.preventDefault()
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
          <h5 class="modal-title" id="exampleModalLabel">Thêm sản phẩm</h5>
          <button
            type="button"
            class="btn-close"
            data-bs-dismiss="modal"
            aria-label="Close"
          ></button>
        </div>
        <div class="modal-body data-createProduct">
          <form @submit.prevent>
            <!-- Tên sản phẩm -->
            <div class="mb-3">
              <label for="tenSanPham" class="form-label">Tên sản phẩm</label>
              <input
                type="text"
                class="form-control"
                id="tenSanPham"
                v-model="product.tenSanPham"
                placeholder="Nhập tên sản phẩm"
                required
              />
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Danh mục -->
            <div class="mb-3">
              <label for="maDanhMuc" class="form-label">Danh mục</label>
              <select class="form-select" id="maDanhMuc" v-model="product.maDanhMuc" required>
                <option value="" disabled>Chọn danh mục</option>
                <option
                  v-for="category in props.categories"
                  :key="category.maDanhMuc"
                  :value="category.maDanhMuc"
                >
                  {{ category.tenDanhMuc }}
                </option>
              </select>
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Mô tả -->
            <div class="mb-3">
              <label for="moTa" class="form-label">Mô tả</label>
              <textarea
                class="form-control"
                id="moTa"
                v-model="product.moTa"
                rows="3"
                placeholder="Nhập mô tả sản phẩm"
              ></textarea>
              <label style="color: red" class="error-message"></label>
            </div>

            <!-- Biến thể sản phẩm -->
            <div>
              <label class="form-label">Biến thể sản phẩm</label>
              <div v-for="(variant, index) in variants" :key="index" class="card mb-3">
                <div class="card-body">
                  <div class="row">
                    <div class="col-md-3">
                      <label class="form-label">Kích thước</label>
                      <input
                        type="text"
                        class="form-control size"
                        v-model="variant.kichThuoc"
                        placeholder="Nhập kích thước"
                      />
                    </div>
                    <div class="col-md-3">
                      <label class="form-label flavor">Hương vị</label>
                      <input
                        type="text"
                        class="form-control"
                        v-model="variant.huongVi"
                        placeholder="Nhập hương vị"
                      />
                    </div>
                    <div class="col-md-3">
                      <label class="form-label">Số lượng tồn</label>
                      <input
                        type="number"
                        class="form-control"
                        v-model="variant.soLuongTon"
                        @keydown="blockNegativeNumbers"
                        min="1"
                      />
                    </div>
                    <div class="col-md-3">
                      <label class="form-label">Đơn giá</label>
                      <input
                        type="number"
                        class="form-control"
                        v-model="variant.donGia"
                        @keydown="blockNegativeNumbers" 
                        min="0"
                      />
                    </div>
                  </div>

                  <!-- Hình ảnh -->
                  <div class="mt-3">
                    <label class="form-label">Hình ảnh</label>
                    <input
                      multiple
                      type="file"
                      class="form-control"
                      accept="image/*"
                      @change="handleFileChange(variant, $event)"
                    />
                  </div>

                  <!-- Nút xóa biến thể -->
                  <button
                    v-if="variants.length > 1"
                    type="button"
                    class="btn btn-danger btn-sm mt-2"
                    @click="removeVariant(index)"
                  >
                    Xóa biến thể
                  </button>
                </div>
              </div>

              <!-- Nút thêm biến thể -->
              <button type="button" class="btn btn-secondary" @click="addVariant">
                Thêm biến thể
              </button>
            </div>
          </form>
        </div>
        <div class="modal-footer">
          <button @click="submitForm" type="button" class="btn btn-primary">Xác nhận</button>
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