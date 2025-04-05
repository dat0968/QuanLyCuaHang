import axios from 'axios'
import { jwtDecode } from 'jwt-decode' // Thư viện cần: npm install jwt-decode
import ResponseAPI from '@/models/ResponseAPI'
import ConfigsRequest from '@/models/ConfigsRequest'

import toastr from 'toastr'
import 'toastr/build/toastr.min.css'
import router from '@/router/index'

// Base Axios Client
const axiosClient = axios.create({
  baseURL: 'https://localhost:7139/api', // Thay bằng base URL của API bạn
  timeout: 500000, // Giới hạn timeout (ms)
  headers: {
    'Content-Type': 'application/json',
  },
})

// Kiểm tra token hết hạn
function isTokenExpired(token) {
  try {
    const decoded = jwtDecode(token)
    return decoded.exp * 1000 < Date.now() // Thời gian hết hạn (exp) là milli-seconds
  } catch {
    return true // Nếu không decode được token, xem như nó đã hết hạn
  }
}

// Hàm refresh token
async function refreshAccessToken() {
  const refreshToken = localStorage.getItem('refreshToken')

  if (!refreshToken) {
    throw new Error('Refresh Token không tồn tại!')
  }

  try {
    const response = await axios.post(axiosClient.baseURL + '/TruyCap/RefreshToken', {
      RefreshToken: refreshToken,
    })

    if (response.status === 200 && response.data) {
      const { accessToken, refreshToken: newRefreshToken } = response.data

      if (accessToken && newRefreshToken) {
        // Lưu lại token mới
        localStorage.setItem('accessToken', accessToken)
        localStorage.setItem('refreshToken', newRefreshToken)

        return accessToken
      } else {
        throw new Error('Token API trả về không hợp lệ.')
      }
    }
  } catch (error) {
    console.error('Không thể refresh access token:', error.message || error)
    toastr.warning('Phiên truy cập đã hết, vui lòng đăng nhập lại.')

    // Xóa tokens và chuyển hướng về trang đăng nhập
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')

    // ? Lưu lịch sử trang web không có quyền truy cập.
    router.push({
      path: '/login',
      state: {
        from: router.currentRoute.fullPath,
      },
    })

    throw new Error('Refresh Token đã hết hạn hoặc không khả dụng.')
  }
}

// Middleware (interceptors) thêm Authorization header
axiosClient.interceptors.request.use(
  async (config) => {
    const requiresAuth = !config.headers.skipAuth
    if (!requiresAuth) return config

    const accessToken = localStorage.getItem('accessToken')

    if (accessToken) {
      if (isTokenExpired(accessToken)) {
        try {
          const newToken = await refreshAccessToken()
          config.headers.Authorization = `Bearer ${newToken}`
        } catch (error) {
          console.error('Lỗi khi refresh token:', error.message || error)
          toastr.info('Phiên truy cập đã kết thúc.\nVui lòng truy cập lại.')

          // ? Lưu lịch sử trang web không có quyền truy cập.
          router.push({
            path: '/login',
            state: {
              from: router.currentRoute.fullPath,
            },
          })
          throw error
        }
      } else {
        config.headers.Authorization = `Bearer ${accessToken}`
      }
    }

    return config
  },
  (error) => Promise.reject(error),
)

// Xử lý phản hồi với các lỗi
axiosClient.interceptors.response.use(
  (response) => response.data,
  (error) => {
    if (error.response) {
      console.error(`API Error: ${error.response.status}`, error.response.data)
      if (error.response.status === 401) {
        toastr.warning(
          'Có 1 số chức năng không hoạt động khi bạn chưa đăng nhập, vui lòng đăng nhập để tiếp tục.',
        )
        router.push('/login')
      }
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
