<script setup>
import { ref, onMounted, watch } from 'vue';
import Swal from 'sweetalert2';

const immutableStatuses = ["Đang sử dụng", "Đang sửa chữa"];
const tables = ref([]);
const searchQuery = ref('');
const statusFilter = ref('');
const isLoading = ref(false);
// const isAdding = ref(false);
const newTableStatus = ref('Trống');

const statusOptions = ["Trống", "Đang sử dụng", "Đã đặt trước", "Đang sửa chữa"];
const currentPage = ref(1); 
const pageSize = ref(10);
const totalItems = ref(0);

const fetchTables = async () => {
  isLoading.value = true;
  try {
    const url = statusFilter.value 
      ? `https://localhost:7139/api/Table/filter?tinhTrang=${encodeURIComponent(statusFilter.value)}`
      : `https://localhost:7139/api/Table`;

    const response = await fetch(url);
    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Không thể tải dữ liệu: ${response.status} - ${errorText}`);
    }
    const data = await response.json();

    // Xử lý dữ liệu từ API
    let tableData = Array.isArray(data) ? data : (data.items || []);
    tables.value = (tableData || []).map(table => ({
      ...table,
      tinhTrang: statusOptions.includes(table.tinhTrang) ? table.tinhTrang : "Trống"
    }));
    //
    totalItems.value = data.totalCount || tableData.length;
    //
    if (searchQuery.value) {
  const query = parseInt(searchQuery.value, 10);
  if (!isNaN(query)) { 
    tables.value = tables.value.filter(table => table.id === query);
  }
}

  } catch (error) {
    console.error('Lỗi khi lấy danh sách bàn:', error);
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error.message,
    });
  } finally {
    isLoading.value = false;
  }
};

const updateStatus = async (table, newStatus) => {
  const previousStatus = table.tinhTrang;
  try {
    const response = await fetch(
      `https://localhost:7139/api/Table/${table.id}`,
      {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ id: table.id, tinhTrang: newStatus }),
      }
    );

    if (!response.ok) {
      let errorMessage = "Cập nhật thất bại";
      try {
        const errorData = await response.json();
        errorMessage = errorData.message || errorMessage;
      } catch {
        errorMessage = response.statusText || "Không thể kết nối tới server";
      }
      throw new Error(errorMessage);
    }

    const result = await response.json();
    table.tinhTrang = result.tinhTrang;
    
    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: 'Cập nhật trạng thái bàn thành công',
      showConfirmButton: false,
      timer: 1500,
    });
  } catch (error) {
    console.error("Lỗi khi cập nhật trạng thái:", error);
    table.tinhTrang = previousStatus;
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error.message,
    });
  }
};

const addTable = async () => {
  try {
    const response = await fetch(
      `https://localhost:7139/api/Table`,
      {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ tinhTrang: newTableStatus.value }),
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Không thể thêm bàn: ${response.status} - ${errorText}`);
    }

    const result = await response.json();
    tables.value.push(result);
    // isAdding.value = false;
    newTableStatus.value = 'Trống';

    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: 'Thêm bàn thành công',
      showConfirmButton: false,
      timer: 1500,
    });
  } catch (error) {
    console.error("Lỗi khi thêm bàn:", error);
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error.message,
    });
  }
};

const deleteTable = async (tableId) => {
  const confirm = await Swal.fire({
    title: 'Bạn có chắc chắn?',
    text: "Bạn sẽ không thể khôi phục bàn này!",
    icon: 'warning',
    showCancelButton: true,
    confirmButtonText: 'Xóa',
    cancelButtonText: 'Hủy',
  });

  if (!confirm.isConfirmed) return;

  try {
    const response = await fetch(
      `https://localhost:7139/api/Table/${tableId}`,
      {
        method: "DELETE",
        headers: { "Content-Type": "application/json" },
      }
    );

    if (!response.ok) {
      const errorText = await response.text();
      throw new Error(`Không thể xóa bàn: ${response.status} - ${errorText}`);
    }

    tables.value = tables.value.filter(table => table.id !== tableId);
    
    Swal.fire({
      icon: 'success',
      title: 'Thành công!',
      text: 'Xóa bàn thành công',
      showConfirmButton: false,
      timer: 1500,
    });
  } catch (error) {
    console.error("Lỗi khi xóa bàn:", error);
    Swal.fire({
      icon: 'error',
      title: 'Lỗi!',
      text: error.message,
    });
  }
};

watch([searchQuery, statusFilter], fetchTables, { debounce: 300 });

onMounted(fetchTables);
</script>

<template>
  <div class="container mt-4">
    <h2 class="mb-4 text-center">Quản lý bàn</h2>

    <!-- Tìm kiếm và lọc -->
    <div class="row g-3 mb-3 justify-content-center">
      <div class="col-md-4">
        <input
          v-model="searchQuery"
          type="text"
          class="form-control shadow-sm border-primary bg-white"
          placeholder="🔍 Tìm theo id bàn..."
        />
      </div>
      <div class="col-md-4">
        <select v-model="statusFilter" class="form-select shadow-sm bg-white">
          <option value="">📋 Tất cả trạng thái</option>
          <option v-for="status in statusOptions" :key="status" :value="status">
            {{ status }}
          </option>
        </select>
      </div>
    </div>

    <!-- Modal -->
<div class="modal fade" id="addTableModal" tabindex="-1" aria-labelledby="addTableModalLabel" aria-hidden="true">
  <div class="modal-dialog modal-dialog-centered">
    <div class="modal-content">
      <div class="modal-header">
        <h5 class="modal-title" id="addTableModalLabel">Thêm bàn mới</h5>
        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
      </div>
      <div class="modal-body">
        <div class="row g-3">
          <div class="col-12">
            <select v-model="newTableStatus" class="form-select shadow-sm bg-white">
              <option v-for="status in statusOptions" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </div>
        </div>
      </div>
      <div class="modal-footer">
        <button class="btn btn-success" @click="addTable" data-bs-dismiss="modal">Lưu</button>
        <button class="btn btn-secondary" data-bs-dismiss="modal">Hủy</button>
      </div>
    </div>
  </div>
</div>

    <!-- Loading -->
    <div v-if="isLoading" class="text-center py-4">Đang tải dữ liệu...</div>

    <!-- Bảng dữ liệu -->
    <div class="table-responsive" v-else>
      <table class="table table-hover table-bordered">
        <thead class="table-dark text-center">
          <tr>
            <th>ID Bàn</th>
            <th>Trạng thái</th>
            <th>Thao tác</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="table in tables" :key="table.id">
            <td class="text-center">{{ table.id }}</td>
            <td class="text-center">
              <select
                class="form-select status-select"
                :value="table.tinhTrang || 'Trống'"
                @change="updateStatus(table, $event.target.value)"
                :disabled="immutableStatuses.includes(table.tinhTrang)"
              >
                <option v-for="status in statusOptions" :key="status" :value="status">
                  {{ status }}
                </option>
              </select>
            </td>
            <td class="text-center">
                <button class="btn btn-primary btn-sm" data-bs-toggle="modal" data-bs-target="#addTableModal">Thêm</button>
                <button class="btn btn-danger btn-sm" @click="deleteTable(table.id)">Xóa</button>
            </td>
          </tr>
          <tr v-if="!tables.length">
            <td colspan="3" class="text-center py-4">Không có dữ liệu</td>
          </tr>
        </tbody>
      </table>
      <!-- Phân trang -->
      <div class="d-flex justify-content-center mt-4">
        <nav>
          <ul class="pagination">
            <li class="page-item" :class="{ disabled: currentPage === 1 }">
              <a class="page-link" @click="prevPage" href="#">«</a>
            </li>
            <li
              v-for="page in Math.ceil(totalItems / pageSize)"
              :key="page"
              :class="{ active: page === currentPage }"
              class="page-item"
            >
              <a class="page-link" @click="changePage(page)">{{ page }}</a>
            </li>
            <li class="page-item" :class="{ disabled: currentPage >= Math.ceil(totalItems / pageSize) }">
              <a class="page-link" @click="nextPage" href="#">»</a>
            </li>
          </ul>
        </nav>
      </div>
    </div>
  </div>
</template>

<style scoped>
.container {
  max-width: 1200px;
  margin: 20px auto;
  padding: 20px;
}

h2 {
  text-align: center;
  color: #2c3e50;
  margin-bottom: 30px;
}

.form-control,
.form-select,
.status-select {
  padding: 10px;
  border: 1px solid #ddd;
  border-radius: 8px;
}

.form-control:focus,
.form-select:focus,
.status-select:focus {
  outline: none;
  border-color: #3498db;
  box-shadow: 0 0 5px rgba(52, 152, 219, 0.3);
}

.table-responsive {
  border-radius: 10px;
  overflow: hidden;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

.table th {
  background: #343a40;
  color: white;
}

.table tr:nth-child(even) {
  background: #f9f9f9;
}

.table tr:hover {
  background: #eef2f7;
}

.btn {
  padding: 8px 16px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  transition: background 0.3s;
}

.btn-primary {
  background: #3498db;
  color: white;
}

.btn-primary:hover {
  background: #2980b9;
}

.btn-success {
  background: #28a745;
  color: white;
}

.btn-success:hover {
  background: #218838;
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.btn-danger:hover {
  background: #c82333;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}

.btn-sm {
  padding: 5px 10px;
  font-size: 14px;
}
</style>