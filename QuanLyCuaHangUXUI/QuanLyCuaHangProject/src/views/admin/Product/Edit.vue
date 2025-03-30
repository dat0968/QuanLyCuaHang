  <script setup>
import 'bootstrap/dist/js/bootstrap.bundle'
import { onMounted, ref, watch } from 'vue'
import { Swiper, SwiperSlide } from 'swiper/vue'
import { Navigation, Pagination } from 'swiper/modules'
import Swal from 'sweetalert2'
const Props = defineProps({
  Product: Object,
  categories: Object,
})

/* Dấu ... nhằm tránh không cho editedVariants tham chiếu trực tiếp đến Props.Product.chitietsanphams 
  dẫn đến việc khi thay đổi editedVariants sẽ dẫn tới thay đổi cả Props.Product.chitietsanphams */
const editedVariants = ref([])
const editedProduct = ref({
  maSp: '',
  maDanhMuc: '',
  tenSanPham: '',
  moTa: '',
  isDelete: false,
})
const initialVariants = ref([])
const initialProduct = ref(null)
onMounted(() => {
  // Deep copy sản phẩm
  initialProduct.value = {
    maSp: Props.Product.maSp,
    maDanhMuc: Props.Product.maDanhMuc,
    tenSanPham: Props.Product.tenSanPham,
    moTa: Props.Product.moTa,
    isDelete: Props.Product.isDelete,
  }
  editedProduct.value = { ...initialProduct.value }

  //Deep copy biến thể
  initialVariants.value = Props.Product.chitietsanphams.map((variant) => ({
    ...variant,
    oldImages: variant.hinhanhs ? [...variant.hinhanhs] : [],
    newImages: [],
  }))
  editedVariants.value = initialVariants.value.map((variant) => ({
    ...variant,
    oldImages: [...(variant.oldImages || [])],
    newImages: [],
  }))

  console.log('CT', editedVariants.value)
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
    const result = await response.json()
  } catch (error) {
    console.error('Error uploading image:', error.message)
  }
}

// Xử lý upload ảnh mới
const handleFileChange = async (variant, event) => {
  const files = Array.from(event.target.files)
  for (const file of files) {
    await uploadImage(file)
  }

  const newImages = files.map((file) => ({
    tenHinhAnh: file.name,
  }))
  variant.newImages = [...(variant.newImages || []), ...newImages]
}

// Xử lí việc hủy thay đổi
const cancelEdit = () => {
  const productChanged = JSON.stringify(editedProduct.value) != JSON.stringify(initialProduct.value)
  const variantsChanged =
    JSON.stringify(editedVariants.value) != JSON.stringify(initialVariants.value)
  console.log(productChanged)
  console.log(variantsChanged)
  if (productChanged == true || variantsChanged == true) {
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
        Swal.fire('Đã cập nhật thông tin sản phẩm', '', 'success')
        UpdateProduct()
      } else if (result.isDenied) {
        // editedProduct.value = JSON.parse(JSON.stringify(initialProduct.value))
        // editedVariants.value = JSON.parse(JSON.stringify(initialVariants.value))
        // Swal.fire('Thông tin được giữ nguyên', '', 'info')
        // setTimeout(function () {
        //   window.location.reload()
        // }, 2000)
        Swal.clickCancel()
      } else {
        editedVariants.value = initialVariants.value.map((variant) => ({
          ...variant,
          oldImages: [...(variant.oldImages || [])],
          newImages: [],
        }))
        editedProduct.value = { ...initialProduct.value }
        // var instanceModal = document.getElementById(`productEditModal_${Props.Product.maSp}`)
        // instanceModal.classList.remove('show')
        // instanceModal.style.display = 'none'
        // document.querySelectorAll('.modal-backdrop.fade.show').forEach((e) => e.remove())
        var instanceModal = document.getElementById(`productEditModal_${Props.Product.maSp}`)
        const closeButton = instanceModal.querySelector('[data-bs-dismiss="modal"]')
        if (closeButton) {
          closeButton.click() // Kích hoạt nút đóng modal
        }
      }
    })
  } else {
    var instanceModal = document.getElementById(`productEditModal_${Props.Product.maSp}`)
    //instanceModal.classList.remove('show')
    const closeButton = instanceModal.querySelector('[data-bs-dismiss="modal"]')
    if (closeButton) {
      closeButton.click() // Kích hoạt nút đóng modal
    }
    //document.querySelectorAll('.modal-backdrop.fade.show').forEach((e) => e.remove())
  }
}

// Hàm thêm dòng điền biến thể
const addVariant = () => {
  editedVariants.value.push({
    kichThuoc: '',
    huongVi: '',
    soLuongTon: 1,
    donGia: 0,
    oldImages: [],
    newImages: [],
  })
}
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

const deleteImage = (variant, imageIndex, isOldImage) => {
  if (isOldImage) {
    variant.oldImages.splice(imageIndex, 1)
  } else {
    variant.newImages.splice(imageIndex, 1)
  }
}

watch(
  editedVariants,
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

// Hàm xóa biến thể
const removeVariant = (index) => {
  if (editedVariants.value.length > 1) {
    editedVariants.value.splice(index, 1)
  }
}

// Lưu thay đổi lên server

async function UpdateProduct() {
  try {
    let isValid = true
    const hasDuplicates = editedVariants.value.some(
      (item, index, arr) =>
        arr.findIndex((obj) => obj.kichThuoc === item.kichThuoc && obj.huongVi === item.huongVi) !==
        index
    )

    if (hasDuplicates) {
      Swal.fire('Vui lòng không để hai dòng biến thể trùng lặp', '', 'error')
      isValid = false
    }
    const form_input_Product = document.querySelectorAll('.data-editProduct .mb-3')
    form_input_Product.forEach((element) => {
      var inputValueProduct = element.querySelector('.form-control, .form-select')
      var messageErrorProduct = element.querySelector('.error-message')
      if (messageErrorProduct) {
        messageErrorProduct.textContent = ''
      }
      if (
        inputValueProduct &&
        'value' in inputValueProduct &&
        inputValueProduct.value.trim() == ''
      ) {
        const label = element.querySelector('.form-label')
        if (messageErrorProduct) {
          messageErrorProduct.textContent = `Không được để trống ${label.textContent}`
          isValid = false
        }
      }
    })
    editedVariants.value.forEach(e => {
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
    const content = {
      maDanhMuc: editedProduct.value.maDanhMuc,
      tenSanPham: editedProduct.value.tenSanPham,
      moTa: editedProduct.value.moTa,
      isDelete: false,
      detailProductEditRequestDTOs: editedVariants.value.map((variant) => ({
        maCtsp: variant.maCtsp,
        kichThuoc: variant.kichThuoc,
        huongVi: variant.huongVi,
        soLuongTon: variant.soLuongTon,
        donGia: variant.donGia,
        imageProductRequestDTOs: [
          ...(variant.oldImages || []).map((img) => ({ tenHinhAnh: img.tenHinhAnh })),
          ...(variant.newImages || []).map((img) => ({ tenHinhAnh: img.tenHinhAnh })),
        ],
      })),
    }
    console.log(content)
    const response = await fetch(
      `https://localhost:7139/api/Products/${editedProduct.value.maSp}`,
      {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(content),
      }
    )
    const result = await response.json()
    if (!response.ok) {
      let errorMessage = `Lỗi khi cập nhật sản phẩm: ${response.status} ${response.statusText}`
      if (result.message) {
        errorMessage += ` - ${result.message}`
      }
      if (result.details) {
        errorMessage += ` (Chi tiết: ${JSON.stringify(result.details)})`
      }
      throw new Error(errorMessage)
    }
    Swal.fire({
      title: 'Đã cập nhật thông tin sản phẩm!',
      icon: 'success',
      draggable: true,
    })
    setTimeout(function () {
      window.location.reload()
    }, 2000)
  } catch (error) {
    console.error('Error:', error.message)
  }
}

const SaveChangeServer = async () => {
  Swal.fire({
    title: 'Bạn có muốn lưu các thay đổi này không ?',
    showCancelButton: true,
    confirmButtonText: 'Xác nhận',
    cancelButtonText: 'Hủy',
  }).then((result) => {
    if (result.isConfirmed) {
      UpdateProduct()
    }
  })
}

const blockNegativeNumbers = (event) => {
  if (event.key === '-') {
    event.preventDefault()
  }
}
const modules = [Navigation, Pagination]
</script>

  <template>
  <div
    class="modal fade"
    :id="`productEditModal_${Props.Product?.maSp || 'default'}`"
    tabindex="-1"
    data-bs-backdrop="static"
    data-bs-keyboard="false"
  >
    <button class="btn-close" data-bs-dismiss="modal"></button>
    <div class="modal-dialog modal-xl text-start">
      <div class="modal-content">
        <div class="modal-header bg-primary text-white">
          <h5 class="modal-title">Sửa thông tin sản phẩm</h5>
          <button @click="cancelEdit()" type="button" style="background: none; border: 0px">
            X
          </button>
        </div>

        <div class="modal-body p-4 data-editProduct">
          <form @submit.prevent>
            <div class="mb-3">
              <label class="form-label">Tên sản phẩm</label>
              <input type="text" class="form-control" v-model="editedProduct.tenSanPham" />
              <label style="color: red" class="error-message"></label>
            </div>
            <!-- Danh mục -->
            <div class="mb-3">
              <label for="maDanhMuc" class="form-label">Danh mục</label>
              <select class="form-select" id="maDanhMuc" v-model="editedProduct.maDanhMuc">
                <option value="" disabled>Chọn danh mục</option>
                <option
                  v-for="category in Props.categories"
                  :key="category.maDanhMuc"
                  :selected="category.maDanhMuc === editedProduct.maDanhMuc"
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
                rows="3"
                placeholder="Nhập mô tả sản phẩm"
                v-model="editedProduct.moTa"
              ></textarea>
              <label style="color: red" class="error-message"></label>
            </div>
            <div class="mb-3">
              <label class="form-label">Biến thể sản phẩm</label>
              <div v-for="(variant, vindex) in editedVariants" :key="vindex" class="card mb-3">
                <div class="card-body">
                  <div class="row">
                    <div class="col-md-3">
                      <label class="form-label">Ảnh</label>
                      <swiper
                        :slides-per-view="1"
                        :modules="modules"
                        navigation
                        pagination
                        class="variant-slider"
                      >
                        <swiper-slide
                          v-for="(image, index) in variant.oldImages"
                          :key="index"
                          class="position-relative"
                        >
                          <img
                            :src="`https://localhost:7139/HinhAnh/Food_Drink/${image.tenHinhAnh}`"
                            alt="Ảnh biến thể"
                            class="img-fluid rounded"
                            style="max-width: 100px; height: auto"
                          />
                          <button
                            type="button"
                            class="btn btn-danger btn-sm position-absolute top-0 end-0"
                            @click="deleteImage(variant, index, true)"
                          >
                            X
                          </button>
                        </swiper-slide>
                      </swiper>
                      <input
                        type="file"
                        multiple
                        @change="handleFileChange(variant, $event)"
                        class="form-control mt-2"
                      />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Kích thước</label>
                      <input type="text" class="form-control" v-model="variant.kichThuoc" />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Hương vị</label>
                      <input type="text" class="form-control" v-model="variant.huongVi" />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Số lượng tồn</label>
                      <input
                        type="number"
                        @keydown="blockNegativeNumbers"
                        class="form-control"
                        v-model="variant.soLuongTon"
                      />
                    </div>
                    <div class="col-md-2">
                      <label class="form-label">Đơn giá</label>
                      <input
                        type="number"
                        @keydown="blockNegativeNumbers"
                        class="form-control"
                        v-model="variant.donGia"
                      />
                    </div>
                  </div>
                  <!-- Nút xóa biến thể -->
                  <button
                    type="button"
                    class="btn btn-danger btn-sm mt-2"
                    @click="removeVariant(vindex)"
                  >
                    Xóa biến thể
                  </button>
                </div>
              </div>
            </div>
            <!-- Nút thêm biến thể -->
            <button type="button" class="btn btn-secondary" @click="addVariant">
              Thêm biến thể
            </button>
            <div class="text-end">
              <button type="button" @click="SaveChangeServer()" class="btn btn-primary">
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
.variant-slider .swiper-slide {
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  position: relative;
}
.variant-slider img {
  max-width: 100px;
  height: auto;
  object-fit: contain;
}
.btn-danger {
  font-size: 12px;
  padding: 2px 6px;
}
</style>
