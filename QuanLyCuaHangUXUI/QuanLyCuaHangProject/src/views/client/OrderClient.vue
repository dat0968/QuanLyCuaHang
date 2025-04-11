<template>
  <div>
    <!-- banner part start-->
    <section class="" style="margin-top: 100px; min-height: 100vh">
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
            <i class="icon-printer py-2 px-4" @click="downloadAllInvoiceAsPDF"></i>
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
                  <div v-if="selectedOrder.lyDoHuy" class="row mb-3">
                    <div class="col-12">
                      <label><strong>Lý do hủy đơn:</strong></label>
                      <input
                        type="text"
                        class="form-control"
                        :value="selectedOrder.lyDoHuy"
                        readonly
                      />
                    </div>
                  </div>
                  <div class="row mb-3">
                    <!-- Tiêu đề cùng nút in hóa đơn-->
                    <div class="d-flex justify-content-between">
                      <h5>Chi tiết hóa đơn</h5>
                      <div class="d-flex gap-4">
                        <i
                          class="icon-printer text-primary"
                          type="button"
                          title="Tải Hóa đơn (PDF)"
                          @click="downloadInvoiceAsPDF(selectedOrder)"
                        ></i>
                        <i
                          class="icon-printer"
                          type="button"
                          title="Tải Hóa đơn (HTML)"
                          @click="downloadInvoiceAsHTML(selectedOrder)"
                        ></i>
                      </div>
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
import pdfMake from 'pdfmake/build/pdfmake'
import pdfFonts from 'pdfmake/build/vfs_fonts'

import * as configsDt from '@/utils/configsDatatable.js'
import * as axiosClient from '@/utils/axiosClient'
import { formatCurrency, convertNumberToWords } from '@/constants/formatCurrency'
import { formatDate } from '@/constants/formatDatetime'
import toastr from 'toastr'
import Swal from 'sweetalert2'
import ConfigsRequest from '@/models/ConfigsRequest'
import TrangThaiDonHang from '@/constants/trangThaiDonHang'
import StoreInfo from '@/constants/storeInfo'

pdfMake.vfs = pdfFonts.vfs // Nhúng font vào pdfmake

export default {
  name: 'OrderClient',
  data() {
    return {
      orders: [],
      filteredOrders: [], // Dữ liệu sau khi lọc
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
    convertNumberToWords,
    formatDate,
    downloadAllInvoiceAsPDF() {
      if (this.filteredOrders.length === 0) {
        toastr.warning('Không có hóa đơn nào để in!')
        return
      }

      const invoiceContents = this.filteredOrders.map((order) => {
        const tableBody = order.chiTietHoaDonKhachs.map((item, index) => [
          index + 1,
          item.tenSanPham ?? 'N/A',
          item.soLuong ?? 0,
          this.formatCurrency(item.donGia ?? 0),
          this.formatCurrency((item.soLuong ?? 0) * (item.donGia ?? 0)),
        ])

        // Thêm tiêu đề cột vào đầu bảng
        tableBody.unshift(['STT', 'Tên sản phẩm', 'Số lượng', 'Đơn giá', 'Thành tiền'])

        const totalAmount = order.chiTietHoaDonKhachs.reduce(
          (sum, item) => sum + (item.soLuong ?? 0) * (item.donGia ?? 0),
          0,
        )
        const vatPercentage = order.vatPercentage || 0
        const vatAmount = totalAmount * (vatPercentage / 100)
        const totalPayment = totalAmount + vatAmount

        return [
          // Thay đổi { content: [...] } thành một mảng
          { text: StoreInfo.COMPANY_NAME, style: 'companyHeader', alignment: 'center' },
          {
            text: StoreInfo.ADDRESS,
            alignment: 'center',
          },
          {
            text: `${StoreInfo.PHONE_NUMBER} - ${StoreInfo.EMAIL}`,
            alignment: 'center',
          },
          { text: ' ' },
          { text: 'HÓA ĐƠN BÁN HÀNG', style: 'header', alignment: 'center' },
          {
            columns: [
              { text: `Số: ${order.maHd}`, alignment: 'left' },
              { text: `Ngày: ${this.formatDate(order.ngayTao)}`, alignment: 'right' },
            ],
            columnGap: 50,
          },
          { text: ' ' },
          { text: 'THÔNG TIN KHÁCH HÀNG', style: 'subheader' },
          { text: `Tên khách hàng: ${order.hoTen ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: `Địa chỉ: ${order.diaChiNhanHang ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: `Số điện thoại: ${order.sdt ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: ' ' },
          { text: 'CHI TIẾT HÓA ĐƠN', style: 'subheader' },
          {
            table: {
              widths: ['auto', '*', 'auto', 'auto', 'auto'],
              body: tableBody,
            },
            layout: 'lightHorizontalLines', // Sử dụng layout mặc định
          },
          { text: ' ' },
          {
            text: `HÌNH THỨC THANH TOÁN: ${order.hinhThucTt ?? 'N/A'}`,
            margin: [0, 5, 0, 5],
          },
          { text: 'TỔNG CỘNG', style: 'subheader', alignment: 'left', margin: [0, 5, 0, 5] },
          {
            table: {
              widths: ['*', 'auto'],
              body: [
                [
                  {
                    text: `Tổng tiền hàng: ${this.formatCurrency(totalAmount)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Thuế VAT (${vatPercentage}%): ${this.formatCurrency(vatAmount)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Tổng tiền thanh toán: ${this.formatCurrency(totalPayment)} (Bằng chữ: ${this.convertNumberToWords(totalPayment)})`,
                    bold: true,
                    alignment: 'left',
                  },
                  '',
                ],
              ],
            },
            layout: 'noBorders',
          },
          { text: ' ' },
          { text: 'GHI CHÚ:', style: 'subheader', margin: [0, 5, 0, 2] },
          { text: `${order.moTa ?? 'Không có ghi chú'}`, margin: [0, 2, 0, 5] },
          { text: ' ' },
          { text: '--------------------------------------------', alignment: 'center' },
          { text: 'Cảm ơn vì đã mua hàng!', alignment: 'center' },
          { text: ' ', pageBreak: 'after' }, // Thêm ngắt trang sau mỗi hóa đơn
        ]
      })

      // Loại bỏ ngắt trang cuối cùng nếu có
      if (invoiceContents.length > 0) {
        invoiceContents[invoiceContents.length - 1].pop() // Xóa phần tử cuối cùng (là ngắt trang)
      }

      // Định nghĩa tài liệu PDF cho tất cả hóa đơn
      const pdfDefinition = {
        content: invoiceContents.flat(), // Sử dụng flat() để kết hợp các mảng con
        styles: {
          header: { fontSize: 18, bold: true, margin: [0, 0, 0, 10] },
          subheader: { fontSize: 14, bold: true, margin: [0, 10, 0, 5] },
          companyHeader: { fontSize: 20, bold: true, margin: [0, 0, 0, 20] },
        },
        footer: function (currentPage, pageCount) {
          return {
            text: `Trang ${currentPage} / ${pageCount}`,
            alignment: 'center',
            margin: [0, 10],
          }
        },
      }

      // Tải xuống file PDF
      pdfMake.createPdf(pdfDefinition).download('DanhSachHoaDon.pdf')
    },
    downloadInvoiceAsPDF(order) {
      if (!order) {
        toastr.info('Không có thông tin hóa đơn để tải xuống!')
        return
      }

      // Kiểm tra chi tiết hóa đơn có tồn tại và là một mảng không
      if (
        !order.chiTietHoaDonKhachs ||
        !Array.isArray(order.chiTietHoaDonKhachs) ||
        order.chiTietHoaDonKhachs.length === 0
      ) {
        toastr.info('Không có chi tiết hóa đơn để tải xuống!')
        return
      }

      // Chuẩn bị dữ liệu cho bảng
      const tableBody = order.chiTietHoaDonKhachs.map((item, index) => [
        index + 1,
        item.tenSanPham ?? 'N/A', // Bổ sung giá trị mặc định nếu không tồn tại
        item.soLuong ?? 0, // Dùng 0 nếu không có số lượng
        this.formatCurrency(item.donGia ?? 0), // Đơn giá
        this.formatCurrency((item.soLuong ?? 0) * (item.donGia ?? 0)), // Thành tiền
      ])

      // Thêm tiêu đề cột vào đầu bảng
      tableBody.unshift(['STT', 'Tên sản phẩm', 'Số lượng', 'Đơn giá', 'Thành tiền'])

      // Tính tổng tiền và thuế VAT
      const totalAmount = order.chiTietHoaDonKhachs.reduce(
        (sum, item) => sum + (item.soLuong ?? 0) * (item.donGia ?? 0),
        0,
      )
      const vatPercentage = order.vatPercentage || 0 // tỷ lệ VAT nếu có
      const vatAmount = totalAmount * (vatPercentage / 100)
      const totalPayment = totalAmount + vatAmount

      // Tạo đối tượng pdfmake
      const docDefinition = {
        content: [
          { text: 'CỬA HÀNG DARK BEE', style: 'companyHeader', alignment: 'center' },
          {
            text: '300, 6 đường Hà Huy Tập, BMT, Đắk Lắk',
            style: 'companyHeader',
            alignment: 'center',
          },
          {
            text: '0262 8884 375 - datntpk03691@gmail.com',
            style: 'companyHeader',
            alignment: 'center',
          },
          { text: ' ', margin: [0, 5, 0, 5] }, // Khoảng trắng giữa các phần
          { text: 'HÓA ĐƠN BÁN HÀNG', style: 'header', alignment: 'center' },
          {
            columns: [
              { text: `Số: ${order.maHd}`, alignment: 'left' },
              { text: `Ngày: ${formatDate(order.ngayTao)}`, alignment: 'right' },
            ],
            columnGap: 50,
          },
          { text: ' ', margin: [0, 5, 0, 5] }, // Khoảng trắng giữa các phần
          { text: 'THÔNG TIN KHÁCH HÀNG', style: 'subheader' },
          { text: `Tên khách hàng: ${order.hoTen ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: `Địa chỉ: ${order.diaChiNhanHang ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: `Số điện thoại: ${order.sdt ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: `Mã số thuế (nếu có): ${order.maSoThue ?? 'N/A'}`, margin: [0, 2, 0, 2] },
          { text: ' ', margin: [0, 5, 0, 5] }, // Khoảng trắng giữa các phần
          { text: 'CHI TIẾT HÓA ĐƠN', style: 'subheader' },
          {
            table: {
              widths: ['auto', '*', 'auto', 'auto', 'auto'],
              body: tableBody,
            },
            layout: {
              hLineWidth: (i, node) => (i === 0 || i === node.table.body.length ? 1 : 0), // Đường viền cho hàng đầu
              vLineWidth: () => 0,
              fillColor: (rowIndex) => (rowIndex === 0 ? '#f0f0f0' : null),
            },
          },
          { text: ' ', margin: [0, 5, 0, 5] }, // Khoảng trắng giữa các phần
          {
            text: `HÌNH THỨC THANH TOÁN: ${order.hinhThucTt ?? 'N/A'}`,
            margin: [0, 5, 0, 5],
          },
          { text: 'TỔNG CỘNG', style: 'subheader', alignment: 'right', margin: [0, 5, 0, 5] },
          {
            table: {
              widths: ['*', 'auto'],
              body: [
                [
                  {
                    text: `Tổng tiền hàng: ${this.formatCurrency(totalAmount)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Thuế VAT (${vatPercentage}%): ${this.formatCurrency(vatAmount)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Tổng tiền thanh toán: ${this.formatCurrency(totalPayment)} (Bằng chữ: ${this.convertNumberToWords(totalPayment)})`,
                    bold: true,
                    alignment: 'left',
                  },
                  '',
                ],
              ],
            },
            layout: 'noBorders', // Không có viền cho bảng tổng cộng
          },
          { text: ' ' },
          { text: 'GHI CHÚ:', style: 'subheader', margin: [0, 5, 0, 2] },
          { text: `${order.moTa ?? 'Không có ghi chú'}`, margin: [0, 2, 0, 5] },
          { text: ' ', margin: [0, 10, 0, 0] }, // Khoảng trắng dưới ghi chú
          { text: '--------------------------------------------', alignment: 'center' },
          { text: 'Cảm ơn vì đã mua hàng!', alignment: 'center' },
        ],
        styles: {
          companyHeader: {
            fontSize: 12,
            bold: true,
            margin: [0, 2, 0, 2],
          },
          header: {
            fontSize: 20,
            bold: true,
            margin: [0, 10, 0, 10],
          },
          subheader: {
            fontSize: 14,
            bold: true,
            margin: [0, 5, 0, 2],
          },
        },
        defaultStyle: {
          font: 'Roboto',
        },
      }

      // Tạo và tải file PDF
      pdfMake.createPdf(docDefinition).download(`HoaDon_${order.maHd}.pdf`)
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
          <p><strong>Tên Khách Hàng:</strong> ${order.hoTen}</p>
          <p><strong>Số Điện Thoại:</strong> ${order.sdt}</p>
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
        .getFromApi(`/OrderClient/Get/`, ConfigsRequest.takeAuth())
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
              let buttonHtml = `<button class="btn btn-primary btn-sm btn-view mx-2" data-id="${row.maHd}"> <i class="icon-doc"></i> Chi tiết</button>`

              // Chỉ hiển thị một nút "Hủy đơn"
              if (TrangThaiDonHang.TrangThaiCoTheDoi.includes(status)) {
                buttonHtml += `<button class="btn btn-danger btn-sm btn-change-status" data-id="${row.maHd}" data-status="${status}"> <i class="icon-close"></i> Hủy đơn</button>`
              }
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

        // Lấy các trạng thái hủy được từ cấu hình
        const availableCancelStatuses = TrangThaiDonHang.isCancelable(currentStatus) || []
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

        // Nếu người dùng chọn trạng thái "Đã hủy"
        let cancellationReason = ''
        if (TrangThaiDonHang.TrangThaiKhongBinhThuong.includes(selectedStatus)) {
          const { value: reason } = await Swal.fire({
            title: 'Nhập lý do hủy đơn',
            input: 'textarea',
            inputPlaceholder: 'Nhập lý do tại đây...',
            showCancelButton: true,
            cancelButtonText: 'Hủy bỏ',
            confirmButtonText: 'Xác nhận',
            inputValidator: (value) => {
              if (!value) {
                return 'Bạn cần nhập lý do!'
              }
              return null
            },
          })

          if (!reason) return // Nếu người dùng không nhập lý do hủy
          cancellationReason = reason // Lưu lý do hủy
        }
        // Gửi yêu cầu API hủy đơn
        axiosClient
          .postToApi(
            `/OrderClient/ChangeStatusOrder?orderId=${orderId}&statusChange=${selectedStatus}${'&reasonCancel=' + cancellationReason}`,
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
      return [TrangThaiDonHang.TatCaTrangThai]
    },
  },
}
</script>

<style scoped></style>
