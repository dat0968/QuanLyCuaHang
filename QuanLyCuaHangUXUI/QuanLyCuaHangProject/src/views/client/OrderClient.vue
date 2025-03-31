<template>
  <div>
    <!-- banner part start-->
    <section class="vh-100" style="margin-top: 5%">
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
            <i class="icon-printer py-2 px-4" @click="downloadAllInvoice"></i>
          </div>
          <div class="col-md-3 d-flex align-items-end"></div>
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
                <div class="modal-body p-4">
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
                            :value="formatDate(selectedOrder.batDauGiao)"
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
                          :value="selectedOrder.hoTen"
                          readonly
                        />
                      </div>
                      <div class="mb-3">
                        <label><strong>Số điện thoại khách hàng:</strong></label>
                        <input
                          type="text"
                          class="form-control"
                          :value="selectedOrder.sdt"
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
                  <div class="row mb-3">
                    <!-- Tiêu đề cùng nút in hóa đơn-->
                    <div class="d-flex justify-content-between">
                      <h5>Chi tiết hóa đơn</h5>
                      <i
                        class="icon-printer"
                        type="button"
                        title="Tải Hóa đơn (PDF)"
                        @click="downloadInvoice(selectedOrder)"
                      ></i>
                      <i
                        class="icon-printer"
                        type="button"
                        title="Tải Hóa đơn (HTML)"
                        @click="downloadInvoiceAsHTML(selectedOrder)"
                      ></i>
                    </div>
                    <div class="col-12 table-responsive">
                      <table class="table">
                        <thead>
                          <tr>
                            <th>#</th>
                            <th>Tên Sản Phẩm</th>
                            <th>Mô Tả</th>
                            <th>Số Lượng</th>
                            <th>Kích Thước</th>
                            <th>Đơn Giá</th>
                            <th>Thành Tiền</th>
                          </tr>
                        </thead>
                        <tbody class="overflow-auto-y">
                          <tr
                            v-for="(item, index) in selectedOrder.chiTietHoaDonKhachs"
                            :key="item.maCtsp"
                          >
                            <td>{{ index + 1 }}</td>
                            <td>{{ item.tenSanPham }}</td>
                            <td>{{ item.moTa }}</td>
                            <td>{{ item.soLuong }}</td>
                            <td>{{ item.kichThuoc || 'N/A' }}</td>
                            <td>{{ formatCurrency(item.donGia) }}</td>
                            <td>{{ formatCurrency(item.soLuong * item.donGia) }}</td>
                          </tr>
                        </tbody>
                      </table>
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
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import jsPDF from 'jspdf'
import autoTable from 'jspdf-autotable'

import * as configsDt from '@/utils/configsDatatable.js'
import * as axiosClient from '@/utils/axiosClient'
import formatCurrency from '@/constants/formatCurrency'
import '@/assets/default/fonts/Roboto-Regular-normal'
import '@/assets/default/fonts/Roboto-Bold-bold'
import '@/assets/default/fonts/Roboto-Italic-italic'
import { formatDate } from '@/constants/formatDatetime'
import toastr from 'toastr'
import Swal from 'sweetalert2'

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
    formatCurrency,
    formatDate,
    downloadAllInvoice() {
      if (this.filteredOrders.length === 0) {
        toastr.warning('Không có hóa đơn nào để in!')
        return
      }

      // Tạo đối tượng jsPDF
      const doc = new jsPDF('p', 'mm', 'a4') // Chế độ dọc, đơn vị mm, kích thước A4

      // Tiêu đề chính của PDF
      doc.setFont('Roboto-Bold', 'bold')
      doc.setFontSize(20)
      doc.text('DANH SÁCH HÓA ĐƠN', 105, 15, { align: 'center' })

      // Vòng lặp qua các hóa đơn để tạo nội dung chi tiết
      let finalY = 25 // Điểm bắt đầu sau tiêu đề chính

      // eslint-disable-next-line no-unused-vars
      this.filteredOrders.forEach((order, index) => {
        // Tiêu đề hóa đơn nhỏ
        doc.setFont('Roboto-Bold', 'bold')
        doc.setFontSize(12)
        doc.text(`Hóa đơn mã ${order.maHd}`, 10, finalY)
        doc.setFont('Roboto-Regular', 'normal')
        doc.setFontSize(10)

        // Điền thông tin chi tiết cơ bản
        doc.text(`Khách hàng: ${order.hoTen}`, 10, finalY + 5)
        doc.text(`Ngày tạo: ${this.formatDate(order.ngayTao)}`, 10, finalY + 10)
        doc.text(`Trạng thái: ${order.tinhTrang}`, 10, finalY + 15)

        finalY += 25 // Bắt đầu cho bảng

        // Cấu hình bảng chi tiết sản phẩm
        const { tableColumns, tableRows } = this.generateTableData(order)

        autoTable(doc, {
          head: [tableColumns.map((column) => column.header)],
          body: tableRows.map((row) => tableColumns.map((column) => row[column.dataKey])),
          startY: finalY,
          styles: { font: 'Roboto-Regular', fontSize: 10 },
          headStyles: { fillColor: [22, 160, 133], textColor: [255, 255, 255] },
          alternateRowStyles: { fillColor: [240, 240, 240] },
        })

        // Cộng dồn Y để bắt đầu hóa đơn tiếp theo bên dưới bảng
        finalY = doc.lastAutoTable.finalY + 10

        // Nếu vượt quá chiều cao của A4, tự động chuyển sang trang mới
        doc.addPage()
        finalY = 10
      })

      // Xuất file PDF
      doc.save('DanhSachHoaDon.pdf')
    },
    generateTableData(order) {
      const tableColumns = [
        { header: 'STT', dataKey: 'stt' },
        { header: 'Tên sản phẩm', dataKey: 'tenSanPham' },
        { header: 'Số lượng', dataKey: 'soLuong' },
        { header: 'Đơn giá', dataKey: 'donGia' },
        { header: 'Thành tiền', dataKey: 'thanhTien' },
      ]

      const tableRows = order.chiTietHoaDonKhachs.map((item, index) => ({
        stt: index + 1,
        tenSanPham: item.tenSanPham,
        soLuong: item.soLuong,
        donGia: this.formatCurrency(item.donGia),
        thanhTien: this.formatCurrency(item.soLuong * item.donGia),
      }))

      return { tableColumns, tableRows }
    },
    downloadInvoice(order) {
      if (!order) {
        alert('Không có thông tin hóa đơn để tải xuống!')
        return
      }

      // Kiểm tra xem chi tiết hóa đơn có tồn tại và là một mảng không
      if (!order.chiTietHoaDonKhachs || !Array.isArray(order.chiTietHoaDonKhachs)) {
        alert('Không có chi tiết hóa đơn để tải xuống!')
        return
      }

      // Xử lý dữ liệu cột và dòng cho bảng
      const tableColumns = [
        { header: 'STT', dataKey: 'stt' },
        { header: 'Tên sản phẩm', dataKey: 'tenSanPham' },
        { header: 'Số lượng', dataKey: 'soLuong' },
        { header: 'Đơn giá', dataKey: 'donGia' },
        { header: 'Thành tiền', dataKey: 'thanhTien' },
      ]
      const tableRows = order.chiTietHoaDonKhachs.map((item, index) => ({
        stt: index + 1,
        tenSanPham: item.tenSanPham,
        soLuong: item.soLuong,
        donGia: this.formatCurrency(item.donGia),
        thanhTien: this.formatCurrency(item.soLuong * item.donGia),
      }))

      // Tạo đối tượng jsPDF
      const doc = new jsPDF()

      // Kiểm tra danh sách font khả dụng
      console.log(doc.getFontList())

      // Sử dụng các font đã được nhúng
      doc.setFont('Roboto-Regular', 'normal') // Font mặc định
      doc.setFontSize(20)

      // Tiêu đề hóa đơn
      doc.text('HÓA ĐƠN CHI TIẾT', 105, 10, { align: 'center' })

      // Điền thông tin khách hàng
      doc.setFontSize(12)
      doc.text(`Tên khách hàng: ${order.hoTen}`, 10, 30)
      doc.text(`Địa chỉ: ${order.diaChiNhanHang}`, 10, 40)
      doc.text(`Số điện thoại: ${order.sdt}`, 10, 50)
      doc.text(`Mã hóa đơn: ${order.maHd}`, 10, 60)
      doc.text(`Ngày tạo: ${formatDate(order.ngayTao)}`, 10, 70)

      // Thêm khoảng cách trước bảng
      doc.setFontSize(14)
      doc.text('Chi tiết sản phẩm:', 10, 90)

      // Thêm bảng chi tiết bằng autoTable
      autoTable(doc, {
        head: [tableColumns.map((column) => column.header)],
        body: tableRows.map((row) => tableColumns.map((column) => row[column.dataKey])),
        startY: 100,
        styles: { font: 'Roboto-Regular', fontSize: 10 },
        headStyles: { fillColor: [22, 160, 133] }, // Màu nền cho tiêu đề bảng
        alternateRowStyles: { fillColor: [240, 240, 240] }, // Màu nền cho hàng chẵn
      })

      // Tính tổng tiền và in ra cuối bảng
      const total = order.chiTietHoaDonKhachs.reduce(
        (sum, item) => sum + item.soLuong * item.donGia,
        0,
      )
      doc.text(`Tổng tiền: ${this.formatCurrency(total)}`, 10, doc.lastAutoTable.finalY + 10)

      // Lưu file PDF với tên file
      doc.save(`HoaDon_${order.maHd}.pdf`)
    },

    downloadInvoiceAsHTML(order) {
      if (!order) {
        alert('Không tìm thấy thông tin hóa đơn để tải xuống!')
        return
      }

      // Nội dung HTML
      const content = `
        <!DOCTYPE html>
        <html>
        <head>
          <title>Hóa Đơn: ${order.maHd}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            h1 { text-align: center; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            th { background-color: #f2f2f2; }
          </style>
        </head>
        <body>
          <h1>Chi Tiết Hóa Đơn</h1>
          <p><strong>Mã Hóa Đơn:</strong> ${order.maHd}</p>
          <p><strong>Tên Khách Hàng:</strong> ${order.tenKhachHang}</p>
          <p><strong>Số Điện Thoại:</strong> ${order.soDienThoaiKhachHang}</p>
          <p><strong>Địa Chỉ:</strong> ${order.diaChiNhanHang}</p>
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Tên Sản Phẩm</th>
                <th>Số Lượng</th>
                <th>Kích Thước</th>
                <th>Đơn Giá</th>
                <th>Thành Tiền</th>
              </tr>
            </thead>
            <tbody>
              ${order.chiTietHoaDonKhachs
                .map(
                  (item, index) => `
                <tr>
                  <td>${index + 1}</td>
                  <td>${item.tenSanPham}</td>
                  <td>${item.soLuong}</td>
                  <td>${item.kichThuoc || 'N/A'}</td>
                  <td>${this.formatCurrency(item.donGia)}</td>
                  <td>${this.formatCurrency(item.soLuong * item.donGia)}</td>
                </tr>
              `,
                )
                .join('')}
            </tbody>
          </table>
          <p><strong>Tổng Thanh Toán:</strong> ${this.formatCurrency(order.chiTietHoaDonKhachs.reduce((sum, item) => sum + item.soLuong * item.donGia, 0))}</p>
        </body>
        </html>
      `

      // Tạo file Blob từ HTML
      const blob = new Blob([content], { type: 'text/html' })
      const link = document.createElement('a')
      link.href = URL.createObjectURL(blob)
      link.download = `HoaDon_${order.maHd}.html`
      link.click()
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
