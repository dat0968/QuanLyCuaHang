<template>
  <div>
    <!-- banner part start-->
    <section class="vh-100" style="margin-top: 3%">
      <!-- breadcrumb start-->
      <section style="margin-top: 60px; width: 100%" class="breadcrumb breadcrumb_bg">
        <div class="container">
          <div class="row">
            <div class="col-lg-12">
              <div class="breadcrumb_iner text-center">
                <div class="breadcrumb_iner_item">
                  <h2>Danh sách đơn hàng</h2>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>
      <!-- breadcrumb start-->
      <div class="container mt-3">
        <!-- Thanh tìm kiếm -->
        <div class="row mb-4">
          <div class="col-md-3">
            <label for="search-status"><strong>Trạng thái:</strong></label>
            <select class="form-select" v-model="filter.tinhTrang" id="search-status">
              <option value="">Tất cả</option>
              <option v-for="status in statusList" :key="status" :value="status">
                {{ status }}
              </option>
            </select>
          </div>
          <div class="col-md-3">
            <label for="search-date-start"><strong>Ngày tạo từ:</strong></label>
            <input
              type="date"
              class="form-control"
              v-model="filter.ngayTaoTu"
              id="search-date-start"
            />
          </div>
          <div class="col-md-3">
            <label for="search-date-end"><strong>Ngày tạo đến:</strong></label>
            <input
              type="date"
              class="form-control"
              v-model="filter.ngayTaoDen"
              id="search-date-end"
            />
          </div>
          <div class="col-md-3 d-flex align-items-end">
            <button class="btn btn-primary w-100" @click="applyFilter">Lọc</button>
          </div>
        </div>

        <!-- Bảng hiển thị -->
        <div class="row align-items-center">
          <div class="table-responsive">
            <table class="table" id="dt-orderClient"></table>
          </div>
        </div>

        <!-- Modal xem chi tiết -->
        <teleport :to="modalTarget" v-if="selectedOrder">
          <div class="modal fade show d-block" tabindex="-1" @click.self="closeModal">
            <div class="modal-dialog modal-xl modal-dialog-centered">
              <div class="modal-content">
                <div class="modal-header">
                  <h5 class="modal-title">Chi tiết hóa đơn</h5>
                  <button
                    type="button"
                    class="btn btn-danger"
                    @click="closeModal"
                    aria-label="Close"
                  >
                    x
                  </button>
                </div>
                <div class="modal-body">
                  <div class="row mb-3">
                    <!-- Thông tin đơn hàng -->
                    <div class="col-md-7">
                      <h4>Thông tin đơn hàng</h4>

                      <div class="row mb-3">
                        <div class="col-12">
                          <label><strong>Mã đặt hàng:</strong></label>
                          <input
                            type="text"
                            class="form-control"
                            :value="selectedOrder.maHd"
                            readonly
                          />
                        </div>
                      </div>
                      <div class="row mb-3">
                        <div class="col-md-6">
                          <label><strong>Ngày đặt hàng:</strong></label>
                          <input
                            type="text"
                            class="form-control"
                            :value="formatDate(selectedOrder.ngayTao)"
                            readonly
                          />
                        </div>
                        <div class="col-md-6">
                          <label><strong>Ngày giao hàng (dự kiến):</strong></label>
                          <input
                            type="text"
                            class="form-control"
                            :value="formatDate(selectedOrder.thoiGianGiao)"
                            readonly
                          />
                        </div>
                      </div>
                      <div class="mb-3">
                        <label><strong>Trạng thái đơn hàng:</strong></label>
                        <input
                          type="text"
                          class="form-control"
                          :value="selectedOrder.tinhTrang || 'Chờ xác nhận'"
                          readonly
                        />
                      </div>
                    </div>

                    <!-- Thông tin khách hàng -->
                    <div class="col-md-5">
                      <h4>Thông tin khách hàng</h4>
                      <div class="mb-3">
                        <label><strong>Tên khách hàng:</strong></label>
                        <input
                          type="text"
                          class="form-control"
                          :value="selectedOrder.tenKhachHang"
                          readonly
                        />
                      </div>
                      <div class="mb-3">
                        <label><strong>Số điện thoại khách hàng:</strong></label>
                        <input
                          type="text"
                          class="form-control"
                          :value="selectedOrder.soDienThoaiKhachHang"
                          readonly
                        />
                      </div>
                      <div class="mb-3">
                        <label><strong>Địa chỉ nhận hàng:</strong></label>
                        <input
                          type="text"
                          class="form-control"
                          :value="selectedOrder.diaChiNhanHang"
                          readonly
                        />
                      </div>
                    </div>
                  </div>
                </div>

                <div class="modal-footer">
                  <button type="button" class="btn btn-secondary" @click="closeModal">Đóng</button>
                </div>
              </div>
            </div>
          </div>
        </teleport>
      </div>
    </section>
  </div>
</template>

<script>
import * as configsDt from '@/utils/configsDatatable.js'
import * as axiosClient from '@/utils/axiosClient'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import toastr from 'toastr'

export default {
  name: 'OrderClient',
  data() {
    return {
      orders: [],
      filteredOrders: [], // Dữ liệu sau khi lọc

      userId: 100,
      selectedOrder: null, // Lưu thông tin hóa đơn để hiển thị chi tiết trong modal
      modalTarget: typeof document !== 'undefined' ? 'body' : null,
      filter: {
        tinhTrang: '', // Lọc theo trạng thái
        ngayTaoTu: '', // Lọc từ ngày tạo
        ngayTaoDen: '', // Lọc đến ngày tạo
      },
    }
  },
  mounted() {
    this.loadOrders()
  },
  methods: {
    formatDate(date) {
      if (!date) return ''
      const d = new Date(date)
      return d.toISOString().split('T')[0] // Format như 'yyyy-MM-dd'
    },
    loadOrders() {
      axiosClient
        .getFromApi(`/HoaDonKhach/Get/${this.userId}`)
        .then((response) => {
          if (response.success) {
            this.orders = response.data
            this.filteredOrders = this.orders // Hiển thị tất cả dữ liệu ban đầu
            this.$nextTick(() => this.initDataTable())
          } else {
            console.error('API Error:', response.message)
          }
        })
        .catch((error) => {
          console.error('Error loading data:', error)
        })
    },
    applyFilter() {
      console.log('You!')
      // Lọc theo trạng thái
      this.filteredOrders = this.orders.filter((order) => {
        const statusMatch = !this.filter.tinhTrang || order.tinhTrang === this.filter.tinhTrang

        // Lọc theo ngày tạo
        const dateCreate = new Date(order.ngayTao).getTime()
        const startDate = this.filter.ngayTaoTu ? new Date(this.filter.ngayTaoTu).getTime() : null
        const endDate = this.filter.ngayTaoDen ? new Date(this.filter.ngayTaoDen).getTime() : null

        const dateMatch =
          (!startDate || dateCreate >= startDate) && (!endDate || dateCreate <= endDate)

        return statusMatch && dateMatch
      })

      // Tải lại DataTable với dữ liệu đã lọc
      this.$nextTick(() => this.initDataTable())
    },
    initDataTable() {
      $(`#dt-orderClient`).DataTable({
        data: this.filteredOrders,
        destroy: true,
        columns: [
          { data: 'maHd', width: '10%', title: 'Mã HD', className: 'text-center' },
          {
            data: 'ngayTao',
            width: '15%',
            title: 'Ngày tạo',
            className: 'text-center',
            render: function (data) {
              return `${data.slice(0, 10)}`
            },
          },
          { data: 'hoTen', width: '20%', title: 'Khách hàng' },
          {
            data: 'tinhTrang',
            width: '15%',
            title: 'Tình trạng',
            className: 'text-center',
          },
          {
            data: null,
            width: '20%',
            title: 'Hành động',
            className: 'text-center',
            render: (data, type, row) => {
              const status = row.tinhTrang
              let buttonHtml = `<button class="btn btn-primary btn-sm btn-view" data-id="${row.maHd}">Xem chi tiết</button>`

              // Chỉ hiển thị một nút "Hủy đơn"
              buttonHtml += `<button class="btn btn-danger btn-sm btn-change-status" data-id="${row.maHd}" data-status="${status}">Hủy đơn</button>`

              return buttonHtml
            },
          },
        ],
        order: [[0, 'asc']],
        language: configsDt.defaultLanguageDatatable,
        initComplete: () => {
          this.handleTableActions()
        },
      })
    },
    handleTableActions() {
      // Xử lý nút "Xem chi tiết"
      $('#dt-orderClient').on('click', '.btn-view', (event) => {
        const orderId = $(event.target).data('id')
        this.selectedOrder = this.orders.find((order) => order.maHd === orderId)
      })

      // Xử lý nút "Thay đổi trạng thái"
      $('#dt-orderClient').on('click', '.btn-change-status', async (event) => {
        const target = $(event.target)
        const orderId = target.data('id')
        const currentStatus = target.data('status')

        // Các trạng thái có thể hủy dựa trên trạng thái hiện tại
        const cancelableStatuses = {
          'Chờ thanh toán': ['Đã hủy'],
          'Đã xác nhận': ['Đã hủy'],
          'Đã giao cho đơn vị vận chuyển': ['Hoàn trả/Hoàn tiền', 'Đã hủy'],
          'Đang giao hàng': ['Hoàn trả/Hoàn tiền', 'Đã hủy'],
        }

        // Lấy các trạng thái hủy được từ cấu hình
        const availableCancelStatuses = cancelableStatuses[currentStatus] || []
        if (availableCancelStatuses.length === 0) {
          Swal.fire({
            icon: 'info',
            title: 'Không thể hủy đơn',
            text: `Trạng thái hiện tại: "${currentStatus}" không cho phép hủy.`,
          })
          return
        }

        // Hiển thị Swal để chọn trạng thái muốn hủy
        const { value: selectedStatus } = await Swal.fire({
          title: 'Chọn trạng thái hủy',
          input: 'select',
          inputOptions: availableCancelStatuses.reduce((options, status) => {
            options[status] = status
            return options
          }, {}),
          inputPlaceholder: 'Chọn trạng thái',
          showCancelButton: true,
          cancelButtonText: 'Hủy bỏ',
          confirmButtonText: 'Xác nhận',
        })

        if (!selectedStatus) return // Nếu người dùng không chọn trạng thái

        // Gửi yêu cầu API hủy đơn
        axiosClient
          .postToApi(
            `/HoaDonKhach/ChangeStatusOrder?userId=${this.userId}&orderId=${orderId}&statusChange=${selectedStatus}`,
          )
          .then((response) => {
            if (response.success) {
              Swal.fire({
                icon: 'success',
                title: 'Thành công',
                text: 'Cập nhật trạng thái thành công!',
              })
              this.loadOrders() // Load lại danh sách hóa đơn
            } else {
              Swal.fire({
                icon: 'error',
                title: 'Lỗi',
                text: response.message || 'Đã xảy ra lỗi khi cập nhật trạng thái.',
              })
            }
          })
          .catch((error) => {
            Swal.fire({
              icon: 'error',
              title: 'Lỗi',
              text: 'Đã xảy ra lỗi khi gửi yêu cầu.',
            })
            console.error('Error updating status:', error)
          })
      })
    },
    closeModal() {
      this.selectedOrder = null // Đóng modal bằng cách đặt selectedOrder về null
    },
  },
  computed: {
    statusList() {
      return [
        'Chờ xác nhận',
        'Đã xác nhận',
        'Đã giao cho đơn vị vận chuyển',
        'Đang giao hàng',
        'Chờ thanh toán',
        'Đã thanh toán',
        'Hoàn trả/Hoàn tiền',
        'Đã hủy',
      ]
    },
  },
}
</script>

<style scoped></style>
