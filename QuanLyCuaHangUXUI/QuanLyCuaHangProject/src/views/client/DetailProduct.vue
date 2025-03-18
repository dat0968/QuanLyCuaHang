<template>
  <div>
    <!-- food_menu start-->
    <section class="food_menu gray_bg">
      <div class="container">
        <div class="row">
          <!-- Categories Sidebar -->
          <div class="col-md-3">
            <ul class="list-group">
              <li class="list-group-item bg-warning">
                <h5 class="me-2 fw-bold text-middle">Danh mục</h5>
              </li>
              <li class="list-group-item">
                <input type="radio" name="category" class="me-2" /> Lunch (8)
              </li>
              <li class="list-group-item">
                <input type="radio" name="category" class="me-2" /> Dinner (2)
              </li>
              <li class="list-group-item">
                <input type="radio" name="category" class="me-2" /> Breakfast (4)
              </li>
              <li class="list-group-item">
                <input type="radio" name="category" class="me-2" /> Others (0)
              </li>
              <li class="list-group-item">
                <input type="radio" name="category" class="me-2" /> Drinks (2)
              </li>
              <li class="list-group-item">
                <input type="radio" name="category" class="me-2" /> All (6)
              </li>
            </ul>
          </div>

          <!-- Product Details -->
          <div class="col-md-9">
            <div class="row">
              <!-- Product Images -->
              <div class="col-lg-6 mb-4">
                <img
                  src="https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg"
                  class="img-fluid w-100 rounded"
                  alt="Classic Food"
                />
                <div class="d-flex mt-2">
                  <img
                    src="https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg?auto=compress&cs=tinysrgb&w=80"
                    class="thumbnail me-2 rounded mr-2"
                    alt="Thumbnail 1"
                    style="width: 60px; height: 60px"
                  />
                  <img
                    src="https://images.pexels.com/photos/1640777/pexels-photo-1640777.jpeg?auto=compress&cs=tinysrgb&w=80"
                    class="thumbnail me-2 rounded mr-2"
                    alt="Thumbnail 2"
                    style="width: 60px; height: 60px"
                  />
                </div>
              </div>

              <!-- Product Info -->
              <div class="col-lg-6">
                <h2 class="fw-bold mb-3">Classic Food</h2>
                <div class="rating d-flex align-items-center mb-3">
                  <i class="fas fa-star text-warning me-1"></i>
                  <i class="fas fa-star text-warning me-1"></i>
                  <i class="fas fa-star text-warning me-1"></i>
                  <i class="fas fa-star text-warning me-1"></i>
                  <i class="fas fa-star text-warning me-1"></i>
                  <a href="#" class="text-decoration-none ms-2 text-muted ml-3"
                    >0 đánh giá / Viết đánh giá</a
                  >
                </div>
                <p class="text-muted mb-4">
                  Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc sodales dui vel
                  ipsum scelerisque, eget egestas quam cursus. Fusce ut augue...
                </p>
                <h2 class="fw-bold mb-3">$122.00</h2>
                <div class="d-flex align-items-center mb-4">
                  <input
                    type="number"
                    class="form-control text-center me-2 mr-2"
                    style="width: 80px; height: 40px"
                    value="1"
                    min="1"
                  />
                  <button class="btn btn-dark me-2 mr-2" style="height: 40px">THÊM VÀO GIỎ</button>
                  <button class="btn btn-outline-secondary me-2 mr-2" style="height: 40px">
                    <i class="far fa-heart"></i>
                  </button>
                  <button class="btn btn-outline-secondary mr-2" style="height: 40px">
                    <i class="fas fa-exchange-alt"></i>
                  </button>
                </div>
              </div>
            </div>

            <!-- Description Tabs -->
            <div class="mt-5">
              <ul class="nav nav-tabs">
                <li class="nav-item">
                  <a class="nav-link active px-4" data-bs-toggle="tab" href="#description">MÔ TẢ</a>
                </li>
                <li class="nav-item">
                  <a class="nav-link px-4" data-bs-toggle="tab" href="#reviews">ĐÁNH GIÁ (0)</a>
                </li>
              </ul>
              <div class="tab-content mt-3">
                <div class="tab-pane fade show active" id="description">
                  <p class="text-muted">
                    Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nunc sodales dui vel
                    ipsum scelerisque, eget egestas quam cursus. Ut tristique lacus at feugiat
                    blandit...
                  </p>
                </div>
                <div class="tab-pane fade" id="reviews">
                  <p class="text-muted">Chưa có đánh giá nào.</p>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!-- food_menu part end-->
  </div>
</template>

<script>
export default {
  name: 'DetailProduct',
  components: {},
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
