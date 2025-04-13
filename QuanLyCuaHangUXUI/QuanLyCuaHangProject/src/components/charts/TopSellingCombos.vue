<template>
  <div class="table-responsive">
    <table class="table" :id="tableId"></table>
    <div v-if="combos.length === 0" class="alert alert-warning" role="alert">
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

export default {
  name: 'TopSellingCombos',
  components: {},
  props: {
    tableId: {
      type: String,
      required: true,
    },
  },
  data() {
    return { combos: [] }
  },
  mounted() {
    this.loadTopSellingProducts()
  },
  methods: {
    loadTopSellingProducts() {
      axiosClient
        .getFromApi('/Dashboard/GetTopSellingCombos')
        .then((response) => {
          if (response.success) {
            this.combos = response.data
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
        data: this.combos,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'maCombo', width: '10%', title: 'ID', className: 'text-center' },
          { data: 'tenCombo', width: '30%', title: 'Tên sản phẩm' },
          { data: 'soLuong', width: '25%', title: 'Số lượng bán', className: 'text-center' },
          {
            data: 'tongTien', // Giả sử bạn có tính toán doanh thu để hiện thị
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

      // Lấy chi tiết sản phẩm từ rowData
      const detailsHtml = `
            <div class="container">
                <div class="row">
                    <p><strong>${rowData.tenCombo}</strong> - ${rowData.moTa}</p>
                </div>
                <div class="row mb-3 gap-1 justify-content-between">
                    ${
                      rowData.chiTietCombos.length > 0
                        ? rowData.chiTietCombos
                            .map(
                              (detail) => `
                                    <div class="col-5">
                                        <div class="row">
                                            <div class="col-md-4 d-flex align-items-center border">
                                                <img src="${detail.hinh || '/images/default.png'}" class="img-fluid rounded" alt="${detail.tenDanhMuc || 'Hình ảnh sản phẩm'}">
                                            </div>
                                            <div class="col-md-8">
                                                <h5>${detail.tenSanPham}</h5>
                                                <p><strong>Giá:</strong> ${formatCurrency(detail.donGia || 0)}</p>
                                                <p><strong>Số lượng tồn:</strong> ${detail.soLuongSp}</p>
                                            </div>
                                        </div>
                                    </div>
                                `,
                            )
                            .join('')
                        : '<p>Không có chi tiết nào để hiển thị.</p>'
                    }
                </div>
            </div>`

      div.html(detailsHtml)
      return div
    },
  },
}
</script>

<style scoped></style>
