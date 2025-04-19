<template>
  <div class="row">
    <div class="card m-b-30 p-4">
      <h5 class="card-title d-flex justify-content-between align-items-center">
        <strong
          >Đơn hàng gần đây
          <router-link
            to="/admin/bill"
            class="icon-info text-decoration-none text-black"
          ></router-link
        ></strong>
        <span @click="fetchRecentOrders" class="icon-loop p-1" style="cursor: pointer"></span>
      </h5>
      <hr />
      <div v-if="loading" class="text-center">
        <p>Đang tải dữ liệu...</p>
      </div>
      <div v-if="errorMessage" class="text-center">
        <p>{{ errorMessage }}</p>
      </div>
      <div v-if="!loading && !errorMessage">
        <p v-if="recentOrders.length === 0" class="text-center">Không có dữ liệu để hiển thị.</p>
        <div class="row mb-1" v-for="order in recentOrders" :key="order.maHd">
          <div class="col">
            <div
              class="card"
              :style="{ borderColor: TrangThaiDonHang.getColorCode(order.tinhTrang) }"
            >
              <div class="card-body">
                <h5 class="card-title row">
                  <span class="col-6">Người đặt: {{ order.hoTen }} </span>
                  <span class="col-6"
                    ><strong>Ngày đặt:</strong> {{ this.formatDate(order.ngayTao) }}</span
                  >
                </h5>
                <div class="row">
                  <!-- <div class="d-flex justify-content-begin">
                    <hr class="w-50" />
                  </div> -->
                  <div class="col-6">
                    <p class="card-text">
                      <strong>Tổng tiền: </strong>
                      <span class="text-black">{{ this.formatCurrency(order.tongTien) }}</span>
                    </p>
                  </div>
                  <div class="col-6">
                    <p class="card-text">
                      <strong>Trạng thái: </strong>
                      <span :style="{ color: TrangThaiDonHang.getColorCode(order.tinhTrang) }">{{
                        order.tinhTrang
                      }}</span>
                    </p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import * as axiosConfig from '@/utils/axiosClient'
import ConfigsRequest from '@/models/ConfigsRequest'
import { formatCurrency } from '@/constants/formatCurrency'
import { formatDate } from '@/constants/formatDatetime'
import TrangThaiDonHang from '@/constants/trangThaiDonHang'

export default {
  name: 'ListRecentOrders',
  data() {
    return {
      recentOrders: [],
      loading: true,
      errorMessage: '',
      TrangThaiDonHang,
    }
  },
  mounted() {
    this.fetchRecentOrders()
  },
  methods: {
    formatDate,
    formatCurrency,
    async fetchRecentOrders() {
      this.loading = true
      this.errorMessage = '' // Reset error message

      try {
        const response = await axiosConfig.getFromApi(
          '/Dashboard/GetRecentOrders/4',
          ConfigsRequest.takeAuth(),
        )

        if (response.success) {
          this.recentOrders = response.data
        } else {
          this.errorMessage = 'Lỗi: Không thể lấy danh sách đơn hàng gần đây.'
        }
      } catch (error) {
        console.error('Error fetching recent orders:', error)
        this.errorMessage = 'Lỗi khi tải dữ liệu.'
      } finally {
        this.loading = false
      }
    },
  },
}
</script>

<style scoped></style>
