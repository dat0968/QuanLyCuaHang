import { jwtDecode } from 'jwt-decode'
import $ from 'jquery'
import toastr from 'toastr'
import Cookies from 'js-cookie' // Import js-cookie
import { GetApiUrl } from '@constants/api'
let getApiUrl = GetApiUrl()
const apiBaseUrl = getApiUrl+'/api'

// Kiểm tra token hết hạn
function isTokenExpired(token) {
  try {
    const decoded = jwtDecode(token)
    return decoded.exp * 1000 < Date.now()
  } catch {
    return true
  }
}

// Hàm refresh token
async function refreshAccessToken() {
  const refreshToken = Cookies.get('refreshToken')

  if (!refreshToken) {
    throw new Error('Refresh Token không tồn tại!')
  }

  try {
    const response = await $.ajax({
      url: `${apiBaseUrl}/TruyCap/RefreshToken`,
      method: 'POST',
      contentType: 'application/json',
      data: JSON.stringify({ RefreshToken: refreshToken }),
    })

    if (response) {
      const { accessToken, refreshToken: newRefreshToken } = response
      Cookies.set('accessToken', accessToken)
      Cookies.set('refreshToken', newRefreshToken)
      return accessToken
    }
  } catch (error) {
    console.error('Không thể refresh access token:', error)
    toastr.warning('Phiên truy cập đã hết, vui lòng truy cập lại.')

    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')

    throw new Error('Refresh Token đã hết hạn')
  }
}

// Hàm xử lý request với jQuery
async function apiRequest(url, method = 'GET', data = null, skipAuth = false) {
  let accessToken = Cookies.get('accessToken')
  let headers = { 'Content-Type': 'application/json' }

  if (!skipAuth && accessToken) {
    if (isTokenExpired(accessToken)) {
      try {
        accessToken = await refreshAccessToken()
      } catch (error) {
        console.error('Lỗi khi refresh token:', error.message)
        throw error
      }
    }
    headers.Authorization = `Bearer ${accessToken}`
  }

  return $.ajax({
    url: `${apiBaseUrl}${url}`,
    method,
    contentType: 'application/json',
    data: data ? JSON.stringify(data) : null,
    headers,
  })
    .done((response) => response)
    .fail((xhr) => {
      console.error(`API Error: ${xhr.status}`, xhr.responseJSON)
      if (xhr.status === 401) {
        toastr.warning(
          'Có 1 số chức năng không hoạt động khi bạn chưa truy cập, vui lòng đăng nhập để sử dụng đầy đủ chức năng của hệ thống.',
        )
      }
      throw new Error(xhr.responseJSON?.message ?? 'Lỗi không xác định')
    })
}

// Các phương thức API tương ứng
function getFromApi(url, skipAuth = true) {
  return apiRequest(url, 'GET', null, skipAuth)
}

function postToApi(url, data, skipAuth = true) {
  return apiRequest(url, 'POST', data, skipAuth)
}

function putToApi(url, data, skipAuth = true) {
  return apiRequest(url, 'PUT', data, skipAuth)
}

function patchToApi(url, data, skipAuth = true) {
  return apiRequest(url, 'PATCH', data, skipAuth)
}

function deleteFromApi(url, skipAuth = true) {
  return apiRequest(url, 'DELETE', null, skipAuth)
}

export { getFromApi, postToApi, putToApi, patchToApi, deleteFromApi }
