<template>
  <div class="w-100 d-flex flex-row align-items-center">
    <div class="col d-flex justify-content-center flex-column align-items-center">
      <input
        v-model="employeeId"
        type="text"
        placeholder="Nhập ID Người làm việc"
        class="form-control mb-2 w-75"
        required
      />
      <qrcode-stream @decode="onScanSuccess" style="width: 200px; height: 200px"></qrcode-stream>
      <p class="mt-2">Hoặc tải ảnh QR lên:</p>
      <div class="d-flex w-75 mb-2 align-items-center">
        <input type="file" @change="onFileUpload" accept="image/*" class="form-control col-9" />
        <button class="btn btn-primary col-3" @click="confirmImageUpload" :disabled="!uploadedFile">
          Xác nhận
        </button>
      </div>
    </div>
    <div v-if="showShiftTable" class="col-4 border-left">
      <div class="mt-3">
        <h5>Nhân Viên Trong Ca</h5>
        <div style="overflow-x: auto">
          <table class="table table-bordered mt-2" id="dt-employeeList">
            <thead>
              <tr>
                <th>Mã NV</th>
                <th>Tên Nhân Viên</th>
                <th>Giờ Vào</th>
                <th>Giờ Ra</th>
                <th>Số Giờ Làm</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="employee in employeeList" :key="employee.maNv">
                <td>{{ employee.maNv }}</td>
                <td>{{ employee.tenNhanVien }}</td>
                <td>{{ employee.gioVao }}</td>
                <td>{{ employee.gioRa }}</td>
                <td>{{ employee.soGioLam }}</td>
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

export default {
  name: 'QrScanner',
  components: { QrcodeStream },
  data() {
    return {
      employeeId: '',
      uploadedFile: null,
      showShiftTable: false,
      employeeList: [],
    }
  },
  methods: {
    async onScanSuccess(qrCodeData) {
      if (!this.employeeId) {
        toastr.info('Vui lòng nhập ID Người làm việc trước khi quét mã QR.')
        return
      }

      try {
        const response = await axiosConfig.postToApi(
          `/LichLamViec/ChamCong?maNv=${this.employeeId}&qrCodeData=${encodeURIComponent(qrCodeData)}`,
          ConfigsRequest.getSkipAuthConfig(),
        )
        toastr.success(response.message)

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
          // Sử dụng Canvas để xử lý ảnh
          const canvas = document.createElement('canvas')
          const ctx = canvas.getContext('2d')

          canvas.width = img.width
          canvas.height = img.height
          ctx.drawImage(img, 0, 0, img.width, img.height)

          const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height)

          // Sử dụng jsQR để giải mã
          const code = jsQR(imageData.data, canvas.width, canvas.height)
          if (code) {
            this.onScanSuccess(code.data) // Xử lý dữ liệu khi giải mã thành công
          } else {
            toastr.info('Không tìm thấy mã QR trong ảnh.')
          }
        }
      }
      reader.readAsDataURL(file) // Đọc tệp dưới dạng Base64
    },
    async confirmImageUpload() {
      if (!this.uploadedFile) return

      const reader = new FileReader()
      reader.onload = async (e) => {
        const img = new Image()
        img.src = e.target.result

        img.onload = () => {
          // Xử lý ảnh bằng Canvas
          const canvas = document.createElement('canvas')
          const ctx = canvas.getContext('2d')

          canvas.width = img.width
          canvas.height = img.height
          ctx.drawImage(img, 0, 0, img.width, img.height)

          const imageData = ctx.getImageData(0, 0, canvas.width, canvas.height)

          // Sử dụng jsQR
          const code = jsQR(imageData.data, canvas.width, canvas.height)
          if (code) {
            this.onScanSuccess(code.data) // Xử lý dữ liệu QR giải mã thành công
          } else {
            toastr.info('Không thể giải mã mã QR trong ảnh.')
          }
        }
      }
      reader.readAsDataURL(this.uploadedFile) // Chuyển tệp sang Base64
    },
    filterEmployeesByShift(maCaKip) {
      // Tìm ca làm việc dựa trên `maCaKip`
      const currentShift = this.listShifts.find((shift) => shift.maCaKip === maCaKip)

      if (currentShift) {
        // Lọc `lichLamViecs` chỉ lấy nhân viên có `trangThai === "Đi làm"`
        this.employeeList = (currentShift.lichLamViecs || []).filter(
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
  },
}
</script>

<style scoped>
/* Các style liên quan đến quét QR */
</style>
