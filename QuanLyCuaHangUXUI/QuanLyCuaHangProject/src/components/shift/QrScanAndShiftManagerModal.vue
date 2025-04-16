<template>
  <!-- Dropdown Item -->
  <a class="dropdown-item" href="#">
    <i class="icon-bell text-primary mr-2"></i>
    <span @click="openModal">Chấm công bằng QR</span>
  </a>

  <div>
    <!-- Modal -->
    <teleport :to="modalTarget" v-if="isModalOpen">
      <div class="modal fade show d-block" tabindex="-1" @click.self="isModalOpen = false">
        <div class="modal-dialog modal-lg modal-dialog-centered">
          <div class="modal-content">
            <div class="modal-header">
              <h5 class="modal-title">
                <div v-if="isNotification" class="">
                  <strong>Thông báo!</strong>
                </div>
                <div v-else-if="isScanning" class="">
                  <strong>Chấm công bằng QR</strong>
                </div>
                <div v-else class="">
                  <strong>Quản lý ca làm việc</strong>
                  <a
                    v-if="!isNotification && !isScanning"
                    href="/admin/shift-manager"
                    class="icon-info ml-2 text-link text-decoration-none"
                  ></a>
                </div>
              </h5>
              <button type="button" class="btn btn-danger rounded" @click="isModalOpen = false">
                x
              </button>
            </div>
            <div class="modal-body d-flex flex-column align-items-center">
              <!-- Notification Content -->
              <div v-if="isNotification" class="text-center">
                <p>{{ notificationMessage }}</p>
                <a href="/login" class="btn btn-primary mt-3">Đăng nhập</a>
              </div>

              <!-- Main Modal Content -->
              <div v-else>
                <QrScanner v-if="isScanning" />
                <ShiftManager v-else />
              </div>
            </div>
            <!-- <div class="modal-footer" v-if="!isNotification">
              <button @click="toggleMode" class="btn btn-secondary">
                {{ isScanning ? 'Chuyển sang tạo QR' : 'Chuyển sang quét QR' }}
              </button>
              <button @click="isModalOpen = false" class="btn btn-danger">Đóng</button>
            </div> -->
          </div>
        </div>
      </div>
    </teleport>
  </div>
</template>

<script>
import QrScanner from '@/components/shift/QrScanner.vue'
import ShiftManager from '@/components/shift/ShiftManager.vue'
import authService from '@/services/authService' // Import auth service

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
    }
  },
  methods: {
    openModal() {
      const isAccess = authService.isAccess() // Check if the user is logged in
      const userRole = authService.getRole() // Get the user's role

      if (!isAccess) {
        // User is not logged in
        this.isNotification = true
        this.notificationMessage = 'Vui lòng đăng nhập để dùng chức năng này.'
        this.isModalOpen = true
        return
      }

      if (userRole === 'Admin' || userRole === 'Cửa hàng trưởng') {
        // User has permission to manage shifts
        this.isScanning = false
        this.isNotification = false
        this.isModalOpen = true
      } else if (userRole === 'Nhân viên') {
        // User has permission to scan QR codes
        this.isScanning = true
        this.isNotification = false
        this.isModalOpen = true
      } else {
        // User does not have permission
        this.isNotification = true
        this.notificationMessage = 'Bạn không có quyền truy cập chức năng này.'
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
