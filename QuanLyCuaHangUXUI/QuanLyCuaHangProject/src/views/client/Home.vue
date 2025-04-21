<script setup>
import DeliciousFoodMenu from '../../components/DeliciousFoodMenu.vue'
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import Cookies from 'js-cookie'
import { GetApiUrl } from '@constants/api'

const router = useRouter()
const isChatOpen = ref(false)
const userInput = ref('')
const messages = ref([])
const isTyping = ref(false)
const previousAnswer = ref('')
const getApiUrl = GetApiUrl()

// Khôi phục tin nhắn từ localStorage khi tải trang
onMounted(() => {
  const savedMessages = localStorage.getItem('chatMessages')
  if (savedMessages) {
    messages.value = JSON.parse(savedMessages)
  } else {
    // Nếu không có tin nhắn nào trong localStorage, hiển thị tin nhắn chào mặc định
    messages.value = [
      {
        type: 'bot',
        text: 'Chào bạn! Tôi có thể giúp gì cho bạn hôm nay? Ví dụ: "Danh sách sản phẩm", "Tư vấn món ăn", "Thêm [tên món] vào giỏ", "Thanh toán".',
      },
    ]
  }
})
// Mở/đóng hộp chat
const toggleChatBox = () => {
  isChatOpen.value = !isChatOpen.value
}

// Hàm điều hướng đến trang chi tiết sản phẩm
const navigateToProduct = (maSp) => {
  //router.push(`/detail/product/${maSp}`)
  router.push(`/product/${maSp}`)
}

// Hàm điều hướng đến trang chi tiết combo
const navigateToCombo = (maCombo) => {
  //router.push(`/detail/combo/${maCombo}`)
  router.push(`/combo/${maCombo}`)
}

// Xóa lịch sử tin nhắn
const clearChatHistory = () => {
  messages.value = [
    {
      type: 'bot',
      text: 'Lịch sử tin nhắn đã được xóa. Tôi có thể giúp gì cho bạn? Ví dụ: "Danh sách sản phẩm", "Tư vấn món ăn", "Thêm [tên món] vào giỏ", "Thanh toán".',
    },
  ]
  localStorage.setItem('chatMessages', JSON.stringify(messages.value))
  scrollToBottom()
}

// Gửi tin nhắn
const sendMessage = async (confirmation = null) => {
  // Chỉ yêu cầu userInput nếu không phải đang xác nhận Yes/No
  if (!userInput.value && confirmation === null) return

  if (userInput.value) {
    messages.value.push({ type: 'user', text: userInput.value })
  }

  isTyping.value = true
  scrollToBottom()

  try {
    const token = Cookies.get('accessToken')
    const payload = {
      userInput: userInput.value || '', // Gửi chuỗi rỗng nếu không có userInput
      previousAnswer: previousAnswer.value || null,
      confirmation: confirmation === true ? true : confirmation === false ? false : null, // Đảm bảo giá trị là true, false, hoặc null
    }

    const headers = {
      'Content-Type': 'application/json',
    }
    if (
      token &&
      (payload.userInput.toLowerCase().includes('thêm') ||
        payload.userInput.toLowerCase().includes('thanh toán') ||
        confirmation !== null)
    ) {
      headers['Authorization'] = `Bearer ${token}`
    }

    const response = await fetch(getApiUrl + '/api/Home/TraLoi', {
      method: 'POST',
      headers,
      body: JSON.stringify(payload),
    })

    if (!response.ok) {
      if (response.status === 401) {
        messages.value.push({
          type: 'bot',
          text: 'Token hết hạn hoặc không hợp lệ. Vui lòng đăng nhập lại!',
        })
        Cookies.remove('accessToken')
        Cookies.remove('refreshToken')
        router.push({ path: '/login', query: { redirect: router.currentRoute.value.fullPath } })
        // Lưu tin nhắn trước khi chuyển hướng
        localStorage.setItem('chatMessages', JSON.stringify(messages.value))
        return
      }
      const errorData = await response.json()
      throw new Error(
        `Lỗi HTTP! trạng thái: ${response.status} - ${
          errorData.response || 'Không có thông tin lỗi'
        }`
      )
    }

    const data = await response.json()
    isTyping.value = false

    let botMessage = { type: 'bot', text: data.response, hasButtons: false }
    if (data.response.includes('[Yes/No]')) {
      botMessage.text = data.response.replace(' [Yes/No]', '')
      botMessage.hasButtons = true
      botMessage.buttonType = 'yes-no'
    } else if (data.response.includes('Đăng nhập')) {
      botMessage.text = `${
        data.response
      } <br><button class="login-btn" onclick="window.location.href='/login?redirect=${encodeURIComponent(
        router.currentRoute.value.fullPath
      )}'">Đăng nhập ngay</button>`
    }
    messages.value.push(botMessage)
    previousAnswer.value = data.response
    // Lưu tin nhắn vào localStorage sau mỗi lần gửi/thêm tin nhắn
    localStorage.setItem('chatMessages', JSON.stringify(messages.value))

    scrollToBottom()
  } catch (error) {
    isTyping.value = false
    messages.value.push({ type: 'bot', text: `Lỗi: ${error.message}` })
    // Lưu tin nhắn vào localStorage ngay cả khi có lỗi
    localStorage.setItem('chatMessages', JSON.stringify(messages.value))
    scrollToBottom()
  }

  userInput.value = ''
}

// Cuộn xuống cuối hộp chat
const chatBody = ref(null)
const scrollToBottom = () => {
  if (chatBody.value) {
    chatBody.value.scrollTop = chatBody.value.scrollHeight
  }
}

// Đăng ký các hàm điều hướng vào window để gọi từ HTML
window.navigateToProduct = navigateToProduct
window.navigateToCombo = navigateToCombo
</script>

<template>
  <div>
    <!-- banner part start-->
    <section class="banner_part">
      <div class="container">
        <div class="row align-items-center">
          <div class="col-lg-6">
            <div class="banner_text">
              <div class="banner_text_iner">
                <h5>Đắt tiền nhưng tốt nhất</h5>
                <h1>Sự ngon lành nhảy vào miệng</h1>
                <p>
                  Cả nhà quây quần, cảm giác như lên thiên đường với từng miếng gà giòn rụm, nhìn
                  thôi đã thấy ngon, nhất là khi chấm sốt. Đĩa gà trên bàn làm mọi người xuýt xoa.
                  Sáng sớm nghe mùi gà rán là chỉ muốn ăn luôn.
                </p>
                <div class="banner_btn">
                  <div class="banner_btn_iner" style="margin-right: 20px">
                    <a href="/menu" class="btn_2">
                      Thực đơn <img src="@/assets/client/img/icon/left_1.svg" alt="" />
                    </a>
                  </div>
                  <a
                    href="https://www.youtube.com/watch?v=pBFQdxA-apI"
                    class="popup-youtube video_popup"
                  >
                    <span><img src="@/assets/client/img/icon/play.svg" alt="" /></span> Xem câu
                    chuyện của chúng tôi
                  </a>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!-- banner part end-->

    <!-- exclusive_item_part start-->
    <section class="exclusive_item_part blog_item_section">
      <div class="container">
        <div class="row">
          <div class="col-xl-5">
            <div class="section_tittle">
              <p>Món ăn phổ biến</p>
              <h2>Các mặt hàng độc quyền của chúng tôi</h2>
            </div>
          </div>
        </div>
        <div class="row">
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="@/assets/client/img/food_item/image1.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>GÀ GIÒN VUI VẺ</h3>
                <p>Trải nghiệm hương vị gà thơm ngon, giòn rụm</p>
                <a href="/combo/3" class="btn_3">
                  Đặt ngay <img src="@/assets/client/img/icon/left_1.svg" alt="" />
                </a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="@/assets/client/img/food_item/image4.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>GÀ GIÒN HƯƠNG VỊ CAY</h3>
                <p>2 Gà Sốt Cay + 1 Khoai tây chiên vừa + 1 Nước ngọt</p>
                <a href="/combo/2" class="btn_3">
                  Đặt ngay <img src="@/assets/client/img/icon/left_2.svg" alt="" />
                </a>
              </div>
            </div>
          </div>
          <div class="col-sm-6 col-lg-4">
            <div class="single_blog_item">
              <div class="single_blog_img">
                <img src="@/assets/client/img/food_item/image3.png" alt="" />
              </div>
              <div class="single_blog_text">
                <h3>COMBO GÀ GIÒN</h3>
                <p>Sự kết hợp giữa mì và gà giòn</p>
                <!-- <a href="http://localhost:5173/combo/1" class="btn_3"> -->
                <a href="/combo/1" class="btn_3">
                  Đặt ngay <img src="@/assets/client/img/icon/left_2.svg" alt="" />
                </a>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!-- exclusive_item_part end-->

    <!-- about part start-->
    <section class="about_part">
      <div class="container-fluid">
        <div class="row align-items-center">
          <div class="col-sm-4 col-lg-5 offset-lg-1">
            <div class="about_img">
              <img src="@/assets/client/img/food_item/image5.png" alt="" />
            </div>
          </div>
          <div class="col-sm-8 col-lg-4">
            <div class="about_text">
              <h5></h5>
              <h2>GÀ GIÒN RỘP RỘP</h2>
              <h4>Đáp ứng cơn thèm những niềm vui đơn giản</h4>
              <p>
                Miếng gà giòn tan, thơm lừng làm ai cũng mê. Mang đến cảm giác vui vẻ, cả nhà cùng
                thưởng thức, chẳng gì sánh bằng khi cắn vào lớp vỏ ngon tuyệt, chấm thêm sốt nữa thì
                đúng là đỉnh cao mỗi bữa ăn.
              </p>
              <a href="http://localhost:5173/product/1001" class="btn_3">
                Đặt ngay <img src="@/assets/client/img/icon/left_2.svg" alt="" />
              </a>
            </div>
          </div>
        </div>
      </div>
    </section>
    <!-- about part end-->

    <!-- food_menu start-->
    <DeliciousFoodMenu />
    <!-- food_menu part end-->

    <!-- Chatbot -->
    <div
      style="
        position: fixed;
        bottom: 20px;
        right: 20px;
        width: 700px;
        z-index: 1000;
        display: flex;
        flex-direction: column;
        align-items: flex-end;
      "
    >
      <div
        id="tawk-bubble-container"
        role="button"
        tabindex="0"
        class="tawk-bubble-container"
        @click="toggleChatBox"
        style="cursor: pointer; margin-bottom: 10px"
      >
        <div class="tawk-icon-right">
          <i
            v-show="isChatOpen"
            role="button"
            tabindex="0"
            data-text="Đóng"
            aria-label="Đóng"
            class="tawk-icon tawk-icon-close tawk-icon-small"
          ></i>
          <img
            src="/src/assets/client/img/Red and Yellow Illustrative Fried Chicken Logo.png"
            alt="Thu hút chú ý đến tính năng trò chuyện"
            style="max-width: 50px; height: 50px; border-radius: 50%"
          />
        </div>
      </div>
      <div
        v-show="isChatOpen"
        id="chat-box"
        class="chat-box"
        style="
          border: 1px solid #ccc;
          border-radius: 10px;
          background-color: #f9f9f9;
          box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
          width: 700px;
        "
      >
        <div
          class="chat-header"
          style="
            padding: 10px;
            background-color: #007bff;
            color: white;
            font-weight: bold;
            border-top-left-radius: 10px;
            border-top-right-radius: 10px;
            display: flex;
            justify-content: space-between;
            align-items: center;
          "
        >
          <span>Hộp Thoại Hỗ Trợ</span>
          <div>
            <button
              @click="clearChatHistory"
              style="
                margin-right: 10px;
                padding: 5px 10px;
                border-radius: 5px;
                background-color: #dc3545;
                color: white;
                border: none;
                cursor: pointer;
              "
            >
              Xóa lịch sử
            </button>
            <i
              role="button"
              tabindex="0"
              @click="toggleChatBox"
              class="tawk-icon tawk-icon-close tawk-icon-small"
              style="cursor: pointer"
              >X</i
            >
          </div>
        </div>
        <div
          ref="chatBody"
          class="chat-body"
          style="padding: 10px; max-height: 300px; overflow-y: auto; width: 700px"
        >
          <div
            v-for="(message, index) in messages"
            :key="index"
            :class="['chat-message', message.type === 'user' ? 'user-message' : 'bot-message']"
            :style="
              message.type === 'user'
                ? 'text-align: right; margin-bottom: 10px; color: #007bff;'
                : 'text-align: left; margin-bottom: 10px;'
            "
          >
            <span v-html="message.text"></span>
            <div v-if="message.hasButtons" style="margin-top: 5px">
              <button class="yes-btn" @click="sendMessage(true)">Yes</button>
              <button class="no-btn" @click="sendMessage(false)">No</button>
            </div>
          </div>
          <div v-show="isTyping" class="chat-message typing-indicator" style="text-align: left">
            <span>...</span>
          </div>
        </div>
        <div class="chat-input-container" style="display: flex; padding: 10px; width: 700px">
          <input
            v-model="userInput"
            @keypress.enter="sendMessage"
            type="text"
            placeholder="Nhập câu hỏi..."
            class="chat-input"
            style="flex: 1; padding: 8px; border-radius: 5px; border: 1px solid #ccc"
          />
          <button
            @click="sendMessage"
            class="chat-send-button"
            style="
              margin-left: 5px;
              padding: 8px 12px;
              border-radius: 5px;
              background-color: #007bff;
              color: white;
              border: none;
            "
          >
            Gửi
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.chat-body table {
  width: 100%;
  border-collapse: collapse;
  margin-bottom: 10px;
  font-size: 14px;
}

.chat-body th,
.chat-body td {
  border: 1px solid #ddd;
  padding: 8px;
  text-align: center;
}

.chat-body th {
  background-color: #007bff;
  color: white;
  font-weight: bold;
}

.chat-body tr:nth-child(even) {
  background-color: #f2f2f2;
}

.chat-body .btn-primary {
  padding: 5px 10px;
  background-color: #007bff;
  border: none;
  border-radius: 5px;
  cursor: pointer;
  font-size: 12px;
  color: white;
}

.yes-btn {
  margin-right: 5px;
  padding: 5px 10px;
  border-radius: 5px;
  background-color: #28a745;
  color: white;
  border: none;
  cursor: pointer;
}

.no-btn {
  padding: 5px 10px;
  border-radius: 5px;
  background-color: #dc3545;
  color: white;
  border: none;
  cursor: pointer;
}

.login-btn {
  padding: 5px 10px;
  border-radius: 5px;
  background-color: #007bff;
  color: white;
  border: none;
  cursor: pointer;
}

.tawk-icon-close::before {
  content: 'X';
}
</style>
