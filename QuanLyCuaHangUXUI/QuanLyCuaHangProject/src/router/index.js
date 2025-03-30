import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/client/Home.vue'
import About from '../views/client/About.vue'
import Menu from '../views/client/FoodMenu.vue'
import Chef from '../views/client/Chef.vue'
import dashboard from '../views/admin/Index.vue'
import ClientLayout from '../views/layout/ClientLayout.vue'
import AdminLayout from '../views/layout/AdminLayout.vue'
import Login from '../views/accounts/Login.vue'
import LoginStaff from '../views/accounts/LoginStaff.vue'
import Register from '../views/accounts/Register.vue'
import ForgotPassword from '../views/accounts/ForgotPassword.vue'
import Error from '../views/error/Error.vue'
import DetailProduct from '@/views/client/DetailProduct.vue'
import OrderClient from '@/views/client/OrderClient.vue'
import ProductIndex from '../views/admin/Product/Index.vue'
import ForgotPasswordStaff from '../views/accounts/ForgotPasswordStaff.vue'
import GoogleLoginSuccess from '../views/accounts/GoogleLoginSuccess.vue'
import CustomerIndex from '../views/Customer/Index.vue'
import ComboIndex from '../views/admin/Combo/Index.vue'
import BillIndex from '@/views/admin/Bill/BillIndex.vue'
const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: ClientLayout,
      children: [
        { path: '', name: 'Home', component: Home },
        { path: '/about', name: 'About', component: About },
        { path: '/menu', name: 'Menu', component: Menu },
        { path: '/chefs', name: 'Chefs', component: Chef },
        { path: '/detail', name: 'DetailProduct', component: DetailProduct },
        { path: '/client-order', name: 'ClientOrder', component: OrderClient },
      ],
    },
    {
      path: '/admin',
      component: AdminLayout,
      children:
        [
          { path: '', component: dashboard },
          { path: '/admin/Product', component: ProductIndex },
          { path: '/admin/Bill', component: BillIndex },
          { path: 'customer', name: 'CustomerIndex', component: CustomerIndex },
          { path: 'combo', component: ComboIndex },
        ]
    },
    {
      path: '/Login',
      name: 'Login',
      component: Login,
    },
    {
      path: '/LoginStaff', name: 'LoginStaff', component: LoginStaff
    },
    {
      path: '/Register', name: 'Register', component: Register
    },
    {
      path: '/ForgotPassword',
      name: 'ForgotPassword',
      component: ForgotPassword,
    },
    {
      path: '/ForgotPasswordStaff', name: 'ForgotPasswordStaff', component: ForgotPasswordStaff
    },
    {
      path: '/Error', name: 'Error', component: Error
    },
    {
      path: '/GoogleLoginSuccess',
      name: 'GoogleLoginSuccess',
      component: GoogleLoginSuccess,
    },
  ],
})

export default router
