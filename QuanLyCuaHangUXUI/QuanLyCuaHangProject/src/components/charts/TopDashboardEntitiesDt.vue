<template>
  <div class="card bg-white m-b-30">
    <div class="card-header bg-white d-flex justify-content-between">
      <h5 class="card-title text-black mb-0">Danh sách thực thể đáng lưu tâm</h5>
      <select v-model="selectedType" @change="loadData" class="form-control w-25">
        <option v-for="(config, key) in tableConfigs" :key="key" :value="key">
          {{ config.title }}
        </option>
      </select>
    </div>
    <div class="card-body">
      <TopSellingProducts v-if="selectedType === 'topSellingProducts'" tableId="dt-topSelling" />
      <TopEmployees v-if="selectedType === 'topEmployees'" tableId="dt-topEmployees" />
    </div>
  </div>
</template>

<script>
import TopSellingProducts from '@/components/charts/TopSellingProducts.vue'
import TopEmployees from '@/components/charts/TopEmployees.vue'

export default {
  name: 'TopDashboardEntities',
  components: {
    TopSellingProducts,
    TopEmployees,
  },
  data() {
    return {
      selectedType: 'topEmployees', // Mặc định
      tableConfigs: {
        topSellingProducts: {
          title: 'Sản phẩm bán chạy',
        },
        topEmployees: {
          title: 'Top nhân viên',
        },
      },
    }
  },
  watch: {
    selectedType() {
      // Không cần gọi loadData ở đây vì các component con sẽ tự động tải dữ liệu
    },
  },
  mounted() {
    // Không cần gọi loadData ở đây vì các component con sẽ tự động tải dữ liệu
  },
}
</script>

<style scoped></style>
