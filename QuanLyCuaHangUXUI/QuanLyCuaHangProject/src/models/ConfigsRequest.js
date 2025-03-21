class ConfigsRequest {
  static getSkipAuthConfig() {
    return { headers: { skipAuth: true } };
  }
  static takeAuth() {
    return { headers: { skipAuth: false } };
  }
}

export default ConfigsRequest;
