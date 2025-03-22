<template>
  <span @click="isModalOpen = true">Chấm công bằng QR</span>
  <teleport :to="modalTarget" v-if="isModalOpen">
    <div class="modal fade show d-block" tabindex="-1" @click.self="isModalOpen = false">
      <div class="modal-dialog modal-lg modal-dialog-centered">
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title">
              {{ isScanning ? 'Chấm Công Bằng QR Code' : 'Tạo QR Check-in' }}
            </h5>
            <button type="button" class="btn btn-danger rounded" @click="isModalOpen = false">
              x
            </button>
          </div>
          <div class="modal-body text-center d-flex flex-column align-items-center">
            <p v-if="isScanning">
              Quét mã QR bằng camera.
              <button
                type="button"
                class="btn btn-info btn-sm ml-2"
                data-bs-toggle="tooltip"
                :title="currentShiftInfo"
                id="tooltip-current-shift"
              >
                ⓘ
              </button>
            </p>
            <p v-else>Quản lý Ca làm việc.</p>

            <div class="content-wrapper w-100 d-flex flex-column align-items-center">
              <div v-if="isScanning" class="w-100 d-flex flex-column align-items-center">
                <input
                  v-model="employeeId"
                  type="text"
                  placeholder="Nhập ID Người làm việc"
                  class="form-control mb-2 w-75"
                />
                <qrcode-stream
                  @decode="onScanSuccess"
                  style="width: 200px; height: 200px"
                ></qrcode-stream>
                <p class="mt-2">Hoặc tải ảnh QR lên:</p>
                <div class="row mb-2 align-items-center">
                  <input
                    type="file"
                    @change="onFileUpload"
                    accept="image/*"
                    class="form-control col-9"
                  />
                  <button
                    class="btn btn-primary col"
                    @click="confirmImageUpload"
                    :disabled="!uploadedFile"
                  >
                    Xác nhận
                  </button>
                </div>
              </div>
              <div v-else class="container-fluid">
                <div class="row">
                  <div class="col-4">
                    <form @submit.prevent="saveShift">
                      <div class="mb-2">
                        <label>Số Người Tối Đa:</label>
                        <input
                          v-model="shift.soNguoiToiDa"
                          type="number"
                          class="form-control"
                          required
                        />
                      </div>
                      <div class="mb-2">
                        <label>Giờ Bắt Đầu:</label>
                        <input
                          v-model="shift.gioBatDau"
                          type="time"
                          class="form-control"
                          required
                        />
                      </div>
                      <div class="mb-2">
                        <label>Giờ Kết Thúc:</label>
                        <input
                          v-model="shift.gioKetThuc"
                          type="time"
                          class="form-control"
                          required
                        />
                      </div>
                      <button type="submit" class="btn btn-primary">
                        {{ shift.maCaKip ? 'Cập nhật' : 'Thêm mới' }}
                      </button>
                    </form>
                  </div>
                  <div class="col-8">
                    <table class="table" id="dt-listShifts"></table>
                  </div>
                </div>
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
import QRCode from 'qrcode'

import { BrowserQRCodeReader } from '@zxing/browser'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'
import toastr from 'toastr'
import * as configsDt from '@/utils/configsDatatable.js'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'

export default {
  name: 'QrCheckInModal',
  components: { QrcodeStream },
  data() {
    return {
      isModalOpen: false,
      isScanning: true,
      employeeId: '',
      qrCodeData: '',
      uploadedFile: null,
      modalTarget: typeof document !== 'undefined' ? 'body' : null,
      listShifts: [],
      datatable: null,
      shift: { maCaKip: null, soNguoiToiDa: 0, gioBatDau: '', gioKetThuc: '' },
      currentShiftInfo: '',
    }
  },
  methods: {
    toggleMode() {
      this.isScanning = !this.isScanning
      if (!this.isScanning) {
        this.loadShifts()
      }
    },
    loadShifts() {
      axiosConfig
        .getFromApi('/CaKip/GetAll')
        .then((response) => {
          if (response.success) {
            this.listShifts = response.data
            this.$nextTick(() => this.initDataTable())
          } else {
            console.error('API Error:', response.message)
          }
        })
        .catch((error) => {
          console.error('Error loading data:', error)
        })
    },
    initDataTable() {
      if (this.datatable) {
        this.datatable.destroy()
        this.datatable = null
      }

      this.datatable = $('#dt-listShifts').DataTable({
        data: this.listShifts,
        columns: [
          {
            data: 'maCaKip',
            width: '5%',
            title: 'ID',
            className: 'text-center',
          },
          {
            data: 'soNguoiToiDa',
            width: '5%',
            title: 'Tối đa\n(người)',
            className: 'text-center',
          },
          {
            data: 'soNguoiHienTai',
            width: '5%',
            title: 'Đang có\n(người)',
            className: 'text-center',
          },
          {
            data: 'gioBatDau',
            width: '5%',
            title: 'Bắt đầu',
            className: 'text-center',
          },
          {
            data: 'gioKetThuc',
            width: '5%',
            title: 'Kết thúc',
            className: 'text-center',
          },
          {
            data: 'maCaKip',
            width: '20%',
            title: 'Thao tác',
            className: 'text-center',
            render: function (data, type, row) {
              const isDelete = row.isDelete
              return `
                  <span class='btn btn-warning btn-sm edit-shift' data-id='${data}' title="Sửa">📝</span>
                  <span class='btn btn-warning btn-sm change-status' data-id='${row.maCaKip}' title="${isDelete ? 'Vô hiệu' : 'Hoạt động'}">
                    ${isDelete ? '✖️' : '✔️'}
                  </span>
                  <span class='btn btn-success btn-sm generate-qr' data-id='${data}' title="Tải QR">🔻</span>
                `
            },
          },
        ],
        language: configsDt.defaultLanguageDatatable,
      })
      $('#dt-listShifts tbody').on('click', '.change-status', (event) => {
        const id = $(event.currentTarget).data('id')
        this.changeStatusShift(id)
      })
      $('#dt-listShifts tbody').on('click', '.edit-shift', (event) => {
        const id = $(event.currentTarget).data('id')
        const shift = this.listShifts.find((s) => s.maCaKip == id)
        if (shift) this.shift = { ...shift }
      })

      $('#dt-listShifts tbody').on('click', '.generate-qr', async (event) => {
        const id = $(event.currentTarget).data('id')
        await this.generateQRCode(id)
      })
    },
    saveShift() {
      // Chuyển đổi `gioBatDau` và `gioKetThuc` sang dạng HH:mm:ss
      const formattedShift = {
        ...this.shift,
        gioBatDau: this.shift.gioBatDau ? `${this.shift.gioBatDau}:00` : '', // Thêm `:00`
        gioKetThuc: this.shift.gioKetThuc ? `${this.shift.gioKetThuc}:00` : '', // Thêm `:00`
      }

      // Gửi request với dữ liệu đã chuyển đổi
      axiosConfig
        .postToApi('/CaKip/UpsertCrew', formattedShift)
        .then((response) => {
          if (response.success) {
            toastr.success('Lưu thành công')
            this.loadShifts()
          } else {
            toastr.error('Lỗi khi lưu: ' + response.message)
          }
        })
        .catch((error) => {
          console.error(error)
          toastr.error('Lỗi không xác định khi lưu thông tin ca làm việc.')
        })
    },
    async changeStatusShift(maCaKip) {
      try {
        const response = await axiosConfig.patchToApi(`/CaKip/ChangeStatusShift?id=${maCaKip}`)
        if (response.success) {
          toastr.success(response.message)
          this.loadShifts()
        } else {
          toastr.error(response.message)
        }
      } catch (error) {
        toastr.error('Lỗi khi thay đổi trạng thái')
      }
    },
    async generateQRCode(maCaKip) {
      try {
        // Tìm dữ liệu qrCodeData trong danh sách các ca làm việc (listShifts)
        const shift = this.listShifts.find((s) => s.maCaKip === maCaKip)
        if (!shift || !shift.qrCodeData) {
          toastr.error('Không tìm thấy dữ liệu QR cho ca làm việc này.')
          return
        }

        // Gán dữ liệu QR từ ca làm việc vào qrCodeData
        const qrCodeData = shift.qrCodeData

        // Sử dụng thư viện qrcode để tạo ảnh QR
        const canvas = document.createElement('canvas')
        await QRCode.toCanvas(canvas, qrCodeData, { width: 200 }) // Kích thước tùy chỉnh

        // Tạo tệp hình ảnh từ canvas và tải xuống
        const link = document.createElement('a')
        link.href = canvas.toDataURL('image/png') // Dạng hình ảnh PNG
        link.download = `QRCode_Ca_${maCaKip}.png`
        link.click()
      } catch (error) {
        toastr.error('Đã xảy ra lỗi khi tải QR Code: ' + error.message)
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
    onFileUpload(event) {
      this.uploadedFile = event.target.files[0]
    },
    async confirmImageUpload() {
      if (!this.uploadedFile) return
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
      reader.readAsDataURL(this.uploadedFile)
    },
    refreshTooltip() {
      const now = new Date() // Lấy thời gian hiện tại
      const currentTime = now.getHours() * 60 + now.getMinutes() // Thời gian hiện tại tính bằng phút

      // Tìm ca làm việc hiện tại từ danh sách
      const currentShift = this.listShifts.find((shift) => {
        const start = this.timeToMinutes(shift.gioBatDau)
        const end = this.timeToMinutes(shift.gioKetThuc)
        return currentTime >= start && currentTime <= end
      })

      // Gán thông tin ca hiện tại hoặc thông báo không tìm thấy
      this.currentShiftInfo = currentShift
        ? `Ca hiện tại: ${currentShift.maCaKip}, Bắt đầu: ${currentShift.gioBatDau}, Kết thúc: ${currentShift.gioKetThuc}, Người hiện tại: ${currentShift.soNguoiHienTai}`
        : 'Không có ca làm việc nào trong thời điểm này.'

      // Refresh tooltip
      const tooltipTrigger = document.getElementById('tooltip-current-shift')
      if (tooltipTrigger) {
        bootstrap.Tooltip.getOrCreateInstance(tooltipTrigger).setContent({
          '.tooltip-inner': this.currentShiftInfo,
        })
      }
    },

    // Hàm chuyển đổi thời gian "HH:mm" sang phút
    timeToMinutes(time) {
      const [hours, minutes] = time.split(':').map(Number)
      return hours * 60 + minutes
    },
  },
  mounted() {
    this.loadShifts()

    // Khởi tạo tooltip Bootstrap
    const tooltipTrigger = document.getElementById('tooltip-current-shift')
    if (tooltipTrigger) {
      new bootstrap.Tooltip(tooltipTrigger)
    }

    // Refresh tooltip khi bảng ca làm việc cập nhật
    this.$watch('listShifts', () => {
      this.refreshTooltip()
    })
  },
}
</script>

<style scoped>
.content-wrapper {
  display: flex;
  flex-direction: column;
  justify-content: center;
  align-items: center;
  height: 100%;
}
#dt-listShifts tbody {
  height: 450px;
  overflow-y: auto;
}
</style>
