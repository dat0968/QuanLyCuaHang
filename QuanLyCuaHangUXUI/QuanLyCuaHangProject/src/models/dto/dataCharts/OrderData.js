export default class OrderData {
  constructor(approvedOrders = 0, pendingOrders = 0, inProgressOrders = 0) {
    this.approvedOrders = approvedOrders // Số đơn hàng đã được phê duyệt
    this.pendingOrders = pendingOrders // Số đơn hàng đang chờ
    this.inProgressOrders = inProgressOrders // Số đơn hàng đang xử lý
  }

  static fromJson(json) {
    return new OrderData(json.approvedOrders, json.pendingOrders, json.inProgressOrders)
  }

  toJson() {
    return {
      approvedOrders: this.approvedOrders,
      pendingOrders: this.pendingOrders,
      inProgressOrders: this.inProgressOrders,
    }
  }
}
