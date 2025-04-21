<template>
  <div class="w-100 d-flex flex-row align-items-center">
    <div
      v-if="!showShiftTable"
      class="col d-flex justify-content-center flex-column align-items-center"
    >
      <qrcode-stream
        @decode="onScanSuccess"
        @init="onInit"
        @error="onError"
        :paused="false"
        style="width: 200px; height: 200px"
      ></qrcode-stream>
      <p class="mt-2">Hoặc tải ảnh QR lên:</p>
      <div class="d-flex w-75 mb-2 align-items-center">
        <input
          type="file"
          @change="onFileUpload"
          accept="image/*"
          class="form-control col-9"
          :disabled="isDisabled"
        />
        <button
          class="btn btn-primary col-3"
          @click="confirmImageUpload"
          :disabled="!uploadedFile || isDisabled"
        >
          Xác nhận
        </button>
      </div>
    </div>
    <div v-if="showShiftTable" class="col border-left">
      <div v-if="loading" class="overlay">
        <div class="spinner-border text-primary" role="status">
          <span class="sr-only">Loading...</span>
        </div>
      </div>

      <div v-else class="mt-3">
        <h5>Nhân Viên cùng đăng kí Ca {{ employeeList[0].maCaKip }}</h5>
        <div style="overflow-x: auto">
          <table class="table table-bordered mt-2" id="dt-employeeList">
            <thead>
              <tr>
                <th>Mã NV</th>
                <th>Tên Nhân Viên</th>
                <th>Giờ Vào</th>
                <th>Giờ Ra</th>
                <th>Trạng thái</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="employee in employeeList" :key="employee.maNv">
                <td>{{ employee.maNv }}</td>
                <td>{{ employee.tenNhanVien }}</td>
                <td>{{ formatTime(employee.gioVao) }}</td>
                <td>{{ formatTime(employee.gioRa) }}</td>
                <td>{{ employee.trangThai }}</td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { QrcodeStream } from 'vue-qrcode-reader'
import jsQR from 'jsqr'
import toastr from 'toastr'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'
import ResponseAPI from '@/models/ResponseAPI'
import { formatTime } from '@/constants/formatDatetime'
import Cookies from 'js-cookie'

export default {
  name: 'QrScanner',
  components: { QrcodeStream },
  data() {
    return {
      uploadedFile: null,
      showShiftTable: false,
      employeeList: [],
      isDisabled: false,
      loading: false, // Biến kiểm soát trạng thái
    }
  },
  async created() {
    await this.loadEmployeeSchedule() // Gọi API khi component được khởi tạo
  },
  methods: {
    formatTime,
    async loadEmployeeSchedule() {
      this.loading = true

      try {
        const response = await axiosConfig.getFromApi(
          `/Schedule/GetScheduleActiveOfUser/`,
          ConfigsRequest.takeAuth(),
        )

        // console.log(response)
        if (response.success && response.data.length > 0) {
          this.employeeList = response.data // Lưu danh sách nhân viên
          this.isDisabled = true // Disable các chức năng quét QR và tải ảnh
          this.showShiftTable = true // Hiển thị bảng nhân viên
        }
      } catch (error) {
        toastr.error('Lỗi khi tải danh sách nhân viên: ' + error.message)
      } finally {
        this.loading = false
      }
    },
    async onScanSuccess(qrCodeData) {
      try {
        // console.log(Cookies.get("accessToken"))
        const response = await axiosConfig.postToApi(
          `/Schedule/TimeKeeping?&qrCodeData=${encodeURIComponent(qrCodeData)}`,
          null,
          ConfigsRequest.takeAuth(),
        )

        if (ResponseAPI.handleNotification(response)) {
          return
        }

        // Sau khi quét QR thành công, lọc danh sách nhân viên trong ca
        const maCaKip = Number(qrCodeData.split('-')[0]) // ID ca làm việc từ mã QR
        this.filterEmployeesByShift(maCaKip)
      } catch (error) {
        toastr.error('Lỗi khi chấm công: ' + error.message)
      }
    },
    async onFileUpload(event) {
      const file = event.target.files[0]
      if (!file) return

      const reader = new FileReader()
      reader.onload = async (e) => {
        const img = new Image()
        img.src = e.target.result

        img.onload = async () => {
          try {
            const qrData = await this.decodeQrFromImage(img)
            this.onScanSuccess(qrData)
          } catch (error) {
            toastr.info(error.message)
          }
        }
      }
      reader.readAsDataURL(file)
    },

    async confirmImageUpload() {
      if (!this.uploadedFile) return

      const reader = new FileReader()
      reader.onload = async (e) => {
        const img = new Image()
        img.src = e.target.result

        img.onload = async () => {
          try {
            const qrData = await this.decodeQrFromImage(img)
            this.onScanSuccess(qrData)
          } catch (error) {
            toastr.error(error.message)
          }
        }
      }
      reader.readAsDataURL(this.uploadedFile)
    },
    decodeQrFromImage(image) {
      return new Promise((resolve, reject) => {
        const canvas = document.createElement('canvas')
        const ctx = canvas.getContext('2d')

        canvas.width = image.width
        canvas.height = image.height
        ctx.drawImage(image, 0, 0, image.width, image.height)

        const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height)
        const code = jsQR(imageData.data, canvas.width, canvas.height)

        if (code) resolve(code.data)
        else reject(new Error('Không tìm thấy mã QR trong ảnh.'))
      })
    },
    async filterEmployeesByShift(maCaKip) {
      // Tìm ca làm việc dựa trên `maCaKip`
      const currentShift = this.listShifts.find((shift) => shift.maCaKip === maCaKip)

      if (currentShift) {
        // Lọc `schedules` chỉ lấy nhân viên có `trangThai === "Đi làm"`
        this.employeeList = (currentShift.schedules || []).filter(
          (employee) => employee.trangThai === 'Đi làm',
        )

        if (this.employeeList.length === 0) {
          toastr.info('Không có nhân viên nào đang đi làm trong ca này.')
        } else {
          this.showShiftTable = true // Hiển thị bảng nhân viên
        }
      } else {
        toastr.error('Không tìm thấy ca làm việc tương ứng.')
      }
    },
    onInit(promise) {
      promise
        .then(() => console.log('Camera đã sẵn sàng'))
        .catch(error => console.error('Lỗi khi khởi tạo camera:', error))
    },
    onError(error) {
      console.error('QR stream error:', error)
    },
  },
}
</script>

<style scoped>
/* Các style liên quan đến quét QR */
</style>
