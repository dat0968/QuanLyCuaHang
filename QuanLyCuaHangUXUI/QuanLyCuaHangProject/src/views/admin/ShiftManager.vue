<template>
  <div class="shift-manager-page">
    <!-- Breadcrumb -->
    <div class="xp-breadcrumbbar">
      <div class="row">
        <div class="col-md-6 col-lg-6">
          <h4 class="xp-page-title"><strong>Quản lý Ca Làm Việc</strong></h4>
        </div>
      </div>
    </div>

    <!-- Content -->
    <div class="xp-contentbar">
      <div class="row">
        <!-- Form Thông Tin Ca Kíp -->
        <div class="col-4 border-right">
          <div class="row border-bottom">
            <h5 class="mb-3">Thông Tin Ca Kíp</h5>
            <hr />
            <form @submit.prevent="saveShift">
              <div class="mb-2">
                <label>Mã Ca:</label>
                <input v-model="shift.maCaKip" type="number" class="form-control" disabled />
              </div>
              <div class="mb-2">
                <label>Tên Ca:</label>
                <input v-model="shift.tenCa" type="text" class="form-control" required />
                <div v-if="errors && errors.tenCa" class="text-danger">{{ errors.tenCa }}</div>
              </div>
              <div class="mb-2">
                <label>Số Người Tối Đa:</label>
                <input v-model="shift.soNguoiToiDa" type="number" class="form-control" required />
                <div v-if="errors && errors.soNguoiToiDa" class="text-danger">
                  {{ errors.soNguoiToiDa }}
                </div>
              </div>
              <div class="row mb-2">
                <div class="col-6">
                  <label>Bắt Đầu:</label>
                  <input
                    v-model="shift.gioBatDau"
                    :max="shift.gioKetThuc"
                    type="time"
                    class="form-control"
                    required
                  />
                  <div v-if="errors && errors.gioBatDau" class="text-danger">
                    {{ errors.gioBatDau }}
                  </div>
                </div>
                <div class="col-6">
                  <label>Kết Thúc:</label>
                  <input
                    v-model="shift.gioKetThuc"
                    :min="shift.gioBatDau"
                    type="time"
                    class="form-control"
                    required
                  />
                  <div v-if="errors && errors.gioKetThuc" class="text-danger">
                    {{ errors.gioKetThuc }}
                  </div>
                </div>
              </div>
              <button type="submit" class="btn btn-primary w-100">
                {{ shift.maCaKip ? 'Cập nhật' : 'Thêm mới' }}
              </button>
            </form>
          </div>
          <div class="row border-top mt-5">
            <h5 class="my-3">Tạo Lịch Làm Việc</h5>
            <hr style="width: 95%" />
            <form @submit.prevent="createSchedule">
              <div class="mb-2">
                <label>Chọn Ca Kíp:</label>
                <select v-model="createScheduleObject.maCaKip" class="form-control" required>
                  <option
                    v-for="shift in listShifts.filter((x) => !x.isDelete)"
                    :key="shift.maCaKip"
                    :value="shift.maCaKip"
                    :disabled="shift.soNguoiHienTai >= shift.soNguoiToiDa"
                    :title="shift.soNguoiHienTai >= shift.soNguoiToiDa ? 'Ca đã đủ người' : ''"
                    placeholder="Chọn ca làm việc"
                  >
                    [ {{ shift.maCaKip }} ] {{ shift.tenCa ?? 'N/A' }}
                  </option>
                </select>
              </div>
              <div class="mb-2">
                <div class="row mr-2" style="max-height: 300px">
                  <label class="col-12">Chọn Nhân Viên:</label>

                  <div class="col-6">
                    <select
                      v-model="createScheduleObject.maNvs"
                      multiple
                      class="form-control"
                      placeholder="Chọn nhân viên đang hoạt động"
                      required
                      style="height: 200px"
                    >
                      <option v-for="user in userIds" :key="user.maNv" :value="user.maNv">
                        {{ user.hoTen }}
                      </option>
                    </select>
                  </div>
                  <div
                    class="col-6 border border-success rounded p-1 overflow-auto"
                    style="height: 200px"
                  >
                    <span
                      v-for="userId in createScheduleObject.maNvs"
                      :key="userId"
                      class="badge bg-primary me-1"
                    >
                      {{ userIds.find((x) => x.maNv === userId)?.hoTen || 'Không xác định' }} [
                      {{ userId }} ]
                    </span>
                  </div>
                </div>
              </div>
              <div class="mb-2">
                <label>Trạng Thái:</label>
                <select v-model="createScheduleObject.trangThaiCapNhap" class="form-control">
                  <option
                    v-for="(status, index) in TrangThaiLichLamViec.TatCaTrangThai"
                    :key="index"
                  >
                    {{ status }}
                  </option>
                </select>
              </div>
              <div class="mb-2">
                <label>Ghi Chú:</label>
                <input
                  v-model="createScheduleObject.ghiChu"
                  type="text"
                  class="form-control"
                  placeholder="Viết ghi chú..."
                />
              </div>
              <button type="submit" class="btn btn-primary w-100">Tạo lịch làm việc</button>
            </form>
          </div>
        </div>

        <!-- Danh Sách Ca Kíp -->
        <div class="col-8 border-left">
          <h5 class="mb-3">Danh Sách Ca Kíp</h5>
          <hr />
          <div style="overflow-y: auto">
            <table class="table" id="dt-listShiftsPage">
              <!-- Nội dung bảng sẽ được thêm bằng JavaScript -->
            </table>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
// import package
import toastr from 'toastr'
import QRCode from 'qrcode' // Đảm bảo đã import QRCode
import $ from 'jquery'
import Swal from 'sweetalert2'
// import thủ công
import * as configsDt from '@/utils/configsDatatable.js'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'
import ResponseAPI from '@/models/ResponseAPI'
import * as formatDatetime from '@/constants/formatDatetime'
import TrangThaiLichLamViec from '@/constants/trangThaiLichLamViec'
// import validate from '@/utils/validateYup'
import * as validate from 'yup'

// #region [Method js]
// Định nghĩa schema validate
const shiftSchema = validate.object().shape({
  tenCa: validate.string().required('Tên ca là bắt buộc'),
  soNguoiToiDa: validate
    .number()
    .required('Số người tối đa là bắt buộc')
    .min(1, 'Số người tối đa phải lớn hơn 0'),
  gioBatDau: validate.string().required('Giờ bắt đầu là bắt buộc'),
  gioKetThuc: validate.string().required('Giờ kết thúc là bắt buộc'),
})
// Hàm validate dữ liệu
const validateShift = (shift) => {
  try {
    shiftSchema.validateSync(shift, { abortEarly: false })
    return null // Không có lỗi
  } catch (error) {
    return error.inner // Trả về mảng lỗi
  }
}
// #endregion

export default {
  name: 'ShiftManagerPage',
  data() {
    return {
      shift: { maCaKip: null, tenCa: '', soNguoiToiDa: 0, gioBatDau: '', gioKetThuc: '' },
      userIds: [],
      messageFormCreateNewShedule: '',
      createScheduleObject: {
        maCaKip: null,
        maNvs: [],
        trangThaiCapNhap: 'Chờ xác nhận',
        ghiChu: '',
      },
      listShifts: [],
      datatable: null, // Thêm biến để lưu trữ DataTable
      errors: null,
      TrangThaiLichLamViec: TrangThaiLichLamViec,
    }
  },
  methods: {
    // #region [Tải danh sách ca làm việc]
    async loadShifts() {
      console.log('Loading shifts...')

      axiosConfig
        .getFromApi('/Shift/GetAll', ConfigsRequest.takeAuth())
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
    // #endregion
    // #region [Khởi tạo DataTable]
    initDataTable() {
      console.log('Initializing DataTable...')
      // Xóa DataTable cũ nếu đã tồn tại
      if (this.datatable) {
        this.datatable.destroy()
        this.datatable = null
      }

      this.datatable = $('#dt-listShiftsPage').DataTable({
        data: this.listShifts,
        columns: [
          configsDt.defaultTdToShowDetail,
          {
            data: null,
            title: 'Thông tin Ca Làm Việc',
            className: 'text-left',
            render: (data, type, row) => {
              const tenCa = row.tenCa || 'N/A'
              const soNguoiHienTai = row.soNguoiHienTai || 0
              const soNguoiToiDa = row.soNguoiToiDa || 'N/A'

              return `
                <div>
                  <strong>${tenCa}</strong><br/>
                  <span>Số người làm: ${soNguoiHienTai}/${soNguoiToiDa}</span>
                </div>
              `
            },
          },
          {
            data: null,
            title: 'Số người chờ xác nhận',
            className: 'text-center',
            render: (data, type, row) => {
              return `${row.schedules.filter((s) => s.trangThai === 'Chờ xác nhận').length} / ${
                row.soNguoiToiDa
              }`
            },
          },
          {
            data: 'isDelete',
            title: 'Trạng thái',
            className: 'text-center',
            render: (data) => {
              return `<span class="badge ${data ? 'bg-danger' : 'bg-success'}">${
                data ? 'Ngừng hoạt động' : 'Hoạt động'
              }</span>`
            },
          },
          {
            data: 'gioBatDau',
            title: 'Thời gian làm',
            className: 'text-center',
            render: function (data, type, row) {
              const gioBatDau = data || 'N/A'
              const gioKetThuc = row.gioKetThuc || 'N/A'
              return `${gioBatDau} - ${gioKetThuc}`
            },
          },
          {
            data: 'maCaKip',
            title: 'Thao tác',
            className: 'text-center',
            render: (data, type, row) => {
              const isActive = row.isDelete
              return `
                  <div class="d-flex gap-1 justify-content-between flex-column">
                    <span class="btn btn-outline-warning edit-shift" data-id="${data}" title="Sửa" style="cursor: pointer;">
                      <i class="bi icon-pencil"></i>Sửa
                    </span>
                    <span class="btn btn-outline-danger change-status" data-id="${
                      row.maCaKip
                    }" title="Kích hoạt" style="cursor: pointer;">
                      <i class="bi ${isActive ? 'icon-close' : 'icon-check'}"></i> ${
                        isActive ? 'Hủy' : 'Mở'
                      }
                    </span>
                    <span class="btn btn-outline-primary generate-qr" data-id="${data}" title="Tải QR" style="cursor: pointer;">
                      <i class="bi icon-pin"></i>QR
                    </span>
                  </div>
              `
            },
          },
        ],
        order: [],
        language: {
          ...configsDt.defaultLanguageDatatable,
          info: 'Hiển thị _START_ đến _END_ ca trong số _TOTAL_ ca',
        },
      })

      // Gắn sự kiện cho các thao tác
      $('#dt-listShiftsPage tbody').on('click', '.edit-shift', (event) => {
        const id = $(event.currentTarget).data('id')
        const shift = this.listShifts.find((s) => s.maCaKip == id)
        if (shift) this.shift = { ...shift }
      })

      $('#dt-listShiftsPage tbody').on('click', '.generate-qr', async (event) => {
        const id = $(event.currentTarget).data('id')
        await this.generateQRCode(id)
      })

      $('#dt-listShiftsPage tbody').on('click', '.change-status', (event) => {
        const id = $(event.currentTarget).data('id')
        this.changeStatusShift(id)
      })

      // Gắn sự kiện hiển thị chi tiết
      configsDt.attachDetailsControl('#dt-listShiftsPage', this.formatDetails)
    },
    // #endregion
    // #region [Hiển thị chi tiết nhân viên]
    /**
     * Hàm hiển thị chi tiết nhân viên trong ca làm việc
     * @param {Object} rowData - Dữ liệu của hàng hiện tại
     * @returns {jQuery} - HTML chứa thông tin chi tiết nhân viên
     */
    formatDetails(rowData) {
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

      if (currentShift && currentShift.schedules && currentShift.schedules.length > 0) {
        const detailsHtml = `
          <div class="container">
              <div class="row mb-3 align-items-center">
                <!-- Bộ lọc trạng thái -->
                <div class="col-md-6">
                  <label for="status-filter"><strong>Lọc theo trạng thái:</strong></label>
                  <select id="status-filter" class="status-filter form-select">
                    <option value="">Tất cả</option>
                    ${validStatuses
                      .map((status) => `<option value="${status}">${status}</option>`)
                      .join('')}
                    ${validStatuses
                      .map((status) => `<option value="${status}">${status}</option>`)
                      .join('')}
                  </select>
                </div>

                <!-- Bộ lọc ngày -->
                <div class="col-md-6">
                  <label for="date-filter"><strong>Lọc theo ngày:</strong></label>
                  <input id="date-filter" type="date" class="date-filter form-control" />
                </div>

                <!-- Nút chuyển đổi -->
                <div class="col-12 mt-3 d-flex justify-content-end gap-2">
                  <button
                    class="btn-update-status btn btn-primary"
                    title="Cập nhật trạng thái đã chọn!"
                    disabled
                  >
                    Cập nhật
                  </button>
                  <button class="toggle-view-btn btn btn-secondary">
                    Chi tiết
                  </button>
                </div>
              </div>

              <div class="row border-left border-right justify-content-center" style="max-height: 400px; overflow-y: auto">
                  <div class="col-12 mb-3 d-flex align-items-start p-1">
                      <div class="border rounded p-1 m-1 w-100">
                        <input type="checkbox" class="select-all-checkbox" />
                        <label class="ml-2">Chọn tất cả</label>
                      </div>
                  <div class="col-12 mb-3 d-flex align-items-start p-1">
                      <div class="border rounded p-1 m-1 w-100">
                        <input type="checkbox" class="select-all-checkbox" />
                        <label class="ml-2">Chọn tất cả</label>
                      </div>
                  </div>
                  ${currentShift.schedules
                    .map(
                      (employee) => `
                      <div class="employee-item col-6 mb-3 d-flex align-items-start p-1">
                        <div class=" border rounded w-100 p-1">
                            <input type="checkbox" class="employee-checkbox" data-ma-nv="${
                              employee.maNv
                            }" />
                            <div class="ml-2">
                                <p><strong>${employee.tenNhanVien} <span class="text-secondary">[${
                                  employee.maNv
                                }]</span></strong></p>
                                <p class="text-muted"><strong>Trạng thái:</strong> <span class="status-screw">${
                                  employee.trangThai
                                }</span></p>
                                <p><strong>Ngày làm:</strong> <span class="date-screw">${
                                  formatDatetime.formatDate(employee.ngayThangNam) || 'Không có'
                                }</span></p>
                                <div class="additional-info d-none">
                                    <div class="row mb-2">
                                        <div class="col-4">
                                            <p><strong>Giờ vào:</strong> ${
                                              formatDatetime.formatTime(employee.gioVao) ||
                                              'Không có'
                                            }</p>
                                        </div>
                                        <div class="col-4">
                                            <p><strong>Giờ ra:</strong> ${
                                              formatDatetime.formatTime(employee.gioRa) ||
                                              'Không có'
                                            }</p>
                                        </div>
                                        <div class="col-4">
                                            <p><strong>Số giờ làm:</strong> ${
                                              employee.soGioLam || 0
                                            }h</p>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-12">
                                            <p><strong>Lý do nghỉ:</strong> ${
                                              employee.lyDoNghi || 'Không có'
                                            }</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                          </div>
                      </div>
                      `,
                    )
                    .join('')}
              </div>
          </div>`

        const container = $(detailsHtml)

        setTimeout(() => {
          const toggleViewBtn = container.find('.toggle-view-btn')
          const employees = container.find('.employee-item')
          const statusFilter = container.find('.status-filter')
          const dateFilter = container.find('.date-filter')
          const btnUpdateStatus = container.find('.btn-update-status')
          const selectAllCheckbox = container.find('.select-all-checkbox')

          // Toggle chế độ hiển thị
          toggleViewBtn.on('click', () => {
            const isExpanded = toggleViewBtn.hasClass('expanded')
            if (isExpanded) {
              toggleViewBtn.removeClass('expanded').text('Chi tiết')
              employees.removeClass('col-12').addClass('col-6')
              employees.find('.additional-info').addClass('d-none')
            } else {
              toggleViewBtn.addClass('expanded').text('Ẩn')
              employees.removeClass('col-6').addClass('col-12')
              employees.find('.additional-info').removeClass('d-none')
            }
          })
          // Hàm gộp lọc trạng thái và ngày
          const applyFilters = () => {
            const selectedStatus = statusFilter.val()?.trim().toLowerCase() || ''
            const selectedDate = formatDatetime.formatDate(dateFilter.val()?.trim()) || ''
            let hasVisibleEmployees = false

            // Lọc danh sách nhân viên
            employees.each(function () {
              const employee = $(this)
              const employeeStatus = employee.find('.status-screw').text().toLowerCase()
              const employeeDate = employee.find('.date-screw').text().trim()
              // console.table(employeeDate, selectedDate, employeeDate == selectedDate)

              // Kiểm tra trạng thái và ngày
              const matchesStatus = !selectedStatus || employeeStatus.includes(selectedStatus)
              const matchesDate = !selectedDate || employeeDate === selectedDate

              if (matchesStatus && matchesDate) {
                employee.removeClass('d-none').addClass('d-flex')
                hasVisibleEmployees = true
              } else {
                employee.removeClass('d-flex').addClass('d-none')
              }
            })

            // Hiển thị hoặc xóa thông báo nếu không có nhân viên phù hợp
            if (!hasVisibleEmployees) {
              if (employees.parent().find('.no-info-message').length === 0) {
                employees.parent().append(`
              <p class="no-info-message text-center line-through w-100">
                Không có thông tin phù hợp với bộ lọc.
              </p>
            `)
              }
            } else {
              employees.parent().find('.no-info-message').remove()
            }

            // Bật hoặc tắt nút "Cập nhật trạng thái"
            btnUpdateStatus.prop('disabled', !hasVisibleEmployees)
          }

          // Gắn sự kiện cho bộ lọc trạng thái và ngày
          statusFilter.off('change').on('change', applyFilters)
          dateFilter.off('change').on('change', applyFilters)

          // Cập nhật trạng thái nhân viên
          btnUpdateStatus.off('click').on('click', () => {
            const selectedEmployees = container
              .find('.employee-checkbox:checked')
              .map((_, checkbox) => $(checkbox).data('ma-nv')) // Lấy mã nhân viên đã chọn
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
                  if (selectedEmployees.length > 0) {
                    await this.updateEmployeeStatus(selectedEmployees, newStatus, rowData)
                  }
                } catch (error) {
                  console.error('Cập nhật trạng thái lỗi:', error)
                  Swal.fire('Thất bại', 'Không thể cập nhật trạng thái.', 'error')
                }
              }
            })
          })

          // Chọn tất cả checkbox
          selectAllCheckbox.on('change', function () {
            const isChecked = $(this).is(':checked')
            container.find('.employee-checkbox').prop('checked', isChecked)
          })
        }, 0)

        return container
      }

      return $('<div>Không có thông tin chi tiết nhân viên trong ca này.</div>')
    },
    // #endregion
    // #region [Cập nhật trạng thái nhân viên]
    async updateEmployeeStatus(selectedEmployees, newStatus, rowData) {
      // Hiển thị SweetAlert để người dùng nhập hoặc bỏ qua ghi chú
      const { value: ghiChu } = await Swal.fire({
        title: 'Cập nhật Trạng thái',
        input: 'textarea',
        inputPlaceholder: 'Nhập ghi chú (Không bắt buộc)...',
        inputAttributes: {
          'aria-label': 'Nhập ghi chú',
        },
        showCancelButton: true,
        confirmButtonText: 'Cập nhật',
        cancelButtonText: 'Hủy',
      })

      const apiEndpoint =
        selectedEmployees.length === 1 ? '/Schedule/SetStatusOne' : '/Schedule/SetStatusList'

      const requestBody =
        selectedEmployees.length === 1
          ? {
              maNv: selectedEmployees[0],
              trangThaiCapNhap: newStatus,
              maCaKip: rowData.maCaKip,
              ghiChu: ghiChu || '', // Ghi chú không bắt buộc, nếu không có thì truyền ""
            }
          : {
              maNvs: selectedEmployees,
              trangThaiCapNhap: newStatus,
              maCaKip: rowData.maCaKip,
              ghiChu: ghiChu || '', // Ghi chú không bắt buộc
            }

      try {
        // Gọi API cập nhật trạng thái
        const response = await axiosConfig.postToApi(
          apiEndpoint,
          requestBody,
          ConfigsRequest.takeAuth(),
        )
        if (ResponseAPI.handleNotification(response)) {
          console.log(response)
          return
        }
        // Nếu cập nhật thành công, cập nhật danh sách nhân viên trong ca
        if (response && response.success) {
          /* const updatedShift = response.data
          const existingShiftIndex = this.listShifts.findIndex(
            (shift) => shift.maCaKip === updatedShift.maCaKip,
          )
          if (existingShiftIndex !== -1) {
            this.listShifts[existingShiftIndex] = updatedShift
          }
          this.listShifts = [...this.listShifts] // Đảm bảo reactivity */
          Swal.fire(
            'Thành công',
            'Trạng thái đã được cập nhật. Vui lòng load lại trang để được hiển thị cập nhập',
            'success',
          )
        }
      } catch (error) {
        console.error('Lỗi khi cập nhật trạng thái:', error)
        Swal.fire('Thất bại', 'Không thể cập nhật trạng thái.', 'error')
      }
    },
    // #endregion
    // #region [Lấy thông tin CaKip]
    async fetchAndUpdateShift(maCaKip) {
      try {
        // Gọi API lấy thông tin cập nhật của CaKip
        const updatedShiftResponse = await axiosConfig.getFromApi(
          `/Shift/Employees/${maCaKip}`,
          ConfigsRequest.takeAuth(),
        )

        if (updatedShiftResponse && updatedShiftResponse.success) {
          // Cập nhật dữ liệu trong listShifts
          const updatedShiftData = updatedShiftResponse.data
          const shiftIndex = this.listShifts.findIndex((shift) => shift.maCaKip === maCaKip)

          if (shiftIndex !== -1) {
            this.listShifts[shiftIndex] = updatedShiftData
            this.listShifts = [...this.listShifts] // Đảm bảo reactivity
          }
        }
      } catch (error) {
        console.error('Lỗi khi lấy thông tin CaKip:', error)
        Swal.fire('Thất bại', 'Không thể tải lại thông tin CaKip.', 'error')
      }
    },
    // #endregion
    // #region [Thêm mới hoặc cập nhật ca làm việc]
    saveShift() {
      // Thêm đoạn mã validate tại đây
      const errors = validateShift(this.shift)
      if (errors) {
        this.errors = {}
        errors.forEach((error) => {
          this.errors[error.path] = error.message
        })
        return
      }
      const formattedShift = {
        ...this.shift,
        gioBatDau: this.shift.gioBatDau ? `${this.shift.gioBatDau}:00`.slice(0, 8) : '',
        gioKetThuc: this.shift.gioKetThuc ? `${this.shift.gioKetThuc}:00`.slice(0, 8) : '',
      }

      axiosConfig
        .postToApi('/Shift/UpsertCrew', formattedShift)
        .then((response) => {
          if (response.success) {
            toastr.success('Lưu thành công')
            /* // Thêm logic để cập nhật danh sách nhân viên trong ca
            /* // Thêm logic để cập nhật danh sách nhân viên trong ca
            const updatedShift = response.data
            const existingShiftIndex = this.listShifts.findIndex(
              (shift) => shift.maCaKip === updatedShift.maCaKip
              (shift) => shift.maCaKip === updatedShift.maCaKip
            )
            if (existingShiftIndex !== -1) {
              this.listShifts[existingShiftIndex] = updatedShift
            } else {
              this.listShifts.push(updatedShift)
            }
            this.listShifts = [...this.listShifts] // Đảm bảo reactivity
            this.shift = {
              maCaKip: null,
              tenCa: '',
              soNguoiToiDa: 0,
              gioBatDau: '',
              gioKetThuc: '',
            } */
            this.loadShifts()
          } else {
            toastr.error('Lỗi khi lưu:' + response.message)
          }
        })
        .catch((error) => {
          console.error('API Error: ', error)
          toastr.error('Lỗi không xác định khi lưu thông tin ca làm việc.')
        })
    },
    // #endregion
    // #region [Thay đổi trạng thái ca làm việc]
    async changeStatusShift(maCaKip) {
      // Hiển thị hộp thoại với 3 lựa chọn
      const { value: action } = await Swal.fire({
        title: 'Chọn hành động',
        text: 'Bạn muốn thực hiện hành động nào?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonText: 'Sửa trạng thái',
        cancelButtonText: 'Hủy hành động',
        showDenyButton: true,
        denyButtonText: 'Xóa',
      })
      if (action === true) {
        console.log('You fix')
        // Nếu chọn "Sửa trạng thái"
        try {
          const response = await axiosConfig.patchToApi(`/Shift/ChangeStatusShift?id=${maCaKip}`)
          if (response.success) {
            toastr.success(response.message)
            await this.loadShifts()
          } else {
            toastr.error(response.message)
          }
        } catch (error) {
          toastr.error('Lỗi khi thay đổi trạng thái')
          console.error(`Lỗi: ${error}`)
        }
      } else if (action === false) {
        console.log('You delete')
        // Nếu chọn "Xóa"
        const { value: confirmDelete } = await Swal.fire({
          title: 'Xác nhận xóa',
          text: 'Bạn có chắc chắn muốn xóa không?',
          icon: 'warning',
          showCancelButton: true,
          confirmButtonText: 'Xóa',
          cancelButtonText: 'Hủy',
        })

        if (confirmDelete) {
          // Gọi API xóa
          try {
            const response = await axiosConfig.deleteFromApi(`/Shift/Remove/${maCaKip}`)
            if (response.success) {
              toastr.success(response.message)
              await this.loadShifts()
            } else {
              toastr.error(response.message)
            }
          } catch (error) {
            toastr.error('Lỗi khi xóa')
            console.error(`Lỗi: ${error}`)
          }
        }
      }
    },
    // #endregion
    // #region [Tải QR Code]
    async generateQRCode(maCaKip) {
      try {
        const shift = this.listShifts.find((s) => s.maCaKip === maCaKip)
        if (!shift || !shift.qrCodeData) {
          toastr.error('Không tìm thấy dữ liệu QR cho ca làm việc này.')
          return
        }

        const qrCodeData = shift.qrCodeData
        const canvas = document.createElement('canvas')
        await QRCode.toCanvas(canvas, qrCodeData, { width: 200 })
        const link = document.createElement('a')
        link.href = canvas.toDataURL('image/png')
        link.download = `QRCode_Ca_${maCaKip}.png`
        link.click()
      } catch (error) {
        toastr.error('Đã xảy ra lỗi khi tải QR Code: ' + error.message)
      }
    },
    // #endregion
    loadUserIds() {
      console.log('Loading user IDs...')
      axiosConfig
        .getFromApi('/Schedule/GetAllUserId', ConfigsRequest.takeAuth())
        .then((response) => {
          if (response.success) {
            this.userIds = response.data
            console.log('User IDs loaded:', this.userIds)
          } else {
            this.messageFormCreateNewShedule = 'Hiện tại chức năng này không khả dụng'
            console.error('API Error:', response.message)
          }
        })
        .catch((error) => {
          this.messageFormCreateNewShedule = 'Lỗi không xác định khi tải danh sách nhân viên'
          console.error('Error loading data:', error)
        })
    },
    async createSchedule() {
      // Logic để tạo lịch làm việc mới
      console.log('Creating schedule...')
      try {
        const response = await axiosConfig.postToApi(
          '/Schedule/CreateSchedules',
          this.createScheduleObject,
          ConfigsRequest.takeAuth(),
        )
        if (response.success) {
          toastr.success('Tạo lịch làm việc thành công')
          this.createScheduleObject = {
            maCaKip: null,
            maNvs: [],
            trangThaiCapNhap: 'Chờ xác nhận',
            ghiChu: '',
          }
          this.loadShifts()
        } else {
          toastr.error('Lỗi khi tạo lịch làm việc: ' + response.message)
        }
      } catch (err) {
        console.error('Error creating schedule:', err)
        toastr.error('Lỗi không xác định khi tạo lịch làm việc.')
      }
    },
  },
  async mounted() {
    await this.loadShifts()
    await this.loadUserIds()
  },
}
</script>

<style scoped>
/* Các style liên quan đến CRUD CaKip */
.table-responsive {
  overflow-x: auto;
}
.line-through {
  position: relative;
  display: inline-block;
}

.line-through::before,
.line-through::after {
  content: '';
  position: absolute;
  top: 50%;
  width: 100%;
  height: 1px; /* Độ dày của đường gạch */
  background: black; /* Màu của đường gạch */
}

.line-through::before {
  left: -100%; /* Đặt đường gạch bên trái */
  right: 100%; /* Đặt đường gạch bên phải */
}
</style>
