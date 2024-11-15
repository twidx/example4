<template>
  <h1>帳號管理</h1>
  <div class="card mb-3">
    <div class="card-body">
      <form class="row g-3" @submit.prevent="fun.doQuery(1)">
        <div class="col-auto">
          <label for="fAccount" class="visually-hidden">帳號</label>
          <input type="text" class="form-control" id="fAccount" placeholder="帳號" v-model.trim="queryData.account" />
        </div>
        <div class="col-auto">
          <button type="submit" class="btn btn-primary">查詢</button>
        </div>
      </form>
    </div>
  </div>
  <div class="d-flex gap-2">
    <button type="button" class="btn btn-warning" @click.prevent="fun.doNew" :disabled="vm.isEdit">新增</button>
    <button type="button" class="btn btn-success" @click.prevent="fun.doExcel" :disabled="vm.isEdit">匯出Excel</button>
  </div>
  <table class="table table-striped table-hover">
    <thead>
      <tr>
        <th style="width: 160px">動作</th>
        <th style="width: 200px">帳號</th>
        <th style="width: 200px">姓名</th>
        <th style="width: 200px">
          <template v-if="vm.isEdit">密碼</template>
        </th>
        <th></th>
      </tr>
    </thead>
    <tbody>
      <tr v-for="(item, idx) in vm.data">
        <template v-if="item.isEdit">
          <td>
            <div class="d-flex gap-2">
              <button type="button" class="btn btn-success" @click.prevent="fun.doSave">存檔</button>
              <button type="button" class="btn btn-secondary" @click.prevent="fun.doCancel" autocomplete="off">取消</button>
            </div>
          </td>
          <td class="align-middle">
            <input type="text" class="form-control" v-model="vm.item.accountNo" autocomplete="off" v-if="item.isNew" />
            <span v-else>{{ item.accountNo }}</span>
          </td>
          <td>
            <input type="text" class="form-control" v-model="vm.item.name" />
          </td>
          <td>
            <input type="password" class="form-control" v-model="vm.item.password" autocomplete="off" />
          </td>
          <th></th>
        </template>
        <template v-else>
          <td>
            <div class="d-flex gap-2">
              <button type="button" class="btn btn-primary" @click.prevent="fun.doEdit(item, idx)" :disabled="vm.isEdit">修改</button>
              <button type="button" class="btn btn-danger" @click.prevent="fun.doRemove(item)" :disabled="vm.isEdit">刪除</button>
            </div>
          </td>
          <td class="align-middle">{{ item.accountNo }}</td>
          <td class="align-middle">{{ item.name }}</td>
          <td></td>
          <th></th>
        </template>
      </tr>
    </tbody>
  </table>

  <nav aria-label="Page navigation example">
    <ul class="pagination">
      <li class="page-item">
        <a class="page-link" href="#" aria-label="Previous">
          <span aria-hidden="true">&laquo;</span>
        </a>
      </li>
      <li class="page-item" v-for="p in pageInfo.size">
        <a class="page-link" href="#" :class="{ active: p == queryData.page }" @click.prevent="fun.doQuery(p)">
          {{ p }}
        </a>
      </li>
      <li class="page-item">
        <a class="page-link" href="#" aria-label="Next">
          <span aria-hidden="true">&raquo;</span>
        </a>
      </li>
    </ul>
  </nav>
</template>

<script setup lang="ts">
import { ref } from "vue";

const vm = ref({
  data: [] as Array<any>,
  item: {} as any,
  idx: -1,
  isEdit: false
});

const token = localStorage.getItem("example1_token");

const headers = {
  "Content-Type": "application/json",
  Accept: "application/json",
  Authorization: `Bearer ${token}`
} as any;

interface Query {
  account?: string;
  page: number;
}

interface Page {
  total: number;
  size: number;
}

const queryData = ref<Query>({
  account: "",
  page: 1
});

const pageInfo = ref<Page>({
  total: 0,
  size: 1
});

const fun = {
  /** 查詢 */
  doQuery: (p: number = 1) => {
    queryData.value.page = p;

    fetch("/api/account/query", {
      method: "POST",
      headers: headers,
      body: JSON.stringify(queryData.value)
    })
      .then((res) => {
        return res.json();
      })
      .then((res) => {
        if (res.success) {
          pageInfo.value.total = res.total;
          pageInfo.value.size = res.total > 0 ? Math.ceil(res.total / 5) : 1;

          vm.value.data = res.results;
        } else {
          alert(res.message); // 錯誤訊息
        }
      });
  },
  /** 新增 */
  doNew: () => {
    vm.value.isEdit = true;
    vm.value.idx = 0;
    vm.value.data.unshift({
      isNew: true,
      isEdit: true,
      accountNo: "",
      name: "",
      password: ""
    });
    vm.value.item = vm.value.data[0];
  },
  /** 修改 */
  doEdit: (item: any, idx: number) => {
    item.isEdit = true;
    vm.value.item = JSON.parse(JSON.stringify(item));
    vm.value.idx = idx;
    vm.value.isEdit = true;
  },
  /** 刪除 */
  doRemove: (item: any) => {
    if (confirm("確定要刪除嗎？")) {
      fetch("/api/account/remove", {
        method: "POST",
        headers: headers,
        body: JSON.stringify({ accountNo: item.accountNo })
      })
        .then((res) => {
          return res.json();
        })
        .then((res) => {
          if (res.success) {
            fun.doQuery();
          } else {
            alert(res.message); // 錯誤訊息
          }
        });
    }
  },
  /** 存檔 */
  doSave: () => {
    if (confirm("確定要存檔嗎？")) {
      const item = vm.value.item;

      fetch(`/api/account/${item.isNew ? "new" : "save"}`, {
        method: "POST",
        headers: headers,
        body: JSON.stringify({ item: item })
      })
        .then((res) => {
          return res.json();
        })
        .then((res) => {
          if (res.success) {
            fun.doQuery();
            vm.value.isEdit = false;
          } else {
            alert(res.message); // 錯誤訊息
          }
        });
    }
  },
  /** 取消 */
  doCancel: () => {
    const item = vm.value.item;
    if (item.isNew) {
      vm.value.data.splice(vm.value.idx, 1);
    } else {
      vm.value.data[vm.value.idx].isEdit = false;
    }
    vm.value.isEdit = false;
  },
  /** 匯出Excel */
  doExcel: () => {
    fetch("/api/account/excel", {
      method: "POST",
      headers: headers,
      body: JSON.stringify(queryData.value)
    })
      .then((res) => {
        return res.blob();
      })
      .then((blob) => {
        console.log(blob);
        const url = window.URL.createObjectURL(blob);
        const fileLink = document.createElement("a");

        fileLink.href = url;
        fileLink.download = "excel.xlsx"; // download filename
        document.body.appendChild(fileLink); // append file link to download
        fileLink.click();
        fileLink.remove(); // remove file link after click
      });
  }
};

fun.doQuery();
</script>
