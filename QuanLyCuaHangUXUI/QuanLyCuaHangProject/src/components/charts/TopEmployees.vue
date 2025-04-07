<template>
  <div class="table-responsive">
    <table class="table" :id="tableId"></table>
  </div>
</template>

<script>
import * as configsDt from '@/utils/configsDatatable.js'
import * as axiosClient from '@/utils/axiosClient'
import $ from 'jquery'
import 'datatables.net'
import 'datatables.net-dt/css/dataTables.dataTables.css'
import formatCurrency from '@/constants/formatCurrency'

export default {
  name: 'TopEmployees',
  props: {
    tableId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      employees: [],
    }
  },
  mounted() {
    this.loadTopEmployees()
  },
  methods: {
    loadTopEmployees() {
      axiosClient
        .getFromApi('/Dashboard/GetEmployeeOrderStats')
        .then((response) => {
          if (response.success) {
            this.employees = response.data
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
        data: this.employees,
        destroy: true,
        columns: [
          configsDt.defaultTdToShowDetail,
          { data: 'maNv', width: '10%', title: 'ID', className: 'text-center' },
          { data: 'hoTen', width: '30%', title: 'Tên nhân viên' },
          {
            data: 'soDonHangDamNhan',
            width: '25%',
            title: 'Số đơn hàng',
            className: 'text-center',
          },
          {
            data: 'doanhThuMangLai',
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
          configsDt.attachDetailsControl(`#${this.tableId}`, this.formatEmployeeDetails.bind(this))
        },
      })
    },
    formatEmployeeDetails(rowData) {
      const employee = this.employees.find((e) => e.maNv === rowData.maNv)
      if (!employee) {
        return $('<div/>').html('<p>Không tìm thấy thông tin chi tiết nhân viên.</p>')
      }

      return $(`
        <div class="container">
          <div class="row">
            <div class="col-md-6">
              <p><strong>Mã nhân viên:</strong> ${employee.maNv}</p>
              <p><strong>Họ tên:</strong> ${employee.hoTen}</p>
              <p><strong>Giới tính:</strong> ${employee.gioiTinh}</p>
              <p><strong>Email:</strong> ${employee.email.trim()}</p>
              <p><strong>Số điện thoại:</strong> ${employee.sdt}</p>
              <p><strong>Địa chỉ:</strong> ${employee.diaChi}</p>
            </div>
            <div class="col-md-6">
              <p><strong>Ngày sinh:</strong> ${new Date(employee.ngaySinh).toLocaleDateString('vi-VN')}</p>
              <p><strong>Ngày vào làm:</strong> ${new Date(employee.ngayVaoLam).toLocaleDateString('vi-VN')}</p>
              <p><strong>Tình trạng:</strong> ${employee.tinhTrang}</p>
              <p><strong>Số đơn hàng đảm nhận:</strong> ${employee.soDonHangDamNhan || '0'}</p>
              <p><strong>Doanh thu mang lại:</strong> ${formatCurrency(employee.doanhThuMangLai || 0)}</p>
            </div>
          </div>
        </div>
      `)
    },
  },
}
</script>

<style scoped></style>
