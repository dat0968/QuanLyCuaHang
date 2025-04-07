import { createRouter, createWebHistory } from 'vue-router'
import Home from '../views/client/Home.vue'
import About from '../views/client/About.vue'
import Menu from '../views/client/FoodMenu.vue'
import Chef from '../views/client/Chef.vue'
import dashboard from '../views/admin/Index.vue'
import Staff from '../views/admin/Staff.vue'
import ClientLayout from '../views/layout/ClientLayout.vue'
import AdminLayout from '../views/layout/AdminLayout.vue'
import Login from '../views/accounts/Login.vue'
import LoginStaff from '../views/accounts/LoginStaff.vue'
import Register from '../views/accounts/Register.vue'
import ForgotPassword from '../views/accounts/ForgotPassword.vue'
import Error from '../views/error/Error.vue'
import ProductIndex from '../views/admin/Product/Index.vue'
import Coupon from '../views/admin/Coupon/Index.vue'
import ForgotPasswordStaff from '../views/accounts/ForgotPasswordStaff.vue'
import GoogleLoginSuccess from '../views/accounts/GoogleLoginSuccess.vue'
import CustomerIndex from '../views/Customer/Index.vue'
import ComboIndex from '../views/admin/Combo/Index.vue'
import BillIndex from '@/views/admin/Bill/BillIndex.vue'
import Checkout from '@/views/client/Checkout.vue'
import DetailProduct from '@/views/client/DetailProduct.vue'
import DetailCombo from '@/views/client/DetailCombo.vue'
import Cart from '@/views/client/Cart.vue'
import TableIndex from '@/views/admin/Table/TableIndex.vue'
import Profile from '../views/Profile/Profile.vue';
const routes = [
  {
    path: '/',
    component: ClientLayout,
    children: [
      {
        path: '',
        name: 'Home',
        component: Home,
      },
      {
        path: 'about',
        name: 'About',
        component: About,
      },
      {
        path: 'menu',
        name: 'Menu',
        component: Menu,
      },
      {
        path: 'chef',
        name: 'Chef',
        component: Chef,
      },
      {
        path: 'product/:id',
        name: 'DetailProduct',
        component: DetailProduct,
      },
      {
        path: 'combo/:id',
        name: 'DetailCombo',
        component: DetailCombo,
      },
      {
        path: 'cart',
        name: 'Cart',
        component: Cart,
      },
      {
        path: 'checkout',
        name: 'Checkout',
        component: Checkout,
      },
      { path: '/profile', name: 'Profile', component: Profile },
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
        { path: '/admin/Coupon', name: 'Coupon', component: Coupon },
        { path: '/admin/Table', component: TableIndex },
        { path: 'staff', name: 'Staff', component: Staff },
      ]
  },
  {
    path: '/Login', name: 'Login', component: Login
  },
  {
    path: '/LoginStaff', name: 'LoginStaff', component: LoginStaff
  },
  {
    path: '/Register', name: 'Register', component: Register
  },
  {
    path: '/ForgotPassword', name: 'ForgotPassword', component: ForgotPassword
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
]

const router = createRouter({
  history: createWebHistory(),
  routes,
})

export default router
