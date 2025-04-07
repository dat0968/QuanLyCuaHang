<template>
  <a class="dropdown-item" href="#"
    ><i class="icon-user text-primary mr-2"></i>
    <span @click="checkAccessAndOpenModal">Chấm công bằng QR</span>
  </a>
  <div>
    <teleport :to="modalTarget" v-if="isModalOpen">
      <div class="modal fade show d-block" tabindex="-1" @click.self="isModalOpen = false">
        <div class="modal-dialog modal-lg modal-dialog-centered">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">
                {{ isScanning ? 'Chấm Công Bằng QR Code' : 'Quản lý Ca làm việc.' }}
              </h5>
              <button type="button" class="btn btn-danger rounded" @click="isModalOpen = false">
                x
              </button>
            </div>
            <div class="modal-body d-flex flex-column align-items-center">
              <QrScanner v-if="isScanning" />
              <ShiftManager v-else />
            </div>
            <div class="modal-footer">
              <button @click="toggleMode" class="btn btn-secondary">
                {{ isScanning ? 'Chuyển sang tạo QR' : 'Chuyển sang quét QR' }}
              </button>
              <button @click="isModalOpen = false" class="btn btn-danger">Đóng</button>
            </div>
          </div>
        </div>
      </div>
    </teleport>
  </div>
</template>

<script>
import QrScanner from '@/components/shift/QrScanner.vue'
import ShiftManager from '@/components/shift/ShiftManager.vue'
import authService from '@/services/authService'
import toastr from 'toastr'

export default {
  name: 'QrScanAndShiftManagerModal',
  components: { QrScanner, ShiftManager },
  data() {
    return {
      isModalOpen: false,
      isScanning: true,
      modalTarget: typeof document !== 'undefined' ? 'body' : null,
    }
  },
  methods: {
    checkAccessAndOpenModal() {
      const roleUser = authService.getRole()
      console.log(`Vai trò đang truy cập ${roleUser}`)
      if (authService.isUserHaveRole(['Admin', 'Cửa hàng trưởng'])) {
        this.isScanning = false // Mở quản lý ca làm việc
        this.isModalOpen = true
      } else if (authService.isUserHaveRole(['Nhân viên'])) {
        this.isScanning = true // Mở quét QR
        this.isModalOpen = true
      } else {
        toastr.error('Bạn không có quyền truy cập.') // Hiển thị thông báo không có quyền
      }
    },
    toggleMode() {
      this.isScanning = !this.isScanning
    },
  },
}
</script>
