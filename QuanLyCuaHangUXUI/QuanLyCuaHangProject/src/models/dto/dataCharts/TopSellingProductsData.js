export default class TopSellingProductsData {
  constructor(products = null) {
    this.products = products // products là một mảng các đối tượng ProductStatistics
  }
}

export class ProductStatistics {
  constructor(productId = null, productName = null, quantity = 0, totalRevenue = 0.0) {
    this.productId = productId
    this.productName = productName
    this.quantity = quantity
    this.totalRevenue = totalRevenue
  }
}
