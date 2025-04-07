// utils/validateYup.js
import * as yup from 'yup'

const validate = {
  userName: yup.string().matches(/^[a-zA-Z0-9]{3,20}$/, 'Tên người dùng không hợp lệ'),
  password: yup
    .string()
    .matches(/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$/, 'Mật khẩu không đủ mạnh'),
  confirmPassword: (password, confirmPassword) =>
    yup
      .string()
      .test('confirmPassword', 'Mật khẩu xác nhận không khớp', (value) => value === password),
  email: yup.string().email('Địa chỉ email không hợp lệ'),
  hoTen: yup
    .string()
    .matches(
      /^[a-zA-ZàáạảãâầấậẩẫăđèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữýỳỹỵÀÁẠẢÃÂẦẤẬẨẪĂĐÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮÝỲỸỴ ]*$/,
      'Tên không hợp lệ',
    ),
  soDienThoai: yup.string().matches(/^(0\d{9})$/, 'Số điện thoại không hợp lệ'),
  diaChi: yup
    .string()
    .matches(
      /^[a-zA-Z0-9àáạảãâầấậẩẫăđèéẹẻẽêềếệểễìíịỉĩòóọỏõôồốộổỗơờớợởỡùúụủũưừứựửữýỳỹỵÀÁẠẢÃÂẦẤẬẨẪĂĐÈÉẸẺẼÊỀẾỆỂỄÌÍỊỈĨÒÓỌỎÕÔỒỐỘỔỖƠỜỚỢỞỠÙÚỤỦŨƯỪỨỰỬỮÝỲỸỴ,./-]*$/,
      'Địa chỉ không hợp lệ',
    ),
  ngaySinh: (date) =>
    yup
      .date()
      .test('ngaySinh', 'Ngày sinh không thể trong tương lai', (value) => value <= new Date()),
  linkAnh: yup
    .string()
    .matches(/^(https?:\/\/.*\.(?:png|jpg|jpeg|gif|svg))$/, 'Liên kết ảnh không hợp lệ'),
}

export default validate
