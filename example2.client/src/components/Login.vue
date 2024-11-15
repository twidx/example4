<template>
  <div class="card" style="width: 20rem; margin: 5rem auto">
    <div class="card-body">
      <form method="post" @submit.prevent="doLogin">
        <div class="mb-3">
          <label for="fAccountNo" class="form-label">帳號</label>
          <input
            type="text"
            class="form-control"
            id="fAccountNo"
            placeholder="請輸入帳號"
            v-model.trim="form.accountNo"
          />
        </div>
        <div class="mb-3">
          <label for="fPassword" class="form-label">密碼</label>
          <input
            type="password"
            class="form-control"
            id="fPassword"
            placeholder="請輸入密碼"
            v-model.trim="form.password"
          />
        </div>
        <button type="submit" class="btn btn-primary">登入</button>
      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from "vue";
import { useSystemStore } from "@/store/system";

const sys = useSystemStore();

const form = ref({
  accountNo: "",
  password: "",
});

/** 登入 */
const doLogin = () => {
  const headers = {
    "Content-Type": "application/json",
    Accept: "application/json",
  } as any;

  fetch("/api/auth/login", {
    method: "POST",
    headers: headers,
    body: JSON.stringify(form.value),
  })
    .then((res) => {
      return res.json();
    })
    .then((res) => {
      if (res.success) {
        // 登入成功
        sys.doLogin(res.user);
        localStorage.setItem("example1_token", res.jwtToken);
      } else {
        alert(res.message); // 錯誤訊息
      }
    });
};
</script>
