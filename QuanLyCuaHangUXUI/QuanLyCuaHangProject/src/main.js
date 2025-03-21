//import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import replaceBrokenImages from './utils/autoReplaceImages'

import App from './App.vue'
import router from './router'

const app = createApp(App)

app.use(createPinia())
app.use(router)

app.mount('#app')

//Tự động thay ảnh lỗi
replaceBrokenImages()
