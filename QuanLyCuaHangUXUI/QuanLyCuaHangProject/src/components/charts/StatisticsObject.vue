<template>
  <div class="container">
    <h3 class="row">Biểu đồ thống kê đối tượng</h3>
    <div class="row">
      <div class="col-6">
        <h4>Khách hàng</h4>
        <canvas ref="customerChart"></canvas>
      </div>
      <div class="col-6">
        <h4>Nhân viên</h4>
        <canvas ref="employeeChart"></canvas>
      </div>
    </div>
  </div>
</template>

<script>
import { Chart, Title, Tooltip, Legend, ArcElement } from 'chart.js'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'

Chart.register(Title, Tooltip, Legend, ArcElement)

export default {
  name: 'StatisticsObjectChart',
  data() {
    return {
      customerChart: null,
      employeeChart: null,
      customerData: [],
      employeeData: [],
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'bottom',
            labels: {
              color: '#333',
            },
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => {
                return `${tooltipItem.label}: ${tooltipItem.raw}`
              },
            },
          },
        },
      },
    }
  },
  methods: {
    async fetchStatisticsData() {
      try {
        const response = await axiosConfig.getFromApi(
          '/Dashboard/GetListStatObject',
          ConfigsRequest.getSkipAuthConfig(),
        )
        if (response.success) {
          this.renderCharts(response.data)
        } else {
          console.error(response.message)
        }
      } catch (error) {
        console.error('Lỗi khi lấy dữ liệu thống kê:', error)
      }
    },
    renderCharts(data) {
      const customerData = data.find((item) => item.nameObject === 'Khách hàng')
      const employeeData = data.find((item) => item.nameObject === 'Nhân viên')

      // Dữ liệu cho biểu đồ Khách hàng
      const customerChartData = [customerData.amountActive, customerData.amountUnactive]
      const customerChartLabels = ['Hoạt động', 'Không hoạt động']

      // Dữ liệu cho biểu đồ Nhân viên
      const employeeChartData = [employeeData.amountActive, employeeData.amountUnactive]
      const employeeChartLabels = ['Hoạt động', 'Không hoạt động']

      // Vẽ biểu đồ Khách hàng
      if (this.customerChart) this.customerChart.destroy()
      const customerCtx = this.$refs.customerChart.getContext('2d')
      this.customerChart = new Chart(customerCtx, {
        type: 'pie',
        data: {
          labels: customerChartLabels,
          datasets: [
            {
              label: 'Khách hàng',
              data: customerChartData,
              backgroundColor: ['#008FFB', '#FF4560'],
            },
          ],
        },
        options: this.chartOptions,
      })

      // Vẽ biểu đồ Nhân viên
      if (this.employeeChart) this.employeeChart.destroy()
      const employeeCtx = this.$refs.employeeChart.getContext('2d')
      this.employeeChart = new Chart(employeeCtx, {
        type: 'pie',
        data: {
          labels: employeeChartLabels,
          datasets: [
            {
              label: 'Nhân viên',
              data: employeeChartData,
              backgroundColor: ['#00E396', '#FF4560'],
            },
          ],
        },
        options: this.chartOptions,
      })
    },
  },
  mounted() {
    this.fetchStatisticsData()
  },
}
</script>

<style scoped>
canvas {
  max-width: 100%;
  max-height: 500px;
  margin-bottom: 20px;
}
</style>
