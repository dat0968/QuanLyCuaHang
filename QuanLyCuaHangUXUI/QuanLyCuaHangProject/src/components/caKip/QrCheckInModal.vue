<template>
  <span @click="isModalOpen = true">Chấm công bằng QR</span>
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
            <p v-if="isScanning">
              Quét mã QR bằng camera.

              <!-- Tooltip Hiển Thị Ca Hiện Tại -->
              <span
                type="button"
                data-bs-toggle="tooltip"
                :title="currentShiftInfo"
                id="tooltip-current-shift"
                @click="toggleShiftTable"
              >
                ⓘ
              </span>
            </p>
            <p v-else></p>

            <div class="content-wrapper w-100 d-flex flex-column align-items-center">
              <div v-if="isScanning" class="w-100 d-flex flex-row align-items-center">
                <div class="col d-flex justify-content-center flex-column align-items-center">
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
                  <div class="d-flex w-75 mb-2 align-items-center">
                    <input
                      type="file"
                      @change="onFileUpload"
                      accept="image/*"
                      class="form-control col-9"
                    />
                    <button
                      class="btn btn-primary col-3"
                      @click="confirmImageUpload"
                      :disabled="!uploadedFile"
                    >
                      Xác nhận
                    </button>
                  </div>
                </div>
                <div v-if="showShiftTable" class="col-4 border-left">
                  <!-- Bảng hiển thị danh sách nhân viên-->
                  <
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
                        <tbody></tbody>
                      </table>
                    </div>
                  </div>
                </div>
              </div>
              <div v-else class="container-fluid">
                <div class="row">
                  <!-- Phần hiển thị Form -->
                  <div class="col-4 border-right">
                    <!-- Tiêu đề cho form -->
                    <h5 class="mb-3">Thông Tin Ca Kíp</h5>
                    <hr />
                    <!-- Form -->
                    <form @submit.prevent="saveShift">
                      <div class="mb-2">
                        <label>Mã Ca:</label>
                        <input
                          v-model="shift.maCaKip"
                          type="number"
                          class="form-control"
                          disabled
                        />
                      </div>
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
                      <button type="submit" class="btn btn-primary w-100">
                        {{ shift.maCaKip ? 'Cập nhật' : 'Thêm mới' }}
                      </button>
                    </form>
                  </div>

                  <!-- Phần danh sách hiển thị -->
                  <div class="col-8 border-left">
                    <!-- Tiêu đề cho danh sách -->
                    <h5 class="mb-3">Danh Sách Ca Kíp</h5>
                    <hr />
                    <div style="height: 400px; overflow-y: auto">
                      <table class="table" id="dt-listShifts">
                        <!-- Nội dung bảng sẽ được thêm bằng JavaScript -->
                      </table>
                    </div>
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
// Import from external libraries
import { QrcodeStream } from 'vue-qrcode-reader'
import QRCode from 'qrcode'
import { BrowserQRCodeReader } from '@zxing/browser'
import toastr from 'toastr'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import jsQR from 'jsqr'
import Swal from 'sweetalert2'

// Import internal modules
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'
import * as configsDt from '@/utils/configsDatatable.js'

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
      showShiftTable: false,
      employeeList: [], // Dữ liệu từ API về nhân viên trong ca
    }
  },
  methods: {
    toggleMode() {
      this.isScanning = !this.isScanning
      if (!this.isScanning) {
        this.loadShifts()
      }
    },
    toggleShiftTable() {
      this.showShiftTable = !this.showShiftTable
      if (this.showShiftTable) {
        this.loadEmployeesInShift()
      }
    },
    async loadEmployeesInShift() {
      try {
        const currentShiftId = 1 // ID ca làm việc hiện tại, có thể thay đổi
        // Lọc danh sách ca làm việc tương ứng với `currentShiftId`
        if (currentShift) {
          // Lấy danh sách nhân viên từ trường `lichLamViecs` của ca hiện tại
          this.employeeList = currentShift.lichLamViecs || [] // Mặc định là mảng rỗng nếu không có dữ liệu

          if (this.employeeList.length === 0) {
            toastr.info('Không có nhân viên nào trong ca này.')
          }
        } else {
          toastr.error('Không tìm thấy ca làm việc tương ứng với ID.')
        }
      } catch (error) {
        console.error('Lỗi khi lọc nhân viên trong ca:', error)
        toastr.error('Đã xảy ra lỗi khi hiển thị danh sách nhân viên trong ca.')
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
          configsDt.defaultTdToShowDetail,
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
                      <div class="d-flex gap-1 justify-content-between">
                        <span
                          class="text-danger edit-shift"
                          data-id="${data}"
                          title="Sửa"
                          style="cursor: pointer;"
                        >
                          <i class="bi icon-pencil"></i>
                        </span>
                        <span
                          class="text-info change-status"
                          data-id="${row.maCaKip}"
                          title="${isDelete ? 'Vô hiệu hóa' : 'Kích hoạt'}"
                          style="cursor: pointer;"
                        >
                          <i class="${isDelete ? 'bi icon-close' : 'bi icon-check'}"></i>
                        </span>
                        <span
                          class="text-primary generate-qr"
                          data-id="${data}"
                          title="Tải QR"
                          style="cursor: pointer;"
                        >
                          <i class="bi icon-pin"></i>
                        </span>
                      </div>


                `
            },
          },
        ],
        order: [[1, 'asc']],
        language: {
          ...configsDt.defaultLanguageDatatable,
          info: 'Có _START_ đến _END_ ca trong số _TOTAL_ ca',
        },
      })

      //Gắn sự kiện hiển thị chi tiết
      configsDt.attachDetailsControl('#dt-listShifts', this.formatDetails)
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
    formatDetails(rowData) {
      // Tìm dữ liệu của ca làm việc dựa trên `maCaKip`
      const currentShift = this.listShifts.find((shift) => shift.maCaKip === rowData.maCaKip)
      const validStatuses = [
        'Chờ xác nhận',
        'Đi làm',
        'Kết thúc ca',
        'Nghỉ phép',
        'Trễ',
        'Nghỉ không phép',
        'Không được xác nhận',
      ]

      // Nếu tìm thấy ca tương ứng, hiển thị thông tin chi tiết
      if (currentShift && currentShift.lichLamViecs && currentShift.lichLamViecs.length > 0) {
        const detailsHtml = `
      <div class="container">
        <!-- Bộ lọc Trạng thái -->
        <div class="row mb-3">
          <div class="col-6">
            <label for="statusFilter">Lọc theo trạng thái:</label>
            <select id="statusFilter" class="form-control">
              <option value="">Tất cả</option>
              ${validStatuses.map((status) => `<option value="${status}">${status}</option>`).join('')}
            </select>
          </div>
          <div class="col-6 d-flex align-items-end">
            <button id="btnUpdateStatus" class="btn btn-primary w-100" disabled>
              Cập nhật trạng thái đã chọn
            </button>
          </div>
        </div>
        
        <!-- Table danh sách nhân viên -->
        <div class="row">
          <div class="col-12 mb-3">
            <input type="checkbox" id="selectAll" />
            <label for="selectAll">Chọn tất cả</label>
          </div>
        </div>
        <div class="row border-left border-right" style="max-height: 400px; overflow-y: auto">
          ${currentShift.lichLamViecs
            .map(
              (employee) => `
                <div class="col-6 mb-3 border rounded d-flex align-items-start">
                  <input type="checkbox" class="employee-checkbox" data-maNv="${employee.maNv}" />
                  <div class="ml-2">
                    <p><strong>${employee.tenNhanVien} <span class="text-secondary">[${employee.maNv}]</span></strong></p>
                    <p class="text-muted">Trạng thái: ${employee.trangThai}</p>
                  </div>
                </div>
              `,
            )
            .join('')}
        </div>
      </div>`

        const container = $(detailsHtml)

        // Gắn sự kiện sau khi HTML đã được tạo
        setTimeout(() => {
          // Lọc trạng thái
          container.find('#statusFilter').on('change', (e) => {
            const selectedStatus = e.target.value
            const employees = container.find('.col-6')

            employees.each(function () {
              const employee = $(this)
              if (!selectedStatus || employee.find('.text-muted').text().includes(selectedStatus)) {
                employee.show()
              } else {
                employee.hide()
              }
            })

            const enableUpdate = !!selectedStatus // Chỉ cho phép cập nhật nếu đang lọc trạng thái
            container.find('#btnUpdateStatus').prop('disabled', !enableUpdate)
          })

          // Checkbox: Chọn tất cả
          container.find('#selectAll').on('change', function () {
            const isChecked = $(this).is(':checked')
            container.find('.employee-checkbox').prop('checked', isChecked)
          })

          // Cập nhật trạng thái
          container.find('#btnUpdateStatus').on('click', () => {
            const selectedEmployees = container
              .find('.employee-checkbox:checked')
              .map((_, checkbox) => $(checkbox).data('manv'))
              .get()

            if (selectedEmployees.length === 0) {
              Swal.fire('Thông báo', 'Vui lòng chọn ít nhất một nhân viên.', 'warning')
              return
            }

            Swal.fire({
              title: 'Cập nhật trạng thái',
              input: 'select',
              inputOptions: validStatuses.reduce((options, status) => {
                options[status] = status
                return options
              }, {}),
              inputPlaceholder: 'Chọn trạng thái mới',
              showCancelButton: true,
              confirmButtonText: 'Cập nhật',
              cancelButtonText: 'Hủy',
            }).then(async (result) => {
              if (result.isConfirmed) {
                const newStatus = result.value

                try {
                  if (selectedEmployees.length === 1) {
                    // Cập nhật trạng thái cho một nhân viên
                    await axiosConfig.postToApi(
                      '/LichLamViec/SetStatusOne',
                      {
                        maNv: selectedEmployees[0],
                        trangThaiCapNhap: newStatus,
                        maCaKip: rowData.maCaKip,
                      },
                      ConfigsRequest.getSkipAuthConfig(),
                    )
                  } else {
                    // Cập nhật trạng thái cho nhiều nhân viên
                    await axiosConfig.postToApi(
                      '/LichLamViec/SetStatusList',
                      {
                        maNvs: selectedEmployees,
                        trangThaiCapNhap: newStatus,
                        maCaKip: rowData.maCaKip,
                      },
                      ConfigsRequest.getSkipAuthConfig(),
                    )
                  }

                  Swal.fire('Thành công', 'Trạng thái đã được cập nhật.', 'success')
                } catch (error) {
                  console.error('Cập nhật trạng thái lỗi:', error)
                  Swal.fire('Thất bại', 'Không thể cập nhật trạng thái.', 'error')
                }
              }
            })
          })
        }, 0)

        return container
      }

      // Nếu không có thông tin chi tiết
      return $(`<div>Không có thông tin chi tiết nhân viên trong ca này.</div>`)
    },
    saveShift() {
      // Chuyển đổi `gioBatDau` và `gioKetThuc` sang dạng HH:mm:ss
      const formattedShift = {
        ...this.shift,
        gioBatDau: this.shift.gioBatDau ? `${this.shift.gioBatDau}:00`.slice(0, 8) : '', // Đảm bảo đúng HH:mm:ss
        gioKetThuc: this.shift.gioKetThuc ? `${this.shift.gioKetThuc}:00`.slice(0, 8) : '', // Đảm bảo đúng HH:mm:ss
      }

      console.log('Data being sent: ', formattedShift)

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
          console.error('API Error: ', error)
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
        const shift = this.listShifts.find((s) => s.maCaKip === maCaKip)
        if (!shift || !shift.qrCodeData) {
          toastr.error('Không tìm thấy dữ liệu QR cho ca làm việc này.')
          return
        }

        const qrCodeData = shift.qrCodeData // Dữ liệu QR

        const canvas = document.createElement('canvas')
        await QRCode.toCanvas(canvas, qrCodeData, { width: 200 }) // Kích thước ảnh QR
        const link = document.createElement('a')
        link.href = canvas.toDataURL('image/png') // Chuyển sang ảnh dạng PNG
        link.download = `QRCode_Ca_${maCaKip}.png` // Đặt tên file
        link.click() // Tải file xuống
      } catch (error) {
        toastr.error('Đã xảy ra lỗi khi tải QR Code: ' + error.message)
      }
    },
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
    filterEmployeesByShift(maCaKip) {
      // Tìm ca làm việc dựa trên `maCaKip`
      const currentShift = this.listShifts.find((shift) => shift.maCaKip === maCaKip)

      console.log('Here!')
      console.log(maCaKip)
      console.log(currentShift)
      if (currentShift) {
        // Lọc `lichLamViecs` chỉ lấy nhân viên có `trangThai === "Đi làm"`
        this.employeeList = (currentShift.lichLamViecs || []).filter(
          (employee) => employee.trangThai === 'Đi làm',
        )

        if (this.employeeList.length === 0) {
          toastr.info('Không có nhân viên nào đang đi làm trong ca này.')
        }

        // Gọi DataTable để hiển thị danh sách nhân viên
        this.initEmployeeDataTable()
      } else {
        toastr.error('Không tìm thấy ca làm việc tương ứng.')
      }
    },
    initEmployeeDataTable() {
      const employeeTableId = '#dt-employeeList'

      // Nếu DataTable đã khởi tạo trước đó thì phá hủy nó
      if ($.fn.DataTable.isDataTable(employeeTableId)) {
        $(employeeTableId).DataTable().destroy()
      }

      // Khởi tạo lại DataTable với dữ liệu `employeeList`
      $(employeeTableId).DataTable({
        data: this.employeeList,
        columns: [
          {
            data: 'maNv',
            title: 'Mã NV',
            className: 'text-center',
          },
          {
            data: 'tenNhanVien',
            title: 'Tên Nhân Viên',
            className: 'text-center',
          },
          {
            data: 'gioVao',
            title: 'Giờ Vào',
            className: 'text-center',
          },
          {
            data: 'gioRa',
            title: 'Giờ Ra',
            className: 'text-center',
          },
          {
            data: 'soGioLam',
            title: 'Số Giờ Làm',
            className: 'text-center',
          },
        ],
        language: {
          lengthMenu: 'Hiển thị _MENU_ mục',
          info: 'Hiển thị mục _START_ đến _END_ trên tổng số _TOTAL_ mục',
          search: 'Tìm kiếm:',
          paginate: {
            first: 'Đầu',
            previous: 'Trước',
            next: 'Tiếp',
            last: 'Cuối',
          },
        },
      })
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
            // Nếu jsQR không giải mã được, sử dụng BrowserQRCodeReader
            const qrCodeReader = new BrowserQRCodeReader()
            try {
              const result = await qrCodeReader.decodeFromImageElement(img)
              this.onScanSuccess(result.getText())
            } catch (error) {
              toastr.info('Không tìm thấy mã QR trong ảnh.')
            }
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
</style>
