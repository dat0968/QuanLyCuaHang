<template>
  <div class="row">
    <div class="card m-b-30">
      <div class="card-header bg-white">
        <h5 class="card-title text-black mb-0">Biểu đồ thống kê trạng thái người dùng</h5>
      </div>
      <div class="card-body text-center">
        <div v-show="loading">Đang tải dữ liệu...</div>
        <div v-show="errorMessage">{{ errorMessage }}</div>
        <canvas v-show="!loading && !errorMessage" ref="userChart"></canvas>
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
      userChart: null,
      loading: true,
      errorMessage: '',
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'top',
            labels: { color: '#333' },
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => `${tooltipItem.label}: ${tooltipItem.raw}`,
            },
          },
        },
      },
    }
  },
  methods: {
    async fetchStatisticsData() {
      this.loading = true
      this.errorMessage = ''
      try {
        const response = await axiosConfig.getFromApi(
          '/Dashboard/GetListStatObject',
          ConfigsRequest.getSkipAuthConfig(),
        )
        if (response.success) {
          this.renderChart(response.data)
        } else {
          this.errorMessage = 'Không có dữ liệu để hiển thị.'
        }
      } catch (error) {
        console.error('Lỗi khi lấy dữ liệu thống kê:', error)
        this.errorMessage = 'Lỗi khi tải dữ liệu.'
      } finally {
        this.loading = false
      }
    },
    renderChart(data) {
      if (this.userChart) this.userChart.destroy()

      // Chuẩn bị dữ liệu cho biểu đồ
      const labels = ['Khách hàng', 'Nhân viên']
      const legendLabels = ['🟢 Hoạt động', '🔴 Không hoạt động']
      this.chartOptions.plugins.legend.labels.generateLabels = (chart) => {
        const datasets = chart.data.datasets[0].data
        return legendLabels.map((label, index) => ({
          text: `${label} (${datasets[index] || 0})`,
          fillStyle: chart.data.datasets[0].backgroundColor[index],
          hidden: false,
          index,
        }))
      }
      const chartData = []

      const customerData = data.find((item) => item.nameObject === 'Khách hàng')
      const employeeData = data.find((item) => item.nameObject === 'Nhân viên')

      if (customerData) {
        chartData.push(customerData.amountActive, customerData.amountUnactive)
      } else {
        chartData.push(0, 0)
      }

      if (employeeData) {
        chartData.push(employeeData.amountActive, employeeData.amountUnactive)
      } else {
        chartData.push(0, 0)
      }

      // Vẽ biểu đồ
      const ctx = this.$refs.userChart.getContext('2d')
      this.userChart = new Chart(ctx, {
        type: 'doughnut',
        data: {
          labels,
          datasets: [
            {
              data: chartData,
              backgroundColor: ['#008FFB', '#FF4560', '#00E396', '#FEB019'],
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
  max-height: 200px;
  margin-bottom: 20px;
}
</style>
