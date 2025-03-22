<template>
  <span @click="isModalOpen = true">Chấm công bằng QR</span>
  <teleport :to="modalTarget" v-if="isModalOpen">
    <div class="modal fade show d-block" tabindex="-1" @click.self="isModalOpen = false">
      <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content" style="width: 400px; min-height: 300px">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isScanning ? 'Chấm Công Bằng QR Code' : 'Tạo QR Check-in' }}
            </h5>
            <button type="button" class="btn-close" @click="isModalOpen = false"></button>
          </div>
          <div class="modal-body text-center">
            <p v-if="isScanning">Quét mã QR bằng camera hoặc tải ảnh QR lên.</p>
            <p v-else>Nhập ID Ca làm việc để tạo QR Code.</p>

            <div v-if="isScanning" class="content-wrapper">
              <input
                v-model="employeeId"
                type="text"
                placeholder="Nhập ID Người làm việc"
                class="form-control mb-3"
              />
              <qrcode-stream @decode="onScanSuccess"></qrcode-stream>
              <p class="mt-2">Hoặc tải ảnh QR lên:</p>
              <input type="file" @change="onFileUpload" accept="image/*" class="form-control" />
            </div>

            <div v-else class="content-wrapper">
              <input
                v-model="screwId"
                type="text"
                placeholder="Nhập ID Ca làm việc"
                class="form-control mb-3"
              />
              <button @click="generateQRCode" class="btn btn-success mb-3">Tạo QR</button>
              <div v-if="qrCodeData">
                <qrcode-vue :value="qrCodeData" :size="200" level="H"></qrcode-vue>
              </div>
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
</template>

<script>
import { QrcodeStream } from 'vue-qrcode-reader'
import QrcodeVue from 'qrcode.vue'
import { BrowserQRCodeReader } from '@zxing/browser'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'
import toastr from 'toastr'
export default {
  name: 'QrCheckInModal',
  components: { QrcodeStream, QrcodeVue },
  data() {
    return {
      isModalOpen: false,
      isScanning: true,
      screwId: '',
      employeeId: '',
      qrCodeData: '',
      modalTarget: typeof document !== 'undefined' ? 'body' : null,
    }
  },
  methods: {
    toggleMode() {
      this.isScanning = !this.isScanning
    },
    async generateQRCode() {
      if (!this.screwId) {
        toastr.info('Vui lòng nhập ID Ca làm việc')
        return
      }

      const today = new Date().toISOString().split('T')[0]

      try {
        const response = await axiosConfig.getFromApi(
          `/LichLamViec/GenerateQRCode?maCaKip=${this.screwId}&ngayLam=${today}`,
          { ...ConfigsRequest.getSkipAuthConfig() },
        )
        console.log(response.data)
        this.qrCodeData = response.data
      } catch (error) {
        toastr.error('Lỗi khi tạo mã QR')
        console.error(error)
      }
    },
    async onScanSuccess(qrCodeData) {
      try {
        const response = await axiosConfig.postToApi(
          `/LichLamViec/ChamCong?maNv=${this.employeeId}&qrCodeData=${encodeURIComponent(qrCodeData)}`,
          ConfigsRequest.getSkipAuthConfig(),
        )
        toastr.success(response.message)
      } catch (error) {
        toastr.error('Lỗi khi chấm công: ' + error.message)
      }
    },
    async onFileUpload(event) {
      const file = event.target.files[0]
      if (!file) return

      const reader = new FileReader()
      reader.onload = async (e) => {
        try {
          const qrCodeReader = new BrowserQRCodeReader()
          const result = await qrCodeReader.decodeFromImageUrl(e.target.result)
          this.onScanSuccess(result.getText())
        } catch (error) {
          toastr.info('Không tìm thấy mã QR trong ảnh')
        }
      }
      reader.readAsDataURL(file)
    },
  },
}
</script>

<style scoped>
.modal-content {
  width: 400px;
  min-height: 300px;
}
.content-wrapper {
  min-height: 200px;
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
}
</style>
