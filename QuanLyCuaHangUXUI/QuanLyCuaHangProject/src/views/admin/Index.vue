<script>
import EarningChart from '@/components/charts/EarningChart.vue'
import GetAllOrderData from '@/components/charts/AllOrderChart.vue'
import OrderOverViewChart from '@/components/charts/OrderOverViewChart.vue'
import TopDashboardEntitiesDt from '@/components/charts/TopDashboardEntitiesDt.vue'
import StatisticsObject from '@/components/charts/StatisticsObject.vue'
export default {
  name: 'DashboardAdmin',
  components: {
    EarningChart,
    GetAllOrderData,
    OrderOverViewChart,
    TopDashboardEntitiesDt,
    StatisticsObject,
  },
  props: {},
  data() {
    return {}
  },
  computed: {},
  watch: {},
  mounted() {},
  methods: {},
}
</script>

<style scoped></style>

<template>
  <div>
    <!-- Start XP Breadcrumbbar -->
    <div class="xp-breadcrumbbar">
      <div class="row">
        <div class="col-md-6 col-lg-6">
          <h4 class="xp-page-title">Bảng điều khiển</h4>
        </div>
      </div>
    </div>
    <!-- End XP Breadcrumbbar -->

    <!-- Start XP Contentbar -->
    <div class="xp-contentbar">
      <!-- Start Widget -->
      <!-- Start XP Row -->
      <div class="row">
        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-12 col-xl-8">
          <!-- Start XP Row -->
          <div class="row">
            <!-- Start XP Col -->
            <div class="col-md-12 col-lg-12 col-xl-12">
              <div class="card m-b-30 p-4">
                <OrderOverViewChart />
              </div>
            </div>
            <!-- End XP Col -->
          </div>
          <!-- End XP Row -->
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-12 col-xl-4">
          <div class="row">
            <!-- Start XP Col -->
            <div class="col-12">
              <div class="card bg-white-gradient m-b-30">
                <div class="card-body">
                  <div class="xp-widget-box text-white pt-3">
                    <GetAllOrderData />
                  </div>
                </div>
              </div>
            </div>
            <!-- End XP Col -->

            <!-- Start XP Col -->
            <div class="col-md-12 col-lg-12 col-xl-12">
              <div class="card bg-white-gradient m-b-30">
                <div class="card-body">
                  <div class="xp-widget-box text-dark">
                    <div class="row">
                      <div class="col-12">
                        <EarningChart />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <!-- End XP Col -->
          </div>
        </div>
        <!-- End XP Col -->
      </div>
      <!-- End XP Row -->

      <!-- ! This ever have 2 row -->

      <!-- Start Project -->
      <!-- End XP Row -->
      <div class="row">
        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-8 col-xl-8 align-self-center">
          <TopDashboardEntitiesDt />
        </div>
        <!-- End XP Col -->
        <div class="col-md-12 col-lg-4 col-xl-4">
          <!-- Start XP Col -->
          <StatisticsObject />
          <!-- End XP Col -->

          <!-- Start XP Col -->
          <div class="row">
            <div class="card m-b-30">
              <div class="card-header bg-white">
                <h5 class="card-title text-black mb-0">Lịch làm việc</h5>
              </div>
              <div class="card-body">
                <div data-language="en" class="datepicker-here"></div>
              </div>
            </div>
          </div>
          <!-- End XP Col -->
        </div>
      </div>
      <!-- End XP Row -->
    </div>
    <!-- End XP Contentbar -->
  </div>
</template>

<style></style>
