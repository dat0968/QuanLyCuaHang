export const StaticValidates = {
  userName: {
    validate: /^[a-zA-Z0-9]{3,20}$/,
    invalidMessage:
      "Tên người dùng không hợp lệ. Vui lòng nhập từ 3 đến 20 ký tự chữ hoặc số.",
    placeholder: "Nhập tên người dùng...",
  },

  password: {
    validate: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/,
    invalidMessage:
      "Mật khẩu không đủ mạnh. Vui lòng nhập ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.",
    placeholder: "P@ssw0rd123",
  },

  confirmPassword: {
    validate: (password, confirmPassword) => password === confirmPassword,
    invalidMessage: "Mật khẩu xác nhận không khớp.",
    placeholder: "Nhập lại mật khẩu...",
  },

  email: {
    validate: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
    invalidMessage: "Địa chỉ email không hợp lệ.",
    placeholder: "example@gmail.com",
  },

  hoTen: {
    validate:
      /^[a-zA-ZàáạảãâầấậẩẫăđèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữýỳỹỵÀÁẠẢÃÂẦẤẬẨẪĂĐÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮÝỲỸỴ ]*$/,
    invalidMessage:
      "Tên không hợp lệ. Chỉ cho phép ký tự chữ, số và khoảng trắng.",
    placeholder: "Nhập họ và tên...",
  },

  soDienThoai: {
    validate: /^(0\d{9})$/,
    invalidMessage:
      "Số điện thoại không hợp lệ. Vui lòng nhập số điện thoại có 10 chữ số.",
    placeholder: "0123456789",
  },

  diaChi: {
    validate:
      /^[a-zA-Z0-9àáạảãâầấậẩẫăđèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữýỳỹỵÀÁẠẢÃÂẦẤẬẨẪĂĐÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮÝỲỸỴ ,./-]*$/,
    invalidMessage:
      "Địa chỉ không hợp lệ. Chỉ cho phép chữ, số và các ký tự (,./-).",
    placeholder: "Nhập địa chỉ...",
  },

  ngaySinh: {
    validate: (date) => new Date(date) <= new Date(),
    invalidMessage: "Ngày sinh không thể trong tương lai.",
    placeholder: "DD/MM/YYYY",
  },

  linkAnh: {
    validate: /^(https?:\/\/.*\.(?:png|jpg|jpeg|gif|svg))$/,
    invalidMessage: "Liên kết ảnh không hợp lệ. Vui lòng nhập URL hợp lệ.",
    placeholder: "https://example.com/image.jpg",
  },
};
