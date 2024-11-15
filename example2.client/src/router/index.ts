import { createWebHistory, createRouter } from "vue-router";

const routes = [
  {
    path: "/",
    name: "Home",
    component: () => import("@/pages/index.vue"),
  },
  {
    path: "/account",
    name: "Account",
    meta: { auth: true },
    component: () => import("@/pages/account/index.vue"),
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
