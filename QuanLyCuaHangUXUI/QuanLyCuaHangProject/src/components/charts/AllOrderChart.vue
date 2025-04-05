<template>
  <div>
    <div class="d-flex justify-content-between align-items-center">
      <div class="col-6">
        <p><strong>Biểu đồ tình trạng đơn hàng</strong></p>
      </div>
      <div class="col-6">
        <div class="row justify-content-between align-items-center">
          <div class="col-6">
            <div v-if="!isSummaryChart" class="mb-2">
              <select
                v-model="selectedRange"
                @change="updateChartData"
                class="form-select form-control"
              >
                <option value="day">Ngày</option>
                <option value="week">Tuần</option>
                <option value="month">Tháng</option>
                <option value="year">Năm</option>
              </select>
            </div>
          </div>
          <div class="col-6">
            <button @click="toggleChart" class="btn-outline-info btn">Đổi biểu đồ</button>
          </div>
        </div>
        v>
      </div>
    </div>
    <canvas ref="orderChart"></canvas>
  </div>
</template>

<script>
import {
  Chart,
  Title,
  Tooltip,
  Legend,
  ArcElement,
  CategoryScale,
  LinearScale,
  PieController,
} from 'chart.js'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'

Chart.register(Title, Tooltip, Legend, ArcElement, CategoryScale, LinearScale, PieController)

export default {
  name: 'GetAllOrderData',
  data() {
    return {
      chart: null,
      chartData: [],
      chartLabels: [],
      summaryData: null,
      detailedData: {},
      selectedRange: 'day',
      isSummaryChart: true,
      chartOptions: {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: {
            position: 'right',
            labels: {
              color: '#333',
            },
          },
          tooltip: {
            callbacks: {
              label: (tooltipItem) => {
                let total = tooltipItem.dataset.data.reduce((acc, val) => acc + val, 0)
                let value = tooltipItem.raw
                let percentage = ((value / total) * 100).toFixed(2) + '%'
                return `${tooltipItem.label}: ${value} (${percentage})`
              },
            },
          },
        },
        animation: {
          duration: 2000,
        },
        hover: {
          mode: 'nearest',
          intersect: true,
        },
        backgroundColor: '#f9f9f9',
      },
    }
  },
  methods: {
    async fetchOrderData() {
      if (this.summaryData) {
        this.chartData = this.summaryData
        this.chartLabels = ['Đã phê duyệt', 'Đang chờ', 'Đang xử lý']
        this.renderChart()
        return
      }
      try {
        const response = await axiosConfig.getFromApi(
          '/Dashboard/GetAllOrderData',
          ConfigsRequest.getSkipAuthConfig(),
        )
        if (response.success) {
          this.summaryData = [
            response.data.approvedOrders,
            response.data.pendingOrders,
            response.data.inProgressOrders,
          ]
          this.chartData = this.summaryData
          this.chartLabels = ['Đã phê duyệt', 'Đang chờ', 'Đang xử lý']
          this.renderChart()
        } else {
          console.error(response.message)
        }
      } catch (error) {
        console.error('Lỗi khi lấy dữ liệu đơn hàng:', error)
      }
    },
    async fetchOrderStatusData() {
      if (this.detailedData[this.selectedRange]) {
        this.chartData = this.detailedData[this.selectedRange].data
        this.chartLabels = this.detailedData[this.selectedRange].labels
        this.renderChart()
        return
      }
      try {
        const response = await axiosConfig.getFromApi(
          `/Dashboard/GetOrderStatusData/${this.selectedRange}`,
          ConfigsRequest.getSkipAuthConfig(),
        )
        if (response.success) {
          this.detailedData[this.selectedRange] = {
            data: response.data.data,
            labels: response.data.labels,
          }
          this.chartData = response.data.data
          this.chartLabels = response.data.labels
          this.renderChart()
        } else {
          console.error(response.message)
        }
      } catch (error) {
        console.error('Lỗi khi lấy dữ liệu chi tiết đơn hàng:', error)
      }
    },
    renderChart() {
      if (this.chart) this.chart.destroy()
      const ctx = this.$refs.orderChart.getContext('2d')

      // Tính tổng dữ liệu của biểu đồ
      let totalData = this.chartData.reduce((acc, val) => acc + val, 0)

      // Nếu tổng dữ liệu bằng 0 thì vẽ biểu đồ trống
      if (totalData === 0) {
        this.chart = new Chart(ctx, {
          type: 'pie',
          data: {
            labels: this.chartLabels,
            datasets: [
              {
                data: [0], // Dữ liệu trống
                backgroundColor: ['transparent'], // Màu nền cho phần dữ liệu trống
                borderColor: ['#ddd'], // Màu viền cho phần trống (nếu muốn)
                borderWidth: 1,
              },
            ],
          },
          options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
              legend: {
                position: 'right',
                labels: {
                  color: '#333',
                },
              },
            },
          },
        })
      } else {
        this.chart = new Chart(ctx, {
          type: 'pie',
          data: {
            labels: this.chartLabels,
            datasets: [
              {
                data: this.chartData,
                backgroundColor: [
                  '#008FFB',
                  '#00E396',
                  '#FF4560',
                  '#775DD0',
                  '#FEB019',
                  '#FF4560',
                  '#546E7A',
                ],
              },
            ],
          },
          options: {
            responsive: true,
            maintainAspectRatio: false,
            plugins: {
              legend: {
                position: 'right',
                labels: {
                  color: '#333',
                },
              },
              tooltip: {
                callbacks: {
                  label: (tooltipItem) => {
                    let total = tooltipItem.dataset.data.reduce((acc, val) => acc + val, 0)
                    let value = tooltipItem.raw
                    let percentage = ((value / total) * 100).toFixed(2) + '%'
                    return `${tooltipItem.label}: ${value} (${percentage})`
                  },
                },
              },
            },
            animation: {
              duration: 2000,
            },
            hover: {
              mode: 'nearest',
              intersect: true,
            },
            backgroundColor: '#f9f9f9',
          },
        })
      }
    },
    toggleChart() {
      this.isSummaryChart = !this.isSummaryChart
      this.updateChartData()
    },
    updateChartData() {
      this.isSummaryChart ? this.fetchOrderData() : this.fetchOrderStatusData()
    },
  },
  mounted() {
    this.fetchOrderData()
  },
}
</script>

<style scoped>
canvas {
  max-width: 100%;
  max-height: 200px;
}
</style>
