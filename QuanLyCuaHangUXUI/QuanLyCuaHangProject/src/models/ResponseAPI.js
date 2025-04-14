import toastr from 'toastr'

class ResponseAPI {
  constructor(
    callBackResult = {
      status: 204,
      success: false,
      message: 'Phản hồi không xác định',
      data: null,
    },
  ) {
    console.log(callBackResult)

    this.status = callBackResult?.status || 204 // Mã trạng thái của phản hồi (status code)
    this.success = callBackResult?.success || false // Trạng thái thành công/chưa thành công
    this.message = callBackResult?.message || 'Phản hồi không xác định' // Thông báo phản hồi từ backend
    this.data = callBackResult?.data || null // Payload dữ liệu trả về từ backend
  }

  static empty() {
    return new ResponseAPI({
      status: null,
      success: false,
      message: 'Phản hồi rỗng',
      data: null,
    })
  }

  // Phương thức dựng object từ JSON đầu vào
  static fromJson(json) {
    return new ResponseAPI({
      status: json?.status || 200,
      success: json?.success || false,
      message: json?.message || '',
      data: json?.data || null,
    })
  }

  // Phương thức kiểm tra và hiển thị thông báo
  static handleNotification(response) {
    const responseJson = this.fromJson(response) // Tạo instance từ JSON

    if (responseJson.success) {
      toastr.success(responseJson.message || 'Thành công!') // Hiển thị thông báo thành công
      return false // Nếu thành công, trả về "false"
    } else {
      toastr.info(responseJson.message || 'Thao tác thất bại!') // Hiển thị thông báo thất bại
      return true // Nếu thất bại, trả về "true"
    }
  }
}

export default ResponseAPI
