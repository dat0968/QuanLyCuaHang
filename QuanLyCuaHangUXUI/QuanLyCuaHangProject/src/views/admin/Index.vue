<script></script>
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
        <div class="col-md-12 col-lg-12 col-xl-7">
          <!-- Start XP Row -->
          <div class="row">
            <!-- Start XP Col -->
            <div class="col-md-12 col-lg-12 col-xl-12">
              <div class="card m-b-30">
                <div class="card-header bg-white">
                  <h5 class="card-title text-black mb-0">Weekly Revenue</h5>
                </div>
                <div class="card-body">
                  <div class="xp-chart-label">
                    <ul class="list-inline">
                      <li class="list-inline-item">
                        <p class="text-black">Current Week</p>
                        <h4 class="text-primary-gradient mb-3">
                          <i class="icon-wallet mr-2"></i>78,254
                        </h4>
                      </li>
                      <li class="list-inline-item">
                        <p class="text-black">Previous Week</p>
                        <h4 class="text-success-gradient mb-3">
                          <i class="icon-wallet mr-2"></i>58,605
                        </h4>
                      </li>
                    </ul>
                  </div>
                  <div
                    id="xp-chartist-series-overrides"
                    class="ct-chart ct-golden-section xp-chartist-simple-line"
                  ></div>
                </div>
              </div>
            </div>
            <!-- End XP Col -->
          </div>
          <!-- End XP Row -->
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-12 col-xl-5">
          <div class="row">
            <!-- Start XP Col -->
            <div class="col-md-12 col-lg-12 col-xl-12">
              <div class="card bg-primary-gradient m-b-30">
                <div class="card-body">
                  <div class="xp-widget-box text-white">
                    <div class="row">
                      <div class="col-md-6 col-lg-5 align-self-center">
                        <p class="xp-icon-timer mb-5"><i class="icon-hourglass"></i></p>
                        <h4 class="mb-0 font-26">5.3 hrs</h4>
                        <p class="mb-2">Avg Members Sessions</p>
                        <p class="mb-0">
                          <span class="f-w-7">+18.68%</span>
                          <span class="font-12">vs in last 7 days</span>
                        </p>
                      </div>
                      <div class="col-md-6 col-lg-7">
                        <div
                          id="xp-chartist-widget-bar"
                          class="ct-chart ct-golden-section xp-chartist-label-placement xp-chartist-widget-color"
                        ></div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
            <!-- End XP Col -->

            <!-- Start XP Col -->
            <div class="col-md-6 col-lg-6 col-xl-6">
              <div class="card bg-success-gradient m-b-30">
                <div class="card-body">
                  <div class="xp-widget-box text-white text-center pt-3">
                    <p class="xp-icon-timer mb-4"><i class="icon-trophy"></i></p>
                    <h4 class="mb-2 font-20">Congratulations, Alex!</h4>
                    <p class="mb-3">Welcome aboard, Thank you for joining our Team.</p>
                    <button class="btn btn-white btn-rounded text-success">Ok, got it!</button>
                  </div>
                </div>
              </div>
            </div>
            <!-- End XP Col -->

            <!-- Start XP Col -->
            <div class="col-md-6 col-lg-6 col-xl-6">
              <div class="card bg-danger-gradient m-b-30">
                <div class="card-body">
                  <div class="xp-widget-box xp-widget-newsletter text-white text-center pt-3">
                    <p class="xp-icon-timer mb-4"><i class="icon-paper-plane"></i></p>
                    <h4 class="mb-2 font-20">Subscribe to Newsletter</h4>
                    <p class="mb-3">Please, provide your email address to get latest updates.</p>
                    <form>
                      <div class="input-group">
                        <input
                          type="search"
                          class="form-control"
                          placeholder="Enter Email"
                          aria-label="Search"
                          aria-describedby="button-addon-news"
                        />
                        <div class="input-group-append">
                          <button class="btn" type="submit" id="button-addon-news">GO</button>
                        </div>
                      </div>
                    </form>
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

      <!-- End XP Row -->
      <div class="row">
        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-12 col-xl-8">
          <div class="card m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">Revenue</h5>
            </div>
            <div class="card-body">
              <div class="xp-chart-label">
                <ul class="list-inline">
                  <li class="list-inline-item">
                    <p class="text-black">Today's</p>
                    <h4 class="text-primary-gradient mb-3">
                      <i class="icon-wallet mr-2"></i>8,390
                    </h4>
                  </li>
                  <li class="list-inline-item">
                    <p class="text-black">Last Month</p>
                    <h4 class="text-success-gradient mb-3">
                      <i class="icon-wallet mr-2"></i>24,420
                    </h4>
                  </li>
                  <li class="list-inline-item">
                    <p class="text-black">Last Year</p>
                    <h4 class="text-danger-gradient mb-3">
                      <i class="icon-wallet mr-2"></i>3,25,780
                    </h4>
                  </li>
                </ul>
              </div>
              <div
                id="xp-chartist-stacked-bar"
                class="ct-chart ct-golden-section xp-chartist-stacked-bar"
              ></div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-12 col-xl-4">
          <div class="card m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">Project Resources</h5>
            </div>
            <div class="card-body">
              <div
                id="xp-chartist-donut-fill-rather-chart"
                class="ct-chart ct-golden-section xp-chartist-donut-fill-rather-chart"
              ></div>
              <div class="xp-chart-label mt-3">
                <ul class="list-group">
                  <li class="list-group-item d-flex justify-content-between align-items-center">
                    <p class="mb-0"><i class="mdi mdi-circle-outline text-primary"></i>Direct</p>
                    <span class="badge badge-primary badge-pill">45%</span>
                  </li>
                  <li class="list-group-item d-flex justify-content-between align-items-center">
                    <p class="mb-0"><i class="mdi mdi-circle-outline text-success"></i>Marketing</p>
                    <span class="badge badge-success badge-pill">35%</span>
                  </li>
                  <li class="list-group-item d-flex justify-content-between align-items-center">
                    <p class="mb-0"><i class="mdi mdi-circle-outline text-danger"></i>Others</p>
                    <span class="badge badge-danger badge-pill">20%</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-12 col-xl-4">
          <div class="card m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">Actions History</h5>
            </div>
            <div class="card-body">
              <div class="xp-actions-history">
                <div class="xp-actions-history-list">
                  <div class="xp-actions-history-item">
                    <h6 class="mb-1 text-black">Start Web Designing</h6>
                    <p class="text-muted font-12">5 mins ago</p>
                    <p class="m-b-30">We are start working on USA Project</p>
                  </div>
                </div>
                <div class="xp-actions-history-list">
                  <div class="xp-actions-history-item">
                    <h6 class="mb-1 text-black">Completed Theme Development</h6>
                    <p class="text-muted font-12">15 mins ago</p>
                    <p class="m-b-30">We are completed a theme development into 5 days</p>
                  </div>
                </div>
                <div class="xp-actions-history-list">
                  <div class="xp-actions-history-item">
                    <h6 class="mb-1 text-black">Project Submitted</h6>
                    <p class="text-muted font-12">30 mins ago</p>
                    <p class="m-b-30">We are done process of submitted project</p>
                  </div>
                </div>
                <div class="xp-actions-history-list">
                  <div class="xp-actions-history-item">
                    <h6 class="mb-1 text-black">Received a Payment</h6>
                    <p class="text-muted font-12">45 mins ago</p>
                    <p class="m-b-30">We got monthy payment from clients</p>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-lg-6 col-xl-4">
          <div class="card m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">Social Profile</h5>
            </div>
            <div class="card-body">
              <div class="xp-social-profile">
                <div class="xp-social-profile-img">
                  <div class="row">
                    <div class="col-4 px-1">
                      <img
                        src="../../assets/admin/images/ui-images/image-circle.jpg"
                        class="rounded img-fluid"
                        alt="img"
                      />
                    </div>
                    <div class="col-4 px-1">
                      <img
                        src="../../assets/admin/images/ui-images/image-rounded.jpg"
                        class="rounded img-fluid"
                        alt="img"
                      />
                    </div>
                    <div class="col-4 px-1">
                      <img
                        src="../../assets/admin/images/ui-images/image-thumbnail.jpg"
                        class="rounded img-fluid"
                        alt="img"
                      />
                    </div>
                  </div>
                </div>
                <div class="xp-social-profile-top">
                  <div class="row">
                    <div class="col-3">
                      <div class="xp-social-profile-star py-3">
                        <i class="icon-star font-20"></i>
                      </div>
                    </div>
                    <div class="col-6">
                      <div class="xp-social-profile-avatar text-center">
                        <img
                          src="../../assets/admin/images/ui-media/media-image-8.jpg"
                          alt="user-profile"
                          class="rounded-circle img-fluid"
                        /><span class="xp-social-profile-live"></span>
                      </div>
                    </div>
                    <div class="col-3">
                      <div class="xp-social-profile-menu text-right py-3">
                        <i class="icon-options font-20"></i>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="xp-social-profile-middle text-center">
                  <div class="row">
                    <div class="col-12">
                      <div class="xp-social-profile-title">
                        <h5 class="my-1 text-black">karina_simons</h5>
                      </div>
                      <div class="xp-social-profile-subtitle">
                        <p class="mb-3 text-muted">Karina Simons</p>
                      </div>
                      <div class="xp-social-profile-desc">
                        <p class="text-muted mb-1">
                          Lifestyle coach and photographer <br />delivering best images only...
                        </p>
                      </div>
                    </div>
                  </div>
                </div>
                <div class="xp-social-profile-bottom text-center">
                  <div class="row">
                    <div class="col-4">
                      <div class="xp-social-profile-media pt-3">
                        <h5 class="text-black my-1">45</h5>
                        <p class="mb-0 text-muted">Posts</p>
                      </div>
                    </div>
                    <div class="col-4">
                      <div class="xp-social-profile-followers pt-3">
                        <h5 class="text-black my-1">278k</h5>
                        <p class="mb-0 text-muted">Fans</p>
                      </div>
                    </div>
                    <div class="col-4">
                      <div class="xp-social-profile-following pt-3">
                        <h5 class="text-black my-1">552</h5>
                        <p class="mb-0 text-muted">Likes</p>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-lg-6 col-xl-4">
          <div class="card m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">To do Lists</h5>
            </div>
            <div class="card-body">
              <div class="xp-to-do-list">
                <ul id="list-group" class="list-group list-group-flush"></ul>
                <form class="add-items">
                  <div class="input-group mt-3">
                    <input
                      type="text"
                      class="form-control"
                      id="todo-list-item"
                      placeholder="What do you need to do today?"
                      aria-label="What do you need to do today?"
                      aria-describedby="button-addon-to-do-list"
                    />
                    <div class="input-group-append">
                      <button
                        class="btn btn-primary add"
                        id="button-addon-to-do-list"
                        type="submit"
                      >
                        Add to List
                      </button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->
      </div>
      <!-- End XP Row -->

      <!-- Start Project -->
      <!-- End XP Row -->
      <div class="row">
        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-8 col-xl-8 align-self-center">
          <div class="card bg-white m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">Our Projects</h5>
            </div>
            <div class="card-body">
              <div class="table-responsive">
                <table class="table table-hover">
                  <thead>
                    <tr>
                      <th class="border-top-0">Index</th>
                      <th class="border-top-0">Project Name</th>
                      <th class="border-top-0">Earnings</th>
                      <th class="border-top-0">Start Date</th>
                      <th class="border-top-0">Due Date</th>
                      <th class="border-top-0">Reviews</th>
                      <th class="border-top-0">Status</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr>
                      <td>1</td>
                      <td>
                        <a href="javascript:void(0);">Web Designing</a>
                      </td>
                      <td>$100</td>
                      <td>01/05/2018</td>
                      <td>30/07/2018</td>
                      <td>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                      </td>
                      <td>
                        <span class="badge badge-pill badge-success btn-shadow">Completed</span>
                      </td>
                    </tr>
                    <tr>
                      <td>2</td>
                      <td>
                        <a href="javascript:void(0);">Website Development</a>
                      </td>
                      <td>$300</td>
                      <td>10/08/2018</td>
                      <td>15/09/2018</td>
                      <td>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star-half text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                      </td>
                      <td>
                        <span class="badge badge-pill badge-primary btn-shadow">Running</span>
                      </td>
                    </tr>
                    <tr>
                      <td>3</td>
                      <td>
                        <a href="javascript:void(0);">UI App Kit</a>
                      </td>
                      <td>$250</td>
                      <td>05/05/2018</td>
                      <td>20/07/2018</td>
                      <td>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                      </td>
                      <td>
                        <span class="badge badge-pill badge-danger btn-shadow">Cancelled</span>
                      </td>
                    </tr>
                    <tr>
                      <td>4</td>
                      <td>
                        <a href="javascript:void(0);">Social Marketing</a>
                      </td>
                      <td>$550</td>
                      <td>01/06/2018</td>
                      <td>30/08/2018</td>
                      <td>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star-half text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                      </td>
                      <td>
                        <span class="badge badge-pill badge-info btn-shadow">Pending</span>
                      </td>
                    </tr>
                    <tr>
                      <td>5</td>
                      <td>
                        <a href="javascript:void(0);">App Development</a>
                      </td>
                      <td>$270</td>
                      <td>11/04/2018</td>
                      <td>25/05/2018</td>
                      <td>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star-half text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                      </td>
                      <td>
                        <span class="badge badge-pill badge-secondary btn-shadow">Processing</span>
                      </td>
                    </tr>
                    <tr>
                      <td>6</td>
                      <td>
                        <a href="javascript:void(0);">Logo Designing</a>
                      </td>
                      <td>$150</td>
                      <td>10/04/2018</td>
                      <td>21/05/2018</td>
                      <td>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star text-warning"></i>
                        <i class="mdi mdi-star-outline text-warning"></i>
                      </td>
                      <td>
                        <span class="badge badge-pill badge-success btn-shadow">Completed</span>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->

        <!-- Start XP Col -->
        <div class="col-md-12 col-lg-4 col-xl-4">
          <div class="card m-b-30">
            <div class="card-header bg-white">
              <h5 class="card-title text-black mb-0">Calender</h5>
            </div>
            <div class="card-body">
              <div data-language="en" class="datepicker-here"></div>
            </div>
          </div>
        </div>
        <!-- End XP Col -->
      </div>
      <!-- End XP Row -->
    </div>
    <!-- End XP Contentbar -->
  </div>
</template>

<style></style>
