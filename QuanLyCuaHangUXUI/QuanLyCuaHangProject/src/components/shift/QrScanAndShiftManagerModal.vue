<template>
  <!-- Dropdown Item -->
  <a class="dropdown-item" href="#">
    <i class="icon-user text-primary mr-2"></i>
    <span @click="checkAccessAndOpenModal">Chấm công bằng QR</span>
  </a>

  <div>
    <!-- Modal -->
    <div>
      <!-- Notification Messages -->
      <div v-show="notificationMessage" class="alert alert-danger mt-3" role="alert">
        {{ notificationMessage }}
        <a v-if="!isLoggedIn" href="/login" class="alert-link">Đăng nhập</a>
      </div>

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
                <!-- Notification Content -->
                <div v-if="isNotification" class="text-center">
                  <p>{{ notificationMessage }}</p>
                  <a v-if="!isLoggedIn" href="/login" class="btn btn-primary mt-3">Đăng nhập</a>
                </div>

                <!-- Main Modal Content -->
                <div v-else>
                  <QrScanner v-if="isScanning" />
                  <ShiftManager v-else />
                </div>
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
  </div>
</template>

<script>
import QrScanner from '@/components/shift/QrScanner.vue'
import ShiftManager from '@/components/shift/ShiftManager.vue'
import authService from '@/services/authService'

export default {
  name: 'QrScanAndShiftManagerModal',
  components: { QrScanner, ShiftManager },
  data() {
    return {
      isModalOpen: false,
      isScanning: true,
      isNotification: false, // Track if the modal is showing a notification
      modalTarget: typeof document !== 'undefined' ? 'body' : null,
      notificationMessage: '', // Message to display notifications
      isLoggedIn: false, // Track if the user is logged in
    }
  },
  methods: {
    checkAccessAndOpenModal() {
      // eslint-disable-next-line no-unused-vars
      const roleUser = authService.getRole()
      this.isLoggedIn = authService.isAccess()

      if (!this.isLoggedIn) {
        this.notificationMessage = 'Vui lòng đăng nhập để dùng chức năng này.'
        this.isNotification = true
        this.isModalOpen = true
        return
      }

      if (authService.isUserHaveRole(['Admin', 'Cửa hàng trưởng'])) {
        this.isScanning = false // Open Shift Manager
        this.isNotification = false
        this.isModalOpen = true
        this.notificationMessage = '' // Clear notification
      } else if (authService.isUserHaveRole(['Nhân viên'])) {
        this.isScanning = true // Open QR Scanner
        this.isNotification = false
        this.isModalOpen = true
        this.notificationMessage = '' // Clear notification
      } else {
        this.notificationMessage = 'Bạn không có quyền truy cập.'
        this.isNotification = true
        this.isModalOpen = true
      }
    },
    toggleMode() {
      this.isScanning = !this.isScanning
    },
  },
}
</script>

<style scoped>
.alert {
  max-width: 500px;
  margin: 0 auto;
}
</style>
