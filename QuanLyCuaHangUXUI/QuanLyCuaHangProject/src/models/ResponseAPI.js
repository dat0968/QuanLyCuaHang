class ResponseAPI {
  constructor(
    callBackResult = {
      status: 500,
      success: false,
      message: "Phản hồi không xác định",
      data: null,
    }
  ) {
    console.log(callBackResult);

    this.status = callBackResult?.status || 500; // Mã trạng thái của phản hồi (status code)
    this.success = callBackResult?.success || false; // Trạng thái thành công/chưa thành công
    this.message = callBackResult?.message || "Phản hồi không xác định"; // Thông báo phản hồi từ backend
    this.data = callBackResult?.data || null; // Payload dữ liệu trả về từ backend
  }

  static empty() {
    return new ResponseAPI({
      status: null,
      success: false,
      message: "Phản hồi rỗng",
      data: null,
    });
  }
}

export default ResponseAPI;
