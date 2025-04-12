//import './assets/main.css'

import { createApp } from 'vue'
import { createPinia } from 'pinia'

import replaceBrokenImages from './utils/autoReplaceImages'

import App from './App.vue'
import router from './router'
import 'bootstrap/dist/css/bootstrap.min.css'
import 'bootstrap/dist/js/bootstrap.bundle.min.js'
import 'bootstrap-icons/font/bootstrap-icons.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import * as bootstrap from 'bootstrap';
window.bootstrap = bootstrap;
const app = createApp(App)

app.use(createPinia())
app.use(router)
app.config.globalProperties.$bootstrap = bootstrap;
app.mount('#app')

//Tự động thay ảnh lỗi
replaceBrokenImages()
