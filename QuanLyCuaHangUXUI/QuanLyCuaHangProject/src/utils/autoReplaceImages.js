import defaultImage from '@/assets/default/default.jpeg' // Import ảnh

const replaceBrokenImages = () => {
  const checkAndReplace = (img) => {
    if (!img.dataset.checked) {
      img.dataset.checked = 'true'
      img.onerror = () => {
        img.src = defaultImage // Sử dụng URL đã import
      }
    }
  }

  document.querySelectorAll('img').forEach(checkAndReplace)

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
