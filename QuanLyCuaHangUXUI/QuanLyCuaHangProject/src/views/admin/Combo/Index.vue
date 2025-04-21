<script setup>
import { onMounted, ref } from 'vue'
import CreateCombo from '../Combo/Create.vue'
import EditCombo from '../Combo/Edit.vue'
import DetailCombo from '../Combo/Details.vue'
import Swal from 'sweetalert2'
import { GetApiUrl } from '@constants/api'
import { ReadToken, ValidateToken } from '../../../Authentication_Authorization/auth.js'
import Cookies from 'js-cookie'
let getApiUrl = GetApiUrl()
const listCombo = ref([])
const ListProduct = ref([])
const TotalPages = ref(0)
const CurrentPage = ref(1)
const valueSearch = ref('')
let accesstoken = Cookies.get('accessToken')
let refreshtoken = Cookies.get('refreshToken')
const role = ref('')
async function fetchCombo() {
  try {
    const validatetoken = await ValidateToken(accesstoken, refreshtoken)
    if(validatetoken){
      accesstoken = Cookies.get('accessToken')
      const readtoken = ReadToken(accesstoken)
      if(readtoken){
        role.value = readtoken.Role;
      }
    }
    const response = await fetch(getApiUrl+`/api/Combos?page=${CurrentPage.value}&search=${valueSearch.value}`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })
    if (!response.ok) {
      throw new Error('Lỗi khi lấy dữ liệu: ' + response.status)
    }
    const data = await response.json()
    listCombo.value = data.data
    TotalPages.value = data.totalPages
    CurrentPage.value = data.currentPage
  } catch (error) {
    console.log('Có lỗi xảy ra: ', error)
  }
}

async function fetchProducts() {
  try {
    const response = await fetch(getApiUrl+`/api/Products/GetAll`, {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
      },
    })

    if (!response.ok) {
      throw new Error('Lỗi khi lấy dữ liệu' + response.status)
    }

    const data = await response.json()
    ListProduct.value = data
  } catch (error) {
    console.error('Có lỗi xảy ra:', error)
  }
}
const ChangePage = (page) => {
console.log("chuyển trang" + page)
  CurrentPage.value = page
  fetchCombo()
}
const ReturnCombo = () => {
  fetchCombo()
}
async function removeCombo(id) {
  try {
    Swal.fire({
      title: 'Bạn có muốn xóa combo này không ?',
      showDenyButton: true,
      showCancelButton: false,
      confirmButtonText: 'Có',
      denyButtonText: `Không`,
    }).then(async (result) => {
      if (result.isConfirmed) {
        const response = await fetch(getApiUrl+`/api/Combos/${id}/Cancel`, {
          method: 'PUT',
        })

        if (!response.ok) {
          throw new Error('Error:', response.status)
        }
        Swal.fire({
          title: 'Đã xóa thông tin combo!',
          icon: 'success',
          draggable: true,
        })
        setTimeout(function () {
          window.location.reload()
        }, 2000)
      } else if (result.isDenied) {
        Swal.clickCancel()
      }
    })
  } catch (error) {
    console.log(error)
  }
}
onMounted(() => {
  fetchCombo()
  fetchProducts()
})
</script>
<template>
  <div class="container mt-4">
    <h2 class="mb-4 text-center">Quản lý combo</h2>

    <!-- Thanh tìm kiếm, lọc, sắp xếp (tĩnh, không có chức năng) -->
    <div class="row g-3 mb-3 align-items-center">
      <div class="col-md-3">
        <input
          type="text"
          class="form-control shadow-sm border-primary bg-white"
          placeholder="🔍 Nhập tên combo..."
          v-model="valueSearch"
          @input="ReturnCombo()"
        />
      </div>
     
    </div>

   
    <!-- Nút thêm combo (tĩnh, không có chức năng) -->
    <div class="mb-4" v-if="role.toLowerCase() == 'admin'">
      <button
        type="button"
        class="btn btn-primary"
        data-bs-toggle="modal"
        data-bs-target="#exampleModal"
      >
        ➕ Thêm combo
      </button>
    </div>
    <CreateCombo :ListProduct="ListProduct" />
    <!-- Bảng dữ liệu (dữ liệu tĩnh) -->
    <div class="table-responsive">
      <table class="table table-hover table-bordered">
        <thead class="table-dark text-center">
          <tr>
            <th>Mã combo</th>
            <th>Tên combo</th>
            <th>Hình</th>
            <th>Số lượng</th>
            <th>Mức giảm</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <!-- Dữ liệu tĩnh cho danh sách combo -->
          <tr v-for="combo in listCombo" :key="combo.maCombo">
            <td class="text-center">{{ combo.maCombo }}</td>
            <td class="text-center">{{ combo.tenCombo }}</td>
            <td class="text-center">
              <img
                :src="getApiUrl+'/HinhAnh/Food_Drink/' + combo.hinh"
                alt="Combo Image"
                width="50"
                height="50"
                style="object-fit: cover; border-radius: 5px"
              />
            </td>
            <td class="text-center">{{ combo.soLuong }}</td>
            <td class="text-center">
              {{
                combo.soTienGiam == null || combo.soTienGiam == 0
                  ? '-' + combo.phanTramGiam + '%'
                  : '-' + combo.soTienGiam + ' VNĐ'
              }}
            </td>
            <td class="text-center">
              <button
                type="button"
                data-bs-toggle="modal"
                :data-bs-target="`#comboEditModal_${combo.maCombo}`"
                class="btn btn-warning btn-sm"
                v-if="role.toLowerCase() == 'admin'"
              >
                ✏️ Sửa
              </button>
              <EditCombo :Combo="combo" :ListProduct="ListProduct" />
              <button
                type="button"
                data-bs-toggle="modal"
                :data-bs-target="`#comboDetailModal_${combo.maCombo}`"
                class="btn btn-info btn-sm"
              >
                ℹ️ Chi tiết
              </button>
              <DetailCombo :Combo="combo" :ListProduct="ListProduct" />
              <button v-if="role.toLowerCase() == 'admin'" @click="removeCombo(combo.maCombo)" class="btn btn-danger btn-sm">
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