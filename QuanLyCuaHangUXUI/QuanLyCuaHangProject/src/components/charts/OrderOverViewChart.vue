<template>
  <div class="card-header bg-white">
    <h5 class="card-title text-black mb-0">Biểu đồ doanh thu</h5>
  </div>
  <div class="cart-body">
    <div>
      <div class="d-flex justify-content-between mb-3">
        <select v-model="selectedRange" @change="fetchOrderOverviewData" class="form-select">
          <option value="day">Ngày</option>
          <option value="week">Tuần</option>
          <option value="month">Tháng</option>
          <option value="year">Năm</option>
        </select>
      </div>

      <canvas ref="orderOverviewChart"></canvas>
    </div>
  </div>
</template>

<script>
import {
  Chart,
  Title,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  BarElement,
  LineElement,
  BarController,
  LineController,
  PointElement,
} from 'chart.js'
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'

Chart.register(
  Title,
  Tooltip,
  Legend,
  CategoryScale,
  LinearScale,
  BarElement,
  LineElement,
  BarController,
  LineController,
  PointElement,
)

export default {
  name: 'OrderOverviewChart',
  data() {
    return {
      chart: null,
      selectedRange: 'week',
      colors: {
        'Đã xác nhận': '#7EABDC',
        'Đã giao cho đơn vị vận chuyển': '#8EC77B',
        'Đang giao hàng': '#F7C567',
        'Chờ thanh toán': '#F39C91',
        'Hoàn trả/Hoàn tiền': '#A89BC8',
        'Đã hủy': '#A0A7B0',
        'Chờ xác nhận': '#6CC2BD',
      },
    }
  },
  methods: {
    async fetchOrderOverviewData() {
      try {
        const response = await axiosConfig.getFromApi(
          `/Dashboard/GetOrderOverViewData/${this.selectedRange}`,
          ConfigsRequest.getSkipAuthConfig(),
        )
        if (response.success) {
          const { overview, categories } = response.data

          const datasets = overview
            .map((item) => {
              const color = this.colors[item.name] || '#999'

              return [
                {
                  label: item.name, // Giữ label chỉ cho bar
                  data: item.data.map((d) => d.count),
                  backgroundColor: this.hexToRGBA(color, 0.4),
                  borderColor: this.hexToRGBA(color, 0.8),
                  yAxisID: 'y-left',
                  type: 'bar',
                  order: 2,
                },
                {
                  label: '', // Không lặp lại tên ở line
                  data: item.data.map((d) => d.revenue),
                  borderColor: color,
                  borderWidth: 2,
                  tension: 0.4,
                  fill: false,
                  yAxisID: 'y-right',
                  type: 'line',
                  order: 1,
                },
              ]
            })
            .flat()

          this.renderChart(categories, datasets)
        }
      } catch (error) {
        console.error('Lỗi khi lấy dữ liệu tổng quan đơn hàng:', error)
      }
    },
    renderChart(labels, datasets) {
      const ctx = this.$refs.orderOverviewChart.getContext('2d')
      if (this.chart) this.chart.destroy()

      this.chart = new Chart(ctx, {
        type: 'bar',
        data: { labels, datasets },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            x: {
              grid: { drawBorder: true, drawOnChartArea: false },
              ticks: { beginAtZero: true },
            },
            'y-left': {
              position: 'left',
              beginAtZero: true,
              title: { display: true, text: 'Số lượng đơn hàng' },
            },
            'y-right': {
              position: 'right',
              beginAtZero: true,
              title: { display: true, text: 'Doanh thu (VND)' },
              grid: { drawOnChartArea: false },
            },
          },
          plugins: {
            legend: {
              position: 'top',
              labels: {
                filter: (legendItem, chartData) =>
                  chartData.datasets[legendItem.datasetIndex].type === 'bar', // Chỉ giữ label của bar
              },
            },

            title: { display: true, text: 'Tổng quan đơn hàng' },
            tooltip: {
              callbacks: {
                label: (tooltipItem) => {
                  const dataset = tooltipItem.dataset
                  return dataset.type === 'bar'
                    ? `Số đơn: ${tooltipItem.raw}`
                    : `Doanh thu: ${tooltipItem.raw.toLocaleString()} VND`
                },
              },
            },
          },
        },
      })
    },
    hexToRGBA(hex, alpha) {
      let r = parseInt(hex.slice(1, 3), 16),
        g = parseInt(hex.slice(3, 5), 16),
        b = parseInt(hex.slice(5, 7), 16)
      return `rgba(${r}, ${g}, ${b}, ${alpha})`
    },
  },
  mounted() {
    this.fetchOrderOverviewData()
  },
}
</script>

<style scoped>
canvas {
  max-width: 100%;
  max-height: 450px;
}
</style>
