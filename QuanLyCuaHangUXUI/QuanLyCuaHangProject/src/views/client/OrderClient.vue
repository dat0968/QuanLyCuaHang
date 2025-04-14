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
                      <h5>
                        Chi tiết hóa đơn
                        <select
                          class="form-select"
                          id="selectTypeObject"
                          @change="filterTypeObjectInDetailOrder"
                        >
                          <option value="">Chọn loại đối tượng</option>
                          <option value="product">Sản phẩm</option>
                          <option value="combo">Combo</option>
                        </select>
                      </h5>
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
                      <table class="overflow-auto-y table" style="max-height: 300px">
                        <thead>
                          <tr>
                            <th>Tên Sản Phẩm</th>
                            <th class="text-center">Số Lượng</th>
                            <th>Đơn Giá</th>
                            <th>Thành Tiền</th>
                          </tr>
                        </thead>
                        <tbody>
                          <tr v-if="selectedOrder.chiTietHoaDonKhachs.length === 0">
                            <td colspan="4" class="text-center">
                              Không có sản phẩm nào trong hóa đơn này.
                            </td>
                          </tr>
                          <tr
                            v-for="item in selectedOrder.chiTietHoaDonKhachs"
                            :key="item.maDoiTuong"
                          >
                            <td>
                              <div class="row align-items-center">
                                <div class="col-3">
                                  <img
                                    :src="getImageUrl(item.hinhAnh, `/HinhAnh/Food_Drink`)"
                                    :alt="item.tenDoiTuong"
                                    class="img-fluid rounded border"
                                    style="width: 100px; height: 100px; object-fit: cover"
                                  />
                                </div>
                                <div class="col-9">
                                  {{ item.tenDoiTuong }}<br />
                                  <p>Loại: {{ item.loaiDoiTuong }}</p>
                                  <p v-if="item.kichThuoc">
                                    Kích thước: {{ item.kichThuoc || 'N/A' }}
                                  </p>
                                </div>
                              </div>
                            </td>
                            <td class="text-center">{{ item.soLuong }}</td>
                            <td>{{ formatCurrency(item.donGia) }}</td>
                            <td>{{ formatCurrency(item.soLuong * item.donGia) }}</td>
                          </tr>
                        </tbody>
                      </table>
                    </div>
                    <div class="col-12">
                      <h4><strong>Tổng cộng</strong></h4>
                      <div class="row">
                        <div class="col-md-6">
                          <h5>Hình thức thanh toán:</h5>
                        </div>
                        <div class="col-md-6 text-end">
                          <h5>{{ selectedOrder.hinhThucTt }}</h5>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-md-6">
                          <h5>Tổng tiền hàng:</h5>
                        </div>
                        <div class="col-md-6 text-end">
                          <h5>{{ formatCurrency(selectedOrder.tienGoc) }}</h5>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-md-6">
                          <h5>Phí vận chuyển:</h5>
                        </div>
                        <div class="col-md-6 text-end">
                          <h5>{{ formatCurrency(selectedOrder.phiVanChuyen) }}</h5>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-md-6">
                          <h5>Giảm giá:</h5>
                        </div>
                        <div class="col-md-6 text-end">
                          <h5>{{ formatCurrency(selectedOrder.giamGiaCoupon) }}</h5>
                        </div>
                      </div>
                      <hr />
                      <div class="row">
                        <div class="col-md-6">
                          <h5>Tổng tiền thanh toán:</h5>
                        </div>
                        <div class="col-md-6 text-end">
                          <h5 class="fw-bold text-danger">
                            {{ formatCurrency(selectedOrder.tongTien) }}
                          </h5>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-md-12 text-end">
                          <h5>
                            <em>(Bằng chữ: {{ convertNumberToWords(selectedOrder.tongTien) }})</em>
                          </h5>
                        </div>
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
import authService from '@/services/authService'
import { getImageUrl } from '@/utils/generalMethod'
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
    authService.hasAnyRole(['Customer'])
    this.loadOrders()
  },
  methods: {
    getImageUrl,
    formatCurrency,
    convertNumberToWords,
    formatDate,
    downloadAllInvoiceAsPDF() {
      if (this.filteredOrders.length === 0) {
        toastr.warning('Không có hóa đơn nào để in!')
        return
      }
      // Kiểm tra xem có hóa đơn nào không
      const invoiceContents = this.filteredOrders.map((order) => {
        const tableBody = order.chiTietHoaDonKhachs.map((item, index) => [
          index + 1,
          item.tenDoiTuong + '\nLoại: ' + item.loaiDoiTuong ?? 'N/A',
          item.soLuong ?? 0,
          this.formatCurrency(item.donGia ?? 0),
          this.formatCurrency((item.soLuong ?? 0) * (item.donGia ?? 0)),
        ])

        // Thêm tiêu đề cột vào đầu bảng
        tableBody.unshift(['STT', 'Tên sản phẩm', 'Số lượng', 'Đơn giá', 'Thành tiền'])

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
                    text: `Tổng tiền hàng: ${this.formatCurrency(order.tienGoc)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Phí vận chuyển: ${this.formatCurrency(order.phiVanChuyen)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Giảm giá: ${this.formatCurrency(order.giamGiaCoupon)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Tổng tiền thanh toán: ${this.formatCurrency(order.tongTien)} (Bằng chữ: ${this.convertNumberToWords(order.tongTien)})`,
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
        item.tenDoiTuong + '\nLoại: ' + item.loaiDoiTuong ?? 'N/A',
        item.soLuong ?? 0, // Dùng 0 nếu không có số lượng
        this.formatCurrency(item.donGia ?? 0), // Đơn giá
        this.formatCurrency((item.soLuong ?? 0) * (item.donGia ?? 0)), // Thành tiền
      ])

      // Thêm tiêu đề cột vào đầu bảng
      tableBody.unshift(['STT', 'Tên sản phẩm', 'Số lượng', 'Đơn giá', 'Thành tiền'])

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
          { text: 'TỔNG CỘNG', style: 'subheader', alignment: 'left', margin: [0, 5, 0, 5] },
          {
            table: {
              widths: ['*', 'auto'],
              body: [
                [
                  {
                    text: `Tổng tiền hàng: ${this.formatCurrency(order.tienGoc)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Phí vận chuyển: ${this.formatCurrency(order.phiVanChuyen)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Giảm giá: ${this.formatCurrency(order.giamGiaCoupon)}`,
                    alignment: 'left',
                  },
                  '',
                ],
                [
                  {
                    text: `Tổng tiền thanh toán: ${this.formatCurrency(order.tongTien)} (Bằng chữ: ${this.convertNumberToWords(order.tongTien)})`,
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

      const content = `
        <!DOCTYPE html>
        <html>
        <head>
          <title>Hóa Đơn: ${order.maHd}</title>
          <style>
            body { font-family: Arial, sans-serif; padding: 20px; }
            h1, h4, h5 { text-align: center; }
            table { width: 100%; border-collapse: collapse; margin-top: 20px; }
            th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
            th { background-color: #f2f2f2; }
            .text-end { text-align: right; }
            .fw-bold { font-weight: bold; }
            .text-danger { color: red; }
          </style>
        </head>
        <body>
          <h1>CỬA HÀNG DARK BEE</h1>
          <p>300, 6 đường Hà Huy Tập, BMT, Đắk Lắk</p>
          <p>0262 8884 375 - datntpk03691@gmail.com</p>
          <hr />
          <h4>HÓA ĐƠN BÁN HÀNG</h4>
          <p><strong>Mã Hóa Đơn:</strong> ${order.maHd}</p>
          <p><strong>Ngày Tạo:</strong> ${this.formatDate(order.ngayTao)}</p>
          <h5>THÔNG TIN KHÁCH HÀNG</h5>
          <p><strong>Tên Khách Hàng:</strong> ${order.hoTen}</p>
          <p><strong>Số Điện Thoại:</strong> ${order.sdt}</p>
          <p><strong>Địa Chỉ Nhận Hàng:</strong> ${order.diaChiNhanHang}</p>
          ${order.lyDoHuy ? `<p><strong>Lý Do Hủy:</strong> ${order.lyDoHuy}</p>` : ''}
          <h5>CHI TIẾT HÓA ĐƠN</h5>
          <table>
            <thead>
              <tr>
                <th>#</th>
                <th>Tên Sản Phẩm</th>
                <th>Số Lượng</th>
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
                  <td>${item.tenDoiTuong}<br><small>Loại: ${item.loaiDoiTuong}</small></td>
                  <td>${item.soLuong}</td>
                  <td>${this.formatCurrency(item.donGia)}</td>
                  <td>${this.formatCurrency(item.soLuong * item.donGia)}</td>
                </tr>
              `,
                )
                .join('')}
            </tbody>
          </table>
          <h5>TỔNG CỘNG</h5>
          <p><strong>Hình Thức Thanh Toán:</strong> ${order.hinhThucTt}</p>
          <p><strong>Tổng Tiền Hàng:</strong> ${this.formatCurrency(order.tienGoc)}</p>
          <p><strong>Phí Vận Chuyển:</strong> ${this.formatCurrency(order.phiVanChuyen)}</p>
          <p><strong>Giảm Giá:</strong> ${this.formatCurrency(order.giamGiaCoupon)}</p>
          <p class="fw-bold text-danger"><strong>Tổng Tiền Thanh Toán:</strong> ${this.formatCurrency(order.tongTien)}</p>
          <p><em>(Bằng chữ: ${this.convertNumberToWords(order.tongTien)})</em></p>
          <hr />
          <p style="text-align: center;">Cảm ơn vì đã mua hàng!</p>
        </body>
        </html>
      `

      const blob = new Blob([content], { type: 'text/html' })
      const link = document.createElement('a')
      link.href = URL.createObjectURL(blob)
      link.download = `HoaDon_${order.maHd}.html`
      link.click()
    },

    loadOrders() {
      axiosClient
        .getFromApi(`/OrderClient/Get`, ConfigsRequest.takeAuth())
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
              if (!TrangThaiDonHang.TrangThaiKhongBinhThuong.includes(status)) {
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
    filterTypeObjectInDetailOrder() {
      // Lọc theo loại đối tượng trong chi tiết hóa đơn
      const typeObject = $('#selectTypeObject').val()
      console.log(typeObject)
      this.selectedOrder = this.orders.find((order) => order.maHd == this.selectedOrder.maHd) // Hiển thị tất cả nếu không có loại nào được chọn

      if (typeObject != '') {
        if (typeObject === 'product') {
          this.selectedOrder.chiTietHoaDonKhachs = this.selectedOrder.chiTietHoaDonKhachs.filter(
            (item) => item.loaiDoiTuong !== 'Combo',
          )
        } else if (typeObject === 'combo') {
          this.selectedOrder.chiTietHoaDonKhachs = this.selectedOrder.chiTietHoaDonKhachs.filter(
            (item) => item.loaiDoiTuong === 'Combo',
          )
        }
      }
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

        // Hiển thị Swal để chọn trạng thái muốn hủy
        const { value: selectedStatus } = await Swal.fire({
          title: 'Chọn trạng thái hủy',
          input: 'select',
          inputOptions: TrangThaiDonHang.TrangThaiKhongBinhThuong.reduce((options, status) => {
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
            `/OrderClient/ChangeStatusOrder?orderId=${orderId}&statusChange=${selectedStatus}&reasonCancel=${cancellationReason}`,
            {
              orderId: orderId,
              statusChange: selectedStatus,
              reasonCancel: cancellationReason,
            },
            ConfigsRequest.takeAuth(),
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
