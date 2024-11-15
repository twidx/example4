import { defineStore } from "pinia";

export const useSystemStore = defineStore("system", {
  state: () => {
    return {
      isLogin: false,
      user: {} as any,
    };
  },
  actions: {
    doLogin(user: any) {
      this.user = user;
      this.isLogin = true;
    },
    doLogout() {
      this.user = {};
      this.isLogin = false;
    }
  },
});
