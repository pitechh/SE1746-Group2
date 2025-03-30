<template>
    <q-page class="q-pa-md">
      <div class="row items-center q-mb-md">
        <q-select
          outlined
          dense
          v-model="selectedType"
          :options="formulaTypes"
          label="Loại công thức"
          class="q-mr-md"
          @update:model-value="fetchFormulas"
        />
  
        <q-btn
          color="primary"
          icon="add"
          label="Thêm công thức"
          @click="showDialog = true"
        />
      </div>
  
      <!-- Công thức đang dùng -->
      <q-banner
        class="bg-blue-1 text-blue-10 q-mb-md"
        v-if="activeFormula"
      >
        Công thức đang dùng cho {{ selectedType }}:
        <span class="text-weight-bold">{{ activeFormula }}</span>
      </q-banner>
  
      <q-table
        :rows="formulas"
        :columns="columns"
        row-key="id"
        flat
        bordered
        dense
      >
        <template v-slot:body-cell-isActive="props">
          <q-td :props="props">
            <q-chip
              color="green"
              v-if="props.row.isActive"
              label="Đang dùng"
            />
          </q-td>
        </template>
  
        <template v-slot:body-cell-actions="props">
          <q-td :props="props">
            <q-btn
              size="sm"
              color="positive"
              label="Kích hoạt"
              :disable="props.row.isActive"
              @click="setActive(props.row.id)"
            />
          </q-td>
        </template>
      </q-table>
  
      <q-dialog v-model="showDialog">
        <q-card style="min-width: 400px">
          <q-card-section>
            <div class="text-h6">Thêm công thức</div>
          </q-card-section>
  
          <q-card-section>
            <q-input
              outlined
              v-model="newFormula.expression"
              label="Biểu thức công thức"
              type="text"
              placeholder="VD: baseSalary * 0.05"
            />
          </q-card-section>
  
          <q-card-actions align="right">
            <q-btn flat label="Huỷ" v-close-popup />
            <q-btn color="primary" label="Lưu" @click="createFormula" />
          </q-card-actions>
        </q-card>
      </q-dialog>
    </q-page>
  </template>
  
  <script setup>
  import { ref, onMounted } from 'vue';
  import { api } from 'src/boot/axios';
  import { useQuasar } from 'quasar';
  
  const $q = useQuasar();
  
  const selectedType = ref('PIT');
  const formulaTypes = ['PIT', 'SocialInsurance'];
  
  const formulas = ref([]);
  const activeFormula = ref('');
  const showDialog = ref(false);
  const newFormula = ref({ expression: '' });
  
  const columns = [
    { name: 'id', label: 'ID', field: 'id', align: 'left' },
    { name: 'expression', label: 'Công thức', field: 'expression', align: 'left' },
    { name: 'isActive', label: 'Trạng thái', field: 'isActive', align: 'center' },
    { name: 'createdAt', label: 'Ngày tạo', field: 'createdAt', align: 'left' },
    { name: 'actions', label: 'Hành động', field: 'actions', align: 'center' },
  ];
  
  const fetchFormulas = async () => {
    try {
      const res = await api.get('/api/v1/salary-formula', {
        params: { type: selectedType.value },
      });
      formulas.value = res.data;
  
      const active = res.data.find(f => f.isActive);
      activeFormula.value = active ? active.expression : '';
    } catch (err) {
      $q.notify({ type: 'negative', message: 'Lỗi khi tải công thức' });
    }
  };
  
  const createFormula = async () => {
    if (!newFormula.value.expression) {
      $q.notify({ type: 'warning', message: 'Vui lòng nhập công thức' });
      return;
    }
  
    try {
      await api.post('/api/v1/salary-formula', {
        type: selectedType.value,
        expression: newFormula.value.expression,
        createdBy: 'admin', // hoặc có thể lấy từ session user hiện tại
      });
  
      $q.notify({ type: 'positive', message: 'Thêm công thức thành công' });
      showDialog.value = false;
      newFormula.value.expression = '';
      fetchFormulas();
    } catch (err) {
      $q.notify({ type: 'negative', message: 'Thêm công thức thất bại' });
    }
  };
  
  const setActive = async (id) => {
  try {
    await api.put(`/api/v1/salary-formula/set-active/${id}`, null, {
      params: { type: selectedType.value },
    });

    $q.notify({ type: 'positive', message: 'Đã kích hoạt công thức' });
    fetchFormulas();
  } catch (err) {
    $q.notify({ type: 'negative', message: 'Không thể kích hoạt' });
  }
};

  
  onMounted(fetchFormulas);
  </script>
  
  <style scoped>
  .q-table {
    font-size: 14px;
  }
  </style>
  