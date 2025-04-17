<script setup>
import { onMounted, ref } from 'vue'
import CreateProductModal from '../Product/Create.vue'
import DetailProductModal from '../Product/Details.vue'
import EditProductModal from '../Product/Edit.vue'
import Swal from 'sweetalert2'
import { GetApiUrl } from '@constants/api'
const ListProduct = ref([])
const TotalPages = ref(0)
const CurrentPage = ref(1)
const valueSearch = ref('')
const valueCategory = ref('')
const valueSort = ref('')
const valuePrices = ref('')
async function fetchProducts() {
  try {
    const response = await fetch(
      GetApiUrl()+`/api/Products?page=${CurrentPage.value}&search=${valueSearch.value}&filterCatories=${valueCategory.value}&sort=${valueSort.value}&filterPrices=${valuePrices.value}`,
      {
        method: 'GET',
        headers: {
          'Content-Type': 'application/json',
        },
      }
    )

    if (!response.ok) {
      throw new Error('Lỗi khi lấy dữ liệu' + response.status)
    }

    const data = await response.json()
    ListProduct.value = data.data
    TotalPages.value = data.totalPages
    CurrentPage.value = data.currentPage
  } catch (error) {
    console.error('Có lỗi xảy ra:', error)
  }
}

const ChangePage = (page) => {
  CurrentPage.value = page
  fetchProducts()
}

const ReturnProduct = () => {
  fetchProducts()
}

// Danh sách danh mục (tĩnh)
const categories = ref([])
async function fetchCategory() {
  const response = await fetch(GetApiUrl()+'/api/Categories', {
    method: 'GET',
    headers: {
      'Content-Type': 'application/json',
    },
  })
  if (!response.ok) {
    throw new Error('Error' + response.status)
  }
  const dataCategories = await response.json()
  categories.value = dataCategories
  console.log(categories.value)
}

onMounted(() => {
  fetchProducts()
  fetchCategory()
})

const RemoveProduct = async (id) => {
  try {
    Swal.fire({
      title: 'Bạn có muốn xóa sản phẩm này không ?',
      showDenyButton: true,
      showCancelButton: false,
      confirmButtonText: 'Có',
      denyButtonText: `Không`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        const response = await fetch(GetApiUrl()+`/api/Products/${id}/Cancel`, {
          method: 'PUT',
          headers: {
            'Content-Type': 'application/json',
          },
        })

        if (response.ok == false) {
          throw new Error('Failed to cancel product')
        }
        Swal.fire({
          title: 'Đã xóa thông tin sản phẩm!',
          icon: 'success',
          draggable: true,
        })
        setTimeout(function () {
          window.location.reload()
        }, 2000)
      }
      else if (result.isDenied) {
        Swal.clickCancel()
      }
    })
  } catch (error) {
    console.error('Error:', error.message)
  }
}
</script>

<template>
  <div class="container mt-4">
    <h2 class="mb-4 text-center">Quản lý sản phẩm</h2>

    <!-- Thanh tìm kiếm, lọc, sắp xếp -->
    <div class="row g-3 mb-3 align-items-center">
      <div class="col-md-3">
        <input
          @input="ReturnProduct()"
          type="text"
          class="form-control shadow-sm border-primary bg-white"
          placeholder="🔍 Nhập tên sản phẩm..."
          v-model="valueSearch"
        />
      </div>
      <div class="col-md-3">
        <select
          @change="ReturnProduct()"
          v-model="valueCategory"
          class="form-select shadow-sm bg-white"
        >
          <option value="">📂 Lọc theo danh mục</option>
          <option
            v-for="category in categories"
            :key="category.maDanhMuc"
            :value="category.maDanhMuc"
          >
            {{ category.tenDanhMuc }}
          </option>
        </select>
      </div>
      <div class="col-md-3">
        <select
          @change="ReturnProduct()"
          v-model="valueSort"
          class="form-select shadow-sm bg-white"
        >
          <option value="">🔄 Sắp xếp theo</option>
          <option value="asc">Giá tăng dần</option>
          <option value="des">Giá giảm dần</option>
        </select>
      </div>
    </div>

    <!-- Lọc theo khoảng giá (Đã chỉnh sửa nền trắng) -->
    <div class="row g-3 mb-3 align-items-center">
      <div class="col-md-3">
        <select
          v-model="valuePrices"
          @change="ReturnProduct()"
          class="form-select shadow-sm bg-white"
        >
          <option value="">💰 Lọc theo giá</option>
          <option value="0 VNĐ - 10.000 VNĐ">0 VNĐ - 10.000 VNĐ</option>
          <option value="10.000 VNĐ - 30.000 VNĐ">10.000 VNĐ - 30.000 VNĐ</option>
          <option value="30.000 VNĐ - 50.000 VNĐ">30.000 VNĐ - 50.000 VNĐ</option>
          <option value="50.000 VNĐ trở lên">50.000 VNĐ trở lên</option>
        </select>
      </div>
    </div>

    <!-- Nút thêm sản phẩm -->
    <div class="mb-4">
      <button
        type="button"
        class="btn btn-primary"
        data-bs-toggle="modal"
        data-bs-target="#exampleModal"
      >
        ➕ Thêm sản phẩm
      </button>
    </div>
    <CreateProductModal :categories="categories" />
    <!-- Bảng dữ liệu -->
    <div class="table-responsive">
      <table class="table table-hover table-bordered">
        <thead class="table-dark text-center">
          <tr>
            <th>Mã sản phẩm</th>
            <th>Tên sản phẩm</th>
            <th>Danh mục</th>
            <th>Khoảng giá</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="product in ListProduct" :key="product.maSp">
            <td class="text-center">{{ product.maSp }}</td>
            <td class="text-center">{{ product.tenSanPham }}</td>
            <td class="text-center">{{ product.tenDanhMuc }}</td>
            <td class="text-center">{{ product.khoangGia }}</td>
            <td class="text-center">
              <button
                type="button"
                data-bs-toggle="modal"
                :data-bs-target="`#productEditModal_${product.maSp}`"
                class="btn btn-warning btn-sm"
              >
                ✏️ Sửa
              </button>
              <EditProductModal :Product="product" :categories="categories" />
              <button
                type="button"
                data-bs-toggle="modal"
                :data-bs-target="`#productDetailModal_${product.maSp}`"
                class="btn btn-info btn-sm"
              >
                ℹ️ Chi tiết
              </button>
              <DetailProductModal :Product="product" />
              <button @click="RemoveProduct(product.maSp)" class="btn btn-danger btn-sm">
                🗑️ Xóa
              </button>
            </td>
          </tr>
        </tbody>
      </table>
    </div>
    <!-- Phân trang -->
    <div class="d-flex justify-content-center mt-4">
      <nav>
        <ul class="pagination">
          <li class="page-item disabled">
            <a class="page-link" href="#" tabindex="-1">&laquo;</a>
          </li>
          <li
            v-for="page in TotalPages"
            :key="page"
            :class="{ active: page === CurrentPage }"
            class="page-item"
          >
            <a class="page-link" @click="ChangePage(page)"> {{ page }} </a>
          </li>
          <li class="page-item">
            <a class="page-link" href="#">&raquo;</a>
          </li>
        </ul>
      </nav>
    </div>
  </div>
</template>

<style>
.sortable {
  cursor: pointer;
  user-select: none;
}
.sortable:hover {
  color: #f8d210;
}
</style>
