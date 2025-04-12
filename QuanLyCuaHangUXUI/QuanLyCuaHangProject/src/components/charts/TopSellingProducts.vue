<template>
  <div class="table-responsive">
    <table class="table" :id="tableId"></table>
    <div v-if="products.length === 0" class="alert alert-warning" role="alert">
      Không có dữ liệu để hiển thị.
    </div>
  </div>
</template>

<script>
import * as configsDt from '@/utils/configsDatatable.js'
import * as axiosClient from '@/utils/axiosClient'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import { formatCurrency } from '@/constants/formatCurrency'
import * as ajaxClient from '@/utils/jqueryApiClient'

export default {
  name: 'TopSellingProducts',
  props: {
    tableId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      products: [],
    }
  },
  mounted() {
    this.loadTopSellingProducts()
  },
  methods: {
    loadTopSellingProducts() {
      axiosClient
        .getFromApi('/Dashboard/GetTopSellingProducts')
        .then((response) => {
          if (response.success) {
            this.products = response.data.products
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
      $(`#${this.tableId}`).DataTable({
        data: this.products,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'productId', width: '10%', title: 'ID', className: 'text-center' },
          { data: 'productName', width: '30%', title: 'Tên sản phẩm' },
          { data: 'quantity', width: '25%', title: 'Số lượng bán', className: 'text-center' },
          {
            data: 'totalRevenue',
            width: '40%',
            title: 'Doanh thu mang lại',
            className: 'text-right',
            render: (data) => formatCurrency(data),
          },
        ],
        order: [
          [1, 'asc'],
          [2, 'desc'],
        ],
        language: configsDt.defaultLanguageDatatable,
        initComplete: () => {
          configsDt.attachDetailsControl(`#${this.tableId}`, this.formatProductDetails.bind(this))
        },
      })
    },
    formatProductDetails(rowData) {
      const div = $('<div/>').addClass('loading').text('Loading...')
      ajaxClient
        .getFromApi(`/Dashboard/GetDetailProduct/${rowData.productId}`)
        .then((json) => {
          if (json.success && json.data) {
            div.removeClass('loading')
            const detailsHtml = `
                <div class="container">
                  <div class="row">
                    <p><strong>${json.data.tenSanPham}</strong> - ${json.data.moTa}</p>
                  </div>
                  <div class="row mb-3">
                    ${json.data.chiTietSanPhams
                      .map(
                        (detail) => `
                          <div class="col-6">
                            <div class="row">
                              <div class="col-md-4 d-flex align-items-center border">
                                <img src="/images/${detail.tenHinhAnh}" class="img-fluid rounded" alt="${detail.huongVi}">
                              </div>
                              <div class="col-md-8">
                                <h5>${detail.huongVi}</h5>
                                <p><strong>Giá:</strong> ${detail.donGia.toLocaleString('vi-VN')} VND</p>
                                <p><strong>Số lượng tồn:</strong> ${detail.soLuongTon}</p>
                                ${detail.kichThuoc ? `<p><strong>Kích thước:</strong> ${detail.kichThuoc}</p>` : ''}
                              </div>
                            </div>
                          </div>
                        `,
                      )
                      .join('')}
                  </div>
                </div>`
            div.html(detailsHtml)
          } else {
            div.html('<p>Không có thông tin chi tiết để hiển thị.</p>')
          }
        })
        .catch((error) => {
          div.html('<p>Không thể lấy thông tin chi tiết.</p>')
          console.error('Error fetching details:', error)
        })
      return div
    },
  },
}
</script>

<style scoped></style>
