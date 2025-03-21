const replaceBrokenImages = () => {
  const defaultImage = './src/assets/default/default.jpeg'
  const checkAndReplace = (img) => {
    if (!img.dataset.checked) {
      img.dataset.checked = 'true' // Đánh dấu đã kiểm tra
      img.onerror = () => {
        img.src = defaultImage
      }
    }
  }

  // Kiểm tra ảnh hiện tại trên trang
  document.querySelectorAll('img').forEach(checkAndReplace)

  // Theo dõi sự thay đổi trên DOM
  const observer = new MutationObserver((mutations) => {
    mutations.forEach((mutation) => {
      mutation.addedNodes.forEach((node) => {
        if (node.nodeType === 1) {
          if (node.tagName === 'IMG') {
            checkAndReplace(node)
          } else {
            node.querySelectorAll?.('img').forEach(checkAndReplace)
          }
        }
      })
    })
  })

  observer.observe(document.body, {
    childList: true,
    subtree: true,
  })
}

export default replaceBrokenImages
