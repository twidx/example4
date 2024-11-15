<template>
  <nav class="navbar navbar-expand-lg bg-primary" data-bs-theme="dark">
    <div class="container-fluid">
      <RouterLink to="/" class="navbar-brand">範例網站</RouterLink>
      <button
        class="navbar-toggler"
        type="button"
        data-bs-toggle="collapse"
        data-bs-target="#navbarNav"
        aria-controls="navbarNav"
        aria-expanded="false"
        aria-label="Toggle navigation"
      >
        <span class="navbar-toggler-icon"></span>
      </button>
      <div class="collapse navbar-collapse" id="navbarNav">
        <ul class="navbar-nav">
          <li class="nav-item">
            <RouterLink to="/account" class="nav-link active">
              帳號管理
            </RouterLink>
          </li>
          <li class="nav-item">
            <a href="#" @click.prevent="doLogout" class="nav-link">登出</a>
          </li>
        </ul>
      </div>
    </div>
  </nav>
  <div class="container" v-if="vm.isInit">
    <Login v-if="route.meta.auth && !sys.isLogin" />
    <RouterView v-else />
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useRoute } from "vue-router";
import { useSystemStore } from "@/store/system";
import Login from "@/components/Login.vue";

const route = useRoute();
const sys = useSystemStore();

const vm = ref({
  isInit: false,
});

const doToken = () => {
  const token = localStorage.getItem("example1_token");

  if (token) {
    const headers = {
      "Content-Type": "application/json",
      Accept: "application/json",
      Authorization: `Bearer ${token}`,
    } as any;

    fetch("/api/auth/token", {
      method: "POST",
      headers: headers,
      body: JSON.stringify({}),
    })
      .then((res) => {
        return res.json();
      })
      .then((res) => {
        if (res.success) {
          // 登入成功
          sys.doLogin(res.user);
        } else {
          alert(res.message); // 錯誤訊息
        }
        vm.value.isInit = true;
      });
  } else {
    vm.value.isInit = true;
  }
};

/** 登出 */
const doLogout = () => {
  if (confirm("確定要登出？")) {
    sys.doLogout();
    localStorage.removeItem("example1_token");
  }
};

doToken();
</script>
