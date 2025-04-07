class ConfigsRequest {
  static getSkipAuthConfig() {
    return { headers: { skipAuth: true } }
  }

  static takeAuth() {
    return { headers: { skipAuth: false } }
  }
  static formDataRequest() {
    return { headers: { 'Content-Type': 'multipart/form-data' } }
  }
}

export default ConfigsRequest
