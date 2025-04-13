export function formatCurrency(value) {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value)
}

// Phương thức chuyển đổi số thành chữ
export function convertNumberToWords(number) {
  const units = ['', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín']
  const teens = [
    'mười',
    'mười một',
    'mười hai',
    'mười ba',
    'mười bốn',
    'mười lăm',
    'mười sáu',
    'mười bảy',
    'mười tám',
    'mười chín',
  ]
  const tens = [
    '',
    '',
    'hai mươi',
    'ba mươi',
    'bốn mươi',
    'năm mươi',
    'sáu mươi',
    'bảy mươi',
    'tám mươi',
    'chín mươi',
  ]
  const magnitudes = ['', 'nghìn', 'triệu', 'tỷ', 'nghìn tỷ', 'triệu tỷ']

  if (number === 0) {
    return 'không đồng'
  }

  if (number < 0) {
    return 'âm ' + convertNumberToWords(-number)
  }

  let words = ''
  let magnitudeIndex = 0

  while (number > 0) {
    const chunk = number % 1000
    if (chunk > 0) {
      words = chunkToWords(chunk) + ' ' + magnitudes[magnitudeIndex] + ' ' + words
    }
    number = Math.floor(number / 1000)
    magnitudeIndex++
  }

  return words.trim() + ' đồng'
}

function chunkToWords(chunk) {
  const units = ['', 'một', 'hai', 'ba', 'bốn', 'năm', 'sáu', 'bảy', 'tám', 'chín']
  const teens = [
    'mười',
    'mười một',
    'mười hai',
    'mười ba',
    'mười bốn',
    'mười lăm',
    'mười sáu',
    'mười bảy',
    'mười tám',
    'mười chín',
  ]
  const tens = [
    '',
    '',
    'hai mươi',
    'ba mươi',
    'bốn mươi',
    'năm mươi',
    'sáu mươi',
    'bảy mươi',
    'tám mươi',
    'chín mươi',
  ]

  let words = ''

  const hundreds = Math.floor(chunk / 100)
  if (hundreds > 0) {
    words += units[hundreds] + ' trăm '
  }

  const remainder = chunk % 100
  if (remainder < 10) {
    words += units[remainder]
  } else if (remainder < 20) {
    words += teens[remainder - 10]
  } else {
    const tensIndex = Math.floor(remainder / 10)
    const unitsIndex = remainder % 10
    words += tens[tensIndex]
    if (unitsIndex > 0) {
      words += ' ' + units[unitsIndex]
    }
  }

  return words.trim()
}
