export default class UserStatisticsData {
  constructor(users = null) {
    this.users = users // users là một mảng các đối tượng UserStatistics
  }
}

export class UserStatistics {
  constructor(userType = null, active = 0, inactive = 0) {
    this.userType = userType
    this.active = active
    this.inactive = inactive
  }
}
