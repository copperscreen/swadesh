import { createRouter, createWebHashHistory } from 'vue-router'
import LangList from '../components/LangList.vue'

const router = createRouter({
  history: createWebHashHistory(import.meta.env.BASE_URL),
  routes: [
    {
//      path: '/list:langs+',
      path: '/list',
      name: 'list',
      // route level code-splitting
      // this generates a separate chunk (List.[hash].js) for this route
      // which is lazy-loaded when the route is visited.
      //component: () => import('../views/CompiledListView.vue'),
      component: () => import('../components/CompiledList.vue'),
      props: true
      },
      {
          path: '/',
          name: 'home',
          component: LangList
      }
  ]
})

export default router

