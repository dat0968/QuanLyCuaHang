class EarningData {
  constructor(name = '', data = [], categories = []) {
    this.name = name
    this.data = data
    this.categories = categories
  }

  static fromJson(json) {
    return new EarningData(json.name || '', json.data || [], json.categories || [])
  }

  toJson() {
    return {
      name: this.name,
      data: this.data,
      categories: this.categories,
    }
  }
}

export default EarningData
