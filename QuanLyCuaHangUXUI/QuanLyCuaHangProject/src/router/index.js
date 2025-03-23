import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/client/Home.vue'
import About from '../views/client/About.vue'
import Menu from '../views/client/FoodMenu.vue'
import Chef from '../views/client/Chef.vue'
import dashboard from '../views/admin/Index.vue'
import ClientLayout from '../views/layout/ClientLayout.vue'
import AdminLayout from '../views/layout/AdminLayout.vue'
import Login from '../views/accounts/Login.vue'
import Register from '../views/accounts/Register.vue'
import ForgotPassword from '../views/accounts/ForgotPassword.vue'
import Error from '../views/error/Error.vue'
import ProductIndex from '../views/admin/Product/Index.vue'
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: ClientLayout,
      children:
        [
          { path: '', name: 'Home', component: Home },
          { path: '/about', name: 'About', component: About },
          { path: '/menu', name: 'Menu', component: Menu },
          { path: '/chefs', name: 'Chefs', component: Chef },
        ]
    },
    {
      path: '/admin',
      component: AdminLayout,
      children:
        [
          { path: '', component: dashboard },
          { path: '/admin/Product', component: ProductIndex }
        ]
    },
    {
      path: '/Login', name: 'Login', component: Login
    },
    {
      path: '/Register', name: 'Register', component: Register
    },
    {
      path: '/ForgotPassword', name: 'ForgotPassword', component: ForgotPassword
    },
    {
      path: '/Error', name: 'Error', component: Error
    }
  ],
})

export default router
