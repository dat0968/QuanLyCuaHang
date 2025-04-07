<template>
  <div>
    <div class="d-flex justify-content-between align-items-end mb-3">
      <label for="timeRange" class="form-label">Doanh thu theo thời gian</label>
      <select
        id="timeRange"
        class="form-select rounded w-25"
        v-model="selectedTimeRange"
        @change="fetchEarningData(selectedTimeRange)"
      >
        <option value="day">Ngày</option>
        <option value="week">Tuần</option>
        <option value="month">Tháng</option>
        <option value="year">Năm</option>
      </select>
    </div>
    <canvas id="earningChart" ref="earningChart"></canvas>
  </div>
</template>

<script>
import {
  Chart,
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  LineController,
} from 'chart.js'
import ConfigsRequest from '@/models/ConfigsRequest'
import EarningData from '@/models/dto/dataCharts/EarningData'
import * as axiosConfig from '@/utils/axiosClient'

// Đăng ký các thành phần của Chart.js
Chart.register(
  Title,
  Tooltip,
  Legend,
  LineElement,
  PointElement,
  LinearScale,
  CategoryScale,
  LineController,
)

export default {
  name: 'EarningChart',
  data() {
    return {
      earningData: new EarningData(),
      selectedTimeRange: 'day', // Giá trị mặc định cho dropdown
      chart: null, // Biến để lưu trữ instance của biểu đồ
    }
  },
  mounted() {
    this.fetchEarningData(this.selectedTimeRange)
  },
  methods: {
    async fetchEarningData(timeRange) {
      try {
        const response = await axiosConfig.getFromApi(
          `Dashboard/GetEarningData/${timeRange}`,
          ConfigsRequest.getSkipAuthConfig(),
        )

        // Cập nhật dữ liệu biểu đồ
        this.earningData = EarningData.fromJson(response.data)

        console.log(this.earningData)

        const labels = this.earningData.categories // Nhãn
        const data = this.earningData.data // Dữ liệu

        // Vẽ biểu đồ
        this.renderChart(labels, data)
      } catch (error) {
        console.error('Error fetching earning data', error)
      }
    },
    renderChart(labels, data) {
      // Nếu biểu đồ đã tồn tại, hủy nó trước khi tạo mới
      if (this.chart) {
        this.chart.destroy()
      }

      const ctx = this.$refs.earningChart.getContext('2d')
      this.chart = new Chart(ctx, {
        type: 'line', // Loại biểu đồ
        data: {
          labels: labels,
          datasets: [
            {
              label: 'Doanh thu',
              data: data,
              backgroundColor: 'rgba(75, 192, 192, 0.2)',
              borderColor: 'rgba(75, 192, 192, 1)',
              borderWidth: 1,
            },
          ],
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: true,
            },
          },
        },
      })
    },
  },
}
</script>

<style scoped>
canvas {
  max-width: 100%;
  max-height: 190px;
}
</style>
