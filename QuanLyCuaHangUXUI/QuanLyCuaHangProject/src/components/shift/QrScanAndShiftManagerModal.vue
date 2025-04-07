<template>
  <span @click="isModalOpen = true">Chấm công bằng QR</span>
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
    toggleMode() {
      this.isScanning = !this.isScanning
    },
  },
}
</script>
