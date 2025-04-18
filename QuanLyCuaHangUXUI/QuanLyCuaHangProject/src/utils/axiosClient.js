import axios from 'axios'
import { jwtDecode } from 'jwt-decode'
import ResponseAPI from '@/models/ResponseAPI'
import ConfigsRequest from '@/models/ConfigsRequest'

import toastr from 'toastr'
import 'toastr/build/toastr.min.css'
import router from '@/router/index'
import Cookies from 'js-cookie' // Import js-cookie
import { GetApiUrl } from '@constants/api'
let getApiUrl = GetApiUrl()
// Base Axios Client
const axiosClient = axios.create({
  baseURL: getApiUrl+'/api', // Thay bằng base URL của API bạn
  timeout: 500000, // Giới hạn timeout (ms)
  headers: {
    'Content-Type': 'application/json',
  },
})

// Hàm đọc accesstoken (tương tự hàm ReadToken auth.js)
export function ReadToken(token) {
  if (token) {
    const decoded = jwtDecode(token)
    return {
      IdUser: decoded.sub,
      Phone: decoded.PhoneNumber,
      Name: decoded.FullName,
      Role: decoded.role,
      Exp: decoded.exp, // Đơn vị giây
    }
  } else {
    return null
  }
}

// Hàm refresh token (dựa trên logic auth.js)
async function refreshAccessToken() {
  const refreshToken = Cookies.get('refreshToken') // Lấy refresh token từ cookie

  if (!refreshToken) {
    console.log('Không tìm thấy refresh token trong cookie.')
    return false // Hoặc ném lỗi nếu cần
  }

  try {
    const readtoken = ReadToken(Cookies.get('accessToken')) // Đọc thông tin từ access token
    if (!readtoken) {
      console.log('Không thể đọc thông tin từ access token.')
      return false // Hoặc ném lỗi
    }

    const content = {
      id: readtoken.IdUser,
      hoTen: readtoken.Name,
      sdt: readtoken.Phone,
      vaiTro: readtoken.Role,
      refreshToken: refreshToken,
    }

    const response = await axios.post(`${axiosClient.baseURL}/Account/RenewAccessToken`, content)

    if (response.status === 200 && response.data.success) {
      const { accessToken } = response.data.data
      Cookies.set('accessToken', accessToken, { expires: 3 / 24 }) // Lưu vào cookie, thời hạn 3 giờ
      return accessToken
    } else {
      console.error('Lỗi khi làm mới access token:', response.data)
      return false
    }
  } catch (error) {
    console.error('Lỗi trong quá trình làm mới access token:', error)
    return false
  }
}

// Middleware (interceptors) thêm Authorization header và xử lý refresh token
axiosClient.interceptors.request.use(
  async (config) => {
    const requiresAuth = !config.headers.skipAuth

    if (!requiresAuth) {
      return config // Không yêu cầu xác thực, bỏ qua
    }

    const accessToken = Cookies.get('accessToken')

    if (!accessToken) {
      // Yêu cầu xác thực nhưng không có token
      console.warn('Không có access token, chuyển hướng đến trang đăng nhập.')
      router.push('/login') // Chuyển hướng đến trang đăng nhập
      return config // Quan trọng: Ngăn chặn request được gửi đi
    }
    // Kiểm tra token hết hạn bằng cách sử dụng ReadToken
    const readtoken = ReadToken(accessToken)
    if (readtoken && readtoken.Exp * 1000 < Date.now()) {
      // Token đã hết hạn, thử làm mới
      const newAccessToken = await refreshAccessToken()
      if (newAccessToken) {
        config.headers.Authorization = `Bearer ${newAccessToken}`
      } else {
        // Không thể làm mới token, chuyển hướng đến trang đăng nhập
        console.log('Không thể làm mới token, chuyển hướng đến trang đăng nhập.')
        router.push('/login')
        return config // Hoặc ném lỗi nếu cần
      }
    } else {
      // Token còn hiệu lực, thêm vào header
      config.headers.Authorization = `Bearer ${accessToken}`
    }

    return config
  },
  (error) => {
    return Promise.reject(error)
  },
)

// Xử lý phản hồi với các lỗi
axiosClient.interceptors.response.use(
  (response) => {
    if (response.status >= 200 && response.status < 300) {
      return response.data
    }
    toastr.info('Hiện không thể xử lí yêu cầu của bạn.')
    router.push('/')
    return response.data
  },
  (error) => {
    if (error.response) {
      console.error(`API Error: ${error.response.status}`, error.response.data)

      let routeParams = {
        name: 'Error',
        params: { status: error.response.status.toString() }, // Chuyển status sang string
        query: { message: error.response?.data?.message ?? 'Lỗi không xác định từ API' },
      }

      switch (error.response.status) {
        case 401:
          // Unauthorized
          toastr.warning(
            'Phiên đăng nhập đã hết hạn hoặc bạn không có quyền truy cập. Vui lòng đăng nhập lại.',
          )

          // ? Lưu lịch sử trang web không có quyền truy cập.
          routeParams.state = {
            from: router.currentRoute.fullPath,
          }

          router.push({
            name: 'Error',
            params: { status: error.response.status.toString() }, // Chuyển status sang string
            query: { message: error.response?.data?.message ?? 'Lỗi không xác định từ API' },
            state: {
              from: router.currentRoute.fullPath,
            },
          })
          break

        case 403:
          // Forbidden
          toastr.error('Bạn không có quyền truy cập vào tài nguyên này.')
          router.push({
            name: 'Error',
            params: { status: error.response.status.toString() }, // Chuyển status sang string
            query: {
              message:
                error.response?.data?.message ?? 'Bạn không có quyền truy cập vào tài nguyên này',
            },
          })
          break

        case 404:
          // Not Found
          toastr.error('Không tìm thấy tài nguyên.')
          router.push({
            name: 'Error',
            params: { status: error.response.status.toString() }, // Chuyển status sang string
            query: { message: error.response?.data?.message ?? 'Không tìm thấy tài nguyên' },
          })
          break

        case 500:
          // Internal Server Error
          toastr.error('Đã xảy ra lỗi máy chủ. Vui lòng thử lại sau.')
          router.push({
            name: 'Error',
            params: { status: error.response.status.toString() }, // Chuyển status sang string
            query: {
              message:
                error.response?.data?.message ?? 'Đã xảy ra lỗi máy chủ. Vui lòng thử lại sau',
            },
          })
          break

        default:
          // Các lỗi khác
          toastr.error('Đã xảy ra lỗi.')
          router.push({
            name: 'Error',
            params: { status: error.response.status.toString() }, // Chuyển status sang string
            query: { message: error.response?.data?.message ?? 'Đã xảy ra lỗi' },
          })
          break
      }
      // Ném lỗi để các promise khác có thể bắt được
      throw new Error(error.response?.data?.message ?? 'Lỗi không xác định từ API.')
    }
    throw error
  },
)

// Hàm xử lý response API
const handleResponse = async (callback) => {
  try {
    const result = await callback()
    return new ResponseAPI(result)
  } catch (error) {
    return new ResponseAPI(null, false, error.message)
  }
}

// Hàm GET
async function getFromApi(url, config = ConfigsRequest.getSkipAuthConfig()) {
  return handleResponse(() =>
    axiosClient.get(url, { ...config, responseType: config.responseType || 'json' }),
  )
}

// Hàm POST
async function postToApi(url, data, config = ConfigsRequest.getSkipAuthConfig()) {
  return handleResponse(() => axiosClient.post(url, data, config))
}

// Hàm PUT
async function putToApi(url, data, config = ConfigsRequest.getSkipAuthConfig()) {
  return handleResponse(() => axiosClient.put(url, data, config))
}

// Hàm PATCH
async function patchToApi(url, data, config = ConfigsRequest.getSkipAuthConfig()) {
  return handleResponse(() => axiosClient.patch(url, data, config))
}

// Hàm DELETE
async function deleteFromApi(url, config = ConfigsRequest.getSkipAuthConfig()) {
  return handleResponse(() => axiosClient.delete(url, config))
}

export { getFromApi, postToApi, putToApi, patchToApi, deleteFromApi }
