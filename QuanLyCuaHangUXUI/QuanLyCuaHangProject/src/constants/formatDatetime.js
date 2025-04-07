// formatDatetime.js
export function formatDate(date) {
  if (!date) return ''
  const d = new Date(date)
  return d.toLocaleDateString('vi-VN', {
    year: 'numeric',
    month: '2-digit',
    day: '2-digit',
  })
}

export function formatTime(inputTime, isGetMilli = false) {
  if (inputTime == null || inputTime == undefined) {
    return
  }
  const [time, milliseconds] = inputTime.split('.') // Tách phần giây và phần dư
  const [hours, minutes, seconds] = time.split(':') // Tách giờ, phút và giây

  if (!isGetMilli) {
    return `${hours}:${minutes}:${seconds}` // Chỉ trả về giờ, phút, giây
  }

  const formattedMilliseconds = milliseconds ? milliseconds.substring(0, 3) : '000'
  return `${hours}:${minutes}:${seconds}.${formattedMilliseconds}`
}
