export default class OrderOverviewData {
  constructor(overview = null, categories = null, totalOrders = 0) {
    this.overview = overview // Danh sách tổng quan đơn hàng, có thể là null
    this.categories = categories // Danh sách các danh mục, có thể là null
    this.totalOrders = totalOrders // Tổng số đơn hàng, mặc định là 0
  }
}

export class OrderOverview {
  constructor(name = null, data = null) {
    this.name = name // Tên tổng quan, có thể là null
    this.data = data // Danh sách dữ liệu đơn hàng, có thể là null
  }
}

export class ParameterDataOrder {
  constructor(count = 0, revenue = 0.0) {
    this.count = count // Số lượng, mặc định là 0
    this.revenue = revenue // Doanh thu, mặc định là 0.0
  }
}
