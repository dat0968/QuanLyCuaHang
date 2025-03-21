export default class OrderStatusData {
  constructor(labels = null, data = null) {
    this.labels = labels // Danh sách nhãn, có thể là null
    this.data = data // Danh sách dữ liệu, có thể là null
  }
}
