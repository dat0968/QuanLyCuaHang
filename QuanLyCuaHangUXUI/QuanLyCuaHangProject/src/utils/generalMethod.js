export function getImageUrl(imageName, pathFolder = '/') {
  const baseUrl = 'https://api.jollibeefood.site'
  // Kiểm tra xem imageName có phải là một URL đầy đủ hay không
  const isUrl = /^https?:\/\//i.test(imageName)

  if (isUrl) {
    // Nếu là URL, chỉ cần trả về imageName
    return imageName
  } else {
    // Nếu chỉ là tên hình ảnh, ghép với address
    return `${baseUrl}${pathFolder}${imageName}`
  }
}
